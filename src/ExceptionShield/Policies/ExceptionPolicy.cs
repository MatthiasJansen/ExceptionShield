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
    public class ExceptionPolicy<TSrc, TEnd> : ExceptionPolicyBase
        where TSrc : Exception
        where TEnd : Exception
    {
        private readonly IReadOnlyDictionary<Type, Type> handlerDefinitions;
        private readonly Type terminator;

        public ExceptionPolicy(IReadOnlyDictionary<Type, Type> handlerDefinitions, Type terminator = null)
        {
            this.handlerDefinitions   = handlerDefinitions;
            this.terminator = terminator;
        }

        public override Type Handles => typeof(TSrc);
        public override Type Returns => typeof(TEnd);

        [CanBeNull]
        public override Exception Handle(IExceptionalResolver resolver, Exception src)
        {
            var cur = src;
            foreach (var handlerDesc in this.handlerDefinitions)
            {
                var handler = resolver.Resolve(handlerDesc.Value) as ExceptionHandlerBase;

                cur = handler?.Handle(cur);
                if (cur == null)
                {
                    throw new ExceptionManagerConfigurationException();
                }
            }

            return this.terminator == null 
                       ? cur // No terminator was defined, the exception will be returned for re-throwing.
                       : Terminate(resolver, src as TSrc, cur as TEnd);
        }

        private Exception Terminate(IExceptionalResolver resolver, TSrc src, TEnd cur)
        {
            if (!(resolver.Resolve(this.terminator) is TerminatorBase<TEnd> terminator))
            {
                return cur;
            }

            terminator.Terminate(cur);

            return null;
        }
    }
}