#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

using System;
using System.Collections.Generic;
using ExceptionShield.Policies;

#endregion

namespace ExceptionShield.Strategies
{
    public interface IPolicyMatchingStrategy
    {
        IExceptionPolicy MatchPolicy(IReadOnlyDictionary<Type, IExceptionPolicyGroup> policyDictionary, Type type, string context);
    }
}