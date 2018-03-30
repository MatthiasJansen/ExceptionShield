using System;
using System.Collections.Generic;
using System.Text;
using ExceptionShield.Policies;
using ExceptionShield.Strategies;
using ExceptionShield.Test.Scaffolding;
using FluentAssertions;
using Xunit;

namespace ExceptionShield.Test.Strategies
{
    public class DefaultPolicyMatchingStrategyTests
    {
        [Fact]
        public void Should_FallbackToNextBaseType_IfNoExactPolicyIsAvailable_ForGivenContext()
        {
            var strategy = new DefaultPolicyMatchingStrategy();

            var handlerDict =
                new Dictionary<Type, Type>
                {
                    {
                        typeof(FruitException),
                        typeof(object)
                    }
                };

            var policyDict =
                new Dictionary<string, ExceptionPolicy<FruitException, BerlinException>>
                {
                    {
                        "bowl of fruits assembly",
                        new ExceptionPolicy<FruitException, BerlinException>(handlerDict)
                    }
                };

            var policyGroupDict =
                new Dictionary<Type, ExceptionPolicyGroupBase>
                {
                    {
                        typeof(FruitException),
                        new ExceptionPolicyGroup<FruitException, BerlinException>(policyDict)
                    }
                };
            var exceptionType = typeof(AppleException);
            var context = "bowl of fruits assembly";

            var matchedPolicy = strategy.MatchPolicy(policyGroupDict, exceptionType, context);

            matchedPolicy.Handles.Should().Be(typeof(FruitException));
            matchedPolicy.Returns.Should().Be(typeof(BerlinException));
        }
    }
}