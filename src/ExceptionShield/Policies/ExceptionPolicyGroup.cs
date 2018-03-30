#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ExceptionShield.Extensions;

namespace ExceptionShield.Policies
{
    public class ExceptionPolicyGroup<TSrc, TEnd> : ExceptionPolicyGroupBase
        where TEnd : Exception where TSrc : Exception
    {
        private readonly IReadOnlyDictionary<string, ExceptionPolicy<TSrc, TEnd>> policyDictionary;

        public ExceptionPolicyGroup(IReadOnlyDictionary<string, ExceptionPolicy<TSrc, TEnd>> policyDictionary)
        {
            this.policyDictionary = policyDictionary;
        }

        public override Type Handles => typeof(TSrc);
        public override Type Returns => typeof(TEnd);

        public override ExceptionPolicyBase PolicyByContextOrDefault(string context)
        {
            if (string.IsNullOrWhiteSpace(context))
                context = Context.Default;

            var policy = default(ExceptionPolicy<TSrc, TEnd>);
            if (context != Context.Default)
                policy = this.policyDictionary.GetValueByKeyOrDefault(context);

            return policy ?? this.policyDictionary.GetValueByKeyOrDefault(Context.Default);
        }
    }
}