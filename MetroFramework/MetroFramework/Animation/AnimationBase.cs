using System;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace MetroFramework.Animation
{
	public abstract class AnimationBase
	{
		private DelayedCall timer;

		private Control targetControl;

		private AnimationAction actionHandler;

		private AnimationFinishedEvaluator evaluatorHandler;

		protected TransitionType transitionType;

		protected int counter;

		protected int startTime;

		protected int targetTime;

		public bool IsCompleted
		{
			get
			{
				if (this.timer == null)
				{
					return true;
				}
				return !this.timer.IsWaiting;
			}
		}

		public bool IsRunning
		{
			get
			{
				if (this.timer == null)
				{
					return false;
				}
				return this.timer.IsWaiting;
			}
		}

		protected AnimationBase()
		{
		}

		public void Cancel()
		{
			if (this.IsRunning)
			{
				this.timer.Cancel();
			}
		}

		private void DoAnimation()
		{
			if (this.evaluatorHandler == null || this.evaluatorHandler())
			{
				this.OnAnimationCompleted();
				return;
			}
			this.actionHandler();
			AnimationBase animationBase = this;
			animationBase.counter = animationBase.counter + 1;
			this.timer.Start();
		}

		protected int MakeTransition(float t, float b, float d, float c)
		{
			switch (this.transitionType)
			{
				case TransitionType.Linear:
				{
					return (int)(c * t / d + b);
				}
				case TransitionType.EaseInQuad:
				{
					float single = t / d;
					t = single;
					return (int)(c * single * t + b);
				}
				case TransitionType.EaseOutQuad:
				{
					float single1 = t / d;
					t = single1;
					return (int)(-c * single1 * (t - 2f) + b);
				}
				case TransitionType.EaseInOutQuad:
				{
					float single2 = t / (d / 2f);
					t = single2;
					if (single2 < 1f)
					{
						return (int)(c / 2f * t * t + b);
					}
					float single3 = t - 1f;
					t = single3;
					return (int)(-c / 2f * (single3 * (t - 2f) - 1f) + b);
				}
				case TransitionType.EaseInCubic:
				{
					float single4 = t / d;
					t = single4;
					return (int)(c * single4 * t * t + b);
				}
				case TransitionType.EaseOutCubic:
				{
					float single5 = t / d - 1f;
					t = single5;
					return (int)(c * (single5 * t * t + 1f) + b);
				}
				case TransitionType.EaseInOutCubic:
				{
					float single6 = t / (d / 2f);
					t = single6;
					if (single6 < 1f)
					{
						return (int)(c / 2f * t * t * t + b);
					}
					float single7 = t - 2f;
					t = single7;
					return (int)(c / 2f * (single7 * t * t + 2f) + b);
				}
				case TransitionType.EaseInQuart:
				{
					float single8 = t / d;
					t = single8;
					return (int)(c * single8 * t * t * t + b);
				}
				case TransitionType.EaseInExpo:
				{
					if (t == 0f)
					{
						return (int)b;
					}
					return (int)((double)c * Math.Pow(2, (double)(10f * (t / d - 1f))) + (double)b);
				}
				case TransitionType.EaseOutExpo:
				{
					if (t == d)
					{
						return (int)(b + c);
					}
					return (int)((double)c * (-Math.Pow(2, (double)(-10f * t / d)) + 1) + (double)b);
				}
			}
			return 0;
		}

		private void OnAnimationCompleted()
		{
			if (this.AnimationCompleted != null)
			{
				this.AnimationCompleted(this, EventArgs.Empty);
			}
		}

		protected void Start(Control control, TransitionType transitionType, int duration, AnimationAction actionHandler)
		{
			this.Start(control, transitionType, duration, actionHandler, null);
		}

		protected void Start(Control control, TransitionType transitionType, int duration, AnimationAction actionHandler, AnimationFinishedEvaluator evaluatorHandler)
		{
			this.targetControl = control;
			this.transitionType = transitionType;
			this.actionHandler = actionHandler;
			this.evaluatorHandler = evaluatorHandler;
			this.counter = 0;
			this.startTime = 0;
			this.targetTime = duration;
			this.timer = DelayedCall.Start(new DelayedCall.Callback(this.DoAnimation), duration);
		}

		public event EventHandler AnimationCompleted;
	}
}