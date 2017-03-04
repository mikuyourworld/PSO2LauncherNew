using System;
using System.ComponentModel;
using System.Drawing;
using System.Media;
using System.Threading;
using System.Windows.Forms;

namespace MetroFramework
{
	public static class MetroMessageBox
	{
		private static void ModalState(MetroMessageBoxControl control)
		{
			while (control.Visible)
			{
			}
		}

		public static DialogResult Show(IWin32Window owner, string message)
		{
			return MetroMessageBox.Show(owner, message, "Notification", 211);
		}

		public static DialogResult Show(IWin32Window owner, string message, int height)
		{
			return MetroMessageBox.Show(owner, message, "Notification", height);
		}

		public static DialogResult Show(IWin32Window owner, string message, string title)
		{
			return MetroMessageBox.Show(owner, message, title, MessageBoxButtons.OK, 211);
		}

		public static DialogResult Show(IWin32Window owner, string message, string title, int height)
		{
			return MetroMessageBox.Show(owner, message, title, MessageBoxButtons.OK, height);
		}

		public static DialogResult Show(IWin32Window owner, string message, string title, MessageBoxButtons buttons)
		{
			return MetroMessageBox.Show(owner, message, title, buttons, MessageBoxIcon.None, 211);
		}

		public static DialogResult Show(IWin32Window owner, string message, string title, MessageBoxButtons buttons, int height)
		{
			return MetroMessageBox.Show(owner, message, title, buttons, MessageBoxIcon.None, height);
		}

		public static DialogResult Show(IWin32Window owner, string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon)
		{
			return MetroMessageBox.Show(owner, message, title, buttons, icon, MessageBoxDefaultButton.Button1, 211);
		}

		public static DialogResult Show(IWin32Window owner, string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon, int height)
		{
			return MetroMessageBox.Show(owner, message, title, buttons, icon, MessageBoxDefaultButton.Button1, height);
		}

		public static DialogResult Show(IWin32Window owner, string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultbutton)
		{
			return MetroMessageBox.Show(owner, message, title, buttons, icon, defaultbutton, 211);
		}

		public static DialogResult Show(IWin32Window owner, string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultbutton, int height)
		{
			DialogResult result = DialogResult.None;
			if (owner != null)
			{
				Form form = (!(owner is Form) ? ((UserControl)owner).ParentForm : (Form)owner);
				MessageBoxIcon messageBoxIcon = icon;
				if (messageBoxIcon == MessageBoxIcon.Hand)
				{
					SystemSounds.Hand.Play();
				}
				else if (messageBoxIcon == MessageBoxIcon.Question)
				{
					SystemSounds.Beep.Play();
				}
				else if (messageBoxIcon == MessageBoxIcon.Exclamation)
				{
					SystemSounds.Exclamation.Play();
				}
				else
				{
					SystemSounds.Asterisk.Play();
				}
				MetroMessageBoxControl metroMessageBoxControl = new MetroMessageBoxControl()
				{
					BackColor = form.BackColor
				};
				metroMessageBoxControl.Properties.Buttons = buttons;
				metroMessageBoxControl.Properties.DefaultButton = defaultbutton;
				metroMessageBoxControl.Properties.Icon = icon;
				metroMessageBoxControl.Properties.Message = message;
				metroMessageBoxControl.Properties.Title = title;
				metroMessageBoxControl.Padding = new Padding(0, 0, 0, 0);
				metroMessageBoxControl.ControlBox = false;
				metroMessageBoxControl.ShowInTaskbar = false;
				metroMessageBoxControl.TopMost = true;
				metroMessageBoxControl.Size = new Size(form.Size.Width, height);
				int x = form.Location.X;
				Point location = form.Location;
				metroMessageBoxControl.Location = new Point(x, location.Y + (form.Height - metroMessageBoxControl.Height) / 2);
				metroMessageBoxControl.ArrangeApperance();
				Size size = metroMessageBoxControl.Size;
				Convert.ToInt32(Math.Floor((double)size.Height * 0.28));
				metroMessageBoxControl.ShowDialog();
				metroMessageBoxControl.BringToFront();
				metroMessageBoxControl.SetDefaultButton();
				Action<MetroMessageBoxControl> action = new Action<MetroMessageBoxControl>(MetroMessageBox.ModalState);
				IAsyncResult asyncResult = action.BeginInvoke(metroMessageBoxControl, null, action);
				bool flag = false;
				try
				{
					while (!asyncResult.IsCompleted)
					{
						Thread.Sleep(1);
						Application.DoEvents();
					}
				}
				catch
				{
					flag = true;
					if (!asyncResult.IsCompleted)
					{
						try
						{
							asyncResult = null;
						}
						catch
						{
						}
					}
					action = null;
				}
				if (!flag)
				{
					result = metroMessageBoxControl.Result;
					metroMessageBoxControl.Dispose();
					metroMessageBoxControl = null;
				}
			}
			return result;
		}
	}
}