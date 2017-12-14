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
using NUnit.Framework;

namespace ExceptionShield.Test
{
    [TestFixture]
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

        internal class TestExceptionHandlerB<TSrc> : ExceptionHandler<TSrc, DummyException>
            where TSrc : Exception
        {
            public bool WasHandled { get; private set; }

            public override DummyException Handle(TSrc src)
            {
                Console.WriteLine($"Handled Exception of type: {src.GetType()} with message: {src.Message}");

                WasHandled = true;
                var next = new DummyException(src.Message);
                return next;
            }
        }

        internal class DummyException : Exception
        {
            public DummyException(string message) : base(message)
            {
            }
        }

        [Test]
        [Description("Creates two policies which both handle the same exception type.")]
        public void FailureBehaviour01()
        {
            var policy1 = PolicyGroupBuilder
                .Create<OutOfMemoryException, Exception>
                (d => d.StartAndComplete
                     (c => c.Set<ExceptionHandler<OutOfMemoryException, Exception>>())
                );

            var policy2 = PolicyGroupBuilder
                .Create<OutOfMemoryException, Exception>
                (d => d.StartAndComplete
                     (c => c.Set<ExceptionHandler<OutOfMemoryException, Exception>>()));


            Assert.Throws<ExceptionManagerConfigurationException>(() => new ExceptionManager(new[]
                                                                                             {
                                                                                                 policy1,
                                                                                                 policy2
                                                                                             }));
        }

        [Test]
        [Description("Creates two policies which both handle the same exception type.")]
        public void FailureBehaviour02()
        {
            var policy1 = PolicyGroupBuilder
                .Create<OutOfMemoryException, Exception>
                (d => d
                     .StartAndComplete(c => c.Set<ExceptionHandler<OutOfMemoryException, Exception>>())
                );

            var policy2 = PolicyGroupBuilder
                .Create<OutOfMemoryException, NullReferenceException>
                (d => d
                     .StartAndComplete(c => c.Set<ExceptionHandler<OutOfMemoryException, NullReferenceException>>())
                );

            Assert.Throws<ExceptionManagerConfigurationException>(() =>
                                                                      new ExceptionManager(new ExceptionPolicyGroupBase
                                                                                               []
                                                                                               {
                                                                                                   policy1,
                                                                                                   policy2
                                                                                               }));
        }

        [Test]
        public void ShouldThrowPolicyMissingException1()
        {
            var manager = new ExceptionManager(Enumerable.Empty<ExceptionPolicyGroupBase>());
            Assert.That(() => manager.Handle(new OutOfMemoryException(), string.Empty),
                        Throws.TypeOf<PolicyMissingException>());
        }

        [Test]
        public void ShouldThrowPolicyMissingException2()
        {
            var manager = new ExceptionManager(Enumerable.Empty<ExceptionPolicyGroupBase>());
            Assert.That(() => manager.Handle(new OutOfMemoryException(), null),
                        Throws.TypeOf<PolicyMissingException>());
        }


        [Test]
        [Description(@"Tests the building a simple policy with a single conversion step.")]
        public void T1()
        {
            var policy1 = PolicyGroupBuilder
                .Create<OutOfMemoryException, Exception>
                (d => d
                     .StartAndComplete(c => c.Set<ExceptionHandler<OutOfMemoryException, Exception>>())
                );

            Assert.IsNotNull(policy1);
            var manager = new ExceptionManager(new[]
                                               {
                                                   policy1
                                               });


            Assert.That(() => manager.Handle(new OutOfMemoryException()), Throws.TypeOf<Exception>());
        }

        [Test]
        public void T2()
        {
            var policy2 = PolicyGroupBuilder
                .Create<OutOfMemoryException, Exception>
                (
                 d => d
                     .Start<ArgumentException>(c => c.Set<ExceptionHandler<OutOfMemoryException, ArgumentException>>())
                     .ThenComplete(c => c.Set<ExceptionHandler<ArgumentException, Exception>>())
                );

            Assert.IsNotNull(policy2);
            var manager = new ExceptionManager(new[]
                                               {
                                                   policy2
                                               });

            Assert.That(() => manager.Handle(new OutOfMemoryException()), Throws.TypeOf<Exception>());
        }

        [Test]
        public void T3()
        {
            var policy3 = PolicyGroupBuilder
                .Create<OutOfMemoryException, Exception>
                (d => d
                     .Start<ArgumentException>(c => c.Set<ExceptionHandler<OutOfMemoryException, ArgumentException>>())
                     .Then<MissingFieldException
                     >(c => c.Set<ExceptionHandler<ArgumentException, MissingFieldException>>())
                     .ThenComplete(c => c.Set<ExceptionHandler<MissingFieldException, Exception>>())
                );

            Assert.IsNotNull(policy3);
            var manager = new ExceptionManager(new[]
                                               {
                                                   policy3
                                               }, new PolicyMissingDefaultRule(),
                                               new DefaultPolicyMatchingStrategy());

            Assert.That(() => manager.Handle(new OutOfMemoryException()), Throws.TypeOf<Exception>());
        }

        [Test]
        public void ThrowsExceptionWhenNoTerminatorDefined()
        {
            var policy1 = PolicyGroupBuilder
                .Create<DummyException, DummyException>
                (d => d.StartAndComplete(c => c.Set<TestExceptionHandler<DummyException>>()));

            Assert.IsNotNull(policy1);
            var manager = new ExceptionManager(new[]
                                               {
                                                   policy1
                                               }, new PolicyMissingDefaultRule(),
                                               new DefaultPolicyMatchingStrategy());

            Assert.That(() => manager.Handle(new OutOfMemoryException()), Throws.TypeOf<PolicyMissingException>());
        }

        [Test]
        public void UnWrappRuleTest01()
        {
            var policy2 = PolicyGroupBuilder
                .Create<AggregateException, DummyException>
                (
                 bd => bd.StartAndComplete(c => c.Set<TestExceptionHandlerB<AggregateException>>()),
                 b1 => b1.SetContext("marten")
                         .StartAndComplete(c => c.Set<TestExceptionHandlerB<AggregateException>>())
                );

            var exception = new AggregateException("Greetings from outer exception",
                                                   Enumerable
                                                       .Repeat(new DummyException("Greetings from inner Exception."),
                                                               1));

            Assert.IsNotNull(policy2);
            var manager = new ExceptionManager
                (new[]
                 {
                     policy2
                 }
                 , new PolicyMissingDefaultRule()
                 , new DefaultPolicyMatchingStrategy());

            Assert.That(() => manager.Handle(exception), Throws.TypeOf<DummyException>());
        }
    }
}