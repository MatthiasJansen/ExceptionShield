#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

using System;
using System.Collections;
using System.Collections.Generic;
using Exceptional.Handlers;
using Exceptional.Installer.Builder;
using Exceptional.Policies;
using Exceptional.Strategies;
using NUnit.Framework;

#endregion

namespace Exceptional.Test
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
                    var g1Cn = PolicyGroupBuilder.Create<L1Ex, Exception>(d => d.StartAndComplete<Exception, ExceptionHandler<L1Ex, Exception>>());
                    var g2Cn = PolicyGroupBuilder.Create<L2Ex, Exception>(d => d.StartAndComplete<Exception, ExceptionHandler<L2Ex, Exception>>());
                    var g3Cn = PolicyGroupBuilder.Create<L3Ex, Exception>(d => d.StartAndComplete<Exception, ExceptionHandler<L3Ex, Exception>>());

                    var g1Cs = PolicyGroupBuilder.Create<L1Ex, Exception>(d => d.StartAndComplete<Exception, ExceptionHandler<L1Ex, Exception>>(),
                        s => s.SetContext("special").StartAndComplete<Exception, ExceptionHandler<L1Ex, Exception>>());

                    yield return new TestCaseData(
                        typeof(L3Ex),
                        new Dictionary<Type, ExceptionPolicyGroupBase> {{g1Cn.Handles, g1Cn}},
                        Context.Default,
                        typeof(L1Ex)
                    );
                    yield return new TestCaseData(
                        typeof(L3Ex),
                        new Dictionary<Type, ExceptionPolicyGroupBase> {{g1Cn.Handles, g1Cn}},
                        "special",
                        typeof(L1Ex)
                    );
                    yield return new TestCaseData(
                        typeof(L3Ex),
                        new Dictionary<Type, ExceptionPolicyGroupBase> {{g1Cs.Handles, g1Cs}},
                        "special",
                        typeof(L1Ex)
                    );
                    yield return new TestCaseData(
                        typeof(L3Ex),
                        new Dictionary<Type, ExceptionPolicyGroupBase> {{g1Cn.Handles, g1Cn}, {g2Cn.Handles, g2Cn}},
                        Context.Default,
                        typeof(L2Ex)
                    );
                    yield return new TestCaseData(
                        typeof(L3Ex),
                        new Dictionary<Type, ExceptionPolicyGroupBase>
                        {
                            {g1Cn.Handles, g1Cn},
                            {g2Cn.Handles, g2Cn},
                            {g3Cn.Handles, g3Cn}
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