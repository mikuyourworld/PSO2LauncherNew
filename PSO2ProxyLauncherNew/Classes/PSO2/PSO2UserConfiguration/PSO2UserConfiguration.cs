using System;
using Leayal;

namespace PSO2ProxyLauncherNew.Classes.PSO2.PSO2UserConfiguration
{
    public class PSO2UserConfiguration : IDisposable
    {
        private RawPSO2UserConfiguration rawdata;

        public PSO2UserConfiguration() : this(new RawPSO2UserConfiguration()) { }

        public PSO2UserConfiguration(RawPSO2UserConfiguration _rawdata)
        {
            this.rawdata = _rawdata;
        }

        public ScreenResolution ScreenResolution
        {
            get
            {
                int width, height;
                if (Leayal.NumberHelper.TryParse(this.rawdata["Ini"]["Windows"].Values["Width"], out width)
                    && Leayal.NumberHelper.TryParse(this.rawdata["Ini"]["Windows"].Values["Height"], out height))
                {
                    return new ScreenResolution(width, height);
                }
                else
                    return new ScreenResolution(640, 480);
            }
            set
            {
                this.rawdata["Ini"]["Windows"].Values["Width"] = value.Width.ToString();
                this.rawdata["Ini"]["Windows"].Values["Height"] = value.Height.ToString();
            }
        }

        public ScreenMode ScreenMode
        {
            get
            {
                bool fullscreen = this.rawdata["Ini"]["Windows"].Values["FullScreen"].IsEqual("true", true),
                    virtualfullscreen = this.rawdata["Ini"]["Windows"].Values["VirtualFullScreen"].IsEqual("true", true);
                if (fullscreen)
                    return ScreenMode.FullScreen;
                else if (virtualfullscreen)
                    return ScreenMode.VirtualFullScreen;
                else
                    return ScreenMode.Windowed;
            }
            set
            {
                switch (value)
                {
                    case ScreenMode.VirtualFullScreen:
                        this.rawdata["Ini"]["Windows"].Values["FullScreen"] = "false";
                        this.rawdata["Ini"]["Windows"].Values["VirtualFullScreen"] = "true";
                        break;
                    case ScreenMode.FullScreen:
                        this.rawdata["Ini"]["Windows"].Values["FullScreen"] = "true";
                        this.rawdata["Ini"]["Windows"].Values["VirtualFullScreen"] = "false";
                        break;
                    default:
                        this.rawdata["Ini"]["Windows"].Values["FullScreen"] = "false";
                        this.rawdata["Ini"]["Windows"].Values["VirtualFullScreen"] = "false";
                        break;
                }
            }
        }

        public ShaderQuality ShaderQuality
        {
            get
            {
                string dduuuh = this.rawdata["Ini"]["Config"]["Draw"].Values["ShaderLevel"];
                int value = 0;
                if (Leayal.NumberHelper.TryParse(dduuuh, out value))
                {
                    if (value > 1)
                        return ShaderQuality.High;
                    else if (value < 1)
                        return ShaderQuality.Off;
                    else
                        return ShaderQuality.Normal;
                }
                return ShaderQuality.Off;
            }
            set
            {
                this.rawdata["Ini"]["Config"]["Draw"].Values["ShaderLevel"] = ((int)value).ToString();
            }
        }

        public TextureQuality TextureQuality
        {
            get
            {
                string dduuuh = this.rawdata["Ini"]["Config"]["Draw"].Values["TextureResolution"];
                int value = 0;
                if (Leayal.NumberHelper.TryParse(dduuuh, out value))
                {
                    if (value > 1)
                        return TextureQuality.HighRes;
                    else if (value < 1)
                        return TextureQuality.Reduced;
                    else
                        return TextureQuality.Normal;
                }
                return TextureQuality.Reduced;
            }
            set
            {
                this.rawdata["Ini"]["Config"]["Draw"].Values["TextureResolution"] = ((int)value).ToString();
            }
        }

        public bool MoviePlay
        {
            get
            {
                return !this.rawdata["Ini"]["Config"]["Basic"].Values["MoviePlay"].IsEqual("false", true);
            }
            set
            {
                this.rawdata["Ini"]["Config"]["Basic"].Values["MoviePlay"] = value ? "true" : "false";
            }
        }

        public int DrawLevel
        {
            get
            {
                int ddlawhg;
                if (Leayal.NumberHelper.TryParse(this.rawdata["Ini"]["Config"]["Simple"].Values["DrawLevel"], out ddlawhg))
                    return ddlawhg;
                else
                    return 1;
            }
            set { this.rawdata["Ini"]["Config"]["Simple"].Values["DrawLevel"] = value.ToString(); }
        }

        public int FrameKeep
        {
            get
            {
                int ddlawhg;
                if (Leayal.NumberHelper.TryParse(this.rawdata["Ini"].Values["FrameKeep"], out ddlawhg))
                    return ddlawhg;
                else
                    return 60;
            }
            set { this.rawdata["Ini"].Values["FrameKeep"] = value.ToString(); }
        }


        public void SaveAs(string filepath)
        {
            this.rawdata.SaveAs(filepath);
        }

        public void Dispose()
        {
            if (this.rawdata != null)
                this.rawdata.Dispose();
        }
    }
}
