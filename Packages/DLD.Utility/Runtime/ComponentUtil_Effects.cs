// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System.Collections.Generic;
using UnityEngine;

namespace DLD.Utility
{
	public static partial class ComponentUtil
	{
		public static void SetEmit(this IReadOnlyList<ParticleSystem> particleSystems, bool emit)
		{
			for (int n = 0, len = particleSystems.Count; n < len; ++n)
			{
				var emission = particleSystems[n].emission;
				emission.enabled = emit;
			}
		}

		public static void SetEmit(this ParticleSystem particleSystem, bool emit)
		{
			var emission = particleSystem.emission;
			emission.enabled = emit;
		}

		public static void SetEmitRateOverTime(this ParticleSystem particleSystem, float newMultiplier)
		{
			var emission = particleSystem.emission;
			emission.rateOverTimeMultiplier = newMultiplier;
		}

		public static void SetXYForceOverLifetime(this ParticleSystem particleSystem, float x, float y)
		{
			var forceOverLifetime = particleSystem.forceOverLifetime;
			forceOverLifetime.x = x;
			forceOverLifetime.y = y;
		}

		public static void SetXYForceOverLifetime(this ParticleSystem particleSystem, Vector2 force)
		{
			var forceOverLifetime = particleSystem.forceOverLifetime;
			forceOverLifetime.x = force.x;
			forceOverLifetime.y = force.y;
		}

		public static void SetSimulationSpeed(this ParticleSystem particleSystem, float speed)
		{
			var main = particleSystem.main;
			main.simulationSpeed = speed;
		}

		// ====================================================================================

		public static void SetEmit(this IReadOnlyList<TrailRenderer> trails, bool emit)
		{
			for (int n = 0, len = trails.Count; n < len; ++n)
			{
				trails[n].emitting = emit;
			}
		}

		public static void RemoveAllPoints(this IReadOnlyList<TrailRenderer> trails)
		{
			for (int n = 0, len = trails.Count; n < len; ++n)
			{
				trails[n].Clear();
			}
		}

		// ====================================================================================

		public static void Enable(this IReadOnlyList<Collider> colliders, bool enabled)
		{
			for (int n = 0, len = colliders.Count; n < len; ++n)
			{
				colliders[n].enabled = enabled;
			}
		}

		public static void Enable(this IReadOnlyList<Renderer> renderers, bool enable)
		{
			for (int n = 0, len = renderers.Count; n < len; ++n)
			{
				renderers[n].enabled = enable;
			}
		}

		// ====================================================================================

		public static void SetSharedMaterial(this IReadOnlyList<Renderer> renderers, Material material)
		{
			for (int n = 0, len = renderers.Count; n < len; ++n)
			{
				renderers[n].sharedMaterial = material;
			}
		}

		public static void SetSharedMaterial(this IReadOnlyList<Renderer> renderers, List<Material> materials)
		{
			for (int n = 0, len = renderers.Count; n < len; ++n)
			{
				renderers[n].sharedMaterial = materials[n];
			}
		}

		public static void SetFloat(this IReadOnlyList<Material> materials,
			int nameId, float newValue)
		{
			for (int n = 0, len = materials.Count; n < len; ++n)
			{
				materials[n].SetFloat(nameId, newValue);
			}
		}

		// ====================================================================================

		/// <summary>
		/// Reset bones of <see cref="SkinnedMeshRenderer"/> back to their original positions.
		/// </summary>
		/// <remarks>Based on https://forum.unity.com/threads/mesh-bindposes.383752/#post-2953960</remarks>
		public static void RestoreBindPose(this SkinnedMeshRenderer skinnedMeshRenderer)
		{
			bool GetBindPose(Transform bone, out Matrix4x4 bindPose)
			{
				for (int i = 0; i < skinnedMeshRenderer.bones.Length; i++)
				{
					if (skinnedMeshRenderer.bones[i] == bone)
					{
						bindPose = skinnedMeshRenderer.sharedMesh.bindposes[i];
						return true;
					}
				}

				bindPose = Matrix4x4.identity;
				return false;
			}

			for (int i = 0; i < skinnedMeshRenderer.bones.Length; i++)
			{
				var bone = skinnedMeshRenderer.bones[i];
				var bindPose = skinnedMeshRenderer.sharedMesh.bindposes[i];
				var parent = bone.parent;

				Matrix4x4 localMatrix;
				if (GetBindPose(parent, out Matrix4x4 parentBindPose))
				{
					localMatrix = (bindPose * parentBindPose.inverse).inverse;
				}
				else
				{
					localMatrix = bindPose.inverse;
				}

				// Recreate local transform from that matrix
				bone.localPosition = localMatrix.MultiplyPoint(Vector3.zero);
				bone.localRotation = Quaternion.LookRotation(
					localMatrix.GetColumn(2),
					localMatrix.GetColumn(1));
				bone.localScale = new Vector3(
					localMatrix.GetColumn(0).magnitude,
					localMatrix.GetColumn(1).magnitude,
					localMatrix.GetColumn(2).magnitude);
			}
		}
	}
}