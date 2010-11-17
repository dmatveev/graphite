using System;
using System.Drawing;
using System.Collections.Generic;

namespace Visuals {
    public class Vertex {
        protected Graphite.Core.Vertex _vertex;
        public Point Position;
        protected const int radius = 10;
        protected const int border = 2;
        
        public Vertex (Graphite.Core.Vertex vertex, Point pos) {
            _vertex = vertex;
            Position = pos;
        }

        public void Connect (Visuals.Vertex v, int weight = 0) {
            _vertex.Connect (v._vertex, weight);
        }

        public bool IsAssignedTo (Graphite.Core.Vertex v) {
            return _vertex == v;
        }

        public Graphite.Core.Edge[] Connections () {
            return _vertex.Edges ();
        }
        
        public static implicit operator string (Vertex v) {
            return String.Format ("Vertex {0} at {1}", v, v.Position);
        }
    }

    public class Edge {
        public Visuals.Vertex First {get; protected set;}
        public Visuals.Vertex Second {get; protected set;}

        public Edge (Visuals.Vertex first, Visuals.Vertex second) {
            First = first;
            Second = second;
        }
    }
}
