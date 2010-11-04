using System;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace Visuals {
    public class Vertex {
        protected Core.Vertex _vertex;
        protected Point _position;
        protected const int radius = 10;
        protected const int border = 2;
        
        public Vertex (Core.Vertex vertex, Point pos) {
            _vertex = vertex;
            _position = pos;
        }

        public void Paint (Graphics g) {
            int x = _position.X;
            int y = _position.Y;
            Rectangle rect = new Rectangle (x - radius,
                                            y - radius,
                                            2 * radius,
                                            2 * radius);
            Pen blackPen = new Pen (Color.Black, border);
            g.DrawEllipse (blackPen, rect);
        }
    }
}

namespace Widgets {
    public class Scene: System.Windows.Forms.Control {
        protected List<Visuals.Vertex> _vertexVisuals;

        public Scene () {
            _vertexVisuals = new List<Visuals.Vertex>();
            DoubleBuffered = true;
        }

        protected override void OnPaint (PaintEventArgs e) {
            base.OnPaint(e);

            SolidBrush whiteBrush = new SolidBrush(Color.White);
            e.Graphics.FillRectangle (whiteBrush, ClientRectangle);

            foreach (Visuals.Vertex visual in _vertexVisuals) {
                visual.Paint (e.Graphics);
            }
        } 

        public void AddVertex (Visuals.Vertex v) {
            _vertexVisuals.Add (v);
            Refresh ();
        }
    }
}


