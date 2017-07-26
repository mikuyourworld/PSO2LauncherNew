using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Threading;

namespace Leayal.Ugoira.WPF
{
    public class UgoiraPlayer : IDisposable
    {
        public static void CreateAsync(string filename, EventHandler<UgoiraPlayerCreatedEventArgs> callback)
        {
            UgoiraPlayer.CreateAsync(filename, null, callback);
        }
        public static void CreateAsync(string filename, EventHandler<UgoiraReadingEventArgs> reportProgress, EventHandler<UgoiraPlayerCreatedEventArgs> callback)
        {
            SynchronizationContext context = SynchronizationContext.Current;
            ThreadPool.QueueUserWorkItem(new WaitCallback(delegate
            {
                try
                {
                    UgoiraPlayer player = UgoiraPlayer.Create(filename, reportProgress);
                    if (player == null)
                    {
                        if (context != null)
                            context.Post(new SendOrPostCallback(delegate { callback?.Invoke(null, new UgoiraPlayerCreatedEventArgs(true)); }), null);
                        else
                            callback?.Invoke(null, new UgoiraPlayerCreatedEventArgs(true));
                    }
                    else
                    {
                        if (context != null)
                            context.Post(new SendOrPostCallback(delegate { callback?.Invoke(null, new UgoiraPlayerCreatedEventArgs(player)); }), null);
                        else
                            callback?.Invoke(null, new UgoiraPlayerCreatedEventArgs(player));
                    }
                }
                catch (Exception ex)
                {
                    if (context != null)
                        context.Post(new SendOrPostCallback(delegate { callback?.Invoke(null, new UgoiraPlayerCreatedEventArgs(ex)); }), null);
                    else
                        callback?.Invoke(null, new UgoiraPlayerCreatedEventArgs(ex));
                }
            }), null);
        }
        public static void CreateAsync(Stream sourceStream, EventHandler<UgoiraPlayerCreatedEventArgs> callback)
        {
            UgoiraPlayer.CreateAsync(sourceStream, null, callback);
        }
        public static void CreateAsync(Stream sourceStream, EventHandler<UgoiraReadingEventArgs> reportProgress, EventHandler<UgoiraPlayerCreatedEventArgs> callback)
        {
            SynchronizationContext context = SynchronizationContext.Current;
            ThreadPool.QueueUserWorkItem(new WaitCallback(delegate 
            {
                try
                {
                    UgoiraPlayer player = UgoiraPlayer.Create(sourceStream, reportProgress);
                    if (player == null)
                    {
                        if (context != null)
                            context.Post(new SendOrPostCallback(delegate { callback?.Invoke(null, new UgoiraPlayerCreatedEventArgs(true)); }), null);
                        else
                            callback?.Invoke(null, new UgoiraPlayerCreatedEventArgs(true));
                    }
                    else
                    {
                        if (context != null)
                            context.Post(new SendOrPostCallback(delegate { callback?.Invoke(null, new UgoiraPlayerCreatedEventArgs(player)); }), null);
                        else
                            callback?.Invoke(null, new UgoiraPlayerCreatedEventArgs(player));
                    }
                }
                catch (Exception ex)
                {
                    if (context != null)
                        context.Post(new SendOrPostCallback(delegate { callback?.Invoke(null, new UgoiraPlayerCreatedEventArgs(ex)); }), null);
                    else
                        callback?.Invoke(null, new UgoiraPlayerCreatedEventArgs(ex));
                }
            }), null);
        }
        public static UgoiraPlayer Create(string filename)
        {
            return UgoiraPlayer.Create(filename, null);
        }
        public static UgoiraPlayer Create(string filename, EventHandler<UgoiraReadingEventArgs> reportProgress)
        {
            using (FileStream fs = File.OpenRead(filename))
                return UgoiraPlayer.Create(fs, reportProgress);
        }
        public static UgoiraPlayer Create(Stream sourceStream)
        {
            return UgoiraPlayer.Create(sourceStream, null);
        }
        public static UgoiraPlayer Create(Stream sourceStream, EventHandler<UgoiraReadingEventArgs> reportProgress)
        {
            string illustTitle = string.Empty, illustUsername = string.Empty;
            Dictionary<string, BitmapImage> framelist = new Dictionary<string, BitmapImage>(StringComparer.OrdinalIgnoreCase);
            Dictionary<string, FrameHeader> headerlist = new Dictionary<string, FrameHeader>(StringComparer.OrdinalIgnoreCase);
            using (UgoiraReader reader = new UgoiraReader(sourceStream))
            {
                UgoiraReadingEventArgs eventa = null;
                if (reportProgress != null)
                    eventa = new UgoiraReadingEventArgs(reader.Entries.Length);
                for (int i = 0; i < reader.Entries.Length; i++)
                {
                    if (eventa != null)
                        eventa.SetCurrent(i + 1);
                    using (IO.RecyclableMemoryStream rms = new IO.RecyclableMemoryStream(reader[i].Filename, Convert.ToInt32(reader[i].Filesize)))
                    {
                        reader[i].Extract(rms);
                        rms.Position = 0;
                        switch (reader[i].Type)
                        {
                            case EntryType.Metadata:
                                JObject jo;
                                using (StreamReader sr = new StreamReader(rms))
                                using (Newtonsoft.Json.JsonTextReader jr = new Newtonsoft.Json.JsonTextReader(sr))
                                {
                                    jr.CloseInput = true;
                                    jo = JObject.Load(jr);
                                    jr.Close();
                                }
                                if (jo != null)
                                {
                                    illustTitle = jo.Value<object>("illustTitle").ToString();
                                    illustUsername = jo.Value<object>("userName").ToString();

                                    JToken jt = jo.SelectToken("ugokuIllustData");
                                    if (jt != null)
                                    {
                                        jt = jo.SelectToken("frames");
                                        if (jt != null)
                                        {
                                            string currentfilename;
                                            foreach (JToken token in jt.Children())
                                            {
                                                currentfilename = token.Value<object>("file").ToString();
                                                headerlist.Add(currentfilename, new FrameHeader(currentfilename, token.Value<int>("delay")));
                                            }
                                        }
                                    }
                                }
                                break;
                            case EntryType.Image:
                                framelist.Add(reader[i].Filename, QuickMethods.CreateBitmapImage(rms, true));
                                break;
                        }
                    }
                    if (eventa != null)
                    {
                        reportProgress?.Invoke(reader, eventa);
                        if (eventa.Cancel)
                        {
                            framelist.Clear();
                            headerlist.Clear();
                            return null;
                        }
                    }
                }
            }
            return new UgoiraPlayer(MakeFrames(framelist, headerlist, true));
        }

