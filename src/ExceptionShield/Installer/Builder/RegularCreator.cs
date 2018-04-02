using System;

namespace ExceptionShield.Installer.Builder
{
    public delegate ICompletePolicyDefinition
        RegularCreator<TSrc>(RegularPolicyDefinitionBuilderProxy<TSrc> builderProxy)
        where TSrc : Exception;
}