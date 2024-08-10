// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Assertions;

namespace DLD.Utility
{
	public static class RandomUtil
	{
		// ------------------------------------------------------------------------------------------------

		/// <summary>
		/// Return random number between sta and end. Both inclusive.
		/// </summary>
		/// <param name="sta"></param>
		/// <param name="end"></param>
		/// <returns></returns>
		public static int Range(int sta, int end)
		{
			return UnityEngine.Random.Range(sta, end + 1);
		}

		public static void ChangeSeed(int newSeed)
		{
			UnityEngine.Random.InitState(newSeed);
		}

		// ------------------------------------------------------------------------------------------------

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
			var result = new StringBuilder(maxSize);
			bool consonantNow = true;
			int count = 0;
			for (int i = 0; i < maxSize; i++)
			{
				if (count >= maxSize - numbersCount)
				{
					result.Append(Numbers.PickRandomElement());
				}
				else if (consonantNow)
				{
					result.Append(Consonants.PickRandomElement());
				}
				else
				{
					result.Append(Vowels.PickRandomElement());
				}

				consonantNow = !consonantNow;
				++count;
			}

			return result.ToString();
		}

		public static string GetNumberString(int length)
		{
			var result = new StringBuilder(length);
			for (int i = 0; i < length; i++)
			{
				result.Append(Numbers.PickRandomElement());
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
				return default;
			}

			if (array.Length == 0)
			{
				return default;
			}

			return array[Range(0, array.Length - 1)];
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="collection"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T PickRandomElement<T>(this IList<T> collection)
		{
			if (collection == null)
			{
				return default;
			}

			var count = collection.Count;

			if (count == 0)
			{
				return default;
			}

			Assert.IsTrue(count >= 1);
			Assert.IsTrue(count <= 256, $"list count has to be within 0 to 256. was: {count}");
			return collection.ElementAt(Range(0, count - 1));
		}

		public static T PickRandomElement<T>(this IEnumerable<T> collection)
		{
			if (collection == null)
			{
				return default;
			}

			var collectionList = collection as IList<T> ?? collection.ToList();
			var count = collectionList.Count();

			if (count == 0)
			{
				return default;
			}

			Assert.IsTrue(count >= 1);
			return collectionList.ElementAt(Range(0, count - 1));
		}

		/// <summary>
		///
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
				result += Range(1, numberOfSides);
			}

			//Debug.Log($"RandomUtil.RollDice: dice roll {numberOfDice}d{numberOfSides}+{bonus} returned {result}");

			return result;
		}

		// ------------------------------------------------------------------------------------------------
	}
}