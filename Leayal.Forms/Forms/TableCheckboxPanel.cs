using Leayal.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms.VisualStyles;
using System.Windows.Forms;

namespace Leayal.Forms
{
    public class TableCheckboxPanel : TableLayoutPanel
    {
        private Dictionary<int, Dictionary<int, FakeCheckBox>> cellllll;
        private Dictionary<FakeCheckBox, TableLayoutPanelCellPosition> strChkBox;

        public bool HighlightText { get; set; }

        public TableCheckboxPanel() : base()
        {
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.UpdateStyles();
            this.strChkBox = new Dictionary<FakeCheckBox, TableLayoutPanelCellPosition>();
            this.cellllll = new Dictionary<int, Dictionary<int, FakeCheckBox>>();
            this.HighlightText = false;
        }

        public FakeCheckBox Add(int column, int row)
        {
            return this.Add(column, row, string.Empty);
        }

        public FakeCheckBox Add(int column, int row, string text)
        {
            string theName = "FakeCheckBox" + this.strChkBox.Count.ToString();
            if (string.IsNullOrEmpty(text))
                text = theName;
            return this.Add(column, row, text, theName);
        }

        public FakeCheckBox Add(int column, int row, string text, string name)
        {
            if (!this.cellllll.ContainsKey(column))
                this.cellllll.Add(column, new Dictionary<int, FakeCheckBox>());
            var something = this.cellllll[column];
            if (!something.ContainsKey(row))
            {
                FakeCheckBox fcb = new FakeCheckBox() { Text = text, Name = name };
                fcb.Invalidating += this.Fcb_CheckedChanged;
                something.Add(row, fcb);
                this.strChkBox.Add(fcb, new TableLayoutPanelCellPosition(column, row));
                this.RedrawCell(column, row);
                return fcb;
            }
            return null;
        }

        private void Fcb_CheckedChanged(object sender, EventArgs e)
        {
            FakeCheckBox fcb = sender as FakeCheckBox;
            if (this.strChkBox.ContainsKey(fcb))
                this.RedrawCell(this.strChkBox[fcb]);
        }

        protected void OnCellPaint(TableLayoutCellPaintEventArgs e, bool isChecked)
        {
            using (QuickBitmap qbm = new QuickBitmap(e.CellBounds.Size))
            {
                qbm.Graphics.Clear(Color.Transparent);
                Rectangle newBound = new Rectangle(Point.Empty, e.CellBounds.Size);
                //base.OnCellPaint(new TableLayoutCellPaintEventArgs(qbm.Graphics, e.ClipRectangle, e.CellBounds, e.Column, e.Row));
                if (backBuffer != null && !backBuffer.Disposed)
                {
                    qbm.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                    qbm.Graphics.DrawImage(backBuffer.Bitmap, newBound, e.CellBounds, GraphicsUnit.Pixel);
                    qbm.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                }
                qbm.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                if (this.cellllll.ContainsKey(e.Column) && this.cellllll[e.Column].ContainsKey(e.Row))
                {
                    FakeCheckBox fcb = this.GetCheckBox(e.Column, e.Row);
                    if (fcb != null && fcb.Visible)
                    {
                        Rectangle qbmRect;
                        if (this.CellBorderStyle == TableLayoutPanelCellBorderStyle.None)
                            qbmRect = new Rectangle(Point.Empty, e.CellBounds.Size);
                        else
                            qbmRect = new Rectangle(new Point(2, 2), e.CellBounds.Size);
                        if (isChecked)
                        {
                            if (fcb.Enabled)
                                this.DrawCheckBox(qbm.Graphics, newBound, fcb.Text, this.Font, CheckBoxState.CheckedNormal);
                            else
                                this.DrawCheckBox(qbm.Graphics, newBound, fcb.Text, this.Font, CheckBoxState.CheckedDisabled);
                        }
                        else
                        {
                            if (fcb.Enabled)
                                this.DrawCheckBox(qbm.Graphics, newBound, fcb.Text, this.Font, CheckBoxState.UncheckedNormal);
                            else
                                this.DrawCheckBox(qbm.Graphics, newBound, fcb.Text, this.Font, CheckBoxState.UncheckedDisabled);
                        }
                    }
                }
                e.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                e.Graphics.DrawImage(qbm.Bitmap, e.CellBounds, newBound, GraphicsUnit.Pixel);
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            if (backBuffer != null && !backBuffer.Disposed)
                backBuffer.Dispose();
            if (this.ClientSize.Width > 0 && this.ClientSize.Height > 0)
                backBuffer = new QuickBitmap(this.ClientSize);
            base.OnSizeChanged(e);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (this.backBuffer != null)
                    this.backBuffer.Dispose();
            }
        }

