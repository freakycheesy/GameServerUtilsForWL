using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServerUtilsForWL.Patching.Patches {
    [HarmonyPatch(typeof(TrafficManager), "SpawnAI")]
    [HarmonyPatch(typeof(TrafficManager), "SpawnAIVehicle")]
    public static class NpcCarPatch {
        public static bool Prefix(TrafficManager __instance) {
            return !Plugin.settings.patchTrafficManager;
        }
    }
}
