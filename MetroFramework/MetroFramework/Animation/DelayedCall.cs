using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Timers;

namespace MetroFramework.Animation
{
	internal class DelayedCall : IDisposable
	{
		protected static List<DelayedCall> dcList;

		protected System.Timers.Timer timer;

		protected object timerLock;

		private DelayedCall.Callback callback;

		protected bool cancelled;

		protected SynchronizationContext context;

		private DelayedCall<object>.Callback oldCallback;

		private object oldData;

		public static bool IsAnyWaiting
		{
			get
			{
				bool flag;
				lock (DelayedCall.dcList)
				{
					foreach (DelayedCall delayedCall in DelayedCall.dcList)
					{
						if (!delayedCall.IsWaiting)
						{
							continue;
						}
						flag = true;
						return flag;
					}
					return false;
				}
				return flag;
			}
		}

		public bool IsWaiting
		{
			get
			{
				bool flag;
				lock (this.timerLock)
				{
					flag = (!this.timer.Enabled ? false : !this.cancelled);
				}
				return flag;
			}
		}

		public int Milliseconds
		{
			get
			{
				int interval;
				lock (this.timerLock)
				{
					interval = (int)this.timer.Interval;
				}
				return interval;
			}
			set
			{
				lock (this.timerLock)
				{
					if (value < 0)
					{
						throw new ArgumentOutOfRangeException("Milliseconds", "The new timeout must be 0 or greater.");
					}
					if (value != 0)
					{
						this.timer.Interval = (double)value;
					}
					else
					{
						this.Cancel();
						this.FireNow();
						DelayedCall.Unregister(this);
					}
				}
			}
		}

		public static int RegisteredCount
		{
			get
			{
				int count;
				lock (DelayedCall.dcList)
				{
					count = DelayedCall.dcList.Count;
				}
				return count;
			}
		}

		static DelayedCall()
		{
			DelayedCall.dcList = new List<DelayedCall>();
		}

		protected DelayedCall()
		{
			this.timerLock = new object();
		}

		[Obsolete("Use the static method DelayedCall.Create instead.")]
		public DelayedCall(DelayedCall.Callback cb) : this()
		{
			DelayedCall.PrepareDCObject(this, 0, false);
			this.callback = cb;
		}

		[Obsolete("Use the static method DelayedCall.Create instead.")]
		public DelayedCall(DelayedCall<object>.Callback cb, object data) : this()
		{
			DelayedCall.PrepareDCObject(this, 0, false);
			this.oldCallback = cb;
			this.oldData = data;
		}

		[Obsolete("Use the static method DelayedCall.Start instead.")]
		public DelayedCall(DelayedCall.Callback cb, int milliseconds) : this()
		{
			DelayedCall.PrepareDCObject(this, milliseconds, false);
			this.callback = cb;
			if (milliseconds > 0)
			{
				this.Start();
			}
		}

		[Obsolete("Use the static method DelayedCall.Start instead.")]
		public DelayedCall(DelayedCall<object>.Callback cb, int milliseconds, object data) : this()
		{
			DelayedCall.PrepareDCObject(this, milliseconds, false);
			this.oldCallback = cb;
			this.oldData = data;
			if (milliseconds > 0)
			{
				this.Start();
			}
		}

		public void Cancel()
		{
			lock (this.timerLock)
			{
				this.cancelled = true;
				DelayedCall.Unregister(this);
				this.timer.Stop();
			}
		}

		public static void CancelAll()
		{
			lock (DelayedCall.dcList)
			{
				foreach (DelayedCall delayedCall in DelayedCall.dcList)
				{
					delayedCall.Cancel();
				}
			}
		}

		public static DelayedCall Create(DelayedCall.Callback cb, int milliseconds)
		{
			DelayedCall delayedCall = new DelayedCall();
			DelayedCall.PrepareDCObject(delayedCall, milliseconds, false);
			delayedCall.callback = cb;
			return delayedCall;
		}

		public static DelayedCall CreateAsync(DelayedCall.Callback cb, int milliseconds)
		{
			DelayedCall delayedCall = new DelayedCall();
			DelayedCall.PrepareDCObject(delayedCall, milliseconds, true);
			delayedCall.callback = cb;
			return delayedCall;
		}

		public void Dispose()
		{
			DelayedCall.Unregister(this);
			this.timer.Dispose();
		}

