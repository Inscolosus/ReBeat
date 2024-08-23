using System;
using System.Reflection;
using ReBeat.HarmonyPatches.Score;
using ReBeat.HarmonyPatches.UI;

namespace ReBeat {
	public class Config {
        public static Config Instance;
        private bool _enabled;

        public virtual bool Enabled {
            get => _enabled;
            set {
                if (value) {
                    //BS_Utils.Gameplay.ScoreSubmission.ProlongedDisableSubmission("ReBeat");
                }
                else {
                    //BS_Utils.Gameplay.ScoreSubmission.RemoveProlongedDisable("ReBeat");
                }
                _enabled = value;
               // HideModifiersPanel.GsvcInstance.RefreshContent(); // this causes a null reference exception somewhere in the game but everything seems to still work fine
            }
        }

        public virtual bool ShowComboPercent { get; set; } = false;
        public virtual bool UseLeftColor { get; set; } = false;
        public virtual float ColorRed { get; set; } = 0f;
        public virtual float ColorGreen { get; set; } = 145f;
        public virtual float ColorBlue { get; set; } = 255f;

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
