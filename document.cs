using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;

namespace Graphite.Core {
    public delegate void LayoutUpdatedHandler      ();
    public delegate void LayoutRebuiltHandler      ();
    public delegate void VertexAddedHandler        (Vertex v);
    public delegate void VertexRemovedHandler      (Vertex v);
    public delegate void VertexConnectedHandler    (Vertex a, Vertex b);
    public delegate void VertexDisconnectedHandler (Vertex a, Vertex b);

    public class Document: Graphite.Abstract.IGraph {
        List<Graphite.Core.Vertex> _vertexes;
        int _counter;

        public Document () {
            _vertexes = new List<Graphite.Core.Vertex> ();
            _counter  = 0;
        }
        
        // Messages
        public void AddVertex (Vertex v) {
            _vertexes.Add (v);

            if (VertexAdded != null)
                VertexAdded (v);
        }

        public void CreateVertex (Point pt, Shape sh) {
            var vertex = new Graphite.Core.Vertex (++_counter, pt);
            vertex.VertexShape = sh;
            AddVertex (vertex);
        }

        public void DeleteVertex (Vertex v) {
            foreach (Graphite.Core.Edge edge in v.Edges()) {
                DisconnectVertex (v, edge.To);
                DisconnectVertex (edge.To, v);
            }
            
            if (VertexRemoved != null)
                VertexRemoved (v);

            _vertexes.Remove (v);
        }

        public void ConnectVertex (Vertex a, Vertex b, int weight) {
            a.Connect (b, weight);

            if (VertexConnected != null)
                VertexConnected (a, b);
        }

        public void DisconnectVertex (Vertex a, Vertex b) {
            a.Disconnect (b);

            if (VertexDisconnected != null)
                VertexDisconnected (a, b);
        }
        
        public void MoveVertex (Vertex v, Point pt) {
            v.Position = pt;

            if (LayoutUpdated != null)
                LayoutUpdated ();
        }

        // Events
        public event LayoutUpdatedHandler      LayoutUpdated;
        public event LayoutRebuiltHandler      LayoutRebuilt;
        public event VertexAddedHandler        VertexAdded;
        public event VertexRemovedHandler      VertexRemoved;
        public event VertexConnectedHandler    VertexConnected;
        public event VertexDisconnectedHandler VertexDisconnected;

        // TODO: Storage backend abstraction layer here
        public void Load (Stream s) {
            var doc   = new XmlDocument ();
            var esk   = new Dictionary<int, List<RefactorMePlease.EdgeSkeleton>>();
            _vertexes = new List <Vertex> ();

            doc.Load (s);

            // first iteration: collect vertexes and edge data
            var vertexNodes = doc.GetElementsByTagName ("vertex");
            foreach (XmlNode node in vertexNodes) {
                XmlElement elt    = (XmlElement) node;
                var vertex        = buildVertexFromXmlNode (elt);
                var edgeNodes     = elt.GetElementsByTagName ("edge");
                var edgeSkeletons = new List <RefactorMePlease.EdgeSkeleton> ();

                foreach (XmlNode edgeNode in edgeNodes)
                    edgeSkeletons.Add (buildEdgeSkeletonFromXmlNode ((XmlElement) edgeNode));
                
                esk [vertex.Id] = edgeSkeletons;
                AddVertex (vertex);
            }

            // second iteration: connect vertexes
            foreach (Vertex vertex in _vertexes) {
                List <RefactorMePlease.EdgeSkeleton> skeletons = esk[vertex.Id];

                foreach (RefactorMePlease.EdgeSkeleton skeleton in skeletons) {
                    Vertex to = _vertexes.First (x => x.Id == skeleton.IdTo);
                    vertex.Connect (to, skeleton.Weight);
                }
            }

            if (LayoutRebuilt != null)
                LayoutRebuilt ();
        }

        // TODO: Refactoring
        private Point buildPositionFromXmlNode (XmlNode node) {
            int x, y;
            XmlElement element = (XmlElement) node;

            if (element.HasAttribute("x"))
                x = System.Convert.ToInt32 (element.Attributes["x"].InnerText);
            else
                throw new Exception ("Position node does not have X attribute!");

            if (element.HasAttribute("y"))
                y = System.Convert.ToInt32 (element.Attributes["y"].InnerText);
            else
                throw new Exception ("Position node does not have Y attribute!");

            return new Point (x, y);
        }

        // TODO: Refactoring
        private Vertex buildVertexFromXmlNode (XmlElement vertexElt) {
            int id;
            string alias;

            if (vertexElt.HasAttribute ("id"))
                id = System.Convert.ToInt32 (vertexElt.Attributes["id"].InnerText);
            else
                throw new Exception ("Vertex node does not have ID attribute!");

            if (vertexElt.HasAttribute ("shape"))
                alias = vertexElt.Attributes ["shape"].InnerText;
            else
                throw new Exception ("Vertex node does not have Shape attribute!");
            
            System.Xml.XmlNode positionNode = vertexElt.GetElementsByTagName ("position")[0];
            Vertex vertex = new Vertex (id, buildPositionFromXmlNode (positionNode));
            vertex.VertexShape = Graphite.Shapes.Manager.instance().FromAlias (alias);
            return vertex;
        }

        private RefactorMePlease.EdgeSkeleton buildEdgeSkeletonFromXmlNode (System.Xml.XmlElement e) {
            var result = new RefactorMePlease.EdgeSkeleton();

            if (e.HasAttribute ("to"))
                result.IdTo = System.Convert.ToInt32 (e.Attributes["to"].InnerText);
            else
                throw new System.Exception ("Edge node does not have To attribute!");

            if (e.HasAttribute ("weight"))
                result.Weight = System.Convert.ToInt32 (e.Attributes["weight"].InnerText);
            else
                throw new System.Exception ("Edge node does not have Weight attribute!");

            return result;
        }

        public void Save (Stream s) {
            var settings = new XmlWriterSettings ();

            settings.Indent = true;
            settings.OmitXmlDeclaration = true;
            
            var writer = XmlWriter.Create (s, settings);
            writer.WriteStartElement ("graph");

            foreach (Vertex v in _vertexes)
                v.Save (writer);

            writer.WriteEndElement ();
            writer.Close ();
        }

        // IGraph interface implementation
        public Graphite.Abstract.IVertex [] vertexes () {
            return _vertexes.ToArray();
        }
    }
}
