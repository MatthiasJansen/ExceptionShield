using System;
using System.Collections.Generic;
using ExceptionShield.Exceptions;
using ExceptionShield.Installer;
using ExceptionShield.Policies;
using ExceptionShield.Rules;
using ExceptionShield.Strategies;

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
}