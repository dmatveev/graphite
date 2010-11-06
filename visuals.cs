using System;
using System.Drawing;
using System.Collections.Generic;

namespace Visuals {
    public class Vertex {
        protected Graphite.Core.Vertex _vertex;
        protected Point _position;
        protected const int radius = 10;
        protected const int border = 2;
        
        public Vertex (Graphite.Core.Vertex vertex, Point pos) {
            _vertex = vertex;
            _position = pos;
        }

        public Point Position {
            get {
                return _position;
            }
        }
    }

    public class Edge {
        protected Visuals.Vertex _first;
        protected Visuals.Vertex _second;

        public Edge (Visuals.Vertex first, Visuals.Vertex second) {
            _first = first;
            _second = second;
        }
        
        public Visuals.Vertex First {
            get {
                return _first;
            }
        }

        public Visuals.Vertex Second {
            get {
                return _second;
            }
        }
    }
}
