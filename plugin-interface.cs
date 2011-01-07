namespace Graphite.Abstract {
    public interface IMarkable {
        void setMark (bool marked);
        bool marked ();
    }

    public interface IEdge: IMarkable {
        IVertex vfrom ();
        IVertex vto ();
     
        int weight ();
    }

    public interface IVertex: IMarkable {
        IEdge [] edges ();

        void   setPrivateData (object data);
        object getPrivateData ();
    }

    public interface IGraph {
        IVertex [] vertexes ();
    }

    public interface IDisplay {
        void update ();
    }
}

namespace Graphite.Modules {
    public interface IPlugin {
        string Name ();
        void   Run (Graphite.Abstract.IGraph g, Graphite.Abstract.IDisplay d);
    }
}
