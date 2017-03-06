using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Timers;
using System.Windows.Forms;

namespace PSO2ProxyLauncherNew.Classes.Controls
{
    class ProgressRing : Control
    {

        // *********************************** constants and variables

        const int CONTROL_HEIGHT = 100;           // pixels
        const int CONTROL_WIDTH = CONTROL_HEIGHT; // pixels
        static Color INDICATOR_COLOR = Color.White;  // Color
        const double INDICATOR_OFFSET = 11.25;       // deg
        const int MAXIMUM_INDICATORS = 6;         // units
        const int SIZE_FACTOR = 20;               // pixels
        const double START_AT = 120.0;                // deg
        const double TIMER_INTERVAL = 100.0;         // ms

        // ***********************************************************

        GraphicsBuffer background = null;
        int control_height = CONTROL_HEIGHT;
        int control_width = CONTROL_WIDTH;
        GraphicsBuffer indicator = null;
        int indicator_center_radius = 0;
        Color indicator_color = INDICATOR_COLOR;
        int indicator_diameter = 0;
        int indicator_radius = 0;
        Indicator[] indicators = new Indicator[
                                                MAXIMUM_INDICATORS];
        int inner_radius = 0;
        int outer_radius = 0;
        Point P0 = Point.Empty;
        int refresh_rate = (int)TIMER_INTERVAL;
        bool revise_background_graphic = true;
        double start_at = START_AT;
        System.Timers.Timer timer = null;
        double timer_interval = TIMER_INTERVAL;
        // trigonometry tables
        double[] cosine = new double[1440];
        double[] sine = new double[1440];

        // ************************************************* deg_2_rad

        double deg_2_rad(double degrees)
        {

            return (Math.PI * (degrees / 180.0));
        }


        // **************************** initialize_trigonometry_tables

        void initialize_trigonometry_tables()
        {

            for (int i = 0; (i < sine.Length); i++)
            {
                double degrees = 0.25 * (double)i;
                double radians = deg_2_rad(degrees);

                sine[i] = Math.Sin(radians);
                cosine[i] = Math.Cos(radians);
            }
        }

        // ******************************************************* cos

        double cos(double degrees)
        {
            int index = round(degrees / 0.25);

            return (cosine[index]);
        }

        // ******************************************************* sin

        double sin(double degrees)
        {
            int index = round(degrees / 0.25);

            return (sine[index]);
        }

        // ************************************* initialize_indicators

        private void initialize_indicators()
        {
            double degrees = start_at;

            for (int i = 0; (i < MAXIMUM_INDICATORS); i++)
            {
                if (degrees < 0.0)
                {
                    degrees += 360.0;
                }

                indicators[i] = new Indicator(degrees);

                degrees -= INDICATOR_OFFSET;
            }
        }

        // ******************************************** memory_cleanup

        void memory_cleanup()
        {
            // DeleteGraphicsBuffer 
            // returns null
            if (background != null)
            {
                background = background.DeleteGraphicsBuffer();
            }

            if (indicator != null)
            {
                indicator = indicator.DeleteGraphicsBuffer();
            }
        }

        // ***************************************************** round

        // http://en.wikipedia.org/wiki/Rounding

        int round(double control_value)
        {

            return ((int)(control_value + 0.5));
        }

        // ********************************* create_background_graphic

        void create_background_graphic()
        {

            if (background != null)
            {
                background = background.DeleteGraphicsBuffer();
            }
            background = new GraphicsBuffer();
            background.CreateGraphicsBuffer(control_width,
                                              control_height);
            background.Graphic.SmoothingMode = SmoothingMode.
                                               HighQuality;
        }

        // ********************************** create_indicator_graphic

        void create_indicator_graphic()
        {

            if (indicator != null)
            {
                indicator = indicator.DeleteGraphicsBuffer();
            }
            indicator = new GraphicsBuffer();
            indicator.CreateGraphicsBuffer(control_width,
                                             control_height);
            indicator.Graphic.SmoothingMode = SmoothingMode.
                                              HighQuality;
        }

        // ********************* adjust_control_dimensions_from_height

        private void adjust_control_dimensions_from_height(int new_height)
        {

            indicator_radius = round((double)new_height /
                                       (double)SIZE_FACTOR);
            indicator_diameter = 2 * indicator_radius;

            control_height = SIZE_FACTOR * indicator_radius;
            control_width = control_height;

            outer_radius = control_height / 2;
            inner_radius = outer_radius - indicator_diameter;

            indicator_center_radius = inner_radius +
                                      indicator_radius;

            this.Height = control_height;
            this.Width = control_width;
        }

        // ***************************************** Win8_ProgressRing

