using System;
using System.Collections.Generic;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using SharpCompress.Readers;

namespace PSO2ProxyLauncherNew.Classes.Components
{
    class AbstractArchive
    {
        public static AbstractArchive Open(string filepath)
        {
            AbstractArchive result = null;
            SharpCompress.Archives.Zip.ZipArchive.Open("");
            return result;
        }

        private AbstractArchive(string asd)
        {

        }
    }
}
