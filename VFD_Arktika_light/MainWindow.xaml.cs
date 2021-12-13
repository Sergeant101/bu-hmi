using Monokot.ScadaExtension.Wpf.Controls;
using System.Reflection;
using System.IO;
using NGO.Elements.Special.Permissions.Model;

namespace VFD_Arktika_light
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