		public static void DisposeAll()
		{
			lock (DelayedCall.dcList)
			{
				while (DelayedCall.dcList.Count > 0)
				{
					DelayedCall.dcList[0].Dispose();
				}
			}
		}

		public void Fire()
		{
			lock (this.timerLock)
			{
				if (this.IsWaiting)
				{
					this.timer.Stop();
				}
				else
				{
					return;
				}
			}
			this.FireNow();
		}

		public static void FireAll()
		{
			lock (DelayedCall.dcList)
			{
				foreach (DelayedCall delayedCall in DelayedCall.dcList)
				{
					delayedCall.Fire();
				}
			}
		}

		public void FireNow()
		{
			this.OnFire();
			DelayedCall.Unregister(this);
		}

		protected virtual void OnFire()
		{
			this.context.Post((object argument0) => {
				lock (this.timerLock)
				{
					if (this.cancelled)
					{
						return;
					}
				}
				if (this.callback != null)
				{
					this.callback();
				}
				if (this.oldCallback != null)
				{
					this.oldCallback(this.oldData);
				}
			}, null);
		}

		protected static void PrepareDCObject(DelayedCall dc, int milliseconds, bool async)
		{
			if (milliseconds < 0)
			{
				throw new ArgumentOutOfRangeException("milliseconds", "The new timeout must be 0 or greater.");
			}
			dc.context = null;
			if (!async)
			{
				dc.context = SynchronizationContext.Current;
				if (dc.context == null)
				{
					throw new InvalidOperationException("Cannot delay calls synchronously on a non-UI thread. Use the *Async methods instead.");
				}
			}
			if (dc.context == null)
			{
				dc.context = new SynchronizationContext();
			}
			dc.timer = new System.Timers.Timer();
			if (milliseconds > 0)
			{
				dc.timer.Interval = (double)milliseconds;
			}
			dc.timer.AutoReset = false;
			DelayedCall delayedCall = dc;
			dc.timer.Elapsed += new ElapsedEventHandler(delayedCall.Timer_Elapsed);
			DelayedCall.Register(dc);
		}

		protected static void Register(DelayedCall dc)
		{
			lock (DelayedCall.dcList)
			{
				if (!DelayedCall.dcList.Contains(dc))
				{
					DelayedCall.dcList.Add(dc);
				}
			}
		}

		[Obsolete("Use the method Restart of the generic class instead.")]
		public void Reset(object data)
		{
			this.Cancel();
			this.oldData = data;
			this.Start();
		}

		[Obsolete("Use the method Restart of the generic class instead.")]
		public void Reset(int milliseconds, object data)
		{
			this.Cancel();
			this.oldData = data;
			this.Reset(milliseconds);
		}

		public void Reset()
		{
			lock (this.timerLock)
			{
				this.Cancel();
				this.Start();
			}
		}

		public void Reset(int milliseconds)
		{
			lock (this.timerLock)
			{
				this.Cancel();
				this.Milliseconds = milliseconds;
				this.Start();
			}
		}

		[Obsolete("Use the method Restart instead.")]
		public void SetTimeout(int milliseconds)
		{
			this.Reset(milliseconds);
		}

		public static DelayedCall Start(DelayedCall.Callback cb, int milliseconds)
		{
			DelayedCall delayedCall = DelayedCall.Create(cb, milliseconds);
			if (milliseconds > 0)
			{
				delayedCall.Start();
			}
			else if (milliseconds == 0)
			{
				delayedCall.FireNow();
			}
			return delayedCall;
		}

		public void Start()
		{
			lock (this.timerLock)
			{
				this.cancelled = false;
				this.timer.Start();
				DelayedCall.Register(this);
			}
		}

		public static DelayedCall StartAsync(DelayedCall.Callback cb, int milliseconds)
		{
			DelayedCall delayedCall = DelayedCall.CreateAsync(cb, milliseconds);
			if (milliseconds > 0)
			{
				delayedCall.Start();
			}
			else if (milliseconds == 0)
			{
				delayedCall.FireNow();
			}
			return delayedCall;
		}

		protected virtual void Timer_Elapsed(object o, ElapsedEventArgs e)
		{
			this.FireNow();
			DelayedCall.Unregister(this);
		}

		protected static void Unregister(DelayedCall dc)
		{
			lock (DelayedCall.dcList)
			{
				DelayedCall.dcList.Remove(dc);
			}
		}

		public delegate void Callback();
	}
}