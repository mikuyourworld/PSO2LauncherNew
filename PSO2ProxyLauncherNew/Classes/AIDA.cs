using System;
using PSO2ProxyLauncherNew.Classes.Infos;
using PSO2ProxyLauncherNew.Classes.Controls;
using PSO2ProxyLauncherNew.Classes.Components.WebClientManger;
using System.IO;
using System.Text;
using Leayal;

namespace PSO2ProxyLauncherNew.Classes
{
    public static class AIDA
    {
        public static readonly string ArksLayerProtocol = Uri.UriSchemeHttps;
        public const string ArksLayerHost = "arks-layer.com";

        public static readonly string RemoteJson = ArksLayerProtocol + Uri.SchemeDelimiter + ArksLayerHost + "/remote.json";

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
            public static string RaiserVersion
            {
                get { return AIDAConfigManager.Instance.GetSetting(DefaultValues.AIDA.Tweaker.Registries.RaiserPatchVersion, string.Empty); }
                set { AIDAConfigManager.Instance.SetSetting(DefaultValues.AIDA.Tweaker.Registries.RaiserPatchVersion, value); }
            }
            public static string RaiserEnabled
            {
                get { return AIDAConfigManager.Instance.GetSetting(DefaultValues.AIDA.Tweaker.Registries.RaiserPatchEnabled, "No"); }
                set { AIDAConfigManager.Instance.SetSetting(DefaultValues.AIDA.Tweaker.Registries.RaiserPatchEnabled, value); }
            }
        }
        public static string PSO2Dir
        {
            get { return AIDAConfigManager.Instance.GetSetting(DefaultValues.AIDA.Tweaker.Registries.PSO2Dir, string.Empty); }
            set { AIDAConfigManager.Instance.SetSetting(DefaultValues.AIDA.Tweaker.Registries.PSO2Dir, value); }
        }

        public static string PSO2RemoteVersion
        {
            get { return AIDAConfigManager.Instance.GetSetting(DefaultValues.AIDA.Tweaker.Registries.PSO2RemoteVersion, string.Empty); }
            set { AIDAConfigManager.Instance.SetSetting(DefaultValues.AIDA.Tweaker.Registries.PSO2RemoteVersion, value); }
        }

        public static string ProxyJSONURL
        {
            get { return AIDAConfigManager.Instance.GetSetting(DefaultValues.AIDA.Tweaker.Registries.ProxyJSONURL, string.Empty); }
            set { AIDAConfigManager.Instance.SetSetting(DefaultValues.AIDA.Tweaker.Registries.ProxyJSONURL, value); }
        }

        public static class TweakerWebPanel
        {
            public static string InfoPageLink { get; internal set; }
            public static string FreedomURL { get; internal set; }
            public static bool ItemPatchWorking { get; internal set; }
            public static string PluginURL { get; internal set; }
            public const string PluginJsonFilename = "plugins.json";
            public const string CutString = "PSO2 Patch Compatibility";
        }

        public static class TransarmOrVedaOrWhatever
        {
            public static Exception VEDA_Activate()
            {
                return VEDA_Activate(DefaultValues.AIDA.Tweaker.TransArmThingiesOrWatever.VEDA_MagicWord);
            }
            public static Exception VEDA_Activate(string s)
            {
                string length;
                //int length4 = DateTime.Now.Hour;
                string str = DateTime.Now.Hour.ToString();
                string pSO2RootDir = AIDA.PSO2Dir;
                if (!string.IsNullOrWhiteSpace(pSO2RootDir))
                {
                    if (Directory.Exists(pSO2RootDir))
                    {
                        length = pSO2RootDir;
                        if (length.IndexOf("://") > -1)
                            length = length.Replace("://", ":/");
                        if (length.IndexOf(":\\\\") > -1)
                            length = length.Replace(":\\\\", ":\\");
                        string md5Hash = CommonMethods.StringToMD5(string.Concat("mCDKdWFxcAc582vt", str, length.Length.ToString()));
                        try
                        {
                            File.WriteAllText(CommonMethods.PathConcat(pSO2RootDir, DefaultValues.AIDA.Tweaker.TransArmThingiesOrWatever.VEDA_Filename), md5Hash.ToLower(), Encoding.ASCII);
                            return null;
                        }
                        catch (Exception ex)
                        { return ex; }
                    }
                    else
                        return new DirectoryNotFoundException($"[TRIALSystem] Directory '{pSO2RootDir}' is not existed.");
                }
                else
                    return new DirectoryNotFoundException();
            }
        }

