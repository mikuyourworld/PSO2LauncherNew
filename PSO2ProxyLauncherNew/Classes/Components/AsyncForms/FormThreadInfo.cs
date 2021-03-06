﻿using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;

namespace PSO2ProxyLauncherNew.Classes.Components.AsyncForms
{
    internal class FormThreadInfo : IDisposable
    {
        private Type _formType;
        public Forms.AnotherExtendedForm Form { get; private set; }
        private bool _isvisible;
        public bool IsVisible{ get { return this._isvisible; } }
#pragma warning disable 0649
        private Exception _lastKnownError;
#pragma warning restore 0649
        public Exception LastKnownError { get { return this._lastKnownError; } }
        private bool _disposed;
        public bool Disposed { get { return this._disposed; } }
        
        public FormThreadInfo(Forms.AnotherExtendedForm _form)
        {
            this._disposed = false;
            this.Form = null;
            this._formType = _form.GetType();
            _form.Dispose();
            this._lastKnownError = null;
        }

        public FormThreadInfo(System.Type _formT)
        {
            this._disposed = false;
            this.Form = null;
            this._formType = _formT;
            this._lastKnownError = null;
        }

#if DEBUG
        private void Worker_DoWork(object obj)
        {
            if (this.Form == null || this.Form.IsClosed)
                this.Form = Activator.CreateInstance(obj as Type, true) as Forms.AnotherExtendedForm;
            if (this.Form != null && !this.Form.IsClosed)
            {
                this.Form.Load += Form_Load;
                this.Form.FormClosed += Form_FormClosed;
                var geeeh = new ApplicationContext(this.Form);
                geeeh.ThreadExit += Geeeh_ThreadExit;
                Application.Run(geeeh);
            }
        }
#else
        private void Worker_DoWork(object obj)
        {
            try
            {
                if (this.Form == null || this.Form.IsClosed)
                    this.Form = Activator.CreateInstance(obj as Type, true) as Forms.AnotherExtendedForm;
                if (this.Form != null && !this.Form.IsClosed)
                {
                    this.Form.Load += Form_Load;
                    this.Form.FormClosed += Form_FormClosed;
                    var geeeh = new ApplicationContext(this.Form);
                    geeeh.ThreadExit += Geeeh_ThreadExit;
                    Application.Run(geeeh);
                }
            }
            catch (Exception ex)
            {
                this._lastKnownError = ex;
            }
        }
#endif
        private void Geeeh_ThreadExit(object sender, EventArgs e)
        {
            ApplicationContext aaa = sender as ApplicationContext;
            if (aaa != null)
                aaa.Dispose();
        }

        private void Form_Load(object sender, EventArgs e)
        {
            WebClientManger.WebClientPool.SynchronizationContext?.Post(new SendOrPostCallback(delegate {
                this.FormLoaded?.Invoke(sender, new Events.FormLoadedEventArgs(this.Form.SyncContext));
            }), null);
        }

        public event EventHandler<Events.FormLoadedEventArgs> FormLoaded;

        private void Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            this._isvisible = false;
        }

        public void Show()
        {
            if (this._isvisible) return;
            this.ShowEx();
        }

        private void ShowEx()
        {
            this._isvisible = true;
            Thread _thread = new Thread(new ParameterizedThreadStart(this.Worker_DoWork));
            _thread.SetApartmentState(ApartmentState.STA);
            _thread.Start(this._formType);
        }

        public void Dispose()
        {
            if (this._disposed) return;
            this._disposed = true;
            if (this.Form != null && this.Form.SyncContext != null)
            {
                try
                {
                    this.Form.SyncContext.Send(new SendOrPostCallback(delegate
                    {
                        if (!this.Form.IsClosed)
                            this.Form.Close();
                        this.Form.Dispose();
                    }), null);
                } catch (System.ComponentModel.InvalidAsynchronousStateException) { }
            }
        }
    }
}
