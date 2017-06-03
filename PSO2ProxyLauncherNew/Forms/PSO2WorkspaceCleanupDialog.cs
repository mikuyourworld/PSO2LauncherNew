using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SharpCompress.Common;
using SharpCompress.Compressors.Deflate;

namespace PSO2ProxyLauncherNew.Forms
{
    public partial class PSO2WorkspaceCleanupDialog : Form
    {
        //private Dictionary<string, CompressionType> cacheCompressionType;
        private Dictionary<string, CompressionLevel> cacheCompressionLevel;

        public PSO2WorkspaceCleanupDialog()
        {
            this._type = CompressionType.None;
            this._level = CompressionLevel.Default;
            /*this.cacheCompressionType = new Dictionary<string, CompressionType>(StringComparer.OrdinalIgnoreCase);
            this.cacheCompressionType.Add("None", CompressionType.None);
            this.cacheCompressionType.Add("Deflate", CompressionType.Deflate);
            this.cacheCompressionType.Add("LZMA", CompressionType.LZMA);
            this.cacheCompressionType.Add("BZip2", CompressionType.BZip2);
            this.cacheCompressionType.Add("PPMd", CompressionType.PPMd);//*/
            this.cacheCompressionLevel = new Dictionary<string, CompressionLevel>(StringComparer.OrdinalIgnoreCase);
            this.cacheCompressionLevel.Add("Normal", CompressionLevel.Default);
            this.cacheCompressionLevel.Add("Low compresstion (Fast)", CompressionLevel.BestSpeed);
            this.cacheCompressionLevel.Add("High compresstion (Slow)", CompressionLevel.BestCompression);
            InitializeComponent();
            this.radioButton2.Checked = true;
            foreach (string key in this.cacheCompressionLevel.Keys)
                this.comboBoxCompressionLevel.Items.Add(key);
            this.comboBoxCompressionLevel.SelectedIndex = 0;
        }

        private CompressionLevel _level;
        public CompressionLevel CompressionLevel => this._level;
        private CompressionType _type;
        public CompressionType CompressionType => this._type;

        private void PSO2WorkspaceCleanupDialog_Load(object sender, EventArgs e)
        {
            
        }

        private void comboBoxCompressionLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cacheCompressionLevel.TryGetValue(this.comboBoxCompressionLevel.SelectedText, out var value))
                this._level = value;
            else
                this._level = CompressionLevel.Default;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            this.comboBoxCompressionLevel.Enabled = this.radioButton2.Checked;
            this.label3.Enabled = this.radioButton2.Checked;
            if (this.radioButton2.Checked)
                this._type = CompressionType.Deflate;
            else
                this._type = CompressionType.None;
        }

        private void checkBoxCreateBackup_CheckedChanged(object sender, EventArgs e)
        {
            this.groupBox1.Enabled = this.checkBoxCreateBackup.Checked;
        }

        public bool CreateBackup
        {
            get { return this.checkBoxCreateBackup.Checked; }
            // set { this.checkBoxCreateBackup.Checked = value; }
        }

        public bool StrictClean
        {
            get { return this.checkBoxStrictClean.Checked; }
            // set { this.checkBoxStrictClean.Checked = value; }
        }
        
        public string BackupPath => this.textBoxArchivePath.Text;

        private void buttonBrowseSaveArchive_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Title = "Select destination for backup archive";
                sfd.RestoreDirectory = true;
                sfd.OverwritePrompt = true;
                sfd.Filter = "ZIP Archive (*.zip)|*.zip";
                sfd.CheckFileExists = false;
                sfd.CheckPathExists = false;
                sfd.DefaultExt = "zip";
                sfd.FileName = "SEGA_PSO2_" + DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss");
                if (sfd.ShowDialog(this) == DialogResult.OK)
                {
                    this.textBoxArchivePath.Text = sfd.FileName;
                }
            }
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (this.CreateBackup)
            {
                if (!string.IsNullOrWhiteSpace(this.textBoxArchivePath.Text))
                {
                    if (!System.IO.Path.IsPathRooted(this.textBoxArchivePath.Text))
                        MessageBox.Show(this, $"This path is not valid:\n{this.textBoxArchivePath.Text}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        this.DialogResult = DialogResult.OK;
                }
                else
                    MessageBox.Show(this, "This path is not valid:\nArchive Path is empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
                this.DialogResult = DialogResult.OK;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
