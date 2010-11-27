using System;
using System.Drawing;
using System.Collections.Generic;

namespace Graphite.Shapes {
    public class Circle: Graphite.Core.Shape {
        protected const int radius = 10;
        protected const int border = 2;

        public override void Render (object shouldBeVertex, object shouldBeGraphics) {
            var v = shouldBeVertex as Graphite.Scene.Elements.Vertex;
            var g = shouldBeGraphics as Graphics;

            int x = v.AssignedTo.Position.X;
            int y = v.AssignedTo.Position.Y;
            var rect     = new Rectangle (x - radius, y - radius, 2 * radius, 2 * radius);
            var blackPen = new Pen (v.Selected ? SystemColors.Highlight : Color.Black, border);
            g.DrawEllipse (blackPen, rect);
        }

        public override bool IsUnder (object shouldBeVertex, Point pt) {
            var    v = shouldBeVertex as Graphite.Scene.Elements.Vertex;
            double x = v.AssignedTo.Position.X;
            double y = v.AssignedTo.Position.Y;
            double r = System.Math.Sqrt (System.Math.Pow (x - pt.X, 2) +
                                         System.Math.Pow (y - pt.Y, 2));
            return r < radius;
        }

        public override string name () {
            return "Circle";
        }

        public override string alias () {
            return "circle";
        }
    }

    public class Square: Graphite.Core.Shape {
        protected const int hside  = 10;
        protected const int border = 2;

        protected Rectangle boundingBox (object shouldBeVertex) {
            var v = shouldBeVertex as Graphite.Scene.Elements.Vertex;
            int x = v.AssignedTo.Position.X;
            int y = v.AssignedTo.Position.Y;
            var r = new Rectangle (x - hside, y - hside, 2 * hside, 2 * hside);
            return r;
        }

        public override void Render (object shouldBeVertex, object shouldBeGraphics) {
            var v = shouldBeVertex as Graphite.Scene.Elements.Vertex;
            var g = shouldBeGraphics as Graphics;
            var r = boundingBox (shouldBeVertex);
            var p = new Pen (v.Selected ? SystemColors.Highlight : Color.Black, border);
            g.DrawRectangle (p, r);
        }

        public override bool IsUnder (object shouldBeVertex, Point pt) {
            return boundingBox (shouldBeVertex).Contains (pt);
        }

        public override string name () {
            return "Square";
        }

        public override string alias () {
            return "square";
        }
    }

    public class Triangle: Graphite.Core.Shape {
        protected const int radius  = 10;
        protected const int border  = 2;

        public override void Render (object shouldBeVertex, object shouldBeGraphics) {
            var v = shouldBeVertex as Graphite.Scene.Elements.Vertex;
            var g = shouldBeGraphics as Graphics;

            int side = (int) ((double) (2 * radius) / 0.866); // cos 30

            Point center = v.AssignedTo.Position;
            Point top    = new Point (center.X, center.Y - radius);
            Point left   = new Point (center.X - side / 2, center.Y + radius);
            Point right  = new Point (center.X + side / 2, center.Y + radius);

            var p = new Pen (v.Selected ? SystemColors.Highlight : Color.Black, border);

            g.DrawLine (p, top, left);
            g.DrawLine (p, top, right);
            g.DrawLine (p, left, right);
        }

        public override bool IsUnder (object shouldBeVertex, Point pt) {
            var v = shouldBeVertex as Graphite.Scene.Elements.Vertex;

            int side = (int) ((double) (2 * radius) / 0.866); // cos 30

            Point center = v.AssignedTo.Position;
            Point top    = new Point (center.X, center.Y - radius);
            Point left   = new Point (center.X - side / 2, center.Y + radius);
            Point right  = new Point (center.X + side / 2, center.Y + radius);

            if (pt.X < left.X || pt.X > right.X || pt.Y < top.Y || pt.Y > left.Y)
                return false;
            
            int xin = pt.X - left.X;
            var line = xin < (side / 2)
                ? new Graphite.Math.Line2D (left, top)
                : new Graphite.Math.Line2D (top, right);

            return pt.Y > line.function (pt.X);
        }

        public override string name () {
            return "Triangle";
        }

        public override string alias () {
            return "triangle";
        }
    }

    public class Manager {
        protected List<Graphite.Core.Shape> _shapes;

        public Manager () {
            _shapes = new List<Graphite.Core.Shape> ();
            _shapes.Add (new Circle ());
            _shapes.Add (new Square ());
            _shapes.Add (new Triangle ());
        }
        
        public Graphite.Core.Shape[] Shapes {
            get {
                return _shapes.ToArray ();
            }
        }
    }
}