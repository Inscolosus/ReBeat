using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ReBeat.Utils
{
	public static class TextureUtils
	{
		public static Texture2D DuplicateTexture(Sprite source)
		{
			Plugin.Log.Info($"{source.name}: {source.textureRect}");
			float width = source.texture.width;
			float height = source.texture.height;
			float cropWidth = source.textureRect.width;
			float cropHeight = source.textureRect.height;
			float cropX = source.textureRect.x;
			float cropY = source.texture.height - source.textureRect.y - cropHeight;
			RenderTexture renderTex = RenderTexture.GetTemporary(
				(int)width,
				(int)height,
				0,
				RenderTextureFormat.Default,
				RenderTextureReadWrite.Linear);

			Graphics.Blit(source.texture, renderTex);
			RenderTexture previous = RenderTexture.active;
			RenderTexture.active = renderTex;

			Texture2D readableText = new Texture2D((int)cropWidth, (int)cropHeight);
			readableText.ReadPixels(new Rect(cropX, cropY, cropWidth, cropHeight), 0, 0);
			readableText.Apply();

			RenderTexture.active = previous;
			RenderTexture.ReleaseTemporary(renderTex);
			return readableText;
		}

		static Color LerpColor(Color c1, Color c2, float value)
		{
			return new Color(c1.r + (c2.r - c1.r) * value, c1.g + (c2.g - c1.g) * value, c1.b + (c2.b - c1.b) * value, c1.a + (c2.a - c1.a) * value);
		}

		static Color[] ScaleTexture(Texture2D texture, int width, int height)
		{
			var origColors = texture.GetPixels();
			Color[] destColors = new Color[width * height];

			int origWidth = texture.width;
			float ratioX = (texture.width - 1) / (float)width;
			float ratioY = (texture.height - 1) / (float)height;

			for (int destY = 0; destY < height; destY++)
			{
				int origY = (int)(destY * ratioY);
				float yLerp = destY * ratioY - origY;

				float yIdx1 = origY * origWidth;
				float yIdx2 = (origY + 1) * origWidth;
				float yIdxDest = destY * width;

				for (int destX = 0; destX < width; destX++)
				{
					int origX = (int)(destX * ratioX);
					float xLerp = destX * ratioX - origX;
					destColors[(int)(yIdxDest + destX)] = LerpColor(
						LerpColor(origColors[(int)(yIdx1 + origX)], origColors[(int)(yIdx1 + origX) + 1], xLerp),
						LerpColor(origColors[(int)(yIdx2 + origX)], origColors[(int)(yIdx2 + origX) + 1], xLerp),
						yLerp
					);
				}
			}
			return destColors;
		}

		public static Texture2D MergeTextures(Texture2D[] textures)
		{
			const int finalHeight = 512; // Fixed height of 512 pixels
			int totalWidth = CalculateTotalWidth(textures, finalHeight);

			Texture2D result = new Texture2D(totalWidth, finalHeight);
			Color[] finalColors = new Color[totalWidth * finalHeight];

			int currentX = 0;
			foreach (Texture2D texture in textures)
			{
				int width = Mathf.RoundToInt((float)finalHeight * texture.width / texture.height);
				int height = finalHeight;

				Color[] scaledTextureColors = ScaleTexture(texture, width, height);

				for (int y = 0; y < height; y++)
				{
					for (int x = 0; x < width; x++)
					{
						int destIndex = y * totalWidth + currentX + x;
						if (destIndex < finalColors.Length)
						{
							finalColors[destIndex] = scaledTextureColors[y * width + x];
						}
					}
				}

				currentX += width;
			}

			result.SetPixels(finalColors);
			result.Apply();

			return result;
		}

		static int CalculateTotalWidth(Texture2D[] textures, int targetHeight)
		{
			int totalWidth = 0;
			foreach (Texture2D texture in textures)
			{
				int width = Mathf.RoundToInt((float)targetHeight * texture.width / texture.height);
				totalWidth += width;
			}
			return totalWidth;
		}
	}
}
