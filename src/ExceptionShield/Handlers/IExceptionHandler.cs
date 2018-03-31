#region headers

// Copyright (c) 2018 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

using System;

#endregion

namespace ExceptionShield.Handlers
{
    public interface IExceptionHandler<TSrc, TEnd> : IExceptionHandler
        where TSrc : Exception
        where TEnd : Exception
    {
        TEnd Handle(TSrc src);
    }

    public interface IExceptionHandler
    {
        Exception Handle(Exception src);
    }
}