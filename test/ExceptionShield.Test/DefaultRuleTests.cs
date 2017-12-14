#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

#endregion

using System;
using ExceptionShield.Installer.Builder;
using ExceptionShield.Rules;
using Moq;
using NUnit.Framework;

namespace ExceptionShield.Test
{
    [TestFixture]
    public class DefaultRuleTests
    {
        [Test]
        public void HonorsDefaultRuleOverrideWhenNoPolicyDefined()
        {
            var defaultRuleMock = new Mock<IUnconfiguredExceptionRule>();
            defaultRuleMock
                .Setup(_ => _.Apply(It.IsAny<Exception>()))
                .Throws<PolicyMissingException>()
                ;

            var policyGroup = PolicyGroupBuilder
                .Create<ExceptionManagerTests.DummyException, ExceptionManagerTests.DummyException>
                (
                    d => d.StartAndComplete(c => c.Set<ExceptionManagerTests.TestExceptionHandler<ExceptionManagerTests.DummyException>>())
                );

            var manager = new ExceptionManager(new []{policyGroup}, defaultRuleMock.Object);

            Assert.That(() => manager.Handle(new OutOfMemoryException()), Throws.TypeOf<PolicyMissingException>());

            defaultRuleMock.Verify();
        }
    }
}