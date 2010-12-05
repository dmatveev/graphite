using System;
using System.Data;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using Graphite.Editor.States;

namespace Windows {
    public class MainWindow: System.Windows.Forms.Form, Graphite.Core.IUISet {
        protected ToolStrip               _toolStrip;
        protected Graphite.Widgets.ShapeSelector   _shapeCombo;
        protected Widgets.Scene           _scene;
        protected State                   _state;
        protected Graphite.Shapes.Manager _shapeMan;
        protected Graphite.Core.Document  _doc;
        protected int                     _counter;

        public MainWindow () {
            _state    = new Graphite.Editor.States.Adding (_doc, this);
            _shapeMan = new Graphite.Shapes.Manager ();
            _doc = new Graphite.Core.Document ();
            _counter  = 0;
            InitializeComponent();

            _doc.LayoutUpdated      += _scene.LayoutUpdated;
            _doc.LayoutRebuilt      += _scene.LayoutRebuilt;
            _doc.VertexAdded        += _scene.VertexAdded;
            _doc.VertexRemoved      += _scene.VertexRemoved;
            _doc.VertexConnected    += _scene.VertexConnected;
            _doc.VertexDisconnected += _scene.VertexDisconnected;
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
                    _doc.Save (stream);
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
                    _scene.Reset ();
                    _doc.Load (stream);
                    stream.Close ();
                }
            }
        }

        private void CreateToolbarButtons () {
            _toolStrip.Items.AddRange (new ToolStripItem [] {
                CreateButton ("Open",       (obj, e) => Open()),
                CreateButton ("Save",       (obj, e) => Save()),
                CreateButton ("Add",        (obj, e) => _state = new Adding (_doc, this)),
                CreateButton ("Connect",    (obj, e) => _state = new Connecting (_doc, this)),
                CreateButton ("Disconnect", (obj, e) => _state = new Disconnecting (_doc, this)),
                CreateButton ("Delete",     (obj, e) => _state = new Deleting (_doc, this)),
                CreateButton ("Select",     (obj, e) => _state = new Idle (_doc, this))
            });
        }

        private void CreateShapeSelector () {
            _shapeCombo = new Graphite.Widgets.ShapeSelector (_shapeMan);

            _toolStrip.Items.Add (new ToolStripSeparator ());
            _toolStrip.Items.Add (new ToolStripLabel ("Shape:"));
            _toolStrip.Items.Add (_shapeCombo);
        }

        public Graphite.Core.IGraphView graphView () {
            return _scene;
        }

        public Graphite.Core.IShapeSelector shapeSelector () {
            return _shapeCombo;
        }
    }
}
