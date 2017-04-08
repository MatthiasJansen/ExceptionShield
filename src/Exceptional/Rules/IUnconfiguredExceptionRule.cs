#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

using System;

#endregion

namespace ExceptionManager.Rules
{
    public interface IUnconfiguredExceptionRule
    {
        Exception Apply(Exception exception);
    }
}