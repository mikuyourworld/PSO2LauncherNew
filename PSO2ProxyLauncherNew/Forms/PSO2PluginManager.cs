using PSO2ProxyLauncherNew.Classes.Components.PSO2Plugin;
using System;
using System.Windows.Forms;

namespace PSO2ProxyLauncherNew.Forms
{
    public partial class PSO2PluginManager : Form
    {
        public PSO2PluginManager()
        {
            InitializeComponent();
        }

        private void PSO2PluginManager_Load(object sender, EventArgs e)
        {

        }

        private void LoadingVisible(bool val)
        {
            this.elementHost1.Visible = val;
            if (val)
                this.elementHost1.BringToFront();
            else
                this.elementHost1.SendToBack();
        }

        private void PSO2PluginManager_Shown(object sender, EventArgs e)
        {
            Classes.Components.PSO2Plugin.PSO2PluginManager.Instance.CheckForPluginCompleted += this.Instance_CheckForPluginCompleted;
            Classes.Components.PSO2Plugin.PSO2PluginManager.Instance.PluginStatusChanged += this.Instance_PluginStatusChanged;
            if (Classes.Components.PSO2Plugin.PSO2PluginManager.Instance.IsBusy)
                this.LoadingVisible(true);
            else
            {
                this.RefreshPluginList();
                this.LoadingVisible(false);
            }
        }

        private void RefreshPluginList()
        {
            if (this.flowLayoutPanel1.Controls.Count > 0)
            {
                CheckBox cb;
                while (this.flowLayoutPanel1.Controls.Count > 0)
                {
                    cb = this.flowLayoutPanel1.Controls[0] as CheckBox;
                    if (cb != null)
                    {
                        cb.Click -= CButton_Click;
                        this.flowLayoutPanel1.Controls.Remove(cb);
                    }
                }
            }
            foreach (var item in Classes.Components.PSO2Plugin.PSO2PluginManager.Instance)
                if (item.Value.Toggleable)
                    this.flowLayoutPanel1.Controls.Add(CreateCheckBoxButton(item.Value, item.Value.Name));
            //oh lol
        }

        private CheckBox CreateCheckBoxButton(PSO2Plugin _Plugin, string _PluginName)
        {
            CheckBox cButton = new CheckBox();
            cButton.Text = _PluginName;
            cButton.Tag = _Plugin;
            cButton.AutoCheck = false;
            cButton.AutoSize = true;
            cButton.Checked = _Plugin.Enabled;
            cButton.Padding = new Padding(1);
            cButton.Click += CButton_Click;
            return cButton;
        }

        private void CButton_Click(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb != null)
            {
                PSO2Plugin plugin = cb.Tag as PSO2Plugin;
                if (plugin != null && plugin.Toggleable)
                {
                    this.LoadingVisible(true);
                    plugin.Enabled = !plugin.Enabled;
                }
            }
        }

        private void Instance_PluginStatusChanged(object sender, Classes.Events.PSO2PluginStatusChanged e)
        {
            this.LoadingVisible(false);
            if (e.Error != null)
                MessageBox.Show(e.Error.Message, e.Plugin.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                bool found = false;
                foreach (CheckBox cb in this.flowLayoutPanel1.Controls)
                    if (cb.Tag.Equals(e.Plugin))
                    {
                        found = true;
                        cb.Checked = e.Plugin.Enabled;
                    }
                if (!found)
                    this.flowLayoutPanel1.Controls.Add(CreateCheckBoxButton(e.Plugin, e.Plugin.Name));
            }
        }

        private void Instance_CheckForPluginCompleted(object sender, EventArgs e)
        {
            this.RefreshPluginList();
            this.LoadingVisible(false);
        }

        private void PSO2PluginManager_FormClosed(object sender, FormClosedEventArgs e)
        {
            Classes.Components.PSO2Plugin.PSO2PluginManager.Instance.CheckForPluginCompleted -= this.Instance_CheckForPluginCompleted;
            Classes.Components.PSO2Plugin.PSO2PluginManager.Instance.PluginStatusChanged -= this.Instance_PluginStatusChanged;
        }

        private void buttonForceCheck_Click(object sender, EventArgs e)
        {
            this.LoadingVisible(true);
            Classes.Components.PSO2Plugin.PSO2PluginManager.Instance.GetPluginList();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
