using System.Linq;
using System.Collections.Generic;
using SharpCompress.Archives.Rar;
using System.IO;
using System.Threading;
using Microsoft.VisualBasic.FileIO;

namespace PSO2ProxyLauncherNew.Classes.Components
{
    public static class AbstractExtractor
    {
        private static SynchronizationContext syncContext;

        public static void SetSyncContext(SynchronizationContext sync)
        {
            syncContext = sync;
        }

        public static ArchiveExtractResult Unrar(string zipPath, string outputFolder, System.EventHandler<ExtractProgress> progress_callback)
        {
            return Unrar(zipPath, null, outputFolder, progress_callback);
        }

        public static ArchiveExtractResult Unrar(string zipPath, SharpCompress.Readers.ReaderOptions _readerOptions, string outputFolder, System.EventHandler<ExtractProgress> progress_callback)
        {
            ArchiveExtractResult result = null;
            using (FileStream fs = new FileStream(zipPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var archive = SharpCompress.Archives.Rar.RarArchive.Open(fs, _readerOptions))
                result = Unrar(archive, outputFolder, progress_callback);
            if (result == null)
                result = new ArchiveExtractResult(new Dictionary<bool, List<SharpCompress.Common.IEntry>>());
            return result;
        }

        public static ArchiveExtractResult Extract(SharpCompress.Archives.IArchive extractor, string outputFolder, System.EventHandler<ExtractProgress> progress_callback)
        {
            Dictionary<bool, List<SharpCompress.Common.IEntry>> myList = new Dictionary<bool, List<SharpCompress.Common.IEntry>>();
            myList.Add(true, new List<SharpCompress.Common.IEntry>());
            myList.Add(false, new List<SharpCompress.Common.IEntry>());
            int total = extractor.Entries.Count();
            int extractedindex = 0;
            string titit;
            using (var entries = extractor.ExtractAllEntries())
                while (entries.MoveToNextEntry())
                {
                    if (!entries.Entry.IsDirectory)
                        try
                        {
                            titit = Path.Combine(outputFolder, entries.Entry.Key);
                            FileSystem.CreateDirectory(FileSystem.GetParentPath(titit));
                            using (FileStream fs = File.Create(titit))
                            {
                                entries.WriteEntryTo(fs);
                                fs.Flush();
                            }
                            myList[true].Add(entries.Entry);
                        }
                        catch (System.Exception)
                        { myList[false].Add(entries.Entry); }
                    extractedindex++;
                    if (progress_callback != null)
                        syncContext.Post(new SendOrPostCallback(delegate { progress_callback?.Invoke(extractor, new ExtractProgress(total, extractedindex)); }), null);
                }
            return (new ArchiveExtractResult(myList));
        }

        public static ArchiveExtractResult FlatExtract(SharpCompress.Archives.IArchive extractor, string outputFolder, System.EventHandler<ExtractProgress> progress_callback)
        {
            Dictionary<bool, List<SharpCompress.Common.IEntry>> myList = new Dictionary<bool, List<SharpCompress.Common.IEntry>>();
            myList.Add(true, new List<SharpCompress.Common.IEntry>());
            myList.Add(false, new List<SharpCompress.Common.IEntry>());
            int total = extractor.Entries.Count();
            int extractedindex = 0;
            string titit;
            FileSystem.CreateDirectory(outputFolder);
            using (var entries = extractor.ExtractAllEntries())
                while (entries.MoveToNextEntry())
                {
                    if (!entries.Entry.IsDirectory)
                        try
                        {
                            titit = Path.Combine(outputFolder, Path.GetFileName(entries.Entry.Key));
                            using (FileStream fs = File.Create(titit))
                            {
                                entries.WriteEntryTo(fs);
                                fs.Flush();
                            }
                            myList[true].Add(entries.Entry);
                        }
                        catch (System.Exception)
                        { myList[false].Add(entries.Entry); }
                    extractedindex++;
                    if (progress_callback != null)
                        syncContext.Post(new SendOrPostCallback(delegate { progress_callback?.Invoke(extractor, new ExtractProgress(total, extractedindex)); }), null);
                }
            return (new ArchiveExtractResult(myList));
        }

        public static ArchiveExtractResult Unrar(RarArchive extractor, string outputFolder, System.EventHandler<ExtractProgress> progress_callback)
        {
            Dictionary<bool, List<SharpCompress.Common.IEntry>> myList = new Dictionary<bool, List<SharpCompress.Common.IEntry>>();
            myList.Add(true, new List<SharpCompress.Common.IEntry>());
            myList.Add(false, new List<SharpCompress.Common.IEntry>());
            int total = extractor.Entries.Count;
            int extractedindex = 0;
            using (var entries = extractor.ExtractAllEntries())
                while (entries.MoveToNextEntry())
                {
                    try
                    {
                        FileInfo fi = new FileInfo(Path.Combine(outputFolder, entries.Entry.Key));
                        FileSystem.CreateDirectory(fi.DirectoryName);
                        using (FileStream fs = fi.Create())
                        {
                            entries.WriteEntryTo(fs);
                            fs.Flush();
                        }
                        myList[true].Add(entries.Entry);
                    }
                    catch (System.Exception)
                    { myList[false].Add(entries.Entry); }
                    extractedindex++;
                    if (progress_callback != null)
                        syncContext.Post(new System.Threading.SendOrPostCallback(delegate { progress_callback?.Invoke(extractor, new ExtractProgress(total, extractedindex)); }), null);
                }
            return (new ArchiveExtractResult(myList));
        }

        public static ArchiveExtractResult Extract7z(SharpCompress.Archives.SevenZip.SevenZipArchive extractor, string outputFolder, System.EventHandler<ExtractProgress> progress_callback)
        {
            Dictionary<bool, List<SharpCompress.Common.IEntry>> myList = new Dictionary<bool, List<SharpCompress.Common.IEntry>>();
            myList.Add(true, new List<SharpCompress.Common.IEntry>());
            myList.Add(false, new List<SharpCompress.Common.IEntry>());
            int total = extractor.Entries.Count;
            int extractedindex = 0;
            using (var entries = extractor.ExtractAllEntries())
                while (entries.MoveToNextEntry())
                {
                    try
                    {
                        FileInfo fi = new FileInfo(Path.Combine(outputFolder, entries.Entry.Key));
                        FileSystem.CreateDirectory(fi.DirectoryName);
                        using (FileStream fs = fi.Create())
                        {
                            entries.WriteEntryTo(fs);
                            fs.Flush();
                        }
                        myList[true].Add(entries.Entry);
                    }
                    catch (System.Exception)
                    { myList[false].Add(entries.Entry); }
                    extractedindex++;
                    if (progress_callback != null)
                        syncContext.Post(new System.Threading.SendOrPostCallback(delegate { progress_callback?.Invoke(extractor, new ExtractProgress(total, extractedindex)); }), null);
                }
            return (new ArchiveExtractResult(myList));

            /*Dictionary<bool, List<ArchiveFileInfo>> myList = new Dictionary<bool, List<ArchiveFileInfo>>();
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
            return (new SevenZipExtractResult(myList));//*/
        }

        public static ArchiveExtractResult ExtractZip(string zipPath, string outputFolder, System.EventHandler<ExtractProgress> progress_callback)
        {
            return ExtractZip(zipPath, null, outputFolder, progress_callback);
        }

        public static ArchiveExtractResult ExtractZip(string zipPath, SharpCompress.Readers.ReaderOptions _readerOptions, string outputFolder, System.EventHandler<ExtractProgress> progress_callback)
        {
            ArchiveExtractResult result = null;
            using (FileStream fs = new FileStream(zipPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var archive = SharpCompress.Archives.Zip.ZipArchive.Open(fs, _readerOptions))
                result = ExtractZip(archive, outputFolder, progress_callback);
            if (result == null)
                result = new ArchiveExtractResult(new Dictionary<bool, List<SharpCompress.Common.IEntry>>());
            return result;
        }

        public static ArchiveExtractResult ExtractZip(SharpCompress.Archives.Zip.ZipArchive extractor, string outputFolder, System.EventHandler<ExtractProgress> progress_callback)
        {
            Dictionary<bool, List<SharpCompress.Common.IEntry>> myList = new Dictionary<bool, List<SharpCompress.Common.IEntry>>();
            myList.Add(true, new List<SharpCompress.Common.IEntry>());
            myList.Add(false, new List<SharpCompress.Common.IEntry>());
            int total = extractor.Entries.Count;
            int extractedindex = 0;
            using (var entries = extractor.ExtractAllEntries())
                while (entries.MoveToNextEntry())
                {
                    try
                    {
                        FileInfo fi = new FileInfo(Path.Combine(outputFolder, entries.Entry.Key));
                        FileSystem.CreateDirectory(fi.DirectoryName);
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
                        syncContext.Post(new System.Threading.SendOrPostCallback(delegate { progress_callback?.Invoke(extractor, new ExtractProgress(total, extractedindex)); }), null);
                }
            return (new ArchiveExtractResult(myList));
        }

        public class SevenZipExtractResult
        {
            protected Dictionary<bool, List<SharpCompress.Archives.SevenZip.SevenZipArchiveEntry>> innerList
            { get; private set; }
            public SevenZipExtractResult(Dictionary<bool, List<SharpCompress.Archives.SevenZip.SevenZipArchiveEntry>> list)
            {
                this.innerList = list;
            }

            private SharpCompress.Archives.SevenZip.SevenZipArchiveEntry[] innerItems;
            private SharpCompress.Archives.SevenZip.SevenZipArchiveEntry[] innerFailedItems;
            private SharpCompress.Archives.SevenZip.SevenZipArchiveEntry[] innerSuccessItems;

            public bool IsSuccess
            { get { return (innerList[false].Count == 0); } }

            public SharpCompress.Archives.SevenZip.SevenZipArchiveEntry[] Items
            {
                get
                {
                    if (innerItems == null)
                        innerItems = innerList[true].Concat(innerList[false]).ToArray();
                    return innerItems;
                }
            }

            public SharpCompress.Archives.SevenZip.SevenZipArchiveEntry[] FailedItems
            {
                get
                {
                    if (innerFailedItems == null)
                        innerFailedItems = innerList[false].ToArray();
                    return innerFailedItems;
                }
            }

            public SharpCompress.Archives.SevenZip.SevenZipArchiveEntry[] SuccessItems
            {
                get
                {
                    if (innerSuccessItems == null)
                        innerSuccessItems = innerList[true].ToArray();
                    return innerSuccessItems;
                }
            }
        }

        public class ExtractProgress : System.EventArgs
        {
            public int CurrentIndex { get; private set; }
            public int Total { get; private set; }
            public int Percent { get; }
            public ExtractProgress(int t, int i)
            {
                this.CurrentIndex = i;
                this.Total = t;
                this.Percent = (int)(System.Math.Round((double)(this.CurrentIndex / this.Total), 2) * 100);
            }
        }

        public class ArchiveExtractResult
        {
            protected Dictionary<bool, List<SharpCompress.Common.IEntry>> innerList
            { get; }
            public ArchiveExtractResult(Dictionary<bool, List<SharpCompress.Common.IEntry>> list)
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
