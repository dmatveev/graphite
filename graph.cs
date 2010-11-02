using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Core {
    class Edge {
        protected Vertex _to;
        protected int _weight;
        
        public Edge (Vertex to, int weight) {
            _to = to;
            _weight = weight;
        }

        public bool Connected (Vertex v) {
            return _to == v;
        }

        public int Weight {
            get {
                return _weight;
            }
        }

        public Vertex To {
            get {
                return _to;
            }
        }

        public static implicit operator string (Edge e) {
            return String.Format ("#{0}:{1}", e._to.Id, e._weight);
        }
    }

    class Vertex {
        protected int _id;
        protected List<Edge> _edges;

        public Vertex (int id = 0) {
            _id = id;
            _edges = new List<Edge>();
        }

        public void Connect (Vertex v, int weight = 0) {
            if (!Connected (v)) {
                _edges.Add (new Edge (v, weight));
            }
        }

        public bool Connected (Vertex v) {
            return _edges.Any (x => x.Connected (v));
        }

        public Edge[] Edges () {
            return _edges.ToArray();
        }

        public int Id {
            get {
                return _id;
            }
        }

        public static implicit operator string (Vertex v) {
            StringWriter writer = new StringWriter ();
            writer.Write ("Vertex #{0}: ", v._id);

            foreach (Edge each in v._edges) {
                writer.Write ("{0} ", (string) each);
            }
            return writer.ToString();
        }
    }
}