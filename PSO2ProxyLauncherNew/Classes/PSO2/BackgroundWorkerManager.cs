using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using PSO2ProxyLauncherNew.Classes.Events;

namespace PSO2ProxyLauncherNew.Classes.PSO2
{
    class BackgroundWorkerManager : IEnumerable<ExtendedBackgroundWorker>
    {

        private List<ExtendedBackgroundWorker> working;
        private List<ExtendedBackgroundWorker> resting;

        public BackgroundWorkerManager(int _maxcount)
        {
            this._Count = 0;
            this.MaxCount = _maxcount;
            this.working = new List<ExtendedBackgroundWorker>();
            this.resting = new List<ExtendedBackgroundWorker>();
        }

        public BackgroundWorkerManager() : this(0) { }

        private int _Count;
        public int Count { get { return this._Count; } }

        private int _MaxCount;
        public int MaxCount
        {
            get { return this._MaxCount; }
            set
            {
                this._MaxCount = value;
                this.AdjustNumberOfBWorker();
            }
        }

        public void CancelAsync()
        {
            for (int i = 0; i < this.working.Count; i++)
                this.working[i].CancelAsync();
        }

        public int GetNumberOfRunning()
        {
            return this.working.Count;
        }

        private void AdjustNumberOfBWorker()
        {
            if (this.Count == this.MaxCount)
            { return; }
            else
            {
                if (this.Count < this.MaxCount)
                {
                    this.Add(new ExtendedBackgroundWorker());
                }
                else if (this.Count > this.MaxCount)
                {
                    if (this.resting.Count > 0)
                        this.Remove(this.resting[0]);
                    else if (this.working.Count > 0)
                        this.Remove(this.working[0]);
                }
                this.AdjustNumberOfBWorker();
            }
        }

        private void Add(ExtendedBackgroundWorker item)
        {
            if (!this.working.Contains(item) && !this.working.Contains(item))
            {
                item.StartWorking += this.Item_StartWorking;
                item.RunWorkerCompleted += this.Item_RunWorkerCompleted;
                this.WorkerAdded?.Invoke(this, new ExtendedBackgroundWorkerEventArgs(item));
                this.resting.Add(item);
                Interlocked.Increment(ref this._Count);
            }
        }

        public event EventHandler<ExtendedBackgroundWorkerEventArgs> WorkerAdded;

        private void Item_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ExtendedBackgroundWorker bw = sender as ExtendedBackgroundWorker;
            if (this.working.Contains(bw))
            {
                this.resting.Add(bw);
                this.working.Remove(bw);
            }
            else if (!this.resting.Contains(bw))
            {
                bw.Dispose();
            }
        }

        public ExtendedBackgroundWorker GetRestingWorker()
        {
            if (this.resting.Count > 0)
                return this.resting[0];
            else
                return null;
        }

        private void Item_StartWorking(object sender, EventArgs e)
        {
            ExtendedBackgroundWorker bw = sender as ExtendedBackgroundWorker;
            if (this.resting.Contains(bw))
            {
                this.working.Add(bw);
                this.resting.Remove(bw);
            }
        }

        public void Clear()
        {
            this.MaxCount = 0;
        }

        public bool Contains(ExtendedBackgroundWorker item)
        {
            return (this.working.Contains(item) || this.working.Contains(item));
        }

        public IEnumerator<ExtendedBackgroundWorker> GetEnumerator()
        {
            List<ExtendedBackgroundWorker> result = new List<ExtendedBackgroundWorker>();
            for (int i = 0; i < this.working.Count; i++)
                if (!result.Contains(this.working[i]))
                    result.Add(this.working[i]);
            for (int i = 0; i < this.resting.Count; i++)
                if (!result.Contains(this.resting[i]))
                    result.Add(this.resting[i]);
            return result.GetEnumerator();
        }

        private bool Remove(ExtendedBackgroundWorker item)
        {
            if (this.working.Contains(item))
            {
                item.StartWorking += this.Item_StartWorking;
                item.RunWorkerCompleted += this.Item_RunWorkerCompleted;
                Interlocked.Decrement(ref this._Count);
                this.WorkerRemoved?.Invoke(this, new ExtendedBackgroundWorkerEventArgs(item));
                return this.working.Remove(item);
            }
            else if (this.resting.Contains(item))
            {
                item.StartWorking += this.Item_StartWorking;
                item.RunWorkerCompleted += this.Item_RunWorkerCompleted;
                Interlocked.Decrement(ref this._Count);
                this.WorkerRemoved?.Invoke(this, new ExtendedBackgroundWorkerEventArgs(item));
                return this.resting.Remove(item);
            }
            else
                return false;
        }

        public event EventHandler<ExtendedBackgroundWorkerEventArgs> WorkerRemoved;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    class ExtendedBackgroundWorker : BackgroundWorker
    {
        public ExtendedBackgroundWorker() : base() { }

        public new void RunWorkerAsync()
        {
            this.StartWorking?.Invoke(this, System.EventArgs.Empty);
            base.RunWorkerAsync();
        }

        public new void RunWorkerAsync(object argument)
        {
            this.StartWorking?.Invoke(this, System.EventArgs.Empty);
            base.RunWorkerAsync(argument);
        }
        public event EventHandler StartWorking;
    }
}
