using System;
using System.Drawing;
using System.Collections.Generic;

namespace Visuals {
    public class Vertex {
        public Graphite.Core.Vertex DomainVertex {get; protected set;}
        public Point Position;
        protected const int radius = 10;
        protected const int border = 2;
        
        public Vertex (Graphite.Core.Vertex vertex, Point pos) {
            DomainVertex = vertex;
            Position = pos;
        }

        public void Connect (Visuals.Vertex v, int weight = 0) {
            DomainVertex.Connect (v.DomainVertex, weight);
        }

        public void Disconnect (Visuals.Vertex v) {
            DomainVertex.Disconnect (v.DomainVertex);
        }

        public bool IsAssignedTo (Graphite.Core.Vertex v) {
            return DomainVertex == v;
        }

        public Graphite.Core.Edge[] Connections () {
            return DomainVertex.Edges ();
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
