using System.Collections.Generic;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;

namespace ReBeat {
	public class Config {
        public static Config Instance;
        
        internal static GameplayModifiers modifiers;
        internal static bool loadMods;
        internal static bool showCharacteristics = false;
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
        public virtual float ColorRed { get; set; } = 0f;
        public virtual float ColorGreen { get; set; } = 145f;
        public virtual float ColorBlue { get; set; } = 255f;
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
