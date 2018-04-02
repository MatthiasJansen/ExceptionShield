#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

#endregion

using System;
using System.Linq;
using ExceptionShield.Exceptions;
using ExceptionShield.Handlers;
using ExceptionShield.Installer.Builder;
using ExceptionShield.Policies;
using ExceptionShield.Rules;
using ExceptionShield.Strategies;
using ExceptionShield.Terminators;
using ExceptionShield.Test.Scaffolding;
using FluentAssertions;
using Xunit;

namespace ExceptionShield.Test
{
    public class ExceptionManagerTests
    {
        internal class TestExceptionHandler<TSrc> : ExceptionHandler<TSrc, TSrc>
            where TSrc : Exception
        {
            public bool WasHandled { get; private set; }

            public override TSrc Handle(TSrc src)
            {
                Console.WriteLine($"Handled Exception of type: {src.GetType()} with message: {src.Message}");

                WasHandled = true;

                return src;
            }
        }

        internal class TestExceptionHandlerB<TSrc> : ExceptionHandler<TSrc, AppleException>
            where TSrc : Exception
        {
            public bool WasHandled { get; private set; }

            public override AppleException Handle(TSrc src)
            {
                Console.WriteLine($"Handled Exception of type: {src.GetType()} with message: {src.Message}");

                WasHandled = true;
                var next = new AppleException(src.Message);
                return next;
            }
        }

        [Fact]
        //[Description("Creates two policies which both handle the same exception type.")]
        public void FailureBehaviour01()
        {
            var policy1 = PolicyGroupBuilder
                .Create<BananaException>
                    (d => d.SetTargetForDefaultContext<BananaException>()
                          .StartAndComplete(c => c.Set<ExceptionHandler<BananaException, BananaException>>())
                           .WithoutTerminator()
                    );

            var policy2 = PolicyGroupBuilder
                .Create<BananaException>
                    (d => d.SetTargetForDefaultContext<BananaException>()
                          .StartAndComplete(c => c.Set<ExceptionHandler<BananaException, BananaException>>())
                           .WithoutTerminator());

            Action ctor = () => new ExceptionManager(new[]
                                                     {
                                                         policy1,
                                                         policy2
                                                     });

            ctor.Should().Throw<ExceptionManagerConfigurationException>();
        }        
        
        [Fact]
        //[Description("Creates two policies which both handle the same exception type.")]
        public void FailureBehaviour02()
        {
            var policy1 = PolicyGroupBuilder
                .Create<AppleException>
                    (d => d.SetTargetForDefaultContext<BerlinException>()
                          .StartAndComplete(c => c.Set<ExceptionHandler<AppleException, BerlinException>>())
                          .WithoutTerminator()
                    );

            var policy2 = PolicyGroupBuilder
                .Create<AppleException>
                    (d => d.SetTargetForDefaultContext<BeirutException>()
                          .StartAndComplete(c => c.Set<ExceptionHandler<AppleException, BeirutException>>())
                          .WithoutTerminator()
                    );

            var policyGroup = new IExceptionPolicyGroup[]
                              {
                                  policy1,
                                  policy2
                              };

            Action ctor = () => new ExceptionManager(policyGroup);
            ctor.Should().Throw<ExceptionManagerConfigurationException>();
        }

        [Fact]
        public void ShouldThrowPolicyMissingException1()
        {
            var manager = new ExceptionManager(Enumerable.Empty<IExceptionPolicyGroup>());
            manager.Invoking(m => m.Handle(new BananaException(), string.Empty))
                   .Should().ThrowExactly<PolicyMissingException>();
        }


        [Fact]
        public void ShouldThrowPolicyMissingException2()
        {
            var manager = new ExceptionManager(Enumerable.Empty<IExceptionPolicyGroup>());

            manager.Invoking(m => m.Handle(new BananaException(), null))
                   .Should().ThrowExactly<PolicyMissingException>();
        }


        [Fact]
        //[Description(@"Tests the building a simple policy with a single conversion step.")]
        public void T1()
        {
            var policy1 = PolicyGroupBuilder
                .Create<PearException>
                    (d => d.SetTargetForDefaultContext<BananaException>()
                          .StartAndComplete(c => c.Set<ExceptionHandler<PearException, BananaException>>())
                          .WithoutTerminator()
                    );

            policy1.Should().NotBeNull();
            var manager = new ExceptionManager(new[]
                                               {
                                                   policy1
                                               });


            manager.Invoking(m => m.Handle(new PearException()))
                   .Should().ThrowExactly<BananaException>();
        }        
        