        private static Frame[] MakeFrames(IDictionary<string, BitmapImage> framelist, IDictionary<string, FrameHeader> headerlist, bool clearListAfterFinished)
        {
            Frame[] results = new Frame[framelist.Count];

            foreach (string filename in framelist.Keys)
            {
                int position = int.Parse(Path.ChangeExtension(filename, null));
                if (headerlist.ContainsKey(filename))
                    results[position] = new Frame(headerlist[filename], framelist[filename]);
                else
                    results[position] = new Frame(framelist[filename]);
            }

            /*int count = 0;
            foreach (string filename in framelist.Keys
                .OrderBy(key => int.Parse(Path.ChangeExtension(key, null))))
            {

            }*/

            if (clearListAfterFinished)
            {
                framelist.Clear();
                headerlist.Clear();
            }

            return results;
        }

        private Frame[] innerArray;
        private System.ComponentModel.BackgroundWorker bWorker;

        public UgoiraPlayer(IEnumerable<Frame> framelist) : this(framelist.ToArray()) { }
        public UgoiraPlayer(params Frame[] frameArray)
        {
            this._isplaying = false;
            this._disposed = false;
            this.innerArray = frameArray;

            this.bWorker = new System.ComponentModel.BackgroundWorker();
            this.bWorker.WorkerReportsProgress = false;
            this.bWorker.WorkerSupportsCancellation = true;
            this.bWorker.DoWork += this.BWorker_DoWork;
            this.bWorker.RunWorkerCompleted += this.BWorker_RunWorkerCompleted;

            this.currentPosition = 0;
            this.OnReady(EventArgs.Empty);
        }

