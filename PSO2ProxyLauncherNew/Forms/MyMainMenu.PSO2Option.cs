using PSO2ProxyLauncherNew.Classes.PSO2.PSO2UserConfiguration;
using System;
using System.IO;
using PSO2ProxyLauncherNew.Classes.PSO2;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using PSO2ProxyLauncherNew.Classes;
using System.Drawing;
using Leayal.Drawing;
using Leayal.Forms;

namespace PSO2ProxyLauncherNew.Forms
{
    public partial class MyMainMenu
    {

        private PSO2UserConfiguration pso2configFile;
        private ScreenResolutionConverter screenresolutionconverter;

        private List<string> screenResList;
        private Dictionary<string, ScreenMode> screenModeList;
        private Dictionary<string, int> framerateList;
        private FakeCheckBox[] graphicsNormal, graphicsHigh;
        private bool pso2optionLoading;
        private Leayal.Forms.ExtendedToolTip pso2optionToolTip;

        public void LoadPSO2Setting(string filepath)
        {
            if (pso2configFile != null)
                pso2configFile.Dispose();
            if (!string.IsNullOrEmpty(filepath))
                pso2configFile = new PSO2UserConfiguration(RawPSO2UserConfiguration.FromFile(filepath));
            else
                pso2configFile = new PSO2UserConfiguration();
            this.PSO2OptionPanel_Load();
        }

