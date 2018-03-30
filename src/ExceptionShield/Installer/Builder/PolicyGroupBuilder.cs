#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ExceptionShield.Policies;

#endregion

namespace ExceptionShield.Installer.Builder
{
    public class PolicyGroupBuilder
    {
        public static ExceptionPolicyGroup<TSrc, TDst> Create<TSrc, TDst>(
            DefaultCreator<TSrc, TDst> defaultCreator,
            params RegularCreator<TSrc, TDst>[] regularCreators)
            where TSrc : Exception
            where TDst : Exception
        {
            var policyDict = new Dictionary<string, ExceptionPolicy<TSrc, TDst>>();

            var defaultPolicyDefinitionBuilderHead = new DefaultPolicyDefinitionBuilder<TSrc, TDst>();
            var defaultPolicyDefinition = defaultCreator.Invoke(defaultPolicyDefinitionBuilderHead);

            var defaultPolicy = defaultPolicyDefinition.CreatePolicy();

            policyDict.Add(defaultPolicyDefinition.Context, defaultPolicy);

            foreach (var regularCreator in regularCreators)
            {
                var pdb = new RegularPolicyDefinitionBuilderProxy<TSrc, TDst>();
                var regularPolicyDefinition = regularCreator.Invoke(pdb);
                var regularPolicy = regularPolicyDefinition.CreatePolicy();
                policyDict.Add(regularPolicyDefinition.Context, regularPolicy);
            }

            return
                new ExceptionPolicyGroup<TSrc, TDst>
                    (new ReadOnlyDictionary<string, ExceptionPolicy<TSrc, TDst>>(policyDict));
        }
    }
}