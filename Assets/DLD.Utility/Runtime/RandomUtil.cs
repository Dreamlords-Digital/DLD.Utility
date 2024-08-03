using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;

namespace DLD.Utility
{
	public static class RandomUtil
	{
		static readonly char[] Consonants =
		{
			'B', 'C', 'D', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'R', 'S', 'T', 'V', 'W', 'X', 'Y', 'Z'
		};

		static readonly char[] Vowels =
		{
			'A', 'E', 'I', 'O', 'U'
		};

		static readonly char[] Numbers =
		{
			'2', '3', '4', '5', '6', '7', '8', '9'
		};

		public static string GetUniqueKey(int maxSize, int numbersCount = 4)
		{
			byte[] data = new byte[1];

			{
				RngCrypto.GetNonZeroBytes(data);
				data = new byte[maxSize];
				RngCrypto.GetNonZeroBytes(data);
			}

			StringBuilder result = new StringBuilder(maxSize);
			bool consonantNow = true;
			int count = 0;
			foreach (byte b in data)
			{
				if (count >= maxSize - numbersCount)
				{
					result.Append(Numbers[b % (Numbers.Length)]);
				}
				else if (consonantNow)
				{
					result.Append(Consonants[b % (Consonants.Length)]);
				}
				else
				{
					result.Append(Vowels[b % (Vowels.Length)]);
				}

				consonantNow = !consonantNow;
				++count;
			}

			return result.ToString();
		}

		public static string GetNumberString(int length)
		{
			byte[] data = new byte[1];

			{
				RngCrypto.GetNonZeroBytes(data);
				data = new byte[length];
				RngCrypto.GetNonZeroBytes(data);
			}

			StringBuilder result = new StringBuilder(length);
			foreach (byte b in data)
			{
				result.Append(Numbers[b % (Numbers.Length)]);
			}

			return result.ToString();
		}

		// ------------------------------------------------------------------------------------------------

		/// <summary>
		/// Fisher-Yates type of shuffling.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		public static void Shuffle<T>(this IList<T> list)
		{
			var count = list.Count;
			var last = count - 1;
			for (var i = 0; i < last; ++i)
			{
				var r = Range(i, count - 1);
				(list[i], list[r]) = (list[r], list[i]);
			}
		}

		public static T PickRandomElement<T>(this T[] array)
		{
			if (array == null)
			{
				return default(T);
			}

			if (array.Length == 0)
			{
				return default(T);
			}

			return array[Range(0, array.Length - 1)];
		}

		/// <summary>
		/// WARNING: Current limitation is that the random choosing only works if the List count is less than 256
		/// </summary>
		/// <param name="collection"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T PickRandomElement<T>(this IList<T> collection)
		{
			if (collection == null)
			{
				return default(T);
			}

			var count = collection.Count;

			if (count == 0)
			{
				return default(T);
			}

			Assert.IsTrue(count >= 1);
			Assert.IsTrue(count <= 256, $"list count has to be within 0 to 256. was: {count}");
			return collection.ElementAt(Range(0, count - 1));
		}

		public static T PickRandomElement<T>(this IEnumerable<T> collection)
		{
			if (collection == null)
			{
				return default(T);
			}

			var collectionList = collection as IList<T> ?? collection.ToList();
			var count = collectionList.Count();

			if (count == 0)
			{
				return default(T);
			}

			Assert.IsTrue(count >= 1);
			return collectionList.ElementAt(Range(0, count - 1));
		}

		/// <summary>
		/// WARNING: Current limitation is that the random choosing only works if the List count is less than 256
		/// </summary>
		/// <param name="collection"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static int PickRandomIndex<T>(this IList<T> collection)
		{
			if (collection == null)
			{
				return -1;
			}

			var count = collection.Count;

			if (count == 0)
			{
				return -1;
			}

			if (count == 1)
			{
				return 0;
			}

			Assert.IsTrue(count >= 1);
			Assert.IsTrue(count <= 256, $"list count has to be within 0 to 256. was: {count}");
			return Range(0, count - 1);
		}

