// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System.Collections.Generic;
using UnityEngine;

namespace DLD.Utility
{
	public static partial class ComponentUtil
	{
		public static void SetXPercent(this IReadOnlyList<RectTransform> r, float x)
		{
			for (int n = 0, len = r.Count; n < len; ++n)
			{
				r[n].SetXPercent(x);
			}
		}

		public static void SetPosition(this IReadOnlyList<RectTransform> r, Vector3 newPos)
		{
			for (int n = 0, len = r.Count; n < len; ++n)
			{
				r[n].position = newPos;
			}
		}

		public static void SetPosition(this IReadOnlyList<Transform> r, Vector3 newPos)
		{
			for (int n = 0, len = r.Count; n < len; ++n)
			{
				r[n].position = newPos;
			}
		}

		public static void SetXPercent(this RectTransform r, float x)
		{
			var anchorMin = r.anchorMin;
			anchorMin.x = x;
			r.anchorMin = anchorMin;

			var anchorMax = r.anchorMax;
			anchorMax.x = x;
			r.anchorMax = anchorMax;

			var anchoredPosition = r.anchoredPosition;
			anchoredPosition.x = 0;
			r.anchoredPosition = anchoredPosition;
		}

		public static void SetYPercent(this RectTransform r, float y)
		{
			var anchorMin = r.anchorMin;
			anchorMin.y = y;
			r.anchorMin = anchorMin;

			var anchorMax = r.anchorMax;
			anchorMax.y = y;
			r.anchorMax = anchorMax;

			var anchoredPosition = r.anchoredPosition;
			anchoredPosition.y = 0;
			r.anchoredPosition = anchoredPosition;
		}

		public static void SetColor(this IReadOnlyList<UnityEngine.UI.Graphic> g, Color newColor)
		{
			for (int n = 0, len = g.Count; n < len; ++n)
			{
				g[n].color = newColor;
			}
		}

		public static void SetAlpha(this IReadOnlyList<CanvasGroup> c, float a)
		{
			for (int n = 0, len = c.Count; n < len; ++n)
			{
				c[n].alpha = a;
			}
		}

		public static void SetInteractable(this IReadOnlyList<CanvasGroup> c, bool b)
		{
			for (int n = 0, len = c.Count; n < len; ++n)
			{
				c[n].interactable = b;
			}
		}

		public static void SetBlocksRaycasts(this IReadOnlyList<CanvasGroup> c, bool b)
		{
			for (int n = 0, len = c.Count; n < len; ++n)
			{
				c[n].blocksRaycasts = b;
			}
		}

		/// <summary>
		/// Change the image's alpha, but keep its current rgb color value same.
		/// </summary>
		public static void SetAlpha(this UnityEngine.UI.Graphic g, float a)
		{
			Color c = g.color;
			g.color = new Color(c.r, c.g, c.b, a);
		}

		/// <summary>
		/// Change alpha of images, but keep their current rgb color value same.
		/// </summary>
		public static void SetAlpha(this IReadOnlyList<UnityEngine.UI.Graphic> g, float a)
		{
			for (int n = 0, len = g.Count; n < len; ++n)
			{
				g[n].SetAlpha(a);
			}
		}

		/// <summary>
		/// Change the image's rgb color, but keep its current alpha value same.
		/// </summary>
		public static void SetColorKeepAlpha(this UnityEngine.UI.Graphic g, Color newColor)
		{
			Color c = g.color;
			g.color = new Color(newColor.r, newColor.g, newColor.b, c.a);
		}

		/// <summary>
		/// Change rgb color of images, but keep their current alpha value same.
		/// </summary>
		public static void SetColorKeepAlpha(this IReadOnlyList<UnityEngine.UI.Graphic> g, Color newColor)
		{
			for (int n = 0, len = g.Count; n < len; ++n)
			{
				g[n].SetColorKeepAlpha(newColor);
			}
		}
	}
}