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
    public class PolicyDefBuilderHead<TSrc, TEnd>
        where TSrc : Exception
        where TEnd : Exception
    {
        private readonly string context;

        public PolicyDefBuilderHead(string context)
        {
            this.context = context;
        }

        public PolicyDefBuilderPart<TSrc, TCur, TNxt, TEnd> Start<TCur, TNxt>(ExceptionHandler<TCur, TNxt> thenHandler)
            where TCur : Exception
            where TNxt : Exception
        {
            return new PolicyDefBuilderPart<TSrc, TCur, TNxt, TEnd>(context,
                new Dictionary<Type, ExceptionHandlerBase>(), thenHandler);
        }

        public CompletePolicyDefinition<TSrc, TEnd> StartAndComplete(ExceptionHandler<TSrc, TEnd> thenHandler,
            TerminatorBase<TEnd> terminator = null)
        {
            var handlers = new Dictionary<Type, ExceptionHandlerBase> {{typeof(TSrc), thenHandler}};

            var policy = new ExceptionPolicy<TSrc, TEnd>(handlers, terminator);

            return new CompletePolicyDefinition<TSrc, TEnd>(context, policy);
        }
    }
}