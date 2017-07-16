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

    public class AccessViolationPolicyInstaller : PolicyGroupInstaller<AccessViolationException, Exception>
    {
        protected override CompletePolicyDefinition<AccessViolationException, Exception> Provide(
            DefaultPolicyDefinitionBuilderHead<AccessViolationException, Exception> builder)
        {
            return builder.StartAndComplete<Exception, ExceptionHandler<AccessViolationException, Exception>>();
        }

        protected override IEnumerable<CompletePolicyDefinition<AccessViolationException, Exception>> Provide(
            PolicyDefinitionBuilder<AccessViolationException, Exception> builder)
        {
            yield return builder
                .SetContext("sql")
                .StartAndComplete<Exception, ExceptionHandler<AccessViolationException, Exception>>();

            yield return builder
                .SetContext("wmi")
                .StartAndComplete<Exception, ExceptionHandler<AccessViolationException, Exception>>();
        }
    }

    public class Entry
    {
        public static void Main(string[] args)
        {
            var dummy = Activator.CreateInstance<DummyWP>();

            ExceptionManagerConfiguration.AddPolicyGroupFrom<AccessViolationPolicyInstaller>();

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