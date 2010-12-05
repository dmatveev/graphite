using System;
using System.Drawing;

namespace Graphite.Math {
    public struct IdealTriangle {
        public Point Top;
        public Point Left;
        public Point Right;

        public int Side () {
            return Right.X - Left.X;
        }

        public Point[] Vertexes {
            get {
                return new Point[] {Top, Left, Right};
            }
        }
    }

    public class Geom {
        public static double Distance (Point ptFrom, Point ptTo) {
            return System.Math.Sqrt (System.Math.Pow (ptFrom.X - ptTo.X, 2) +
                                     System.Math.Pow (ptFrom.Y - ptTo.Y, 2));
        }

        public static Rectangle Square (Point cntr, int radius) {
            return new Rectangle (cntr.X - radius, cntr.Y - radius, 2 * radius, 2 * radius);
        }

        public static IdealTriangle Triangle (Point cntr, int radius) {
            int side = (int) ((double) (2 * radius) / 0.866); // cos 30
            var rslt = new IdealTriangle ();

            rslt.Top   = new Point (cntr.X, cntr.Y - radius);
            rslt.Left  = new Point (cntr.X - side / 2, cntr.Y + radius);
            rslt.Right = new Point (cntr.X + side / 2, cntr.Y + radius);
            return rslt;
        }
    }
}