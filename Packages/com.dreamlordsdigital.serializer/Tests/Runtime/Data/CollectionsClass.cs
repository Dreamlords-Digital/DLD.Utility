// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;
using System.Collections.Generic;

namespace DLD.Serializer.Tests
{

[Serializable]
public class CollectionsClass : ITextData
{
	public HashSet<string> HashSet = new();
	public Dictionary<string, int> Dictionary = new();

	public void PostLoad(string fullPath, string filename)
	{
	}

	public void PrepareSave()
	{
	}
}

}
