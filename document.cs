namespace Graphite.Core {
    public interface IDocument {
        void CreateVertex ();
        void SelectVertex ();
        void ConnectVertexes (Visuals.Vertex a, Visuals.Vertex b);
        Visuals.Vertex SelectedVertex ();
    }
}