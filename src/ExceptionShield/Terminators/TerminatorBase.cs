#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

#endregion

using System;

namespace ExceptionShield.Terminators
{
    public abstract class TerminatorBase<TEnd>
        where TEnd : Exception
    {
        /// <summary>
        /// The terminate method is guaranteed to never throw.
        /// Any exception thrown by TerminateInner will be swallowed.
        /// </summary>
        /// <param name="exception"></param>
        public void Terminate(TEnd exception)
        {
            try
            {
                TerminateInner(exception);
            }
            catch (Exception)
            {

            }
        }

        protected abstract void TerminateInner(TEnd exception);
    }
}