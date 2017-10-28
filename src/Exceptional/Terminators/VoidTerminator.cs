#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

using System;

#endregion

namespace Exceptional.Terminators
{
    public sealed class VoidTerminator<TEnd> : TerminatorBase<TEnd>
        where TEnd : Exception
    {
        protected override void TerminateInner(TEnd exception)
        {
            // Do nothing.
        }
    }
}