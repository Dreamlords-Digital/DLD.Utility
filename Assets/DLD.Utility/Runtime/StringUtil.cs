using System;
using System.Globalization;

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
	}
}