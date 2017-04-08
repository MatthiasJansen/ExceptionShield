#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

using System;
using System.Collections;
using System.Collections.Generic;
using ExceptionManager;
using ExceptionManager.Builder;
using ExceptionManager.Policies;
using ExceptionManager.Strategies;
using NUnit.Framework;

#endregion

namespace ExceptionManagerTests
{
    [TestFixture]
    public class DefaultStrategyTests
    {
        public class L3Ex : L2Ex
        {
        }

        public class L2Ex : L1Ex
        {
        }

        public class L1Ex : Exception
        {
        }

        public class MyFactoryClass
        {
            public static IEnumerable TestCases
            {
                get
                {
                    var g1cn = PolicyGroupBuilder.Create<L1Ex, Exception>(d => d.StartAndComplete(null));
                    var g2cn = PolicyGroupBuilder.Create<L2Ex, Exception>(d => d.StartAndComplete(null));
                    var g3cn = PolicyGroupBuilder.Create<L3Ex, Exception>(d => d.StartAndComplete(null));

                    var g1cs = PolicyGroupBuilder.Create<L1Ex, Exception>(d => d.StartAndComplete(null),
                        s => s.SetContext("special").StartAndComplete(null));

                    yield return new TestCaseData(
                        typeof(L3Ex),
                        new Dictionary<Type, ExceptionPolicyGroupBase> {{g1cn.Handles, g1cn}},
                        Context.Default,
                        typeof(L1Ex)
                    );
                    yield return new TestCaseData(
                        typeof(L3Ex),
                        new Dictionary<Type, ExceptionPolicyGroupBase> {{g1cn.Handles, g1cn}},
                        "special",
                        typeof(L1Ex)
                    );
                    yield return new TestCaseData(
                        typeof(L3Ex),
                        new Dictionary<Type, ExceptionPolicyGroupBase> {{g1cs.Handles, g1cs}},
                        "special",
                        typeof(L1Ex)
                    );
                    yield return new TestCaseData(
                        typeof(L3Ex),
                        new Dictionary<Type, ExceptionPolicyGroupBase> {{g1cn.Handles, g1cn}, {g2cn.Handles, g2cn}},
                        Context.Default,
                        typeof(L2Ex)
                    );
                    yield return new TestCaseData(
                        typeof(L3Ex),
                        new Dictionary<Type, ExceptionPolicyGroupBase>
                        {
                            {g1cn.Handles, g1cn},
                            {g2cn.Handles, g2cn},
                            {g3cn.Handles, g3cn}
                        },
                        Context.Default,
                        typeof(L3Ex)
                    );
                }
            }
        }

        [Test]
        [TestCaseSource(typeof(MyFactoryClass), nameof(MyFactoryClass.TestCases))]
        public void T1(Type requested, Dictionary<Type, ExceptionPolicyGroupBase> policyGroups, string context,
            Type expected)
        {
            var strategy = new DefaultPolicyMatchingStrategy();

            var result = strategy.MatchPolicy(policyGroups, requested, Context.Default);

            Assert.That(result.Handles, Is.EqualTo(expected));
        }
    }
}