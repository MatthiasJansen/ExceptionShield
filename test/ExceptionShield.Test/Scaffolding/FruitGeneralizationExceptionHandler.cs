using ExceptionShield.Handlers;

namespace ExceptionShield.Test.Scaffolding
{
    public class FruitGeneralizationExceptionHandler : ExceptionHandler<AppleException, FruitException>
    {
        /// <inheritdoc />
        public override FruitException Handle(AppleException src)
        {
            return base.Handle(src);
        }
    }
}