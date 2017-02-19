using System.Linq;
using System.Collections.Generic;
using SharpCompress.Archives.Rar;
using SevenZip;
using System.IO;
using System.Windows.Forms;

namespace PSO2ProxyLauncherNew.Classes.Components
{
    public static class AbstractExtractor
    {
        private static WindowsFormsSynchronizationContext syncContext;
        public static void SetSevenZipLib(string libPath)
        {
            SevenZip.SevenZipExtractor.SetLibraryPath(libPath);
        }

        public static void SetSyncContext(WindowsFormsSynchronizationContext sync)
        {
            syncContext = sync;
        }

        public static RARExtractResult Unrar(RarArchive extractor, string outputFolder, System.EventHandler<RARExtractProgress> progress_callback)
        {
            Dictionary<bool, List<SharpCompress.Common.IEntry>> myList = new Dictionary<bool, List<SharpCompress.Common.IEntry>>();
            myList.Add(true, new List<SharpCompress.Common.IEntry>());
            myList.Add(false, new List<SharpCompress.Common.IEntry>());
            int total = extractor.Entries.Count;
            int extractedindex = 0;
            using (var entries = extractor.ExtractAllEntries())
            {
                while (entries.MoveToNextEntry())
                {
                    try
                    {
                        FileInfo fi = new FileInfo(Path.Combine(outputFolder, entries.Entry.Key));
                        Directory.CreateDirectory(fi.DirectoryName);
                        using (FileStream fs = fi.Create())
                        {
                            entries.WriteEntryTo(fs);
                            fs.Flush();
                        }
                        myList[true].Add(entries.Entry);
                    }
                    catch (System.Exception)
                    {
                        myList[false].Add(entries.Entry);
                    }
                    extractedindex++;
                    if (progress_callback != null)
                        syncContext.Post(new System.Threading.SendOrPostCallback(delegate { progress_callback?.Invoke(extractor, new RARExtractProgress(total, extractedindex)); }), null);
                }
            }
            return (new RARExtractResult(myList));
        }

        public static SevenZipExtractResult Extract7z(SevenZipExtractor extractor, string outputFolder, System.EventHandler<ProgressEventArgs> progress_callback)
        {
            Dictionary<bool, List<ArchiveFileInfo>> myList = new Dictionary<bool, List<ArchiveFileInfo>>();
            myList.Add(true, new List<ArchiveFileInfo>());
            myList.Add(false, new List<ArchiveFileInfo>());
            extractor.ExtractArchive(outputFolder);
            if (progress_callback != null)
                extractor.Extracting += progress_callback;
            foreach (var node in extractor.ArchiveFileData)
            {
                if (!node.IsDirectory)
                {
                    if (File.Exists(Path.Combine(outputFolder, node.FileName)))
                        myList[true].Add(node);
                    else
                        myList[false].Add(node);
                }
            }
            return (new SevenZipExtractResult(myList));
        }

        public class SevenZipExtractResult
        {
            protected Dictionary<bool, List<ArchiveFileInfo>> innerList
            { get; private set; }
            public SevenZipExtractResult(Dictionary<bool, List<ArchiveFileInfo>> list)
            {
                this.innerList = list;
            }

            private ArchiveFileInfo[] innerItems;
            private ArchiveFileInfo[] innerFailedItems;
            private ArchiveFileInfo[] innerSuccessItems;

            public bool IsSuccess
            { get { return (innerList[false].Count == 0); } }

            public ArchiveFileInfo[] Items
            {
                get
                {
                    if (innerItems == null)
                        innerItems = innerList[true].Concat(innerList[false]).ToArray();
                    return innerItems;
                }
            }

            public ArchiveFileInfo[] FailedItems
            {
                get
                {
                    if (innerFailedItems == null)
                        innerFailedItems = innerList[false].ToArray();
                    return innerFailedItems;
                }
            }

            public ArchiveFileInfo[] SuccessItems
            {
                get
                {
                    if (innerSuccessItems == null)
                        innerSuccessItems = innerList[true].ToArray();
                    return innerSuccessItems;
                }
            }
        }

        public class RARExtractProgress : System.EventArgs
        {
            public int CurrentIndex { get; private set; }
            public int Total { get; private set; }
            public int Percent { get { return (int)(System.Math.Round((double)(this.CurrentIndex / this.Total), 2) * 100); } }
            public RARExtractProgress(int t, int i) { this.CurrentIndex = i; this.Total = t; }
        }

        public class RARExtractResult
        {
            protected Dictionary<bool, List<SharpCompress.Common.IEntry>> innerList
            { get; private set; }
            public RARExtractResult(Dictionary<bool, List<SharpCompress.Common.IEntry>> list)
            {
                this.innerList = list;
            }

            private SharpCompress.Common.IEntry[] innerItems;
            private SharpCompress.Common.IEntry[] innerFailedItems;
            private SharpCompress.Common.IEntry[] innerSuccessItems;

            public bool IsSuccess
            { get { return (innerList[false].Count == 0); } }

            public SharpCompress.Common.IEntry[] Items
            {
                get
                {
                    if (innerItems == null)
                        innerItems = innerList[true].Concat(innerList[false]).ToArray();
                    return innerItems;
                }
            }

            public SharpCompress.Common.IEntry[] FailedItems
            {
                get
                {
                    if (innerFailedItems == null)
                        innerFailedItems = innerList[false].ToArray();
                    return innerFailedItems;
                }
            }

            public SharpCompress.Common.IEntry[] SuccessItems
            {
                get
                {
                    if (innerSuccessItems == null)
                        innerSuccessItems = innerList[true].ToArray();
                    return innerSuccessItems;
                }
            }
        }
    }
}