        private bool pso2OptionPanel_isInitialize;
        private void PSO2OptionPanel_Initialize()
        {
            if (pso2OptionPanel_isInitialize) return;
            pso2OptionPanel_isInitialize = true;
            this.pso2optionToolTip = new Leayal.Forms.ExtendedToolTip();
            this.pso2optionToolTip.UseFading = true;
            this.pso2optionToolTip.BackColor = Color.FromArgb(17, 17, 17);
            this.pso2optionToolTip.Font = new Font(this.Font.FontFamily, 10F);
            this.pso2optionToolTip.ForeColor = Color.FromArgb(254, 254, 254);
            this.pso2optionToolTip.FormColor = this.optionToolTip.BackColor;
            this.pso2optionToolTip.PreferedSize = new Size(300, 400);
            this.pso2optionToolTip.Opacity = 0.75F;
            this.pso2optionToolTip.Popup += this.Pso2optionToolTip_Popup;
            this.pso2optionToolTip.Draw += this.Pso2optionToolTip_Draw;

            this.pso2optionToolTip.SetToolTip(this.pso2optionDetailedModelNum, LanguageManager.GetMessageText("PSO2Option_DetailedModelNum", "Determine how many model will be loaded in the screen, any model which not detailed one will be the \"gray models\"."));
            this.pso2optionToolTip.SetToolTip(this.pso2optioncomboBoxScreenRes, LanguageManager.GetMessageText("PSO2Option_ScreenRes", "Set the game screen resolution."));
            this.pso2optionToolTip.SetToolTip(this.pso2optioncomboBoxScreenMode, LanguageManager.GetMessageText("PSO2Option_ScreenMode", "Set the game screen mode."));
            this.pso2optionToolTip.SetToolTip(this.pso2optionFPS, LanguageManager.GetMessageText("PSO2Option_FPS", "Set the Frame-per-second limit."));
            this.pso2optionToolTip.SetToolTip(this.pso2optionTextSize, LanguageManager.GetMessageText("PSO2Option_UIScale", "Set the interface scale. If the screen resolution is lower than the fit width, this setting will be skipped.") + "\r\n" + LanguageManager.GetMessageText("PSO2Option_UIScaleWarn", "If you don't know what this is, please leave this at 'Default'."));
            this.pso2optionToolTip.SetToolTip(this.pso2optionRarityNotify, LanguageManager.GetMessageText("PSO2Option_RarityNotify", "Set the in-game Rare Item Drop notification that it will only notify according to the setting.\n(Can be changed while playing in-game)"));
            
            this.screenResList = new List<string>(new string[] { "640x480", "800x600", "1024x576", "1024x600", "1024x768", "1280x720", "1280x800", "1280x960" ,"1280x1024",
                "1366x768", "1440x900", "1400x1050", "1600x900", "1600x1200","1680x1050","1920x1080","1920x1200"});

            this.pso2optioncomboBoxScreenRes.Items.Clear();
            for (int i = 0; i < screenResList.Count; i++)
                this.pso2optioncomboBoxScreenRes.Items.Add(screenResList[i]);

            this.screenModeList = new Dictionary<string, ScreenMode>();
            this.pso2optioncomboBoxScreenMode.Items.Clear();
            string[] thenames = Enum.GetNames(typeof(ScreenMode));
            for (int i = 0; i < thenames.Length; i++)
            {
                this.screenModeList.Add(thenames[i], (ScreenMode)Enum.Parse(typeof(ScreenMode), thenames[i]));
                this.pso2optioncomboBoxScreenMode.Items.Add(thenames[i]);
            }

            this.optiontextureReduced.Tag = TextureQuality.Reduced;
            this.optiontextureNormal.Tag = TextureQuality.Normal;
            this.optiontextureHigh.Tag = TextureQuality.HighRes;

            this.optionShaderOff.Tag = ShaderQuality.Off;
            this.optionShaderNormal.Tag = ShaderQuality.Normal;
            this.optionShaderHigh.Tag = ShaderQuality.High;

            this.framerateList = new Dictionary<string, int>();
            this.framerateList.Add("30", 30);
            this.pso2optionFPS.Items.Add("30");
            this.framerateList.Add("60", 60);
            this.pso2optionFPS.Items.Add("60");
            this.framerateList.Add("90", 90);
            this.pso2optionFPS.Items.Add("90");
            this.framerateList.Add("120", 120);
            this.pso2optionFPS.Items.Add("120");
            this.framerateList.Add("Unlimited", 0);
            this.pso2optionFPS.Items.Add("Unlimited");

            EventHandler checkChangedEvent = new EventHandler(this.graphicsCheckBoxes_CheckedChanged);
            this.graphicsNormal = new FakeCheckBox[10];
            this.graphicsHigh = new FakeCheckBox[4];
            FakeCheckBox fcb = this.pso2optionGraphicsTable.Add(1, 0, "Blur", "pso2optionBlur");
            fcb.Tag = "Blur";
            fcb.CheckedChanged += checkChangedEvent;
            this.graphicsNormal[0] = fcb;
            fcb = this.pso2optionGraphicsTable.Add(0, 0, "Anti-Aliasing", "pso2optionAntiAliasing");
            fcb.Tag = "AntiAliasing";
            fcb.CheckedChanged += checkChangedEvent;
            this.graphicsNormal[1] = fcb;
            fcb = this.pso2optionGraphicsTable.Add(2, 0, "Light Source Effect", "pso2optionLightEffect");
            fcb.Tag = "LightEffect";
            fcb.CheckedChanged += checkChangedEvent;
            this.graphicsNormal[2] = fcb;
            fcb = this.pso2optionGraphicsTable.Add(3, 0, "Terrain Lighting", "pso2optionLightGeoGraphy");
            fcb.Tag = "LightGeoGraphy";
            fcb.CheckedChanged += checkChangedEvent;
            this.graphicsNormal[3] = fcb;
            fcb = this.pso2optionGraphicsTable.Add(0, 1, "Detailed Effects", "pso2optionEffectDraw");
            fcb.Tag = "EffectDraw";
            fcb.CheckedChanged += checkChangedEvent;
            this.graphicsNormal[4] = fcb;
            fcb = this.pso2optionGraphicsTable.Add(1, 1, "Depth of Field", "pso2optionDepth");
            fcb.Tag = "Depth";
            fcb.CheckedChanged += checkChangedEvent;
            this.graphicsNormal[5] = fcb;
            fcb = this.pso2optionGraphicsTable.Add(2, 1, "Reflection", "pso2optionReflection");
            fcb.Tag = "Reflection";
            fcb.CheckedChanged += checkChangedEvent;
            this.graphicsNormal[6] = fcb;
            fcb = this.pso2optionGraphicsTable.Add(3, 1, "LightShaft", "pso2optionLightShaft");
            fcb.Tag = "LightShaft";
            fcb.CheckedChanged += checkChangedEvent;
            this.graphicsNormal[7] = fcb;
            fcb = this.pso2optionGraphicsTable.Add(0, 2, "Bloom", "pso2optionBloom");
            fcb.Tag = "Bloom";
            fcb.CheckedChanged += checkChangedEvent;
            this.graphicsNormal[8] = fcb;
            fcb = this.pso2optionGraphicsTable.Add(1, 2, "Soft Particles", "pso2optionSoftParticle");
            fcb.Tag = "SoftParticle";
            fcb.CheckedChanged += checkChangedEvent;
            this.graphicsNormal[9] = fcb;

            fcb = this.pso2optionGraphicsTable.Add(0, 3, "Ambient Occlusion", "pso2optionAmbientOcclusion");
            fcb.Tag = "AmbientOcclusion";
            fcb.CheckedChanged += checkChangedEvent;
            this.graphicsHigh[0] = fcb;
            fcb = this.pso2optionGraphicsTable.Add(1, 3, "Peripheral Dimming", "pso2optionVignetting");
            fcb.Tag = "Vignetting";
            fcb.CheckedChanged += checkChangedEvent;
            this.graphicsHigh[1] = fcb;
            fcb = this.pso2optionGraphicsTable.Add(2, 3, "Color Tone Curve", "pso2optionColorToneCurve");
            fcb.Tag = "ColorToneCurve";
            fcb.CheckedChanged += checkChangedEvent;
            this.graphicsHigh[2] = fcb;
            fcb = this.pso2optionGraphicsTable.Add(3, 3, "Edge Lighting", "pso2optionEdgeLight");
            fcb.Tag = "EdgeLight";
            fcb.CheckedChanged += checkChangedEvent;
            this.graphicsHigh[3] = fcb;

            this.pso2optionRarityNotify.Items.Clear();
            this.pso2optionRarityNotify.Items.Add("7★ or above");
            this.pso2optionRarityNotify.Items.Add("10★ or above");
            this.pso2optionRarityNotify.Items.Add("13★ or above");

            this.pso2optionTextSize.Items.Clear();
            this.pso2optionTextSize.Items.Add("Default (fits 1280-pixel width)");
            this.pso2optionTextSize.Items.Add("x1.25 (fits 1600-pixel width)");
            this.pso2optionTextSize.Items.Add("x1.50 (fits 1920-pixel width)");
        }

