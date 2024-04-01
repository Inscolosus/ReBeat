using System;
using System.Reflection;
using BeatSaber5.HarmonyPatches.Score;

namespace BeatSaber5 {
	public class Config {
        public static Config Instance;
        private bool _enabled;

        public virtual bool Enabled {
            get => _enabled;
            set {
                if (value) {
                    BS_Utils.Gameplay.ScoreSubmission.ProlongedDisableSubmission("BeatSaber5");
                    try {
                        SetModifiersMultiplier.SetMultipliers();
                    }
                    catch (NullReferenceException e) {
                        // startup; ignore
                    }
                }
                else {
                    BS_Utils.Gameplay.ScoreSubmission.RemoveProlongedDisable("BeatSaber5");
                    try {
                        SetModifiersMultiplier.ResetMultipliers();
                    }
                    catch (NullReferenceException e) {
                        // startup; ignore
                    }
                }
                _enabled = value;
            }
        }

        public virtual bool ShowComboPercent { get; set; } = false;
        public virtual bool ScoreDebug { get; set; } = false;
        public virtual bool DebugTwo { get; set; } = false;
        public virtual bool DebugThree { get; set; } = false;
        public bool ProMode { get; set; } = false; // might want to move these ones out of the config
        public bool SameColor { get; set; } = false; // see promode
        public virtual bool UseLeftColor { get; set; } = false;
        public bool EasyMode { get; set; } // see promode

        public virtual float ColorRed { get; set; } = 0f;
        public virtual float ColorGreen { get; set; } = 145f;
        public virtual float ColorBlue { get; set; } = 255f;
        public virtual float ThisDoesNothing { get; set; } = 1f;

        public const float StartingHealth = 5.0f;

        public virtual float FadeEndDistance { get; set; } = 2.5f;
        public virtual float FadeDistanceDuration { get; set; } = 2f;
        public virtual float DebugSlider { get; set; } = 0.25f;

        /// <summary>
        /// This is called whenever BSIPA reads the config from disk (including when file changes are detected).
        /// </summary>
        public virtual void OnReload() {
            // Do stuff after config is read from disk.
            Changed();
        }

        /// <summary>
        /// Call this to force BSIPA to update the config file. This is also called by BSIPA if it detects the file was modified.
        /// </summary>
        public virtual void Changed() {
            // Do stuff when the config is changed.
        }

        /// <summary>
        /// Call this to have BSIPA copy the values from <paramref name="other"/> into this config.
        /// </summary>
        public virtual void CopyFrom(Config other) {
            // This instance's members populated from other
        }
    }
}
