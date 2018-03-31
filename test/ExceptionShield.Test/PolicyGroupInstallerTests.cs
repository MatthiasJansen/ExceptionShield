#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

#endregion

using System;
using System.Collections.Generic;
using ExceptionShield.Handlers;
using ExceptionShield.Installer;
using ExceptionShield.Installer.Builder;
using ExceptionShield.Test.Scaffolding;
using FluentAssertions;
using Xunit;

namespace ExceptionShield.Test
{
    
    public class PolicyGroupInstallerTests
    {
        internal class Handler1 : Handlers.ExceptionHandler<AppleException, CherryException>
        {
            /// <inheritdoc />
            public override CherryException Handle(AppleException src)
            {
                Console.WriteLine($"{this.GetType().Name}");
                return base.Handle(src);
            }
        }

        internal class Handler2 : ExceptionHandler<CherryException, PearException>
        {
            /// <inheritdoc />
            public override PearException Handle(CherryException src)
            {
                Console.WriteLine($"{this.GetType().Name}");
                return base.Handle(src);
            }
        }

        internal class Handler3 : ExceptionHandler<AppleException, PearException>
        {
            /// <inheritdoc />
            public override PearException Handle(AppleException src)
            {
                return base.Handle(src);
            }
        }

        internal class DummyPolicyGroupInstaller : PolicyGroupInstaller<AppleException, PearException>
        {
            /// <inheritdoc />
            protected override CompletePolicyDefinition<AppleException, PearException> Provide(
                DefaultPolicyDefinitionBuilder<AppleException, PearException> builder)
            {
                return builder
                    .Start<CherryException>(c => c.Set<Handler1>())
                    .ThenComplete(c => c.Set<Handler2>())
                    .WithoutTerminator()  
                    ;
            }

            /// <inheritdoc />
            protected override IEnumerable<CompletePolicyDefinition<AppleException, PearException>> Provide(RegularPolicyDefinitionBuilderProxy<AppleException, PearException> builderProxy)
            {
                yield return builderProxy.SetContext("red wine")
                                         .StartAndComplete(c => c.Set<Handler3>())
                                         .WithoutTerminator();
            }
        }

        [Fact]
        public void T1()
        {
            var policyGroup = new DummyPolicyGroupInstaller().Provide();

            var manager = new ExceptionManager(new []{policyGroup});

            manager.Invoking(_ => _.Handle(new AppleException()))
                   .Should().ThrowExactly<PearException>();
        }
    }
}