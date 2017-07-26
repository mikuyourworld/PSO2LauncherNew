using System;
using System.Collections.Generic;
using System.IO;

namespace Leayal.Ugoira.WPF
{
    public class UgoiraReader : IDisposable
    {
        private ZipStorer archive;
        private ArchiveEntry[] _entries;
        // private IReader archivereader;

        public UgoiraReader(Stream sourceStream)
        {
            this._disposed = false;
            this.archive = ZipStorer.Open(sourceStream, FileAccess.Read);
            /// this.archivereader = this.archive.ExtractAllEntries();
            var list = this.archive.ReadCentralDir();
            this._entries = new ArchiveEntry[list.Count];
            for (int i = 0; i < list.Count; i++)
                this._entries[i] = new ArchiveEntry(this.archive, list[i]);
        }

        public ArchiveEntry this[int index] => this._entries[index];

        public ArchiveEntry[] Entries => this._entries;


        private bool _disposed;
        public void Dispose()
        {
            if (this._disposed) return;
            this._disposed = true;
            if (this.archive != null)
                this.archive.Dispose();
        }
    }
}
