#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

using System;

#endregion

namespace ExceptionManager.Builder
{
    public class DefaultPolicyDefinitionBuilderHead<TSrc, TEnd> : PolicyDefBuilderHead<TSrc, TEnd>
        where TSrc : Exception
        where TEnd : Exception
    {
        public DefaultPolicyDefinitionBuilderHead() : base(Context.Default)
        {
        }
    }
}