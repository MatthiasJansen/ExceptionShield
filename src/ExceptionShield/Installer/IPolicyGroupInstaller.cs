using ExceptionShield.Policies;

namespace ExceptionShield.Installer
{
    public interface IPolicyGroupInstaller
    {
        IExceptionPolicyGroup Provide();
    }
}