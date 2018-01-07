#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

#endregion

using System;
using System.Collections.Generic;
using ExceptionShield.Handlers;
using ExceptionShield.Installer.Builder;
using ExceptionShield.Policies;
using ExceptionShield.Strategies;
using FluentAssertions;
using Xunit;

namespace ExceptionShield.Test
{
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


        public static IEnumerable<object[]> TestCases
        {
            get
            {
                var g1Cn =
                    PolicyGroupBuilder.Create<L1Ex, Exception>(d =>
                                                                   d.StartAndComplete(c => c
                                                                                          .Set<ExceptionHandler<L1Ex
                                                                                              , Exception>>()).WithoutTerminator());
                var g2Cn =
                    PolicyGroupBuilder.Create<L2Ex, Exception>(d =>
                                                                   d.StartAndComplete(c => c
                                                                                          .Set<ExceptionHandler<L2Ex
                                                                                              , Exception>>()).WithoutTerminator());
                var g3Cn =
                    PolicyGroupBuilder.Create<L3Ex, Exception>(d =>
                                                                   d.StartAndComplete(c => c
                                                                                          .Set<ExceptionHandler<L3Ex
                                                                                              , Exception>>()).WithoutTerminator());

                var g1Cs =
                    PolicyGroupBuilder
                        .Create<L1Ex, Exception
                        >(d => d.StartAndComplete(c => c.Set<ExceptionHandler<L1Ex, Exception>>()).WithoutTerminator(),
                          s => s.SetContext("special")
                                .StartAndComplete(c => c.Set<ExceptionHandler<L1Ex, Exception>>()).WithoutTerminator());

                yield return new object[]
                             {
                                 typeof(L3Ex),
                                 new Dictionary<Type, ExceptionPolicyGroupBase>
                                 {
                                     {g1Cn.Handles, g1Cn}
                                 },
                                 Context.Default,
                                 typeof(L1Ex)
                             };
                yield return new object[]
                             {
                                 typeof(L3Ex),
                                 new Dictionary<Type, ExceptionPolicyGroupBase>
                                 {
                                     {g1Cn.Handles, g1Cn}
                                 },
                                 "special",
                                 typeof(L1Ex)
                             };
                yield return new object[]
                             {
                                 typeof(L3Ex),

                                 new Dictionary<Type, ExceptionPolicyGroupBase>
                                 {
                                     {
                                         g1Cs.Handles, g1Cs
                                     }
                                 },
                                 "special",
                                 typeof(L1Ex)
                             };

                yield return new object[]
                             {
                                 typeof(L3Ex),
                                 new Dictionary<Type, ExceptionPolicyGroupBase>
                                 {
                                     {g1Cn.Handles, g1Cn},
                                     {g2Cn.Handles, g2Cn}
                                 },
                                 Context.Default,
                                 typeof(L2Ex)
                             };
                yield return new object[]
                             {
                                 typeof(L3Ex),
                                 new Dictionary<Type, ExceptionPolicyGroupBase>
                                 {
                                     {g1Cn.Handles, g1Cn},
                                     {g2Cn.Handles, g2Cn},
                                     {g3Cn.Handles, g3Cn}
                                 },
                                 Context.Default,
                                 typeof(L3Ex)
                             };
            }
        }


        [Theory]
        [MemberData(nameof(TestCases))]
        public void T1(Type requested, Dictionary<Type, ExceptionPolicyGroupBase> policyGroups, string context,
                       Type expected)
        {
            var strategy = new DefaultPolicyMatchingStrategy();

            var result = strategy.MatchPolicy(policyGroups, requested, Context.Default);

            result.Handles.Should().Be(expected);
        }
    }
}