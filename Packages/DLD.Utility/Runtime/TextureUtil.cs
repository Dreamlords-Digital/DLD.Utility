// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System.IO;
using UnityEngine;

namespace DLD.Utility
{
	public static class TextureUtil
	{
		/// <summary>
		/// Creates a new <see cref="Texture2D"/> given the raw image bytes.
		/// Uses <see cref="UnityEngine.ImageConversion.LoadImage(Texture2D, byte[], bool)"/> from <see cref="Texture2D"/>.
		/// </summary>
		public static bool SetTextureFromBytes(byte[] imageBytes, bool nonReadable, TextureWrapMode wrapMode,
			out Texture2D texture)
		{
			texture = new Texture2D(4, 4);
			texture.wrapMode = wrapMode;

			bool success = texture.LoadImage(imageBytes, nonReadable);

			if (!success)
			{
				return false;
			}

			texture.filterMode = FilterMode.Trilinear;
			texture.anisoLevel = 16;

			return true;
		}

		/// <summary>
		/// Load image from an absolute file path in the computer storage.
		/// Uses <see cref="UnityEngine.ImageConversion.LoadImage(Texture2D, byte[], bool)"/> from <see cref="Texture2D"/>.
		/// </summary>
		/// <param name="file"></param>
		/// <param name="forceReload"></param>
		public static Texture2D SetTextureFromFile(string file, bool forceReload = false)
		{
			Texture2D result;
			var registeredTexture = IoC.Resolve<Texture2D>(file);
			bool isNotNull = IoC.IsRegistered<Texture2D>(file) && registeredTexture != null;
			if (isNotNull && !forceReload)
			{
				result = registeredTexture;
			}
			else
			{
				// either not registered (first time loading),
				// or registered but we force reload

				if (isNotNull)
				{
#if UNITY_EDITOR
					if (!UnityEditor.EditorApplication.isPlaying)
						Object.DestroyImmediate(registeredTexture);
					else
#endif
						Object.Destroy(registeredTexture);

					IoC.Remove(file);
				}

				byte[] imageBytes = File.ReadAllBytes(file);
				bool success = SetTextureFromBytes(imageBytes, true, TextureWrapMode.Clamp, out result);
				if (success)
				{
					IoC.Register(result, file);
				}
				else
				{
					IoC.Remove(file);
				}
			}

			return result;
		}

		public static void SetTextureFromSimpleGrid(int size, out Texture2D texture)
		{
			SetTextureFromSimpleGrid(size, new Color(0.75f, 0.75f, 0.75f), out texture);
		}

		public static void SetTextureFromColor(Color32 fillColor, ref Texture2D texture)
		{
			if (texture == null)
			{
				texture = new Texture2D(16, 16);
			}

			for (int y = 0; y < texture.height; ++y)
			{
				for (int x = 0; x < texture.width; ++x)
				{
					texture.SetPixel(x, y, fillColor);
				}
			}

			texture.Apply();
		}

		public static void SetTextureFromSimpleGrid(int size, Color fillColor, out Texture2D texture)
		{
			texture = new Texture2D(size, size);

			for (int y = 0; y < size; ++y)
			{
				texture.SetPixel(0, y, Color.black);
				texture.SetPixel(size - 1, y, Color.black);

				for (int x = 0; x < size; ++x)
				{
					texture.SetPixel(x, 0, Color.black);

					texture.SetPixel(x, size - 1, Color.black);

					if (x > 0 && x < size - 1 && y > 0 && y < size - 1)
					{
						texture.SetPixel(x, y, fillColor);
					}
				}
			}

			texture.Apply();

			texture.filterMode = FilterMode.Trilinear;
			texture.anisoLevel = 16;
		}
	}
}