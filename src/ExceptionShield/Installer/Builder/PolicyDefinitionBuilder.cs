#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

#endregion

using System;

namespace ExceptionShield.Installer.Builder
{
    public class PolicyDefinitionBuilder<TSrc, TEnd>
        where TSrc : Exception
        where TEnd : Exception
    {
        public PolicyDefBuilderHead<TSrc, TEnd> SetContext(string context)
        {
            return new PolicyDefBuilderHead<TSrc, TEnd>(context);
        }
    }
}