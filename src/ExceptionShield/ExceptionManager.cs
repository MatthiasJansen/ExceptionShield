#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

using System;
using System.Collections.Generic;
using System.Linq;
using ExceptionShield.Exceptions;
using ExceptionShield.Plugable.Resolver;
using ExceptionShield.Policies;
using ExceptionShield.Rules;
using ExceptionShield.Strategies;
using JetBrains.Annotations;

#endregion

namespace ExceptionShield
{
    public class ExceptionManager : IExceptionManager
    {
        [NotNull] private readonly IUnconfiguredExceptionRule defaultRule;
        [NotNull] private readonly IPolicyMatchingStrategy strategy;
        [NotNull] private readonly IReadOnlyDictionary<Type, ExceptionPolicyGroupBase> policyGroupDictionary;
        [NotNull] private readonly IExceptionalResolver resolver = new DefaultResolver();

        public ExceptionManager(IEnumerable<ExceptionPolicyGroupBase> policyGroupDictionary,
                                IUnconfiguredExceptionRule defaultRule = null, IPolicyMatchingStrategy strategy = null)
        {
            try
            {
                this.policyGroupDictionary = policyGroupDictionary.ToDictionary(i => i.Handles, i => i);
            }
            catch (ArgumentException)
            {
                throw new ExceptionManagerConfigurationException();
            }

            this.strategy = strategy ?? new DefaultPolicyMatchingStrategy();
            this.defaultRule = defaultRule ?? new PolicyMissingDefaultRule();
        }

        /// <inheritdoc />
        public void Handle<TSrc>(TSrc exception, string context = Context.Default)
            where TSrc : Exception
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            if (string.IsNullOrWhiteSpace(context))
            {
                context = Context.Default;
            }

            var result = HandleInner(exception, context);

            if (result != null)
            {
                throw result;
            }
        }

        private Exception HandleInner<TSrc>(TSrc exception, string context)
            where TSrc : Exception
        {
            var policy = this.strategy.MatchPolicy(this.policyGroupDictionary, exception.GetType(), context);
            return policy != null
                       ? policy.Handle(this.resolver, exception)
                       : this.defaultRule.Apply(exception);
        }
    }
}