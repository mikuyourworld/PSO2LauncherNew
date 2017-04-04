using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Leayal.Forms
{
    public static class SystemEvents
    {
        //Make a dummy variable
        private static readonly InnerSystemEvents Instance = new InnerSystemEvents();
        internal sealed class InnerSystemEvents
        {
            private float scaleFactor;
            [DllImport("gdi32.dll")]
            static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
            internal enum DeviceCap : int
            {
                VERTRES = 10,
                DESKTOPVERTRES = 117,
                LOGPIXELSY = 90,
            }

            public static float GetResolutionScale()
            {
                try
                {
                    System.Drawing.Graphics g = System.Drawing.Graphics.FromHwnd(IntPtr.Zero);
                    IntPtr desktop = g.GetHdc();
                    int LogicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.VERTRES);
                    int PhysicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.DESKTOPVERTRES);
                    int logpixelsy = GetDeviceCaps(desktop, (int)DeviceCap.LOGPIXELSY);
                    float screenScalingFactor = (float)PhysicalScreenHeight / (float)LogicalScreenHeight;
                    float dpiScalingFactor = (float)logpixelsy / (float)96;

                    g.ReleaseHdc();
                    g.Dispose();

                    if (dpiScalingFactor > 1)
                        FormWrapper._ScalingFactor = dpiScalingFactor;
                    else if (screenScalingFactor > 1)
                        FormWrapper._ScalingFactor = screenScalingFactor;
                    else
                        FormWrapper._ScalingFactor = 1F;
                }
                catch { FormWrapper._ScalingFactor = 1F; }
                return FormWrapper._ScalingFactor;
            }

            internal InnerSystemEvents()
            {
                Application.ApplicationExit += Application_ApplicationExit;
                this.scaleFactor = GetResolutionScale();
                Microsoft.Win32.SystemEvents.UserPreferenceChanged += this.SystemEvents_UserPreferenceChanged;
            }

            private void SystemEvents_UserPreferenceChanged(object sender, Microsoft.Win32.UserPreferenceChangedEventArgs e)
            {
                float newf = GetResolutionScale();
                if (this.scaleFactor != newf)
                {
                    this.scaleFactor = newf;
                    ScalingFactorChanged?.Invoke(sender, System.EventArgs.Empty);
                }
            }

            private void Application_ApplicationExit(object sender, EventArgs e)
            {
                Microsoft.Win32.SystemEvents.UserPreferenceChanged -= this.SystemEvents_UserPreferenceChanged;
            }
        }

        public static event EventHandler ScalingFactorChanged;
    }
}
