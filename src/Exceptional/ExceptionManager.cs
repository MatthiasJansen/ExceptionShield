#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

using System;
using System.Collections.Generic;
using ExceptionManager.Exceptions;
using ExceptionManager.Policies;
using ExceptionManager.Rules;
using ExceptionManager.Strategies;

#endregion

namespace ExceptionManager
{
    public static class Context
    {
        public const string Default = @"3E52F3D4-4898-4324-8360-81C1D07C79CE";
    }

    public class ExceptionManager
    {
        private readonly IUnconfiguredExceptionRule defaultRule;

        private readonly Dictionary<Type, ExceptionPolicyGroupBase> policyGroupDictionary =
            new Dictionary<Type, ExceptionPolicyGroupBase>();

        private readonly IPolicyMatchingStrategy strategy;

        public ExceptionManager(IUnconfiguredExceptionRule defaultRule = null, IPolicyMatchingStrategy strategy = null)
        {
            this.strategy = strategy ?? new DefaultPolicyMatchingStrategy();
            this.defaultRule = defaultRule ?? new PolicyMissingDefaultRule();
        }

        public Exception Handle<TSrc>(TSrc exception, string context = Context.Default)
            where TSrc : Exception
        {
            if (string.IsNullOrWhiteSpace(context))
                context = Context.Default;

            var policy = strategy.MatchPolicy(policyGroupDictionary, exception.GetType(), context);

            return policy != null
                ? policy.Handle(exception)
                : defaultRule.Apply(exception);
        }

        public void AddPolicyGroup<TSrc, TEnd>(ExceptionPolicyGroup<TSrc, TEnd> policyGroup)
            where TSrc : Exception
            where TEnd : Exception
        {
            if (policyGroupDictionary.ContainsKey(policyGroup.Handles))
                throw new ExceptionManagerConfigurationException();

            policyGroupDictionary.Add(policyGroup.Handles, policyGroup);
        }
    }
}