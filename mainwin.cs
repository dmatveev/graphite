using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

namespace Windows {
    public class MainWindow: System.Windows.Forms.Form, Graphite.Core.IDocument {
        protected ToolBar _toolbar;
        protected Widgets.Scene _scene;
        protected Graphite.Editor.States.State _state;

        public MainWindow () {
            _state = new Graphite.Editor.States.Adding (this);
            InitializeComponent();
        }

        private void InitializeComponent () {
            SuspendLayout();
            CreateToolbar ();

            _scene = new Widgets.Scene();
            _scene.Location = new System.Drawing.Point (0, 0);
            _scene.Size = new System.Drawing.Size (640, 480);
            _scene.Anchor = (AnchorStyles.Left | AnchorStyles.Right |
                             AnchorStyles.Top  | AnchorStyles.Bottom);

            Text = "Graphite";
            ClientSize = new System.Drawing.Size (640, 480);
            Controls.Add (_scene);
            ResumeLayout (false);

            _scene.Click     += new EventHandler      (OnClick);
            _scene.MouseMove += new MouseEventHandler (OnMouseMove);
            _scene.MouseDown += new MouseEventHandler (OnMouseDown);
            _scene.MouseUp   += new MouseEventHandler (OnMouseUp);
        }

        private void CreateToolbar () {
            _toolbar = new ToolBar();
            ToolBarButton addBtn = new ToolBarButton ("Add");
            ToolBarButton conBtn = new ToolBarButton ("Connect");
            ToolBarButton disBtn = new ToolBarButton ("Disconnect");
            ToolBarButton delBtn = new ToolBarButton ("Delete");
            ToolBarButton selBtn = new ToolBarButton ("Select");

            _toolbar.Buttons.Add (addBtn);
            _toolbar.Buttons.Add (conBtn);
            _toolbar.Buttons.Add (disBtn);
            _toolbar.Buttons.Add (delBtn);
            _toolbar.Buttons.Add (selBtn);
            
            _toolbar.ButtonClick += new ToolBarButtonClickEventHandler (this.OnCommand);
            
            Controls.Add (_toolbar);
        }

        public void OnClick (object obj, EventArgs args) {
            _state.ProcessClick ();
        }

        public void OnMouseMove (object obj, EventArgs args) {
            _state.ProcessMouseMove ();
        }

        public void OnMouseDown (object obj, EventArgs args) {
            _state.ProcessMouseDown ();
        }

        public void OnMouseUp (object obj, EventArgs args) {
            _state.ProcessMouseUp ();
        }

        public void CreateVertex () {
            Point screen = System.Windows.Forms.Cursor.Position;
            Point client = _scene.PointToClient (screen);
            _scene.AddVertex (new Graphite.Core.Vertex (0, client));
        }

        public void DeleteVertex (Graphite.Core.Vertex v) {
            _scene.DeleteVertex (v);
        }

        public void SelectVertex () {
            Point screen = System.Windows.Forms.Cursor.Position;
            Point client = _scene.PointToClient (screen);
            _scene.TrySelectVertex (client);
        }

        public void SelectEdge () {
            Point screen = System.Windows.Forms.Cursor.Position;
            Point client = _scene.PointToClient (screen);
            _scene.TrySelectEdge (client);
        }

        public Graphite.Core.Vertex SelectedVertex () {
            return _scene.SelectedVertex;
        }

        public Graphite.Core.Edge SelectedEdge () {
            return _scene.SelectedEdge;
        }

        public void ConnectVertexes (Graphite.Core.Vertex a, Graphite.Core.Vertex b) {
            _scene.ConnectVertexes (a, b);
        }

        public void DisconnectVertexes (Graphite.Core.Vertex a, Graphite.Core.Vertex b) {
            _scene.DisconnectVertexes (a, b);
        }

        public void MoveVertex (Graphite.Core.Vertex v) {
            Point screen = System.Windows.Forms.Cursor.Position;
            Point client = _scene.PointToClient (screen);
            v.Position = client;
            _scene.Refresh ();
        }

        private void OnCommand (Object sender, ToolBarButtonClickEventArgs e) {
            switch (_toolbar.Buttons.IndexOf (e.Button)) {
            case 0:
                _state = new Graphite.Editor.States.Adding (this);
                break;
            case 1:
                _state = new Graphite.Editor.States.Connecting (this);
                break;
            case 2:
                _state = new Graphite.Editor.States.Disconnecting (this);
                break;
            case 3:
                _state = new Graphite.Editor.States.Deleting (this);
                break;
            case 4:
                _state = new Graphite.Editor.States.Idle (this);
                break;
            }   
        }
    }
}