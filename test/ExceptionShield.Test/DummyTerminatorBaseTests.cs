#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

#endregion

using System;
using ExceptionShield.Terminators;
using NUnit.Framework;

namespace ExceptionShield.Test
{
    [TestFixture]
    public class DummyTerminatorBaseTests
    {
        internal class SpecializedDummyTerminator : TerminatorBase<Exception>
        {
            protected override void TerminateInner(Exception exception)
            {
                
            }
        }

        [Test]
        public void Terminate_ShouldNeverThrow()
        {
            var terminator = new SpecializedDummyTerminator();

            Assert.That(() => terminator.Terminate(new Exception()), Throws.Nothing);
            Assert.That(() => terminator.Terminate(null), Throws.Nothing);
        }
    }
}