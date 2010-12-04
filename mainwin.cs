using System;
using System.Data;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using Graphite.Editor.States;

namespace Windows {
    public class MainWindow: System.Windows.Forms.Form, Graphite.Core.IDocument {
        protected ToolStrip               _toolStrip;
        protected ToolStripComboBox       _shapeCombo;
        protected Widgets.Scene           _scene;
        protected State                   _state;
        protected Graphite.Shapes.Manager _shapeMan;

        protected int                     _counter;

        public MainWindow () {
            _state    = new Graphite.Editor.States.Adding (this);
            _shapeMan = new Graphite.Shapes.Manager ();
            _counter  = 0;
            InitializeComponent();
        }

        private void CreateScene () {
            _scene  = new Widgets.Scene();

            _scene.Location = new System.Drawing.Point (0, 0);
            _scene.Anchor   = (AnchorStyles.Left | AnchorStyles.Right |
                               AnchorStyles.Top  | AnchorStyles.Bottom);
            _scene.Size     = ClientSize;

            _scene.Click     += (obj, e) => _state.ProcessClick();
            _scene.MouseMove += (obj, e) => _state.ProcessMouseMove();
            _scene.MouseDown += (obj, e) => _state.ProcessMouseDown();
            _scene.MouseUp   += (obj, e) => _state.ProcessMouseUp();
        }

        private void InitializeComponent () {
            Text = "Graphite";
            ClientSize = new Size (640, 480);

            _toolStrip = new ToolStrip();
            _toolStrip.SuspendLayout ();
            SuspendLayout();
            
            CreateToolbarButtons ();
            CreateShapeSelector ();
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

        private void Save () {
            SaveFileDialog   dialog;

            dialog = new SaveFileDialog();
            dialog.Filter = "XML files (*.xml)|*.xml";
            dialog.FilterIndex = 1;
            dialog.RestoreDirectory = true;
            
            if (dialog.ShowDialog() == DialogResult.OK) {
                var stream = new System.IO.FileStream (dialog.FileName, System.IO.FileMode.Create);
                if (stream != null) {
                    _scene.Save (stream);
                    stream.Close ();
                }
            }
        }

        private void Open () {
            System.IO.Stream stream;
            OpenFileDialog   dialog;

            dialog = new OpenFileDialog();
            dialog.Filter = "XML files (*.xml)|*.xml";
            dialog.FilterIndex = 1;
            dialog.RestoreDirectory = true;
            
            if (dialog.ShowDialog() == DialogResult.OK) {
                if ((stream = dialog.OpenFile()) != null) {
                    _scene.Load (stream);
                    stream.Close ();
                }
            }
        }

        private void CreateToolbarButtons () {
            _toolStrip.Items.AddRange (new ToolStripItem [] {
                CreateButton ("Open",       (obj, e) => Open()),
                CreateButton ("Save",       (obj, e) => Save()),
                CreateButton ("Add",        (obj, e) => _state = new Adding (this)),
                CreateButton ("Connect",    (obj, e) => _state = new Connecting (this)),
                CreateButton ("Disconnect", (obj, e) => _state = new Disconnecting (this)),
                CreateButton ("Delete",     (obj, e) => _state = new Deleting (this)),
                CreateButton ("Select",     (obj, e) => _state = new Idle (this))
            });
        }

        private void CreateShapeSelector () {
            _shapeCombo = new ToolStripComboBox ();
            _shapeCombo.DropDownStyle = ComboBoxStyle.DropDownList;

            foreach (Graphite.Core.Shape sh in _shapeMan.Shapes)
                _shapeCombo.Items.Add (sh.name ());

            _shapeCombo.SelectedIndex = 0;

            _toolStrip.Items.Add (new ToolStripSeparator ());
            _toolStrip.Items.Add (new ToolStripLabel ("Shape:"));
            _toolStrip.Items.Add (_shapeCombo);
        }

        public void CreateVertex () {
            Point screen = System.Windows.Forms.Cursor.Position;
            Point client = _scene.PointToClient (screen);
            var v = new Graphite.Core.Vertex (++_counter, client);
            v.VertexShape = _shapeMan.Shapes[_shapeCombo.SelectedIndex];
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
