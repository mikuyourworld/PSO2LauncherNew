using System;
using System.Windows.Forms;

namespace Leayal.Forms
{
    public abstract class FakeControl
    {
        public AnchorStyles Anchor { get; set; }

        private string _text;
        public string Text
        {
            get { return this._text; }
            set
            {
                if (this._text != value)
                {
                    this._text = value;
                    this.OnTextChanged(EventArgs.Empty);
                }
            }
        }

        public string Name { get; set; }

        private bool _visible;
        public bool Visible
        {
            get { return this._visible; }
            set
            {
                if (this._visible != value)
                {
                    this._visible = value;
                    this.OnVisibleChanged(EventArgs.Empty);
                }
            }
        }

        private bool _enabled;
        public bool Enabled
        {
            get { return this._enabled; }
            set
            {
                if (this._enabled != value)
                {
                    this._enabled = value;
                    this.OnEnabledChanged(EventArgs.Empty);
                }
            }
        }

        public object Tag { get; set; }

        internal FakeControl()
        {
            this.Visible = true;
            this.Enabled = true;
        }

        public event EventHandler VisibleChanged;
        protected virtual void OnVisibleChanged(EventArgs e)
        {
            this.OnInvalidating(e);
            this.VisibleChanged?.Invoke(this, e);
        }
        public event EventHandler EnabledChanged;
        protected virtual void OnEnabledChanged(EventArgs e)
        {
            this.OnInvalidating(e);
            this.EnabledChanged?.Invoke(this, e);
        }
        public event EventHandler TextChanged;
        protected virtual void OnTextChanged(EventArgs e)
        {
            this.OnInvalidating(e);
            this.TextChanged?.Invoke(this, e);
        }
        public event EventHandler AnchorChanged;
        protected virtual void OnAnchorChanged(EventArgs e)
        {
            this.OnInvalidating(e);
            this.AnchorChanged?.Invoke(this, e);
        }

        internal event EventHandler Invalidating;
        protected virtual void OnInvalidating(EventArgs e)
        {
            this.Invalidating?.Invoke(this, e);
        }
    }
}
