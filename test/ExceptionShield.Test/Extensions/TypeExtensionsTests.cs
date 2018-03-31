using System;
using System.Collections.Generic;
using System.Text;
using ExceptionShield.Extensions;
using ExceptionShield.Test.Scaffolding;
using FluentAssertions;
using Xunit;
using TypeExtensions = ExceptionShield.Extensions.TypeExtensions;

namespace ExceptionShield.Test.Extensions
{
    public class TypeExtensionsTests
    {
        [Theory]
        [InlineData(typeof(AppleException), typeof(FruitException), true)]
        [InlineData(typeof(AppleException), typeof(CityException), false)]
        public void ShouldDetermineSubtype(Type c, Type b, bool expected)
        {
            TypeExtensions.IsSubclassOfOpenGeneric(c, b).Should().Be(expected);
        }

        [Fact]
        public void ShouldThrow_WhenCurrentIsNull()
        {
            Action act = () => TypeExtensions.IsSubclassOfOpenGeneric(null, typeof(AppleException));

            act.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("current");
        }        
        
        [Fact]
        public void ShouldThrow_WhenBaseTypeIsNull()
        {
            Action act = () => TypeExtensions.IsSubclassOfOpenGeneric(typeof(AppleException), null);

            act.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("baseType");
        }
    }
}
