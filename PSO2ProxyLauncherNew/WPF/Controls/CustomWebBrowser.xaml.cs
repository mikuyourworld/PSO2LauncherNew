using System.Windows.Controls;
using System.Windows.Navigation;

namespace PSO2ProxyLauncherNew.WPF.Controls
{
    /// <summary>
    /// Interaction logic for CustomWebBrowser.xaml
    /// </summary>
    public partial class CustomWebBrowser : UserControl
    {
        public CustomWebBrowser()
        {
            InitializeComponent();
            this.LockNavigate = true;
        }

        public event RequestNavigateEventHandler HyperlinkRequest;

        public bool LockNavigate { get; set; }

        public void NavigateContentAsync(string htmlcontent)
        {
            this.mainWebBrowser.NavigateToString(htmlcontent);
        }
        public void NavigateToStreamAsync(System.IO.Stream _stream)
        {
            this.mainWebBrowser.NavigateToStream(_stream);
        }
        public void NavigateAsync(string htmlURL)
        {
            this.NavigateAsync(new System.Uri(htmlURL));
        }
        public void NavigateAsync(System.Uri htmlURL)
        {
            this.mainWebBrowser.Navigate(htmlURL);
        }

        private void WebBrowser_Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            if (this.LockNavigate)
                this.HyperlinkRequest?.Invoke(sender, e);
        }

        private void WebBrowser_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            e.Cancel = !this.LockNavigate;
        }
    }
}
