#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

using System;
using System.Collections.Generic;
using System.Reflection;
using ExceptionShield.Extensions;
using ExceptionShield.Policies;

#endregion

namespace ExceptionShield.Strategies
{
    public class DefaultPolicyMatchingStrategy : IPolicyMatchingStrategy
    {
        public ExceptionPolicyBase MatchPolicy(IReadOnlyDictionary<Type, ExceptionPolicyGroupBase> policyDictionary,
            Type type, string context)
        {
            var currentType = type;
            while (currentType != null)
            {
                if (currentType == typeof(Exception))
                    break;

                var policyGroup = policyDictionary.GetValueByKeyOrDefault(currentType);
                if (policyGroup != null)
                {
                    var policy = default(ExceptionPolicyBase);
                    if (context == Context.Default)
                        policy = policyGroup.PolicyByContextOrDefault(context);

                    policy = policy ?? policyGroup.PolicyByContextOrDefault(Context.Default);
                    if (policy != null)
                        return policy;
                }

                currentType = currentType.GetTypeInfo().BaseType;
            }

            return null;
        }
    }
}