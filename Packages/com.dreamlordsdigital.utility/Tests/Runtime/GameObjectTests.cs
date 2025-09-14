// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using NUnit.Framework;
using UnityEngine;

namespace DLD.Utility.Tests
{

[TestFixture]
public class GameObjectTests
{
	[Test(Description = "Check that FindBitIndex returns correct value for assigning layers.")]
	public void FindBitIndex_WhenUsedForLayerToName_ReturnsCorrectValue()
	{
		// We'll only test the built-in layers, since they have the same name on all projects.

		const int FirstBitSet = 0b00000001;
		Assert.AreEqual("Default", LayerMask.LayerToName(FirstBitSet.FindBitIndex()));

		const int SecondBitSet = 0b00000010;
		Assert.AreEqual("TransparentFX", LayerMask.LayerToName(SecondBitSet.FindBitIndex()));

		const int ThirdBitSet = 0b00000100;
		Assert.AreEqual("Ignore Raycast", LayerMask.LayerToName(ThirdBitSet.FindBitIndex()));

		const int FifthBitSet = 0b00010000;
		Assert.AreEqual("Water", LayerMask.LayerToName(FifthBitSet.FindBitIndex()));

		const int SixthBitSet = 0b00100000;
		Assert.AreEqual("UI", LayerMask.LayerToName(SixthBitSet.FindBitIndex()));
	}
}

}
