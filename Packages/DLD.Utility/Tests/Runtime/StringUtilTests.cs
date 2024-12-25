// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

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

		[Test]
		public void ConvertBackToForwardSlash_Works()
		{
			string text = "C:\\speech/test/proj\\Assets/Scripts\\Something.cs";
			Assert.AreEqual("C:/speech/test/proj/Assets/Scripts/Something.cs", text.ConvertBackToForwardSlash());
		}

		// ----------------------------------------------------

		[Test]
		public void PreviousWordHasLetters_HasLetters1stWord_ReturnsTrue()
		{
			string text = "Placeholder 2 Test Game";
			Assert.AreEqual(' ', text[11]);
			Assert.AreEqual(true, text.PreviousWordHasLetters(11));
		}

		[Test]
		public void PreviousWordHasLetters_HasLetters3rdWord_ReturnsTrue()
		{
			string text = "Placeholder 2 Test Game";
			Assert.AreEqual(' ', text[18]);
			Assert.AreEqual(true, text.PreviousWordHasLetters(18));
		}

		[Test]
		public void PreviousWordHasLetters_NoLetters1stWord_ReturnsFalse()
		{
			string text = "123123 2 Test Game";
			Assert.AreEqual(' ', text[6]);
			Assert.AreEqual(false, text.PreviousWordHasLetters(6));
		}

		[Test]
		public void PreviousWordHasLetters_NoLetters2ndWord_ReturnsFalse()
		{
			string text = "Placeholder 2 Test Game";
			Assert.AreEqual(' ', text[13]);
			Assert.AreEqual(false, text.PreviousWordHasLetters(13));
		}

		[Test]
		public void NextWordHasLetters_NoLetters_ReturnsFalse()
		{
			string text = "Placeholder 2 Test Game";
			Assert.AreEqual(' ', text[11]);
			Assert.AreEqual(false, text.NextWordHasLetters(11));
		}

		[Test]
		public void NextWordHasLetters_HasLetters_ReturnsTrue()
		{
			string text = "Placeholder 2 Test Game";
			Assert.AreEqual(' ', text[13]);
			Assert.AreEqual(true, text.NextWordHasLetters(13));
		}

		[Test]
		public void NextWordHasLetters_HasLettersFinalIdx_ReturnsTrue()
		{
			string text = "Placeholder 2 Test Game";
			Assert.AreEqual(' ', text[18]);
			Assert.AreEqual(true, text.NextWordHasLetters(18));
		}

		[Test]
		public void NextWordHasLetters_NoLettersFinalIdx_ReturnsFalse()
		{
			string text = "Placeholder 2";
			Assert.AreEqual(' ', text[11]);
			Assert.AreEqual(false, text.NextWordHasLetters(11));
		}

		[Test]
		public void GetNewlineCountUntilNextWord_WithNewlines_Works()
		{
			string text = "Placeholder 2\n\nTest Game";

			Assert.AreEqual(2, text.GetNewlineCountUntilNextWord(13));
		}

		[Test]
		public void GetWords_WithNumbers_ShouldNotSeparate()
		{
			string text = "Placeholder 2";

			using var words = IoC.GetFromPool<PooledList<(string word, byte newlines)>>();
			text.GetWords(words);

			Assert.AreEqual(1, words.Count);
			Assert.AreEqual("Placeholder 2", words[0].word);
			Assert.AreEqual(0, words[0].newlines);
		}

		[Test]
		public void GetWords_WithNumbers_Works()
		{
			string text = "Placeholder 2 Test Game";

			using var words = IoC.GetFromPool<PooledList<(string word, byte newlines)>>();
			text.GetWords(words);

			Assert.AreEqual(3, words.Count);

			Assert.AreEqual("Placeholder 2", words[0].word);
			Assert.AreEqual("Test", words[1].word);
			Assert.AreEqual("Game", words[2].word);

			Assert.AreEqual(0, words[0].newlines);
			Assert.AreEqual(0, words[1].newlines);
			Assert.AreEqual(0, words[2].newlines);
		}

		[Test]
		public void GetWords_WithTwoLetterWord_Works()
		{
			string text = "Wait between 15 to 30 seconds";

			using var words = IoC.GetFromPool<PooledList<(string word, byte newlines)>>();
			text.GetWords(words);

			Assert.AreEqual(4, words.Count);

			Assert.AreEqual("Wait", words[0].word);
			Assert.AreEqual("between 15", words[1].word);
			Assert.AreEqual("to 30", words[2].word);
			Assert.AreEqual("seconds", words[3].word);
		}

		[Test]
		public void GetWords_WithTwoLetterWord2_Works()
		{
			string text = "Move to random spot within 2 tiles";

			using var words = IoC.GetFromPool<PooledList<(string word, byte newlines)>>();
			text.GetWords(words);

			Assert.AreEqual(6, words.Count);

			Assert.AreEqual("Move", words[0].word);
			Assert.AreEqual("to", words[1].word);
			Assert.AreEqual("random", words[2].word);
			Assert.AreEqual("spot", words[3].word);
			Assert.AreEqual("within 2", words[4].word);
			Assert.AreEqual("tiles", words[5].word);
		}

		[Test]
		public void GetWords_WithAmpersand_Works()
		{
			string text = "Is near & has line-of-sight to a living player character by 15 tiles or less?";

			using var words = IoC.GetFromPool<PooledList<(string word, byte newlines)>>();
			text.GetWords(words);

			Assert.AreEqual(14, words.Count);

			Assert.AreEqual("Is", words[0].word);
			Assert.AreEqual("near", words[1].word);
			Assert.AreEqual("&", words[2].word);
			Assert.AreEqual("has", words[3].word);
			Assert.AreEqual("line-of-sight", words[4].word);
			Assert.AreEqual("to", words[5].word);
			Assert.AreEqual("a", words[6].word);
			Assert.AreEqual("living", words[7].word);
			Assert.AreEqual("player", words[8].word);
			Assert.AreEqual("character", words[9].word);
			Assert.AreEqual("by 15", words[10].word);
			Assert.AreEqual("tiles", words[11].word);
			Assert.AreEqual("or", words[12].word);
			Assert.AreEqual("less?", words[13].word);

			Assert.AreEqual(0, words[0].newlines);
			Assert.AreEqual(0, words[1].newlines);
			Assert.AreEqual(0, words[2].newlines);
			Assert.AreEqual(0, words[3].newlines);
			Assert.AreEqual(0, words[4].newlines);
			Assert.AreEqual(0, words[5].newlines);
			Assert.AreEqual(0, words[6].newlines);
			Assert.AreEqual(0, words[7].newlines);
			Assert.AreEqual(0, words[8].newlines);
			Assert.AreEqual(0, words[9].newlines);
			Assert.AreEqual(0, words[10].newlines);
			Assert.AreEqual(0, words[11].newlines);
			Assert.AreEqual(0, words[12].newlines);
			Assert.AreEqual(0, words[13].newlines);
		}

		[Test]
		public void GetWords_WithNewlines_Works()
		{
			string text = "Placeholder 2\n\nTest Game";

			using var words = IoC.GetFromPool<PooledList<(string word, byte newlines)>>();
			text.GetWords(words);

			Assert.AreEqual(3, words.Count);

			Assert.AreEqual("Placeholder 2", words[0].word);
			Assert.AreEqual("Test", words[1].word);
			Assert.AreEqual("Game", words[2].word);

			Assert.AreEqual(2, words[0].newlines);
			Assert.AreEqual(0, words[1].newlines);
			Assert.AreEqual(0, words[2].newlines);
		}

		[Test]
		public void GetWords_WithNewline_Works()
		{
			string text = "Placeholder 3\n2nd Line";

			using var words = IoC.GetFromPool<PooledList<(string word, byte newlines)>>();
			text.GetWords(words);

			Assert.AreEqual(3, words.Count);

			Assert.AreEqual("Placeholder 3", words[0].word);
			Assert.AreEqual("2nd", words[1].word);
			Assert.AreEqual("Line", words[2].word);

			Assert.AreEqual(1, words[0].newlines);
			Assert.AreEqual(0, words[1].newlines);
			Assert.AreEqual(0, words[2].newlines);
		}

		[Test]
		public void GetWords_WithNumbersWhiteSpaceAtEnd_Works()
		{
			string text = "Placeholder 2 Test Game     ";

			using var words = IoC.GetFromPool<PooledList<(string word, byte newlines)>>();
			text.GetWords(words);

			Assert.AreEqual(3, words.Count);

			Assert.AreEqual("Placeholder 2", words[0].word);
			Assert.AreEqual("Test", words[1].word);
			Assert.AreEqual("Game", words[2].word);

			Assert.AreEqual(0, words[0].newlines);
			Assert.AreEqual(0, words[1].newlines);
			Assert.AreEqual(0, words[2].newlines);
		}

		[Test]
		public void GetWords_WithNumbersWhiteSpaceAtStart_Works()
		{
			string text = "         Placeholder 2 Test Game";

			using var words = IoC.GetFromPool<PooledList<(string word, byte newlines)>>();
			text.GetWords(words);

			Assert.AreEqual(3, words.Count);

			Assert.AreEqual("Placeholder 2", words[0].word);
			Assert.AreEqual("Test", words[1].word);
			Assert.AreEqual("Game", words[2].word);

			Assert.AreEqual(0, words[0].newlines);
			Assert.AreEqual(0, words[1].newlines);
			Assert.AreEqual(0, words[2].newlines);
		}

		[Test]
		public void GetWords_WithNumbersWhiteSpace_Works()
		{
			string text = "         Placeholder      2   Test  Game    ";

			using var words = IoC.GetFromPool<PooledList<(string word, byte newlines)>>();
			text.GetWords(words);

			Assert.AreEqual(3, words.Count);

			Assert.AreEqual("Placeholder 2", words[0].word);
			Assert.AreEqual("Test", words[1].word);
			Assert.AreEqual("Game", words[2].word);

			Assert.AreEqual(0, words[0].newlines);
			Assert.AreEqual(0, words[1].newlines);
			Assert.AreEqual(0, words[2].newlines);
		}
	}
}