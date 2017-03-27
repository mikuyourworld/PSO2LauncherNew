using Microsoft.Win32;
using System.IO;
using System;

namespace PSO2ProxyLauncherNew.Classes
{
    internal sealed partial class ConfigManager
    {
        private static ConfigManager defaultInstance = new ConfigManager();
        public static ConfigManager Instance
        { get { return defaultInstance; } }

        private RegistryKey theReg;

        public ConfigManager()
        {
            this.theReg = Registry.LocalMachine.CreateSubKey(Path.Combine("SOFTWARE", "Leayal", "PSO2Launcher"), RegistryKeyPermissionCheck.ReadWriteSubTree);
        }

        private object GetKeyValue(string KeyName, object DefaultValue)
        {
            object result = this.theReg.GetValue(KeyName);
            if (result == null)
                return DefaultValue;
            else
                return result;
        }

        private void SetKeyValue(string KeyName, object TheValue, RegistryValueKind type)
        {
            this.theReg.SetValue(KeyName, TheValue, type);
            this.theReg.Flush();
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
            this.theReg.Close();
        }

        public void Dispose()
        {
            this.theReg.Dispose();
        }
    }

    internal sealed partial class AIDAConfigManager
    {
        private static AIDAConfigManager defaultInstance = new AIDAConfigManager();
        public static AIDAConfigManager Instance
        { get { return defaultInstance; } }

        private RegistryKey theReg;

        public AIDAConfigManager()
        {
            this.theReg = Registry.CurrentUser.CreateSubKey(Path.Combine("SOFTWARE", "AIDA"), RegistryKeyPermissionCheck.ReadWriteSubTree);
        }

        private object GetKeyValue(string KeyName, object DefaultValue)
        {
            object result = this.theReg.GetValue(KeyName);
            if (result == null)
                return DefaultValue;
            else
                return result;
        }

        private void SetKeyValue(string KeyName, object TheValue, RegistryValueKind type)
        {
            this.theReg.SetValue(KeyName, TheValue, type);
            this.theReg.Flush();
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
            return (Convert.ToInt32(this.GetKeyValue(SettingName, DefaultValue ? 1 : 0)) == 0);
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
