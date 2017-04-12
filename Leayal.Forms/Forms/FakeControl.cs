using System;
using System.Windows.Forms;
using System.Drawing;

namespace Leayal.Forms
{
    public abstract class FakeControl
    {
        public AnchorStyles Anchor { get; set; }

        private Font _font;
        public Font Font
        {
            get { return this._font; }
            set
            {
                if (this._font != value)
                {
                    if (this._font != null)
                    {
                        Font dunnuFont = this._font;
                        dunnuFont.Dispose();
                    }
                    this._font = value;
                    this.OnFontChanged(EventArgs.Empty);
                }
            }
        }

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

        private object _parent;
        public object Parent
        {
            get { return this._parent; }
            set
            {
                if (this._parent == null)
                {
                    if (value != null)
                    {
                        this._parent = value;
                        this.OnParentChanged(EventArgs.Empty);
                    }
                }
                else
                {
                    if (this._parent.Equals(value))
                    {
                        this._parent = value;
                        this.OnParentChanged(EventArgs.Empty);
                    }
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

        private Color _foreColor;
        public Color ForeColor
        {
            get { return this._foreColor; }
            set
            {
                if (this._foreColor != value)
                {
                    this._foreColor = value;
                    this.OnEnabledChanged(EventArgs.Empty);
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
            this._font = Control.DefaultFont;
            this._visible = true;
            this._enabled = true;
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
        public event EventHandler ForeColorChanged;
        protected virtual void OnForeColorChanged(EventArgs e)
        {
            this.OnInvalidating(e);
            this.ForeColorChanged?.Invoke(this, e);
        }
        public event EventHandler FontChanged;
        protected virtual void OnFontChanged(EventArgs e)
        {
            this.OnInvalidating(e);
            this.FontChanged?.Invoke(this, e);
        }
        public event EventHandler ParentChanged;
        protected virtual void OnParentChanged(EventArgs e)
        {
            //this.OnInvalidating(e);
            this.ParentChanged?.Invoke(this, e);
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
