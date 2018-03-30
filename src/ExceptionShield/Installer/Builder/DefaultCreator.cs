using System;

namespace ExceptionShield.Installer.Builder
{
    public delegate CompletePolicyDefinition<TSrc, TEnd>
        DefaultCreator<TSrc, TEnd>(DefaultPolicyDefinitionBuilder<TSrc, TEnd> builder)
        where TSrc : Exception
        where TEnd : Exception;
}