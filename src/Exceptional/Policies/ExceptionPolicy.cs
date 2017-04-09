#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

using System;
using System.Collections.Generic;
using Exceptional.Exceptions;
using Exceptional.Handlers;
using Exceptional.Terminators;
using JetBrains.Annotations;

#endregion

namespace Exceptional.Policies
{
    public class ExceptionPolicy<TSrc, TEnd> : ExceptionPolicyBase
        where TSrc : Exception
        where TEnd : Exception
    {
        private readonly Dictionary<Type, ExceptionHandlerBase> handlers;
        private readonly TerminatorBase<TEnd> terminatorBase;

        public ExceptionPolicy(Dictionary<Type, ExceptionHandlerBase> handlers, TerminatorBase<TEnd> terminatorBase)
        {
            this.handlers = handlers;
            this.terminatorBase = terminatorBase;
        }

        public override Type Handles => typeof(TSrc);
        public override Type Returns => typeof(TEnd);

        [CanBeNull]
        public override Exception Handle(Exception src)
        {
            var cur = src;
            foreach (var handler in handlers)
            {
                cur = handler.Value.Handle(cur);
                if (cur == null)
                    throw new ExceptionManagerConfigurationException();
            }

            // No terminator was defined, the exception will be returned for re-throwing.
            if (terminatorBase == null)
                return cur;

            terminatorBase.Terminate((TEnd) cur);

            return null;
        }
    }
}