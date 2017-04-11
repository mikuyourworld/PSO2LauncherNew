using System.Collections.Concurrent;
using System.IO;
using System.Windows.Forms;
using PSO2ProxyLauncherNew.Classes.Infos;
using Leayal.Ini;
using System.Linq;

namespace PSO2ProxyLauncherNew.Classes
{
    internal sealed partial class LanguageManager
    {
        private static LanguageManager defaultInstance = new LanguageManager(MySettings.Language);
        public static LanguageManager Instance
        { get { return defaultInstance; } }
        public string Language
        { get; private set; }
        internal IniFile LanguageIniFile
        { get; private set; }
        public LanguageManager(string langName)
        {
            if (!File.Exists(Path.Combine(DefaultValues.MyInfo.Directory.LanguageFolder, langName + ".ini")))
                langName = "english";
            this.Language = langName;
            this.LanguageIniFile = new IniFile(Path.Combine(DefaultValues.MyInfo.Directory.LanguageFolder, langName.ToLower() + ".ini"), (sender, e)=> {
                if (e.Error != null)
                    Leayal.Log.LogManager.GetLogDefaultPath("languageManager.txt", true).Print(e.Error);
                else
                    Leayal.Log.LogManager.GetLogDefaultPath("languageManager.txt", true).Print(e.DuplicatedKeys.ToString(), Leayal.Log.LogLevel.None);
            });
        }

        public static void GenerateLangFile(string filepath)
        {
            using (StreamWriter sw = new StreamWriter(filepath, false, System.Text.Encoding.UTF8))
                Instance.LanguageIniFile.SaveAs(sw);
        }

        public static void TranslateForm(Form target)
        {
            var values = Instance.LanguageIniFile.GetAllValues("Form." + target.Name);
            if (values != null && values.Count > 0)
                TranslateForm(target, values);
        }

        public static void TranslateForm(Form target, params Control[] exclude)
        {
            var values = Instance.LanguageIniFile.GetAllValues("Form." + target.Name);
            if (values != null && values.Count > 0)
                TranslateForm(target, values, exclude);
        }

        public static string GetMessageText(string msgID, string defaultString)
        {
            string dundun = Instance.LanguageIniFile.GetValue("Message", msgID, string.Empty).Trim();
            if (string.IsNullOrEmpty(dundun))
            {
                Instance.LanguageIniFile.SetValue("Message", msgID, defaultString);
                Instance.LanguageIniFile.Save();
                return defaultString;
            }
            else
                return dundun;
        }

        private static void TranslateForm(Form targetForm, ConcurrentDictionary<string, IniKeyValue> langmap, params Control[] exclude)
        {
            foreach (Control child in targetForm.Controls)
                TranslateChildren(child, langmap, exclude);
        }

        private static void TranslateChildren(Control parent, ConcurrentDictionary<string, IniKeyValue> langmap, params Control[] exclude)
        {
            IniKeyValue asd;
            if (exclude.Where((x) => x.Equals(parent)).Count() == 0)
                if (langmap.TryGetValue(parent.Name, out asd))
                    parent.Text = asd.Value;
            foreach (Control child in parent.Controls)
                TranslateChildren(child, langmap, exclude);
        }

        private static void TranslateForm(Form targetForm, ConcurrentDictionary<string, IniKeyValue> langmap)
        {
            foreach (Control child in targetForm.Controls)
                TranslateChildren(child, langmap);
        }

        private static void TranslateChildren(Control parent, ConcurrentDictionary<string, IniKeyValue> langmap)
        {
            IniKeyValue asd;
            if (langmap.TryGetValue(parent.Name, out asd))
                parent.Text = asd.Value;
            foreach (Control child in parent.Controls)
                TranslateChildren(child, langmap);
        }
    }
}
