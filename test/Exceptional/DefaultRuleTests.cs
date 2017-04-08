#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

using System;
using ExceptionManager.Builder;
using ExceptionManager.Rules;
using NUnit.Framework;
using Rhino.Mocks;

#endregion

namespace ExceptionManagerTests
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
                .Return(new OutOfMemoryException());
            var manager = new ExceptionManager.ExceptionManager(defaultRule);

            var handlerT01 = new ExceptionManagerTests.TestExceptionHandler<ExceptionManagerTests.DummyException>();

            var policyGroup = PolicyGroupBuilder
                .Create<ExceptionManagerTests.DummyException, ExceptionManagerTests.DummyException>
                (d => d.StartAndComplete(handlerT01)
                );


            manager.AddPolicyGroup(policyGroup);

            var exception = manager.Handle(new OutOfMemoryException());

            Assert.IsInstanceOf<OutOfMemoryException>(exception);
            defaultRule.AssertWasCalled(r => r.Apply(Arg<Exception>.Is.Anything));
        }
    }
}