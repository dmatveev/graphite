using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;

namespace Graphite.Core {
    public abstract class Shape {
        public Shape () {
        }

        public abstract void   Render  (object shouldBeVertex, object shouldBeCanvas);
        public abstract bool   IsUnder (object shouldBeVertex, Point pt);
        public abstract string name    ();
        public abstract string alias   ();
    }

    public class Edge {
        public Vertex From {get; protected set;}
        public Vertex To {get; protected set;}
        public int Weight {get; protected set;}
        
        public Edge (Vertex fromv, Vertex tov, int weight) {
            From = fromv;
            To = tov;
            Weight = weight;
        }

        public bool Connected (Vertex v) {
            return To == v;
        }

        public bool Matches (Edge e) {
            return e.From == this.To && e.To == this.From;
        }

        public static implicit operator string (Edge e) {
            return String.Format ("#{0}:{1}", e.To.Id, e.Weight);
        }

        public void Save (System.Xml.XmlWriter w) {
            w.WriteStartElement ("edge");
            w.WriteAttributeString ("to", To.Id.ToString());
            w.WriteAttributeString ("weight", Weight.ToString());
            w.WriteEndElement ();
        }
    }

    public class Vertex {
        public int Id {get; protected set;}
        public Point Position;
        public Shape VertexShape;

        protected List<Edge> _edges;

        public Vertex (int id, Point pos) {
            Id = id;
            Position = pos;
            _edges = new List<Edge>();
        }

        public void Connect (Vertex v, int weight = 0) {
            if (!Connected (v))
                _edges.Add (new Edge (this, v, weight));
        }

        public void Disconnect (Vertex v) {
            Edge e = _edges.First (x => x.Connected (v));
            _edges.Remove (e);
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

        public void Save (System.Xml.XmlWriter w) {
            w.WriteStartElement ("vertex");
            w.WriteAttributeString ("id", Id.ToString());
            w.WriteAttributeString ("shape", VertexShape.alias ());

            /* TODO: Introduce a Position class */ {
                w.WriteStartElement ("position");
                w.WriteAttributeString ("x", Position.X.ToString ());
                w.WriteAttributeString ("y", Position.Y.ToString ());
                w.WriteEndElement ();
            }
            
            foreach (Edge each in _edges)
                each.Save (w);
            
            w.WriteEndElement ();
        }
    }
}