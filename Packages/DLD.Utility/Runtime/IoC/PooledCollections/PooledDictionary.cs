// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System.Collections.Generic;

namespace DLD.Utility
{
	/// <summary>
	/// Same as <see cref="Dictionary{TKey,TValue}"/> but implements <see cref="IPooled"/>
	/// so that you can use <see cref="IoC.GetFromPool{T}()"/> on it.
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	public class PooledDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IPooled
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
}