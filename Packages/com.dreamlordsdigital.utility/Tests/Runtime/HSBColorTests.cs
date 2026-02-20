// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using NUnit.Framework;
using UnityEngine;

namespace DLD.Utility.Tests
{

[TestFixture]
public class HSBColorTests
{
	[Test(Description = "Test that conversion between HSBColor and Color works.")]
	public void HSBColorFromToColor_Works()
	{
		Color rgbColor1 = new Color(0.4f, 1f, 0.84f, 1f);
		HSBColor hsbColor1 = HSBColor.FromColor(rgbColor1);
		Assert.AreEqual(rgbColor1.ToString(), hsbColor1.ToColor().ToString());

		Color rgbColor2 = new Color(0.643137f, 0.321568f, 0.329411f, 0.5f);
		HSBColor hsbColor2 = HSBColor.FromColor(rgbColor2);
		Assert.AreEqual(rgbColor2.ToString(), hsbColor2.ToColor().ToString());
	}
}

}
