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
        public static ExceptionPolicyGroup<TSrc> Create<TSrc>(
                   DefaultCreator<TSrc> defaultCreator,
            params RegularCreator<TSrc>[] regularCreators)
            where TSrc : Exception
        {
            var policyDict = new Dictionary<string, IExceptionPolicy>();

            var defaultPolicyDefinitionBuilderHead = new DefaultPolicyDefinitionBuilderProxy<TSrc>();
            var defaultPolicyDefinition = defaultCreator.Invoke(defaultPolicyDefinitionBuilderHead);

            var defaultPolicy = defaultPolicyDefinition.CreatePolicy();

            policyDict.Add(defaultPolicyDefinition.Context, defaultPolicy);

            foreach (var regularCreator in regularCreators)
            {
                var pdb = new RegularPolicyDefinitionBuilderProxy<TSrc>();
                var regularPolicyDefinition = regularCreator.Invoke(pdb);
                var regularPolicy = regularPolicyDefinition.CreatePolicy();
                policyDict.Add(regularPolicyDefinition.Context, regularPolicy);
            }

            return
                new ExceptionPolicyGroup<TSrc>
                    (new ReadOnlyDictionary<string, IExceptionPolicy>(policyDict));
        }
    }
}