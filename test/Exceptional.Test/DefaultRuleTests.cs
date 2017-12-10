#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

using System;
using Exceptional.Installer.Builder;
using Exceptional.Rules;
using Moq;
using NUnit.Framework;
using static Exceptional.Test.ExceptionManagerTests;

#endregion

namespace Exceptional.Test
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
                .Create<DummyException, DummyException>
                (
                    d => d.StartAndComplete(c => c.Set<TestExceptionHandler<DummyException>>())
                );

            var manager = new ExceptionManager(new []{policyGroup}, defaultRuleMock.Object);

            Assert.That(() => manager.Handle(new OutOfMemoryException()), Throws.TypeOf<PolicyMissingException>());

            defaultRuleMock.Verify();
        }
    }
}