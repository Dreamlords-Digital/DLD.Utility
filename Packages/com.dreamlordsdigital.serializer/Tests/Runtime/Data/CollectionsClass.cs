// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;
using System.Collections.Generic;

namespace DLD.Serializer.Tests
{

[Serializable]
public class CollectionsClass : ITextData
{
	public HashSet<string> HashSet = new();

	// Note: We are not using Unity's serializer for this,
	// but UAC1015 will complain so we put
	// [UnityEngine.SerializeField] here even though it's useless.
	[UnityEngine.SerializeField]
	public Dictionary<string, int> Dictionary = new();

	public void PostLoad(string fullPath, string filename)
	{
	}

	public void PrepareSave()
	{
	}
}

}
