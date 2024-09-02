using System.Linq;
using System.Reflection;
using HarmonyLib;
using BGLib.Polyglot;
using ReBeat.Utils;

namespace ReBeat.HarmonyPatches.UI {
	[HarmonyPatch(typeof(BeatmapCharacteristicCollection))]
    class RegisterCustomCharacteristics {
	    [HarmonyPostfix]
	    [HarmonyPatch(MethodType.Constructor, typeof(BeatmapCharacteristicCollectionSO), typeof(AppStaticSettingsSO))]
	    static void RegisterAll(BeatmapCharacteristicCollectionSO collection) {
		    Plugin.Log.Debug("Registering custom characteristics");
		    BeatmapCharacteristicSO[] baseChars = (BeatmapCharacteristicSO[])collection.GetType()
			    .GetField("_beatmapCharacteristics", BindingFlags.Instance | BindingFlags.NonPublic)
			    .GetValue(collection);
		    var allCharacteristics = baseChars.Concat(SongCore.Collections.customCharacteristics.ToList());

		    var rebeatIcon = SongCore.Utilities.Utils.LoadTextureFromResources("ReBeat.Assets.icon.png");
		    var transferIcon = SongCore.Utilities.Utils.LoadTextureFromResources("ReBeat.Assets.transfer.png");

		    foreach (var characteristic in allCharacteristics) {
			    var charIcon = characteristic.icon;
			    var charTex = charIcon.texture.isReadable ? charIcon.texture : TextureUtils.DuplicateTexture(charIcon);
			    var combined = SongCore.Utilities.Utils.LoadSpriteFromTexture(
				    TextureUtils.MergeTextures(new [] { rebeatIcon, transferIcon, charTex }));
			    SongCore.Collections.RegisterCustomCharacteristic(combined, $"ReBeat! {characteristic.name}",
				    "ReBeat! Localization not loaded"/*$"ReBeat! {Localization.Get(characteristic.descriptionLocalizationKey)}"*/, $"ReBeat_{characteristic.serializedName}",
				    $"ReBeat_{characteristic.compoundIdPartName}", characteristic.requires360Movement,
				    characteristic.containsRotationEvents, characteristic.sortingOrder);
		    }
	    }
    }
}