// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System.Collections.Generic;

namespace DLD.Utility
{

/// <summary>
///    Same as <see cref="Stack{T}"/> but implements <see cref="IPooled"/>
///    so that you can use <see cref="IoC.GetFromPool{T}()"/> on it.
/// </summary>
/// <typeparam name="T"></typeparam>
public class PooledStack<T> : Stack<T>, IPooled
{
	bool _used;

	public bool IsUnused => !_used;

	public void Dispose()
	{
		Clear();
		_used = false;
	}

	public void OnTakenFromPool()
	{
		_used = true;
	}
}

}
