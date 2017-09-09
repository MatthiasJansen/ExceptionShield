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
    public class PolicyDefBuilderPart<TSrc, TCur, TNxt, TEnd>
        where TSrc : Exception
        where TCur : Exception
        where TNxt : Exception
        where TEnd : Exception
    {
        private readonly string context;
        private readonly Dictionary<Type, Type> handlers;

        public PolicyDefBuilderPart(string context, Dictionary<Type, Type> handlers,
            Type defaultHandler)
        {
            handlers.Add(typeof(TCur), defaultHandler);

            this.handlers = handlers;
            this.context = context;
        }

        public PolicyDefBuilderPart<TSrc, TNxt, TTar, TEnd> Then<TTar, THnd>()
            where TTar : Exception
        {
            return new PolicyDefBuilderPart<TSrc, TNxt, TTar, TEnd>(this.context, this.handlers, typeof(THnd));
        }

        public CompletePolicyDefinition<TSrc, TEnd> ThenComplete<THnd>()
        {
            this.handlers.Add(typeof(TNxt), typeof(THnd));

            var policy = new ExceptionPolicy<TSrc, TEnd>(this.handlers);

            return new CompletePolicyDefinition<TSrc, TEnd>(this.context, policy);
        }
    }
}