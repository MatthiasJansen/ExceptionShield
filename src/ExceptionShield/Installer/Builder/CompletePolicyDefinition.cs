#region headers

// Copyright (c) 2018 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

using System;
using System.Collections.Generic;
using ExceptionShield.Policies;

#endregion

namespace ExceptionShield.Installer.Builder
{
    public interface ICompletePolicyDefinition
    {
        string Context { get; }
        IExceptionPolicy CreatePolicy();
    }

    public class CompletePolicyDefinition<TSrc, TDst> : ICompletePolicyDefinition
        where TSrc : Exception
        where TDst : Exception
    {
        private readonly IReadOnlyDictionary<Type, Type> policyDict;
        private readonly Type terminator;

        public CompletePolicyDefinition(string context, IReadOnlyDictionary<Type, Type> policyDict, Type terminator)
        {
            Context = context;
            this.policyDict = policyDict;
            this.terminator = terminator;
        }

        public string Context { get; }

        public IExceptionPolicy CreatePolicy()
        {
            return new ExceptionPolicy<TSrc, TDst>(this.policyDict, this.terminator);
        }
    }
}