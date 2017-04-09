#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

using System;
using System.Collections.Generic;
using Exceptional.Exceptions;
using Exceptional.Policies;
using Exceptional.Rules;
using Exceptional.Strategies;

#endregion

namespace Exceptional
{
    public class ExceptionManager : IExceptionManager
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

        public void Handle<TSrc>(TSrc exception, string context = Context.Default)
            where TSrc : Exception
        {
            var result = HandleInner(exception, context);

            if (result != null)
                throw result;
        }

        public void AddPolicyGroup<TSrc, TEnd>(ExceptionPolicyGroup<TSrc, TEnd> policyGroup)
            where TSrc : Exception
            where TEnd : Exception
        {
            if (policyGroupDictionary.ContainsKey(policyGroup.Handles))
                throw new ExceptionManagerConfigurationException();

            policyGroupDictionary.Add(policyGroup.Handles, policyGroup);
        }

        private Exception HandleInner<TSrc>(TSrc exception, string context)
            where TSrc : Exception
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            if (string.IsNullOrWhiteSpace(context))
                context = Context.Default;

            var policy = strategy.MatchPolicy(policyGroupDictionary, exception.GetType(), context);
            return policy != null
                ? policy.Handle(exception)
                : defaultRule.Apply(exception);
        }
    }
}