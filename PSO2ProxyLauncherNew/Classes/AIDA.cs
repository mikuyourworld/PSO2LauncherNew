using System;
using PSO2ProxyLauncherNew.Classes.Infos;
using PSO2ProxyLauncherNew.Classes.Controls;
using PSO2ProxyLauncherNew.Classes.Components.WebClientManger;
using System.IO;

namespace PSO2ProxyLauncherNew.Classes
{
    public static class AIDA
    {

        public const string ArksLayerProtocol = "http";
        public const string ArksLayerHost = "arks-layer.com";
        public const string RemoteJson = ArksLayerProtocol + "://" + ArksLayerHost + "/remote.json";

        public static class LocalPatches
        {
            public static string EnglishVersion
            {
                get { return AIDAConfigManager.Instance.GetSetting(DefaultValues.AIDA.Tweaker.Registries.ENPatchVersion, DefaultValues.AIDA.Tweaker.Registries.NoPatchString); }
                set { AIDAConfigManager.Instance.SetSetting(DefaultValues.AIDA.Tweaker.Registries.ENPatchVersion, value); }
            }
            public static string LargeFilesVersion
            {
                get { return AIDAConfigManager.Instance.GetSetting(DefaultValues.AIDA.Tweaker.Registries.LargeFilesVersion, DefaultValues.AIDA.Tweaker.Registries.NoPatchString); }
                set { AIDAConfigManager.Instance.SetSetting(DefaultValues.AIDA.Tweaker.Registries.LargeFilesVersion, value); }
            }
            public static string StoryVersion
            {
                get { return AIDAConfigManager.Instance.GetSetting(DefaultValues.AIDA.Tweaker.Registries.StoryPatchVersion, DefaultValues.AIDA.Tweaker.Registries.NoPatchString); }
                set { AIDAConfigManager.Instance.SetSetting(DefaultValues.AIDA.Tweaker.Registries.StoryPatchVersion, value); }
            }
        }
        public static string PSO2Dir
        {
            get { return AIDAConfigManager.Instance.GetSetting(DefaultValues.AIDA.Tweaker.Registries.PSO2Dir, string.Empty); }
            set { AIDAConfigManager.Instance.SetSetting(DefaultValues.AIDA.Tweaker.Registries.PSO2Dir, value); }
        }

        public static class TweakerWebPanel
        {
            public static string InfoPageLink { get; internal set; }
            public static string FreedomURL { get; internal set; }
            public static bool ItemPatchWorking { get; internal set; }
            public static string PluginURL { get; internal set; }
            public const string CutString = "PSO2 Patch Compatibility";
        }

        public static class WebPatches
        {
            public static string PatchesInfos { get { return CommonMethods.URLConcat(TweakerWebPanel.FreedomURL, "patches.json"); } }
            public static string PatchesFileListInfos { get { return CommonMethods.URLConcat(TweakerWebPanel.FreedomURL, "filelists.json"); } }
        }

        public static void GetIdeaServer()
        {
            try
            {
                string TheExternalServer = WebClientPool.GetWebClient_AIDA().DownloadString(RemoteJson);

                if (!string.IsNullOrEmpty(TheExternalServer))
                    using (var jsonStringReader = new System.IO.StringReader(TheExternalServer))
                    using (var jsonReader = new Newtonsoft.Json.JsonTextReader(jsonStringReader))
                        while (jsonReader.Read())
                            if (jsonReader.TokenType == Newtonsoft.Json.JsonToken.PropertyName)
                                switch ((jsonReader.Value as string).ToLower())
                                {
                                    case "infourl":
                                        TweakerWebPanel.InfoPageLink = jsonReader.ReadAsString().URLtrim();
                                        break;
                                    case "freedomurl":
                                        TweakerWebPanel.FreedomURL = jsonReader.ReadAsString().URLtrim();
                                        break;
                                    case "pluginurl":
                                        TweakerWebPanel.PluginURL = jsonReader.ReadAsString().URLtrim();
                                        break;
                                    case "itempatchworking":
                                        string tmp = jsonReader.ReadAsString();
                                        tmp = tmp.ToLower();
                                        if (tmp == "yes" | tmp == "true")
                                        { TweakerWebPanel.ItemPatchWorking = true; }
                                        else
                                        { TweakerWebPanel.ItemPatchWorking = false; }
                                        break;
                                    default:
                                        break;
                                }
            }
            catch { }
        }

        public static T FlatJsonFetch<T>(string jsonText, string propertyName)
        {
            T result;
            using (var sr = new System.IO.StringReader(jsonText))
            using (var jsonReader = new Newtonsoft.Json.JsonTextReader(sr))
            {
                result = FlatJsonFetch<T>(jsonReader, propertyName);
            }
            return result;
        }

        public static T FlatJsonFetch<T>(Newtonsoft.Json.JsonReader jsonReader, string propertyName)
        {
            T result = default(T);
            propertyName = propertyName.ToLower();
            while (jsonReader.Read())
                if (jsonReader.TokenType == Newtonsoft.Json.JsonToken.PropertyName)
                    if ((jsonReader.Value as string).ToLower() == propertyName)
                        if (jsonReader.Read())
                        {
                            result = (T)jsonReader.Value;
                            break;
                        }
            return result;
        }

