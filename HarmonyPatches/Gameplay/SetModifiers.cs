using System;
using System.Reflection;
using HarmonyLib;
using ReBeat.HarmonyPatches.UI;
using Zenject;

namespace ReBeat.HarmonyPatches.Gameplay {
    [HarmonyPatch]
    class SetModifiers {
        static MethodBase TargetMethod() {
            return AccessTools.FirstMethod(typeof(MenuTransitionsHelper), method => method.Name == "StartStandardLevel");
        }
        static void Prefix(ref GameplayModifiers gameplayModifiers) {
            if (!Config.Instance.Enabled) return;
            
            var g = typeof(GameplayModifiers);
            const BindingFlags b = BindingFlags.Instance | BindingFlags.NonPublic;
            var m = Modifiers.instance;
            
            // ReSharper disable PossibleNullReferenceException
            g.GetField("_energyType", b).SetValue(gameplayModifiers, GameplayModifiers.EnergyType.Battery);
            g.GetField("_noFailOn0Energy", b).SetValue(gameplayModifiers, m.NoFail);
            g.GetField("_instaFail", b).SetValue(gameplayModifiers, m.OneLife);
            g.GetField("_failOnSaberClash", b).SetValue(gameplayModifiers, false);
            g.GetField("_enabledObstacleType", b).SetValue(gameplayModifiers, m.EnabledObstacleType);
            g.GetField("_noBombs", b).SetValue(gameplayModifiers, m.NoBombs);
            g.GetField("_fastNotes", b).SetValue(gameplayModifiers, false);
            g.GetField("_strictAngles", b).SetValue(gameplayModifiers, false);
            g.GetField("_disappearingArrows", b).SetValue(gameplayModifiers, m.DisappearingArrows);
            g.GetField("_songSpeed", b).SetValue(gameplayModifiers, m.SongSpeed);
            g.GetField("_noArrows", b).SetValue(gameplayModifiers, m.NoArrows);
            g.GetField("_ghostNotes", b).SetValue(gameplayModifiers, m.GhostNotes || m.Hidden);
            g.GetField("_proMode", b).SetValue(gameplayModifiers, m.ProMode);
            g.GetField("_zenMode", b).SetValue(gameplayModifiers, false);
            g.GetField("_smallCubes", b).SetValue(gameplayModifiers, m.SmallNotes);
        }
    }
}