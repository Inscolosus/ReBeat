using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using ReBeat.HarmonyPatches.UI;

namespace ReBeat.HarmonyPatches.Score {
    [HarmonyPatch]
    class BeatleaderModifiers {
        static bool Prepare() {
            bool blExists = AppDomain.CurrentDomain.GetAssemblies().Any(asm => asm.GetName().Name == "BeatLeader");
            if (!blExists) Plugin.Log.Warn("BeatLeader not present! Scores will not be uploaded.");
            return blExists;
        }
        
        static MethodBase TargetMethod() {
            var mapEnhancer = AccessTools.TypeByName("BeatLeader.Core.Managers.ReplayEnhancer.MapEnhancer");
            return AccessTools.Method(mapEnhancer, "modifiers");
        }

        [HarmonyPostfix]
        static void Mods(List<string> __result) {
            if (!Config.Instance.Enabled) return;

            __result.Remove("BE");
            var m = Modifiers.instance;
            if (m.Hidden) {
                __result.Add("HD");
                __result.Remove("GN");
            }
            if (m.SameColor) __result.Add("SMC");
            if (m.EasyMode) __result.Add("EZ");
            if (m.OneHp) __result.Add("OHP");
        }
    }
}