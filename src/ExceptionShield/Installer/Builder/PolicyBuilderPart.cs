#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

using System;
using System.Collections.Generic;
using ExceptionShield.Policies;

#endregion

namespace ExceptionShield.Installer.Builder
{
    public class PolicyBuilderPart<TSrc, TCur, TEnd>
        where TSrc : Exception // the original exception
        where TCur : Exception // the current exception
        where TEnd : Exception // the final exception
    {
        private readonly string context;
        private readonly Dictionary<Type, Type> handlers;

        public PolicyBuilderPart(string context, Dictionary<Type, Type> handlers)
        {
            this.handlers = handlers;
            this.context = context;
        }

        public PolicyBuilderPart<TSrc, TTar, TEnd>
            Then<TTar>(Action<HandlerSpecifier<TCur, TTar>> action)

            where TTar : Exception
        {
            var handlerSpecifier = new HandlerSpecifier<TCur, TTar>();
            action(handlerSpecifier);

            this.handlers.Add(typeof(TCur), handlerSpecifier.HandlerType);

            return new PolicyBuilderPart<TSrc, TTar, TEnd>(this.context, this.handlers);
        }

        public PolicyBuilderTail<TSrc, TEnd> ThenComplete(Action<HandlerSpecifier<TCur, TEnd>> action)
        {
            var handlerSpecifier = new HandlerSpecifier<TCur, TEnd>();
            action(handlerSpecifier);

            this.handlers.Add(typeof(TCur), handlerSpecifier.HandlerType);
            return new PolicyBuilderTail<TSrc, TEnd>(this.context, this.handlers);
        }
    }
}