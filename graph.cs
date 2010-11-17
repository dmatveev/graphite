using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Graphite.Core {
    public class Edge {
        public Vertex To {get; protected set;}
        public int Weight {get; protected set;}
        
        public Edge (Vertex to, int weight) {
            To = to;
            Weight = weight;
        }

        public bool Connected (Vertex v) {
            return To == v;
        }

        public static implicit operator string (Edge e) {
            return String.Format ("#{0}:{1}", e.To.Id, e.Weight);
        }
    }

    public class Vertex {
        public int Id {get; protected set;}
        protected List<Edge> _edges;

        public Vertex (int id = 0) {
            Id = id;
            _edges = new List<Edge>();
        }

        public void Connect (Vertex v, int weight = 0) {
            if (!Connected (v))
                _edges.Add (new Edge (v, weight));
        }

        public bool Connected (Vertex v) {
            return _edges.Any (x => x.Connected (v));
        }

        public Edge[] Edges () {
            return _edges.ToArray();
        }

        public static implicit operator string (Vertex v) {
            StringWriter writer = new StringWriter ();
            writer.Write ("Vertex #{0}: ", v.Id);

            foreach (Edge each in v._edges)
                writer.Write ("{0} ", (string) each);

            return writer.ToString();
        }
    }

    public class GraphModel {
        protected List<Vertex> _vertexes;
    }
}