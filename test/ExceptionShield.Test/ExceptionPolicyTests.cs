#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

#endregion

using System;
using ExceptionShield.Policies;
using ExceptionShield.Terminators;
using ExceptionShield.Test.Scaffolding;
using FluentAssertions;
using Xunit;

namespace ExceptionShield.Test
{
    public class ExceptionPolicyTests
    {
        [Fact]
        public void Assert_ThatCorrectTypeGuidsAreAvailable()
        {
            var policy = new ExceptionPolicy<AppleException, BananaException>(null);

            policy.Handles.Should().Be(typeof(AppleException));
            policy.Returns.Should().Be(typeof(BananaException));
        }

        [Fact]
        public void Should_ThrowConfigurationExceptionWhenTerminatorMissmatchIsFound()
        {
            Action ctor
                = () => new ExceptionPolicy<AppleException, BananaException>(null, typeof(VoidTerminator<CityException>));

            ctor.Should().Throw<ArgumentException>();
        }        
        
        [Fact]
        public void Should_ThrowConfigurationExceptionWhenTerminatorMissmatchIsFound_2()
        {
            Action ctor
                = () => new ExceptionPolicy<AppleException, BananaException>(null, typeof(VoidTerminator<FruitException>));

            ctor.Should().Throw<ArgumentException>();
        }
    }
}