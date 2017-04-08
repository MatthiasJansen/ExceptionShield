#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

using System;

#endregion

namespace ExceptionManager.Handlers
{
    public class UnwrapExceptionHandler : ExceptionHandler<AggregateException, AggregateException>
    {
        private readonly ExceptionManager manager;

        public UnwrapExceptionHandler(ExceptionManager manager)
        {
            this.manager = manager;
        }

        public override AggregateException Handle(AggregateException src)
        {
            foreach (var innerException in src.InnerExceptions)
                manager.Handle(innerException);

            return src;
        }
    }
}