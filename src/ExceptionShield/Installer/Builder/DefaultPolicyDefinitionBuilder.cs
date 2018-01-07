#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

#endregion

using System;

namespace ExceptionShield.Installer.Builder
{
    public class DefaultPolicyDefinitionBuilder<TSrc, TEnd> : PolicyBuilderHead<TSrc, TEnd>
        where TSrc : Exception
        where TEnd : Exception
    {
        public DefaultPolicyDefinitionBuilder() : base(Context.Default)
        {
        }
    }
}