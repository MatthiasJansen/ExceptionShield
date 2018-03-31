#region headers

// Copyright (c) 2018 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

using System;
using System.Collections.Generic;
using ExceptionShield.Installer.Builder;
using ExceptionShield.Plugable.Resolver;
using ExceptionShield.Terminators;
using ExceptionShield.Test.Scaffolding;
using FluentAssertions;
using Xunit;

namespace ExceptionShield.Test.Installer.Builder
{
    public class PolicyBuilderTailTests
    {
        [Fact]
        public void WithTerminator_ShouldYieldTerminatedPolicyBuilder()
        {
            var context = "some_context";

            var tail = new PolicyBuilderTail<AppleException, PearException>
                (context, new Dictionary<Type, Type>
                          {
                              {
                                  typeof(AppleException),
                                  typeof(FruitGeneralizationExceptionHandler)
                              }
                          });

            var policy = tail.WithTerminator<VoidTerminator<PearException>>();

            var input = new AppleException();

            policy.Context.Should().Be(context);
            policy.CreatePolicy().Handle(new DefaultResolver(), input).Should().BeNull();
        }        
        
        [Fact]
        public void WithoutTerminator_ShouldYieldUnterminatedPolicyBuilder()
        {
            var context = "some_context";

            var tail = new PolicyBuilderTail<AppleException, PearException>
                (context, new Dictionary<Type, Type>
                          {
                              {
                                  typeof(AppleException),
                                  typeof(FruitGeneralizationExceptionHandler)
                              }
                          });

            var policy = tail.WithoutTerminator();

            var input = new AppleException();

            policy.Context.Should().Be(context);
            policy.CreatePolicy().Handle(new DefaultResolver(), input).Should().BeOfType<FruitException>();
        }
    }
}