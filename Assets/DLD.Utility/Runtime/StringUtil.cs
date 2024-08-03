using System;
using System.Globalization;
using System.Text;

namespace DLD.Utility
{
	public static class StringUtil
	{
		/// <summary>
		/// Equality checker for strings but will regard
		/// null value and an empty string as equivalent.
		/// For example, if first string is assigned a null
		/// value while the second string is assigned an
		/// empty string like "", then this method returns
		/// true (i.e. they are the same).
		/// </summary>
		public static bool IsSameWith(this string first, string second,
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
		/// If the value is positive or zero, this will return the number with a plus symbol ahead.
		/// If the value is negative, this will return the number with a minus symbol ahead.
		/// </summary>
		/// <param name="me"></param>
		/// <returns></returns>
		public static string ToPlusMinusNumber(this int me)
		{
			const string FORMAT = "+#;-#;0";

			return me.ToString(FORMAT, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// If the value is positive, this will return the number with a plus symbol ahead.
		/// If the value is negative, this will return the number with a minus symbol ahead.
		/// If the value is zero, this will return a blank string.
		/// </summary>
		public static string ToPlusMinusNumberNoZero(this int me)
		{
			const string FORMAT = "+#;-#; ";

			return me.ToString(FORMAT, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Returns either "Yes" or "No".
		/// </summary>
		public static string ToYesNo(this bool b)
		{
			return b ? "Yes" : "No";
		}

		/// <summary>
		/// Returns either "yes" or "no".
		/// </summary>
		public static string ToYesNoSmall(this bool b)
		{
			return b ? "yes" : "no";
		}

		/// <summary>
		/// After the first letter, add a space before every capital letter.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="preserveAcronyms"></param>
		/// <returns></returns>
		public static string AddSpacesToSentence(this string text, bool preserveAcronyms = true)
		{
			if (string.IsNullOrEmpty(text))
			{
				return string.Empty;
			}

			StringBuilder newText = new StringBuilder(text.Length * 2);
			newText.Append(text[0]);
			for (int i = 1; i < text.Length; i++)
			{
				if (char.IsUpper(text[i]))
				{
					if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) ||
					    (preserveAcronyms &&
					     char.IsUpper(text[i - 1]) &&
					     i < text.Length - 1 &&
					     !char.IsUpper(text[i + 1])))
					{
						newText.Append(' ');
					}
				}

				newText.Append(text[i]);
			}

			return newText.ToString();
		}

		/// <summary>
		/// Remove the starting parts of a string.
		/// </summary>
		/// <param name="text">String that will be edited.</param>
		/// <param name="subStringToSearch">Substring that will be searched for.
		///	Everything in the text that came before this substring will be removed.
		///	The substring itself will not be removed.</param>
		/// <param name="idxAdjust">Offset to the text that will be removed.
		///	Use this to partially remove parts of the substring itself from the result.
		///	If you specify endTextToRemove.Length, then this will remove the substring as well from the result.</param>
		/// <returns>The new edited string.</returns>
		public static string RemoveFromStart(this string text, string subStringToSearch, int idxAdjust = 0)
		{
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
	}
}