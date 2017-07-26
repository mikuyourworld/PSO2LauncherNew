using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PSO2ProxyLauncherNew.Classes;
using System.Threading;
using System.ComponentModel;
using PSO2ProxyLauncherNew.Classes.Components;
using PSO2ProxyLauncherNew.Classes.Events;
using PSO2ProxyLauncherNew.Classes.Components.WebClientManger;

namespace PSO2ProxyLauncherNew.WPF
{
    /// <summary>
    /// Interaction logic for MyMainMenu.xaml
    /// </summary>
    public partial class MyMainMenu
    {
        private SynchronizationContext SyncContext;
        private BackgroundWorker bWorker_tweakerWebBrowser_load, bWorker_Boot;
        private PSO2Controller _pso2controller;
        private SelfUpdate _selfUpdater;
        private Leayal.WMI.ProcessWatcher pso2processwatcher;
        private Control[] targetedButtons;
        private Leayal.Ugoira.WPF.UgoiraPlayer _player;
        private ContextMenu tweakerWebBrowserContextMenu;

        public MyMainMenu()
        {
            InitializeComponent();
            this.Loaded += this.MyMainMenu_Shown;
            this.Unloaded += this.MyMainMenu_Unloaded;

        }

        private void MyMainMenu_Unloaded(object sender, RoutedEventArgs e)
        {
            if (this._player != null)
                this._player.Dispose();
        }

        private void MyMainMenu_Shown(object sender, RoutedEventArgs e)
        {
            Program.ApplicationController.HideSplashScreenEx();
            /*ImageBrush asd = new ImageBrush();
            BitmapImage awlighalwigh = new BitmapImage();
            using (System.IO.FileStream fs = System.IO.File.OpenRead(@"D:\Data2\addd\Downloads\Image\61615584_響 3 - GIF★ZipHQ.ugoira"))
            {
                awlighalwigh.BeginInit();
                awlighalwigh.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                awlighalwigh.CacheOption = BitmapCacheOption.OnLoad;
                awlighalwigh.StreamSource = fs;
                awlighalwigh.EndInit();
                awlighalwigh.Freeze();
            }
            // This is zoom
            asd.Stretch = Stretch.Uniform;
            // This should be Fill
            asd.Stretch = Stretch.UniformToFill;
            asd.ImageSource = awlighalwigh;*/

            var sync = System.Threading.SynchronizationContext.Current;
            Leayal.Ugoira.WPF.UgoiraPlayer.CreateAsync(@"D:\Data2\addd\Downloads\Image\Miku\46509847_水ZipHQ.ugoira", new EventHandler<Leayal.Ugoira.WPF.UgoiraPlayerCreatedEventArgs>((senderex, eventargsex) =>
            {
                if (eventargsex.Error != null)
                    MessageBox.Show(eventargsex.Error.ToString(), "Error");
                else if (eventargsex.Cancelled)
                    MessageBox.Show("Ugoira Cancelled", "Warning");
                else
                {
                    this._player = eventargsex.UgoiraPlayer;
                    this._player.SynchronizationContext = sync;
                    this._player.FrameDraw += this._player_FrameDraw;
                    this._player.Begin();
                }
            }));

            this.Height = 480;
            this.Width = 640;
            
            this.richtextbox_log.AppendText(LanguageManager.GetMessageText("Checkingforupdates", "Checking for launcher updates..."));

            // this.webBrowserTweaker.

            // this.webBrowserTweaker.Source = "http://arks-layer.com/justice/tweaker.php";
            // this.webBrowserTweaker.navi

            this.Activate();
            //this.x_gamestartbutton.Text = "INSTALL";

            // backgroundCanvas.
        }

        #region "Tweaker Browser Methods"
        private void RefreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.bWorker_tweakerWebBrowser_load.RunWorkerAsync();
        }

