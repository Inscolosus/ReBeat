using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace ReBeat.HarmonyPatches.Score {
    [HarmonyPatch]
    class DisableScoresaberSubmission {
        static bool Prepare() {
            return AppDomain.CurrentDomain.GetAssemblies().Any(asm => asm.GetName().Name == "ScoreSaber");
        }
        
        static MethodBase TargetMethod() {
            return AccessTools.TypeByName("ScoreSaber.Core.Daemons.UploadDaemon").GetMethod("Five");
        }

        static bool Prefix() {
            return !Config.Instance.Enabled;
        }
    }
}