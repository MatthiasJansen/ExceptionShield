using System;

namespace ExceptionShield.Handlers
{
    public interface IExceptionHandler
    {
        Exception Handle(Exception src);
    }
}