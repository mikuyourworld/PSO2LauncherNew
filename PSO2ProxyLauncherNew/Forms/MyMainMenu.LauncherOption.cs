using Leayal.Forms;
using MetroFramework;
using PSO2ProxyLauncherNew.Classes;
using PSO2ProxyLauncherNew.Classes.Infos;
using System;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;

namespace PSO2ProxyLauncherNew.Forms
{
    public partial class MyMainMenu
    {
        private ExtendedToolTip optionToolTip;

        private void launcherOption_Click(object sender, EventArgs e)
        {
            this.RefreshOptionPanel();
            this.SelectedTab = this.panelOption;
        }

        private void optionButtonOK_Click(object sender, EventArgs e)
        {
            this.SaveOptionSettings();
            this.SelectedTab = this.panelMainMenu;
        }

        private void OptionPanel_Load()
        {
            if (DesignMode) return;
            if (this.optionToolTip == null)
            {
                this.optionToolTip = new ExtendedToolTip();
                this.optionToolTip.UseFading = true;
                this.optionToolTip.BackColor = Color.FromArgb(17, 17, 17);
                this.optionToolTip.Font = new Font(this.Font.FontFamily, 10F);
                this.optionToolTip.ForeColor = Color.FromArgb(254, 254, 254);
                this.optionToolTip.FormColor = this.optionToolTip.BackColor;
                this.optionToolTip.PreferedSize = new Size(300, 400);
                this.optionToolTip.Opacity = 0.75F;
                this.optionToolTip.Popup += this.OptionToolTip_Popup;
                this.optionToolTip.SetToolTip(this.optionComboBoxUpdateThread, LanguageManager.GetMessageText("OptionTooltip_UpdateThreads", "This option is to determine how many threads the launcher will use to check the game files while updating your game client.\nMore threads = cost more computer resource."));
                this.optionToolTip.SetToolTip(this.optioncomboBoxThrottleCache, LanguageManager.GetMessageText("OptionTooltip_UpdateThreadsThrottle", "This option is to throttle how fast the cache process will be to reduce CPU usage. Only avaiable if using update cache.\nSlower = cost less CPU usage."));
                this.optionToolTip.SetToolTip(this.optioncheckboxpso2updatecache, LanguageManager.GetMessageText("OptionTooltip_UpdateCache", "This option is to determine if the launcher should use update cache to speed up file checking."));
                this.optionToolTip.SetToolTip(this.optioncheckBoxMinimizeNetworkUsage, LanguageManager.GetMessageText("OptionTooltip_MinimizeNetworkUsage", "This option is to determine if the launcher should reduce network usage by reading the resource from cache."));

                this.optionToolTip.SetToolTip(this.optionSliderFormScale, LanguageManager.GetMessageText("OptionTooltip_SliderFormScale", "Set the launcher size scale factor.\nThis scale factor must be equal or higher than user's font scale settings.\nToo big will break launcher rendering. Be careful!"));
                this.optionToolTip.SetToolTip(this.optionbuttonResetBG, LanguageManager.GetMessageText("OptionTooltip_ResetBG", "Reset background image and background color to default."));
                this.optionToolTip.SetToolTip(this.optioncomboBoxBGImgMode, LanguageManager.GetMessageText("OptionTooltip_ImgMode", "Set the image layout for the custom background image."));
            }
        }

        private void OptionToolTip_Popup(object sender, ExPopupEventArgs e)
        {
            if (e.AssociatedControl is ComboBox)
                e.Location = new Point(e.AssociatedControl.PointToScreen(new Point(e.AssociatedControl.Width, 0)).X, e.Location.Y);
        }

        private void RefreshOptionPanel()
        {
            if (this.optionComboBoxUpdateThread.Items.Count != CommonMethods.MaxThreadsCount)
            {
                this.optionComboBoxUpdateThread.Items.Clear();
                if (CommonMethods.MaxThreadsCount == 1)
                {
                    this.optionComboBoxUpdateThread.Items.Add("1");
                    this.optionComboBoxUpdateThread.Enabled = false;
                }
                else
                {
                    for (int i = 1; i <= CommonMethods.MaxThreadsCount; i++)
                        this.optionComboBoxUpdateThread.Items.Add(i.ToString());
                    this.optionComboBoxUpdateThread.Enabled = true;
                }
            }
            this.optionComboBoxUpdateThread.SelectedItem = MySettings.GameClientUpdateThreads.ToString();
            int _threadspeedcount = (int)ThreadSpeed.ThreadSpeedCount;
            if (this.optioncomboBoxThrottleCache.Items.Count != _threadspeedcount)
            {
                this.optioncomboBoxThrottleCache.Items.Clear();
                for (int i = 0; i < _threadspeedcount; i++)
                    this.optioncomboBoxThrottleCache.Items.Add(((ThreadSpeed)i).ToString());
            }
            this.optioncomboBoxThrottleCache.SelectedItem = ((ThreadSpeed)MySettings.GameClientUpdateThrottleCache).ToString();

            this.optioncheckboxpso2updatecache.Checked = MySettings.GameClientUpdateCache;
            this.optioncheckBoxMinimizeNetworkUsage.Checked = MySettings.MinimizeNetworkUsage;

            this.optionSliderFormScale.MouseWheelBarPartitions = ((optionSliderFormScale.Maximum - optionSliderFormScale.Minimum) / 25);
        }

