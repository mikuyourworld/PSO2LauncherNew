using System;
using System.Management;
using System.Diagnostics;

namespace Leayal.WMI
{
    public static class ProcessParent
    {
        public static int GetParentProcessID()
        {
            using (ManagementObjectSearcher search = new ManagementObjectSearcher("root\\CIMV2", string.Format("SELECT ParentProcessId FROM Win32_Process WHERE ProcessId = {0}", AppInfo.CurrentProcess.Id)))
            using (var results = search.Get().GetEnumerator())
            {
                if (results.MoveNext())
                    return Convert.ToInt32(results.Current["ParentProcessId"]);
                else
                    return -1;
            }
        }

        public static Process GetParentProcess()
        {
            int id = GetParentProcessID();
            if (id > -1)
                return Process.GetProcessById(id);
            else
                return null;
        }
    }
}
