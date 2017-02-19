namespace PSO2ProxyLauncherNew.Classes.Controls
{
    partial class OwfProgressControl
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.drawTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // drawTimer
            // 
            this.drawTimer.Enabled = true;
            this.drawTimer.Interval = 25;
            this.drawTimer.Tick += new System.EventHandler(this.drawTimer_Tick);
            // 
            // OwfProgressControl
            // 
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.OwfProgressControl_Paint);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer drawTimer;
    }

}