		// ------------------------------------------------------------------------------------------------

		public static bool IsRangeAllowed(int sta, int end)
		{
			int intRange = end - sta + 1;
			return intRange is > 0 and < 255;
		}

		public static int Range(int sta, int end)
		{
			// Examples:
			//
			// 7 to 12
			// (12 - 7 + 1) = 6
			// (1 to 6) + 7 - 1 = 7 to 12
			//
			// 1 to 9
			// (9 - 1 + 1) = 9
			// (1 to 9) + 1 - 1 = 1 to 9
			//
			// 0 to 9
			// (9 - 0 + 1) = 10
			// (1 to 10) + 0 - 1 = 0 to 9

			int intRange = end - sta + 1;
			if (intRange >= 255)
			{
				Debug.LogAssertion(
					$"RNG.Range: Range between {sta.ToString()} to {end.ToString()} ({intRange.ToString()}) should be not more than 255");
				return 0;
			}

			byte range = (byte)intRange;

			return RollDiceRngCrypto(range) + sta - 1;
		}

		// ------------------------------------------------------------------------------------------------

		public static int RollDice(int numberOfDice, int numberOfSides)
		{
			if (numberOfDice == 0 || numberOfSides == 0)
			{
				return 0;
			}

			Assert.IsTrue(numberOfSides is >= 2 and <= 255,
				$"number of sides has to be within 2 to 255. was: {numberOfSides}");

			return RollDice(numberOfDice, numberOfSides, 0);
		}

		public static int RollDice(int numberOfDice, int numberOfSides, int bonus)
		{
			if (numberOfDice == 0 || numberOfSides == 0)
			{
				return bonus;
			}

			Assert.IsTrue(numberOfSides is >= 2 and <= 255,
				$"number of sides has to be within 2 to 255. was: {numberOfSides}");

			int result = bonus;

			for (int n = 0; n < numberOfDice; ++n)
			{
				result += RollDice(numberOfSides);
			}

			//Debug.Log($"RandomUtil.RollDice: dice roll {numberOfDice}d{numberOfSides}+{bonus} returned {result}");

			return result;
		}

		public static int RollDice(int numberOfSides)
		{
			Assert.IsTrue(numberOfSides is >= 2 and <= 255,
				$"number of sides has to be within 2 to 255. was: {numberOfSides}");

			return RollDiceRngCrypto((byte)numberOfSides);
		}

		// ------------------------------------------------------------------------------------------------
		// current implementation is using RNGCryptoServiceProvider

		static readonly RNGCryptoServiceProvider RngCrypto = new RNGCryptoServiceProvider();

		static readonly byte[] RandomNumber = new byte[1];

		static byte RollDiceRngCrypto(byte numberSides)
		{
			if (numberSides <= 0)
			{
				return 0;
			}

			do
			{
				// Fill the array with a random value.
				RngCrypto.GetBytes(RandomNumber);
			} while (!IsFairRoll(RandomNumber[0], numberSides));

			// Return the random number mod the number of sides.
			// The possible values are zero-based, so we add one.
			return (byte)((RandomNumber[0] % numberSides) + 1);
		}

		static bool IsFairRoll(byte roll, byte numSides)
		{
			// There are MaxValue / numSides full sets of numbers that can come up
			// in a single byte.  For instance, if we have a 6 sided die, there are
			// 42 full sets of 1-6 that come up.  The 43rd set is incomplete.
			int fullSetsOfValues = Byte.MaxValue / numSides;

			// If the roll is within this range of fair values, then we let it continue.
			// In the 6 sided die case, a roll between 0 and 251 is allowed.  (We use
			// < rather than <= since the = portion allows through an extra 0 value).
			// 252 through 255 would provide an extra 0, 1, 2, 3 so they are not fair
			// to use.
			return roll < numSides * fullSetsOfValues;
		}
	}
}