using System;
using System.Drawing;
using System.Windows.Forms;

namespace Graphite.Editor.States {
    public class State {
        protected Graphite.Core.IDocument _document;

        public State (Graphite.Core.IDocument doc) {
            _document = doc;
        }

        public virtual void ProcessClick () {
        }

        public virtual void ProcessMouseDown () {
        }

        public virtual void ProcessMouseMove () {
        }

        public virtual void ProcessMouseUp () {
        }
    }
    
    public class Adding: State {
        public Adding (Graphite.Core.IDocument doc) : base (doc) {
        }

        public override void ProcessClick () {
            _document.CreateVertex ();
        }
    }

    public class Deleting: State {
        public Deleting (Graphite.Core.IDocument doc) : base (doc) {
        }

        public override void ProcessClick () {
            _document.SelectVertex ();

            Visuals.Vertex v = _document.SelectedVertex ();
            if (v != null)
                _document.DeleteVertex (v);
        }
    }

    public class Disconnecting: State {
        public Disconnecting (Graphite.Core.IDocument doc) : base (doc) {
        }

        public override void ProcessClick () {
            _document.SelectEdge ();

            Visuals.Edge e = _document.SelectedEdge ();
            if (e != null)
                _document.DisconnectVertexes (e.First, e.Second);
        }
    }

    public class Idle: State {
        protected bool _pressed;
        protected Visuals.Vertex _draggedVertex;

        public Idle (Graphite.Core.IDocument doc) : base (doc) {
        }

        public override void ProcessClick () {
            _document.SelectVertex ();
        }
        
        public override void ProcessMouseDown () {
            _document.SelectVertex ();
            _pressed = ((_draggedVertex = _document.SelectedVertex()) != null);
        }

        public override void ProcessMouseMove () {
            if (_pressed)
                _document.MoveVertex (_draggedVertex);
        }

        public override void ProcessMouseUp () {
            _pressed = false;
        }
    }

    public class Connecting: State {
        protected Visuals.Vertex _fromVertex;

        public Connecting (Graphite.Core.IDocument doc) : base (doc) {
        }

        public override void ProcessClick () {
            _document.SelectVertex ();
            
            Visuals.Vertex thisVisual;

            if ((thisVisual = _document.SelectedVertex()) == null)
                return;

            if (_fromVertex != null)
                _document.ConnectVertexes (_fromVertex, thisVisual);
            else
                _fromVertex = thisVisual;
        }
    }
}
