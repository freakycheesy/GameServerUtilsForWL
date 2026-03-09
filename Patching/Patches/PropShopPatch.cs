using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServerUtilsForWL.Patching.Patches {
    [HarmonyPatch(typeof(PropShop), "ServerPurchase")]
    public static class PropShopPatch {
        public static bool Prefix(PropShop __instance) {
            return !Plugin.settings.patchPropShop;
        }
    }
}
