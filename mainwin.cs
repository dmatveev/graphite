using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

namespace Windows {
    public class MainWindow: System.Windows.Forms.Form, Graphite.Core.IDocument {
        protected ToolBar _toolbar;
        protected Widgets.Scene _scene;
        protected Graphite.Editor.States.IState _state;

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

            _scene.Click += new EventHandler (Clicked);
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

        public void Clicked (object obj, EventArgs args) {
            _state.ProcessClick ();
        }

        public void CreateVertex () {
            Point screen = System.Windows.Forms.Cursor.Position;
            Point client = _scene.PointToClient (screen);
            _scene.AddVertex (new Graphite.Core.Vertex(), client);
        }

        public void SelectVertex () {
            Point screen = System.Windows.Forms.Cursor.Position;
            Point client = _scene.PointToClient (screen);
            _scene.TrySelectVertex (client);
        }

        public Visuals.Vertex SelectedVertex () {
            return _scene.SelectedVertex;
        }

        public void ConnectVertexes (Visuals.Vertex a, Visuals.Vertex b) {
            a.Connect (b);
            b.Connect (a);
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
            case 4:
                _state = new Graphite.Editor.States.Idle (this);
                break;
            }   
        }
    }
}