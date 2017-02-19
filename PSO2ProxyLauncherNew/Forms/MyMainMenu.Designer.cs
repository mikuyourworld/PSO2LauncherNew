namespace PSO2ProxyLauncherNew.Forms
{
    partial class MyMainMenu
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
            this.components = new System.ComponentModel.Container();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.LargeFilesPatchButton = new System.Windows.Forms.Button();
            this.EnglishPatchButton = new System.Windows.Forms.Button();
            this.mainFormLoadingHost = new System.Windows.Forms.Integration.ElementHost();
            this.mainFormLoading = new PSO2ProxyLauncherNew.WPF.LoadingPictureBox();
            this.mainProgressBarHost = new System.Windows.Forms.TableLayoutPanel();
            this.mainProgressBar = new PSO2ProxyLauncherNew.Classes.Controls.CircleProgressBar();
            this.LogRichTextBox = new PSO2ProxyLauncherNew.Classes.Controls.ExRichTextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tweakerWebBrowserLoading = new PSO2ProxyLauncherNew.Classes.Controls.OwfProgressControl(this.components);
            this.tweakerWebBrowser = new PSO2ProxyLauncherNew.Classes.Controls.TweakerWebBrowser();
            this.tweakerWebBrowserContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.englishPatchContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.installToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uninstallToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.forceUninstallToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.mainProgressBarHost.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tweakerWebBrowserContextMenu.SuspendLayout();
            this.englishPatchContext.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.LogRichTextBox, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 1, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(11, 33);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(618, 424);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // panel1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.panel1, 2);
            this.panel1.Controls.Add(this.LargeFilesPatchButton);
            this.panel1.Controls.Add(this.EnglishPatchButton);
            this.panel1.Controls.Add(this.mainFormLoadingHost);
            this.panel1.Controls.Add(this.mainProgressBarHost);
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(612, 204);
            this.panel1.TabIndex = 0;
            // 
            // LargeFilesPatchButton
            // 
            this.LargeFilesPatchButton.BackColor = System.Drawing.Color.WhiteSmoke;
            this.LargeFilesPatchButton.FlatAppearance.BorderColor = System.Drawing.Color.Yellow;
            this.LargeFilesPatchButton.FlatAppearance.BorderSize = 2;
            this.LargeFilesPatchButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Silver;
            this.LargeFilesPatchButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.LargeFilesPatchButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.LargeFilesPatchButton.Location = new System.Drawing.Point(25, 155);
            this.LargeFilesPatchButton.Name = "LargeFilesPatchButton";
            this.LargeFilesPatchButton.Size = new System.Drawing.Size(200, 23);
            this.LargeFilesPatchButton.TabIndex = 7;
            this.LargeFilesPatchButton.Text = "LargeFiles Patch: Not Installed";
            this.LargeFilesPatchButton.UseCompatibleTextRendering = true;
            this.LargeFilesPatchButton.UseVisualStyleBackColor = false;
            this.LargeFilesPatchButton.Click += new System.EventHandler(this.LargeFilesPatchButton_Click);
            // 
            // EnglishPatchButton
            // 
            this.EnglishPatchButton.BackColor = System.Drawing.Color.WhiteSmoke;
            this.EnglishPatchButton.FlatAppearance.BorderColor = System.Drawing.Color.Yellow;
            this.EnglishPatchButton.FlatAppearance.BorderSize = 2;
            this.EnglishPatchButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Silver;
            this.EnglishPatchButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.EnglishPatchButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.EnglishPatchButton.Location = new System.Drawing.Point(25, 126);
            this.EnglishPatchButton.Name = "EnglishPatchButton";
            this.EnglishPatchButton.Size = new System.Drawing.Size(200, 23);
            this.EnglishPatchButton.TabIndex = 4;
            this.EnglishPatchButton.Text = "English Patch: Not Installed";
            this.EnglishPatchButton.UseCompatibleTextRendering = true;
            this.EnglishPatchButton.UseVisualStyleBackColor = false;
            this.EnglishPatchButton.Click += new System.EventHandler(this.EnglishPatchButton_Click);
            // 
            // mainFormLoadingHost
            // 
            this.mainFormLoadingHost.BackColor = System.Drawing.Color.White;
            this.mainFormLoadingHost.BackColorTransparent = true;
            this.mainFormLoadingHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainFormLoadingHost.Location = new System.Drawing.Point(0, 0);
            this.mainFormLoadingHost.Name = "mainFormLoadingHost";
            this.mainFormLoadingHost.Size = new System.Drawing.Size(612, 204);
            this.mainFormLoadingHost.TabIndex = 3;
            this.mainFormLoadingHost.Visible = false;
            this.mainFormLoadingHost.Child = this.mainFormLoading;
            // 
            // mainProgressBarHost
            // 
            this.mainProgressBarHost.ColumnCount = 2;
            this.mainProgressBarHost.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.mainProgressBarHost.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.mainProgressBarHost.Controls.Add(this.mainProgressBar, 0, 0);
            this.mainProgressBarHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainProgressBarHost.Location = new System.Drawing.Point(0, 0);
            this.mainProgressBarHost.Name = "mainProgressBarHost";
            this.mainProgressBarHost.RowCount = 2;
            this.mainProgressBarHost.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.mainProgressBarHost.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.mainProgressBarHost.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.mainProgressBarHost.Size = new System.Drawing.Size(612, 204);
            this.mainProgressBarHost.TabIndex = 6;
            this.mainProgressBarHost.Visible = false;
            // 
            // mainProgressBar
            // 
            this.mainProgressBar.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.mainProgressBarHost.SetColumnSpan(this.mainProgressBar, 2);
            this.mainProgressBar.Font = new System.Drawing.Font("Segoe UI", 15F);
            this.mainProgressBar.Location = new System.Drawing.Point(241, 37);
            this.mainProgressBar.Maximum = ((long)(100));
            this.mainProgressBar.MinimumSize = new System.Drawing.Size(100, 100);
            this.mainProgressBar.Name = "mainProgressBar";
            this.mainProgressBar.ProgressColor1 = System.Drawing.Color.DarkRed;
            this.mainProgressBar.ProgressColor2 = System.Drawing.Color.LightGray;
            this.mainProgressBar.ProgressShape = PSO2ProxyLauncherNew.Classes.Controls.CircleProgressBar._ProgressShape.Round;
            this.mainProgressBarHost.SetRowSpan(this.mainProgressBar, 2);
            this.mainProgressBar.Size = new System.Drawing.Size(130, 130);
            this.mainProgressBar.TabIndex = 0;
            this.mainProgressBar.Value = ((long)(0));
            // 
            // LogRichTextBox
            // 
            this.LogRichTextBox.BackColor = System.Drawing.Color.White;
            this.LogRichTextBox.HiglightColor = PSO2ProxyLauncherNew.Classes.Controls.RtfColor.White;
            this.LogRichTextBox.Location = new System.Drawing.Point(3, 215);
            this.LogRichTextBox.Name = "LogRichTextBox";
            this.LogRichTextBox.ReadOnly = true;
            this.LogRichTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.LogRichTextBox.ShortcutsEnabled = false;
            this.LogRichTextBox.Size = new System.Drawing.Size(303, 206);
            this.LogRichTextBox.TabIndex = 1;
            this.LogRichTextBox.Text = " Checking for updates...";
            this.LogRichTextBox.TextColor = PSO2ProxyLauncherNew.Classes.Controls.RtfColor.Black;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tweakerWebBrowserLoading);
            this.panel2.Controls.Add(this.tweakerWebBrowser);
            this.panel2.Location = new System.Drawing.Point(312, 215);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(303, 206);
            this.panel2.TabIndex = 2;
            // 
            // tweakerWebBrowserLoading
            // 
            this.tweakerWebBrowserLoading.AnimationSpeed = ((short)(75));
            this.tweakerWebBrowserLoading.BackColor = System.Drawing.Color.Transparent;
            this.tweakerWebBrowserLoading.CirclesColor = System.Drawing.Color.WhiteSmoke;
            this.tweakerWebBrowserLoading.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tweakerWebBrowserLoading.Location = new System.Drawing.Point(0, 0);
            this.tweakerWebBrowserLoading.Name = "tweakerWebBrowserLoading";
            this.tweakerWebBrowserLoading.Size = new System.Drawing.Size(303, 206);
            this.tweakerWebBrowserLoading.TabIndex = 3;
            this.tweakerWebBrowserLoading.TitileText = "";
            // 
            // tweakerWebBrowser
            // 
            this.tweakerWebBrowser.ContextMenuStrip = this.tweakerWebBrowserContextMenu;
            this.tweakerWebBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tweakerWebBrowser.EnglishPatchStatus = PSO2ProxyLauncherNew.Classes.Controls.PatchStatus.Unknown;
            this.tweakerWebBrowser.IsWebBrowserContextMenuEnabled = false;
            this.tweakerWebBrowser.ItemPatchStatus = PSO2ProxyLauncherNew.Classes.Controls.PatchStatus.Unknown;
            this.tweakerWebBrowser.Location = new System.Drawing.Point(0, 0);
            this.tweakerWebBrowser.LockNavigate = false;
            this.tweakerWebBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.tweakerWebBrowser.Name = "tweakerWebBrowser";
            this.tweakerWebBrowser.ScriptErrorsSuppressed = true;
            this.tweakerWebBrowser.Size = new System.Drawing.Size(303, 206);
            this.tweakerWebBrowser.TabIndex = 2;
            this.tweakerWebBrowser.WebBrowserShortcutsEnabled = false;
            this.tweakerWebBrowser.LockedNavigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.tweakerWebBrowser_LockedNavigating);
            // 
            // tweakerWebBrowserContextMenu
            // 
            this.tweakerWebBrowserContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshToolStripMenuItem});
            this.tweakerWebBrowserContextMenu.Name = "tweakerWebBrowserContextMenu";
            this.tweakerWebBrowserContextMenu.Size = new System.Drawing.Size(114, 26);
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
            this.refreshToolStripMenuItem.Text = "Refresh";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // englishPatchContext
            // 
            this.englishPatchContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.installToolStripMenuItem,
            this.uninstallToolStripMenuItem,
            this.forceUninstallToolStripMenuItem});
            this.englishPatchContext.Name = "englishPatchContext";
            this.englishPatchContext.Size = new System.Drawing.Size(153, 70);
            // 
            // installToolStripMenuItem
            // 
            this.installToolStripMenuItem.Name = "installToolStripMenuItem";
            this.installToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.installToolStripMenuItem.Text = "Install";
            this.installToolStripMenuItem.Click += new System.EventHandler(this.installToolStripMenuItem_Click);
            // 
            // uninstallToolStripMenuItem
            // 
            this.uninstallToolStripMenuItem.Name = "uninstallToolStripMenuItem";
            this.uninstallToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.uninstallToolStripMenuItem.Text = "Uninstall";
            this.uninstallToolStripMenuItem.Click += new System.EventHandler(this.uninstallToolStripMenuItem_Click);
            // 
            // forceUninstallToolStripMenuItem
            // 
            this.forceUninstallToolStripMenuItem.Name = "forceUninstallToolStripMenuItem";
            this.forceUninstallToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.forceUninstallToolStripMenuItem.Text = "Force Uninstall";
            // 
            // MyMainMenu
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = MetroFramework.Forms.MetroFormBorderStyle.FixedSingle;
            this.ClientSize = new System.Drawing.Size(640, 480);
            this.Controls.Add(this.tableLayoutPanel1);
            this.DisplayHeader = false;
            this.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(1)))), ((int)(((byte)(1)))));
            this.MaximizeBox = false;
            this.Name = "MyMainMenu";
            this.Padding = new System.Windows.Forms.Padding(20, 30, 20, 20);
            this.Resizable = false;
            this.ShadowType = MetroFramework.Forms.MetroFormShadowType.DropShadow;
            this.Style = MetroFramework.MetroColorStyle.Red;
            this.Text = "PSO2 Launcher";
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form_Shown);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.mainProgressBarHost.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.tweakerWebBrowserContextMenu.ResumeLayout(false);
            this.englishPatchContext.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private Classes.Controls.ExRichTextBox LogRichTextBox;
        private System.Windows.Forms.Panel panel2;
        private Classes.Controls.TweakerWebBrowser tweakerWebBrowser;
        private System.Windows.Forms.Integration.ElementHost mainFormLoadingHost;
        private WPF.LoadingPictureBox mainFormLoading;
        private Classes.Controls.OwfProgressControl tweakerWebBrowserLoading;
        private System.Windows.Forms.ContextMenuStrip tweakerWebBrowserContextMenu;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
        private System.Windows.Forms.Button EnglishPatchButton;
        private System.Windows.Forms.ContextMenuStrip englishPatchContext;
        private System.Windows.Forms.ToolStripMenuItem installToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uninstallToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem forceUninstallToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel mainProgressBarHost;
        private Classes.Controls.CircleProgressBar mainProgressBar;
        private System.Windows.Forms.Button LargeFilesPatchButton;
    }
}

