#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

using System;
using System.Collections.Generic;
using ExceptionShield.Policies;
using ExceptionShield.Terminators;

#endregion

namespace ExceptionShield.Installer.Builder
{
    public class CompletePolicyDefinition<TSrc, TDst>
        where TSrc : Exception
        where TDst : Exception
    {
        private readonly IReadOnlyDictionary<Type, Type> policyDict;
        private readonly string context;
        private readonly Type terminator;

        public CompletePolicyDefinition(string context, IReadOnlyDictionary<Type, Type> policyDict, Type terminator)
        {
            this.context = context;
            this.policyDict = policyDict;
            this.terminator = terminator;
        }

        public ExceptionPolicy<TSrc, TDst> CreatePolicy()
        {
            return new ExceptionPolicy<TSrc, TDst>(this.policyDict, this.terminator);
        }

        public string Context => this.context;
    }

    public class PolicyBuilderTail<TSrc, TEnd>
        where TSrc : Exception
        where TEnd : Exception
    {
        private readonly IReadOnlyDictionary<Type, Type> policy;
        private readonly string context;

        public PolicyBuilderTail(string context, IReadOnlyDictionary<Type,Type> policy)
        {
            this.policy = policy;
            this.context = context;
        }

        //public string Context { get; }

        //public IReadOnlyDictionary<Type, Type> Policy { get; }

        public CompletePolicyDefinition<TSrc,TEnd> WithTerminator<TTer>()
        where TTer : TerminatorBase<TEnd>
        {
            return new CompletePolicyDefinition<TSrc, TEnd>(this.context, this.policy, typeof(TTer));
        }

        public CompletePolicyDefinition<TSrc, TEnd> WithoutTerminator()
        {
            return new CompletePolicyDefinition<TSrc, TEnd>(this.context, this.policy, null);
        }
    }
}