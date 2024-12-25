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

			const int FIRST_BIT_SET = 0b00000001;
			Assert.AreEqual("Default", LayerMask.LayerToName(FIRST_BIT_SET.FindBitIndex()));

			const int SECOND_BIT_SET = 0b00000010;
			Assert.AreEqual("TransparentFX", LayerMask.LayerToName(SECOND_BIT_SET.FindBitIndex()));

			const int THIRD_BIT_SET = 0b00000100;
			Assert.AreEqual("Ignore Raycast", LayerMask.LayerToName(THIRD_BIT_SET.FindBitIndex()));

			const int FIFTH_BIT_SET = 0b00010000;
			Assert.AreEqual("Water", LayerMask.LayerToName(FIFTH_BIT_SET.FindBitIndex()));

			const int SIXTH_BIT_SET = 0b00100000;
			Assert.AreEqual("UI", LayerMask.LayerToName(SIXTH_BIT_SET.FindBitIndex()));
		}
	}
}