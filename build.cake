// Target - The task you want to start. Runs the Default task if not specified.
var target = Argument("Target", "Default");
var codecovToken = Argument("CodeCovToken", "00000000-0000-0000-0000-000000000000");

// Configuration - The build configuration (Debug/Release) to use.
var configuration = 
    HasArgument("Configuration") 
        ? Argument<string>("Configuration") 
        : EnvironmentVariable("Configuration") ?? "Release";

// The build number to use in the version number of the built NuGet packages.
var versionInfo = default (Cake.Common.Tools.GitVersion.GitVersion);
// A directory path to an Artifacts directory.
var artifactsDirectory = MakeAbsolute(Directory("./artifacts"));
 
// Deletes the contents of the Artifacts folder if it should contain anything from a previous build.
Task("Clean").Does(() => CleanDirectory(artifactsDirectory));

#tool "nuget:?package=GitVersion.CommandLine"
Task("Version")
    .Does(() => {
        GitVersion(new GitVersionSettings{
            UpdateAssemblyInfo = true,
            OutputType = GitVersionOutput.BuildServer
        });
    versionInfo = GitVersion(new GitVersionSettings{ OutputType = GitVersionOutput.Json });
    Information("Version: " + versionInfo.NuGetVersion);
    });

Task("NuGetRestore")
    .Does(() => {
        DotNetCoreRestore("ExceptionShield.sln");
    });

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("NuGetRestore")
    .IsDependentOn("Version")
        .Does(() => 
        {
            var settings = new DotNetCoreBuildSettings
            {
                Configuration = configuration
            };

            DotNetCoreBuild("ExceptionShield.sln", settings);
        }
    );

// Run dotnet pack to produce NuGet packages from our projects. 
Task("Pack")
    .IsDependentOn("Build")
    .Does(() =>
    {
        var settings = new DotNetCorePackSettings()
            {
                Configuration = configuration,
                NoBuild = true,
                OutputDirectory = artifactsDirectory,
                IncludeSymbols = true,
                ArgumentCustomization = args => args
                    .Append($"/p:PackageVersion={versionInfo.NuGetVersion}")
            };
        foreach(var project in GetFiles("./src/**/*.csproj")){
            DotNetCorePack(project.ToString(), settings);
        }
    });

// Look under a 'test' folder and run dotnet test against all of those projects.
#addin "nuget:?package=Cake.Incubator"
Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
    {
        var projects = GetFiles("./test/**/*.csproj");
        foreach(var project in projects)
        {
            var settings = new DotNetCoreTestSettings
                {
                    Configuration = configuration,
                    NoBuild = true,
                };
            var xUnitSettings = new XUnit2Settings{
                        OutputDirectory = artifactsDirectory,
            };
            DotNetCoreTest(settings, project, xUnitSettings);
            }
    });

// Look under a 'test' folder, run them and get coverage.
#tool "nuget:?package=OpenCover"

#tool "nuget:?package=Codecov"
#addin "nuget:?package=Cake.Codecov"

#addin nuget:?package=Cake.FileHelpers&version=2.0.0

//#tool "nuget:?package=xunit.runner.console"
Task("Coverage")
    .IsDependentOn("Pack")
    .Does(() =>
    {
        var projects = GetFiles("./src/**/*.csproj");
        // foreach(var project in projects)
        // {
        //     TransformTextFile(project.FullPath, ">", "<")
        //         .WithToken("portable", ">full<")
        //         .Save(project.FullPath);
        // }

        projects = GetFiles("./test/**/*.csproj");
        var resultsFile = artifactsDirectory.CombineWithFilePath("coverage.xml");

        DeleteFile(resultsFile.FullPath);

        foreach(var project in projects)
        {
            OpenCover(
                x => x.DotNetCoreTest(
                     project.FullPath,
                     new DotNetCoreTestSettings() { Configuration = "Debug" }
                ),
                resultsFile,
                new OpenCoverSettings()
                {
                    // ArgumentCustomization = args => args
                    //     .Append("-threshold:100")
                    //     .Append("-returntargetcode")
                    //     .Append("-hideskipped:Filter;Attribute;MissingPdb"),
                    Register = "user",
                    OldStyle = true,
                    MergeOutput = true,
                    SkipAutoProps = true,
                }
        
        .WithFilter("+[ExceptionShield*]*")
        .WithFilter("-[*Test]*")
        .ExcludeByAttribute("System.CodeDom.Compiler.GeneratedCodeAttribute")
        .ExcludeByAttribute("*.ExcludeFromCodeCoverage*")
            );
        }
        
        if (codecovToken != "00000000-0000-0000-0000-000000000000")
        {
            ReplaceTextInFiles(resultsFile.FullPath, "\r\n", "\n");
            
            var coverageUploadSettings = new CodecovSettings {
                Files = new[] { resultsFile.FullPath },
                Token = codecovToken,
                Flags = "ut"
            };

            Codecov(coverageUploadSettings);
        }
    });

// The default task to run if none is explicitly specified.
Task("Default")
    .IsDependentOn("Test")
    .IsDependentOn("Coverage")
    ;
 
// Executes the task specified in the target argument.
RunTarget(target);
