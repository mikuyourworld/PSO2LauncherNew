namespace Leayal.Ugoira.WPF
{
    public class ArchiveEntry
    {
        public EntryType Type { get; }
        public string Filename { get; }
        public uint CRC { get; }
        public uint Filesize { get; }

        public void Extract(System.IO.Stream writeStream)
        {
            this.innterzip.ExtractFile(this.innerentry, writeStream);
        }

        public byte[] Extract()
        {
            byte[] bytes = null;
            this.innterzip.ExtractFile(this.innerentry, out bytes);
            return bytes;
        }

        private ZipStorer.ZipFileEntry innerentry;
        private ZipStorer innterzip;

        internal ArchiveEntry(ZipStorer zip, ZipStorer.ZipFileEntry entry)
        {
            if (entry.FilenameInZip.EndsWith(".json", System.StringComparison.OrdinalIgnoreCase))
                this.Type = EntryType.Metadata;
            else if (entry.FilenameInZip.EndsWith(".jpg", System.StringComparison.OrdinalIgnoreCase))
                this.Type = EntryType.Image;
            else if (entry.FilenameInZip.EndsWith(".png", System.StringComparison.OrdinalIgnoreCase))
                this.Type = EntryType.Image;
            else if (entry.FilenameInZip.EndsWith(".bmp", System.StringComparison.OrdinalIgnoreCase))
                this.Type = EntryType.Image;
            else if (entry.FilenameInZip.EndsWith(".jpeg", System.StringComparison.OrdinalIgnoreCase))
                this.Type = EntryType.Image;
            else
                this.Type = EntryType.None;

            this.Filename = entry.FilenameInZip;
            this.CRC = entry.Crc32;
            this.Filesize = entry.FileSize;
            this.innerentry = entry;
            this.innterzip = zip;
        }
    }
}
