using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using ReBeat.HarmonyPatches.UI;

namespace ReBeat.HarmonyPatches.Score {
    [HarmonyPatch]
    class BeatleaderModifiers {
        static MethodBase TargetMethod() {
            var mapEnhancer = AccessTools.TypeByName("BeatLeader.Core.Managers.ReplayEnhancer.MapEnhancer");
            return AccessTools.Method(mapEnhancer, "modifiers");
        }

        [HarmonyPostfix]
        static void Mods(List<string> __result) {
            if (!Config.Instance.Enabled) return;

            __result.Remove("BE");
            var m = Modifiers.instance;
            if (m.Hidden) __result.Add("HD");
            if (m.SameColor) __result.Add("SMC");
            if (m.EasyMode) __result.Add("EZ");
            if (m.OneHp) __result.Add("OHP");
        }
    }
}