using System;
using System.Collections.Generic;

namespace DLD.Utility
{
	public static class ListUtil
	{
		public static void SetMinCapacity<T>(this List<T> list, int minCapacity)
		{
			if (list.Count < minCapacity)
			{
				list.Capacity = minCapacity;
			}
		}

		/// <summary>
		/// Add to list only if <see cref="IList{T}.Contains"/> returns false.
		/// </summary>
		/// <param name="list"></param>
		/// <param name="valueToAdd"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns>True if <see cref="valueToAdd"/> was added (it was not found prior to adding), false if not.</returns>
		public static bool AddIfNotInYet<T>(this IList<T> list, T valueToAdd)
		{
			if (!list.Contains(valueToAdd))
			{
				list.Add(valueToAdd);
				return true;
			}

			return false;
		}

		public static void AddRangeIfNotInYet<T>(this IList<T> list, IList<T> valuesToAdd)
		{
			if (valuesToAdd == null)
			{
				return;
			}

			for (int n = 0, len = valuesToAdd.Count; n < len; ++n)
			{
				list.AddIfNotInYet(valuesToAdd[n]);
			}
		}

		public static bool AddIfNotInYet(this IList<string> list, string valueToAdd,
			StringComparison stringComparison = StringComparison.Ordinal)
		{
			for (int n = 0, len = list.Count; n < len; ++n)
			{
				if (list[n].IsSameWith(valueToAdd, stringComparison))
				{
					return false;
				}
			}

			list.Add(valueToAdd);
			return true;
		}

		public static void AddRangeIfNotInYet(this IList<string> list, IList<string> valuesToAdd,
			StringComparison stringComparison = StringComparison.Ordinal)
		{
			if (valuesToAdd == null)
			{
				return;
			}

			for (int n = 0, len = valuesToAdd.Count; n < len; ++n)
			{
				list.AddIfNotInYet(valuesToAdd[n], stringComparison);
			}
		}

		/// <summary>
		/// Removes an element then re-inserts it into a new index in the list.
		/// All the other elements in the list are pushed upwards/downwards as a result.
		/// </summary>
		/// <param name="list"></param>
		/// <param name="currentIdx"></param>
		/// <param name="destinationIdx"></param>
		/// <typeparam name="T"></typeparam>
		/// <remarks>
		/// Note: this doesn't actually call <see cref="System.Collections.Generic.List{T}.Remove"/>
		/// or <see cref="System.Collections.Generic.List{T}.Insert"/>.
		/// </remarks>
		public static void MoveElement<T>(this IList<T> list, int currentIdx, int destinationIdx)
		{
			if (list.Count < 2)
			{
				// nothing to move if there's 0 or only 1 thing in the list
				// has to be at least 2
				return;
			}

			if (currentIdx < 0 || currentIdx >= list.Count)
			{
				// invalid index
				return;
			}

			if (destinationIdx < 0 || destinationIdx >= list.Count)
			{
				// invalid index
				return;
			}

			if (currentIdx == destinationIdx)
			{
				// same index, nothing to move
				return;
			}

			if (currentIdx == destinationIdx + 1 || currentIdx == destinationIdx - 1)
			{
				(list[currentIdx], list[destinationIdx]) = (list[destinationIdx], list[currentIdx]);
			}
			else if (currentIdx > destinationIdx)
			{
				// moving up

				T temp = list[currentIdx];

				for (int n = currentIdx - 1; n >= destinationIdx; --n)
				{
					list[n + 1] = list[n];
				}

				list[destinationIdx] = temp;
			}
			else if (currentIdx < destinationIdx)
			{
				// moving down

				T temp = list[currentIdx];

				for (int n = currentIdx + 1; n <= destinationIdx; ++n)
				{
					list[n - 1] = list[n];
				}

				list[destinationIdx] = temp;
			}
		}

		public static void SwapElements<T>(this IList<T> me, int firstIdx, int secondIdx)
		{
			if (me == null)
			{
				return;
			}

			(me[firstIdx], me[secondIdx]) = (me[secondIdx], me[firstIdx]);
		}

		public static bool MoveUp<T>(this IList<T> me, int idxToMove)
		{
			if (me == null || me.Count == 0)
			{
				return false;
			}

			if (idxToMove <= 0 || idxToMove >= me.Count)
			{
				// can't allow 0 since that's the upper limit, it can't be moved up further
				return false;
			}

			me.SwapElements(idxToMove, idxToMove - 1);
			return true;
		}

