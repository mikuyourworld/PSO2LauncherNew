using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace PSO2ProxyLauncherNew
{
    public static class AeroControl
    {
        #region "Windows 10"
        internal enum AccentState
        {
            ACCENT_DISABLED = 0,
            ACCENT_ENABLE_GRADIENT = 1,
            ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
            ACCENT_ENABLE_BLURBEHIND = 3,
            ACCENT_INVALID_STATE = 4
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct AccentPolicy
        {
            public AccentState AccentState;
            public int AccentFlags;
            public int GradientColor;
            public int AnimationId;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct WindowCompositionAttributeData
        {
            public WindowCompositionAttribute Attribute;
            public IntPtr Data;
            public int SizeOfData;
        }

        internal enum WindowCompositionAttribute
        {
            // ...
            WCA_ACCENT_POLICY = 19
            // ...
        }

        /// <summary>
        /// Interaction logic for MainWindow.xaml
        /// </summary>
        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);
        #endregion

        #region "Windows 7"
        [DllImport("gdi32")]
        private static extern IntPtr CreateEllipticRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);
        [DllImport("dwmapi")]
        private static extern int DwmEnableBlurBehindWindow(IntPtr hWnd, ref DwmBlurbehind pBlurBehind);
        public struct DwmBlurbehind
        {
            public int DwFlags;
            public bool FEnable;
            public IntPtr HRgnBlur;
            public bool FTransitionOnMaximized;
        }
        private static void TargetForm_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            System.Windows.Forms.Form target = sender as System.Windows.Forms.Form;
            e.Graphics.FillRectangle(new SolidBrush(target.BackColor), new Rectangle(0, 0, target.Width, target.Height));
        }
        #endregion

        public static void EnableBlur(System.Windows.Forms.Form targetForm, bool keepBG = false)
        {
            string OSName = Classes.Infos.OSVersionInfo.Name;
            if (OSName == "Windows 10")
            {
                var accent = new AccentPolicy();
                accent.AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND;

                var accentStructSize = Marshal.SizeOf(accent);

                var accentPtr = Marshal.AllocHGlobal(accentStructSize);
                Marshal.StructureToPtr(accent, accentPtr, false);

                var data = new WindowCompositionAttributeData();
                data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
                data.SizeOfData = accentStructSize;
                data.Data = accentPtr;

                SetWindowCompositionAttribute(targetForm.Handle, ref data);

                Marshal.FreeHGlobal(accentPtr);
                if (!keepBG)
                {
                    targetForm.BackColor = Color.Lavender;
                    targetForm.TransparencyKey = Color.Lavender;
                }
                //targetForm.Paint += TargetForm_Paint;
            }
            else if (OSName == "Windows 7")
            {
                var hr = CreateEllipticRgn(0, 0, targetForm.Width, targetForm.Height);
                var dbb = new DwmBlurbehind { FEnable = true, DwFlags = 1, HRgnBlur = hr, FTransitionOnMaximized = false };
                DwmEnableBlurBehindWindow(targetForm.Handle, ref dbb);
                if (!keepBG)
                    targetForm.BackColor = Color.Black;
                targetForm.Paint += TargetForm_Paint;
            }
        }

        public static void EnableBlur(System.Windows.Forms.Control targetForm, bool keepBG = false)
        {
            string OSName = Classes.Infos.OSVersionInfo.Name;
            if (OSName == "Windows 10")
            {
                var accent = new AccentPolicy();
                accent.AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND;

                var accentStructSize = Marshal.SizeOf(accent);

                var accentPtr = Marshal.AllocHGlobal(accentStructSize);
                Marshal.StructureToPtr(accent, accentPtr, false);

                var data = new WindowCompositionAttributeData();
                data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
                data.SizeOfData = accentStructSize;
                data.Data = accentPtr;

                SetWindowCompositionAttribute(targetForm.Handle, ref data);

                Marshal.FreeHGlobal(accentPtr);
                if (!keepBG)
                {
                    targetForm.BackColor = Color.Lavender;
                }
                //targetForm.Paint += TargetForm_Paint;
            }
            else if (OSName == "Windows 7")
            {
                var hr = CreateEllipticRgn(0, 0, targetForm.Width, targetForm.Height);
                var dbb = new DwmBlurbehind { FEnable = true, DwFlags = 1, HRgnBlur = hr, FTransitionOnMaximized = false };
                DwmEnableBlurBehindWindow(targetForm.Handle, ref dbb);
                if (!keepBG)
                    targetForm.BackColor = Color.Black;
                targetForm.Paint += TargetForm_Paint;
            }
        }
    }
}