        private void Pso2optionToolTip_Popup(object sender, Leayal.Forms.ExPopupEventArgs e)
        {
            if (e.AssociatedControl is ComboBox)
                e.Location = new Point(e.AssociatedControl.PointToScreen(new Point(e.AssociatedControl.Width, 0)).X, e.Location.Y);
            if (e.AssociatedControl.Equals(this.pso2optionTextSize))
            {
                e.ToolTipSize = Leayal.Forms.TextRendererWrapper.WrapString(this.pso2optionToolTip.GetToolTipText(this.pso2optionTextSize), this.pso2optionToolTip.PreferedSize.Width, this.pso2optionToolTip.Font, TextFormatFlags.Left).Size;
            }
        }

        QuickBitmap bm;
        private void Pso2optionToolTip_Draw(object sender, DrawToolTipEventArgs e)
        {
            using (bm = new QuickBitmap(e.Bounds.Size))
            {
                if (e.AssociatedControl.Equals(this.pso2optionTextSize))
                {
                    var _stringSize = Leayal.Forms.TextRendererWrapper.WrapString(LanguageManager.GetMessageText("PSO2Option_UIScale", "Set the interface scale. If the screen resolution is lower than the fit width, this setting will be skipped."), this.pso2optionToolTip.PreferedSize.Width, this.pso2optionToolTip.Font, TextFormatFlags.Left);
                    TextRenderer.DrawText(e.Graphics, _stringSize.Result, this.pso2optionToolTip.Font, new Rectangle(e.Bounds.Location, _stringSize.Size), this.pso2optionToolTip.ForeColor, this.pso2optionToolTip.BackColor, TextFormatFlags.Left);

                    Point thePointHuh = new Point(e.Bounds.X, _stringSize.Size.Height);
                    _stringSize = Leayal.Forms.TextRendererWrapper.WrapString(LanguageManager.GetMessageText("PSO2Option_UIScaleWarn", "If you don't know what this is, please leave this at 'Default'."), this.pso2optionToolTip.PreferedSize.Width, this.pso2optionToolTip.Font, TextFormatFlags.Left);
                    TextRenderer.DrawText(e.Graphics, _stringSize.Result, this.pso2optionToolTip.Font, new Rectangle(thePointHuh, TextRenderer.MeasureText(e.Graphics, _stringSize.Result, this.pso2optionToolTip.Font, e.Bounds.Size, TextFormatFlags.Left)), Color.Red, this.pso2optionToolTip.BackColor, TextFormatFlags.Left);
                }
                else
                {
                    TextRenderer.DrawText(e.Graphics, e.ToolTipText, this.pso2optionToolTip.Font, e.Bounds, Color.FromArgb(255, 255, 255), Color.FromArgb(17, 17, 17), TextFormatFlags.Left);
                }
            }
        }

