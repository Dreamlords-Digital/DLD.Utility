/*
COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.
THIS WORK IS UNPUBLISHED.

PROPRIETARY AND CONFIDENTIAL

ALL INFORMATION AND MATERIAL CONTAINED HEREIN IS CONFIDENTIAL, AND PROPRIETARY
TO DREAMLORDS DIGITAL INC.

UNAUTHORIZED REPRODUCTION, MODIFICATION, OR PUBLICATION OF THIS SOURCE CODE
OR DISCLOSURE OF ITS INFORMATION, VIA ANY MEDIUM, IS STRICTLY FORBIDDEN.
*/

namespace DLD.Utility
{
	/// <summary>
	/// Implement so your class can be pooled, using the <see cref="IoC"/> static class:
	/// <list type="bullet">
	/// <item><term><see cref="IoC.GetFromPool{T}()"/></term></item>
	/// <item><term><see cref="IoC.GetFromPool{T}(System.Type)"/></term></item>
	/// </list>
	/// </summary>
	public interface IPooled
	{
		/// <summary>
		/// Is this object currently inactive and ready to be re-used?
		/// </summary>
		bool IsUnused { get; }

		/// <summary>
		/// Called when the object came from the pool and has just been re-used.
		/// Code in here should be for any initialization required for the object to do its job.
		/// </summary>
		void OnTakenFromPool();

		/// <summary>
		/// Called when the object has finished its job, so it can be returned to the pool for re-use.
		/// Code in here should be for any cleanup required.
		/// </summary>
		void ReleaseToPool();
	}
}