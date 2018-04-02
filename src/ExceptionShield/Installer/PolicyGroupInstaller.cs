#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ExceptionShield.Installer.Builder;
using ExceptionShield.Policies;
using ExceptionShield.Terminators;

#endregion

namespace ExceptionShield.Installer
{
    public abstract class PolicyGroupInstaller<TSrc, TDst> : IPolicyGroupInstaller where TSrc : Exception
        where TDst : Exception
    {
        public IExceptionPolicyGroup Provide()
        {
            var hBuilder = new DefaultPolicyDefinitionBuilderProxy<TSrc>();
            var nBuilder = new RegularPolicyDefinitionBuilderProxy<TSrc>();

            var defaultPolicyDefinition = Provide(hBuilder);

            var defaultPolicy = defaultPolicyDefinition.CreatePolicy();

            var policyDict = new Dictionary<string, IExceptionPolicy>
            {
                {defaultPolicyDefinition.Context, defaultPolicy}
            };

            var regularPolicyDefinitions = Provide(nBuilder) ?? Enumerable.Empty<CompletePolicyDefinition<TSrc, TDst>>();
            foreach (var regularPolicyDefinition in regularPolicyDefinitions)
            {
                var regularPolicy = regularPolicyDefinition.CreatePolicy();
                policyDict.Add(regularPolicyDefinition.Context, regularPolicy);
            }

            return
                new ExceptionPolicyGroup<TSrc>(
                    new ReadOnlyDictionary<string, IExceptionPolicy>(policyDict));
        }

        protected abstract CompletePolicyDefinition<TSrc, TDst> 
            Provide(DefaultPolicyDefinitionBuilderProxy<TSrc> builder);

        protected abstract IEnumerable<CompletePolicyDefinition<TSrc, TDst>>
            Provide(RegularPolicyDefinitionBuilderProxy<TSrc> builderProxy);
    }
}