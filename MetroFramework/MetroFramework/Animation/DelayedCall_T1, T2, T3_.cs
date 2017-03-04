using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace MetroFramework.Animation
{
	internal class DelayedCall<T1, T2, T3> : DelayedCall
	{
		private DelayedCall<T1, T2, T3>.Callback callback;

		private T1 data1;

		private T2 data2;

		private T3 data3;

		public DelayedCall()
		{
		}

		public static DelayedCall<T1, T2, T3> Create(DelayedCall<T1, T2, T3>.Callback cb, T1 data1, T2 data2, T3 data3, int milliseconds)
		{
			DelayedCall<T1, T2, T3> delayedCall = new DelayedCall<T1, T2, T3>();
			DelayedCall.PrepareDCObject(delayedCall, milliseconds, false);
			delayedCall.callback = cb;
			delayedCall.data1 = data1;
			delayedCall.data2 = data2;
			delayedCall.data3 = data3;
			return delayedCall;
		}

		public static DelayedCall<T1, T2, T3> CreateAsync(DelayedCall<T1, T2, T3>.Callback cb, T1 data1, T2 data2, T3 data3, int milliseconds)
		{
			DelayedCall<T1, T2, T3> delayedCall = new DelayedCall<T1, T2, T3>();
			DelayedCall.PrepareDCObject(delayedCall, milliseconds, true);
			delayedCall.callback = cb;
			delayedCall.data1 = data1;
			delayedCall.data2 = data2;
			delayedCall.data3 = data3;
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
					this.callback(this.data1, this.data2, this.data3);
				}
			}, null);
		}

		public void Reset(T1 data1, T2 data2, T3 data3, int milliseconds)
		{
			lock (this.timerLock)
			{
				base.Cancel();
				this.data1 = data1;
				this.data2 = data2;
				this.data3 = data3;
				base.Milliseconds = milliseconds;
				base.Start();
			}
		}

		public static DelayedCall<T1, T2, T3> Start(DelayedCall<T1, T2, T3>.Callback cb, T1 data1, T2 data2, T3 data3, int milliseconds)
		{
			DelayedCall<T1, T2, T3> delayedCall = DelayedCall<T1, T2, T3>.Create(cb, data1, data2, data3, milliseconds);
			delayedCall.Start();
			return delayedCall;
		}

		public static DelayedCall<T1, T2, T3> StartAsync(DelayedCall<T1, T2, T3>.Callback cb, T1 data1, T2 data2, T3 data3, int milliseconds)
		{
			DelayedCall<T1, T2, T3> delayedCall = DelayedCall<T1, T2, T3>.CreateAsync(cb, data1, data2, data3, milliseconds);
			delayedCall.Start();
			return delayedCall;
		}

		public delegate void Callback(T1 data1, T2 data2, T3 data3);
	}
}