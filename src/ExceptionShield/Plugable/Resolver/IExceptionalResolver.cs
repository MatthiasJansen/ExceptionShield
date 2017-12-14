#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

using System;

namespace ExceptionShield.Plugable.Resolver
{
    public interface IExceptionalResolver
    {
        T Resolve<T>();

        object Resolve(Type type);
    }
}