using System;
using System.Data;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using Graphite.Editor.States;

namespace Windows {
    public class MainWindow: System.Windows.Forms.Form, Graphite.Core.IDocument {
        protected ToolStrip _toolStrip;
        protected Widgets.Scene _scene;
        protected Graphite.Editor.States.State _state;
        protected Graphite.Scene.Elements.CircleRenderer _cr;
        protected ContextMenu _shapesMenu;

        public MainWindow () {
            _state = new Graphite.Editor.States.Adding (this);
            _cr = new Graphite.Scene.Elements.CircleRenderer ();
            InitializeComponent();
        }

        private void CreateScene () {
            _scene = new Widgets.Scene();
            _scene.Location = new System.Drawing.Point (0, 0);
            _scene.Anchor = (AnchorStyles.Left | AnchorStyles.Right |
                             AnchorStyles.Top  | AnchorStyles.Bottom);
            _scene.Size = ClientSize;

            _scene.Click     += new EventHandler      (OnClick);
            _scene.MouseMove += new MouseEventHandler (OnMouseMove);
            _scene.MouseDown += new MouseEventHandler (OnMouseDown);
            _scene.MouseUp   += new MouseEventHandler (OnMouseUp);

        }

        private void InitializeComponent () {
            Text = "Graphite";
            ClientSize = new Size (640, 480);

            _toolStrip = new ToolStrip();
            _toolStrip.SuspendLayout ();
            SuspendLayout();
            
            CreateToolbarButtons ();
            CreateScene ();
            
            Controls.Add (_toolStrip);
            Controls.Add (_scene);
            
            _toolStrip.ResumeLayout (false);
            ResumeLayout (false);
            PerformLayout ();
        }

        private ToolStripButton CreateButton (string label, EventHandler e) {
            var btn = new ToolStripButton ();

            btn.DisplayStyle =  ToolStripItemDisplayStyle.Text;
			btn.Text         =  label;
			btn.TextAlign    =  System.Drawing.ContentAlignment.MiddleRight;
			btn.Click        += e;

            return btn;
        }

        private void CreateToolbarButtons () {

            _toolStrip.Items.AddRange (new ToolStripItem [] {
                CreateButton ("Add",        (obj, e) => _state = new Adding (this)),
                CreateButton ("Connect",    (obj, e) => _state = new Connecting (this)),
                CreateButton ("Disconnect", (obj, e) => _state = new Disconnecting (this)),
                CreateButton ("Delete",     (obj, e) => _state = new Deleting (this)),
                CreateButton ("Select",     (obj, e) => _state = new Idle (this))
            });
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
            var v = new Graphite.Core.Vertex (0, client);
            v.Renderer = _cr;
            _scene.AddVertex (v);
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
   }
}