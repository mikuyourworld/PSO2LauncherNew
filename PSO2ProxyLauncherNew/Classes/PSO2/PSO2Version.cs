using System;
using Leayal;

namespace PSO2ProxyLauncherNew.Classes.PSO2
{
    public class PSO2Version
    {
        public static readonly char[] underline = { '_' };
        //v40500_rc_131
        private string innerRaw;
        public string MajorVersionString { get; }
        public string ReleaseCandidateVersionString { get; }
        public int MajorVersion { get; }
        public int ReleaseCandidateVersion { get; }

        public PSO2Version(string rawstring)
        {
            this.innerRaw = rawstring;
            string[] spplitted = null;
            if (this.innerRaw.IndexOf(underline[0]) > -1)
                spplitted = this.innerRaw.Split(underline, StringSplitOptions.RemoveEmptyEntries);
            if (spplitted != null && spplitted.Length == 3)
            {
                this.MajorVersionString = spplitted[0];
                this.ReleaseCandidateVersionString = spplitted[2];
                string str;
                if (this.MajorVersionString.StartsWith("v"))
                    str = this.MajorVersionString.Remove(0, 1);
                else
                    str = this.MajorVersionString;
                this.MajorVersion = str.ToInt();
                if (this.ReleaseCandidateVersionString.StartsWith("v"))
                    str = this.ReleaseCandidateVersionString.Remove(0, 1);
                else
                    str = this.ReleaseCandidateVersionString;
                this.ReleaseCandidateVersion = str.ToInt();
            }
            else
            {
                this.MajorVersionString = this.innerRaw;
                this.ReleaseCandidateVersionString = null;
                string str;
                if (this.MajorVersionString.StartsWith("v"))
                    str = this.MajorVersionString.Remove(0, 1);
                else
                    str = this.MajorVersionString;
                this.MajorVersion = str.ToInt();
                this.ReleaseCandidateVersion = -1;
            }
        }

        public bool IsEqual(string version)
        {
            return (this.innerRaw.IsEqual(version, true));
        }

        public bool IsEqual(PSO2Version version)
        {
            if (this.MajorVersion == version.MajorVersion && this.ReleaseCandidateVersion == version.ReleaseCandidateVersion)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Compare two version. 0 if equal, 1 if this version is higher than compared version, -1 if this version is lower than compared version.
        /// </summary>
        /// <param name="pso2ver">PSO2Version. The version to be compared.</param>
        /// <returns>int. 0 if equal, 1 if this version is higher than compared version, -1 if this version is lower than compared version.</returns>
        public int CompareTo(PSO2Version pso2ver)
        {
            if (this.MajorVersion < pso2ver.MajorVersion)
                return -1;
            else if (this.MajorVersion > pso2ver.MajorVersion)
                return 1;
            else
            {
                if (this.ReleaseCandidateVersion < pso2ver.ReleaseCandidateVersion)
                    return -1;
                else if (this.ReleaseCandidateVersion > pso2ver.ReleaseCandidateVersion)
                    return 1;
                else
                    return 0;
            }
        }

        public override string ToString()
        {
            return this.innerRaw;
        }
    }
}
