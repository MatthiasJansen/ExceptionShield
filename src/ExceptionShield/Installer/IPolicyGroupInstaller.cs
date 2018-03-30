using ExceptionShield.Policies;

namespace ExceptionShield.Installer
{
    public interface IPolicyGroupInstaller
    {
        ExceptionPolicyGroupBase Provide();
    }
}