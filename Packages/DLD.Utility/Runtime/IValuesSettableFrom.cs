// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

namespace DLD.Utility
{
	/// <summary>
	/// Allows an object to set its own data to match the data from another object.
	/// This is a deep copy operation. No references will be shared. Object allocations will be created if necessary.
	/// </summary>
	/// <remarks>
	/// This is different from <see cref="System.ICloneable"/> because ICloneable generates a new copy of the object,
	/// whereas the <see cref="SetValuesFrom"/> just inspects the values of another instance and recreates that data within itself.
	/// </remarks>
	/// <typeparam name="T"></typeparam>
	public interface IValuesSettableFrom<in T>
	{
		/// <summary>
		/// Takes a look at the values of the other object, and set our own values to match them.
		/// This is a deep copy operation. No references will be shared. Object allocations will be created if necessary.
		/// </summary>
		/// <param name="other"></param>
		void SetValuesFrom(T other);
	}

	/// <summary>
	/// Allows an object to set its own data to match the data from another object.
	/// This is a deep copy operation. No references will be shared. Object allocations will be created if necessary.
	/// This also allows you to specify whether the object's Unique ID is copied, or we keep our own.
	/// </summary>
	/// <remarks>
	/// This is different from <see cref="System.ICloneable"/> because ICloneable generates a new copy of the object,
	/// whereas the <see cref="SetValuesFrom"/> just inspects the values of another instance and recreates that data within itself.
	/// </remarks>
	public interface IValuesSettableFromWithUid<in T> : IValuesSettableFrom<T>
	{
		/// <summary>
		/// Allows an object to set its own data to match the data from another object.
		/// This is a deep copy operation. No references will be shared. Object allocations will be created if necessary.
		/// This also allows you to specify whether the object's Unique ID is copied, or we keep our own.
		/// </summary>
		/// <param name="other">instance to copy from</param>
		/// <param name="overwriteUid"><para>true means we copy the UID of the instance and discard our own, false means we keep our own UID.</para>
		/// <para>Use true if you want to make a snapshot copy whose sole job is to be compared against the original (saving the duplicate effectively overwrites the original).</para>
		/// <para>Use false when you want to make a distinct duplicate that can co-exist with the original (saving the duplicate will create its own file and not overwrite the original).</para></param>
		void SetValuesFrom(T other, bool overwriteUid);
	}
}