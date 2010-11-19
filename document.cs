namespace Graphite.Core {
    public interface IDocument {
        void CreateVertex ();
        void SelectVertex ();
        void DeleteVertex (Visuals.Vertex v);
        void MoveVertex (Visuals.Vertex v);
        void ConnectVertexes (Visuals.Vertex a, Visuals.Vertex b);
        Visuals.Vertex SelectedVertex ();
    }
}