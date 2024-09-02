using System.Collections.Generic;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using UnityEngine;

namespace ReBeat {
	public class Config {
        public static Config Instance;
        
        internal static GameplayModifiers modifiers;
        internal static bool loadMods;

        public virtual string HsvConfig { get; set; } = "HitScoreVisualizerConfig_100max.json";
        private bool _enabled;
        public bool Enabled {
            get => _enabled;
            set { 
                loadMods = !value;
                _enabled = value;
            }
        }

        public virtual bool ShowComboPercent { get; set; } = false;
        public virtual bool UseLeftColor { get; set; } = false;
        public const float FadeEndDistance /*{ get; set; }*/ = 2; // will be a slider setting once I figure out how to fix the arrows
        public const float FadeDurationDistance /*{ get; set; }*/ = 7;
        public virtual Color ShieldColor { get; set; } = Color.cyan;
        public virtual Color LowShieldColor { get; set; } = new Color(0, 0.57f, 1);
        public virtual Color HealthColor { get; set; } = Color.green;
        public virtual Color LowHealthColor { get; set; } = Color.yellow;
        public virtual Color MinHealthColor { get; set; } = Color.red;
        [UseConverter(typeof(ListConverter<bool>))]
        public virtual List<bool> Modifiers { get; set; } = new List<bool>(16);

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
