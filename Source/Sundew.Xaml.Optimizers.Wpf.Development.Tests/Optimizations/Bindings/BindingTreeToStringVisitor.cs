using System.Collections.Generic;
using System.Text;
using Sundew.Base.Text;
using Sundew.Base.Visiting;
using Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.Xaml;

namespace Sundew.Xaml.UnitTests.Optimizations.Bindings
{
    internal class BindingTreeToStringVisitor : IBindingWalker<StringBuilder, int, string>
    {
        public string Visit(IBindingNode bindingNode, StringBuilder stringBuilder, int indent)
        {
            stringBuilder ??= new StringBuilder();
            bindingNode.Visit(this, stringBuilder, indent);
            return stringBuilder.ToString();
        }

        public void VisitUnknown(IBindingNode bindingNode, StringBuilder stringBuilder, int indent)
        {
            throw VisitException.Create(bindingNode, stringBuilder, indent);
        }

        public void BindingTree(BindingTree bindingTree, StringBuilder stringBuilder, int indent)
        {
            foreach (var bindingContainer in bindingTree.BindingRoots)
            {
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.AppendLine();
                }

                bindingContainer.Visit(this, stringBuilder, indent);
            }
        }

        public void BindingRoot(BindingRootNode bindingRootNode, StringBuilder stringBuilder, int indent)
        {
            PrivateVisit(stringBuilder, indent, bindingRootNode, bindingRootNode.Bindings);
        }

        public void Binding(BindingNode bindingNode, StringBuilder stringBuilder, int indent)
        {
            stringBuilder.AppendLine($"{' '.Repeat(indent * 2)}{bindingNode}");
        }

        public void DataContextTargetBinding(DataContextTargetBindingNode dataContextTargetBindingNode, StringBuilder stringBuilder, int indent)
        {
            PrivateVisit(stringBuilder, indent, dataContextTargetBindingNode, dataContextTargetBindingNode.Bindings);
        }

        public void CastDataContextSourceBinding(CastDataContextBindingSourceNode castSourceBinding,
            StringBuilder stringBuilder,
            int indent)
        {
            PrivateVisit(stringBuilder, indent, castSourceBinding, castSourceBinding.Bindings);
        }

        public void ControlTemplateCastDataContextBindingSource(
            ControlTemplateCastDataContextBindingSourceNode controlTemplateCastDataContextBindingSourceNode,
            StringBuilder stringBuilder,
            int indent)
        {
            PrivateVisit(stringBuilder, indent, controlTemplateCastDataContextBindingSourceNode, controlTemplateCastDataContextBindingSourceNode.Bindings);
        }

        public void DataTemplateCastDataContextBindingSource(
            DataTemplateCastDataContextBindingSourceNode dataTemplateCastDataContextBindingSourceNode,
            StringBuilder stringBuilder,
            int indent)
        {
            PrivateVisit(stringBuilder, indent, dataTemplateCastDataContextBindingSourceNode, dataTemplateCastDataContextBindingSourceNode.Bindings);
        }

        public void ElementBindingSource(ElementBindingSourceNode elementBindingSourceNode, StringBuilder stringBuilder, int indent)
        {
            PrivateVisit(stringBuilder, indent, elementBindingSourceNode, elementBindingSourceNode.Bindings);
        }

        private void PrivateVisit(StringBuilder stringBuilder, int indent, object text, IReadOnlyList<IBinding> bindings)
        {
            stringBuilder.AppendLine($"{' '.Repeat(indent * 2)}{text}");
            indent++;
            foreach (var binding in bindings)
            {
                binding.Visit(this, stringBuilder, indent);
            }
        }
    }
}