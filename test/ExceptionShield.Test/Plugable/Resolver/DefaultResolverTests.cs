#region headers

// Copyright (c) 2018 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

using ExceptionShield.Plugable.Resolver;
using ExceptionShield.Test.Scaffolding;
using FluentAssertions;
using Xunit;

#endregion

namespace ExceptionShield.Test.Plugable.Resolver
{
    public class DefaultResolverTests
    {
        [Fact]
        public void Resolve_ShouldCreateInstanceByGenericOfType()
        {
            var resolver = new DefaultResolver();
            var instance = resolver.Resolve<AppleException>();

            instance.Should().NotBeNull();
            instance.Should().BeOfType<AppleException>();
        }

        [Fact]
        public void Resolve_ShouldCreateInstanceByTypeOfType()
        {
            var resolver = new DefaultResolver();
            var instance = resolver.Resolve(typeof(AppleException));

            instance.Should().NotBeNull();
            instance.Should().BeOfType<AppleException>();
        }
    }
}