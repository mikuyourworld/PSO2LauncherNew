using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace MetroFramework.Animation
{
	internal class DelayedCall<T> : DelayedCall
	{
		private DelayedCall<T>.Callback callback;

		private T data;

		public DelayedCall()
		{
		}

		public static DelayedCall<T> Create(DelayedCall<T>.Callback cb, T data, int milliseconds)
		{
			DelayedCall<T> delayedCall = new DelayedCall<T>();
			DelayedCall.PrepareDCObject(delayedCall, milliseconds, false);
			delayedCall.callback = cb;
			delayedCall.data = data;
			return delayedCall;
		}

		public static DelayedCall<T> CreateAsync(DelayedCall<T>.Callback cb, T data, int milliseconds)
		{
			DelayedCall<T> delayedCall = new DelayedCall<T>();
			DelayedCall.PrepareDCObject(delayedCall, milliseconds, true);
			delayedCall.callback = cb;
			delayedCall.data = data;
			return delayedCall;
		}

		protected override void OnFire()
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
					this.callback(this.data);
				}
			}, null);
		}

		public void Reset(T data, int milliseconds)
		{
			lock (this.timerLock)
			{
				base.Cancel();
				this.data = data;
				base.Milliseconds = milliseconds;
				base.Start();
			}
		}

		public static DelayedCall<T> Start(DelayedCall<T>.Callback cb, T data, int milliseconds)
		{
			DelayedCall<T> delayedCall = DelayedCall<T>.Create(cb, data, milliseconds);
			delayedCall.Start();
			return delayedCall;
		}

		public static DelayedCall<T> StartAsync(DelayedCall<T>.Callback cb, T data, int milliseconds)
		{
			DelayedCall<T> delayedCall = DelayedCall<T>.CreateAsync(cb, data, milliseconds);
			delayedCall.Start();
			return delayedCall;
		}

		public delegate void Callback(T data);
	}
}