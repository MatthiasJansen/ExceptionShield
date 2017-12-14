#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

using System;
using System.Collections.Generic;
using ExceptionShield.Handlers;
using ExceptionShield.Policies;

#endregion

namespace ExceptionShield.Installer.Builder
{
    public class HandlerSpecifier<TSrc, TTar>
        where TSrc : Exception
        where TTar : Exception
    {
        private Type handlerSpecification;

        public void Set<THandler>()
            where THandler : ExceptionHandler<TSrc, TTar>
        {
            this.handlerSpecification = typeof(THandler);
        }

        internal Type HandlerType => this.handlerSpecification;
    }

    public class PolicyDefBuilderPart<TSrc, TCur, TEnd>
        where TSrc : Exception // the original exception
        where TCur : Exception // the current exception
        where TEnd : Exception // the final exception
    {
        private readonly string context;
        private readonly Dictionary<Type, Type> handlers;

        public PolicyDefBuilderPart(string context, Dictionary<Type, Type> handlers)
        {
            this.handlers = handlers;
            this.context = context;
        }

        public PolicyDefBuilderPart<TSrc, TTar, TEnd>
            Then<TTar>(Action<HandlerSpecifier<TCur, TTar>> action)

            where TTar : Exception
        {
            var handlerSpecifier = new HandlerSpecifier<TCur, TTar>();
            action(handlerSpecifier);

            this.handlers.Add(typeof(TCur), handlerSpecifier.HandlerType);

            return new PolicyDefBuilderPart<TSrc, TTar, TEnd>(this.context, this.handlers);
        }

        public CompletePolicyDefinition<TSrc, TEnd> ThenComplete(Action<HandlerSpecifier<TCur, TEnd>> action)
        {
            var handlerSpecifier = new HandlerSpecifier<TCur, TEnd>();
            action(handlerSpecifier);

            this.handlers.Add(typeof(TCur), handlerSpecifier.HandlerType);

            var policy = new ExceptionPolicy<TSrc, TEnd>(this.handlers);

            return new CompletePolicyDefinition<TSrc, TEnd>(this.context, policy);
        }
    }
}