        private void BWorker_tweakerWebBrowser_load_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            TweakerWebBrowser_IsLoading(false);
            if (e.Error != null)
                Leayal.Log.LogManager.GeneralLog.Print(e.Error);
        }

        private void BWorker_tweakerWebBrowser_load_DoWork(object sender, DoWorkEventArgs e)
        {
            /*string linefiletoskip = Classes.AIDA.TweakerWebPanel.CutString;
            linefiletoskip = WebClientPool.GetWebClient(DefaultValues.MyServer.GetWebLink).DownloadString(DefaultValues.MyServer.GetWebLink + DefaultValues.MyServer.Web.TweakerSidePanelLiner);
            if (string.IsNullOrWhiteSpace(linefiletoskip))
                linefiletoskip = Classes.AIDA.TweakerWebPanel.CutString;*/
            TweakerWebBrowser_IsLoading(true);
            if (!AIDA.IsPingedAIDA)
                AIDA.GetIdeaServer();
            if (AIDA.IsPingedAIDA)
            {
                this.webBrowserTweaker.LockNavigate = false;
                string resultofgettinghtmlfile = WebClientPool.GetWebClient_AIDA().DownloadString(AIDA.TweakerWebPanel.InfoPageLink);
                if (string.IsNullOrEmpty(resultofgettinghtmlfile))
                {
                    this.webBrowserTweaker.NavigateAsync(AIDA.TweakerWebPanel.InfoPageLink);
                    this.webBrowserTweaker.LockNavigate = true;
                }
                else
                {
                    this.webBrowserTweaker.NavigateContentAsync(resultofgettinghtmlfile);
                    this.webBrowserTweaker.LockNavigate = true;
                    //this.tweakerWebBrowser.EnglishPatchStatus = result.EnglishPatch;
                    //this.tweakerWebBrowser.ItemPatchStatus = result.ItemPatch;
                }//*/
            }
            else
            {
                this.webBrowserTweaker.LockNavigate = false;
                this.webBrowserTweaker.NavigateContentAsync(
@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"" />
    <style>
        div{text-align:center;width:100%;font-weight:bold;font-family:""Times New Roman"",Times,serif;font-size:17px;}
        a:visited,a{color:red;}
    </style>
</head>
<body>
    <div>
        <span>Failed to connect to Arks-Layer's server.</span><br/>
        <a href=""leayal://retry"">Click me to retry</a>
    </div>
</body>
</html>
");
                this.webBrowserTweaker.LockNavigate = true;
            }
        }

        private void TweakerWebBrowser_CommandLink(object sender, StepEventArgs e)
        {
            switch (e.Step)
            {
                case "retry":
                    this.refreshToolStripMenuItem.PerformClick();
                    break;
            }
        }

        private BackgroundWorker CreatetweakerWebBrowser()
        {
            BackgroundWorker result = new BackgroundWorker();
            result.WorkerSupportsCancellation = false;
            result.WorkerReportsProgress = false;
            result.DoWork += BWorker_tweakerWebBrowser_load_DoWork;
            result.RunWorkerCompleted += BWorker_tweakerWebBrowser_load_RunWorkerCompleted;
            return result;
        }

        private void TweakerWebBrowser_LockedNavigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(delegate {
                try
                { System.Diagnostics.Process.Start(e.Url.OriginalString); }
                catch (Exception ex)
                { Leayal.Log.LogManager.GeneralLog.Print(ex); }
            }));
        }

        public void TweakerWebBrowser_IsLoading(bool theBool)
        {
            this.SyncContext.Post(new SendOrPostCallback(delegate {
                this.tweakerWebBrowserLoading.Visible = theBool;
                foreach (ContextMenu item in this.tweakerWebBrowserContextMenu.Items)
                    item.Enabled = !theBool;
            }), null);
        }
        #endregion

        ImageBrush asd;
        private void _player_FrameDraw(object sender, Leayal.Ugoira.WPF.FrameDrawEventArgs e)
        {
            if (asd == null)
            {
                asd = new ImageBrush();
                asd.Stretch = Stretch.None;
                asd.ImageSource = e.Image;
                this.Background = asd;
            }
            else
                asd.ImageSource = e.Image;
        }

        private void UpperGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (e.HeightChanged && e.NewSize.Height > 1)
            {
                if (element != null)
                {
                    this.buttonsRow.MinHeight = element.ActualHeight / 2;
                    this.buttonsRow.MaxHeight = element.ActualHeight - 10;
                }
                else
                {
                    this.buttonsRow.MinHeight = e.NewSize.Height / 2;
                    this.buttonsRow.MaxHeight = e.NewSize.Height - 10;
                }
                if (this.buttonsRow.ActualHeight < this.buttonsRow.MinHeight)
                    this.buttonsRow.Height = new GridLength(this.buttonsRow.MinHeight, GridUnitType.Pixel);
                else if (this.buttonsRow.ActualHeight > this.buttonsRow.MaxHeight)
                    this.buttonsRow.Height = new GridLength(this.buttonsRow.MaxHeight, GridUnitType.Pixel);
            }
        }

        private void LowerGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (e.WidthChanged && e.NewSize.Width > 1)
            {
                if (element != null)
                    this.richtextboxcolumn.MaxWidth = element.ActualWidth - 10;
                else
                    this.richtextboxcolumn.MaxWidth = e.NewSize.Width - 10;
                if (this.richtextboxcolumn.ActualWidth < this.richtextboxcolumn.MinWidth)
                    this.richtextboxcolumn.Width = new GridLength(this.richtextboxcolumn.MinWidth, GridUnitType.Pixel);
                else if (this.richtextboxcolumn.ActualWidth > this.richtextboxcolumn.MaxWidth)
                    this.richtextboxcolumn.Width = new GridLength(this.richtextboxcolumn.MaxWidth, GridUnitType.Pixel);
            }
        }

        public void PrintOut(Exception ex)
        {
            this.PrintOut(ex.ToString());
        }
        public void PrintOut(string text)
        {
            this.PrintOut(text, true);
        }
        public void PrintOut(params Run[] texts)
        {
            this.richtextbox_log.AppendText(texts, true);
        }
        public void PrintOut(string text, bool newline)
        {
            this.richtextbox_log.AppendText(text, newline);
        }
    }
}
