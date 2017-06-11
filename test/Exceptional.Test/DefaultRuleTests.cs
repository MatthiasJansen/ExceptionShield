#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

using System;
using Exceptional.Installer.Builder;
using Exceptional.Rules;
using NUnit.Framework;
using Rhino.Mocks;
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
            var defaultRule = MockRepository.GenerateMock<IUnconfiguredExceptionRule>();
            defaultRule.Expect(r => r.Apply(Arg<Exception>.Is.Anything))
                .Repeat.Once()
                .Return(new PolicyMissingException());

            var handlerT01 = new TestExceptionHandler<DummyException>();

            var policyGroup = PolicyGroupBuilder
                .Create<DummyException, DummyException>
                (d => d.StartAndComplete(handlerT01)
                );

            ExceptionManagerConfiguration.SetUnconfiguredExceptionRule(defaultRule);

            ExceptionManagerConfiguration.AddPolicyGroup(policyGroup);
            var manager = ExceptionManagerConfiguration.LockAndCreateManager();
            
            Assert.That(() => manager.Handle(new OutOfMemoryException()), Throws.TypeOf<PolicyMissingException>());

            defaultRule.AssertWasCalled(r => r.Apply(Arg<Exception>.Is.Anything));
        }
    }
}