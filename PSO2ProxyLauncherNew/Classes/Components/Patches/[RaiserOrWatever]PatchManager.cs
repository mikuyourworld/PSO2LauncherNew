using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSO2ProxyLauncherNew.Classes.Components.Patches
{
    sealed class RaiserOrWateverPatchManager : PatchManager 
    {
        public override void InstallPatch()
        {
            //this.CheckUpdate(new Uri(Classes.AIDA.WebPatches.PatchesInfos), true);
        }
        public override void CheckUpdate()
        {
            //this.CheckUpdate(new Uri(Classes.AIDA.WebPatches.PatchesInfos), false);
        }
    }
}
