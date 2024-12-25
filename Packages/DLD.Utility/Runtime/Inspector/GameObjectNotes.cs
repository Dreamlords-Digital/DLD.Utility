// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using UnityEngine;

namespace DLD.Utility.Inspector
{
	public class GameObjectNotes : MonoBehaviour
	{
#if UNITY_EDITOR
		[TextArea(1, 20)]
		public string Notes;
#endif
	}
}