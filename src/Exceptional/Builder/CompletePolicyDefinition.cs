#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

using System;
using Exceptional.Policies;

#endregion

namespace Exceptional.Builder
{
    public class CompletePolicyDefinition<TSrc, TEnd>
        where TSrc : Exception
        where TEnd : Exception
    {
        public CompletePolicyDefinition(string context, ExceptionPolicy<TSrc, TEnd> policy)
        {
            Policy = policy;
            Context = context;
        }

        public string Context { get; }

        public ExceptionPolicy<TSrc, TEnd> Policy { get; }
    }
}