using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using PSO2ProxyLauncherNew.Classes.Infos;
using PSO2ProxyLauncherNew.Classes.Components.Ini;

namespace PSO2ProxyLauncherNew.Classes
{
    internal sealed partial class LanguageManager
    {
        private static LanguageManager defaultInstance = new LanguageManager(MySettings.Language);
        public static LanguageManager Instance
        { get { return defaultInstance; } }
        public string Language
        { get; private set; }
        public IniFile LanguageIniFile
        { get; private set; }
        public LanguageManager(string langName)
        {
            if (!File.Exists(Path.Combine(DefaultValues.MyInfo.Directory.LanguageFolder, langName + ".ini")))
                langName = "english";
            this.Language = langName;
            this.LanguageIniFile = new IniFile(Path.Combine(DefaultValues.MyInfo.Directory.LanguageFolder, langName.ToLower() + ".ini"));
        }

        public static void TranslateForm(Form target)
        {
            var values = Instance.LanguageIniFile.GetAllValues("Form." + target.Name);
            if (values != null && values.Count > 0)
                TranslateForm(target, values);
            //target.Name;
        }

        public static string GetMessageText(string msgID, string defaultString)
        {
            return Instance.LanguageIniFile.GetValue("Message", msgID, defaultString).Trim();
            //target.Name;
        }

        private static void TranslateForm(Form targetForm, Dictionary<string, IniKeyValue> langmap)
        {
            foreach (Control child in targetForm.Controls)
                TranslateChildren(child, langmap);
        }

        private static void TranslateChildren(Control parent, Dictionary<string, IniKeyValue> langmap)
        {
            if (langmap.ContainsKey(parent.Name))
                parent.Text = langmap[parent.Name].Value;
            foreach (Control child in parent.Controls)
                TranslateChildren(child, langmap);
        }
    }
}
