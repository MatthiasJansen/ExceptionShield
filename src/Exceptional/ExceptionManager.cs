#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

using System;
using System.Collections.Generic;
using System.Linq;
using Exceptional.Exceptions;
using Exceptional.Installer;
using Exceptional.Plugable.Resolver;
using Exceptional.Policies;
using Exceptional.Rules;
using Exceptional.Strategies;
using JetBrains.Annotations;

#endregion

namespace Exceptional
{
    public static class ExceptionManagerConfiguration
    {
        private static bool isLockedAgainstChanges = false;

        private static IUnconfiguredExceptionRule rule;
        public static void SetUnconfiguredExceptionRule(IUnconfiguredExceptionRule rule)
        {
            if (isLockedAgainstChanges) { return;}

            ExceptionManagerConfiguration.rule = rule;
        }

        private static IPolicyMatchingStrategy strategy;
        public static void SetPolicyMatchingStrategy(IPolicyMatchingStrategy strategy)
        {
            if (isLockedAgainstChanges) { return; }
            ExceptionManagerConfiguration.strategy = strategy;
        }

        private static readonly Dictionary<Type, ExceptionPolicyGroupBase> policyGroupDictionary =
            new Dictionary<Type, ExceptionPolicyGroupBase>();
        public static void AddPolicyGroup(ExceptionPolicyGroupBase policyGroup)
        {
            if (isLockedAgainstChanges) { return; }
            if (policyGroup == null)
                throw new ArgumentNullException(nameof(policyGroup));
            if (policyGroupDictionary.ContainsKey(policyGroup.Handles))
                throw new ExceptionManagerConfigurationException();

            policyGroupDictionary.Add(policyGroup.Handles, policyGroup);
        }

        public static void AddPolicyGroupFrom<TInstaller>()
            where TInstaller : IPolicyGroupInstaller, new ()
        {
            var installer = Activator.CreateInstance<TInstaller>();

            AddPolicyGroup(installer.Provide());
        }

        public static IExceptionManager LockAndCreateManager()
        {
            isLockedAgainstChanges = true;
            return new ExceptionManager(policyGroupDictionary, rule, strategy);
        }
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

        public ExceptionManager(IEnumerable<ExceptionPolicyGroupBase> policyGroupDictionary, IUnconfiguredExceptionRule defaultRule = null, IPolicyMatchingStrategy strategy = null)
        {
            this.policyGroupDictionary = policyGroupDictionary.ToDictionary(i => i.Handles, i => i);
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

            var policy = strategy.MatchPolicy(policyGroupDictionary, exception.GetType(), context);
            return policy != null
                ? policy.Handle(resolver, exception)
                : defaultRule.Apply(exception);
        }
    }
}