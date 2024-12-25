/*
COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.
THIS WORK IS UNPUBLISHED.

PROPRIETARY AND CONFIDENTIAL

ALL INFORMATION AND MATERIAL CONTAINED HEREIN IS CONFIDENTIAL, AND PROPRIETARY
TO DREAMLORDS DIGITAL INC.

UNAUTHORIZED REPRODUCTION, MODIFICATION, OR PUBLICATION OF THIS SOURCE CODE
OR DISCLOSURE OF ITS INFORMATION, VIA ANY MEDIUM, IS STRICTLY FORBIDDEN.
*/

using System.Collections.Generic;

namespace DLD.Utility
{
	/// <summary>
	/// Same as <see cref="List{T}"/> but implements <see cref="IPooled"/>
	/// so that you can use <see cref="IoC.GetFromPool{T}()"/> on it.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class PooledList<T> : List<T>, IPooled
	{
		bool _used;

		public bool IsUnused
		{
			get { return !_used; }
		}

		public void Dispose()
		{
			_used = false;
		}

		public void OnTakenFromPool()
		{
			Clear();
			_used = true;
		}
	}

	/// <summary>
	/// Same as <see cref="PooledList{T}"/> but expects <see cref="T"/> to implement <see cref="IPooled"/>
	/// so that elements inside are properly released back to pool when removed.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class PooledPooledList<T> : List<T>, IPooled where T : IPooled
	{
		bool _used;

		public bool IsUnused
		{
			get { return !_used; }
		}

		public void Dispose()
		{
			Clear();
			_used = false;
		}

		public void OnTakenFromPool()
		{
			Clear();
			_used = true;
		}

		public new void Clear()
		{
			for (int n = 0, count = Count; n < count; ++n)
			{
				this[n]?.Dispose();
			}

			base.Clear();
		}

		public void RemoveAt(int index, bool releaseToPool = false)
		{
			if (index < 0 || index >= Count)
			{
				return;
			}

			if (releaseToPool)
			{
				this[index]?.Dispose();
			}

			base.RemoveAt(index);
		}

		public bool Remove(T item, bool releaseToPool = false)
		{
			int index = IndexOf(item);
			if (index < 0)
				return false;

			RemoveAt(index, releaseToPool);
			return true;
		}

		public void RemoveRange(int index, int count, bool releaseToPool = false)
		{
			if (index < 0 || count < 0)
			{
				return;
			}
			if (Count - index < count)
			{
				return;
			}

			if (count <= 0)
			{
				return;
			}

			if (releaseToPool)
			{
				for (int n = index; n < index + count; ++n)
				{
					this[n]?.Dispose();
				}
			}

			base.RemoveRange(index, count);
		}
	}
}