        private bool _appearenceChanged;
        private void SaveOptionSettings()
        {
            this.optionToolTip.Hide();
            MySettings.GameClientUpdateThreads = int.Parse(this.optionComboBoxUpdateThread.SelectedItem.ToString());
            MySettings.GameClientUpdateThrottleCache = (int)(Enum.Parse(typeof(ThreadSpeed), (string)this.optioncomboBoxThrottleCache.SelectedItem));
            MySettings.GameClientUpdateCache = this.optioncheckboxpso2updatecache.Checked;
            MySettings.MinimizeNetworkUsage = this.optioncheckBoxMinimizeNetworkUsage.Checked;
            if (this._appearenceChanged)
            {
                this._appearenceChanged = false;
                MySettings.LauncherBGColor = new Nullable<Color>(optionbuttonPickBackColor.BackColor);
                MySettings.LauncherForeColor = new Nullable<Color>(optionbuttonPickForeColor.BackColor);
                MySettings.HighlightTexts = this.optioncheckboxHighlightText.Checked;
                MySettings.LauncherBGlocation = optiontextBoxBGlocation.Text;
                MySettings.LauncherSizeScale = optionSliderFormScale.Value;
                MySettings.LauncherBGImgLayout = (ImageLayout)Enum.Parse(typeof(ImageLayout), (string)this.optioncomboBoxBGImgMode.SelectedItem, true);
                MetroMessageBox.Show(this, LanguageManager.GetMessageText("OptionAppearenceApplyNextBoot", "The appearence changes in your settings will be applied at next startup."), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private bool LoadingAppearenceOption;
        private void LoadAppearenceSetting()
        {
            LoadingAppearenceOption = true;

            float f = Classes.Infos.CommonMethods.GetResScale();
            if (f == 1F)
                this.Size = this.MinimumSize;
            else
                this.Size = new System.Drawing.Size(Convert.ToInt32(this.MinimumSize.Width * f), Convert.ToInt32(this.MinimumSize.Height * f));

            string[] names = Enum.GetNames(typeof(ImageLayout));
            if (this.optioncomboBoxBGImgMode.Items.Count != names.Length)
            {
                this.optioncomboBoxBGImgMode.Items.Clear();
                for (int i = 0; i < names.Length; i++)
                    this.optioncomboBoxBGImgMode.Items.Add(names[i]);
            }
            var myImglayout = MySettings.LauncherBGImgLayout;
            this.optioncomboBoxBGImgMode.SelectedItem = myImglayout.ToString();

            string bgloc = MySettings.LauncherBGlocation;
            Color? bgcolor = MySettings.LauncherBGColor;
            Color? forecolor = MySettings.LauncherForeColor;
            bool highlighttext = MySettings.HighlightTexts;
            if (!string.IsNullOrWhiteSpace(bgloc) && System.IO.File.Exists(bgloc))
            {
                Leayal.Drawing.MemoryImage mi = null;
                try
                {
                    mi = Leayal.Drawing.MemoryImage.FromFile(bgloc, false);
                    this.BackgroundImageLayout = myImglayout;
                    if (this.BackgroundImage != null)
                    {
                        Image asd = this.BackgroundImage;
                        this.BackgroundImage = mi.Image;
                        asd.Dispose();
                    }
                    else
                        this.BackgroundImage = mi.Image;
                    if (bgcolor != null && bgcolor.HasValue)
                        this.BackColor = bgcolor.Value;
                    if (forecolor != null && forecolor.HasValue)
                        this.ForeColor = forecolor.Value;
                    if (forecolor.HasValue || highlighttext)
                        foreach (Control cc in Leayal.Forms.FormWrapper.GetControls(this).Where((x)=> { return (x is CheckBox || x is RadioButton || x is Label || x is GroupBox); }))
                        {
                            if (cc is GroupBox)
                            {
                                if (forecolor.HasValue && cc.BackColor == Color.Transparent)
                                    cc.ForeColor = forecolor.Value;
                            }
                            else
                                if (cc.BackColor == Color.Transparent)
                                {
                                    if (highlighttext)
                                        cc.BackColor = Color.FromArgb(150, 255, 255, 255);
                                    if (forecolor.HasValue)
                                        cc.ForeColor = forecolor.Value;
                                }
                        }
                }
                catch (Exception ex)
                {
                    if (mi != null)
                        mi.Dispose();
                    Leayal.Log.LogManager.GeneralLog.Print(ex);
                }
            }
            optiontextBoxBGlocation.Text = bgloc;
            if (bgcolor != null && bgcolor.HasValue)
                optionbuttonPickBackColor.BackColor = bgcolor.Value;
            else
                optionbuttonPickBackColor.BackColor = this.BackColor;
            if (forecolor != null && forecolor.HasValue)
                optionbuttonPickForeColor.BackColor = forecolor.Value;
            else
                optionbuttonPickForeColor.BackColor = this.ForeColor;
            this.optioncheckboxHighlightText.Checked = highlighttext;
            
            optionSliderFormScale.Value = Convert.ToInt32(f * 100);
            LoadingAppearenceOption = false;
        }

        private void optioncheckboxpso2updatecache_CheckedChanged(object sender, EventArgs e)
        {
            this.optioncomboBoxThrottleCache.Enabled = this.optioncheckboxpso2updatecache.Checked;
        }

        private void SystemEvents_ScalingFactorChanged(object sender, EventArgs e)
        {
            this.optionSliderFormScale.ValueAvailableRange = new AvailableIntRange(Convert.ToInt32(Leayal.Forms.FormWrapper.ScalingFactor * 100), optionSliderFormScale.Maximum);
        }

        private void optioncomboBoxBGImgMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!LoadingAppearenceOption)
                this._appearenceChanged = true;
        }

        private void optionbuttonResetBG_Click(object sender, EventArgs e)
        {
            this._appearenceChanged = true;
            optiontextBoxBGlocation.Text = string.Empty;
            optionbuttonPickBackColor.BackColor = Color.FromArgb(17, 17, 17);
            optionbuttonPickForeColor.BackColor = Color.FromArgb(254, 254, 254);
        }
        
        private void optioncheckboxHighlightText_CheckedChanged(object sender, EventArgs e)
        {
            if (!LoadingAppearenceOption)
                this._appearenceChanged = true;
        }

        private void optionbuttonBrowseBG_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Select background image location";
                ofd.SupportMultiDottedExtensions = false;
                ofd.RestoreDirectory = true;
                ofd.AutoUpgradeEnabled = true;
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;
                ofd.Multiselect = false;
                if (!string.IsNullOrWhiteSpace(optiontextBoxBGlocation.Text) && System.IO.File.Exists(optiontextBoxBGlocation.Text))
                    ofd.FileName = optiontextBoxBGlocation.Text;
                using (Leayal.Forms.DialogFileFilterBuilder dffb = new DialogFileFilterBuilder())
                {
                    dffb.AppendAllSupportedTypes = AppendOrder.Last;
                    dffb.Append("Portable Network Graphics", "*.png");
                    dffb.Append("Bitmap Image", "*.bmp");
                    dffb.Append("JPEG Image", "*jpg", "*.jpeg");
                    ofd.Filter = dffb.ToFileFilterString();
                    ofd.FilterIndex = dffb.OutputCount;
                }
                if (ofd.ShowDialog(this) == DialogResult.OK)
                {
                    this._appearenceChanged = true;
                    optiontextBoxBGlocation.Text = ofd.FileName;
                }
            }
        }

