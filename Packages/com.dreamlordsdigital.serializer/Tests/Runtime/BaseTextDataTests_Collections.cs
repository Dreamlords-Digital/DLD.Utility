// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using UnityEngine;
using NUnit.Framework;

namespace DLD.Serializer.Tests
{

public abstract partial class BaseTextDataTests
{
	[Test(Description = "Ensure a HashSet<string> gets serialized/deserialized.")]
	public void FromSerializedString_ForHashSet_DeserializesProperly()
	{
		var collections = new CollectionsClass();

		collections.HashSet.Add("Twilight");
		collections.HashSet.Add("Princess");

		string serialized = _textDataIO.ToSerializedString(collections);
		Debug.Log(serialized);

		collections.HashSet.Remove("Twilight");
		collections.HashSet.Add("Peach");

		var deserialized = _textDataIO.FromSerializedString<CollectionsClass>(serialized);

		// -----------------------------------------------

		Assert.NotNull(deserialized);

		Assert.IsTrue(deserialized.HashSet.Contains("Twilight"));
		Assert.IsFalse(collections.HashSet.Contains("Twilight"));

		Assert.IsTrue(collections.HashSet.Contains("Peach"));
		Assert.IsFalse(deserialized.HashSet.Contains("Peach"));
	}

	[Test(Description = "Ensure a Dictionary<string, int> gets serialized/deserialized.")]
	public void FromSerializedString_ForDictionary_DeserializesProperly()
	{
		var collections = new CollectionsClass();

		collections.Dictionary.Add("Twilight", 2343);
		collections.Dictionary.Add("Princess", 1224);

		string serialized = _textDataIO.ToSerializedString(collections);
		Debug.Log(serialized);

		collections.Dictionary.Remove("Princess");
		collections.Dictionary.Add("Peach", 543);

		var deserialized = _textDataIO.FromSerializedString<CollectionsClass>(serialized);

		// -----------------------------------------------

		Assert.NotNull(deserialized);
	}
}

}
