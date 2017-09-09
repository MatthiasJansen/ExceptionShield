#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

using System;
using Exceptional.Policies;
using NUnit.Framework;

#endregion

namespace Exceptional.Test
{
    [TestFixture]
    public class ExceptionPolicyTests
    {
        [Test]
        public void Assert_ThatCorrectTypeGuidsAreAvailable()
        {
            var policy = new ExceptionPolicy<TimeoutException, NotSupportedException>(null);

            Assert.That(policy.Handles, Is.EqualTo(typeof(TimeoutException)));
            Assert.That(policy.Returns, Is.EqualTo(typeof(NotSupportedException)));
        }
    }
}