		public static bool MoveDown<T>(this IList<T> me, int idxToMove)
		{
			if (me == null || me.Count == 0)
			{
				return false;
			}

			if (idxToMove < 0 || idxToMove >= me.Count - 1)
			{
				// can't allow Count - 1 since that's the lower limit, it can't be moved down further
				return false;
			}

			me.SwapElements(idxToMove, idxToMove + 1);
			return true;
		}

		public static int RemoveAll<T>(this IList<T> me, T valueToRemove)
		{
			if (me == null)
			{
				return 0;
			}

			int elementsRemoved = 0;

			for (int n = me.Count - 1; n >= 0; --n)
			{
				if (EqualityComparer<T>.Default.Equals(me[n], valueToRemove))
				{
					me.RemoveAt(n);
					++elementsRemoved;
				}
			}

			return elementsRemoved;
		}

		public static int RemoveAllReference<T>(this IList<T> me, T referenceToRemove) where T : class
		{
			if (me == null)
			{
				return 0;
			}

			int elementsRemoved = 0;

			for (int n = me.Count - 1; n >= 0; --n)
			{
				if (ReferenceEquals(me[n], referenceToRemove))
				{
					me.RemoveAt(n);
					++elementsRemoved;
				}
			}

			return elementsRemoved;
		}

		public static int RemoveAllDefault<T>(this IList<T> me)
		{
			if (me == null)
			{
				return 0;
			}

			int elementsRemoved = 0;

			for (int n = me.Count - 1; n >= 0; --n)
			{
				if (EqualityComparer<T>.Default.Equals(me[n], default))
				{
					me.RemoveAt(n);
					++elementsRemoved;
				}
			}

			return elementsRemoved;
		}

		public static int RemoveAllNull<T>(this IList<T> me) where T : class
		{
			if (me == null)
			{
				return 0;
			}

			int elementsRemoved = 0;

			for (int n = me.Count - 1; n >= 0; --n)
			{
				if (me[n] == null)
				{
					me.RemoveAt(n);
					++elementsRemoved;
				}
			}

			return elementsRemoved;
		}

		public static int RemoveAllNullOrEmpty(this IList<string> me)
		{
			if (me == null)
			{
				return 0;
			}

			int elementsRemoved = 0;

			for (int n = me.Count - 1; n >= 0; --n)
			{
				if (string.IsNullOrEmpty(me[n]))
				{
					me.RemoveAt(n);
					++elementsRemoved;
				}
			}

			return elementsRemoved;
		}

		public static int RemoveAllNullOrWhitespace(this IList<string> me)
		{
			if (me == null)
			{
				return 0;
			}

			int elementsRemoved = 0;

			for (int n = me.Count - 1; n >= 0; --n)
			{
				if (string.IsNullOrWhiteSpace(me[n]))
				{
					me.RemoveAt(n);
					++elementsRemoved;
				}
			}

			return elementsRemoved;
		}

		/// <summary>
		/// Is this List of strings empty or entirely composed of null or empty strings?
		/// </summary>
		/// <param name="me"></param>
		/// <returns></returns>
		public static bool AllNullOrEmpty(this IList<string> me)
		{
			if (me == null)
			{
				return true;
			}

			if (me.Count == 0)
			{
				return true;
			}

			for (int n = 0, len = me.Count; n < len; ++n)
			{
				if (!string.IsNullOrEmpty(me[n]))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Is this List of strings empty or entirely composed of null or white space strings?
		/// </summary>
		/// <param name="me"></param>
		/// <returns></returns>
		public static bool AllNullOrWhiteSpace(this IList<string> me)
		{
			if (me == null)
			{
				return true;
			}

			if (me.Count == 0)
			{
				return true;
			}

			for (int n = 0, len = me.Count; n < len; ++n)
			{
				if (!string.IsNullOrWhiteSpace(me[n]))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Is this List empty or entirely composed of null references?
		/// </summary>
		/// <param name="me"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static bool AllNull<T>(this IList<T> me) where T : class
		{
			if (me == null)
			{
				return true;
			}

			if (me.Count == 0)
			{
				return true;
			}

			for (int n = 0, len = me.Count; n < len; ++n)
			{
				if (me[n] != null)
				{
					return false;
				}
			}

			return true;
		}
	}
}