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

    public enum RareDropLevelType : int
    {
        SevenUp,
        TenUp,
        ThirteenUp
    }

    public enum InterfaceSize : int
    {
        Default,
        x125,
        x150
    }
}
