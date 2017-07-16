#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Exceptional.Handlers;
using Exceptional.Installer.Builder;
using Exceptional.Policies;
using JetBrains.Annotations;

#endregion

namespace Exceptional.Installer
{
    public interface IPolicyGroupInstaller
    {
        ExceptionPolicyGroupBase Provide();
    }

    public abstract class PolicyGroupInstaller<TSrc, TDst> : IPolicyGroupInstaller where TSrc : Exception
        where TDst : Exception
    {
        public ExceptionPolicyGroupBase Provide()
        {
            var hBuilder = new DefaultPolicyDefinitionBuilderHead<TSrc, TDst>();
            var nBuilder = new PolicyDefinitionBuilder<TSrc, TDst>();

            var defaultPolicy = Provide(hBuilder);
            var policyDict = new Dictionary<string, ExceptionPolicy<TSrc, TDst>>
            {
                {defaultPolicy.Context, defaultPolicy.Policy}
            };

            var policies = Provide(nBuilder) ?? Enumerable.Empty<CompletePolicyDefinition<TSrc, TDst>>();
            foreach (var policy in policies)
            {
                policyDict.Add(policy.Context, policy.Policy);
            }

            return
                new ExceptionPolicyGroup<TSrc, TDst>(
                    new ReadOnlyDictionary<string, ExceptionPolicy<TSrc, TDst>>(policyDict));
        }

        protected abstract CompletePolicyDefinition<TSrc, TDst> Provide(
            DefaultPolicyDefinitionBuilderHead<TSrc, TDst> builder);

        protected virtual IEnumerable<CompletePolicyDefinition<TSrc, TDst>> Provide(
            PolicyDefinitionBuilder<TSrc, TDst> builder)
        {
            yield break;
        }
    }
}