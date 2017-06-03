namespace PSO2ProxyLauncherNew.Forms
{
    partial class PSO2WorkspaceCleanupDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.checkBoxStrictClean = new System.Windows.Forms.CheckBox();
            this.checkBoxCreateBackup = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBoxCompressionLevel = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxArchivePath = new System.Windows.Forms.TextBox();
            this.buttonBrowseSaveArchive = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonStart = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBoxStrictClean
            // 
            this.checkBoxStrictClean.AutoSize = true;
            this.checkBoxStrictClean.Location = new System.Drawing.Point(12, 12);
            this.checkBoxStrictClean.Name = "checkBoxStrictClean";
            this.checkBoxStrictClean.Size = new System.Drawing.Size(222, 17);
            this.checkBoxStrictClean.TabIndex = 0;
            this.checkBoxStrictClean.Text = "Strict clean: Only clean PSO2-related files";
            this.checkBoxStrictClean.UseVisualStyleBackColor = true;
            // 
            // checkBoxCreateBackup
            // 
            this.checkBoxCreateBackup.AutoSize = true;
            this.checkBoxCreateBackup.Location = new System.Drawing.Point(12, 35);
            this.checkBoxCreateBackup.Name = "checkBoxCreateBackup";
            this.checkBoxCreateBackup.Size = new System.Drawing.Size(96, 17);
            this.checkBoxCreateBackup.TabIndex = 1;
            this.checkBoxCreateBackup.Text = "Create backup";
            this.checkBoxCreateBackup.UseVisualStyleBackColor = true;
            this.checkBoxCreateBackup.CheckedChanged += new System.EventHandler(this.checkBoxCreateBackup_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.radioButton2);
            this.groupBox1.Controls.Add(this.radioButton1);
            this.groupBox1.Controls.Add(this.comboBoxCompressionLevel);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBoxArchivePath);
            this.groupBox1.Controls.Add(this.buttonBrowseSaveArchive);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Enabled = false;
            this.groupBox1.Location = new System.Drawing.Point(12, 58);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(365, 99);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Backup option";
            // 
            // comboBoxCompressionLevel
            // 
            this.comboBoxCompressionLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCompressionLevel.FormattingEnabled = true;
            this.comboBoxCompressionLevel.Location = new System.Drawing.Point(101, 69);
            this.comboBoxCompressionLevel.Name = "comboBoxCompressionLevel";
            this.comboBoxCompressionLevel.Size = new System.Drawing.Size(121, 21);
            this.comboBoxCompressionLevel.TabIndex = 6;
            this.comboBoxCompressionLevel.SelectedIndexChanged += new System.EventHandler(this.comboBoxCompressionLevel_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Compression Level";
            // 
            // textBoxArchivePath
            // 
            this.textBoxArchivePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxArchivePath.Location = new System.Drawing.Point(80, 16);
            this.textBoxArchivePath.Name = "textBoxArchivePath";
            this.textBoxArchivePath.Size = new System.Drawing.Size(198, 20);
            this.textBoxArchivePath.TabIndex = 2;
            // 
            // buttonBrowseSaveArchive
            // 
            this.buttonBrowseSaveArchive.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowseSaveArchive.Location = new System.Drawing.Point(284, 14);
            this.buttonBrowseSaveArchive.Name = "buttonBrowseSaveArchive";
            this.buttonBrowseSaveArchive.Size = new System.Drawing.Size(75, 23);
            this.buttonBrowseSaveArchive.TabIndex = 1;
            this.buttonBrowseSaveArchive.Text = "Browse";
            this.buttonBrowseSaveArchive.UseVisualStyleBackColor = true;
            this.buttonBrowseSaveArchive.Click += new System.EventHandler(this.buttonBrowseSaveArchive_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "ZIP Path";
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.Location = new System.Drawing.Point(302, 163);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonStart
            // 
            this.buttonStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonStart.Location = new System.Drawing.Point(12, 163);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(127, 23);
            this.buttonStart.TabIndex = 4;
            this.buttonStart.Text = "Start Cleanup";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Compression Type";
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(101, 45);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(51, 17);
            this.radioButton1.TabIndex = 7;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "None";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(193, 45);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(59, 17);
            this.radioButton2.TabIndex = 8;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "Deflate";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // PSO2WorkspaceCleanupDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(389, 198);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.checkBoxCreateBackup);
            this.Controls.Add(this.checkBoxStrictClean);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MinimumSize = new System.Drawing.Size(405, 212);
            this.Name = "PSO2WorkspaceCleanupDialog";
            this.Opacity = 0.95D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Cleanup options";
            this.Load += new System.EventHandler(this.PSO2WorkspaceCleanupDialog_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxStrictClean;
        private System.Windows.Forms.CheckBox checkBoxCreateBackup;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonBrowseSaveArchive;
        private System.Windows.Forms.TextBox textBoxArchivePath;
        private System.Windows.Forms.ComboBox comboBoxCompressionLevel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.Label label2;
    }
}