using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace MetroFramework.Animation
{
	public sealed class MoveAnimation : AnimationBase
	{
		public MoveAnimation()
		{
		}

		private int DoMoveAnimation(int startPos, int targetPos)
		{
			float single = (float)this.counter - (float)this.startTime;
			float single1 = (float)startPos;
			float single2 = (float)targetPos - (float)startPos;
			float single3 = (float)this.targetTime - (float)this.startTime;
			return base.MakeTransition(single, single1, single3, single2);
		}

		public void Start(Control control, Point targetPoint, TransitionType transitionType, int duration)
		{
			base.Start(control, transitionType, duration, () => {
				int num = this.DoMoveAnimation(control.Location.X, targetPoint.X);
				int num1 = this.DoMoveAnimation(control.Location.Y, targetPoint.Y);
				control.Location = new Point(num, num1);
			}, () => control.Location.Equals(targetPoint));
		}
	}
}