        QuickBitmap backBuffer;
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (backBuffer != null && !backBuffer.Disposed)
            {
                base.OnPaintBackground(new PaintEventArgs(backBuffer.Graphics, e.ClipRectangle));
                e.Graphics.DrawImage(backBuffer.Bitmap, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);
            }
            else
                base.OnPaintBackground(e);
        }

        protected override void OnCellPaint(TableLayoutCellPaintEventArgs e)
        {
            using (QuickBitmap qbm = new QuickBitmap(e.CellBounds.Size))
            {
                qbm.Graphics.Clear(Color.Transparent);
                Rectangle newBound = new Rectangle(Point.Empty, e.CellBounds.Size);
                //base.OnCellPaint(new TableLayoutCellPaintEventArgs(qbm.Graphics, e.ClipRectangle, e.CellBounds, e.Column, e.Row));
                if (backBuffer != null && !backBuffer.Disposed)
                {
                    qbm.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                    qbm.Graphics.DrawImage(backBuffer.Bitmap, newBound, e.CellBounds, GraphicsUnit.Pixel);
                    qbm.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                }
                qbm.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                if (this.cellllll.ContainsKey(e.Column) && this.cellllll[e.Column].ContainsKey(e.Row))
                {
                    FakeCheckBox fcb = this.GetCheckBox(e.Column, e.Row);
                    if (fcb != null && fcb.Visible)
                    {
                        if (fcb.Checked)
                        {
                            if (fcb.Enabled)
                            {
                                if (this.lastKnownCoordinate != null && this.lastKnownCoordinate.Value.Column == e.Column && this.lastKnownCoordinate.Value.Row == e.Row)
                                {
                                    if (this.isLeftMouseDown)
                                        this.DrawCheckBox(qbm.Graphics, newBound, fcb.Text, this.Font, CheckBoxState.CheckedPressed);
                                    else
                                        this.DrawCheckBox(qbm.Graphics, newBound, fcb.Text, this.Font, CheckBoxState.CheckedHot);
                                }
                                else
                                    this.DrawCheckBox(qbm.Graphics, newBound, fcb.Text, this.Font, CheckBoxState.CheckedNormal);
                            }
                            else
                                this.DrawCheckBox(qbm.Graphics, newBound, fcb.Text, this.Font, CheckBoxState.CheckedDisabled);
                        }
                        else
                        {
                            if (fcb.Enabled)
                            {
                                if (this.lastKnownCoordinate != null && this.lastKnownCoordinate.Value.Column == e.Column && this.lastKnownCoordinate.Value.Row == e.Row)
                                {
                                    if (this.isLeftMouseDown)
                                        this.DrawCheckBox(qbm.Graphics, newBound, fcb.Text, this.Font, CheckBoxState.UncheckedPressed);
                                    else
                                        this.DrawCheckBox(qbm.Graphics, newBound, fcb.Text, this.Font, CheckBoxState.UncheckedHot);
                                }
                                else
                                    this.DrawCheckBox(qbm.Graphics, newBound, fcb.Text, this.Font, CheckBoxState.UncheckedNormal);
                            }
                            else
                                this.DrawCheckBox(qbm.Graphics, newBound, fcb.Text, this.Font, CheckBoxState.UncheckedDisabled);
                        }
                    }
                }
                e.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                e.Graphics.DrawImage(qbm.Bitmap, e.CellBounds, newBound, GraphicsUnit.Pixel);
            }
        }

