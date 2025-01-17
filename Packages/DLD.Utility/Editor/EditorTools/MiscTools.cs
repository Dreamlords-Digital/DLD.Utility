// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using UnityEditor;
using UnityEngine;

namespace DLD.Utility.Editor
{
	public class MiscTools
	{
		[MenuItem("GameObject/DLD/Reset Animator Pose")]
		public static void ResetAnimatorPose()
		{
			var g = Selection.activeGameObject;
			if (g == null)
			{
				return;
			}

			var skinnedMeshRenderers = g.GetComponentsInChildren<SkinnedMeshRenderer>();
			for (int i = 0; i < skinnedMeshRenderers.Length; i++)
			{
				skinnedMeshRenderers[i].RestoreBindPose();
			}
		}
	}
}