        public ProgressRing()
        {

            initialize_trigonometry_tables();
            adjust_control_dimensions_from_height(CONTROL_HEIGHT);
            initialize_indicators();

            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.UpdateStyles();

            timer = new System.Timers.Timer();
            timer.Elapsed += new ElapsedEventHandler(tick);
            timer.Interval = TIMER_INTERVAL;
            timer.Enabled = true;
        }

        // *************************************************** Animate

        [Category("Appearance"),
         Description("Start/stops indicator animation"),
         DefaultValue(true),
         Bindable(true)]
        public bool Animate
        {

            get
            {
                return (timer.Enabled);
            }

            set
            {
                timer.Enabled = value;
            }
        }

        // ******************************************** Control_Height

        [Category("Appearance"),
         Description("Gets/Sets control height"),
         DefaultValue(100),
         Bindable(true)]
        public int Control_Height
        {

            get
            {
                return (control_height);
            }
            set
            {
                int old_control_height = control_height;

                control_height = value;
                if (old_control_height != control_height)
                {
                    adjust_control_dimensions_from_height(value);
                    revise_background_graphic = true;
                    this.Invalidate();
                }
            }
        }

        // ******************************************* Indicator_Color

        [Category("Appearance"),
         Description("Gets/Sets color of the rotating indicators"),
         DefaultValue(typeof(Color), "White"),
         Bindable(true)]
        public Color Indicator_Color
        {

            get
            {
                return (indicator_color);
            }
            set
            {
                Color old_indicator_color = indicator_color;

                indicator_color = value;
                if (old_indicator_color != indicator_color)
                {
                    revise_background_graphic = true;
                    this.Invalidate();
                }
            }
        }

        // ***************************************************** Pulse

        public void Pulse()
        {

            this.Invalidate();
        }

        // ********************************************** Refresh_Rate

        [Category("Appearance"),
         Description("Specifies indicator rotational refresh"),
         DefaultValue(200),
         Bindable(true)]
        public int Refresh_Rate
        {

            get
            {
                return (refresh_rate);
            }

            set
            {
                int old_refresh_rate = refresh_rate;
                bool timer_running = timer.Enabled;

                refresh_rate = value;
                if (refresh_rate < 10)
                {
                    refresh_rate = 10;
                }
                if (refresh_rate > 200)
                {
                    refresh_rate = 200;
                }

                if (timer_running)
                {
                    timer.Stop();
                }

                timer_interval = (double)refresh_rate;
                timer.Interval = timer_interval;

                if (timer_running)
                {
                    timer.Start();
                }

                if (old_refresh_rate != refresh_rate)
                {
                    revise_background_graphic = true;
                    this.Invalidate();
                }
            }
        }

        // *********************************** draw_background_graphic

        void draw_background_graphic(Graphics graphics)
        {
            // currently nothing to do
        }

        // ************************************ draw_indicator_graphic

        void draw_indicator_graphic(Graphics graphics)
        {
            Brush brush = new SolidBrush(Indicator_Color);
            Rectangle rectangle = new Rectangle();

            for (int i = (MAXIMUM_INDICATORS - 1);
                      (i >= 0);
                      i--)
            {
                double degrees = indicators[i].Degrees;
                int dx;
                int dy;

                if (degrees < 0.0)
                {
                    degrees += 360.0;
                }

                dx = round((double)indicator_center_radius *
                             cos(degrees)) +
                     indicator_center_radius;
                dy = indicator_center_radius -
                     round((double)indicator_center_radius *
                             sin(degrees));

                rectangle.Location = new Point(dx, dy);
                rectangle.Size = new Size(indicator_diameter,
                                            indicator_diameter);
                graphics.FillEllipse(brush, rectangle);

                degrees -= (double)indicators[i].Fast *
                           INDICATOR_OFFSET;

                if (indicators[i].Fast > 1.0)
                {
                    indicators[i].Fast += 0.25;
                }

                if (degrees < 0.0)
                {
                    indicators[i].Fast = 1.25;
                }
                else if (degrees < START_AT)
                {
                    indicators[i].Fast = 1.0;
                }

                indicators[i].Degrees = degrees;
            }

            brush.Dispose();
        }

        // ****************************************************** tick

        void tick(object source,
                    ElapsedEventArgs e)
        {

            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(
                        new EventHandler(
                            delegate
                            {
                                this.Refresh();
                            }
                            )
                        );
                }
                else
                {
                    this.Refresh();
                }
            }

