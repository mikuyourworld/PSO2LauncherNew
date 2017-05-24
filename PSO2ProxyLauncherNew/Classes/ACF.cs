using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSO2ProxyLauncherNew.Classes
{
    public static class ACF
    {
        public const string EnglishPatchManualHome = "http://pso2.acf.me.uk/Manual/";
        public const string EnglishPatchBaseURL = "https://pso2.acf.me.uk/Manual/index.php?file=";

        public static Uri GetUrlEnglishPatch(string version)
        {
            return new Uri(EnglishPatchBaseURL + version + ".zip");
        }

        public static string GetVersionFromURL(Uri url)
        {
            return GetVersionFromURL(url.AbsoluteUri);
        }

        public static string GetVersionFromURL(string url)
        {
            url = url.Remove(0, EnglishPatchBaseURL.Length);
            return System.IO.Path.GetFileNameWithoutExtension(url);
        }

        /// <summary>
        /// Praise the sun parody. Joke aside, it's a dirty code to fetch the latest patch's URL
        /// </summary>
        /// <param name="htmlcontent"></param>
        /// <returns></returns>
        public static string ParseTheWeb(string htmlcontent)
        {
            //<a href=https://pso2.acf.me.uk/Manual/index.php?file=patch_2017_05_24.zip>Click here to download the latest patch.</a>
            int startindex = htmlcontent.IndexOf("<a");
            if (startindex > -1)
            {
                startindex += 2;
                startindex = htmlcontent.IndexOf("href", startindex);
                if (startindex > -1)
                {
                    startindex += 4;
                    startindex = htmlcontent.IndexOf("=", startindex);
                    if (startindex > -1)
                    {
                        startindex += 1;
                        int startquot = htmlcontent.IndexOf("\"", startindex);
                        if (startquot > -1 && string.IsNullOrWhiteSpace(htmlcontent.Substring(startquot + 1, (startquot + 1) - startindex + 1)))
                        {
                            int endquot = htmlcontent.IndexOf("\"", ++startquot);
                            if (endquot > -1)
                                return htmlcontent.Substring(startindex, endquot - startindex);
                        }
                        else
                        {
                            int startspace = htmlcontent.IndexOf(">", startindex);
                            if (startspace > -1)
                            {
                                string returnresult = htmlcontent.Substring(startindex, startspace - startindex);
                                if (!string.IsNullOrWhiteSpace(returnresult))
                                {
                                    returnresult = returnresult.Trim();
                                    returnresult = returnresult.Trim('"');
                                    return returnresult;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }
    }
}
