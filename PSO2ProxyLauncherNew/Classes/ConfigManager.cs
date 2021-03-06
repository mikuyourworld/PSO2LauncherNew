﻿using Microsoft.Win32;
using System.IO;
using System;
using Newtonsoft.Json.Linq;
using Leayal.Ini;

namespace PSO2ProxyLauncherNew.Classes
{
    internal sealed partial class ConfigManager : IDisposable
    {
        private static ConfigManager defaultInstance = new ConfigManager();
        public static ConfigManager Instance
        { get { return defaultInstance; } }

        private RegistryKey theReg;
        private IniFile portable;

        public ConfigManager()
        {
            this.theReg = Registry.LocalMachine.OpenSubKey(Path.Combine("SOFTWARE", "Leayal", "PSO2Launcher"), RegistryKeyPermissionCheck.ReadSubTree);
            this.portable = new IniFile(Path.Combine(Leayal.AppInfo.AssemblyInfo.DirectoryPath, "config.ini"));
            if (this.portable.IsEmpty && this.theReg != null)
            {
                string[] names = this.theReg.GetValueNames();
                for (int i =0; i< names.Length; i++)
                    this.SetKeyValue(names[i], this.theReg.GetValue(names[i]), theReg.GetValueKind(names[i]));
            }
        }

        private object GetKeyValue(string KeyName, object DefaultValue)
        {
            string output = this.portable.GetValue("Settings", KeyName, string.Empty);
            if (!string.IsNullOrWhiteSpace(output))
            {
                return output;
            }
            else
            {
                if (this.theReg != null)
                {
                    object result = this.theReg.GetValue(KeyName);
                    if (result == null)
                        return DefaultValue;
                    else
                    {
                        if (result is string)
                            this.portable.SetValue("Settings", KeyName, (string)result);
                        else if (result is int)
                            this.portable.SetValue("Settings", KeyName, result.ToString());
                        else if (result is bool)
                            this.portable.SetValue("Settings", KeyName, (bool)result ? "1" : "0");
                        return result;
                    }
                }
                else
                {
                    if (DefaultValue is string)
                        this.portable.SetValue("Settings", KeyName, (string)DefaultValue);
                    else if (DefaultValue is bool)
                        this.portable.SetValue("Settings", KeyName, (bool)DefaultValue ? "1" : "0");
                    else
                        this.portable.SetValue("Settings", KeyName, DefaultValue.ToString());
                    return DefaultValue;
                }
            }
        }

        private void SetKeyValue(string KeyName, object TheValue, RegistryValueKind type)
        {
            switch (type)
            {
                case RegistryValueKind.String:
                    this.portable.SetValue("Settings", KeyName, (string)TheValue);
                    break;
                case RegistryValueKind.ExpandString:
                    this.portable.SetValue("Settings", KeyName, (string)TheValue);
                    break;
                case RegistryValueKind.MultiString:
                    this.portable.SetValue("Settings", KeyName, (string)TheValue);
                    break;
                default:
                    this.portable.SetValue("Settings", KeyName, TheValue.ToString());
                    break;
            }
            this.portable.Save();
            // Discard Registry
            /*this.theReg.SetValue(KeyName, TheValue, type);
            this.theReg.Flush();//*/
        }

        public void SetSetting(string SettingName, string SettingValue)
        {
            this.SetKeyValue(SettingName, SettingValue, RegistryValueKind.String);
        }

        public string GetSetting(string SettingName, string DefaultValue)
        {
            return (this.GetKeyValue(SettingName, DefaultValue) as string);
        }

        public bool GetBool(string SettingName, bool DefaultValue)
        {
            return (Convert.ToInt32(this.GetKeyValue(SettingName, DefaultValue ? 1 : 0)) != 0);
        }

        public void SetBool(string SettingName, bool SettingValue)
        {
            this.SetKeyValue(SettingName, SettingValue ? 1 : 0, RegistryValueKind.DWord);
        }

        public int GetInt(string SettingName, int DefaultValue)
        {
            return Convert.ToInt32(this.GetKeyValue(SettingName, DefaultValue));
        }

        public void SetInt(string SettingName, int SettingValue)
        {
            this.SetKeyValue(SettingName, SettingValue, RegistryValueKind.DWord);
        }

