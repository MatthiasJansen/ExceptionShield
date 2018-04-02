#region headers

// Copyright (c) 2018 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

using System;
using System.Collections.Generic;
using ExceptionShield.Extensions;

#endregion

namespace ExceptionShield.Policies
{
    public class ExceptionPolicyGroup<TSrc> : IExceptionPolicyGroup
        where TSrc : Exception
    {
        private readonly IReadOnlyDictionary<string, IExceptionPolicy> policyDictionary;

        public ExceptionPolicyGroup(IReadOnlyDictionary<string, IExceptionPolicy> policyDictionary)
        {
            this.policyDictionary = policyDictionary;
        }

        public Type Handles => typeof(TSrc);

        public IExceptionPolicy PolicyByContextOrDefault(string context)
        {
            if (string.IsNullOrWhiteSpace(context))
            {
                context = Context.Default;
            }

            var policy = default(IExceptionPolicy);
            if (context != Context.Default)
            {
                policy = this.policyDictionary.GetValueByKeyOrDefault(context);
            }

            return policy ?? this.policyDictionary.GetValueByKeyOrDefault(Context.Default);
        }
    }
}