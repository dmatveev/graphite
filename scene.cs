using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Windows.Forms;

namespace Graphite.Scene.Elements {
    public interface IElement {
        void Paint   (Graphics g);
        bool IsUnder (Point pt);
    }

    public class Vertex: Visuals.Vertex, IElement {
        public bool Selected { set; get; }

        public Vertex (Graphite.Core.Vertex vertex, Point pos) : base (vertex, pos) {
        }

        public void Paint (Graphics g) {
            int x = Position.X;
            int y = Position.Y;
            Rectangle rect = new Rectangle (x - radius,
                                            y - radius,
                                            2 * radius,
                                            2 * radius);
            Pen blackPen = new Pen (Selected ? SystemColors.Highlight : Color.Black, border);
            g.DrawEllipse (blackPen, rect);
        }

        public bool IsUnder (Point pt) {
            double r = Math.Sqrt (Math.Pow ((double) Position.X - pt.X, 2) +
                                  Math.Pow ((double) Position.Y - pt.Y, 2));
            return r < radius;
        }
    }

    public class Edge: Visuals.Edge, IElement {
        public Edge (Visuals.Vertex first, Visuals.Vertex second) : base (first, second) {
        }

        public void Paint (Graphics g) {
            Pen blackPen = new Pen (Color.Black, 1);
            g.DrawLine (blackPen, First.Position, Second.Position);
        }

        public bool IsUnder (Point pt) {
            return false;
        }
    }

    public class LazyEdge: Edge {
        protected Core.Vertex _domainVertex;

        public LazyEdge (Visuals.Vertex first, Core.Vertex second) : base (first, null) {
            _domainVertex = second;
        }

        public void Complete (Visuals.Vertex second) {
            Second = second;
        }

        public bool Matches (Core.Vertex known, Visuals.Vertex unknown) {
            return First.IsAssignedTo (known) && unknown.IsAssignedTo (_domainVertex);
        }

        public static implicit operator string (LazyEdge le) {
            return String.Format ("LazyEdge 1st: {0}, 2nd: {1}, domain: {2}",
                                  (string) le.First, (string) le.Second, le._domainVertex);
        }
    }
}

namespace Widgets {
    public class Scene: System.Windows.Forms.Control {
        protected List<Graphite.Scene.Elements.Vertex> _vertexVisuals;

        public Scene () {
            _vertexVisuals = new List<Graphite.Scene.Elements.Vertex>();
            DoubleBuffered = true;
        }

        public Visuals.Vertex SelectedVertex {
            get {
                foreach (Graphite.Scene.Elements.Vertex v in _vertexVisuals)
                    if (v.Selected)
                        return v;

                return null;
            }
        }

        protected override void OnPaint (PaintEventArgs e) {
            base.OnPaint(e);

            var edges = new List<Graphite.Scene.Elements.LazyEdge>();
            foreach (Graphite.Scene.Elements.Vertex visual in _vertexVisuals) {
                foreach (Graphite.Core.Edge edge in visual.Connections ()) {
                    Graphite.Scene.Elements.LazyEdge lazyEdge = null;

                    if (edges.Any (x => x.Matches (edge.To, visual))) {
                        lazyEdge = edges.First (x => x.Matches (edge.To, visual));
                        lazyEdge.Complete (visual);
                    }
                    else {
                        lazyEdge = new Graphite.Scene.Elements.LazyEdge (visual, edge.To);
                        edges.Add (lazyEdge);
                    }
                }
            }
            foreach (Graphite.Scene.Elements.LazyEdge edge in edges)
                edge.Paint (e.Graphics);
            
            foreach (Graphite.Scene.Elements.Vertex visual in _vertexVisuals)
                visual.Paint (e.Graphics);
        } 

        public void AddVertex (Graphite.Core.Vertex v, Point pos) {
            _vertexVisuals.Add (new Graphite.Scene.Elements.Vertex (v, pos));
            Refresh ();
        }

        public void TrySelectVertex (Point pos) {
            bool found = false;
            foreach (Graphite.Scene.Elements.Vertex visual in _vertexVisuals)
                if (!found && visual.IsUnder (pos)) {
                    visual.Selected = true;
                    found = true;
                }
                else
                    visual.Selected = false;
            
            Refresh ();
        }
    }
}
