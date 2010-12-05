using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Graphite.Widgets {
    public class ShapeSelector: ToolStripComboBox, Graphite.Core.IShapeSelector {
        protected Graphite.Shapes.Manager _shapeMan;

        public ShapeSelector (Graphite.Shapes.Manager shapeMan) {
            _shapeMan = shapeMan;
            
            foreach (Graphite.Core.Shape sh in _shapeMan.Shapes)
                Items.Add (sh.name ());

            DropDownStyle = ComboBoxStyle.DropDownList;
            SelectedIndex = 0;
        }
        
        public Graphite.Core.Shape SelectedShape () {
            return _shapeMan.Shapes [SelectedIndex];
        }
    }
}