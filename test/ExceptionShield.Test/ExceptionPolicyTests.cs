#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

#endregion

using System;
using ExceptionShield.Policies;
using FluentAssertions;
using Xunit;

namespace ExceptionShield.Test
{
    
    public class ExceptionPolicyTests
    {
        [Fact]
        public void Assert_ThatCorrectTypeGuidsAreAvailable()
        {
            var policy = new ExceptionPolicy<TimeoutException, NotSupportedException>(null);

            policy.Handles.Should().Be(typeof(TimeoutException));
            policy.Returns.Should().Be(typeof(NotSupportedException));
        }
    }
}