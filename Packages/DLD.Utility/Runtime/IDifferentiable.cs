// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

namespace DLD.Utility
{
	/// <summary>
	/// Adds a difference checker with another instance.
	/// Instead of overriding the Equals method, this makes
	/// it apparent that the class has a value-based equality checker.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IDifferentiable<in T>
	{
		/// <summary>
		/// Method for checking whether another instance's data is different from self.
		/// This is a value-based equality checker.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		bool IsDifferentFromMe(T other);
	}
}