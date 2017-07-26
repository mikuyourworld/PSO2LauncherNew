using System;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PSO2LauncherWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            this.Icon = Imaging.CreateBitmapSourceFromHIcon(Properties.Resources._1.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }
    }
}
