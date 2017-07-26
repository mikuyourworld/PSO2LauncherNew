using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PSO2ProxyLauncherNew.WPF.Controls
{
    /// <summary>
    /// Interaction logic for GameStartButton.xaml
    /// </summary>
    public partial class GameStartButton
    {
        public GameStartButton()
        {
            InitializeComponent();
            this.SizeChanged += this.GameStartButton_SizeChanged;
            this.Loaded += this.GameStartButton_Loaded;

            this.Text = "START";
        }

        private void GameStartButton_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void GameStartButton_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width != e.NewSize.Height)
            {
                if (e.NewSize.Width > e.NewSize.Height)
                    this.Height = this.Width;
                else if (e.NewSize.Height > e.NewSize.Width)
                    this.Width = this.Height;
            }
        }
        
        public string Text
        {
            get => this._text.Content as string;
            set
            {
                if (value == null)
                    value = string.Empty;
                if (((string)this._text.Content) != value)
                    this._text.Content = value;
            }
        }

        /// <summary>
        /// Draw an Arc of an ellipse or circle. Static extension method of DrawingContext.
        /// </summary>
        /// <param name="dc">DrawingContext</param>
        /// <param name="pen">Pen for outline. set to null for no outline.</param>
        /// <param name="brush">Brush for fill. set to null for no fill.</param>
        /// <param name="rect">Box to hold the whole ellipse described by the arc</param>
        /// <param name="startDegrees">Start angle of the arc degrees within the ellipse. 0 degrees is a line to the right.</param>
        /// <param name="sweepDegrees">Sweep angle, -ve = Counterclockwise, +ve = Clockwise</param>
        private void DrawArc(DrawingContext dc, Pen pen, Brush brush, Rect rect, double startDegrees, double sweepDegrees)
        {
            // degrees to radians conversion
            double startRadians = startDegrees * Math.PI / 180.0;
            double sweepRadians = sweepDegrees * Math.PI / 180.0;

            // x and y radius
            double dx = rect.Width / 2;
            double dy = rect.Height / 2;

            // determine the start point 
            double xs = rect.X + dx + (Math.Cos(startRadians) * dx);
            double ys = rect.Y + dy + (Math.Sin(startRadians) * dy);

            // determine the end point 
            double xe = rect.X + dx + (Math.Cos(startRadians + sweepRadians) * dx);
            double ye = rect.Y + dy + (Math.Sin(startRadians + sweepRadians) * dy);

            // draw the arc into a stream geometry
            StreamGeometry streamGeom = new StreamGeometry();
            using (StreamGeometryContext ctx = streamGeom.Open())
            {
                bool isLargeArc = Math.Abs(sweepDegrees) > 180;
                SweepDirection sweepDirection = sweepDegrees < 0 ? SweepDirection.Counterclockwise : SweepDirection.Clockwise;

                ctx.BeginFigure(new Point(xs, ys), false, false);
                ctx.ArcTo(new Point(xe, ye), new Size(dx, dy), 0, isLargeArc, sweepDirection, true, false);
            }

            // create the drawing
            GeometryDrawing drawing = new GeometryDrawing();
            drawing.Geometry = streamGeom;

            dc.DrawGeometry(brush, pen, drawing.Geometry);
        }
    }
}
