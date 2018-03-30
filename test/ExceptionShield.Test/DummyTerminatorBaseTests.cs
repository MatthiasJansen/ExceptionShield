#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

#endregion

using System;
using ExceptionShield.Terminators;
using FluentAssertions;
using Xunit;

namespace ExceptionShield.Test
{
    
    public class DummyTerminatorBaseTests
    {
        internal class SpecializedDummyTerminator : TerminatorBase<Exception>
        {
            protected override void TerminateInner(Exception exception)
            {
                
            }
        }

        [Fact]
        public void Terminate_ShouldNeverThrow()
        {
            var terminator = new SpecializedDummyTerminator();

            terminator.Invoking(t => t.Terminate(new Exception())).Should().NotThrow();
            terminator.Invoking(t => t.Terminate(null)).Should().NotThrow();
        }
    }
}