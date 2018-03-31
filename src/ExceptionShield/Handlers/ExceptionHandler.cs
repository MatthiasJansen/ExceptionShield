#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

#endregion

using System;

namespace ExceptionShield.Handlers
{
    public class ExceptionHandler<TSrc, TEnd> : IExceptionHandler<TSrc, TEnd>
        where TSrc : Exception
        where TEnd : Exception
    {
        public virtual TEnd Handle(TSrc src)
        {
            var result = Activator.CreateInstance<TEnd>();

            return result;
        }

        public Exception Handle(Exception src)
        {
            return Handle(src as TSrc);
        }
    }
}