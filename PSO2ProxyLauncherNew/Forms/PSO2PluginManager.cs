using PSO2ProxyLauncherNew.Classes.PSO2.PSO2Plugin;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PSO2ProxyLauncherNew.Classes.Components;
using System.ComponentModel;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using PSO2ProxyLauncherNew.Classes.Controls;

namespace PSO2ProxyLauncherNew.Forms
{
    partial class PSO2PluginManager : AnotherExtendedForm
    {

        private static Classes.Components.AsyncForms.FormThreadInfo _formInfo;
        public static Classes.Components.AsyncForms.FormThreadInfo FormInfo
        {
            get
            {
                if (_formInfo == null)
                    _formInfo = AsyncForm.Get(new PSO2PluginManager());
                return _formInfo;
            }
        }


        private Dictionary<PSO2Plugin, CheckBox> innerDict;
        private ExtendedToolTip _tooltip;

        private PSO2PluginManager()
        {
            this.innerDict = new Dictionary<PSO2Plugin, CheckBox>();
            this._tooltip = new ExtendedToolTip();
            this._tooltip.PreferedSize = new Size(300, 600);
            this._tooltip.ForeColor = Color.FromArgb(255, 255, 255);
            this._tooltip.BackColor = Color.FromArgb(17, 17, 17);
            this._tooltip.Opacity = 0.75F;
            this._tooltip.Font = new Font(this.Font.FontFamily, 9F, FontStyle.Regular);
            this._tooltip.Popup += this.Tooltip_Popup;
            this._tooltip.Draw += this.Tooltip_Draw;
            InitializeComponent();
        }

        private void PSO2PluginManager_Load(object sender, EventArgs e)
        {

        }

        private void LoadingVisible(System.Threading.SynchronizationContext sync, bool val)
        {
            sync.Post(new System.Threading.SendOrPostCallback(delegate {
                this.elementHost1.Visible = val;
                if (val)
                    this.elementHost1.BringToFront();
                else
                    this.elementHost1.SendToBack();
            }), null);
        }

        private void LoadingVisible(bool val)
        {
            this.LoadingVisible(this.SyncContext, val);
        }

        private void PSO2PluginManager_Shown(object sender, EventArgs e)
        {
            Classes.PSO2.PSO2Plugin.PSO2PluginManager.Instance.CheckForPluginCompleted += this.Instance_CheckForPluginCompleted;
            Classes.PSO2.PSO2Plugin.PSO2PluginManager.Instance.PluginStatusChanged += this.Instance_PluginStatusChanged;
            if (Classes.PSO2.PSO2Plugin.PSO2PluginManager.Instance.IsBusy)
                this.LoadingVisible(true);
            else
            {
                this.RefreshPluginList();
                this.LoadingVisible(false);
            }
        }

