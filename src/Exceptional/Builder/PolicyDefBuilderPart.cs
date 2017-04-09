#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

using System;
using System.Collections.Generic;
using Exceptional.Handlers;
using Exceptional.Policies;
using Exceptional.Terminators;

#endregion

namespace Exceptional.Builder
{
    public class PolicyDefBuilderPart<TSrc, TCur, TNxt, TEnd>
        where TSrc : Exception
        where TCur : Exception
        where TNxt : Exception
        where TEnd : Exception
    {
        private readonly string context;
        private readonly Dictionary<Type, ExceptionHandlerBase> handlers;

        public PolicyDefBuilderPart(string context, Dictionary<Type, ExceptionHandlerBase> handlers,
            ExceptionHandler<TCur, TNxt> handler)
        {
            handlers.Add(typeof(TCur), handler);

            this.handlers = handlers;
            this.context = context;
        }

        public PolicyDefBuilderPart<TSrc, TNxt, TTar, TEnd> Then<TTar>(ExceptionHandler<TNxt, TTar> thenHandler)
            where TTar : Exception
        {
            return new PolicyDefBuilderPart<TSrc, TNxt, TTar, TEnd>(context, handlers, thenHandler);
        }

        public CompletePolicyDefinition<TSrc, TEnd> ThenComplete(ExceptionHandler<TNxt, TEnd> handler,
            TerminatorBase<TEnd> terminatorBase = null)
        {
            handlers.Add(typeof(TNxt), handler);

            var policy = new ExceptionPolicy<TSrc, TEnd>(handlers, terminatorBase);

            return new CompletePolicyDefinition<TSrc, TEnd>(context, policy);
        }
    }
}