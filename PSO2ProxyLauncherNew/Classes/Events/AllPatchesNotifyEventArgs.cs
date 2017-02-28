using System;

namespace PSO2ProxyLauncherNew.Classes.Events
{
    class AllPatchesNotifyEventArgs : EventArgs
    {
        public string EnglishPatchVer { get; }
        public string LargeFilesPatchVer { get; }
        public string StoryPatchVer { get; }

        public AllPatchesNotifyEventArgs(string english, string largefiles, string story) : base()
        {
            this.EnglishPatchVer = english;
            this.LargeFilesPatchVer = largefiles;
            this.StoryPatchVer = story;
        }
    }
}