        private void RefreshPluginList()
        {
            this.SyncContext.Send(new SendOrPostCallback(delegate {
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
                            cb.Tag = null;
                            cb.Dispose();
                            this.SyncContext.Send(new SendOrPostCallback(delegate {

                            }), null);
                        }
                    }
                }
            }), null);
            this.innerDict.Clear();
            foreach (var item in Classes.PSO2.PSO2Plugin.PSO2PluginManager.Instance)
                if (item.Value.Toggleable)
                    this.SyncContext.Send(new SendOrPostCallback(delegate { this.flowLayoutPanel1.Controls.Add(CreateCheckBoxButton(item.Value, item.Value.Name)); }), null);
        }

        private CheckBox CreateCheckBoxButton(PSO2Plugin _Plugin, string _PluginName)
        {
            CheckBox cButton = new CheckBox();
            cButton.Text = _PluginName;
            cButton.Tag = _Plugin;
            if (!_Plugin.Managed)
                cButton.ForeColor = System.Drawing.Color.Red;
            cButton.AutoCheck = false;
            cButton.AutoSize = true;
            cButton.Checked = _Plugin.Enabled;
            cButton.Padding = new Padding(1);
            cButton.Click += CButton_Click;
            this._tooltip.SetToolTip(cButton, _Plugin.GetToolTip(this._tooltip.Font, this._tooltip.PreferedSize.Width, TextFormatFlags.Left).Result);
            this.innerDict.Add(_Plugin, cButton);
            return cButton;
        }

        private void Tooltip_Popup(object sender, PopupEventArgs e)
        {
            PSO2Plugin _plug = e.AssociatedControl.Tag as PSO2Plugin;
            if (_plug != null)
            {
                if (!_plug.Managed)
                {
                    var _stringSize = _plug.GetToolTip(true, this._tooltip.Font, this._tooltip.PreferedSize.Width, TextFormatFlags.Left);

                    Rectangle rect1 = new Rectangle(0, 0, _stringSize.Size.Width, _stringSize.Size.Height);
                    using (Font f = new Font(this._tooltip.Font.FontFamily, this._tooltip.Font.Size, FontStyle.Bold))
                    {
                        _stringSize = Classes.Infos.CommonMethods.WrapString(_plug.GetWarningMessage(), this._tooltip.PreferedSize.Width, f, TextFormatFlags.Left);
                        e.ToolTipSize = new Size(Math.Max(rect1.Width, _stringSize.Size.Width) + 4, rect1.Height + _stringSize.Size.Height + 4);
                    }
                }
                else
                {
                    var _stringSize = _plug.GetToolTip(true, this._tooltip.Font, this._tooltip.PreferedSize.Width, TextFormatFlags.Left);
                    e.ToolTipSize = new Size(_stringSize.Size.Width + 4, _stringSize.Size.Height + 4);
                }
            }
        }

        QuickBitmap bm;
        DrawToolTipEventArgs ee;
        private void Tooltip_Draw(object sender, DrawToolTipEventArgs e)
        {
            using (bm = new QuickBitmap(e.Bounds.Size))
            {
                e.Graphics.Clear(this._tooltip.BackColor);
                ee = new DrawToolTipEventArgs(bm.Graphics, e.AssociatedWindow, e.AssociatedControl, new Rectangle(0, 0, e.Bounds.Width, e.Bounds.Height), e.ToolTipText, this._tooltip.BackColor, this._tooltip.ForeColor, this._tooltip.Font);
                ee.Graphics.Clear(Color.Transparent);
                ee.DrawBorder();
                //e.Graphics.CopyFromScreen(new Point(e.Bounds.X + MousePosition.X, e.Bounds.Y + MousePosition.Y), Point.Empty, e.Bounds.Size, CopyPixelOperation.SourceCopy);
                ee.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                ee.Bounds.Offset(2, 2);
                ee.Bounds.Inflate(-4, -4);
                PSO2Plugin _plug = ee.AssociatedControl.Tag as PSO2Plugin;
                if (_plug != null && !_plug.Managed)
                {
                    var _stringSize = _plug.GetToolTip(true, this._tooltip.Font, this._tooltip.PreferedSize.Width, TextFormatFlags.Left);                    
                    Rectangle rect = new Rectangle(ee.Bounds.X, ee.Bounds.Y, _stringSize.Size.Width, _stringSize.Size.Height);
                    TextRenderer.DrawText(ee.Graphics, _stringSize.Result, this._tooltip.Font, rect, this._tooltip.ForeColor, this._tooltip.BackColor, TextFormatFlags.Left);
                    using (Font f = new Font(this._tooltip.Font.FontFamily, this._tooltip.Font.Size, FontStyle.Bold))
                    {
                        _stringSize = Classes.Infos.CommonMethods.WrapString(_plug.GetWarningMessage(), this._tooltip.PreferedSize.Width, f, TextFormatFlags.Left);
                        TextRenderer.DrawText(ee.Graphics, _stringSize.Result, f, new Rectangle(ee.Bounds.X, ee.Bounds.Y + rect.Height, _stringSize.Size.Width, _stringSize.Size.Height), Color.Red, this._tooltip.BackColor, TextFormatFlags.Left);
                    }
                }
                else
                    TextRenderer.DrawText(ee.Graphics, ee.ToolTipText, this._tooltip.Font, ee.Bounds, Color.FromArgb(255, 255, 255), Color.FromArgb(17, 17, 17), TextFormatFlags.Left);
                e.Graphics.DrawImage(bm.Bitmap, e.Bounds, ee.Bounds, GraphicsUnit.Pixel);
            }
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

        private void Instance_PluginStatusChanged(SynchronizationContext sync, object sender, Classes.Events.PSO2PluginStatusChanged e)
        {
            sync.Post(new System.Threading.SendOrPostCallback(delegate {
                if (e.Error != null)
                    MessageBox.Show(e.Error.Message, e.Plugin.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    if (this.innerDict.ContainsKey(e.Plugin))
                        this.innerDict[e.Plugin].Checked = e.Plugin.Enabled;
                    else
                        this.flowLayoutPanel1.Controls.Add(CreateCheckBoxButton(e.Plugin, e.Plugin.Name));
                }
            }), null);
        }

        private void Instance_PluginStatusChanged(object sender, Classes.Events.PSO2PluginStatusChanged e)
        {
            this.LoadingVisible(false);
            this.Instance_PluginStatusChanged(SyncContext, sender, e);
        }

        private void Instance_CheckForPluginCompleted(object sender, EventArgs e)
        {
            this.RefreshPluginList();
            this.LoadingVisible(false);
        }

        private void PSO2PluginManager_FormClosed(object sender, FormClosedEventArgs e)
        {
            Classes.PSO2.PSO2Plugin.PSO2PluginManager.Instance.CheckForPluginCompleted -= this.Instance_CheckForPluginCompleted;
            Classes.PSO2.PSO2Plugin.PSO2PluginManager.Instance.PluginStatusChanged -= this.Instance_PluginStatusChanged;
            this.innerDict.Clear();
            this._tooltip.Hide();
            this._tooltip.Dispose();
            this.Dispose();
        }

        private void buttonForceCheck_Click(object sender, EventArgs e)
        {
            this.LoadingVisible(true);
            Classes.PSO2.PSO2Plugin.PSO2PluginManager.Instance.GetPluginList();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
