#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

using System;
using System.Linq;
using Exceptional.Exceptions;
using Exceptional.Handlers;
using Exceptional.Installer.Builder;
using Exceptional.Rules;
using NUnit.Framework;

#endregion

namespace Exceptional.Test
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

        internal class DummyException : Exception
        {
            public DummyException(string message) : base(message)
            {
            }
        }

        [Test]
        public void FailureBehaviour01()
        {
            var policy1 =
                PolicyGroupBuilder.Create<OutOfMemoryException, ApplicationException>(d => d
                    .StartAndComplete(new ExceptionHandler<OutOfMemoryException, ApplicationException>()));

            var policy2 =
                PolicyGroupBuilder.Create<OutOfMemoryException, ApplicationException>(d => d
                    .StartAndComplete(new ExceptionHandler<OutOfMemoryException, ApplicationException>()));

            ExceptionManagerConfiguration.AddPolicyGroup(policy1);
            var manager = ExceptionManagerConfiguration.LockAndCreateManager();

            Assert.Throws<ExceptionManagerConfigurationException>(
                () => ExceptionManagerConfiguration.AddPolicyGroup(policy2));
        }

        [Test]
        public void FailureBehaviour02()
        {
            var policy1 = PolicyGroupBuilder.Create<OutOfMemoryException, ApplicationException>(d => d
                .StartAndComplete(new ExceptionHandler<OutOfMemoryException, ApplicationException>()));

            var policy2 = PolicyGroupBuilder.Create<OutOfMemoryException, NullReferenceException>(d => d
                .StartAndComplete(new ExceptionHandler<OutOfMemoryException, NullReferenceException>()));

            ExceptionManagerConfiguration.AddPolicyGroup(policy1);
            var manager = ExceptionManagerConfiguration.LockAndCreateManager();

            Assert.Throws<ExceptionManagerConfigurationException>(
                () => ExceptionManagerConfiguration.AddPolicyGroup(policy2));
        }

        [Test]
        public void ShouldThrowPolicyMissingException1()
        {
            var manager = ExceptionManagerConfiguration.LockAndCreateManager();
            Assert.That(() => manager.Handle(new OutOfMemoryException(), string.Empty),
                Throws.TypeOf<PolicyMissingException>());
        }

        [Test]
        public void ShouldThrowPolicyMissingException2()
        {
            var manager = ExceptionManagerConfiguration.LockAndCreateManager();
            Assert.That(() => manager.Handle(new OutOfMemoryException(), null),
                Throws.TypeOf<PolicyMissingException>());
        }


        [Test]
        [Description(@"Tests the building a simple policy with a single conversion step.")]
        public void T1()
        {
            var policy1 = PolicyGroupBuilder.Create<OutOfMemoryException, ApplicationException>(d => d
                .StartAndComplete(new ExceptionHandler<OutOfMemoryException, ApplicationException>())
            );

            Assert.IsNotNull(policy1);
            ExceptionManagerConfiguration.AddPolicyGroup(policy1);
            var manager = ExceptionManagerConfiguration.LockAndCreateManager();


            Assert.That(() => manager.Handle(new OutOfMemoryException()), Throws.TypeOf<ApplicationException>());
        }

        [Test]
        public void T2()
        {
            var policy2 = PolicyGroupBuilder.Create<OutOfMemoryException, ApplicationException>(d => d
                .Start(new ExceptionHandler<OutOfMemoryException, ArgumentException>())
                .ThenComplete(new ExceptionHandler<ArgumentException, ApplicationException>())
            );

            Assert.IsNotNull(policy2);

            ExceptionManagerConfiguration.AddPolicyGroup(policy2);
            var manager = ExceptionManagerConfiguration.LockAndCreateManager();

            Assert.That(() => manager.Handle(new OutOfMemoryException()), Throws.TypeOf<ApplicationException>());
        }

        [Test]
        public void T3()
        {
            var policy3 = PolicyGroupBuilder.Create<OutOfMemoryException, ApplicationException>(d => d
                .Start(new ExceptionHandler<OutOfMemoryException, ArgumentException>())
                .Then(new ExceptionHandler<ArgumentException, AccessViolationException>())
                .ThenComplete(new ExceptionHandler<AccessViolationException, ApplicationException>())
            );

            Assert.IsNotNull(policy3);

            ExceptionManagerConfiguration.AddPolicyGroup(policy3);
            var manager = ExceptionManagerConfiguration.LockAndCreateManager();

            Assert.That(() => manager.Handle(new OutOfMemoryException()), Throws.TypeOf<ApplicationException>());
        }

        [Test]
        public void ThrowsExceptionWhenNoTerminatorDefined()
        {
            var handlerT01 = new TestExceptionHandler<DummyException>();

            var policy1 = PolicyGroupBuilder.Create<DummyException, DummyException>(d => d
                    .StartAndComplete(handlerT01));
            ExceptionManagerConfiguration.AddPolicyGroup(policy1);
            var manager = ExceptionManagerConfiguration.LockAndCreateManager();

            Assert.That(() => manager.Handle(new OutOfMemoryException()), Throws.TypeOf<PolicyMissingException>());
        }

        [Test]
        public void UnWrappRuleTest01()
        {
            var handlerT01 = new TestExceptionHandler<DummyException>();


            var policy2 =
                PolicyGroupBuilder.Create<DummyException, DummyException>(d => d
                    .StartAndComplete(handlerT01));

            ExceptionManagerConfiguration.AddPolicyGroup(policy2);

            var exception = new AggregateException("Greetings from outer exception",
                Enumerable.Repeat(new DummyException("Greetings from inner Exception."), 1));

            var manager = ExceptionManagerConfiguration.LockAndCreateManager();

            Assert.That(() => manager.Handle(exception), Throws.TypeOf<DummyException>());

            Assert.IsTrue(handlerT01.WasHandled);
        }
    }
}