        public static string[] StringToTableString(string source, char split = '\n')
        {
            System.Collections.Generic.List<string> theList = new System.Collections.Generic.List<string>();
            if (!string.IsNullOrWhiteSpace(source))
            {
                if ((split == '\n') || (split == '\r'))
                {
                    string linebuffer;
                    using (StringReader sr = new StringReader(source))
                        while (sr.Peek() > -1)
                        {
                            linebuffer = sr.ReadLine();
                            if (!string.IsNullOrWhiteSpace(linebuffer))
                                theList.Add(linebuffer);
                        }
                }
                else
                {
                    string linebuffer = string.Empty;
                    char currentChar;
                    for (int i = 0; i < source.Length; i++)
                    {
                        currentChar = source[i];
                        if (currentChar == split)
                        {
                            if (!string.IsNullOrWhiteSpace(linebuffer))
                                theList.Add(linebuffer);
                            linebuffer = string.Empty;
                        }
                        else
                        {
                            linebuffer += currentChar;
                        }
                    }
                }
            }
            return theList.ToArray();
        }

        public static RerwiteHTMLResult RerwiteHTML(string htmlContent, string StartString)
        {
            string result1 = htmlContent;
            PatchStatus result2 = PatchStatus.Unknown;
            PatchStatus result3 = PatchStatus.Unknown;
            System.Text.StringBuilder TheStringBuilder = new System.Text.StringBuilder();
            bool StartedAr = false;
            StartString = StartString.ToLower();
            using (System.IO.StringReader TheStreamReader = new System.IO.StringReader(htmlContent))
            {
                //"PSO2 Patch Compatibility"
                string TheLine = null;
                string TheLowerLine = null;
                while ((TheStreamReader.Peek() > 0))
                {
                    TheLine = TheStreamReader.ReadLine();
                    TheLowerLine = TheLine.ToLower();
                    if ((TheLowerLine.IndexOf(StartString) > -1))
                        StartedAr = true;
                    if ((StartedAr == true))
                        TheStringBuilder.AppendLine(TheLine);
                    if ((TheLowerLine.IndexOf("english patch:") > -1))
                    {
                        if ((TheLowerLine.IndexOf("color=\"green\"") > -1))
                            result2 = PatchStatus.Compatible;
                        else if ((TheLowerLine.IndexOf("color=\"red\"") > -1))
                            result2 = PatchStatus.Incompatible;
                        else if ((TheLowerLine.IndexOf("color=\"d4a017\"") > -1))
                            result2 = PatchStatus.Unknown;
                    }
                    else if ((TheLowerLine.IndexOf(">") > -1) && (TheLowerLine.IndexOf("item") > -1) && (TheLowerLine.IndexOf(":") > -1))
                    {
                        if ((TheLowerLine.IndexOf("color=\"green\"") > -1))
                            result3 = PatchStatus.Compatible;
                        else if ((TheLowerLine.IndexOf("color=\"red\"") > -1))
                            result3 = PatchStatus.Incompatible;
                        else if ((TheLowerLine.IndexOf("color=\"d4a017\"") > -1))
                            result3 = PatchStatus.Unknown;
                    }
                }
            }
            string TheStringB = TheStringBuilder.ToString();
            if ((TheStringB.ToLower().IndexOf("</html>") > -1))
            {
                TheStringB = TheStringB.Remove(TheStringB.ToLower().IndexOf("</html>"), 7);
            }
            if (string.IsNullOrWhiteSpace(TheStringB))
                result1 = string.Empty;
            else
                result1 = "<HTML><META HTTP-EQUIV=\"Pragma\" CONTENT=\"no-cache\"><META HTTP-EQUIV=\"Expires\" CONTENT=\"-1\"><BODY>" + TheStringB + "</BODY></HTML>";
            return (new Classes.AIDA.RerwiteHTMLResult(result1, result2, result3));
        }

        public static DateTime StringToDateTime(string source, string formatString, char splitString)
        {
            DateTime dt = DateTime.MinValue;
            if (!string.IsNullOrWhiteSpace(formatString))
            {
                if (!DateTime.TryParseExact(source, formatString, null, System.Globalization.DateTimeStyles.None, out dt))
                    dt = StringToDateTime(source, splitString);
            }
            else
                dt = StringToDateTime(source, splitString);
            return dt;
        }

        public static DateTime StringToDateTime(string source, char splitString)
        {
            DateTime dt = DateTime.MinValue;
            string[] TheSplitLargeFiles = source.Split(splitString);
            int TheMonth = -1;
            if (int.TryParse(TheSplitLargeFiles[0], out TheMonth))
            {
                int TheDay = -1;
                if (int.TryParse(TheSplitLargeFiles[1], out TheDay))
                {
                    int TheYear = -1;
                    if (int.TryParse(TheSplitLargeFiles[2], out TheYear))
                    {
                        dt = new DateTime(TheYear, TheMonth, TheDay);
                    }
                }
            }
            return dt;
        }

        public static string ToVersionString(this DateTime dt)
        {
            return (dt.Month.ToString() + @"/" + dt.Day.ToString() + @"/" + dt.Year.ToString());
        }

        public class RerwiteHTMLResult
        {
            public string HTML { get; private set; }
            public PatchStatus EnglishPatch { get; private set; }
            public PatchStatus ItemPatch { get; private set; }
            public RerwiteHTMLResult(string htmlContent) : this(htmlContent, PatchStatus.Unknown) { }
            public RerwiteHTMLResult(string htmlContent, PatchStatus en) : this(htmlContent, en, PatchStatus.Unknown) { }
            public RerwiteHTMLResult(string htmlContent, PatchStatus en, PatchStatus item)
            {
                this.HTML = htmlContent;
                this.EnglishPatch = en;
                this.ItemPatch = item;
            }
        }
    }
}
