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
using ExceptionShield.Test.Scaffolding;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace ExceptionShield.Test
{
    public class DefaultStrategyTests
    {
        public static IEnumerable<object[]> TestCases
        {
            get
            {
                var fruitPolicyNormal =
                    PolicyGroupBuilder.Create<FruitException>
                        (d => d.SetTargetForDefaultContext<PearException>()
                               .StartAndComplete(c => c.Set<ExceptionHandler<FruitException, PearException>>())
                               .WithoutTerminator());

                var cherryPolicyNormal =
                    PolicyGroupBuilder.Create<CherryException>
                        (d => d.SetTargetForDefaultContext<PearException>()
                               .StartAndComplete(c => c.Set<ExceptionHandler<CherryException, PearException>>())
                               .WithoutTerminator()
                        );
                var bananaPolicyNormal =
                    PolicyGroupBuilder.Create<BananaException>
                        (d => d.SetTargetForDefaultContext<PearException>()
                               .StartAndComplete(c => c.Set<ExceptionHandler<BananaException, PearException>>())
                               .WithoutTerminator()
                        );

                var fruitPolicySpecial =
                    PolicyGroupBuilder.Create<FruitException>
                        (d => d.SetTargetForDefaultContext<PearException>()
                               .StartAndComplete(c => c.Set<ExceptionHandler<FruitException, PearException>>())
                               .WithoutTerminator(),
                         s => s.SetTargetForContext<PearException>("special")
                               .StartAndComplete(c => c.Set<ExceptionHandler<FruitException, PearException>>())
                               .WithoutTerminator());

                yield return new object[]
                             {
                                 typeof(BananaException),
                                 typeof(FruitException),
                                 new Dictionary<Type, IExceptionPolicyGroup>
                                 {
                                     {fruitPolicyNormal.Handles, fruitPolicyNormal}
                                 },
                                 Context.Default
                             };
                yield return new object[]
                             {
                                 typeof(BananaException),
                                 typeof(FruitException),
                                 new Dictionary<Type, IExceptionPolicyGroup>
                                 {
                                     {fruitPolicyNormal.Handles, fruitPolicyNormal}
                                 },
                                 "special"
                             };
                yield return new object[]
                             {
                                 typeof(BananaException),
                                 typeof(FruitException),

                                 new Dictionary<Type, IExceptionPolicyGroup>
                                 {
                                     {
                                         fruitPolicySpecial.Handles, fruitPolicySpecial
                                     }
                                 },
                                 "special"
                             };

                yield return new object[]
                             {
                                 typeof(BananaException),
                                 typeof(FruitException),
                                 new Dictionary<Type, IExceptionPolicyGroup>
                                 {
                                     {fruitPolicyNormal.Handles, fruitPolicyNormal},
                                     {cherryPolicyNormal.Handles, cherryPolicyNormal}
                                 },
                                 Context.Default
                             };
                yield return new object[]
                             {
                                 typeof(BananaException),
                                 typeof(BananaException),
                                 new Dictionary<Type, IExceptionPolicyGroup>
                                 {
                                     {fruitPolicyNormal.Handles, fruitPolicyNormal},
                                     {cherryPolicyNormal.Handles, cherryPolicyNormal},
                                     {bananaPolicyNormal.Handles, bananaPolicyNormal}
                                 },
                                 Context.Default
                             };
            }
        }

        private readonly ITestOutputHelper outputHelper;

        public DefaultStrategyTests(ITestOutputHelper outputHelper)
        {
            this.outputHelper = outputHelper;
        }

        [Theory]
        [MemberData(nameof(TestCases))]
        public void ShouldSelect_ExpectedPolicy(Type requested, Type expected, Dictionary<Type, IExceptionPolicyGroup> policyGroups, string context)
        {
            this.outputHelper.WriteLine($"Requested policy for {requested.Name}");
            this.outputHelper.WriteLine($"Expected matched policy: {expected.Name}");

            this.outputHelper.WriteLine("Policies provided:");
            foreach (var policyGroupsKey in policyGroups.Keys)
            {
                this.outputHelper.WriteLine($"    {policyGroupsKey.Name}");
            }
            
            var strategy = new DefaultPolicyMatchingStrategy();

            var actual = strategy.MatchPolicy(policyGroups, requested, Context.Default);
            this.outputHelper.WriteLine($"Actual matched policy: {actual.Handles.Name}");
            actual.Handles.Should().Be(expected);
        }
    }
}