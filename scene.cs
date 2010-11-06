using System;
using System.Drawing;
using System.Collections.Generic;
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
            int x = _position.X;
            int y = _position.Y;
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
}

namespace Widgets {
    public class Scene: System.Windows.Forms.Control {
        protected List<Graphite.Scene.Elements.Vertex> _vertexVisuals;

        public Scene () {
            _vertexVisuals = new List<Graphite.Scene.Elements.Vertex>();
            DoubleBuffered = true;
        }

        protected override void OnPaint (PaintEventArgs e) {
            base.OnPaint(e);

            foreach (Graphite.Scene.Elements.Vertex visual in _vertexVisuals) {
                visual.Paint (e.Graphics);
            }
        } 

        public void AddVertex (Graphite.Core.Vertex v, Point pos) {
            _vertexVisuals.Add (new Graphite.Scene.Elements.Vertex (v, pos));
            Refresh ();
        }

        public void TrySelectVertex (Point pos) {
            bool found = false;
            foreach (Graphite.Scene.Elements.Vertex visual in _vertexVisuals) {
                if (!found && visual.IsUnder (pos)) {
                    visual.Selected = true;
                    found = true;
                }
                else {
                    visual.Selected = false;
                }
            }
            Refresh ();
        }
    }
}
