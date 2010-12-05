using System.Drawing;

namespace Graphite.Core {
    public interface IGraphView {
        System.Drawing.Point CurrentPoint   ();
        Graphite.Core.Vertex SelectedVertex ();
        Graphite.Core.Edge   SelectedEdge   ();

        void LayoutUpdated      ();
        void VertexAdded        (Vertex v);
        void VertexRemoved      (Vertex v);
        void VertexConnected    (Vertex a, Vertex b);
        void VertexDisconnected (Vertex a, Vertex b);

        void TrySelectVertex (Point pos);
        void TrySelectEdge   (Point pos);
    }

    public interface IShapeSelector {
        Graphite.Core.Shape  SelectedShape ();
    }

    public interface IUISet {
        IGraphView     graphView     ();
        IShapeSelector shapeSelector ();
    }
}
