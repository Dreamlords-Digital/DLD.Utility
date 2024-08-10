// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System.Globalization;
using UnityEngine;

namespace DLD.Utility
{
	public static class ColorUtil
	{
		public static Color ChangeColorAlpha(Color colorToChange, float newAlpha)
		{
			Color result;
			result.r = colorToChange.r;
			result.g = colorToChange.g;
			result.b = colorToChange.b;
			result.a = newAlpha;
			return result;
		}

		public static Color32 GetRandomColor32()
		{
			byte r = (byte)UnityEngine.Random.Range(byte.MinValue, byte.MaxValue);
			byte g = (byte)UnityEngine.Random.Range(byte.MinValue, byte.MaxValue);
			byte b = (byte)UnityEngine.Random.Range(byte.MinValue, byte.MaxValue);

			return new Color32(r, g, b, byte.MaxValue);
		}

		public static Color32 GetRandomColor32FromHSB()
		{
			var newColor = new HSBColor(
				UnityEngine.Random.value,
				UnityEngine.Random.Range(0.5f, 1.0f),
				UnityEngine.Random.Range(0.5f, 1.0f));

			return newColor.ToColor();
		}

		// ----------------------------------------------------------------------------------------------
		// color to hex string conversion
		// from http://wiki.unity3d.com/index.php?title=HexConverter

		// Note that Color32 and Color implicitly convert to each other. You may pass a Color object to this method without first casting it.
		public static string ColorToHex(this Color32 color)
		{
			return $"{color.r.ToString("X2")}{color.g.ToString("X2")}{color.b.ToString("X2")}";
		}

		public static string ColorWithAlphaToHex(this Color32 color)
		{
			return $"{color.r.ToString("X2")}{color.g.ToString("X2")}{color.b.ToString("X2")}{color.a.ToString("X2")}";
		}

		public static string ColorToNguiHex(this Color32 color)
		{
			return $"[{color.r.ToString("X2")}{color.g.ToString("X2")}{color.b.ToString("X2")}]";
		}

		public static Color HexToColor(this string hex)
		{
			byte r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
			byte g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
			byte b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
			return new Color32(r, g, b, 255);
		}

		public static bool IsDifferentFrom(this Color32 me, Color32 other)
		{
			return me.r != other.r ||
			       me.g != other.g ||
			       me.b != other.b ||
			       me.a != other.a;
		}
	}

}