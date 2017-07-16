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

namespace Exceptional.Installer.Builder
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

        public PolicyDefBuilderPart<TSrc, TSrc, TNxt, TEnd> Start<TNxt, THnd>()
            where TNxt : Exception
            where THnd : ExceptionHandler<TSrc, TNxt>
        {
            return new PolicyDefBuilderPart<TSrc, TSrc, TNxt, TEnd>
                (context,
                 new Dictionary<Type, Type>(), typeof(THnd));
        }

        public CompletePolicyDefinition<TSrc, TEnd> StartAndComplete<TNxt, THnd>()
            where TNxt : Exception
            where THnd : ExceptionHandler<TSrc, TNxt>
        {
            var handlers = new Dictionary<Type, Type>
                           {
                               {typeof(TSrc), typeof(THnd)}
                           };

            var policy = new ExceptionPolicy<TSrc, TEnd>(handlers);

            return new CompletePolicyDefinition<TSrc, TEnd>(context, policy);
        }
    }
}