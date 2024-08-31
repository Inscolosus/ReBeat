using System.Linq;
using HarmonyLib;
using Polyglot;
using ReBeat.Utils;

namespace ReBeat.HarmonyPatches.UI {
    [HarmonyPatch(typeof(MainSystemInit))]
    class RegisterCustomCharacteristics {
	    [HarmonyPrefix]
	    [HarmonyPatch(nameof(MainSystemInit.InstallBindings))]
	    static void RegisterAll(MainSystemInit __instance) {
		    Plugin.Log.Debug("Registering custom characteristics");
		    var allCharacteristics =
			    __instance._beatmapCharacteristicCollection._beatmapCharacteristics.Concat(SongCore.Collections
				    .customCharacteristics.ToList());

		    var rebeatIcon = SongCore.Utilities.Utils.LoadTextureFromResources("ReBeat.Assets.icon.png");
		    var transferIcon = SongCore.Utilities.Utils.LoadTextureFromResources("ReBeat.Assets.transfer.png");

		    foreach (var characteristic in allCharacteristics) {
			    var charIcon = characteristic.icon;
			    var charTex = charIcon.texture.isReadable ? charIcon.texture : TextureUtils.DuplicateTexture(charIcon);
			    var combined = SongCore.Utilities.Utils.LoadSpriteFromTexture(
				    TextureUtils.MergeTextures(new [] { rebeatIcon, transferIcon, charTex }));
			    SongCore.Collections.RegisterCustomCharacteristic(combined, $"ReBeat! {characteristic.name}",
				    $"ReBeat! {Localization.Get(characteristic.descriptionLocalizationKey)}", $"ReBeat_{characteristic.serializedName}",
				    $"ReBeat_{characteristic.compoundIdPartName}", characteristic.requires360Movement,
				    characteristic.containsRotationEvents, characteristic.sortingOrder);
		    }
	    }
    }
}