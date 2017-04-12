using System;
using System.ComponentModel;

namespace Leayal.Forms
{
    public class FakeCheckBox : FakeControl, IFakeControlHighLightText
    {
        internal FakeCheckBox() : base() { this._checked = false; }
        
        private bool _highlightText;
        [DefaultValue(false)]
        public bool HighlightText
        {
            get { return this._highlightText; }
            set
            {
                if (this._highlightText != value)
                {
                    this._highlightText = value;
                    this.OnHighlightTextChanged(EventArgs.Empty);
                }
            }
        }

        public event EventHandler HighlightTextChanged;
        protected virtual void OnHighlightTextChanged(EventArgs e)
        {
            this.OnInvalidating(e);
            this.HighlightTextChanged?.Invoke(this, e);
        }

        private bool _checked;
        public bool Checked
        {
            get { return this._checked; }
            set
            {
                if (this._checked != value)
                {
                    this._checked = value;
                    this.OnCheckedChanged(EventArgs.Empty);
                }
            }
        }

        public event EventHandler CheckedChanged;
        protected virtual void OnCheckedChanged(EventArgs e)
        {
            this.OnInvalidating(e);
            this.CheckedChanged?.Invoke(this, e);
        }
    }
}
