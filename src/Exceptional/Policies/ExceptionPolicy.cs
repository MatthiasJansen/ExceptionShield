#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

using System;
using System.Collections.Generic;
using System.Linq;
using ExceptionManager.Handlers;
using ExceptionManager.Terminators;

#endregion

namespace ExceptionManager.Policies
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
            this.terminatorBase = terminatorBase ?? new DummyTerminatorBase<TEnd>();
        }

        public override Type Handles => typeof(TSrc);
        public override Type Returns => typeof(TEnd);

        public override Exception Handle(Exception src)
        {
            var cur = handlers.Aggregate(src, (current, handler) => handler.Value.Handle(current));

            terminatorBase.Terminate(cur as TEnd);

            return cur;
        }
    }
}