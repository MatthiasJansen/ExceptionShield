using System;

namespace ExceptionShield.Installer.Builder
{
    public delegate CompletePolicyDefinition<TSrc, TEnd>
        RegularCreator<TSrc, TEnd>(RegularPolicyDefinitionBuilderProxy<TSrc, TEnd> builderProxy)
        where TSrc : Exception
        where TEnd : Exception;
}