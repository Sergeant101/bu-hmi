using Monokot.Hmi.Core.Framework.Runtime;
using NGO.Elements.Special.ReadyWindow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NGO.Elements.Message
{
    public partial class DzuStatusBox : UserControl
    {
        private const string STRING_NAME_FOR_SEARCH = "AuxStatusBox_list";

        public DzuStatusBox()
        {
            InitializeComponent();           
        }        
    }
}
