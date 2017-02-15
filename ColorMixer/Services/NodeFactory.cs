using ColorMixer.Model;
using Splat;
using System.Diagnostics.CodeAnalysis;

namespace ColorMixer.Services
{
    public interface INodeFactory
    {
        INode Create<TNode>() where TNode : INode;
    }

    [ExcludeFromCodeCoverage]
    public class NodeFactory : INodeFactory
    {
        public INode Create<TNode>() where TNode : INode
            => Locator.Current.GetService<TNode>();
    }
}