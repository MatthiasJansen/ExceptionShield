#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

using System;

#endregion

namespace Exceptional.Policies
{
    public abstract class ExceptionPolicyBase
    {
        public abstract Type Handles { get; }
        public abstract Type Returns { get; }

        public abstract Exception Handle(Exception src);
    }
}