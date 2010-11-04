using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

namespace Windows {
    public class MainWindow: System.Windows.Forms.Form, Graphite.Core.IDocument {
        protected Widgets.Scene _scene;
        protected Graphite.Editor.States.IState _state;

        public MainWindow () {
            _state = new Graphite.Editor.States.Adding (this);
            InitializeComponent();
        }

        private void InitializeComponent () {
            SuspendLayout();

            _scene = new Widgets.Scene();
            _scene.Location = new System.Drawing.Point (0, 0);
            _scene.Size = new System.Drawing.Size (100, 100);
            _scene.Anchor = (AnchorStyles.Left | AnchorStyles.Right |
                             AnchorStyles.Top  | AnchorStyles.Bottom);

            ClientSize = new System.Drawing.Size (100, 100);
            Controls.Add (_scene);
            ResumeLayout (false);

            _scene.Click += new EventHandler (Clicked);
        }

        public void Clicked (object obj, EventArgs args) {
            _state.ProcessClick ();
        }

        public void CreateVertex () {
            Point screen = System.Windows.Forms.Cursor.Position;
            Point client = _scene.PointToClient (screen);
            _scene.AddVertex (new Visuals.Vertex (new Core.Vertex (),
                                                  client));
        }
    }
}