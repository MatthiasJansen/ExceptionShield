#region headers

// Copyright (c) 2018 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

#endregion

#region imports

using System;
using System.Collections.Generic;
using ExceptionShield.Exceptions;
using ExceptionShield.Handlers;
using ExceptionShield.Plugable.Resolver;
using ExceptionShield.Policies;
using ExceptionShield.Terminators;
using ExceptionShield.Test.Scaffolding;
using FluentAssertions;
using Xunit;

#endregion

namespace ExceptionShield.Test
{
    public class ExceptionPolicyTests
    {
        private class FaultyExceptionHandler : IExceptionHandler
        {
            /// <inheritdoc />
            public Exception Handle(Exception src)
            {
                return null;
            }
        }

        [Fact]
        public void Assert_ThatCorrectTypeGuidsAreAvailable()
        {
            var policy = new ExceptionPolicy<AppleException, BananaException>(null);

            policy.Handles.Should().Be(typeof(AppleException));
            policy.Returns.Should().Be(typeof(BananaException));
        }


        [Fact]
        public void Should_ThrowConfigurationExceptionWhenInvalidTerminatorIsProvided()
        {
            Action ctor
                = () => new ExceptionPolicy<AppleException, BananaException>
                      (null, typeof(CityException));

            ctor.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Should_ThrowConfigurationExceptionWhenTerminatorMissmatchIsFound()
        {
            Action ctor
                = () => new ExceptionPolicy<AppleException, BananaException>
                      (null, typeof(VoidTerminator<CityException>));

            ctor.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Should_ThrowConfigurationExceptionWhenTerminatorMissmatchIsFound_2()
        {
            Action ctor
                = () => new ExceptionPolicy<AppleException, BananaException>
                      (null, typeof(VoidTerminator<FruitException>));

            ctor.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Should_ThrowWhenHandlerReturnsNull()
        {
            var handlerDefinitions
                = new Dictionary<Type, Type>
                  {
                      {typeof(AppleException), typeof(FaultyExceptionHandler)}
                  };

            var policy = new ExceptionPolicy<AppleException, BananaException>(handlerDefinitions, null);

            Action act = () => policy.Handle(new DefaultResolver(), new AppleException());

            act.Should().Throw<ExceptionManagerConfigurationException>();
        }
    }
}