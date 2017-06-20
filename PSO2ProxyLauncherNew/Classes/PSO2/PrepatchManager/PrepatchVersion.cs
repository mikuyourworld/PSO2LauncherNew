using System;

namespace PSO2ProxyLauncherNew.Classes.PSO2.PrepatchManager
{
    public struct PrepatchVersion
    {
        public static PrepatchVersion Parse(string data)
        {
            string[] splitted = null;
            if (data.IndexOf(Microsoft.VisualBasic.ControlChars.Tab) > -1)
                splitted = data.Split(new char[] { Microsoft.VisualBasic.ControlChars.Tab }, 2, StringSplitOptions.RemoveEmptyEntries);
            else if (data.IndexOf(' ') > -1)
                splitted = data.Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
            PrepatchVersion result;
            if (splitted != null)
                result = new PrepatchVersion(splitted[0], Leayal.NumberHelper.Parse(splitted[1]));
            else
                result = new PrepatchVersion(data);
            return result;
        }

        public static bool TryParse(string data, out PrepatchVersion result)
        {
            try
            {
                string[] splitted = null;
                if (data.IndexOf(Microsoft.VisualBasic.ControlChars.Tab) > -1)
                    splitted = data.Split(new char[] { Microsoft.VisualBasic.ControlChars.Tab }, 2, StringSplitOptions.RemoveEmptyEntries);
                else if (data.IndexOf(' ') > -1)
                    splitted = data.Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (splitted != null)
                    result = new PrepatchVersion(splitted[0], Leayal.NumberHelper.Parse(splitted[1]));
                else
                    result = new PrepatchVersion(data);
                return true;
            }
            catch
            {
                result = new PrepatchVersion();
                return false;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is PrepatchVersion)
                return this.Equals((PrepatchVersion)obj);
            else
                return false;
        }

        public bool Equals(PrepatchVersion target)
        {
            if (Leayal.StringHelper.IsEqual(this.Version, target.Version, true))
                if (this.ListCount == target.ListCount)
                    return true;
            return false;
        }

        public PrepatchVersion(string _version) : this(_version, 1) { }
        public PrepatchVersion(string _version, int count)
        {
            this.ListCount = count;
            this.Version = _version;
        }
        public string Version { get; }
        public int ListCount { get; }

        public override string ToString()
        {
            return this.ToString(Microsoft.VisualBasic.ControlChars.Tab);
        }

        public string ToString(string delimiter)
        {
            return string.Concat(this.Version, delimiter, this.ListCount);
        }

        public string ToString(char delimiter)
        {
            return string.Concat(this.Version, delimiter, this.ListCount);
        }
    }
}
