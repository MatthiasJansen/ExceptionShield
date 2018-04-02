#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

#endregion

using System;

namespace ExceptionShield.Installer.Builder
{
    public class DefaultPolicyDefinitionBuilderProxy<TSrc>
        where TSrc : Exception
        
    {
        public PolicyBuilderHead<TSrc, TEnd> SetTargetForDefaultContext<TEnd>()
            where TEnd : Exception
        {
            return new PolicyBuilderHead<TSrc, TEnd>(Context.Default);
        }
    }

    public class RegularPolicyDefinitionBuilderProxy<TSrc>
        where TSrc : Exception
    {
        public PolicyBuilderHead<TSrc, TEnd> SetTargetForContext<TEnd>(string context) where TEnd : Exception
        {
            return new PolicyBuilderHead<TSrc, TEnd>(context);
        }
    }
}