#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

using System;
using System.Collections.Generic;
using ExceptionShield.Exceptions;
using ExceptionShield.Handlers;
using ExceptionShield.Plugable.Resolver;
using ExceptionShield.Terminators;
using JetBrains.Annotations;

#endregion

namespace ExceptionShield.Policies
{
    public class ExceptionPolicy<TSrc, TEnd>
        : ExceptionPolicy<TSrc, TEnd, VoidTerminator<TEnd>>
        where TEnd : Exception
        where TSrc : Exception
    {
        public ExceptionPolicy(Dictionary<Type, Type> handlerDefinitions) : base(handlerDefinitions)
        {
        }
    }

    public class ExceptionPolicy<TSrc, TEnd, TTer> : ExceptionPolicyBase
        where TSrc : Exception
        where TEnd : Exception
        where TTer : TerminatorBase<TEnd>
    {
        private readonly Dictionary<Type, Type> handlerDefinitions;

        public ExceptionPolicy(Dictionary<Type, Type> handlerDefinitions)
        {
            this.handlerDefinitions   = handlerDefinitions;
        }

        public override Type Handles => typeof(TSrc);
        public override Type Returns => typeof(TEnd);

        [CanBeNull]
        public override Exception Handle(IExceptionalResolver resolver, Exception src)
        {
            var cur = src;
            foreach (var handlerDesc in this.handlerDefinitions)
            {
                var handler  = (ExceptionHandlerBase) resolver.Resolve(handlerDesc.Value);

                cur = handler?.Handle(cur);
                if (cur == null)
                {
                    throw new ExceptionManagerConfigurationException();
                }
            }


            // No terminator was defined, the exception will be returned for re-throwing.
            if (typeof(TTer) == typeof(VoidTerminator<TEnd>))
            {
                return cur;
            }

            var terminator = resolver.Resolve<TTer>();
            terminator.Terminate((TEnd) cur);

            return null;
        }
    }
}