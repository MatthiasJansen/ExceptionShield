#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

using System;

#endregion

namespace ExceptionManager.Handlers
{
    public class ExceptionHandler<TSrc, TEnd> : ExceptionHandlerBase
        where TSrc : Exception
        where TEnd : Exception
    {
        public virtual TEnd Handle(TSrc src)
        {
            var result = Activator.CreateInstance<TEnd>();

            return result;
        }

        public sealed override Exception Handle(Exception src)
        {
            return Handle(src as TSrc);
        }
    }
}