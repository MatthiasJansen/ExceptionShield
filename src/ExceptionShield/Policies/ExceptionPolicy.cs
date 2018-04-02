#region headers

// Copyright (c) 2018 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

using System;
using System.Collections.Generic;
using System.Linq;
using ExceptionShield.Exceptions;
using ExceptionShield.Extensions;
using ExceptionShield.Handlers;
using ExceptionShield.Plugable.Resolver;
using ExceptionShield.Terminators;
using JetBrains.Annotations;

#endregion

namespace ExceptionShield.Policies
{
    public class ExceptionPolicy<TSrc, TEnd> : IExceptionPolicy
        where TSrc : Exception
        where TEnd : Exception
    {
        protected readonly IReadOnlyDictionary<Type, Type> handlerDefinitions;
        private readonly Type terminatorType;

        public ExceptionPolicy(IReadOnlyDictionary<Type, Type> handlerDefinitions, Type terminator = null)
        {
            if (terminator != null)
            {
                // needs to be any subtype of TerminatorBase
                // with a generic type in the inheritance chain of TEnd, but not more specific.
                if (!terminator.IsSubclassOfOpenGeneric(typeof(TerminatorBase<>)))
                {
                    throw new ArgumentException(nameof(terminator));
                }

                if (!typeof(TEnd).IsAssignableFrom(terminator.GenericTypeArguments.Single()))
                {
                    throw new ArgumentException(nameof(terminator));
                }
            }


            this.handlerDefinitions = handlerDefinitions;
            this.terminatorType = terminator;
        }

        public Type Handles => typeof(TSrc);
        public Type Returns => typeof(TEnd);

        [CanBeNull]
        public Exception Handle(IExceptionalResolver resolver, Exception src)
        {
            var cur = src;
            foreach (var handlerDesc in this.handlerDefinitions)
            {
                var handler = resolver.Resolve(handlerDesc.Value) as IExceptionHandler;

                cur = handler?.Handle(cur);
                if (cur == null)
                {
                    throw new ExceptionManagerConfigurationException();
                }
            }

            return this.terminatorType == null
                       ? cur // No terminatorType was defined, the exception will be returned for re-throwing.
                       : Terminate(resolver, cur as TEnd);
        }


        private Exception Terminate(IExceptionalResolver resolver, TEnd cur)
        {
            ((TerminatorBase<TEnd>) resolver.Resolve(this.terminatorType)).Terminate(cur);

            return null;
        }
    }
}