using System;
using System.Collections.Generic;
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

namespace PSO2ProxyLauncherNew.WPF
{
    /// <summary>
    /// Interaction logic for LoadingPictureBox.xaml
    /// </summary>
    public partial class LoadingPictureBox : UserControl
    {
        public LoadingPictureBox()
        {
            InitializeComponent();
        }

        public void SetRingColor(System.Drawing.Color theColor)
        {
            MetroRing.Foreground = new SolidColorBrush(Color.FromArgb(theColor.A, theColor.R, theColor.G, theColor.B));
        }
    }
}
