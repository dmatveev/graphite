using System;
using System.Windows.Forms;

class GraphiteApplication {
    [STAThread]
    public static void Main () {
        Application.EnableVisualStyles();
        Application.Run (new Windows.MainWindow());
   }
}