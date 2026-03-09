using System;
using System.Collections.Generic;
using System.Text;

namespace GameServerUtilsForWL.Patching.Patches {
    [HarmonyLib.HarmonyPatch(typeof(JobDispensor<JobMission>), "ServerRequestJob")]
    [HarmonyLib.HarmonyPatch(typeof(JobDispensor<JobMission>), "ServerClientAcceptJob")]
    public static class JobPatch {
        public static bool Prefix() {
            return !Plugin.settings.patchJobs;
        }
    }
}
