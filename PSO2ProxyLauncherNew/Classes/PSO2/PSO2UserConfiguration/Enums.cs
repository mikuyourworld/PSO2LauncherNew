using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSO2ProxyLauncherNew.Classes.PSO2.PSO2UserConfiguration
{
    public enum ScreenMode : byte
    {
        Windowed,
        VirtualFullScreen,
        FullScreen
    }

    public enum ShaderQuality : int
    {
        Off,
        Normal,
        High
    }

    public enum TextureQuality : int
    {
        Reduced,
        Normal,
        HighRes
    }
}
