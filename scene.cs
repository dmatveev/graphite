using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Windows.Forms;

namespace Graphite.Math {
    public class Line2D {
        protected double _k;
        protected double _b;

        public Line2D (Point a, Point b) {
            _k = (double) (a.Y - b.Y) / (a.X - b.X);
            _b = (a.Y - _k * a.X);
        }

        public double function (double x) {
            return _k * x + _b;
        }

        public virtual bool HitTest (Point p, double precision) {
            return System.Math.Abs (p.Y - function (p.X)) <= precision;
        }
    }

    public class LinePiece2D: Line2D {
        protected double _left;
        protected double _right;

        public LinePiece2D (Point a, Point b) : base (a, b) {
            if (a.X <= b.X) {
                _left = a.X;
                _right = b.X;
            } else {
                _left = b.X;
                _right = a.X;
            }
        }

        public override bool HitTest (Point p, double precision) {
            return base.HitTest (p, precision) && p.X >= _left && p.X <= _right;
        }
    }
}

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
            double r = System.Math.Sqrt (System.Math.Pow ((double) Position.X - pt.X, 2) +
                                         System.Math.Pow ((double) Position.Y - pt.Y, 2));
            return r < radius;
        }
    }

    public class Edge: Visuals.Edge, IElement {
        public bool Selected { set; get; }
        
        public Edge (Visuals.Vertex first, Visuals.Vertex second) : base (first, second) {
        }

        public void Paint (Graphics g) {
            Pen blackPen = new Pen (Color.Black, 1);
            g.DrawLine (blackPen, First.Position, Second.Position);
        }

        public bool IsUnder (Point pt) {
            var line = new Graphite.Math.LinePiece2D (First.Position, Second.Position);
            return line.HitTest (pt, 7.0);
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
        protected List<Graphite.Scene.Elements.LazyEdge> _edgeVisuals;

        public Scene () {
            _vertexVisuals = new List<Graphite.Scene.Elements.Vertex>();
            _edgeVisuals = new List<Graphite.Scene.Elements.LazyEdge>();
            DoubleBuffered = true;
        }

        // TODO: Refactoring
        public Visuals.Vertex SelectedVertex {
            get {
                foreach (Graphite.Scene.Elements.Vertex v in _vertexVisuals)
                    if (v.Selected)
                        return v;

                return null;
            }
        }

        // TODO: Refactoring
        public Visuals.Edge SelectedEdge {
            get {
                foreach (Graphite.Scene.Elements.Edge e in _edgeVisuals)
                    if (e.Selected)
                        return e;

                return null;
            }
        }

        protected override void OnPaint (PaintEventArgs e) {
            base.OnPaint(e);

            foreach (Graphite.Scene.Elements.LazyEdge edge in _edgeVisuals)
                edge.Paint (e.Graphics);
            
            foreach (Graphite.Scene.Elements.Vertex visual in _vertexVisuals)
                visual.Paint (e.Graphics);
        } 

        public void AddVertex (Graphite.Core.Vertex v, Point pos) {
            _vertexVisuals.Add (new Graphite.Scene.Elements.Vertex (v, pos));
            Refresh ();
        }

        public void DeleteVertex (Visuals.Vertex v) {
            foreach (Graphite.Core.Edge edge in v.Connections ()) {
                Graphite.Core.Vertex to = edge.To;
                v.DomainVertex.Disconnect (to);
                to.Disconnect (v.DomainVertex);
            }
            _vertexVisuals.Remove (v as Graphite.Scene.Elements.Vertex);
            UpdateEdges ();
            Refresh ();
        }

        // TODO: Refactoring
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

        // TODO: Refactoring
        public void TrySelectEdge (Point pos) {
            bool found = false;
            foreach (Graphite.Scene.Elements.Edge visual in _edgeVisuals)
                if (!found && visual.IsUnder (pos)) {
                    visual.Selected = true;
                    found = true;
                }
                else
                    visual.Selected = false;
            
            Refresh ();
        }

        public void ConnectVertexes (Visuals.Vertex a, Visuals.Vertex b) {
            a.Connect (b);
            b.Connect (a);
            UpdateEdges ();
            Refresh ();
        }

        protected void UpdateEdges () {
            _edgeVisuals = new List<Graphite.Scene.Elements.LazyEdge>();
         
            foreach (Graphite.Scene.Elements.Vertex visual in _vertexVisuals) {
                foreach (Graphite.Core.Edge edge in visual.Connections ()) {
                    Graphite.Scene.Elements.LazyEdge lazyEdge = null;

                    if (_edgeVisuals.Any (x => x.Matches (edge.To, visual))) {
                        lazyEdge = _edgeVisuals.First (x => x.Matches (edge.To, visual));
                        lazyEdge.Complete (visual);
                    }
                    else {
                        lazyEdge = new Graphite.Scene.Elements.LazyEdge (visual, edge.To);
                        _edgeVisuals.Add (lazyEdge);
                    }
                }
            }
        }

        public void DisconnectVertexes (Visuals.Vertex a, Visuals.Vertex b) {
            a.Disconnect (b);
            b.Disconnect (a);
            UpdateEdges ();
            Refresh ();
        }
    }
}
