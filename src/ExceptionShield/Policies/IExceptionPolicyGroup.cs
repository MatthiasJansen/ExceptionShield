#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

#endregion

using System;

namespace ExceptionShield.Policies
{
    public interface IExceptionPolicyGroup
    {
        Type Handles { get; }
        IExceptionPolicy PolicyByContextOrDefault(string context);
    }
}