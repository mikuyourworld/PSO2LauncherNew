using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace MetroFramework.Animation
{
	internal class DelayedCall<T1, T2> : DelayedCall
	{
		private DelayedCall<T1, T2>.Callback callback;

		private T1 data1;

		private T2 data2;

		public DelayedCall()
		{
		}

		public static DelayedCall<T1, T2> Create(DelayedCall<T1, T2>.Callback cb, T1 data1, T2 data2, int milliseconds)
		{
			DelayedCall<T1, T2> delayedCall = new DelayedCall<T1, T2>();
			DelayedCall.PrepareDCObject(delayedCall, milliseconds, false);
			delayedCall.callback = cb;
			delayedCall.data1 = data1;
			delayedCall.data2 = data2;
			return delayedCall;
		}

		public static DelayedCall<T1, T2> CreateAsync(DelayedCall<T1, T2>.Callback cb, T1 data1, T2 data2, int milliseconds)
		{
			DelayedCall<T1, T2> delayedCall = new DelayedCall<T1, T2>();
			DelayedCall.PrepareDCObject(delayedCall, milliseconds, true);
			delayedCall.callback = cb;
			delayedCall.data1 = data1;
			delayedCall.data2 = data2;
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
					this.callback(this.data1, this.data2);
				}
			}, null);
		}

		public void Reset(T1 data1, T2 data2, int milliseconds)
		{
			lock (this.timerLock)
			{
				base.Cancel();
				this.data1 = data1;
				this.data2 = data2;
				base.Milliseconds = milliseconds;
				base.Start();
			}
		}

		public static DelayedCall<T1, T2> Start(DelayedCall<T1, T2>.Callback cb, T1 data1, T2 data2, int milliseconds)
		{
			DelayedCall<T1, T2> delayedCall = DelayedCall<T1, T2>.Create(cb, data1, data2, milliseconds);
			delayedCall.Start();
			return delayedCall;
		}

		public static DelayedCall<T1, T2> StartAsync(DelayedCall<T1, T2>.Callback cb, T1 data1, T2 data2, int milliseconds)
		{
			DelayedCall<T1, T2> delayedCall = DelayedCall<T1, T2>.CreateAsync(cb, data1, data2, milliseconds);
			delayedCall.Start();
			return delayedCall;
		}

		public delegate void Callback(T1 data1, T2 data2);
	}
}