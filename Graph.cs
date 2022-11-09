using System.Collections.Generic;

namespace FluentApi.Graph
{
    public class Graph
    {
        private readonly List<GraphEdge> _edges = new List<GraphEdge>();
        private readonly Dictionary<string, GraphNode> _nodes = new Dictionary<string, GraphNode>();

        public Graph(string graphName, bool directed, bool strict)
        {
            GraphName = graphName;
            Directed = directed;
            Strict = strict;
        }

        public string GraphName { get; }
        public bool Directed { get; }
        public bool Strict { get; }

        public IEnumerable<GraphEdge> Edges => _edges;
        public IEnumerable<GraphNode> Nodes => _nodes.Values;

        public GraphNode AddNode(string name)
        {
            if (!_nodes.TryGetValue(name, out var result))
                _nodes.Add(name, result = new GraphNode(name));
            return result;
        }

        public GraphEdge AddEdge(string sourceNode, string destinationNode)
        {
            var result = new GraphEdge(sourceNode, destinationNode, Directed);
            _edges.Add(result);
            return result;
        }
    }
}