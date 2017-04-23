using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exceptional.Builder;
using Exceptional.Policies;
using JetBrains.Annotations;

namespace Exceptional.Installer
{
    public abstract class PolicyGroupInstaller<TSrc, TDst>
        where TSrc : Exception
        where TDst : Exception
    {
        private readonly List<Func<PolicyDefinitionBuilder<TSrc, TDst>, CompletePolicyDefinition<TSrc, TDst>>> ctxCreators = 
            new List<Func<PolicyDefinitionBuilder<TSrc, TDst>, CompletePolicyDefinition<TSrc, TDst>>>();

        private Func<DefaultPolicyDefinitionBuilderHead<TSrc, TDst>, CompletePolicyDefinition<TSrc, TDst>> defCreator;

        public ExceptionPolicyGroup<TSrc, TDst> Provide()
        {
            Define();
            return PolicyGroupBuilder.Create(defCreator, ctxCreators.ToArray());
        }

        public abstract void Define();

        protected void DefineDefaultPolicy([NotNull] Func<DefaultPolicyDefinitionBuilderHead<TSrc, TDst>, CompletePolicyDefinition<TSrc, TDst>> defPolicy)
        {
            if (defPolicy == null)
            {
                throw new ArgumentNullException(nameof(defPolicy));
            }
            if (defCreator != null)
            {
                throw new InvalidOperationException("The default policy has already been defined.");
            }

            defCreator = defPolicy;
        }

        protected void DefinePolicy(string context, [NotNull]
            Func<PolicyDefinitionBuilder<TSrc, TDst>, CompletePolicyDefinition<TSrc, TDst>> ctxPolicy)
        {
            if (ctxPolicy == null)
            {
                throw new ArgumentNullException(nameof(ctxPolicy));
            }
            ctxCreators.Add(ctxPolicy);   
        }
    }
}
