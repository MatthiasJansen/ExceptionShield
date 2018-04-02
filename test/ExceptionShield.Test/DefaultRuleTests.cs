#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

#endregion

using System;
using ExceptionShield.Installer.Builder;
using ExceptionShield.Rules;
using ExceptionShield.Test.Scaffolding;
using FluentAssertions;
using Moq;
using Xunit;

namespace ExceptionShield.Test
{
    public class DefaultRuleTests
    {
        [Fact]
        public void HonorsDefaultRuleOverrideWhenNoPolicyDefined()
        {
            var defaultRuleMock = new Mock<IUnconfiguredExceptionRule>();
            defaultRuleMock
                .Setup(_ => _.Apply(It.IsAny<Exception>()))
                .Throws<PolicyMissingException>()
                ;

            var policyGroup = PolicyGroupBuilder
                .Create<AppleException>
                    (
                     d => d.SetTargetForDefaultContext<AppleException>()
                           .StartAndComplete(c => c.Set<ExceptionManagerTests.TestExceptionHandler<AppleException>>())
                           .WithoutTerminator()
                    );

            var manager = new ExceptionManager(new[] {policyGroup}, defaultRuleMock.Object);

            manager.Invoking(m => m.Handle(new OutOfMemoryException()))
                   .Should().ThrowExactly<PolicyMissingException>();

            defaultRuleMock.Verify();
        }
    }
}