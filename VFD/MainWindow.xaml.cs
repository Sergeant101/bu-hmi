using Monokot.ScadaExtension.Wpf.Controls;
using Path = System.IO.Path;
using NGO.Elements.Special.Permissions.Model;
using System.Reflection;
using System.Windows;


namespace VFD
{
    public partial class MainWindow : SEWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            // Auth
            var usrPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "usr.cfg");
            NgoAuth.auth.loadFromFile(usrPath);            
        }
}
}