        private bool _isplaying;
        public bool IsPlaying => this._isplaying;
        public SynchronizationContext SynchronizationContext { get; set; }

        #region "Events"
        public event EventHandler Ready;
        protected virtual void OnReady(EventArgs e)
        {
            if (this.SynchronizationContext != null)
                this.SynchronizationContext.Post(new SendOrPostCallback(delegate
                {
                    this.Ready?.Invoke(this, e);
                }), null);
            else
                this.Ready?.Invoke(this, e);
        }
        public event EventHandler<FrameDrawEventArgs> FrameDraw;
        protected virtual void OnFrameDraw(FrameDrawEventArgs e)
        {
            if (this.SynchronizationContext != null)
                this.SynchronizationContext.Post(new SendOrPostCallback(delegate 
                {
                    this.FrameDraw?.Invoke(this, e);
                }), null);
            else
                this.FrameDraw?.Invoke(this, e);
        }
        public event EventHandler Stopped;
        protected virtual void OnStopped(EventArgs e)
        {
            if (this.SynchronizationContext != null)
                this.SynchronizationContext.Post(new SendOrPostCallback(delegate
                {
                    this.Stopped?.Invoke(this, e);
                }), null);
            else
                this.Stopped?.Invoke(this, e);
        }
        public event EventHandler Started;
        protected virtual void OnStarted(EventArgs e)
        {
            if (this.SynchronizationContext != null)
                this.SynchronizationContext.Post(new SendOrPostCallback(delegate
                {
                    this.Started?.Invoke(this, e);
                }), null);
            else
                this.Started?.Invoke(this, e);
        }
        public event EventHandler Disposing;
        protected virtual void OnDisposing(EventArgs e)
        {
            if (this.SynchronizationContext != null)
                this.SynchronizationContext.Send(new SendOrPostCallback(delegate
                {
                    this.Disposing?.Invoke(this, e);
                }), null);
            else
                this.Disposing?.Invoke(this, e);
        }
        public event EventHandler Disposed;
        protected virtual void OnDisposed(EventArgs e)
        {
            if (this.SynchronizationContext != null)
                this.SynchronizationContext.Post(new SendOrPostCallback(delegate
                {
                    this.Disposed?.Invoke(this, e);
                }), null);
            else
                this.Disposed?.Invoke(this, e);
        }
        #endregion

        public void Begin()
        {
            if (!this.bWorker.IsBusy)
                this.bWorker.RunWorkerAsync();
        }

        private void BWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            this._isplaying = false;
            this.OnStopped(EventArgs.Empty);
        }

        private int currentPosition;
        private void BWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            this._isplaying = true;
            this.OnStarted(EventArgs.Empty);

            Frame currentFrame;

            while (!this.bWorker.CancellationPending)
            {

                currentFrame = this.innerArray[this.currentPosition];
                this.OnFrameDraw(new FrameDrawEventArgs(currentFrame.Image));

                Interlocked.Increment(ref this.currentPosition);
                if (this.currentPosition >= this.innerArray.Length)
                    this.currentPosition = 0;

                Thread.Sleep(currentFrame.Header.FrameDelay);

            }

            if (this.bWorker.CancellationPending)
                e.Cancel = true;
        }

        public void Stop()
        {
            if (this.bWorker.IsBusy)
                this.bWorker.CancelAsync();
        }

        private bool _disposed;
        public void Dispose()
        {
            if (this._disposed) return;
            this._disposed = true;
            this.OnDisposing(EventArgs.Empty);
            this.Stop();
            if (this.bWorker != null)
                this.bWorker.Dispose();



            this.OnDisposed(EventArgs.Empty);
        }

        private void DisposeFrames()
        {
            for (int i = 0; i < this.innerArray.Length; i++)
            {
                // this.innerArray[i].Image.
            }
        }
    }
}
