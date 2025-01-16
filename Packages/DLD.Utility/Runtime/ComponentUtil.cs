// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace DLD.Utility
{
	public static partial class ComponentUtil
	{
		public static void SetActive(this IReadOnlyList<GameObject> gameObjects, bool active)
		{
			for (int n = 0, len = gameObjects.Count; n < len; ++n)
			{
				if (gameObjects[n] == null)
				{
#if UNITY_EDITOR
					Debug.LogWarning($"Can't deactivate GameObject #{n+1}, null");
#endif
					continue;
				}
#if UNITY_EDITOR
				Debug.Log($"Deactivate GameObject #{n+1}: {gameObjects[n].name}", gameObjects[n]);
#endif
				gameObjects[n].SetActive(active);
			}
		}

		public static void SetActive(this IReadOnlyList<Component> c, bool active)
		{
			for (int n = 0, len = c.Count; n < len; ++n)
			{
				if (c[n] == null)
				{
#if UNITY_EDITOR
					Debug.LogWarning($"Can't SetActive GameObject #{n+1}, null");
#endif
					continue;
				}
#if UNITY_EDITOR
				Debug.Log($"SetActive GameObject #{n+1}: {c[n].name}", c[n]);
#endif
				c[n].gameObject.SetActive(active);
			}
		}

		public static bool AreAllActiveSelf(this IReadOnlyList<Component> c)
		{
			for (int n = 0, len = c.Count; n < len; ++n)
			{
				if (c[n] == null)
				{
					continue;
				}

				if (!c[n].gameObject.activeSelf)
				{
					return false;
				}
			}

			return true;
		}

		public static bool IsAnyActiveSelf(this IReadOnlyList<Component> c)
		{
			for (int n = 0, len = c.Count; n < len; ++n)
			{
				if (c[n] == null)
				{
					continue;
				}

				if (c[n].gameObject.activeSelf)
				{
					return true;
				}
			}

			return false;
		}

		public static void SetEnabled(this IReadOnlyList<MonoBehaviour> b, bool enabled)
		{
			for (int n = 0, len = b.Count; n < len; ++n)
			{
				if (b[n] == null)
				{
#if UNITY_EDITOR
					Debug.LogWarning($"Can't SetEnabled MonoBehaviour #{n+1}, null");
#endif
					continue;
				}
				b[n].enabled = enabled;
			}
		}

		public static bool AreAllEnabled(this IReadOnlyList<MonoBehaviour> b)
		{
			for (int n = 0, len = b.Count; n < len; ++n)
			{
				if (b[n] == null)
				{
					continue;
				}

				if (!b[n].enabled)
				{
					return false;
				}
			}

			return true;
		}

		public static bool IsAnyEnabled(this IReadOnlyList<MonoBehaviour> b)
		{
			for (int n = 0, len = b.Count; n < len; ++n)
			{
				if (b[n] == null)
				{
					continue;
				}

				if (b[n].enabled)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Is GameObject tagged with any of the tags in the list?
		/// </summary>
		/// <param name="self"></param>
		/// <param name="tags"></param>
		/// <returns></returns>
		public static bool CompareTag(this GameObject self, IReadOnlyList<string> tags)
		{
			if (tags == null)
			{
				return false;
			}

			for (int n = 0; n < tags.Count; ++n)
			{
				if (self.CompareTag(tags[n]))
				{
					return true;
				}
			}

			return false;
		}

		public static void DestroyChildren(this Transform transform)
		{
			for (int n = 0; n < transform.childCount; n++)
			{
				UnityEngine.Object.Destroy(transform.GetChild(n).gameObject);
			}
		}

		public static void SetLayer(this IReadOnlyList<Collider> colliders, LayerMask newLayer)
		{
			// Note: despite passing in a LayerMask, we're only interested in the first layer assigned in the mask
			int layerIntValue = newLayer.value.FindBitIndex();
			for (int n = 0, len = colliders.Count; n < len; ++n)
			{
				colliders[n].gameObject.layer = layerIntValue;
			}
		}

		public static void SetLayer(this IReadOnlyList<Collider> colliders, int newLayerValue)
		{
			for (int n = 0, len = colliders.Count; n < len; ++n)
			{
				colliders[n].gameObject.layer = newLayerValue;
			}
		}

		public static bool IsInLayer(this GameObject g, LayerMask layerMask)
		{
			return layerMask.value == (layerMask.value | (1 << g.layer));
		}

		public static bool IsInLayer(this Component c, LayerMask layerMask)
		{
			return layerMask.value == (layerMask.value | (1 << c.gameObject.layer));
		}

		public static bool IsLayerInMask(this byte layerIdx, LayerMask layerMask)
		{
			return layerMask.value == (layerMask.value | (1 << layerIdx));
		}

		public static string GetSceneNameFromPath(this string scenePath)
		{
			var begin = scenePath.LastIndexOf("/", StringComparison.Ordinal) + 1;
			var end = scenePath.LastIndexOf(".", StringComparison.Ordinal);
			return scenePath.Substring(begin, end - begin);
		}
	}
}