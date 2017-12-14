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
    public class PolicyDefBuilderHead<TSrc, TEnd>
        where TSrc : Exception
        where TEnd : Exception
    {
        private readonly string context;

        public PolicyDefBuilderHead(string context)
        {
            this.context = context;
        }

        //(Action<HandlerSpecifier<TCur, TEnd>> action)
        //{
        //    var handlerSpecifier = new HandlerSpecifier<TCur, TEnd>();
        //    action(handlerSpecifier);

        //    this.handlers.Add(typeof(TCur), handlerSpecifier.HandlerType);

        public PolicyDefBuilderPart<TSrc, TNxt, TEnd> Start<TNxt>(Action<HandlerSpecifier<TSrc, TNxt>> action)
            where TNxt : Exception
        {
            var handlers = new Dictionary<Type, Type>();

            var handlerSpecifier = new HandlerSpecifier<TSrc, TNxt>();
                action(handlerSpecifier);

            handlers.Add(typeof(TSrc), handlerSpecifier.HandlerType);

            return new PolicyDefBuilderPart<TSrc, TNxt, TEnd>
                (this.context, handlers);
        }

        public CompletePolicyDefinition<TSrc, TEnd> StartAndComplete(Action<HandlerSpecifier<TSrc, TEnd>> action)
        {
            var handlers = new Dictionary<Type, Type>();

            var handlerSpecifier = new HandlerSpecifier<TSrc, TEnd>();
            action(handlerSpecifier);

            handlers.Add(typeof(TSrc), handlerSpecifier.HandlerType);

            var policy = new ExceptionPolicy<TSrc, TEnd>(handlers);

            return new CompletePolicyDefinition<TSrc, TEnd>(this.context, policy);
        }
    }
}