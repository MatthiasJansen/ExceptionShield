#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

using System;
using ExceptionManager.Terminators;
using NUnit.Framework;

#endregion

namespace ExceptionManagerTests
{
    [TestFixture]
    public class DummyTerminatorBaseTests
    {
        internal class SpecializedDummyTerminator : DummyTerminatorBase<Exception>
        {
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