        private void optionbuttonPickBackColor_Click(object sender, EventArgs e)
        {
            using (ColorDialog cd = new ColorDialog())
            {
                cd.AllowFullOpen = true;
                cd.AnyColor = true;
                cd.SolidColorOnly = true;
                cd.Color = optionbuttonPickBackColor.BackColor;
                if (cd.ShowDialog(this) == DialogResult.OK)
                {
                    this._appearenceChanged = true;
                    optionbuttonPickBackColor.BackColor = cd.Color;
                }
            }
        }

        private void optionbuttonPickForeColor_Click(object sender, EventArgs e)
        {
            using (ColorDialog cd = new ColorDialog())
            {
                cd.AllowFullOpen = true;
                cd.AnyColor = true;
                cd.SolidColorOnly = true;
                cd.Color = optionbuttonPickBackColor.BackColor;
                if (cd.ShowDialog(this) == DialogResult.OK)
                {
                    this._appearenceChanged = true;
                    optionbuttonPickForeColor.BackColor = cd.Color;
                }
            }
        }

        private void colorSlider1_ValueChanged(object sender, EventArgs e)
        {
            if (!LoadingAppearenceOption)
                this._appearenceChanged = true;
        }

        private enum ThreadSpeed : int { Fastest, Faster, Normal, Slower, Slowest, ThreadSpeedCount }
    }
}
