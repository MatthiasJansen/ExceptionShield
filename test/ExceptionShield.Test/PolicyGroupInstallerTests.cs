#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

#endregion

using System;
using ExceptionShield.Installer;
using ExceptionShield.Installer.Builder;
using NUnit.Framework;

namespace ExceptionShield.Test
{
    [TestFixture]
    public class PolicyGroupInstallerTests
    {
        internal class Handler1 : Handlers.ExceptionHandler<OutOfMemoryException, AccessViolationException>
        {
            /// <inheritdoc />
            public override AccessViolationException Handle(OutOfMemoryException src)
            {
                Console.WriteLine($"{this.GetType().Name}");
                return base.Handle(src);
            }
        }

        internal class Handler2 : Handlers.ExceptionHandler<AccessViolationException, AppDomainUnloadedException>
        {
            /// <inheritdoc />
            public override AppDomainUnloadedException Handle(AccessViolationException src)
            {
                Console.WriteLine($"{this.GetType().Name}");
                return base.Handle(src);
            }
        }

        internal class DummyPolicyGroupInstaller : PolicyGroupInstaller<OutOfMemoryException, AppDomainUnloadedException>
        {
            /// <inheritdoc />
            protected override CompletePolicyDefinition<OutOfMemoryException, AppDomainUnloadedException> Provide(
                DefaultPolicyDefinitionBuilderHead<OutOfMemoryException, AppDomainUnloadedException> builder)
            {
                return builder
                    .Start<AccessViolationException>(c => c.Set<Handler1>())
                    .ThenComplete(c => c.Set<Handler2>());
            }
        }

        [Test]
        public void T1()
        {
            var policyGroup = new DummyPolicyGroupInstaller().Provide();

            var manager = new ExceptionManager(new []{policyGroup});

            manager.Handle(new OutOfMemoryException());
        }
    }
}