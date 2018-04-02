#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ExceptionShield.Policies;
using FluentAssertions;
using Xunit;

namespace ExceptionShield.Test
{
    
    public class ExceptionPolicyGroupTests
    {
        [Fact]
        public void Handles_ShouldBeTheTypeOfTheSrcException()
        {
            var epg = new ExceptionPolicyGroup<ArgumentNullException>(
                new ReadOnlyDictionary<string, IExceptionPolicy>(
                    new Dictionary<string, IExceptionPolicy>()));

            epg.Handles.Should().Be(typeof(ArgumentNullException));
        }

        [Fact]
        public void PolicyByContextOrDefault_DoesNotThrow_WhenContextIsNull()
        {
            var epg = new ExceptionPolicyGroup<ArgumentNullException>(
                new ReadOnlyDictionary<string, IExceptionPolicy>(
                    new Dictionary<string, IExceptionPolicy>()));

            var ctx = epg.PolicyByContextOrDefault(null);
        }
    }
}