namespace Graphite.Core {
    public interface IDocument {
        // TODO: Refactoring
        Edge   SelectedEdge ();
        Vertex SelectedVertex ();
        void ConnectVertexes    (Vertex a, Vertex b);
        void DisconnectVertexes (Vertex a, Vertex b);
        void CreateVertex ();
        void DeleteVertex (Vertex v);
        void MoveVertex (Vertex v);
        void SelectEdge ();
        void SelectVertex ();
    }
}