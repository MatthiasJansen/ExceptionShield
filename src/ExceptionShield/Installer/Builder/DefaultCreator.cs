using System;

namespace ExceptionShield.Installer.Builder
{
    public delegate ICompletePolicyDefinition
        DefaultCreator<TSrc>(DefaultPolicyDefinitionBuilderProxy<TSrc> builder)
        where TSrc : Exception;
}