using NUnit.Framework;

namespace DLD.Utility.Tests
{
	[TestFixture]
	public class StringUtilTests
	{
		[SetUp]
		public void SetUp()
		{
		}

		// ----------------------------------------------------

		[Test]
		public void IsSameWith_EmptyAgainstNull_Works()
		{
			// this is expected
			Assert.IsTrue(string.Equals("", "", System.StringComparison.Ordinal));
			Assert.IsTrue("".IsSameWith(""));

			// "" and string.Empty are same
			Assert.IsTrue(string.Equals("", string.Empty, System.StringComparison.Ordinal));
			Assert.IsTrue("".IsSameWith(string.Empty));

			// "" and null are not same
			Assert.IsFalse(string.Equals("", null, System.StringComparison.Ordinal));
			// but IsSameWith will treat them as same
			Assert.IsTrue("".IsSameWith(null));
		}

		[Test]
		public void IsSameWith_NullAgainstEmpty_Works()
		{
			string nullString = null;

			// they're both null
			Assert.IsTrue(string.Equals(nullString, null, System.StringComparison.Ordinal));
			Assert.IsTrue(nullString.IsSameWith(null));

			// null and "" are not same
			Assert.IsFalse(string.Equals(nullString, "", System.StringComparison.Ordinal));
			// but IsSameWith will treat them as same
			Assert.IsTrue(nullString.IsSameWith(""));

			// null and string.Empty are not same
			Assert.IsFalse(string.Equals(nullString, string.Empty, System.StringComparison.Ordinal));
			// but IsSameWith will treat them as same
			Assert.IsTrue(nullString.IsSameWith(string.Empty));
		}

		// ----------------------------------------------------

		[Test]
		public void AddSpacesToSentence_Works()
		{
			string text = "ItIsWhatItIs";
			Assert.AreEqual("It Is What It Is", text.AddSpacesToSentence());
		}

		[Test]
		public void AddSpacesToSentence_WithAcronymPreserved_Works()
		{
			string text = "ATMMachineActivated";
			Assert.AreEqual("ATM Machine Activated", text.AddSpacesToSentence());
		}

		[Test]
		public void AddSpacesToSentence_AcronymNotPreserved_Works()
		{
			string text = "ATMMachineActivated";
			Assert.AreEqual("ATMMachine Activated", text.AddSpacesToSentence(false));
		}

		// ----------------------------------------------------

		[Test]
		public void RemoveFromStart_WithIdxAdjust_Works()
		{
			string text = "C:/speech/test/proj/Assets/Scripts/Something.cs";
			Assert.AreEqual("Assets/Scripts/Something.cs", text.RemoveFromStart("/Assets/", 1));
		}

		[Test]
		public void RemoveFromStart_SearchTextNotFound_ReturnsSameString()
		{
			string text = "C:/speech/test/proj/Assets/Scripts/Something.cs";
			Assert.AreEqual(text, text.RemoveFromStart("NOT"));
		}
	}
}