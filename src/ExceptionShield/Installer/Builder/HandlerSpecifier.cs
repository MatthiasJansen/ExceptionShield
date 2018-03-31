using System;
using ExceptionShield.Handlers;

namespace ExceptionShield.Installer.Builder
{
    public class HandlerSpecifier<TSrc, TTar>
        where TSrc : Exception
        where TTar : Exception
    {
        private Type handlerSpecification;

        public void Set<THandler>()
            where THandler : IExceptionHandler<TSrc, TTar>
        {
            this.handlerSpecification = typeof(THandler);
        }

        internal Type HandlerType => this.handlerSpecification;
    }
}