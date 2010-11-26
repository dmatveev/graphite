namespace Graphite.Core {
    public interface IDocument {
        // TODO: Refactoring
        Visuals.Edge   SelectedEdge ();
        Visuals.Vertex SelectedVertex ();
        void ConnectVertexes    (Visuals.Vertex a, Visuals.Vertex b);
        void DisconnectVertexes (Visuals.Vertex a, Visuals.Vertex b);
        void CreateVertex ();
        void DeleteVertex (Visuals.Vertex v);
        void MoveVertex (Visuals.Vertex v);
        void SelectEdge ();
        void SelectVertex ();
    }
}