        public static class WebPatches
        {
            public static string PatchesInfos { get { return CommonMethods.URLConcat(TweakerWebPanel.FreedomURL, "patches.json"); } }
            public static string PatchesFileListInfos { get { return CommonMethods.URLConcat(TweakerWebPanel.FreedomURL, "filelists.json"); } }
            public static string TransAmEXE { get { return CommonMethods.URLConcat(TweakerWebPanel.FreedomURL, DefaultValues.AIDA.Tweaker.TransArmThingiesOrWatever.TransAmEXE); } }
            public static string LargeFilesDB { get { return CommonMethods.URLConcat(TweakerWebPanel.FreedomURL, Path.ChangeExtension(DefaultValues.AIDA.Tweaker.TransArmThingiesOrWatever.LargeFilesDB, ".zip")); } }
            public static string StoryDB { get { return CommonMethods.URLConcat(TweakerWebPanel.FreedomURL, Path.ChangeExtension(DefaultValues.AIDA.Tweaker.TransArmThingiesOrWatever.StoryDB, ".zip")); } }
        }

        private static bool _ispingedaida = false;
        public static bool IsPingedAIDA { get { return _ispingedaida; } }

#if DEBUG
        public static bool GetIdeaServer()
        {
            bool result = false;
            try
            {
                string TheExternalServer = WebClientPool.GetWebClient_AIDA().DownloadString(RemoteJson);
                if (!string.IsNullOrEmpty(TheExternalServer))
                {
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
                    _ispingedaida = true;
                    result = true;
                }
            } catch (System.Net.WebException) { _ispingedaida = false; result = false; }
            return result;
        }
#else
        public static bool GetIdeaServer()
        {
            bool result = false;
            try
            {
                string TheExternalServer = WebClientPool.GetWebClient_AIDA().DownloadString(RemoteJson);
                if (!string.IsNullOrEmpty(TheExternalServer))
                {
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
                    _ispingedaida = true;
                    result = true;
                }
            }
            catch (System.Net.WebException ex) { result = false; Log.LogManager.GeneralLog.Print(ex); }
            return result;
        }
#endif

        public static string ToAIDASettings(this bool val)
        {
            if (val)
                return "Yes";
            else
                return "No";
        }

        public static bool BoolAIDASettings(this string val, bool defaultValue)
        {
            if (!string.IsNullOrWhiteSpace(val))
            {
                val = val.ToLower();
                if (val == "yes" || val == "true")
                    return true;
                else
                    return false;
            }
            else
                return defaultValue;
        }

        public static void ActivatePSO2Plugin()
        {
            ActivatePSO2Plugin(MySettings.PSO2Dir);
        }

        public static void ActivatePSO2Plugin(string dir)
        {
            using (Stream resourceStream = MyApp.CurrentAssembly.GetManifestResourceStream("PSO2ProxyLauncherNew.Resources.ddraw7z"))
            using (var archive = SharpCompress.Archives.ArchiveFactory.Open(resourceStream))
            using (var reader = archive.ExtractAllEntries())
                if (reader.MoveToNextEntry())
                    using (FileStream fs = File.Create(CommonMethods.PathConcat(dir, DefaultValues.MyInfo.Filename.ddraw)))
                    {
                        reader.WriteEntryTo(fs);
                        fs.Flush();
                    }
        }

        public static void DeactivatePSO2Plugin()
        {
            DeactivatePSO2Plugin(MySettings.PSO2Dir);
        }

        public static void DeactivatePSO2Plugin(string dir)
        {
            File.Delete(CommonMethods.PathConcat(dir, DefaultValues.MyInfo.Filename.ddraw));
        }

        public static T FlatJsonFetch<T>(string jsonText, string propertyName)
        {
            T result;
            using (var sr = new System.IO.StringReader(jsonText))
            using (var jsonReader = new Newtonsoft.Json.JsonTextReader(sr))
                result = FlatJsonFetch<T>(jsonReader, propertyName);
            return result;
        }

        public static T FlatJsonFetch<T>(Newtonsoft.Json.JsonReader jsonReader, string propertyName)
        {
            T result = default(T);
            propertyName = propertyName.ToLower();
            while (jsonReader.Read())
                if (jsonReader.TokenType == Newtonsoft.Json.JsonToken.PropertyName)
                    if (((string)jsonReader.Value).ToLower() == propertyName)
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
                            linebuffer += currentChar;
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
                        dt = new DateTime(TheYear, TheMonth, TheDay);
                }
            }
            return dt;
        }

        public static string ToVersionString(this DateTime dt)
        {
            return (dt.Month.ToString() + "/" + dt.Day.ToString() + "/" + dt.Year.ToString());
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
