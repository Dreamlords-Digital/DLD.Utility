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

			var words = IoC.GetFromPool<PooledList<(string word, ushort newlines)>>();
			text.GetWords(words);

			Assert.AreEqual(1, words.Count);
			Assert.AreEqual("Placeholder 2", words[0].word);
			Assert.AreEqual(0, words[0].newlines);

			words.ReleaseToPool();
		}

		[Test]
		public void GetWords_WithNumbers_Works()
		{
			string text = "Placeholder 2 Test Game";

			var words = IoC.GetFromPool<PooledList<(string word, ushort newlines)>>();
			text.GetWords(words);

			Assert.AreEqual(3, words.Count);

			Assert.AreEqual("Placeholder 2", words[0].word);
			Assert.AreEqual("Test", words[1].word);
			Assert.AreEqual("Game", words[2].word);

			Assert.AreEqual(0, words[0].newlines);
			Assert.AreEqual(0, words[1].newlines);
			Assert.AreEqual(0, words[2].newlines);

			words.ReleaseToPool();
		}

		[Test]
		public void GetWords_WithNewlines_Works()
		{
			string text = "Placeholder 2\n\nTest Game";

			var words = IoC.GetFromPool<PooledList<(string word, ushort newlines)>>();
			text.GetWords(words);

			Assert.AreEqual(3, words.Count);

			Assert.AreEqual("Placeholder 2", words[0].word);
			Assert.AreEqual("Test", words[1].word);
			Assert.AreEqual("Game", words[2].word);

			Assert.AreEqual(2, words[0].newlines);
			Assert.AreEqual(0, words[1].newlines);
			Assert.AreEqual(0, words[2].newlines);

			words.ReleaseToPool();
		}

		[Test]
		public void GetWords_WithNewline_Works()
		{
			string text = "Placeholder 3\n2nd Line";

			var words = IoC.GetFromPool<PooledList<(string word, ushort newlines)>>();
			text.GetWords(words);

			Assert.AreEqual(3, words.Count);

			Assert.AreEqual("Placeholder 3", words[0].word);
			Assert.AreEqual("2nd", words[1].word);
			Assert.AreEqual("Line", words[2].word);

			Assert.AreEqual(1, words[0].newlines);
			Assert.AreEqual(0, words[1].newlines);
			Assert.AreEqual(0, words[2].newlines);

			words.ReleaseToPool();
		}

		[Test]
		public void GetWords_WithNumbersWhiteSpaceAtEnd_Works()
		{
			string text = "Placeholder 2 Test Game     ";

			var words = IoC.GetFromPool<PooledList<(string word, ushort newlines)>>();
			text.GetWords(words);

			Assert.AreEqual(3, words.Count);

			Assert.AreEqual("Placeholder 2", words[0].word);
			Assert.AreEqual("Test", words[1].word);
			Assert.AreEqual("Game", words[2].word);

			Assert.AreEqual(0, words[0].newlines);
			Assert.AreEqual(0, words[1].newlines);
			Assert.AreEqual(0, words[2].newlines);

			words.ReleaseToPool();
		}

		[Test]
		public void GetWords_WithNumbersWhiteSpaceAtStart_Works()
		{
			string text = "         Placeholder 2 Test Game";

			var words = IoC.GetFromPool<PooledList<(string word, ushort newlines)>>();
			text.GetWords(words);

			Assert.AreEqual(3, words.Count);

			Assert.AreEqual("Placeholder 2", words[0].word);
			Assert.AreEqual("Test", words[1].word);
			Assert.AreEqual("Game", words[2].word);

			Assert.AreEqual(0, words[0].newlines);
			Assert.AreEqual(0, words[1].newlines);
			Assert.AreEqual(0, words[2].newlines);

			words.ReleaseToPool();
		}

		[Test]
		public void GetWords_WithNumbersWhiteSpace_Works()
		{
			string text = "         Placeholder      2   Test  Game    ";

			var words = IoC.GetFromPool<PooledList<(string word, ushort newlines)>>();
			text.GetWords(words);

			Assert.AreEqual(3, words.Count);

			Assert.AreEqual("Placeholder 2", words[0].word);
			Assert.AreEqual("Test", words[1].word);
			Assert.AreEqual("Game", words[2].word);

			Assert.AreEqual(0, words[0].newlines);
			Assert.AreEqual(0, words[1].newlines);
			Assert.AreEqual(0, words[2].newlines);

			words.ReleaseToPool();
		}
	}
}