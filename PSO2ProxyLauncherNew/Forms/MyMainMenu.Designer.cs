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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MyMainMenu));
            this.tweakerWebBrowserContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.englishPatchContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.installToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uninstallToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.forceUninstallToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuPSO2GameOption = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.pSO2ProxyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.installToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.uninstallToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.panelMainMenu = new PSO2ProxyLauncherNew.Classes.Controls.ExtendedTableLayoutPanel();
            this.panel1 = new PSO2ProxyLauncherNew.Classes.Controls.DoubleBufferedPanel();
            this.launcherOption = new PSO2ProxyLauncherNew.Classes.Controls.RelativeButton();
            this.mainFormLoadingHost = new PSO2ProxyLauncherNew.Classes.Controls.DoubleBufferedElementHost();
            this.mainFormLoading = new PSO2ProxyLauncherNew.WPF.LoadingPictureBox();
            this.buttonOptionPSO2 = new PSO2ProxyLauncherNew.Classes.Controls.RelativeButton();
            this.buttonPluginManager = new PSO2ProxyLauncherNew.Classes.Controls.RelativeButton();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.gameStartButton1 = new PSO2ProxyLauncherNew.Classes.Controls.GameStartButton();
            this.mainProgressBar = new PSO2ProxyLauncherNew.Classes.Controls.CircleProgressBar();
            this.StoryPatchButton = new PSO2ProxyLauncherNew.Classes.Controls.RelativeButton();
            this.LargeFilesPatchButton = new PSO2ProxyLauncherNew.Classes.Controls.RelativeButton();
            this.EnglishPatchButton = new PSO2ProxyLauncherNew.Classes.Controls.RelativeButton();
            this.LogRichTextBox = new PSO2ProxyLauncherNew.Classes.Controls.ExRichTextBox();
            this.panel2 = new PSO2ProxyLauncherNew.Classes.Controls.DoubleBufferedPanel();
            this.tweakerWebBrowserLoading = new PSO2ProxyLauncherNew.Classes.Controls.OwfProgressControl(this.components);
            this.tweakerWebBrowser = new PSO2ProxyLauncherNew.Classes.Controls.TweakerWebBrowser();
            this.panelOption = new System.Windows.Forms.Panel();
            this.optionGroupPSO2Update = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.optionComboBoxUpdateThread = new System.Windows.Forms.ComboBox();
            this.optionButtonOK = new System.Windows.Forms.Button();
            this.optioncheckboxpso2updatecache = new System.Windows.Forms.CheckBox();
            this.tweakerWebBrowserContextMenu.SuspendLayout();
            this.englishPatchContext.SuspendLayout();
            this.contextMenuPSO2GameOption.SuspendLayout();
            this.panelMainMenu.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panelOption.SuspendLayout();
            this.optionGroupPSO2Update.SuspendLayout();
            this.SuspendLayout();
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
            // contextMenuPSO2GameOption
            // 
            this.contextMenuPSO2GameOption.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pSO2ProxyToolStripMenuItem});
            this.contextMenuPSO2GameOption.Name = "contextMenuPSO2GameOption";
            this.contextMenuPSO2GameOption.Size = new System.Drawing.Size(132, 26);
            // 
            // pSO2ProxyToolStripMenuItem
            // 
            this.pSO2ProxyToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.installToolStripMenuItem1,
            this.uninstallToolStripMenuItem1});
            this.pSO2ProxyToolStripMenuItem.Name = "pSO2ProxyToolStripMenuItem";
            this.pSO2ProxyToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.pSO2ProxyToolStripMenuItem.Text = "PSO2Proxy";
            // 
            // installToolStripMenuItem1
            // 
            this.installToolStripMenuItem1.Name = "installToolStripMenuItem1";
            this.installToolStripMenuItem1.Size = new System.Drawing.Size(120, 22);
            this.installToolStripMenuItem1.Text = "Install";
            this.installToolStripMenuItem1.Click += new System.EventHandler(this.installToolStripMenuItem1_Click);
            // 
            // uninstallToolStripMenuItem1
            // 
            this.uninstallToolStripMenuItem1.Name = "uninstallToolStripMenuItem1";
            this.uninstallToolStripMenuItem1.Size = new System.Drawing.Size(120, 22);
            this.uninstallToolStripMenuItem1.Text = "Uninstall";
            this.uninstallToolStripMenuItem1.Click += new System.EventHandler(this.uninstallToolStripMenuItem1_Click);
            // 
            // panelMainMenu
            // 
            this.panelMainMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.panelMainMenu.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panelMainMenu.BackgroundImage")));
            this.panelMainMenu.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelMainMenu.ColumnCount = 2;
            this.panelMainMenu.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.panelMainMenu.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.panelMainMenu.Controls.Add(this.panel1, 0, 0);
            this.panelMainMenu.Controls.Add(this.LogRichTextBox, 0, 1);
            this.panelMainMenu.Controls.Add(this.panel2, 1, 1);
            this.panelMainMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMainMenu.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(1)))), ((int)(((byte)(1)))));
            this.panelMainMenu.Location = new System.Drawing.Point(20, 30);
            this.panelMainMenu.Name = "panelMainMenu";
            this.panelMainMenu.RowCount = 2;
            this.panelMainMenu.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 51.59818F));
            this.panelMainMenu.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 48.40182F));
            this.panelMainMenu.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.panelMainMenu.Size = new System.Drawing.Size(600, 430);
            this.panelMainMenu.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.panelMainMenu.SetColumnSpan(this.panel1, 2);
            this.panel1.Controls.Add(this.launcherOption);
            this.panel1.Controls.Add(this.mainFormLoadingHost);
            this.panel1.Controls.Add(this.buttonOptionPSO2);
            this.panel1.Controls.Add(this.buttonPluginManager);
            this.panel1.Controls.Add(this.buttonCancel);
            this.panel1.Controls.Add(this.gameStartButton1);
            this.panel1.Controls.Add(this.mainProgressBar);
            this.panel1.Controls.Add(this.StoryPatchButton);
            this.panel1.Controls.Add(this.LargeFilesPatchButton);
            this.panel1.Controls.Add(this.EnglishPatchButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(594, 215);
            this.panel1.TabIndex = 0;
            // 
            // launcherOption
            // 
            this.launcherOption.Location = new System.Drawing.Point(372, 124);
            this.launcherOption.Name = "launcherOption";
            this.launcherOption.RelativeLocation = new System.Drawing.Point(0, 0);
            this.launcherOption.Size = new System.Drawing.Size(200, 23);
            this.launcherOption.TabIndex = 13;
            this.launcherOption.Text = "Launcher Options";
            this.launcherOption.UseVisualStyleBackColor = true;
            this.launcherOption.Visible = false;
            this.launcherOption.Click += new System.EventHandler(this.launcherOption_Click);
            // 
            // mainFormLoadingHost
            // 
            this.mainFormLoadingHost.BackColorTransparent = true;
            this.mainFormLoadingHost.Location = new System.Drawing.Point(226, 39);
            this.mainFormLoadingHost.Name = "mainFormLoadingHost";
            this.mainFormLoadingHost.RelativeLocation = new System.Drawing.Point(0, 0);
            this.mainFormLoadingHost.Size = new System.Drawing.Size(140, 140);
            this.mainFormLoadingHost.TabIndex = 12;
            this.mainFormLoadingHost.Child = this.mainFormLoading;
            // 
            // buttonOptionPSO2
            // 
            this.buttonOptionPSO2.Location = new System.Drawing.Point(382, 95);
            this.buttonOptionPSO2.Name = "buttonOptionPSO2";
            this.buttonOptionPSO2.RelativeLocation = new System.Drawing.Point(0, 0);
            this.buttonOptionPSO2.Size = new System.Drawing.Size(200, 23);
            this.buttonOptionPSO2.TabIndex = 11;
            this.buttonOptionPSO2.Text = "PSO2 Game Option";
            this.buttonOptionPSO2.UseVisualStyleBackColor = true;
            this.buttonOptionPSO2.Visible = false;
            this.buttonOptionPSO2.Click += new System.EventHandler(this.buttonOptionPSO2_Click);
            // 
            // buttonPluginManager
            // 
            this.buttonPluginManager.Location = new System.Drawing.Point(372, 66);
            this.buttonPluginManager.Name = "buttonPluginManager";
            this.buttonPluginManager.RelativeLocation = new System.Drawing.Point(0, 0);
            this.buttonPluginManager.Size = new System.Drawing.Size(200, 23);
            this.buttonPluginManager.TabIndex = 10;
            this.buttonPluginManager.Text = "PSO2 Plugin Manager";
            this.buttonPluginManager.UseVisualStyleBackColor = true;
            this.buttonPluginManager.Visible = false;
            this.buttonPluginManager.Click += new System.EventHandler(this.buttonPluginManager_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.Location = new System.Drawing.Point(241, 182);
            this.buttonCancel.MinimumSize = new System.Drawing.Size(111, 23);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(111, 23);
            this.buttonCancel.TabIndex = 9;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Visible = false;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // gameStartButton1
            // 
            this.gameStartButton1.BackColor = System.Drawing.Color.Transparent;
            this.gameStartButton1.Font = new System.Drawing.Font("Tahoma", 17F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.gameStartButton1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(254)))), ((int)(((byte)(254)))));
            this.gameStartButton1.FPS = 80D;
            this.gameStartButton1.Location = new System.Drawing.Point(226, 39);
            this.gameStartButton1.MainColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(45)))), ((int)(((byte)(55)))));
            this.gameStartButton1.MinimumSize = new System.Drawing.Size(140, 140);
            this.gameStartButton1.Name = "gameStartButton1";
            this.gameStartButton1.Opacity = 50;
            this.gameStartButton1.RelativeLocation = new System.Drawing.Point(0, 0);
            this.gameStartButton1.Size = new System.Drawing.Size(140, 140);
            this.gameStartButton1.SubColor1 = System.Drawing.Color.DarkCyan;
            this.gameStartButton1.SubColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(45)))), ((int)(((byte)(35)))));
            this.gameStartButton1.TabIndex = 3;
            this.gameStartButton1.Text = "START";
            this.gameStartButton1.Click += new System.EventHandler(this.gameStartButton1_Click);
            // 
            // mainProgressBar
            // 
            this.mainProgressBar.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.mainProgressBar.BackColor = System.Drawing.Color.Transparent;
            this.mainProgressBar.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mainProgressBar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(254)))), ((int)(((byte)(254)))));
            this.mainProgressBar.Location = new System.Drawing.Point(226, 39);
            this.mainProgressBar.Maximum = ((long)(100));
            this.mainProgressBar.MinimumSize = new System.Drawing.Size(140, 140);
            this.mainProgressBar.Name = "mainProgressBar";
            this.mainProgressBar.Opacity = 35;
            this.mainProgressBar.ProgressColor1 = System.Drawing.Color.DarkRed;
            this.mainProgressBar.ProgressColor2 = System.Drawing.Color.Gainsboro;
            this.mainProgressBar.ProgressShape = PSO2ProxyLauncherNew.Classes.Controls.CircleProgressBar._ProgressShape.Round;
            this.mainProgressBar.RelativeLocation = new System.Drawing.Point(235, 41);
            this.mainProgressBar.ShowSmallText = false;
            this.mainProgressBar.Size = new System.Drawing.Size(140, 140);
            this.mainProgressBar.SmallTextFont = new System.Drawing.Font("Tahoma", 9F);
            this.mainProgressBar.TabIndex = 0;
            this.mainProgressBar.Value = ((long)(0));
            this.mainProgressBar.Visible = false;
            // 
            // StoryPatchButton
            // 
            this.StoryPatchButton.BackColor = System.Drawing.Color.WhiteSmoke;
            this.StoryPatchButton.FlatAppearance.BorderColor = System.Drawing.Color.Yellow;
            this.StoryPatchButton.FlatAppearance.BorderSize = 2;
            this.StoryPatchButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Silver;
            this.StoryPatchButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.StoryPatchButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.StoryPatchButton.Font = new System.Drawing.Font("Tahoma", 8F);
            this.StoryPatchButton.Location = new System.Drawing.Point(20, 124);
            this.StoryPatchButton.MinimumSize = new System.Drawing.Size(200, 23);
            this.StoryPatchButton.Name = "StoryPatchButton";
            this.StoryPatchButton.RelativeLocation = new System.Drawing.Point(0, 0);
            this.StoryPatchButton.Size = new System.Drawing.Size(200, 23);
            this.StoryPatchButton.TabIndex = 8;
            this.StoryPatchButton.Text = "Story Patch: Not Installed";
            this.StoryPatchButton.UseVisualStyleBackColor = false;
            this.StoryPatchButton.Visible = false;
            this.StoryPatchButton.Click += new System.EventHandler(this.StoryPatchButton_Click);
            // 
            // LargeFilesPatchButton
            // 
            this.LargeFilesPatchButton.BackColor = System.Drawing.Color.WhiteSmoke;
            this.LargeFilesPatchButton.FlatAppearance.BorderColor = System.Drawing.Color.Yellow;
            this.LargeFilesPatchButton.FlatAppearance.BorderSize = 2;
            this.LargeFilesPatchButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Silver;
            this.LargeFilesPatchButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.LargeFilesPatchButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.LargeFilesPatchButton.Font = new System.Drawing.Font("Tahoma", 8F);
            this.LargeFilesPatchButton.Location = new System.Drawing.Point(10, 95);
            this.LargeFilesPatchButton.MinimumSize = new System.Drawing.Size(200, 23);
            this.LargeFilesPatchButton.Name = "LargeFilesPatchButton";
            this.LargeFilesPatchButton.RelativeLocation = new System.Drawing.Point(0, 0);
            this.LargeFilesPatchButton.Size = new System.Drawing.Size(200, 23);
            this.LargeFilesPatchButton.TabIndex = 7;
            this.LargeFilesPatchButton.Text = "LargeFiles Patch: Not Installed";
            this.LargeFilesPatchButton.UseVisualStyleBackColor = false;
            this.LargeFilesPatchButton.Visible = false;
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
            this.EnglishPatchButton.Font = new System.Drawing.Font("Tahoma", 8F);
            this.EnglishPatchButton.Location = new System.Drawing.Point(20, 66);
            this.EnglishPatchButton.MinimumSize = new System.Drawing.Size(200, 23);
            this.EnglishPatchButton.Name = "EnglishPatchButton";
            this.EnglishPatchButton.RelativeLocation = new System.Drawing.Point(0, 0);
            this.EnglishPatchButton.Size = new System.Drawing.Size(200, 23);
            this.EnglishPatchButton.TabIndex = 4;
            this.EnglishPatchButton.Text = "English Patch: Not Installed";
            this.EnglishPatchButton.UseVisualStyleBackColor = false;
            this.EnglishPatchButton.Visible = false;
            this.EnglishPatchButton.Click += new System.EventHandler(this.EnglishPatchButton_Click);
            // 
            // LogRichTextBox
            // 
            this.LogRichTextBox.AutoScrollToCarret = true;
            this.LogRichTextBox.BackColor = System.Drawing.Color.White;
            this.LogRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LogRichTextBox.Font = new System.Drawing.Font("Tahoma", 8.5F);
            this.LogRichTextBox.HiglightColor = PSO2ProxyLauncherNew.Classes.Controls.RtfColor.White;
            this.LogRichTextBox.Location = new System.Drawing.Point(3, 224);
            this.LogRichTextBox.Name = "LogRichTextBox";
            this.LogRichTextBox.ReadOnly = true;
            this.LogRichTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.LogRichTextBox.ShortcutsEnabled = false;
            this.LogRichTextBox.Size = new System.Drawing.Size(294, 203);
            this.LogRichTextBox.TabIndex = 1;
            this.LogRichTextBox.Text = " Checking for updates...";
            this.LogRichTextBox.TextColor = PSO2ProxyLauncherNew.Classes.Controls.RtfColor.Black;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.Controls.Add(this.tweakerWebBrowserLoading);
            this.panel2.Controls.Add(this.tweakerWebBrowser);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(303, 224);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(294, 203);
            this.panel2.TabIndex = 2;
            // 
            // tweakerWebBrowserLoading
            // 
            this.tweakerWebBrowserLoading.AnimationSpeed = ((short)(90));
            this.tweakerWebBrowserLoading.BackColor = System.Drawing.Color.Transparent;
            this.tweakerWebBrowserLoading.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tweakerWebBrowserLoading.CirclesColor = System.Drawing.Color.WhiteSmoke;
            this.tweakerWebBrowserLoading.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tweakerWebBrowserLoading.Location = new System.Drawing.Point(0, 0);
            this.tweakerWebBrowserLoading.Name = "tweakerWebBrowserLoading";
            this.tweakerWebBrowserLoading.Size = new System.Drawing.Size(294, 203);
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
            this.tweakerWebBrowser.Size = new System.Drawing.Size(294, 203);
            this.tweakerWebBrowser.TabIndex = 2;
            this.tweakerWebBrowser.WebBrowserShortcutsEnabled = false;
            this.tweakerWebBrowser.LockedNavigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.tweakerWebBrowser_LockedNavigating);
            // 
            // panelOption
            // 
            this.panelOption.BackColor = System.Drawing.Color.Transparent;
            this.panelOption.Controls.Add(this.optionGroupPSO2Update);
            this.panelOption.Controls.Add(this.optionButtonOK);
            this.panelOption.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelOption.Location = new System.Drawing.Point(20, 30);
            this.panelOption.Name = "panelOption";
            this.panelOption.Size = new System.Drawing.Size(600, 430);
            this.panelOption.TabIndex = 3;
            // 
            // optionGroupPSO2Update
            // 
            this.optionGroupPSO2Update.Controls.Add(this.optioncheckboxpso2updatecache);
            this.optionGroupPSO2Update.Controls.Add(this.label1);
            this.optionGroupPSO2Update.Controls.Add(this.optionComboBoxUpdateThread);
            this.optionGroupPSO2Update.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(254)))), ((int)(((byte)(254)))));
            this.optionGroupPSO2Update.Location = new System.Drawing.Point(6, 6);
            this.optionGroupPSO2Update.Name = "optionGroupPSO2Update";
            this.optionGroupPSO2Update.Size = new System.Drawing.Size(200, 100);
            this.optionGroupPSO2Update.TabIndex = 3;
            this.optionGroupPSO2Update.TabStop = false;
            this.optionGroupPSO2Update.Text = "PSO2 Update";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Thread(s)";
            // 
            // optionComboBoxUpdateThread
            // 
            this.optionComboBoxUpdateThread.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.optionComboBoxUpdateThread.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(1)))), ((int)(((byte)(1)))));
            this.optionComboBoxUpdateThread.FormattingEnabled = true;
            this.optionComboBoxUpdateThread.Location = new System.Drawing.Point(67, 14);
            this.optionComboBoxUpdateThread.Name = "optionComboBoxUpdateThread";
            this.optionComboBoxUpdateThread.Size = new System.Drawing.Size(55, 21);
            this.optionComboBoxUpdateThread.TabIndex = 1;
            // 
            // optionButtonOK
            // 
            this.optionButtonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.optionButtonOK.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(1)))), ((int)(((byte)(1)))));
            this.optionButtonOK.Location = new System.Drawing.Point(525, 407);
            this.optionButtonOK.Name = "optionButtonOK";
            this.optionButtonOK.Size = new System.Drawing.Size(75, 23);
            this.optionButtonOK.TabIndex = 0;
            this.optionButtonOK.Text = "OK";
            this.optionButtonOK.UseVisualStyleBackColor = true;
            this.optionButtonOK.Click += new System.EventHandler(this.optionButtonOK_Click);
            // 
            // optioncheckboxpso2updatecache
            // 
            this.optioncheckboxpso2updatecache.AutoSize = true;
            this.optioncheckboxpso2updatecache.Location = new System.Drawing.Point(6, 38);
            this.optioncheckboxpso2updatecache.Name = "optioncheckboxpso2updatecache";
            this.optioncheckboxpso2updatecache.Size = new System.Drawing.Size(75, 17);
            this.optioncheckboxpso2updatecache.TabIndex = 3;
            this.optioncheckboxpso2updatecache.Text = "Use cache";
            this.optioncheckboxpso2updatecache.UseVisualStyleBackColor = true;
            // 
            // MyMainMenu
            // 
            this.AllowDrop = true;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackgroundImage = global::PSO2ProxyLauncherNew.Properties.Resources._bgimg;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.BorderStyle = MetroFramework.Forms.MetroFormBorderStyle.FixedSingle;
            this.ClientSize = new System.Drawing.Size(640, 480);
            this.Controls.Add(this.panelOption);
            this.Controls.Add(this.panelMainMenu);
            this.DisplayHeader = false;
            this.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(254)))), ((int)(((byte)(254)))));
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(640, 480);
            this.Name = "MyMainMenu";
            this.Padding = new System.Windows.Forms.Padding(20, 30, 20, 20);
            this.Resizable = false;
            this.SelectedTab = this.panelOption;
            this.ShadowType = MetroFramework.Forms.MetroFormShadowType.DropShadow;
            this.Style = MetroFramework.MetroColorStyle.Red;
            this.Text = "PSO2 Launcher";
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MyMainMenu_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form_Shown);
            this.tweakerWebBrowserContextMenu.ResumeLayout(false);
            this.englishPatchContext.ResumeLayout(false);
            this.contextMenuPSO2GameOption.ResumeLayout(false);
            this.panelMainMenu.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panelOption.ResumeLayout(false);
            this.optionGroupPSO2Update.ResumeLayout(false);
            this.optionGroupPSO2Update.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private PSO2ProxyLauncherNew.Classes.Controls.ExtendedTableLayoutPanel panelMainMenu;
        private PSO2ProxyLauncherNew.Classes.Controls.DoubleBufferedPanel panel1;
        private Classes.Controls.ExRichTextBox LogRichTextBox;
        private PSO2ProxyLauncherNew.Classes.Controls.DoubleBufferedPanel panel2;
        private Classes.Controls.TweakerWebBrowser tweakerWebBrowser;
        private Classes.Controls.OwfProgressControl tweakerWebBrowserLoading;
        private System.Windows.Forms.ContextMenuStrip tweakerWebBrowserContextMenu;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
        private PSO2ProxyLauncherNew.Classes.Controls.RelativeButton EnglishPatchButton;
        private System.Windows.Forms.ContextMenuStrip englishPatchContext;
        private System.Windows.Forms.ToolStripMenuItem installToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uninstallToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem forceUninstallToolStripMenuItem;
        private Classes.Controls.CircleProgressBar mainProgressBar;
        private PSO2ProxyLauncherNew.Classes.Controls.RelativeButton LargeFilesPatchButton;
        private PSO2ProxyLauncherNew.Classes.Controls.RelativeButton StoryPatchButton;
        private Classes.Controls.GameStartButton gameStartButton1;
        private System.Windows.Forms.Button buttonCancel;
        private PSO2ProxyLauncherNew.Classes.Controls.RelativeButton buttonPluginManager;
        private Classes.Controls.RelativeButton buttonOptionPSO2;
        private System.Windows.Forms.ContextMenuStrip contextMenuPSO2GameOption;
        private System.Windows.Forms.ToolStripMenuItem pSO2ProxyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem installToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem uninstallToolStripMenuItem1;
        private Classes.Controls.DoubleBufferedElementHost mainFormLoadingHost;
        private WPF.LoadingPictureBox mainFormLoading;
        private Classes.Controls.RelativeButton launcherOption;
        private System.Windows.Forms.Panel panelOption;
        private System.Windows.Forms.Button optionButtonOK;
        private System.Windows.Forms.ComboBox optionComboBoxUpdateThread;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox optionGroupPSO2Update;
        private System.Windows.Forms.CheckBox optioncheckboxpso2updatecache;
    }
}

