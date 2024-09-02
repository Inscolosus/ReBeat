using HarmonyLib;
using ReBeat.HarmonyPatches.UI;
using UnityEngine;

namespace ReBeat.HarmonyPatches.Gameplay {
    [HarmonyPatch(typeof(BoxCuttableBySaber))]
    class NoteColliderSize {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(BoxCuttableBySaber.Awake))]
        static void SetColliderSize(ref BoxCollider ____collider) {
            if (!Config.Instance.Enabled) return;
            ____collider.size = Modifiers.instance.ProMode ? new Vector3(0.45f, 0.45f, 0.6f) :
                Modifiers.instance.EasyMode ? new Vector3(0.8f, 0.5f, 0.8f) : 
            new Vector3(0.5f, 0.5f, 0.7f);
        }
    }
}