        private void pso2optionTextSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (pso2optionLoading) return;
            pso2configFile.InterfaceSize = (InterfaceSize)pso2optionTextSize.SelectedIndex;
            this.pso2optionbuttonSave.Enabled = true;
        }

        private void graphicsCheckBoxes_CheckedChanged(object sender, EventArgs e)
        {
            if (pso2optionLoading) return;
            FakeCheckBox fcb = sender as FakeCheckBox;
            if (fcb != null)
            {
                pso2configFile.SetDrawFunctionValue((string)fcb.Tag, fcb.Checked);
                this.pso2optionbuttonSave.Enabled = true;
            }
        }

        private void RadioGroup_optiontexture(object sender, EventArgs e)
        {
            if (pso2optionLoading) return;
            Control cc = sender as Control;
            if (cc!= null)
            {
                TextureQuality? asdasd = cc.Tag as TextureQuality?;
                if (asdasd.HasValue)
                    pso2configFile.TextureQuality = asdasd.Value;
                this.pso2optionbuttonSave.Enabled = true;
            }
        }

        private void RadioGroup_optionShader(object sender, EventArgs e)
        {
            Control cc = sender as Control;
            ShaderQuality? asdasd = cc.Tag as ShaderQuality?;
            if (cc != null && asdasd.HasValue)
                switch (asdasd.Value)
                {
                    case ShaderQuality.High:
                        this.SetEnabledControls(true, graphicsNormal);
                        this.SetEnabledControls(true, graphicsHigh);
                        break;
                    case ShaderQuality.Normal:
                        this.SetEnabledControls(true, graphicsNormal);
                        this.SetEnabledControls(false, graphicsHigh);
                        break;
                    default:
                        this.SetEnabledControls(false, graphicsNormal);
                        this.SetEnabledControls(false, graphicsHigh);
                        break;
                }
            if (pso2optionLoading) return;            
            if (cc != null)
            {
                if (asdasd.HasValue)
                    pso2configFile.ShaderQuality = asdasd.Value;
                this.pso2optionbuttonSave.Enabled = true;
            }
        }

        private void pso2optionPlayMovie_CheckedChanged(object sender, EventArgs e)
        {
            if (pso2optionLoading) return;
            pso2configFile.MoviePlay = this.pso2optionPlayMovie.Checked;
            this.pso2optionbuttonSave.Enabled = true;
        }

