using System;

namespace ExceptionShield.Test.Scaffolding
{
    public class FruitException : Exception
    {
        public FruitException()
        {
            
        }
        public FruitException(string message) : base(message)
        {
        }
    }
}