        [Fact]
        public void Handle_ShouldNotThrowTheResultingException_WhenThePolicyIsTerminated()
        {
            var policy1 = PolicyGroupBuilder
                .Create<AppleException>
                    (d => d.SetTargetForDefaultContext<PearException>()
                          .StartAndComplete(c => c.Set<ExceptionHandler<AppleException, PearException>>())
                          .WithTerminator<VoidTerminator<PearException>>()
                    );

            policy1.Should().NotBeNull();
            var manager = new ExceptionManager(new[]
                                               {
                                                   policy1
                                               });


            manager.Invoking(m => m.Handle(new AppleException()))
                   .Should().NotThrow();
        }        

        [Fact]
        public void Handle_ShouldThrowArgumentNullException_WhenExceptionIsNull()
        {
            var policy1 = PolicyGroupBuilder
                .Create<AppleException>
                    (d => d.SetTargetForDefaultContext<PearException>()
                          .StartAndComplete(c => c.Set<ExceptionHandler<AppleException, PearException>>())
                          .WithTerminator<VoidTerminator<PearException>>()
                    );

            policy1.Should().NotBeNull();
            var manager = new ExceptionManager(new[]
                                               {
                                                   policy1
                                               });


            manager.Invoking(m => m.Handle(null as Exception))
                   .Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void T2()
        {
            var policy2 = PolicyGroupBuilder
                .Create<AppleException>
                    (
                     d => d.SetTargetForDefaultContext<BananaException>()
                          .Start<PearException>(c => c.Set<ExceptionHandler<AppleException, PearException>>())
                          .ThenComplete(c => c.Set<ExceptionHandler<PearException, BananaException>>())
                          .WithoutTerminator()
                    );

            policy2.Should().NotBeNull();
            var manager = new ExceptionManager(new[]
                                               {
                                                   policy2
                                               });

            manager.Invoking(m => m.Handle(new AppleException()))
                   .Should().ThrowExactly<BananaException>();
        }

        [Fact]
        public void T3()
        {
            var policy3 = PolicyGroupBuilder
                .Create<CherryException>
                    (d => d.SetTargetForDefaultContext<BananaException>()
                          .Start<PearException>(c => c.Set<ExceptionHandler<CherryException, PearException>>())
                          .Then<MissingFieldException>(c => c.Set<ExceptionHandler<PearException, MissingFieldException>>())
                          .ThenComplete(c => c.Set<ExceptionHandler<MissingFieldException, BananaException>>())
                          .WithoutTerminator()
                    );

            policy3.Should().NotBeNull();
            var manager = new ExceptionManager(new[]
                                               {
                                                   policy3
                                               }, new PolicyMissingDefaultRule(),
                                               new DefaultPolicyMatchingStrategy());

            manager.Invoking(m => m.Handle(new CherryException()))
                   .Should().ThrowExactly<BananaException>();
        }

        [Fact]
        public void ThrowsExceptionWhenNoTerminatorDefined()
        {
            var policy1 = PolicyGroupBuilder
                .Create<AppleException>
                    (d => d.SetTargetForDefaultContext<AppleException>()
                          .StartAndComplete(c => c.Set<TestExceptionHandler<AppleException>>())
                           .WithoutTerminator());

            policy1.Should().NotBeNull();
            var manager = new ExceptionManager(new[]
                                               {
                                                   policy1
                                               }, new PolicyMissingDefaultRule(),
                                               new DefaultPolicyMatchingStrategy());

            manager.Invoking(m => m.Handle(new BananaException()))
                   .Should().ThrowExactly<PolicyMissingException>();
        }

        [Fact]
        public void UnWrappRuleTest01()
        {
            var policy2 = PolicyGroupBuilder
                .Create<AggregateException>
                    (
                     bd => bd.SetTargetForDefaultContext<AppleException>()
                           .StartAndComplete(c => c.Set<TestExceptionHandlerB<AggregateException>>())
                             .WithoutTerminator(),
                     b1 => b1.SetTargetForContext<AppleException>("marten")
                             .StartAndComplete(c => c.Set<TestExceptionHandlerB<AggregateException>>())
                             .WithoutTerminator()
                    );

            var exception = new AggregateException("Greetings from outer exception",
                                                   Enumerable
                                                       .Repeat(new AppleException("Greetings from inner Exception."),
                                                               1));

            policy2.Should().NotBeNull();
            var manager = new ExceptionManager
                (new[]
                 {
                     policy2
                 }
                 , new PolicyMissingDefaultRule()
                 , new DefaultPolicyMatchingStrategy());

            manager.Invoking(m => m.Handle(exception))
                   .Should().ThrowExactly<AppleException>();
        }
    }
}