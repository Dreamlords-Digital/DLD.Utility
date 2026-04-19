// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace DLD.Utility
{

public static class StringUtil
{
	public static string ToStringLabel(this object obj) => obj.ToString().AddSpacesToSentence();

	public static (string, string) ExtractHRef(this string stringText, bool onlyGetHttpsLinks = true, string replacementStartTag = null, string replacementEndTag = null)
	{
		if (string.IsNullOrEmpty(stringText) || !stringText.Contains("href"))
		{
			return (null, null);
		}

		// For example:
		// Go to <see href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings">Custom date and time format strings documentation</see> for more info.

		var text = stringText.AsSpan();

		int hrefIdx = text.IndexOf("href");
		if (hrefIdx == -1)
		{
			return (null, null);
		}

		// Before the "href", there ought to be a "<see".
		// We check "<" and "see" individually because whitespace is allowed between those two.
		var beforeHref = text[..hrefIdx].TrimEnd();
		if (!beforeHref.EndsWith("see"))
		{
			return (null, null);
		}

		var beforeSee = beforeHref[..^3].Trim();
		if (!beforeSee.EndsWith("<"))
		{
			return (null, null);
		}

		int beforeSeeIdx = text.IndexOf(beforeSee);
		var leftOfSee = text.Slice(beforeSeeIdx, beforeSeeIdx + beforeSee.Length - 1); // -1 because we don't want "<" to be included

		Span<char> formattedText = null;
		if (!string.IsNullOrEmpty(replacementStartTag) && !string.IsNullOrEmpty(replacementEndTag))
		{
			char[] buffer = new char[stringText.Length + replacementStartTag.Length + replacementEndTag.Length];
			formattedText = new Span<char>(buffer);
			leftOfSee.CopyTo(formattedText);
			replacementStartTag.AsSpan().CopyTo(formattedText.Slice(leftOfSee.Length));
		}

		text = text[(hrefIdx + "href".Length)..].TrimStart();
		if (!text.StartsWith("="))
		{
			return (null, null);
		}

		var linkText = text[1..].TrimStart();

		// We should now have the text without the equals and any whitespace between the equals and the starting quote.
		// The first character should now be either a double quote or single quote.

		ReadOnlySpan<char> restOfText;
		int endingQuoteIdx;
		if (linkText.StartsWith("\""))
		{
			linkText = linkText[1..].TrimStart();
			endingQuoteIdx = linkText.IndexOf("\"");
			restOfText = linkText;
			if (endingQuoteIdx != -1)
			{
				linkText = linkText[..endingQuoteIdx].Trim();
			}
		}
		else if (linkText.StartsWith("'"))
		{
			linkText = linkText[1..].TrimStart();
			endingQuoteIdx = linkText.IndexOf("'");
			restOfText = linkText;
			if (endingQuoteIdx != -1)
			{
				linkText = linkText[..endingQuoteIdx].Trim();
			}
		}
		else
		{
			// no quote?
			return (null, null);
		}

		if (!string.IsNullOrEmpty(replacementStartTag) && !string.IsNullOrEmpty(replacementEndTag))
		{
			int endingBracketIdx = restOfText.IndexOf(">");
			restOfText = restOfText.Slice(endingBracketIdx + 1);
			int endingSeeTagIdx = restOfText.IndexOf("</see>");
			var rightOfSee = restOfText.Slice(endingSeeTagIdx + 6);
			var linkLabel = restOfText.Slice(0, endingSeeTagIdx);
			linkLabel.CopyTo(formattedText.Slice(leftOfSee.Length + replacementStartTag.Length));
			replacementEndTag.AsSpan().CopyTo(formattedText.Slice(leftOfSee.Length + replacementStartTag.Length + linkLabel.Length));
			rightOfSee.CopyTo(formattedText.Slice(leftOfSee.Length + replacementStartTag.Length + linkLabel.Length + replacementEndTag.Length));
		}

		string foundUrl = linkText.ToString();
		if (Uri.TryCreate(foundUrl, UriKind.Absolute, out Uri uri) && uri.IsDefaultPort)
		{
			if (onlyGetHttpsLinks && uri.Scheme != Uri.UriSchemeHttps)
			{
				return (null, null);
			}

			return (uri.ToString(), formattedText.ToString());
		}

		// Uri.TryCreate failed
		return (null, null);
	}

	/// <summary>
	///    Equality checker for strings but will regard
	///    null value and an empty string as equivalent.
	///    For example, if first string is assigned a null
	///    value while the second string is assigned an
	///    empty string like "", then this method returns
	///    true (i.e. they are the same).
	/// </summary>
	public static bool IsSameWith(
		this string first, string second,
		StringComparison stringComparison = StringComparison.Ordinal)
	{
		if (string.IsNullOrEmpty(first) && string.IsNullOrEmpty(second))
		{
			// both are null/empty strings
			// we consider null and an empty string to be conceptually equivalent
			return true;
		}

		return string.Equals(first, second, stringComparison);
	}

	/// <summary>
	///    If the value is positive or zero, this will return the number with a plus symbol ahead.
	///    If the value is negative, this will return the number with a minus symbol ahead.
	/// </summary>
	/// <param name="me"></param>
	/// <returns></returns>
	public static string ToPlusMinusNumber(this int me)
	{
		const string Format = "+#;-#;0";

		return me.ToString(Format, CultureInfo.InvariantCulture);
	}

	/// <summary>
	///    If the value is positive, this will return the number with a plus symbol ahead.
	///    If the value is negative, this will return the number with a minus symbol ahead.
	///    If the value is zero, this will return a blank string.
	/// </summary>
	public static string ToPlusMinusNumberNoZero(this int me)
	{
		const string Format = "+#;-#; ";

		return me.ToString(Format, CultureInfo.InvariantCulture);
	}

	/// <summary>
	///    Returns either "Yes" or "No".
	/// </summary>
	public static string ToYesNo(this bool b)
	{
		return b ? "Yes" : "No";
	}

	/// <summary>
	///    Returns either "yes" or "no".
	/// </summary>
	public static string ToYesNoSmall(this bool b)
	{
		return b ? "yes" : "no";
	}

	/// <summary>
	///    After the first letter, add a space before every capital letter.
	/// </summary>
	/// <param name="text"></param>
	/// <param name="preserveAcronyms">When true, "WindowGUILabel" will become "Window GUI Label" instead of "Window GUILabel".</param>
	/// <param name="treatNumbersAsWords">When true, "Xbox360" will become "Xbox 360" instead of "Xbox360".</param>
	/// <returns></returns>
	public static string AddSpacesToSentence(this string text, bool preserveAcronyms = true, bool treatNumbersAsWords = true)
	{
		if (text == null)
		{
			return null;
		}

		if (text == string.Empty)
		{
			return string.Empty;
		}

		StringBuilder newText = new StringBuilder(text.Length * 2);
		newText.Append(text[0]); // first letter is always inserted as-is (we don't want to insert a space before the first letter)
		for (int i = 1; i < text.Length; i++)
		{
			if (char.IsUpper(text[i])) // current char is uppercase (or digit if treatNumbersAsWords is active)
			{
				// we want to insert a space before we insert this uppercase, but we need to check if it's the right situation to do so
				if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) || // previous char is not uppercase (this will be true if we are currently at 'S' of "CoolSpot")
				     (preserveAcronyms && char.IsUpper(text[i - 1]) && i < text.Length - 1 && !char.IsUpper(text[i + 1]) && (!treatNumbersAsWords || !char.IsDigit(text[i + 1])))) // previous char is uppercase and next char is lowercase or some other symbol (this will be true if we are currently at 'L' of "GUILabel")
				{
					// right situation to insert a space before the uppercase letter
					newText.Append(' ');
				}
			}
			else if (treatNumbersAsWords && char.IsDigit(text[i])) // current char is digit
			{
				if (text[i - 1] != ' ' && !char.IsDigit(text[i - 1]))
				{
					// right situation to insert a space before the digit
					newText.Append(' ');
				}
			}

			newText.Append(text[i]);
		}

		return newText.ToString();
	}

	/// <summary>
	///    Remove the starting parts of a string.
	/// </summary>
	/// <param name="text">String that will be edited.</param>
	/// <param name="subStringToSearch">
	///    Substring that will be searched for.
	///    Everything in the text that came before this substring will be removed.
	///    The substring itself will not be removed.
	/// </param>
	/// <param name="idxAdjust">
	///    Offset to the text that will be removed.
	///    Use this to partially remove parts of the substring itself from the result.
	///    If you specify endTextToRemove.Length, then this will remove the substring as well from the result.
	/// </param>
	/// <returns>The new edited string.</returns>
	public static string SearchAndRemoveFromStart(this string text, string subStringToSearch, int idxAdjust = 0)
	{
		if (text == null)
		{
			return null;
		}

		if (text == string.Empty)
		{
			return string.Empty;
		}

		int foundIdx = text.IndexOf(subStringToSearch, StringComparison.Ordinal);
		if (foundIdx < 0)
		{
			// substring was not found
			return text;
		}

		if (foundIdx + idxAdjust >= 0 && foundIdx + idxAdjust < text.Length)
		{
			foundIdx += idxAdjust;
		}

		return text[foundIdx..];
	}

	public static string RemoveFromStart(this string text, string subStringAtStart)
	{
		if (!string.IsNullOrEmpty(text) && text.StartsWith(subStringAtStart))
		{
			return text[subStringAtStart.Length..];
		}

		return text;
	}

	public static string RemoveFromEnd(this string text, string subStringAtEnd)
	{
		if (!string.IsNullOrEmpty(text) && text.EndsWith(subStringAtEnd))
		{
			return text[..^subStringAtEnd.Length];
		}

		return text;
	}

	public static string ReplaceFromEnd(this string text, string subStringToSearch, string stringToReplace)
	{
		if (!string.IsNullOrEmpty(text) && !text.EndsWith(subStringToSearch))
		{
			return text;
		}

		return text[..^subStringToSearch.Length] + stringToReplace;
	}

	public static string ConvertBackToForwardSlash(this string text)
	{
		return text.Replace("\\", "/");
	}

	/// <summary>
	///    If text is all 1234567890ABCDEF (or abcdef, the check is not case-sensitive)
	/// </summary>
	/// <param name="text">The string to check.</param>
	/// <param name="startIdx">Start checking from this character index. Starts at 0. Leave at 0 for default.</param>
	/// <param name="endIdx">End checking at this char. Leave at -1 to check until last char of string (text.Length - 1).</param>
	/// <returns></returns>
	public static bool IsAllHexadecimals(this string text, int startIdx = 0, int endIdx = -1)
	{
		if (string.IsNullOrWhiteSpace(text))
		{
			return false;
		}

		if (text.Length == 1)
		{
			return (text[0] >= '0' && text[0] <= '9') ||
			       (text[0] >= 'a' && text[0] <= 'f') ||
			       (text[0] >= 'A' && text[0] <= 'F');
		}

		if (startIdx < 0)
		{
			startIdx = 0;
		}

		if (endIdx < 0 || endIdx >= text.Length)
		{
			endIdx = text.Length - 1;
		}

		if (startIdx >= endIdx)
		{
			startIdx = endIdx - 1;
		}

		//BetterDebug.Log(
		//	$"IsAllHexadecimals: {text} (from {startIdx.ToString()} '{text[startIdx].ToString()}' to {endIdx.ToString()} '{text[endIdx].ToString()}')");

		for (int n = startIdx; n <= endIdx; ++n)
		{
			char c = text[n];
			if ((c < '0' || c > '9') &&
			    (c < 'a' || c > 'f') &&
			    (c < 'A' || c > 'F'))
			{
				// not a hexadecimal
				return false;
			}
		}

		return true;
	}

	public static bool IsAll(this string text, char charToCheck, int startIdx = 0, int endIdx = -1)
	{
		if (string.IsNullOrWhiteSpace(text))
		{
			return false;
		}

		if (text.Length == 1)
		{
			return text[0] == charToCheck;
		}

		if (startIdx < 0)
		{
			startIdx = 0;
		}

		if (endIdx < 0 || endIdx >= text.Length)
		{
			endIdx = text.Length - 1;
		}

		if (startIdx >= endIdx)
		{
			startIdx = endIdx - 1;
		}

		for (int n = startIdx; n <= endIdx; ++n)
		{
			if (text[n] != charToCheck)
			{
				// not the char we're looking for
				return false;
			}
		}

		return true;
	}

	static readonly char[] Letters =
	{
		'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
		'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
		'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
		'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
	};

	// Note: We purposefully do not include newlines here.
	static readonly char[] WhiteSpace = { ' ', '\t' };

	static readonly char[] WhiteSpaceWithNewline = { ' ', '\t', '\n', '\r' };
	static readonly char[] Newline = { '\n', '\r' };

	/// <summary>
	///    If string has spaces or tabs. Does not count newlines as whitespace.
	/// </summary>
	public static bool HasWhiteSpace(this string text)
	{
		return text.IndexOfAny(WhiteSpace) > -1;
	}

	public static bool PreviousWordHasLetters(this string text, int idx)
	{
		int startIdx = 0;

		int foundStartingSpaceIdx = 0;
		while (startIdx < idx)
		{
			int tryFoundStartingSpaceIdx = text.IndexOfAny(WhiteSpace, startIdx);
			if (tryFoundStartingSpaceIdx < idx)
			{
				foundStartingSpaceIdx = tryFoundStartingSpaceIdx;
			}
			else
			{
				break;
			}

			startIdx = foundStartingSpaceIdx + 1;
		}

		if (foundStartingSpaceIdx >= idx)
		{
			foundStartingSpaceIdx = 0;
		}

		int foundLetterIdx = text.IndexOfAny(Letters, foundStartingSpaceIdx);
		return foundLetterIdx != -1 && foundLetterIdx < idx;
	}

	public static bool NextWordHasLetters(this string text, int idx)
	{
		while (char.IsWhiteSpace(text[idx]) &&
		       idx + 1 < text.Length)
		{
			idx += 1;
		}

		int nextSpaceIdx = text.IndexOfAny(WhiteSpaceWithNewline, idx + 1);
		if (nextSpaceIdx == -1)
		{
			return text.IndexOfAny(Letters, idx) != -1;
		}

		for (int i = idx; i < nextSpaceIdx; ++i)
		{
			if (!char.IsDigit(text[i]))
			{
				return true;
			}
		}

		return false;
	}

	public static bool NoMoreWordsAfter(this string text, int idx)
	{
		int i = idx + 1;
		while (i < text.Length && char.IsWhiteSpace(text[i]))
		{
			i += 1;
		}

		return i >= text.Length;
	}

	public static byte GetNewlineCountUntilNextWord(this string text, int idx)
	{
		byte newlines = 0;
		for (int i = idx; i < text.Length; ++i)
		{
			if (text[i] == '\n')
			{
				++newlines;
			}
			else if (!char.IsWhiteSpace(text[i]))
			{
				break;
			}
		}

		return newlines;
	}

	public static void GetWords(this string text, IList<(string word, byte newlines)> outputWords, string leadingIndent = null, string trailingIndent = null)
	{
		int idx = 0;
		int startIdx = 0;
		int lastWordStartIdx = -1;
		while (idx < text.Length)
		{
			// jump to the first non-whitespace
			while (idx < text.Length && char.IsWhiteSpace(text[idx]))
			{
				idx += 1;
			}

			int nextSpaceIdx = text.IndexOfAny(WhiteSpaceWithNewline, idx);
			if (nextSpaceIdx == -1)
			{
				break;
			}

			if (startIdx != lastWordStartIdx && lastWordStartIdx >= 0)
			{
				startIdx = idx;
			}

			while (startIdx < text.Length && char.IsWhiteSpace(text[startIdx]))
			{
				startIdx += 1;
			}

			bool nextWordHasLetters = text.NextWordHasLetters(nextSpaceIdx);
			bool noMoreWordsAfter = text.NoMoreWordsAfter(nextSpaceIdx);

			if (nextWordHasLetters || noMoreWordsAfter)
			{
				string nextWord = text.Substring(startIdx, nextSpaceIdx - startIdx);

				var chars = new StringBuilder(nextWord.Length);
				for (int i = 0; i < nextWord.Length; ++i)
				{
					if (i > 0 && char.IsWhiteSpace(nextWord[i - 1]) && char.IsWhiteSpace(nextWord[i]))
					{
						continue;
					}

					chars.Append(nextWord[i]);
				}

				// from nextSpaceIdx to the next non-whitespace character, count how many newlines there are
				byte newlines = text.GetNewlineCountUntilNextWord(nextSpaceIdx);

				string newProperNextWord = chars.ToString();
				if (outputWords.Count == 0 && !string.IsNullOrEmpty(leadingIndent))
				{
					newProperNextWord = newProperNextWord.Insert(0, leadingIndent);
				}

				if (noMoreWordsAfter && !string.IsNullOrEmpty(trailingIndent))
				{
					newProperNextWord = $"{newProperNextWord}{trailingIndent}";
				}

				outputWords.Add((newProperNextWord, newlines));
				startIdx = nextSpaceIdx + 1;
				lastWordStartIdx = startIdx;
			}

			idx = nextSpaceIdx + 1;
		}

		if (startIdx < text.Length)
		{
			string finalWord = text.Substring(startIdx).Trim();
			if (!string.IsNullOrWhiteSpace(finalWord))
			{
				if (!string.IsNullOrEmpty(trailingIndent))
				{
					finalWord = $"{finalWord}{trailingIndent}";
				}

				outputWords.Add((finalWord, 0));
			}
		}
	}

	public static int Count(this string text, char charToCount)
	{
		int count = 0;

		foreach (char c in text)
		{
			if (c == charToCount)
			{
				++count;
			}
		}

		return count;
	}

	public static int GetLineCount(this string text)
	{
		// +1 since a newline creates two lines by itself,
		// so the +1 is for the last line.
		return text.Count('\n') + 1;
	}

	public static string ToPlural(this int me, string singular, string plural)
	{
		if (me == 1)
		{
			return singular;
		}

		return plural;
	}

	public static string AddSimpleOrdinal(this int num)
	{
		if (num <= 0)
		{
			return null;
		}

		switch (num % 100)
		{
			case 11:
			case 12:
			case 13:
				return "th";
		}

		switch (num % 10)
		{
			case 1:
				return "st";
			case 2:
				return "nd";
			case 3:
				return "rd";
			default:
				return "th";
		}
	}

	public static string ComputeMD5Hash(string input)
	{
		// Use input string to calculate MD5 hash
		using var md5 = System.Security.Cryptography.MD5.Create();
		byte[] inputBytes = Encoding.UTF8.GetBytes(input);
		byte[] hashBytes = md5.ComputeHash(inputBytes);

		// Convert the byte array to hexadecimal string
		var sb = new StringBuilder();
		for (int i = 0; i < hashBytes.Length; i++)
		{
			sb.Append(hashBytes[i].ToString("X2"));
		}

		return sb.ToString();
	}
}

}