        private void DrawCheckBox(Graphics g, Rectangle bound, string text, Font font, CheckBoxState state)
        {
            Size precalculatedSize = TextRenderer.MeasureText(g, text, font, new Size(bound.Width - 12, bound.Height), TextFormatFlags.PreserveGraphicsClipping | TextFormatFlags.Default);
            Size TextSize = new Size(Math.Min(precalculatedSize.Width, bound.Width - 12), Math.Min(precalculatedSize.Height, bound.Height));
            Rectangle textRect = new Rectangle(new Point(bound.X + 12, bound.Y), TextSize);
            if (this.HighlightText)
            {
                using (Brush bru = new SolidBrush(Color.FromArgb(100, Color.White)))
                    g.FillRectangle(bru, textRect);
            }
            bool focused = false;
            switch (state)
            {
                case CheckBoxState.CheckedHot:
                    focused = true;
                    break;
                case CheckBoxState.CheckedPressed:
                    focused = true;
                    break;
                case CheckBoxState.UncheckedHot:
                    focused = true;
                    break;
                case CheckBoxState.UncheckedPressed:
                    focused = true;
                    break;
            }
            CheckBoxRenderer.DrawCheckBox(g, new Point(bound.Location.X, bound.Location.Y + 1), textRect, text, font, focused, state);
            //CheckBoxRenderer.DrawCheckBox(g, new Point(bound.Location.X, bound.Location.Y + 1), new Rectangle(bound.X + 11, bound.Y, bound.Width - 11, bound.Height), text, font, focused, state);
            //var theresult = TextRendererWrapper.WrapString(text, bound.Width - 11, this.Font, TextFormatFlags.Left);
            //TextRenderer.DrawText(g, theresult.Result, font, new Rectangle(bound.X + 11, bound.Y, theresult.Size.Width, theresult.Size.Height), this.ForeColor, this.BackColor, TextFormatFlags.PreserveGraphicsClipping | TextFormatFlags.Left);
        }

