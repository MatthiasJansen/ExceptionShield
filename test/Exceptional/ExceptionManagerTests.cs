#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

using System;
using System.Linq;
using ExceptionManager.Builder;
using ExceptionManager.Exceptions;
using ExceptionManager.Handlers;
using NUnit.Framework;

#endregion

namespace ExceptionManagerTests
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
        public void ByDefaultReturnsExceptionWhenNoPolicyDefined()
        {
            var manager = new ExceptionManager.ExceptionManager();

            var handlerT01 = new TestExceptionHandler<DummyException>();

            var policy1 =
                PolicyGroupBuilder.Create<DummyException, DummyException>(d => d
                    .StartAndComplete(handlerT01));

            manager.AddPolicyGroup(policy1);

            var exception = manager.Handle(new OutOfMemoryException());

            Assert.IsInstanceOf<OutOfMemoryException>(exception);
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

            var manager = new ExceptionManager.ExceptionManager();
            manager.AddPolicyGroup(policy1);

            Assert.Throws<ExceptionManagerConfigurationException>(() => manager.AddPolicyGroup(policy2));
        }

        [Test]
        public void FailureBehaviour02()
        {
            var policy1 = PolicyGroupBuilder.Create<OutOfMemoryException, ApplicationException>(d => d
                .StartAndComplete(new ExceptionHandler<OutOfMemoryException, ApplicationException>()));

            var policy2 = PolicyGroupBuilder.Create<OutOfMemoryException, NullReferenceException>(d => d
                .StartAndComplete(new ExceptionHandler<OutOfMemoryException, NullReferenceException>()));

            var manager = new ExceptionManager.ExceptionManager();
            manager.AddPolicyGroup(policy1);

            Assert.Throws<ExceptionManagerConfigurationException>(() => manager.AddPolicyGroup(policy2));
        }

        [Test]
        public void ShouldNotThrowWhenContextIsEmpty()
        {
            var manager = new ExceptionManager.ExceptionManager();
            Assert.That(() => manager.Handle(new OutOfMemoryException(), string.Empty), Throws.Nothing);
        }

        [Test]
        public void ShouldNotThrowWhenContextIsNull()
        {
            var manager = new ExceptionManager.ExceptionManager();
            Assert.That(() => manager.Handle(new OutOfMemoryException(), null), Throws.Nothing);
        }


        [Test]
        [Description(@"Tests the building a simple policy with a single conversion step.")]
        public void T1()
        {
            //var policy1 =
            //    PolicyGroupBuilder.Create<OutOfMemoryException, ApplicationException>()
            //        .StartAndComplete(() => new ExceptionHandler<OutOfMemoryException, ApplicationException>());

            var policy1 = PolicyGroupBuilder.Create<OutOfMemoryException, ApplicationException>
            (
                d => d.StartAndComplete(new ExceptionHandler<OutOfMemoryException, ApplicationException>())
            );

            Assert.IsNotNull(policy1);

            var manager = new ExceptionManager.ExceptionManager();
            manager.AddPolicyGroup(policy1);

            var result = manager.Handle(new OutOfMemoryException());

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ApplicationException>(result);
        }

        [Test]
        public void T2()
        {
            var policy2 = PolicyGroupBuilder.Create<OutOfMemoryException, ApplicationException>
            (d => d.Start(new ExceptionHandler<OutOfMemoryException, ArgumentException>())
                .ThenComplete(new ExceptionHandler<ArgumentException, ApplicationException>()));

            Assert.IsNotNull(policy2);

            var manager = new ExceptionManager.ExceptionManager();
            manager.AddPolicyGroup(policy2);

            var result = manager.Handle(new OutOfMemoryException());

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ApplicationException>(result);
        }

        [Test]
        public void T3()
        {
            var policy3 = PolicyGroupBuilder.Create<OutOfMemoryException, ApplicationException>(d => d
                .Start(new ExceptionHandler<OutOfMemoryException, ArgumentException>())
                .Then(new ExceptionHandler<ArgumentException, AccessViolationException>())
                .ThenComplete(new ExceptionHandler<AccessViolationException, ApplicationException>()));

            Assert.IsNotNull(policy3);

            var manager = new ExceptionManager.ExceptionManager();
            manager.AddPolicyGroup(policy3);

            var result = manager.Handle(new OutOfMemoryException());

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ApplicationException>(result);
        }

        [Test]
        public void UnWrappRuleTest01()
        {
            var manager = new ExceptionManager.ExceptionManager();

            var handlerT01 = new TestExceptionHandler<DummyException>();
            var handlerT02 = new UnwrapExceptionHandler(manager);

            var policy1 =
                PolicyGroupBuilder.Create<AggregateException, AggregateException>(d => d
                    .StartAndComplete(handlerT02));

            var policy2 =
                PolicyGroupBuilder.Create<DummyException, DummyException>(d => d
                    .StartAndComplete(handlerT01));

            manager.AddPolicyGroup(policy1);
            manager.AddPolicyGroup(policy2);

            var exception = new AggregateException("Greetings from outer exception",
                Enumerable.Repeat(new DummyException("Greetings from inner Exception."), 1));

            manager.Handle(exception);

            Assert.IsTrue(handlerT01.WasHandled);
        }
    }
}