#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

using System;
using Exceptional.Policies;

#endregion

namespace Exceptional
{
    public interface IExceptionManager
    {
        void Handle<TSrc>(TSrc exception, string context = Context.Default)
            where TSrc : Exception;

        void AddPolicyGroup<TSrc, TEnd>(ExceptionPolicyGroup<TSrc, TEnd> policyGroup)
            where TSrc : Exception
            where TEnd : Exception;
    }
}