using System;
using System.Drawing;
using System.Windows.Forms;

namespace Graphite.Editor.States {
    public class State {
        protected Graphite.Core.Document _document;
        protected Graphite.Core.IUISet   _ui;
        
        public State (Graphite.Core.Document doc, Graphite.Core.IUISet ui) {
            _document = doc;
            _ui = ui;
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
        public Adding (Graphite.Core.Document doc, Graphite.Core.IUISet ui) : base (doc, ui) {
        }

        public override void ProcessClick () {
            _document.CreateVertex (_ui.graphView().CurrentPoint (),
                                    _ui.shapeSelector().SelectedShape ());
        }
    }

    public class Deleting: State {
        public Deleting (Graphite.Core.Document doc, Graphite.Core.IUISet ui) : base (doc, ui) {
        }

        public override void ProcessClick () {
            var v = _ui.graphView().SelectedVertex ();
            if (v != null)
                _document.DeleteVertex (v);
        }
    }

    public class Disconnecting: State {
        public Disconnecting (Graphite.Core.Document doc, Graphite.Core.IUISet ui) : base (doc, ui) {
        }

        public override void ProcessClick () {
            var e = _ui.graphView().SelectedEdge ();
            if (e != null) {
                _document.DisconnectVertex (e.From, e.To);
                _document.DisconnectVertex (e.To, e.From);
            }
        }
    }

    public class Idle: State {
        protected bool _pressed;
        protected Graphite.Core.Vertex _draggedVertex;

        public Idle (Graphite.Core.Document doc, Graphite.Core.IUISet ui) : base (doc, ui) {
        }
    
        public override void ProcessMouseDown () {
            _ui.graphView().TrySelectVertex (_ui.graphView().CurrentPoint());
            _pressed = ((_draggedVertex = _ui.graphView().SelectedVertex()) != null);
        }

        public override void ProcessMouseMove () {
            if (_pressed)
                _document.MoveVertex (_draggedVertex, _ui.graphView().CurrentPoint());
        }

        public override void ProcessMouseUp () {
            _pressed = false;
        }
    }

    public class Connecting: State {
        protected Graphite.Core.Vertex _fromVertex;

        public Connecting (Graphite.Core.Document doc, Graphite.Core.IUISet ui) : base (doc, ui) {
        }

        public override void ProcessClick () {
            Graphite.Core.Vertex thisVisual;

            if ((thisVisual = _ui.graphView().SelectedVertex()) == null)
                 return;

            if (_fromVertex != null) {
                _document.ConnectVertex (_fromVertex, thisVisual, 0);
                _document.ConnectVertex (thisVisual, _fromVertex, 0);
            }
            else
                _fromVertex = thisVisual;
        }
    }
}
