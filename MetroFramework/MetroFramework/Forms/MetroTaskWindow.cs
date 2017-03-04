using MetroFramework.Animation;
using MetroFramework.Components;
using MetroFramework.Controls;
using MetroFramework.Drawing;
using MetroFramework.Interfaces;
using MetroFramework.Native;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MetroFramework.Forms
{
	public sealed class MetroTaskWindow : MetroForm
	{
		private static MetroTaskWindow singletonWindow;

		private bool cancelTimer;

		private readonly int closeTime;

		private int elapsedTime;

		private int progressWidth;

		private DelayedCall timer;

		private readonly MetroPanel controlContainer;

		private bool isInitialized;

		public bool CancelTimer
		{
			get
			{
				return this.cancelTimer;
			}
			set
			{
				this.cancelTimer = value;
			}
		}

		public MetroTaskWindow()
		{
			this.controlContainer = new MetroPanel();
			base.Controls.Add(this.controlContainer);
		}

		public MetroTaskWindow(int duration, Control userControl) : this()
		{
			this.controlContainer.Controls.Add(userControl);
			userControl.Dock = DockStyle.Fill;
			this.closeTime = duration * 500;
			if (this.closeTime > 0)
			{
				this.timer = DelayedCall.Start(new DelayedCall.Callback(this.UpdateProgress), 5);
			}
		}

		public static void CancelAutoClose()
		{
			if (MetroTaskWindow.singletonWindow != null)
			{
				MetroTaskWindow.singletonWindow.CancelTimer = true;
			}
		}

		public static void ForceClose()
		{
			if (MetroTaskWindow.singletonWindow != null)
			{
				MetroTaskWindow.CancelAutoClose();
				MetroTaskWindow.singletonWindow.Close();
				MetroTaskWindow.singletonWindow.Dispose();
				MetroTaskWindow.singletonWindow = null;
			}
		}

		public static bool IsVisible()
		{
			if (MetroTaskWindow.singletonWindow == null)
			{
				return false;
			}
			return MetroTaskWindow.singletonWindow.Visible;
		}

		protected override void OnActivated(EventArgs e)
		{
			if (!this.isInitialized)
			{
				this.controlContainer.Theme = base.Theme;
				this.controlContainer.Style = base.Style;
				this.controlContainer.StyleManager = base.StyleManager;
				base.MaximizeBox = false;
				base.MinimizeBox = false;
				base.Movable = true;
				base.TopMost = true;
				base.Size = new System.Drawing.Size(400, 200);
				Taskbar taskbar = new Taskbar();
				switch (taskbar.Position)
				{
					case TaskbarPosition.Unknown:
					{
						Rectangle bounds = Screen.PrimaryScreen.Bounds;
						Rectangle rectangle = Screen.PrimaryScreen.Bounds;
						base.Location = new Point(bounds.Width - base.Width - 5, rectangle.Height - base.Height - 5);
						break;
					}
					case TaskbarPosition.Left:
					{
						Rectangle bounds1 = taskbar.Bounds;
						Rectangle rectangle1 = taskbar.Bounds;
						base.Location = new Point(bounds1.Width + 5, rectangle1.Height - base.Height - 5);
						break;
					}
					case TaskbarPosition.Top:
					{
						Rectangle bounds2 = taskbar.Bounds;
						Rectangle rectangle2 = taskbar.Bounds;
						base.Location = new Point(bounds2.Width - base.Width - 5, rectangle2.Height + 5);
						break;
					}
					case TaskbarPosition.Right:
					{
						Rectangle bounds3 = taskbar.Bounds;
						Rectangle rectangle3 = taskbar.Bounds;
						base.Location = new Point(bounds3.X - base.Width - 5, rectangle3.Height - base.Height - 5);
						break;
					}
					case TaskbarPosition.Bottom:
					{
						Rectangle bounds4 = taskbar.Bounds;
						Rectangle rectangle4 = taskbar.Bounds;
						base.Location = new Point(bounds4.Width - base.Width - 5, rectangle4.Y - base.Height - 5);
						break;
					}
					default:
					{
						goto case TaskbarPosition.Unknown;
					}
				}
				this.controlContainer.Location = new Point(0, 60);
				this.controlContainer.Size = new System.Drawing.Size(base.Width - 40, base.Height - 80);
				this.controlContainer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
				this.controlContainer.AutoScroll = false;
				this.controlContainer.HorizontalScrollbar = false;
				this.controlContainer.VerticalScrollbar = false;
				this.controlContainer.Refresh();
				if (base.StyleManager != null)
				{
					base.StyleManager.Update();
				}
				this.isInitialized = true;
				MoveAnimation moveAnimation = new MoveAnimation();
				moveAnimation.Start(this.controlContainer, new Point(20, 60), TransitionType.EaseInOutCubic, 15);
			}
			base.OnActivated(e);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			using (SolidBrush solidBrush = new SolidBrush(MetroPaint.BackColor.Form(base.Theme)))
			{
				e.Graphics.FillRectangle(solidBrush, new Rectangle(base.Width - this.progressWidth, 0, this.progressWidth, 5));
			}
		}

		public static void ShowTaskWindow(IWin32Window parent, string title, Control userControl, int secToClose)
		{
			if (MetroTaskWindow.singletonWindow != null)
			{
				MetroTaskWindow.singletonWindow.Close();
				MetroTaskWindow.singletonWindow.Dispose();
				MetroTaskWindow.singletonWindow = null;
			}
			MetroTaskWindow.singletonWindow = new MetroTaskWindow(secToClose, userControl)
			{
				Text = title,
				Resizable = false,
				Movable = true,
				StartPosition = FormStartPosition.Manual
			};
			if (parent != null && parent is IMetroForm)
			{
				MetroTaskWindow.singletonWindow.Theme = ((IMetroForm)parent).Theme;
				MetroTaskWindow.singletonWindow.Style = ((IMetroForm)parent).Style;
				MetroTaskWindow.singletonWindow.StyleManager = ((IMetroForm)parent).StyleManager.Clone(MetroTaskWindow.singletonWindow) as MetroStyleManager;
			}
			MetroTaskWindow.singletonWindow.Show();
		}

		public static void ShowTaskWindow(IWin32Window parent, string text, Control userControl)
		{
			MetroTaskWindow.ShowTaskWindow(parent, text, userControl, 0);
		}

		public static void ShowTaskWindow(string text, Control userControl, int secToClose)
		{
			MetroTaskWindow.ShowTaskWindow(null, text, userControl, secToClose);
		}

		public static void ShowTaskWindow(string text, Control userControl)
		{
			MetroTaskWindow.ShowTaskWindow(null, text, userControl);
		}

		private void UpdateProgress()
		{
			if (this.elapsedTime == this.closeTime)
			{
				this.timer.Dispose();
				this.timer = null;
				base.Close();
				return;
			}
			MetroTaskWindow metroTaskWindow = this;
			metroTaskWindow.elapsedTime = metroTaskWindow.elapsedTime + 5;
			if (this.cancelTimer)
			{
				this.elapsedTime = 0;
			}
			double num = (double)this.elapsedTime / ((double)this.closeTime / 100);
			this.progressWidth = (int)((double)base.Width * (num / 100));
			base.Invalidate(new Rectangle(0, 0, base.Width, 5));
			if (!this.cancelTimer)
			{
				this.timer.Reset();
			}
		}
	}
}