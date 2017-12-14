#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

#endregion

using System;

namespace ExceptionShield
{
    public interface IExceptionManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSrc"></typeparam>
        /// <param name="exception"></param>
        /// <param name="context"></param>
        void Handle<TSrc>(TSrc exception, string context = Context.Default)
            where TSrc : Exception;
    }
}