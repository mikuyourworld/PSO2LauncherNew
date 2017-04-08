using PSO2ProxyLauncherNew.Classes.PSO2.PSO2UserConfiguration;
using System;
using System.IO;
using PSO2ProxyLauncherNew.Classes.PSO2;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using PSO2ProxyLauncherNew.Classes;

namespace PSO2ProxyLauncherNew.Forms
{
    public partial class MyMainMenu
    {

        private PSO2UserConfiguration pso2configFile;
        private ScreenResolutionConverter screenresolutionconverter;

        private List<string> screenResList;
        private Dictionary<string, ScreenMode> screenModeList;
        private Dictionary<string, int> framerateList;
        private bool pso2optionLoading;

        public void LoadPSO2Setting(string filepath)
        {
            if (pso2configFile != null)
                pso2configFile.Dispose();
            if (File.Exists(filepath))
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
            if (pso2optionLoading) return;
            Control cc = sender as Control;
            if (cc != null)
            {
                ShaderQuality? asdasd = cc.Tag as ShaderQuality?;
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
            pso2optionLoD.Value = pso2configFile.DrawLevel;
            pso2optionFPS.SelectedItem = pso2configFile.FrameKeep.ToString();
            pso2optionLoading = false;
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
            pso2configFile.DrawLevel = pso2optionLoD.Value;
            this.pso2optionbuttonSave.Enabled = true;
        }

        private void buttonPSO2Option_Click(object sender, EventArgs e)
        {
            this.pso2optionbuttonSave.Enabled = false;
            this.LoadPSO2Setting(DefaultValues.Directory.UserSettingPath);
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
