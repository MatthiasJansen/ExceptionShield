#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

using System;

#endregion

namespace ExceptionManager.Terminators
{
    public abstract class TerminatorBase<TEnd>
        where TEnd : Exception
    {
        public abstract void Terminate(TEnd exception);
    }
}