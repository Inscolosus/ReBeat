using HarmonyLib;
using UnityEngine;

namespace BeatSaber5.HarmonyPatches.Gameplay {
    [HarmonyPatch(typeof(BoxCuttableBySaber))]
    class NoteColliderSize {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(BoxCuttableBySaber.Awake))]
        static void SetColliderSize(ref BoxCollider ____collider) {
            float j = Config.Instance.DebugHitboxSize;
            ____collider.size = Config.Instance.ProMode ? new Vector3(0.45f, 0.45f, 0.45f) :
                Config.Instance.DebugHitbox ? new Vector3(j,j,j) : 
            new Vector3(0.5f, 0.5f, 0.5f);
        }
    }
}