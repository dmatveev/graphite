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
    public class Vertex {
        protected Graphite.Core.Vertex _assignedVertex;
        public bool Selected { set; get; }

        public bool Marked {
            get {
                return _assignedVertex.marked();
            }
        }

        public Vertex (Graphite.Core.Vertex vertex) {
            _assignedVertex = vertex;
        }

        public Graphite.Core.Vertex AssignedTo {
            get {
                return _assignedVertex;
            }
        }

        public void Paint (Graphics g) {
            _assignedVertex.VertexShape.Render (this, g);
        }

        public bool IsUnder (Point pt) {
            return _assignedVertex.VertexShape.IsUnder (this, pt);
        }
    }

    public class Edge {
        public bool Selected { set; get; }

        public bool Marked {
            get {
                return _assignedEdge.marked();
            }
        }

        protected Graphite.Core.Edge _assignedEdge;
        
        public Edge (Graphite.Core.Edge edge) {
            _assignedEdge = edge;
        }

        public Graphite.Core.Edge AssignedTo {
            get {
                return _assignedEdge;
            }
        }

        public void Paint (Graphics g) {
            Pen blackPen = new Pen (Marked ?  Color.Red : Color.Black, 1);
            g.DrawLine (blackPen, _assignedEdge.From.Position, _assignedEdge.To.Position);
        }

        public bool IsUnder (Point pt) {
            var line = new Graphite.Math.LinePiece2D (_assignedEdge.From.Position,
                                                      _assignedEdge.To.Position);
            return line.HitTest (pt, 7.0);
        }
    }
}

namespace RefactorMePlease {
    struct EdgeSkeleton {
        public int IdTo;
        public int Weight;
    }
}

namespace Widgets {
    public class Scene: System.Windows.Forms.Control, Graphite.Core.IGraphView,
        Graphite.Abstract.IDisplay {
        protected List<Graphite.Scene.Elements.Vertex> _vertexVisuals;
        protected List<Graphite.Scene.Elements.Edge>   _edgeVisuals;
        protected Brush _whiteBrush;
        public void Reset () {
            _vertexVisuals = new List<Graphite.Scene.Elements.Vertex> ();
            _edgeVisuals   = new List<Graphite.Scene.Elements.Edge> ();
            _whiteBrush    = new SolidBrush (Color.White);
        }

        public void LayoutUpdated () {
            Refresh ();
        }

        public void LayoutRebuilt () {
            UpdateEdges ();
            Refresh ();
        }

        public void VertexAdded (Graphite.Core.Vertex v) {
            _vertexVisuals.Add (new Graphite.Scene.Elements.Vertex (v));
            Refresh ();
        }

        public void VertexRemoved (Graphite.Core.Vertex v) {
            var element = _vertexVisuals.First (x => x.AssignedTo == v);
            _vertexVisuals.Remove (element);
            Refresh ();
        }

        public void VertexConnected (Graphite.Core.Vertex a, Graphite.Core.Vertex b) {
            UpdateEdges ();
            Refresh ();
        }

        public void VertexDisconnected (Graphite.Core.Vertex a, Graphite.Core.Vertex b) {
            UpdateEdges ();
            Refresh ();
        }

        public Scene () {
            Reset ();
            DoubleBuffered = true;
            
            // TODO: Refactor edge selection on click
            Click += (obj, e) => TrySelectVertex (CurrentPoint ());
            Click += (obj, e) => TrySelectEdge   (CurrentPoint ());
        }

        public Point CurrentPoint () {
            return PointToClient (System.Windows.Forms.Cursor.Position);
        }

        public Graphite.Core.Vertex SelectedVertex () {
            foreach (Graphite.Scene.Elements.Vertex v in _vertexVisuals)
                if (v.Selected)
                    return v.AssignedTo;

            return null;
        }

        public Graphite.Core.Edge SelectedEdge () {
            foreach (Graphite.Scene.Elements.Edge e in _edgeVisuals)
                if (e.Selected)
                    return e.AssignedTo;

            return null;
        }

        protected override void OnPaint (PaintEventArgs e) {
            base.OnPaint(e);

            var rect = new Rectangle (0, 0, ClientSize.Width, ClientSize.Height);
            e.Graphics.FillRectangle (_whiteBrush, rect);

            foreach (Graphite.Scene.Elements.Edge edge in _edgeVisuals)
                edge.Paint (e.Graphics);
            
            foreach (Graphite.Scene.Elements.Vertex visual in _vertexVisuals)
                visual.Paint (e.Graphics);
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

        protected void UpdateEdges () {
            _edgeVisuals = new List<Graphite.Scene.Elements.Edge>();
            
            foreach (Graphite.Scene.Elements.Vertex ve in _vertexVisuals)
                foreach (Graphite.Core.Edge edge in ve.AssignedTo.Edges ())
                    if (!_edgeVisuals.Any (x => x.AssignedTo.Matches (edge)))
                        _edgeVisuals.Add (new Graphite.Scene.Elements.Edge (edge));
        }

        // IDisplay implementation
        public void update () {
            Refresh ();
        }
    }
}
