// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;
using System.Globalization;
using UnityEngine;

namespace DLD.Utility
{

/// <summary>
/// Color expressed as normalized values of Hue, Saturation, and Brightness.
/// </summary>
[Serializable]
public struct HSBColor
{
	/// <summary>
	/// Hue
	/// </summary>
	/// <remarks>
	/// The range of hues is basically, think of the colors of the rainbow.
	/// From 0 to 1:
	/// Red - Violet - Indigo - Blue - Green - Yellow - Orange - Red
	/// </remarks>
	public float H;

	/// <summary>
	/// Saturation
	/// </summary>
	/// <remarks>
	/// 0 = grayscale<br/>
	/// 1 = current hue in its "purest"
	/// </remarks>
	public float S;

	/// <summary>
	/// Brightness
	/// </summary>
	/// <remarks>
	/// 0 = black<br/>
	/// 1 = color at its brightest
	/// </remarks>
	public float B;

	/// <summary>
	/// Alpha/Transparency
	/// </summary>
	public float A;

	public HSBColor(float h, float s, float b, float a)
	{
		H = h;
		S = s;
		B = b;
		A = a;
	}

	public HSBColor(float h, float s, float b)
	{
		H = h;
		S = s;
		B = b;
		A = 1f;
	}

	public HSBColor(Color col)
	{
		HSBColor temp = FromColor(col);
		H = temp.H;
		S = temp.S;
		B = temp.B;
		A = temp.A;
	}

	public static HSBColor FromColor(Color color)
	{
		HSBColor ret;
		ret.H = 0;
		ret.S = 0;
		ret.B = 0;
		ret.A = color.a;

		float r = color.r;
		float g = color.g;
		float b = color.b;

		float max = Mathf.Max(r, Mathf.Max(g, b));

		if (max <= 0)
		{
			return ret;
		}

		float min = Mathf.Min(r, Mathf.Min(g, b));
		float dif = max - min;

		if (max > min)
		{
			if (Mathf.Approximately(g, max))
			{
				ret.H = (b - r) / dif * 60f + 120f;
			}
			else if (Mathf.Approximately(b, max))
			{
				ret.H = (r - g) / dif * 60f + 240f;
			}
			else if (b > g)
			{
				ret.H = (g - b) / dif * 60f + 360f;
			}
			else
			{
				ret.H = (g - b) / dif * 60f;
			}

			if (ret.H < 0)
			{
				ret.H = ret.H + 360f;
			}
		}
		else
		{
			ret.H = 0;
		}

		ret.H *= 1f / 360f;
		ret.S = (dif / max) * 1f;
		ret.B = max;

		return ret;
	}

	public Color ToColor()
	{
		float red = B;
		float grn = B;
		float blu = B;
		if (S != 0)
		{
			float max = B;
			float dif = B * S;
			float min = B - dif;

			float h360 = H * 360f;

			if (h360 < 60f)
			{
				red = max;
				grn = h360 * dif / 60f + min;
				blu = min;
			}
			else if (h360 < 120f)
			{
				red = -(h360 - 120f) * dif / 60f + min;
				grn = max;
				blu = min;
			}
			else if (h360 < 180f)
			{
				red = min;
				grn = max;
				blu = (h360 - 120f) * dif / 60f + min;
			}
			else if (h360 < 240f)
			{
				red = min;
				grn = -(h360 - 240f) * dif / 60f + min;
				blu = max;
			}
			else if (h360 < 300f)
			{
				red = (h360 - 240f) * dif / 60f + min;
				grn = min;
				blu = max;
			}
			else if (h360 <= 360f)
			{
				red = max;
				grn = min;
				blu = -(h360 - 360f) * dif / 60 + min;
			}
			else
			{
				red = 0;
				grn = 0;
				blu = 0;
			}
		}

		Color c;
		c.r = Mathf.Clamp01(red);
		c.g = Mathf.Clamp01(grn);
		c.b = Mathf.Clamp01(blu);
		c.a = A;
		return c;
	}

	public override string ToString()
	{
		return ToString("0.000");
	}

	public string ToString(string format)
	{
		return string.Format("HSB({0}, {1}, {2})", H.ToString(format, CultureInfo.InvariantCulture),
			S.ToString(format, CultureInfo.InvariantCulture), B.ToString(format, CultureInfo.InvariantCulture));
	}

	public static HSBColor Lerp(HSBColor a, HSBColor b, float t)
	{
		float h, s;

		//check special case black (color.b==0): interpolate neither hue nor saturation!
		//check special case grey (color.s==0): don't interpolate hue!
		if (a.B == 0)
		{
			h = b.H;
			s = b.S;
		}
		else if (b.B == 0)
		{
			h = a.H;
			s = a.S;
		}
		else
		{
			if (a.S == 0)
			{
				h = b.H;
			}
			else if (b.S == 0)
			{
				h = a.H;
			}
			else
			{
				// works around bug with LerpAngle
				float angle = Mathf.LerpAngle(a.H * 360f, b.H * 360f, t);
				while (angle < 0f)
				{
					angle += 360f;
				}

				while (angle > 360f)
				{
					angle -= 360f;
				}

				h = angle / 360f;
			}

			s = Mathf.Lerp(a.S, b.S, t);
		}

		HSBColor ret;
		ret.H = h;
		ret.S = s;
		ret.B = Mathf.Lerp(a.B, b.B, t);
		ret.A = Mathf.Lerp(a.A, b.A, t);
		return ret;
	}
}

}
