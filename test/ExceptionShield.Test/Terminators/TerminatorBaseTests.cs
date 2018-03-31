#region headers

// Copyright (c) 2018 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

using ExceptionShield.Terminators;
using ExceptionShield.Test.Scaffolding;
using FluentAssertions;
using Xunit;

#endregion

namespace ExceptionShield.Test.Terminators
{
    public class TerminatorBaseTests
    {
        private class FaultyTerminator : TerminatorBase<AppleException>
        {
            /// <inheritdoc />
            protected override void TerminateInner(AppleException exception)
            {
                throw new CityException();
            }
        }

        [Fact]
        public void Terminate_ShouldNeverThrow()
        {
            var terminator = new FaultyTerminator();

            terminator.Invoking(_ => _.Terminate(new AppleException())).Should().NotThrow();
        }
    }
}