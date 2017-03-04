using PSO2ProxyLauncherNew.Classes;
using System;
using System.Windows.Forms;

namespace PSO2ProxyLauncherNew.Forms
{
    public partial class PSO2ProxyInstallForm : Form
    {
        public PSO2ProxyInstallForm()
        {
            InitializeComponent();
        }

        private Uri _configurl;
        public Uri ConfigURL { get { return this._configurl; } }

        private string DNSResolveInside(string _original)
        {
            if (Classes.PSO2.PSO2Proxy.PSO2ProxyInstaller.URLInside.ContainsKey(_original))
                return Classes.PSO2.PSO2Proxy.PSO2ProxyInstaller.URLInside[_original];
            else
                return _original;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool IsSupportedProtocol(Uri url)
        {
            if (url.Scheme == Uri.UriSchemeHttp || url.Scheme == Uri.UriSchemeHttps || url.Scheme == Uri.UriSchemeFile)
                return true;
            else
                return false;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (Uri.IsWellFormedUriString(this.comboBox1.Text, UriKind.Absolute))
            {
                try
                {
                    Uri newUri = new Uri(this.comboBox1.Text);
                    if (this.IsSupportedProtocol(newUri))
                    {
                        this._configurl = new Uri(this.comboBox1.Text);
                        this.DialogResult = DialogResult.OK;
                    }
                    else
                        MessageBox.Show(this, string.Format(LanguageManager.GetMessageText("PSO2ProxyInstallForm_SchemeNotSupported", "Scheme '{0}' is not supported.\nPlease use common scheme like '" + Uri.UriSchemeHttp + "', '" + Uri.UriSchemeHttps + "', or '" + Uri.UriSchemeFile + "'"), newUri.Scheme), "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                catch (UriFormatException ex)
                {
                    this._configurl = null;
                    MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
                MessageBox.Show(this, LanguageManager.GetMessageText("PSO2ProxyInstallForm_InvalidURLform", "This URL is not a valid URL or not an absolute URL"), "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
    }
}
