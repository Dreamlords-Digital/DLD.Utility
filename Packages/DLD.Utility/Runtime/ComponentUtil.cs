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

		public static void SetEnabled(this IReadOnlyList<Behaviour> b, bool enabled)
		{
			for (int n = 0, len = b.Count; n < len; ++n)
			{
				if (b[n] == null)
				{
#if UNITY_EDITOR
					Debug.LogWarning($"Can't SetEnabled Behaviour #{n+1}, null");
#endif
					continue;
				}
				b[n].enabled = enabled;
			}
		}

		public static bool AreAllEnabled(this IReadOnlyList<Behaviour> b)
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

		public static bool IsAnyEnabled(this IReadOnlyList<Behaviour> b)
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

		public static void SetPositionXY(this IReadOnlyList<Transform> r, Vector3 pos)
		{
			for (int n = 0, len = r.Count; n < len; ++n)
			{
				var newPos = r[n].position;
				newPos.x = pos.x;
				newPos.y = pos.y;
				r[n].position = newPos;
			}
		}

		public static void SetPositionXZ(this IReadOnlyList<Transform> r, Vector3 pos)
		{
			for (int n = 0, len = r.Count; n < len; ++n)
			{
				var newPos = r[n].position;
				newPos.x = pos.x;
				newPos.z = pos.z;
				r[n].position = newPos;
			}
		}

		public static void SetPositionYZ(this IReadOnlyList<Transform> r, Vector3 pos)
		{
			for (int n = 0, len = r.Count; n < len; ++n)
			{
				var newPos = r[n].position;
				newPos.y = pos.y;
				newPos.z = pos.z;
				r[n].position = newPos;
			}
		}

		public static void SetLocalPositionXY(this IReadOnlyList<Transform> r, Vector3 pos)
		{
			for (int n = 0, len = r.Count; n < len; ++n)
			{
				var newPos = r[n].localPosition;
				newPos.x = pos.x;
				newPos.y = pos.y;
				r[n].localPosition = newPos;
			}
		}

		public static void SetLocalPositionXZ(this IReadOnlyList<Transform> r, Vector3 pos)
		{
			for (int n = 0, len = r.Count; n < len; ++n)
			{
				var newPos = r[n].localPosition;
				newPos.x = pos.x;
				newPos.z = pos.z;
				r[n].localPosition = newPos;
			}
		}

		public static void SetLocalPositionYZ(this IReadOnlyList<Transform> r, Vector3 pos)
		{
			for (int n = 0, len = r.Count; n < len; ++n)
			{
				var newPos = r[n].localPosition;
				newPos.y = pos.y;
				newPos.z = pos.z;
				r[n].localPosition = newPos;
			}
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

		/// <summary>
		/// Get the filename out of the scenePath without the extension type.
		/// </summary>
		/// <remarks>
		/// Given "Assets/Scenes/Others/Scene1.unity", this will return "Scene1".
		/// </remarks>
		public static string GetSceneNameFromPath(this string scenePath)
		{
			int begin = scenePath.LastIndexOf("/", StringComparison.Ordinal) + 1;
			int end = scenePath.LastIndexOf(".", StringComparison.Ordinal);

			if (begin == -1)
			{
				begin = 0;
			}

			if (end == -1)
			{
				end = scenePath.Length;
			}

			return scenePath.Substring(begin, end - begin);
		}

		/// <summary>
		/// Get the scene path that is suitable for use with <see cref="UnityEngine.SceneManagement.SceneManager.GetSceneByName"/>.
		/// It will remove any "Assets/" in the beginning, and ".unity" at the end.
		/// </summary>
		/// <remarks>
		/// Given "Assets/Scenes/Others/Scene1.unity", this will return "Scenes/Others/Scene1".
		/// </remarks>
		public static string GetScenePartialNameFromPath(this string scenePath)
		{
			if (scenePath.StartsWith("Assets/"))
			{
				int assetsLength = "Assets/".Length;
				scenePath = scenePath.Substring(assetsLength);
			}
			if (scenePath.EndsWith(".unity"))
			{
				int extensionTypeLength = ".unity".Length;
				scenePath = scenePath.Substring(0, scenePath.Length - extensionTypeLength);
			}

			return scenePath;
		}
	}
}
