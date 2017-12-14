#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

using System;
using System.Collections.Generic;
using System.Linq;
using ExceptionShield.Exceptions;
using ExceptionShield.Installer;
using ExceptionShield.Plugable.Resolver;
using ExceptionShield.Policies;
using ExceptionShield.Rules;
using ExceptionShield.Strategies;
using JetBrains.Annotations;

#endregion

namespace ExceptionShield
{
    public class ExceptionManagerConfiguration
    {
        private readonly Dictionary<Type, ExceptionPolicyGroupBase> PolicyGroupDictionary =
    new Dictionary<Type, ExceptionPolicyGroupBase>();

        private IUnconfiguredExceptionRule rule;
        public void SetUnconfiguredExceptionRule(IUnconfiguredExceptionRule rule)
        {
            this.rule = rule;
        }

        private IPolicyMatchingStrategy strategy;
        public void SetPolicyMatchingStrategy(IPolicyMatchingStrategy strategy)
        {
            this.strategy = strategy;
        }


        public void AddPolicyGroup(ExceptionPolicyGroupBase policyGroup)
        {
            if (policyGroup == null)
                throw new ArgumentNullException(nameof(policyGroup));
            if (this.PolicyGroupDictionary.ContainsKey(policyGroup.Handles))
                throw new ExceptionManagerConfigurationException();

            this.PolicyGroupDictionary.Add(policyGroup.Handles, policyGroup);
        }

        public void AddPolicyGroupFrom<TInstaller>()
            where TInstaller : IPolicyGroupInstaller, new ()
        {
            var installer = Activator.CreateInstance<TInstaller>();

            AddPolicyGroup(installer.Provide());
        }

        internal IUnconfiguredExceptionRule Rule => this.rule;
        internal IPolicyMatchingStrategy Strategy => this.strategy;
        internal IEnumerable<ExceptionPolicyGroupBase> Policies => this.PolicyGroupDictionary.Values;
    }

    public class ExceptionManager : IExceptionManager
    {
        [NotNull]
        private readonly IUnconfiguredExceptionRule defaultRule;
        [NotNull]
        private readonly IPolicyMatchingStrategy strategy;
        [NotNull]
        private readonly IReadOnlyDictionary<Type, ExceptionPolicyGroupBase> policyGroupDictionary;
        [NotNull]
        private readonly IExceptionalResolver resolver = new DefaultResolver();

        public ExceptionManager(ExceptionManagerConfiguration configuration) : 
            this(configuration.Policies, configuration.Rule, configuration.Strategy)
        {
            
        }

        public ExceptionManager(IEnumerable<ExceptionPolicyGroupBase> policyGroupDictionary, IUnconfiguredExceptionRule defaultRule = null, IPolicyMatchingStrategy strategy = null)
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

        public ExceptionManager(IReadOnlyDictionary<Type, ExceptionPolicyGroupBase> policyGroupDictionary, IUnconfiguredExceptionRule defaultRule = null, IPolicyMatchingStrategy strategy = null)
        {
            this.policyGroupDictionary = policyGroupDictionary;
            this.strategy = strategy ?? new DefaultPolicyMatchingStrategy();
            this.defaultRule = defaultRule ?? new PolicyMissingDefaultRule();
        }

        /// <inheritdoc />
        public void Handle<TSrc>(TSrc exception, string context = Context.Default)
            where TSrc : Exception
        {
            var result = HandleInner(exception, context);

            if (result != null)
                throw result;
        }

        private Exception HandleInner<TSrc>(TSrc exception, string context)
            where TSrc : Exception
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            if (string.IsNullOrWhiteSpace(context))
                context = Context.Default;

            var policy = this.strategy.MatchPolicy(this.policyGroupDictionary, exception.GetType(), context);
            return policy != null
                ? policy.Handle(this.resolver, exception)
                : this.defaultRule.Apply(exception);
        }
    }
}