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
	/// Same as <see cref="Queue{T}"/> but implements <see cref="IPooled"/>
	/// so that you can use <see cref="IoC.GetFromPool{T}()"/> on it.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class PooledQueue<T> : Queue<T>, IPooled
	{
		bool _used;

		public bool IsUnused
		{
			get { return !_used; }
		}

		public void ReleaseToPool()
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