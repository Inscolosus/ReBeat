﻿using HarmonyLib;
using UnityEngine;

namespace BeatSaber5.HarmonyPatches.Gameplay {
    [HarmonyPatch(typeof(BoxCuttableBySaber))]
    class NoteColliderSize {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(BoxCuttableBySaber.Awake))]
        static void SetColliderSize(ref BoxCollider ____collider) {
            if (!Config.Instance.Enabled) return;
            ____collider.size = Config.Instance.ProMode ? new Vector3(0.45f, 0.45f, 0.45f) :
                Config.Instance.EasyMode ? new Vector3(0.8f, 0.5f, 0.8f) : 
            new Vector3(0.5f, 0.5f, 0.5f);
        }
    }
}