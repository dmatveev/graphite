using System;
using System.Drawing;
using System.Windows.Forms;

namespace Graphite.Editor.States {
    public interface IState {
        void ProcessClick     ();
        void ProcessMouseDown ();
        void ProcessMouseMove ();
        void ProcessMouseUp   ();
    }

    public class State {
        protected Graphite.Core.IDocument _document;

        public State (Graphite.Core.IDocument doc) {
            _document = doc;
        }
    }
    
    public class Adding: State, IState {
        public Adding (Graphite.Core.IDocument doc) : base (doc) {
        }

        public void ProcessClick () {
            _document.CreateVertex ();
        }

        public void ProcessMouseDown () {
        }

        public void ProcessMouseMove () {
        }

        public void ProcessMouseUp () {
        }
    }

    public class Deleting: State, IState {
        public Deleting (Graphite.Core.IDocument doc) : base (doc) {
        }

        public void ProcessClick () {
            _document.SelectVertex ();

            Visuals.Vertex v = _document.SelectedVertex ();
            if (v != null)
                _document.DeleteVertex (v);

        }

        public void ProcessMouseDown () {
        }

        public void ProcessMouseMove () {
        }

        public void ProcessMouseUp () {
        }
    }

    public class Disconnecting: State, IState {
        public Disconnecting (Graphite.Core.IDocument doc) : base (doc) {
        }

        public void ProcessClick () {
            _document.SelectEdge ();

            Visuals.Edge e = _document.SelectedEdge ();
            if (e != null)
                _document.DisconnectVertexes (e.First, e.Second);
        }

        public void ProcessMouseDown () {
        }

        public void ProcessMouseMove () {
        }

        public void ProcessMouseUp () {
        }
    }

    public class Idle: State, IState {
        protected bool _pressed;
        protected Visuals.Vertex _draggedVertex;

        public Idle (Graphite.Core.IDocument doc) : base (doc) {
        }

        public void ProcessClick () {
            _document.SelectVertex ();
        }
        
        public void ProcessMouseDown () {
            _document.SelectVertex ();
            _pressed = ((_draggedVertex = _document.SelectedVertex()) != null);
        }

        public void ProcessMouseMove () {
            if (_pressed)
                _document.MoveVertex (_draggedVertex);
        }

        public void ProcessMouseUp () {
            _pressed = false;
        }
    }

    public class Connecting: State, IState {
        protected Visuals.Vertex _fromVertex;

        public Connecting (Graphite.Core.IDocument doc) : base (doc) {
        }

        public void ProcessClick () {
            _document.SelectVertex ();
            
            Visuals.Vertex thisVisual;

            if ((thisVisual = _document.SelectedVertex()) == null)
                return;

            if (_fromVertex != null)
                _document.ConnectVertexes (_fromVertex, thisVisual);
            else
                _fromVertex = thisVisual;
        }

        
        public void ProcessMouseDown () {
        }
    
        public void ProcessMouseMove () {
        }

        public void ProcessMouseUp () {
        }
    }
}