using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;


namespace Graphite.Shapes {
    public abstract class WinFormsShape: Graphite.Core.Shape {
        public System.Drawing.Font Font;
        protected const int radius = 12;
        protected const int border = 2;

        public WinFormsShape () {
            Font = new System.Drawing.Font ("Arial", 9, FontStyle.Regular);
        }

        protected virtual Rectangle boundingBox (Graphite.Scene.Elements.Vertex v) {
            var vertex = (v as Graphite.Scene.Elements.Vertex).AssignedTo;
            return Graphite.Math.Geom.Square (vertex.Position, radius);
        }

        public System.Drawing.Color Color (Graphite.Scene.Elements.Vertex v) {
            return (v.Selected ? SystemColors.Highlight : System.Drawing.Color.Black);
        }

        protected void RenderId (Graphite.Scene.Elements.Vertex v, Graphics g) {
            var text = v.AssignedTo.Id.ToString ();
            TextRenderer.DrawText (g, text, Font,
                                   boundingBox(v), Color (v),
                                   TextFormatFlags.HorizontalCenter |
                                   TextFormatFlags.VerticalCenter);
        }
    }

    public class Circle: WinFormsShape {

        public override void Render (object shouldBeVertex, object shouldBeGraphics) {
            var v = shouldBeVertex as Graphite.Scene.Elements.Vertex;
            var g = shouldBeGraphics as Graphics;
            var p = new Pen (Color (v), border);
            var b = new SolidBrush (System.Drawing.Color.White);
            var r = boundingBox (v);

            g.DrawEllipse (p, r);
            g.FillEllipse (b, Rectangle.Inflate (r, -1, -1));
            RenderId (v, g);
        }

        public override bool IsUnder (object shouldBeVertex, Point pt) {
            var v = (shouldBeVertex as Graphite.Scene.Elements.Vertex).AssignedTo;
            return Graphite.Math.Geom.Distance (v.Position, pt) < radius;
        }

        public override string name () {
            return "Circle";
        }

        public override string alias () {
            return "circle";
        }
    }

    public class Square: WinFormsShape {
        public override void Render (object shouldBeVertex, object shouldBeGraphics) {
            var v = shouldBeVertex as Graphite.Scene.Elements.Vertex;
            var g = shouldBeGraphics as Graphics;
            var p = new Pen (Color (v), border);
            var b = new SolidBrush (System.Drawing.Color.White);
            g.DrawRectangle (p, boundingBox (v));
            g.FillRectangle (b, Graphite.Math.Geom.Square (v.AssignedTo.Position, radius - 1));
            RenderId (v, g);
        }

        public override bool IsUnder (object shouldBeVertex, Point pt) {
            var v = shouldBeVertex as Graphite.Scene.Elements.Vertex;
            return boundingBox (v).Contains (pt);
        }

        public override string name () {
            return "Square";
        }

        public override string alias () {
            return "square";
        }
    }

    public class Triangle: WinFormsShape {
        public override void Render (object shouldBeVertex, object shouldBeGraphics) {
            var v = shouldBeVertex as Graphite.Scene.Elements.Vertex;
            var g = shouldBeGraphics as Graphics;
            var p = new Pen (Color (v), border);
            var t = Graphite.Math.Geom.Triangle (v.AssignedTo.Position, radius);
            var i = Graphite.Math.Geom.Triangle (v.AssignedTo.Position, radius - 1);
            var b = new SolidBrush (System.Drawing.Color.White);

            g.DrawLine (p, t.Top, t.Left);
            g.DrawLine (p, t.Top, t.Right);
            g.DrawLine (p, t.Left, t.Right);
            g.FillPolygon (b, i.Vertexes);

            RenderId (v, g);
        }

        public override bool IsUnder (object shouldBeVertex, Point pt) {
            var v = shouldBeVertex as Graphite.Scene.Elements.Vertex;
            var t = Graphite.Math.Geom.Triangle (v.AssignedTo.Position, radius);

            if (pt.X < t.Left.X || pt.X > t.Right.X || pt.Y < t.Top.Y || pt.Y > t.Left.Y)
                return false;
            
            int xin = pt.X - t.Left.X;
            var line = xin < (t.Side() / 2)
                ? new Graphite.Math.Line2D (t.Left, t.Top)
                : new Graphite.Math.Line2D (t.Top, t.Right);

            return pt.Y > line.function (pt.X);
        }

        protected override Rectangle boundingBox (Graphite.Scene.Elements.Vertex v) {
            var r = base.boundingBox (v);
            r.Offset (0, radius / 3);
            return r;
        }

        public override string name () {
            return "Triangle";
        }

        public override string alias () {
            return "triangle";
        }
    }

    public class Manager {
        protected List<WinFormsShape> _shapes;
        static protected Manager _instance;

        static public Manager instance () {
            if (_instance == null)
                _instance = new Manager();

            return _instance;
        }

        public Manager () {
            _shapes = new List<WinFormsShape> ();
            _shapes.Add (new Circle ());
            _shapes.Add (new Square ());
            _shapes.Add (new Triangle ());
        }
        
        public WinFormsShape[] Shapes {
            get {
                return _shapes.ToArray ();
            }
        }

        public WinFormsShape FromAlias (string alias) {
            return _shapes.First (x => x.alias() == alias);
        }
    }
}