        private void PSO2OptionPanel_Load()
        {
            pso2optionLoading = true;
            int dlawhgliahwg = this.screenResList.IndexOf(pso2configFile.ScreenResolution.ToString());
            if (dlawhgliahwg > -1)
                this.pso2optioncomboBoxScreenRes.SelectedIndex = dlawhgliahwg;
            else
                this.pso2optioncomboBoxScreenRes.SelectedIndex = 0;

            this.pso2optioncomboBoxScreenMode.SelectedItem = pso2configFile.ScreenMode.ToString();
            switch (pso2configFile.ShaderQuality)
            {
                case ShaderQuality.High:
                    this.optionShaderHigh.Checked = true;
                    break;
                case ShaderQuality.Normal:
                    this.optionShaderNormal.Checked = true;
                    break;
                default:
                    this.optionShaderOff.Checked = true;
                    break;
            }
            switch (pso2configFile.TextureQuality)
            {
                case TextureQuality.HighRes:
                    this.optiontextureHigh.Checked = true;
                    break;
                case TextureQuality.Normal:
                    this.optiontextureNormal.Checked = true;
                    break;
                default:
                    this.optiontextureReduced.Checked = true;
                    break;
            }
            this.pso2optionPlayMovie.Checked = pso2configFile.MoviePlay;
            pso2optionDetailedModelNum.Value = pso2configFile.DetailedModelCount;

            string tmpString = pso2configFile.FrameKeep.ToString();
            if (pso2configFile.FrameKeep == 0)
                pso2optionFPS.SelectedItem = "Unlimited";
            else if (this.framerateList != null && this.framerateList.ContainsKey(tmpString))
                pso2optionFPS.SelectedItem = tmpString;
            else
                pso2optionFPS.SelectedItem = "60";

            pso2optionAutoPickMst.Checked = pso2configFile.MesetaPickUp;

            for (int i = 0; i < this.graphicsNormal.Length; i++)
                this.graphicsNormal[i].Checked = pso2configFile.GetDrawFunctionValue((string)this.graphicsNormal[i].Tag);
            for (int i = 0; i < this.graphicsHigh.Length; i++)
                this.graphicsHigh[i].Checked = pso2configFile.GetDrawFunctionValue((string)this.graphicsHigh[i].Tag);

            this.pso2optionRarityNotify.SelectedIndex = ((int)pso2configFile.RareDropLevelType);
            pso2optionTextSize.SelectedIndex = (int)pso2configFile.InterfaceSize;

            pso2optionLoading = false;
        }
        private void pso2optionRarityNotify_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (pso2optionLoading || this.pso2optionRarityNotify.Items.Count == 0) return;
            pso2configFile.RareDropLevelType = (RareDropLevelType)this.pso2optionRarityNotify.SelectedIndex;
            this.pso2optionbuttonSave.Enabled = true;
        }

        private void pso2optionAutoPickMst_CheckedChanged(object sender, EventArgs e)
        {
            if (pso2optionLoading) return;
            pso2configFile.MesetaPickUp = this.pso2optionAutoPickMst.Checked;
            this.pso2optionbuttonSave.Enabled = true;
        }

        private void pso2optioncomboBoxScreenRes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (pso2optionLoading) return;
            if (screenresolutionconverter == null)
                screenresolutionconverter = new ScreenResolutionConverter();
            pso2configFile.ScreenResolution = (ScreenResolution)screenresolutionconverter.ConvertFromString((string)this.pso2optioncomboBoxScreenRes.SelectedItem);
            this.pso2optionbuttonSave.Enabled = true;
        }

        private void pso2optioncomboBoxScreenMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (pso2optionLoading) return;
            string derp = pso2optioncomboBoxScreenMode.SelectedItem as string;
            if (this.screenModeList.ContainsKey(derp))
                pso2configFile.ScreenMode = this.screenModeList[derp];
            else
                pso2configFile.ScreenMode = ScreenMode.Windowed;
            this.pso2optionbuttonSave.Enabled = true;
        }

        private void pso2optionFPS_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (pso2optionLoading) return;
            string laiwhgliahwg = pso2optionFPS.SelectedItem as string;
            if (this.framerateList != null && this.framerateList.ContainsKey(laiwhgliahwg))
                pso2configFile.FrameKeep = this.framerateList[laiwhgliahwg];
            else
                pso2configFile.FrameKeep = 60;
            this.pso2optionbuttonSave.Enabled = true;
        }

        private void pso2optionLoD_ValueChanged(object sender, EventArgs e)
        {
            if (pso2optionLoading) return;
            pso2configFile.DetailedModelCount = pso2optionDetailedModelNum.Value;
            this.pso2optionbuttonSave.Enabled = true;
        }

        private void buttonPSO2Option_Click(object sender, EventArgs e)
        {
            if (File.Exists(DefaultValues.Directory.UserSettingPath))
            {
                this.pso2optionbuttonSave.Enabled = false;
                this.LoadPSO2Setting(DefaultValues.Directory.UserSettingPath);
            }
            else
            {
                this.pso2optionbuttonSave.Enabled = true;
                this.LoadPSO2Setting(null);
            }
            this.SelectedTab = this.panelPSO2Option;
        }

        private void pso2optionbuttonSave_Click(object sender, EventArgs e)
        {
            this.pso2optionbuttonSave.Enabled = false;
            this.SavePSO2Setting(DefaultValues.Directory.UserSettingPath);
            MetroFramework.MetroMessageBox.Show(this, LanguageManager.GetMessageText("PSO2Settings_SettingsSaved", "The settings have been saved."), "Settings saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void pso2optionbuttonClose_Click(object sender, EventArgs e)
        {
            this.pso2optionToolTip.Hide();
            this.SelectedTab = this.panelMainMenu;
            if (pso2configFile != null)
            {
                pso2configFile.Dispose();
                pso2configFile = null;
            }
        }
        private void SavePSO2Setting(string filepath)
        {
            if (pso2configFile != null)
                pso2configFile.SaveAs(filepath);
        }
    }
}
