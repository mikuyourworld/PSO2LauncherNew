using MetroFramework.Controls;
using MetroFramework.Localization;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace MetroFramework
{
	public class MetroMessageBoxControl : Form
	{
		private MetroLocalize metroLocalize;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Color _defaultColor = Color.FromArgb(57, 179, 215);

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Color _errorColor = Color.FromArgb(210, 50, 45);

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Color _warningColor = Color.FromArgb(237, 156, 40);

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Color _success = Color.FromArgb(71, 164, 71);

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Color _question = Color.FromArgb(71, 164, 71);

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private MetroMessageBoxProperties _properties;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private System.Windows.Forms.DialogResult _result;

		private IContainer components;

		private Panel panelbody;

		private Label titleLabel;

		private Label messageLabel;

		private MetroButton metroButton1;

		private MetroButton metroButton2;

		private MetroButton metroButton3;

		private TableLayoutPanel tlpBody;

		private Panel pnlBottom;

		public Panel Body
		{
			get
			{
				return this.panelbody;
			}
		}

		public new MetroMessageBoxProperties Properties
		{
			get
			{
				return this._properties;
			}
		}

		public System.Windows.Forms.DialogResult Result
		{
			get
			{
				return this._result;
			}
		}

		public MetroMessageBoxControl()
		{
			this.InitializeComponent();
			this._properties = new MetroMessageBoxProperties(this);
			this.StylizeButton(this.metroButton1);
			this.StylizeButton(this.metroButton2);
			this.StylizeButton(this.metroButton3);
			this.metroButton1.Click += new EventHandler(this.button_Click);
			this.metroButton2.Click += new EventHandler(this.button_Click);
			this.metroButton3.Click += new EventHandler(this.button_Click);
			this.metroLocalize = new MetroLocalize(this);
		}

		public void ArrangeApperance()
		{
			this.titleLabel.Text = this._properties.Title;
			this.messageLabel.Text = this._properties.Message;
			MessageBoxIcon icon = this._properties.Icon;
			if (icon == MessageBoxIcon.Hand)
			{
				this.panelbody.BackColor = this._errorColor;
			}
			else if (icon == MessageBoxIcon.Exclamation)
			{
				this.panelbody.BackColor = this._warningColor;
			}
			switch (this._properties.Buttons)
			{
				case MessageBoxButtons.OK:
				{
					this.EnableButton(this.metroButton1);
					this.metroButton1.Text = this.metroLocalize.translate("Ok");
					this.metroButton1.Location = this.metroButton3.Location;
					this.metroButton1.Tag = System.Windows.Forms.DialogResult.OK;
					this.EnableButton(this.metroButton2, false);
					this.EnableButton(this.metroButton3, false);
					break;
				}
				case MessageBoxButtons.OKCancel:
				{
					this.EnableButton(this.metroButton1);
					this.metroButton1.Text = this.metroLocalize.translate("Ok");
					this.metroButton1.Location = this.metroButton2.Location;
					this.metroButton1.Tag = System.Windows.Forms.DialogResult.OK;
					this.EnableButton(this.metroButton2);
					this.metroButton2.Text = this.metroLocalize.translate("Cancel");
					this.metroButton2.Location = this.metroButton3.Location;
					this.metroButton2.Tag = System.Windows.Forms.DialogResult.Cancel;
					this.EnableButton(this.metroButton3, false);
					break;
				}
				case MessageBoxButtons.AbortRetryIgnore:
				{
					this.EnableButton(this.metroButton1);
					this.metroButton1.Text = this.metroLocalize.translate("Abort");
					this.metroButton1.Tag = System.Windows.Forms.DialogResult.Abort;
					this.EnableButton(this.metroButton2);
					this.metroButton2.Text = this.metroLocalize.translate("Retry");
					this.metroButton2.Tag = System.Windows.Forms.DialogResult.Retry;
					this.EnableButton(this.metroButton3);
					this.metroButton3.Text = this.metroLocalize.translate("Ignore");
					this.metroButton3.Tag = System.Windows.Forms.DialogResult.Ignore;
					break;
				}
				case MessageBoxButtons.YesNoCancel:
				{
					this.EnableButton(this.metroButton1);
					this.metroButton1.Text = this.metroLocalize.translate("Yes");
					this.metroButton1.Tag = System.Windows.Forms.DialogResult.Yes;
					this.EnableButton(this.metroButton2);
					this.metroButton2.Text = this.metroLocalize.translate("No");
					this.metroButton2.Tag = System.Windows.Forms.DialogResult.No;
					this.EnableButton(this.metroButton3);
					this.metroButton3.Text = this.metroLocalize.translate("Cancel");
					this.metroButton3.Tag = System.Windows.Forms.DialogResult.Cancel;
					break;
				}
				case MessageBoxButtons.YesNo:
				{
					this.EnableButton(this.metroButton1);
					this.metroButton1.Text = this.metroLocalize.translate("Yes");
					this.metroButton1.Location = this.metroButton2.Location;
					this.metroButton1.Tag = System.Windows.Forms.DialogResult.Yes;
					this.EnableButton(this.metroButton2);
					this.metroButton2.Text = this.metroLocalize.translate("No");
					this.metroButton2.Location = this.metroButton3.Location;
					this.metroButton2.Tag = System.Windows.Forms.DialogResult.No;
					this.EnableButton(this.metroButton3, false);
					break;
				}
				case MessageBoxButtons.RetryCancel:
				{
					this.EnableButton(this.metroButton1);
					this.metroButton1.Text = this.metroLocalize.translate("Retry");
					this.metroButton1.Location = this.metroButton2.Location;
					this.metroButton1.Tag = System.Windows.Forms.DialogResult.Retry;
					this.EnableButton(this.metroButton2);
					this.metroButton2.Text = this.metroLocalize.translate("Cancel");
					this.metroButton2.Location = this.metroButton3.Location;
					this.metroButton2.Tag = System.Windows.Forms.DialogResult.Cancel;
					this.EnableButton(this.metroButton3, false);
					break;
				}
			}
			MessageBoxIcon messageBoxIcon = this._properties.Icon;
			if (messageBoxIcon > MessageBoxIcon.Question)
			{
				if (messageBoxIcon == MessageBoxIcon.Exclamation)
				{
					this.panelbody.BackColor = this._warningColor;
					return;
				}
				if (messageBoxIcon == MessageBoxIcon.Asterisk)
				{
					this.panelbody.BackColor = this._defaultColor;
					return;
				}
			}
			else
			{
				if (messageBoxIcon == MessageBoxIcon.Hand)
				{
					this.panelbody.BackColor = this._errorColor;
					return;
				}
				if (messageBoxIcon == MessageBoxIcon.Question)
				{
					this.panelbody.BackColor = this._question;
					return;
				}
			}
			this.panelbody.BackColor = Color.DarkGray;
		}

		private void button_Click(object sender, EventArgs e)
		{
			MetroButton metroButton = (MetroButton)sender;
			if (!metroButton.Enabled)
			{
				return;
			}
			this._result = (System.Windows.Forms.DialogResult)metroButton.Tag;
			base.Hide();
		}

		private void button_MouseClick(object sender, MouseEventArgs e)
		{
		}

		private void button_MouseEnter(object sender, EventArgs e)
		{
			this.StylizeButton((MetroButton)sender, true);
		}

		private void button_MouseLeave(object sender, EventArgs e)
		{
			this.StylizeButton((MetroButton)sender);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void EnableButton(MetroButton button)
		{
			this.EnableButton(button, true);
		}

		private void EnableButton(MetroButton button, bool enabled)
		{
			button.Enabled = enabled;
			button.Visible = enabled;
		}

		private void InitializeComponent()
		{
			this.panelbody = new Panel();
			this.tlpBody = new TableLayoutPanel();
			this.messageLabel = new Label();
			this.titleLabel = new Label();
			this.metroButton1 = new MetroButton();
			this.metroButton3 = new MetroButton();
			this.metroButton2 = new MetroButton();
			this.pnlBottom = new Panel();
			this.panelbody.SuspendLayout();
			this.tlpBody.SuspendLayout();
			this.pnlBottom.SuspendLayout();
			base.SuspendLayout();
			this.panelbody.BackColor = Color.DarkGray;
			this.panelbody.Controls.Add(this.tlpBody);
			this.panelbody.Dock = DockStyle.Fill;
			this.panelbody.Location = new Point(0, 0);
			this.panelbody.Margin = new System.Windows.Forms.Padding(0);
			this.panelbody.Name = "panelbody";
			this.panelbody.Size = new System.Drawing.Size(804, 211);
			this.panelbody.TabIndex = 2;
			this.tlpBody.ColumnCount = 3;
			this.tlpBody.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10f));
			this.tlpBody.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80f));
			this.tlpBody.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10f));
			this.tlpBody.Controls.Add(this.messageLabel, 1, 2);
			this.tlpBody.Controls.Add(this.titleLabel, 1, 1);
			this.tlpBody.Controls.Add(this.pnlBottom, 1, 3);
			this.tlpBody.Dock = DockStyle.Fill;
			this.tlpBody.Location = new Point(0, 0);
			this.tlpBody.Name = "tlpBody";
			this.tlpBody.RowCount = 4;
			this.tlpBody.RowStyles.Add(new RowStyle(SizeType.Absolute, 5f));
			this.tlpBody.RowStyles.Add(new RowStyle(SizeType.Absolute, 25f));
			this.tlpBody.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
			this.tlpBody.RowStyles.Add(new RowStyle(SizeType.Absolute, 40f));
			this.tlpBody.Size = new System.Drawing.Size(804, 211);
			this.tlpBody.TabIndex = 6;
			this.messageLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			this.messageLabel.BackColor = Color.Transparent;
			this.messageLabel.ForeColor = Color.White;
			this.messageLabel.Location = new Point(83, 30);
			this.messageLabel.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.messageLabel.Name = "messageLabel";
			this.messageLabel.Size = new System.Drawing.Size(640, 141);
			this.messageLabel.TabIndex = 0;
			this.messageLabel.Text = "message here";
			this.titleLabel.AutoSize = true;
			this.titleLabel.BackColor = Color.Transparent;
			this.titleLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 14.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.titleLabel.ForeColor = Color.WhiteSmoke;
			this.titleLabel.Location = new Point(80, 5);
			this.titleLabel.Margin = new System.Windows.Forms.Padding(0);
			this.titleLabel.Name = "titleLabel";
			this.titleLabel.Size = new System.Drawing.Size(125, 25);
			this.titleLabel.TabIndex = 1;
			this.titleLabel.Text = "message title";
			this.metroButton1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.metroButton1.BackColor = Color.ForestGreen;
			this.metroButton1.FontWeight = MetroButtonWeight.Regular;
			this.metroButton1.Location = new Point(357, 1);
			this.metroButton1.Name = "metroButton1";
			this.metroButton1.Size = new System.Drawing.Size(90, 26);
			this.metroButton1.TabIndex = 3;
			this.metroButton1.Text = "button 1";
			this.metroButton1.UseSelectable = true;
			this.metroButton3.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.metroButton3.FontWeight = MetroButtonWeight.Regular;
			this.metroButton3.Location = new Point(553, 1);
			this.metroButton3.Name = "metroButton3";
			this.metroButton3.Size = new System.Drawing.Size(90, 26);
			this.metroButton3.TabIndex = 5;
			this.metroButton3.Text = "button 3";
			this.metroButton3.UseSelectable = true;
			this.metroButton2.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.metroButton2.FontWeight = MetroButtonWeight.Regular;
			this.metroButton2.Location = new Point(455, 1);
			this.metroButton2.Name = "metroButton2";
			this.metroButton2.Size = new System.Drawing.Size(90, 26);
			this.metroButton2.TabIndex = 4;
			this.metroButton2.Text = "button 2";
			this.metroButton2.UseSelectable = true;
			this.pnlBottom.BackColor = Color.Transparent;
			this.pnlBottom.Controls.Add(this.metroButton2);
			this.pnlBottom.Controls.Add(this.metroButton1);
			this.pnlBottom.Controls.Add(this.metroButton3);
			this.pnlBottom.Dock = DockStyle.Fill;
			this.pnlBottom.Location = new Point(80, 171);
			this.pnlBottom.Margin = new System.Windows.Forms.Padding(0);
			this.pnlBottom.Name = "pnlBottom";
			this.pnlBottom.Size = new System.Drawing.Size(643, 40);
			this.pnlBottom.TabIndex = 2;
			base.AutoScaleDimensions = new SizeF(8f, 21f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(804, 211);
			base.ControlBox = false;
			base.Controls.Add(this.panelbody);
			this.Font = new System.Drawing.Font("Segoe UI Light", 12f, FontStyle.Regular, GraphicsUnit.Point, 0);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			base.Name = "MetroMessageBoxControl";
			base.StartPosition = FormStartPosition.Manual;
			this.panelbody.ResumeLayout(false);
			this.tlpBody.ResumeLayout(false);
			this.tlpBody.PerformLayout();
			this.pnlBottom.ResumeLayout(false);
			base.ResumeLayout(false);
		}

		public void SetDefaultButton()
		{
			MessageBoxDefaultButton defaultButton = this._properties.DefaultButton;
			if (defaultButton == MessageBoxDefaultButton.Button1)
			{
				if (this.metroButton1 != null && this.metroButton1.Enabled)
				{
					this.metroButton1.Focus();
					return;
				}
			}
			else if (defaultButton != MessageBoxDefaultButton.Button2)
			{
				if (defaultButton != MessageBoxDefaultButton.Button3)
				{
					return;
				}
				if (this.metroButton3 != null && this.metroButton3.Enabled)
				{
					this.metroButton3.Focus();
				}
			}
			else if (this.metroButton2 != null && this.metroButton2.Enabled)
			{
				this.metroButton2.Focus();
				return;
			}
		}

		private void StylizeButton(MetroButton button)
		{
			this.StylizeButton(button, false);
		}

		private void StylizeButton(MetroButton button, bool hovered)
		{
			button.Cursor = Cursors.Hand;
			button.MouseClick -= new MouseEventHandler(this.button_MouseClick);
			button.MouseClick += new MouseEventHandler(this.button_MouseClick);
			button.MouseEnter -= new EventHandler(this.button_MouseEnter);
			button.MouseEnter += new EventHandler(this.button_MouseEnter);
			button.MouseLeave -= new EventHandler(this.button_MouseLeave);
			button.MouseLeave += new EventHandler(this.button_MouseLeave);
		}
	}
}