        private bool isLeftMouseDown;
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (!this.isLeftMouseDown)
                {
                    this.isLeftMouseDown = true;
                    this.RedrawCell(this.lastKnownCoordinate);
                }
            }
            base.OnMouseDown(e);            
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.isLeftMouseDown = false;
                if (this.lastKnownCoordinate != null)
                {
                    FakeCheckBox fcb = this.GetCheckBox(this.lastKnownCoordinate.Value.Column, this.lastKnownCoordinate.Value.Row);
                    if (fcb!= null && fcb.Enabled)
                        fcb.Checked = !fcb.Checked;
                }
            }
            base.OnMouseDown(e);
        }

        protected void RedrawCell(TableLayoutPanelCellPosition? _coordinate, bool isChecked)
        {
            if (_coordinate != null)
                this.RedrawCell(_coordinate.Value.Column, _coordinate.Value.Row, isChecked);
        }

        protected void RedrawCell(TableLayoutPanelCellPosition? _coordinate)
        {
            if (_coordinate != null)
                this.RedrawCell(_coordinate.Value.Column, _coordinate.Value.Row);
        }

        protected void RedrawCell(int column, int row, bool isChecked)
        {
            using (Graphics gr = this.CreateGraphics())
            {
                int[] cellWidths = this.GetColumnWidths();
                int _left = 0, _top = 0;
                for (int i = 0; i < column; i++)
                    _left += cellWidths[i];
                int[] cellHeights = this.GetRowHeights();
                for (int i = 0; i < row; i++)
                    _top += cellHeights[i];
                int cellwidth = cellWidths[column];
                int cellheight = cellHeights[row];
                Rectangle newRect = new Rectangle(_left, _top, cellwidth, cellheight);
                TableLayoutCellPaintEventArgs theevent = new TableLayoutCellPaintEventArgs(gr, newRect, newRect, column, row);
                this.OnCellPaint(theevent, isChecked);
                theevent.Dispose();
            }
        }

        /// <summary>
        /// Replace usage of Invalidate because we need only the cell paint, not redraw the whole control.
        /// </summary>
        /// <param name="column"></param>
        /// <param name="row"></param>
        protected void RedrawCell(int column, int row)
        {
            using (Graphics gr = this.CreateGraphics())
            {
                int[] cellWidths = this.GetColumnWidths();
                int _left = 0, _top = 0;
                for (int i = 0; i < column; i++)
                    _left += cellWidths[i];
                int[] cellHeights = this.GetRowHeights();
                for (int i = 0; i < row; i++)
                    _top += cellHeights[i];
                int cellwidth = cellWidths[column];
                int cellheight = cellHeights[row];
                Rectangle newRect = new Rectangle(_left, _top, cellwidth, cellheight);
                TableLayoutCellPaintEventArgs theevent = new TableLayoutCellPaintEventArgs(gr, newRect, newRect, column, row);
                this.OnCellPaint(theevent);
                theevent.Dispose();
            }
        }

        public FakeCheckBox GetCheckBox(int column, int row)
        {
            FakeCheckBox result = null;
            if (this.cellllll.ContainsKey(column) && this.cellllll[column].ContainsKey(row))
                result = this.cellllll[column][row];
            return result;
        }

        public event EventHandler<CellPointedEventArgs> CellEnter;
        protected virtual void OnCellEnter(CellPointedEventArgs e)
        {
            this.CellEnter?.Invoke(this, e);
            this.RedrawCell(e.Coordinate);
        }

        public event EventHandler<CellPointedEventArgs> CellLeave;
        protected virtual void OnCellLeave(CellPointedEventArgs e)
        {
            this.CellLeave?.Invoke(this, e);
            FakeCheckBox fcb = GetCheckBox(e.Coordinate.Value.Column, e.Coordinate.Value.Row);
            if (fcb != null)
            {
                this.RedrawCell(e.Coordinate, fcb.Checked);
            }
        }

        private TableLayoutPanelCellPosition? lastKnownCoordinate;
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            TableLayoutPanelCellPosition? currentPointing = this.GetRowColIndex(e.Location);
            if (currentPointing != this.lastKnownCoordinate)
            {
                if (this.lastKnownCoordinate.HasValue)
                    this.OnCellLeave(new CellPointedEventArgs(this.lastKnownCoordinate));
                this.lastKnownCoordinate = currentPointing;
                if (currentPointing.HasValue)
                    this.OnCellEnter(new CellPointedEventArgs(this.lastKnownCoordinate));
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (this.lastKnownCoordinate != null)
            {
                this.OnCellLeave(new CellPointedEventArgs(this.lastKnownCoordinate));
                this.lastKnownCoordinate = null;
            }
        }

        private TableLayoutPanelCellPosition? GetRowColIndex(Point point)
        {
            if (point.X > this.Width || point.Y > this.Height)
                return null;

            int w = this.Width;
            int h = this.Height;
            int[] widths = this.GetColumnWidths();

            int i;
            for (i = widths.Length - 1; i >= 0 && point.X < w; i--)
                w -= widths[i];
            int col = i + 1;

            int[] heights = this.GetRowHeights();
            for (i = heights.Length - 1; i >= 0 && point.Y < h; i--)
                h -= heights[i];

            int row = i + 1;

            return new TableLayoutPanelCellPosition(col, row);
        }
    }

    public class CellPointedEventArgs : EventArgs
    {
        public TableLayoutPanelCellPosition? Coordinate { get; }
        public CellPointedEventArgs(TableLayoutPanelCellPosition? _coordinate) : base()
        {
            this.Coordinate = _coordinate;
        }
    }

    public class FakeCheckBox : FakeControl
    {
        public FakeCheckBox() : base() { this._checked = false; }

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
