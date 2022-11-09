using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace FluentApi.Graph
{
    public class DotGraphBuilder : IAdd<NodeAttributes>, IAdd<EdgeAttributes>
    {
        private Graph Graph { get; set; }
        private static readonly DotFormatWriter Writer = new DotFormatWriter(new StringWriter());
        private DotGraphBuilder() { }
        
        public string Build()
        {
            Writer.Write(Graph);
            return Graph.ToDotFormat();
        }
        
        public static IAddWith DirectedGraph(string graphName)
        {
            return new DotGraphBuilder
            {
                Graph = new Graph(graphName, true, false)
            };
        }
        
        public static IAddWith UndirectedGraph(string graphName)
        {
            return new DotGraphBuilder
            {
                Graph = new Graph(graphName, false, false)
            };
        }

        public IAdd<NodeAttributes> AddNode(string nodeName)
        {
            Graph.AddNode(nodeName);
            return this;
        }

        public IAdd<EdgeAttributes> AddEdge(string from, string to)
        {
            Graph.AddEdge(from, to);
            return this;
        }
        
        public IAddWith With(Func<NodeAttributes, NodeAttributes> func)
        {
            var att = func(new NodeAttributes());
            foreach (var attribute in att.SavedAttributes)
                Graph.Nodes.Last().Attributes.Add(attribute.Key, attribute.Value);
            return this;
        }
        
        public IAddWith With(Func<EdgeAttributes, EdgeAttributes> func)
        {
            var att = func(new EdgeAttributes());
            foreach (var attribute in att.SavedAttributes)
                Graph.Edges.Last().Attributes.Add(attribute.Key, attribute.Value);
            return this;
        }

        public IAddWith With(Action<NodeAttributes> action) => this;
        public IAddWith With(Action<EdgeAttributes> action) => this;
    }
    
    public interface IAddWith
    {
        IAdd<NodeAttributes> AddNode(string nodeName);
        IAdd<EdgeAttributes> AddEdge(string from, string to);
        string Build();
    }

    public interface IAdd<T> : IAddWith where T : class
    {
        IAddWith With(Func<T, T> func);
        IAddWith With(Action<T> action);
    }
    
    public abstract class Attributes<T> where T : class
    {
        public readonly Dictionary<string, string> SavedAttributes = new Dictionary<string, string>();

        public T Color(string color)
        {
            SaveAttribute("color", color);
            return this as T;
        }

        public T FontSize(int fontSize)
        {
            SaveAttribute("fontsize", fontSize.ToString());
            return this as T;
        }
		
        public T Label(string label)
        {
            SaveAttribute("label", label);
            return this as T;
        }

        protected void SaveAttribute(string name, string value)
        {
            if (SavedAttributes.ContainsKey(name))
                SavedAttributes[name] = value;
            else SavedAttributes.Add(name, value);
        }
    }
    
    public class NodeAttributes : Attributes<NodeAttributes>
    {
        public NodeAttributes Shape(string shape)
        {
            SaveAttribute("shape", shape);
            return this;
        }
    }
    
    public class EdgeAttributes : Attributes<EdgeAttributes>
    {
        public EdgeAttributes Weight(double weight)
        {
            SaveAttribute("weight", weight.ToString(CultureInfo.InvariantCulture));
            return this;
        }
    }
    
    public static class NodeShape
    {
        public const string Box = "box";
        public const string Ellipse = "ellipse";
    }
}