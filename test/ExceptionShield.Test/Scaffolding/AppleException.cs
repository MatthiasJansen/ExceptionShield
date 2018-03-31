using System.Collections.Generic;
using System.Text;

namespace ExceptionShield.Test.Scaffolding
{
    public class AppleException : FruitException
    {
        public AppleException()
        {
            
        }
        public AppleException(string message): base(message)
        {

        }
    }
}