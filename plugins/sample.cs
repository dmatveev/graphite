using Graphite.Abstract;

namespace Graphite.Sample {
    public class Plugin: Graphite.Modules.IPlugin {
        public string Name () {
            return "Sample";
        }

        public void Run (IGraph g, IDisplay d) {
            IVertex v = g.vertexes()[0];
            v.setMark (true);
            foreach (IEdge e in v.edges()) {
                e.setMark (true);
                e.vto().setMark (true);
                d.update();
            }
            System.Console.WriteLine ("I am ok!");
        }
    }
}