            catch (Exception ex)
            {

            }
        }

        // ****************************************** OnControlRemoved

        protected override void OnControlRemoved(
                                                ControlEventArgs e)
        {

            base.OnControlRemoved(e);

            memory_cleanup();
        }

        // ************************************************** OnResize

        protected override void OnResize(EventArgs e)
        {

            base.OnResize(e);

            adjust_control_dimensions_from_height(this.Height);

            revise_background_graphic = true;

            this.Refresh();
        }

        // *************************************************** OnPaint

        // see community additions in
        // http://msdn.microsoft.com/en-us/library/wk5b13s4(v=vs.90).aspx
        // regarding transparent background

        protected override void OnPaint(PaintEventArgs e)
        {

            {                       // begin transparent background
                Rectangle clip;
                PaintEventArgs pea;
                GraphicsContainer state;

                state = e.Graphics.BeginContainer();
                e.Graphics.TranslateTransform(-this.Left,
                                                -this.Top);
                clip = e.ClipRectangle;
                clip.Offset(this.Left, this.Top);
                pea = new PaintEventArgs(e.Graphics, clip);
                // paint the container's 
                // background
                InvokePaintBackground(this.Parent, pea);
                // paint the container's 
                // foreground
                InvokePaint(this.Parent, pea);
                // restores graphics to 
                // original state
                e.Graphics.EndContainer(state);
            }                       // end transparent background

            if ((background == null) || revise_background_graphic)
            {
                if (revise_background_graphic)
                {
                    revise_background_graphic = false;
                }
                create_background_graphic();
                draw_background_graphic(background.Graphic);
            }
            background.RenderGraphicsBuffer(e.Graphics);

            create_indicator_graphic();
            draw_indicator_graphic(indicator.Graphic);

            indicator.RenderGraphicsBuffer(e.Graphics);
        }

    } // class Win8_ProgressRing

    // *********************************************** class Indicator

    public class Indicator
    {
        double degrees = 0.0;
        double fast = 1.0;

        // ************************************************* Indicator

        public Indicator()
        {

            Degrees = 0.0;
            Fast = 1.0;
        }

        // ************************************************* Indicator

        public Indicator(double degrees)
        {

            Degrees = degrees;
            Fast = 1.0;
        }

        // *************************************************** Degrees

        public double Degrees
        {

            get
            {
                return (degrees);
            }
            set
            {
                degrees = value;
            }
        }

        // ****************************************************** Fast

        public double Fast
        {

            get
            {
                return (fast);
            }
            set
            {
                fast = value;
            }
        }

    } // class Indicator

    // ****************************************** class GraphicsBuffer

    public class GraphicsBuffer
    {
        Bitmap bitmap;
        Graphics graphics;
        int height;
        int width;

        // ******************************************** GraphicsBuffer

        /// <summary>
        /// constructor for the GraphicsBuffer
        /// </summary>
        public GraphicsBuffer()
        {

            width = 0;
            height = 0;
        }

        // ************************************** CreateGraphicsBuffer

        /// <summary>
        /// completes the creation of the GraphicsBuffer object
        /// </summary>
        /// <param name="width">
        /// width of the bitmap
        /// </param>
        /// <param name="height">
        /// height of the bitmap
        /// </param>
        /// <returns>
        /// true, if GraphicsBuffer created; otherwise, false
        /// </returns>
        public bool CreateGraphicsBuffer(int width,
                                           int height)
        {
            bool success = true;

            DeleteGraphicsBuffer();

            this.width = 0;
            this.height = 0;

            if ((width == 0) || (height == 0))
            {
                success = false;
            }
            else
            {
                this.width = width;
                this.height = height;

                bitmap = new Bitmap(this.width, this.height);
                graphics = Graphics.FromImage(bitmap);

                success = true;
            }

            return (success);
        }

        // ************************************** DeleteGraphicsBuffer

        /// <summary>
        /// deletes the current GraphicsBuffer
        /// </summary>
        /// <returns>
        /// null, always
        /// </returns>
        public GraphicsBuffer DeleteGraphicsBuffer()
        {

            if (graphics != null)
            {
                graphics.Dispose();
                graphics = null;
            }

            if (bitmap != null)
            {
                bitmap.Dispose();
                bitmap = null;
            }

            width = 0;
            height = 0;

            return (null);
        }

        // *************************************************** Graphic

        /// <summary>
        /// returns the current Graphic Graphics object
        /// </summary>
        public Graphics Graphic
        {

            get
            {
                return (graphics);
            }
        }

        // ************************************** RenderGraphicsBuffer

        /// <summary>
        /// Renders the buffer to the graphics object identified by 
        /// graphic
        /// </summary>
        /// <param name="graphic">
        /// target graphics object (e.g., PaintEventArgs e.Graphics)
        /// </param>
        public void RenderGraphicsBuffer(Graphics graphic)
        {

            if (bitmap != null)
            {
                graphic.DrawImage(
                            bitmap,
                            new Rectangle(0, 0, width, height),
                            new Rectangle(0, 0, width, height),
                            GraphicsUnit.Pixel);
            }
        }

    } // class GraphicsBuffer
}