        public void Close()
        {
            this.Dispose();
        }

        public void Save()
        {
            if (_disposed) return;
            this.portable.Save();
        }

        private bool _disposed;
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            //this.portable.Save();
            this.theReg.Dispose();
            this.portable.Close();
        }
    }

    internal sealed partial class AIDAConfigManager : IDisposable
    {
        private static string AIDASettingsLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PSO2 Tweaker", "settings.json");
        private static AIDAConfigManager defaultInstance = new AIDAConfigManager();
        public static AIDAConfigManager Instance
        { get { return defaultInstance; } }

        private RegistryKey theReg;
        private JObject AIDASettings;
        private DateTime jsonLastWrite = DateTime.MinValue;

        public AIDAConfigManager()
        {
            this.theReg = Registry.CurrentUser.CreateSubKey(Path.Combine("SOFTWARE", "AIDA"), RegistryKeyPermissionCheck.ReadWriteSubTree);
        }

        private bool readJson()
        {
            if (File.Exists(AIDASettingsLocation))
            {
                DateTime dt = File.GetLastWriteTime(AIDASettingsLocation);
                if (this.jsonLastWrite != dt)
                {
                    using (StreamReader sr = new StreamReader(AIDASettingsLocation))
                        AIDASettings = JObject.Parse(sr.ReadToEnd());
                    this.jsonLastWrite = dt;
                }
                return true;
            }
            else
                return false;
        }

        private object GetKeyValue(string KeyName, object DefaultValue)
        {
            if (this.readJson())
            {
                JToken token;
                if (AIDASettings.TryGetValue(KeyName, out token))
                    return token.Value<object>();
            }
            object result = this.theReg.GetValue(KeyName);
            if (result == null)
                return DefaultValue;
            else
                return result;
        }

        private void SetKeyValue(string KeyName, object TheValue, RegistryValueKind type)
        {
            if (this.readJson())
            {
                JToken token;
                bool write = true;
                if (AIDASettings.TryGetValue(KeyName, out token))
                {
                    string asdd = (token.Value<object>()).ToString();
                    if (type == RegistryValueKind.String || type == RegistryValueKind.DWord || type == RegistryValueKind.ExpandString || type == RegistryValueKind.MultiString || type == RegistryValueKind.QWord)
                        write = (!string.IsNullOrEmpty(asdd) && asdd.ToLower() != TheValue.ToString().ToLower());
                }
                if (write)
                    using (JTokenWriter asd = new JTokenWriter())
                    {
                        asd.WriteValue(TheValue);
                        asd.Flush();
                        AIDASettings[KeyName] = asd.Token;
                        using (StreamWriter sr = new StreamWriter(AIDASettingsLocation, false))
                        using (Newtonsoft.Json.JsonTextWriter jr = new Newtonsoft.Json.JsonTextWriter(sr))
                        {
                            AIDASettings.WriteTo(jr);
                            jr.Flush();
                        }
                        this.jsonLastWrite = DateTime.Now;
                        File.SetLastWriteTime(AIDASettingsLocation, this.jsonLastWrite);
                    }
            }
            this.theReg.SetValue(KeyName, TheValue, type);
            this.theReg.Flush();
        }

        public void SetSetting(string SettingName, string SettingValue)
        {
            this.SetKeyValue(SettingName, SettingValue, RegistryValueKind.String);
        }

        public string GetSetting(string SettingName, string DefaultValue)
        {
            object ho = this.GetKeyValue(SettingName, DefaultValue);
            var re = ho as string;
            if (re != null)
                return re;
            else
                return ho.ToString();
        }

        public bool GetBool(string SettingName, bool DefaultValue)
        {
            return (Convert.ToInt32(this.GetKeyValue(SettingName, DefaultValue ? 1 : 0)) != 0);
        }

        public void SetBool(string SettingName, bool SettingValue)
        {
            this.SetKeyValue(SettingName, SettingValue ? 1 : 0, RegistryValueKind.DWord);
        }

        public void Close()
        {
            this.theReg.Close();
        }

        public void Dispose()
        {
            this.theReg.Dispose();
        }
    }
}
