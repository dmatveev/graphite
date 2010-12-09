using System;
using System.Data;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using Graphite.Editor.States;
using System.Collections.Generic;
using Graphite.Utils;
using System.Linq;

namespace Graphite.Utils {
    public class Command {
        public string       Name  {get; protected set;}
        public EventHandler Event {get; protected set;}

        public Command (string name, EventHandler associatedEvent) {
            Name  = name;
            Event = associatedEvent;
        }
    }

    public class CommandGroup {
        public string        Name  {get; protected set;}
        public List<Command> Items {get; protected set;}

        public CommandGroup (string name) {
            Name = name;
            Items = new List<Command> ();
        }
    }
}

namespace Windows {
    public class MainWindow: System.Windows.Forms.Form, Graphite.Core.IUISet {
        protected MenuStrip               _menuStrip;
        protected ToolStrip               _toolStrip;
        protected Graphite.Widgets.ShapeSelector   _shapeCombo;
        protected Widgets.Scene           _scene;
        protected State                   _state;
        protected Graphite.Shapes.Manager _shapeMan;
        protected Graphite.Core.Document  _doc;
        protected int                     _counter;

        public MainWindow () {
            _doc      = new Graphite.Core.Document ();
            _state    = new Graphite.Editor.States.Adding (_doc, this);
            _shapeMan = new Graphite.Shapes.Manager ();
            _counter  = 0;
            InitializeComponent();
            ConnectModelWithView();
        }

        private void ConnectModelWithView () {
            _doc.LayoutUpdated      += _scene.LayoutUpdated;
            _doc.LayoutRebuilt      += _scene.LayoutRebuilt;
            _doc.VertexAdded        += _scene.VertexAdded;
            _doc.VertexRemoved      += _scene.VertexRemoved;
            _doc.VertexConnected    += _scene.VertexConnected;
            _doc.VertexDisconnected += _scene.VertexDisconnected;
        }

        private void CreateScene () {
            _scene  = new Widgets.Scene();

            var yoffset = _toolStrip.Size.Height + _menuStrip.Size.Height;
            _scene.Location = new System.Drawing.Point (0, yoffset);
            _scene.Anchor   = (AnchorStyles.Left | AnchorStyles.Right |
                               AnchorStyles.Top  | AnchorStyles.Bottom);
            _scene.Size     = new Size (ClientSize.Width, ClientSize.Height - yoffset);

            _scene.Click     += (obj, e) => _state.ProcessClick();
            _scene.MouseMove += (obj, e) => _state.ProcessMouseMove();
            _scene.MouseDown += (obj, e) => _state.ProcessMouseDown();
            _scene.MouseUp   += (obj, e) => _state.ProcessMouseUp();
        }

        private void CreateMenu (CommandGroup [] groups) {
            _menuStrip = new MenuStrip ();

            foreach (CommandGroup cg in groups) {
                var topItem = new ToolStripMenuItem (cg.Name);
                topItem.DropDownItems.AddRange
                    (cg.Items.Select (x => new ToolStripMenuItem (x.Name, null, x.Event)).ToArray());
                
                _menuStrip.Items.Add (topItem);
            }
            MainMenuStrip = _menuStrip;
        }

        private void InitializeComponent () {
            Text = "Graphite";
            ClientSize = new Size (640, 480);

            _toolStrip = new ToolStrip();
            _toolStrip.SuspendLayout ();
            SuspendLayout ();

            var g = CreateCommandGroups ();
            CreateToolbarButtons (g);
            CreateShapeSelector ();
            CreateMenu (g);
            CreateScene ();
            
            Controls.Add (_toolStrip);
            Controls.Add (_scene);
            Controls.Add (_menuStrip);
            
            _toolStrip.ResumeLayout (false);
            ResumeLayout (false);
            PerformLayout ();
        }

        protected CommandGroup[] CreateCommandGroups () {
            var fileOps = new CommandGroup ("File");
            fileOps.Items.AddRange (new Command [] {
                    new Command ("Open",    (obj, e) => Open()),
                    new Command ("Save",    (obj, e) => Save()),
                });
            
            var actions = new CommandGroup ("Actions");
            actions.Items.AddRange (new Command [] {
                    new Command ("Add",        (obj, e) => _state = new Adding (_doc, this)),
                    new Command ("Delete",     (obj, e) => _state = new Deleting (_doc, this)),
                    new Command ("Connect",    (obj, e) => _state = new Connecting (_doc, this)),
                    new Command ("Disconnect", (obj, e) => _state = new Disconnecting (_doc, this)),
                    new Command ("Select",     (obj, e) => _state = new Idle (_doc, this)),
                });
            
            return new CommandGroup [] {fileOps, actions};
        }

        private ToolStripButton CreateButton (Command c) {
            var btn = new ToolStripButton ();

            btn.DisplayStyle =  ToolStripItemDisplayStyle.Text;
			btn.Text         =  c.Name;
			btn.TextAlign    =  System.Drawing.ContentAlignment.MiddleRight;
			btn.Click        += c.Event;

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

        private void CreateToolbarButtons (CommandGroup [] groups) {
            foreach (CommandGroup cg in groups) {
                _toolStrip.Items.AddRange (cg.Items.Select (x => CreateButton (x)).ToArray ());
                _toolStrip.Items.Add (new ToolStripSeparator ());
            }
        }

        private void CreateShapeSelector () {
            _shapeCombo = new Graphite.Widgets.ShapeSelector (_shapeMan);
            
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
