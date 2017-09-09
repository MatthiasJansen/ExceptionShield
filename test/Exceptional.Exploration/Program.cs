
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Exceptional.Handlers;
using Exceptional.Installer;
using Exceptional.Installer.Builder;
using Exceptional.Policies;

namespace Exceptional.Exploration
{
    public class DummyWP
    {
        private readonly string text;

        public DummyWP(string text)
        {
            this.text = text;
        }
    }

    public class FileNotFoundPolicyInstaller : PolicyGroupInstaller<FileNotFoundException, Exception>
    {
        /// <inheritdoc />
        protected override CompletePolicyDefinition<FileNotFoundException, Exception> Provide(DefaultPolicyDefinitionBuilderHead<FileNotFoundException, Exception> builder)
        {
            return builder.StartAndComplete<Exception, ExceptionHandler<FileNotFoundException, Exception>>();
        }
    }

    public class AccessViolationPolicyInstaller : PolicyGroupInstaller<FieldAccessException, Exception>
    {
        protected override CompletePolicyDefinition<FieldAccessException, Exception> Provide(
            DefaultPolicyDefinitionBuilderHead<FieldAccessException, Exception> builder)
        {
            return builder.StartAndComplete<Exception, ExceptionHandler<FieldAccessException, Exception>>();
        }

        protected override IEnumerable<CompletePolicyDefinition<FieldAccessException, Exception>> Provide(
            PolicyDefinitionBuilder<FieldAccessException, Exception> builder)
        {
            yield return builder
                .SetContext("sql")
                .StartAndComplete<Exception, ExceptionHandler<FieldAccessException, Exception>>();

            yield return builder
                .SetContext("wmi")
                .StartAndComplete<Exception, ExceptionHandler<FieldAccessException, Exception>>();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ExceptionManagerConfiguration.AddPolicyGroupFrom<AccessViolationPolicyInstaller>();
            ExceptionManagerConfiguration.AddPolicyGroupFrom<FileNotFoundPolicyInstaller>();

            var manager = ExceptionManagerConfiguration.LockAndCreateManager();

            try
            {
                File.Open("d05c746801b147e281ef52e6dfbfc781", FileMode.Open);
            }
            catch (Exception e)
            {
                manager.Handle(e);
            }

        }
    }
}