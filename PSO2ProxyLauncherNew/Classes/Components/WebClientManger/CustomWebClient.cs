using System;
using System.Timers;

namespace PSO2ProxyLauncherNew.Classes.Components.WebClientManger
{
    class CustomWebClient : ExtendedWebClient
    {
        //private System.ComponentModel.BackgroundWorker myBWorker;
        private Timer myTimer;

        public double LifeTime
        {
            get
            {
                if (this.myTimer != null)
                    return this.myTimer.Interval;
                else
                    return -1;
            }
            set
            {
                if (value > -1)
                {
                    if (this.myTimer != null)
                        this.myTimer.Interval = value;
                    else
                        this.myTimer = this.CreateTimer(value);
                }
                else if (this.myTimer != null)
                {
                    this.myTimer.Dispose();
                    this.myTimer = null;
                }
            }
        }

        public CustomWebClient(double iLifeTime) : base()
        {
            this.WorkStarted += CustomWebClient_WorkStarted;
            this.WorkFinished += CustomWebClient_WorkFinished;
            if (iLifeTime > -1)
                this.myTimer = this.CreateTimer(iLifeTime);
        }

        public CustomWebClient() : this(-1) { }

        private Timer CreateTimer(double iLifeTime)
        {
            var tmp = new Timer(iLifeTime);
            tmp.AutoReset = false;
            tmp.Enabled = false;
            tmp.Stop();
            tmp.Elapsed += MyTimer_Elapsed;
            return tmp;
        }

        private void CustomWebClient_WorkFinished(object sender, EventArgs e)
        {
            this.CustomWebClient_FinishTask();
        }

        private void CustomWebClient_WorkStarted(object sender, EventArgs e)
        {
            this.Cancel_SelfDestruct();
        }

        private void CustomWebClient_FinishTask()
        {
            if (this.myTimer != null)
                this.myTimer.Start();
        }

        public void Cancel_SelfDestruct()
        {
            if (this.myTimer != null)
                this.myTimer.Stop();
        }

        private void MyTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Dispose();
        }

        public new void Dispose()
        {
            if (this.myTimer != null)
                this.myTimer.Dispose();
            base.Dispose();
            this.Disposed?.Invoke(this, null);
        }

        public event EventHandler Disposed;
    }
}
