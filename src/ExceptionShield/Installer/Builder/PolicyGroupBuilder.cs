#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ExceptionShield.Policies;

#endregion

namespace ExceptionShield.Installer.Builder
{
    public class PolicyGroupBuilder
    {
        public static ExceptionPolicyGroup<TSrc, TEnd> Create<TSrc, TEnd>(
            Func<DefaultPolicyDefinitionBuilderHead<TSrc, TEnd>, CompletePolicyDefinition<TSrc, TEnd>>
                defaultPolicyDefinitionCreator,
            params Func<PolicyDefinitionBuilder<TSrc, TEnd>, CompletePolicyDefinition<TSrc, TEnd>>[]
                policyDefinitionCreators)
            where TSrc : Exception
            where TEnd : Exception
        {
            var policyDict = new Dictionary<string, ExceptionPolicy<TSrc, TEnd>>();

            var dpdb = new DefaultPolicyDefinitionBuilderHead<TSrc, TEnd>();
            var cdpd = defaultPolicyDefinitionCreator.Invoke(dpdb);

            policyDict.Add(cdpd.Context, cdpd.Policy);

            foreach (var sdc in policyDefinitionCreators)
            {
                var pdb = new PolicyDefinitionBuilder<TSrc, TEnd>();
                var cpd = sdc.Invoke(pdb);
                policyDict.Add(cpd.Context, cpd.Policy);
            }

            return
                new ExceptionPolicyGroup<TSrc, TEnd>(
                    new ReadOnlyDictionary<string, ExceptionPolicy<TSrc, TEnd>>(policyDict));
        }
    }
}