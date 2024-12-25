// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;
using UnityEngine;

namespace DLD.Utility.Inspector
{
	/// <summary>
	/// Use a property to determine a message to be drawn below the Serialized Field.
	/// </summary>
	/// <remarks>
	/// <para>This is referring to a C# property, not a <see cref="UnityEditor.SerializedProperty"/>.</para>
	/// <para>This is assuming the property returns a string. The property does not have to be public.</para>
	/// </remarks>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public class MessagePropertyAttribute : PropertyAttribute
	{
		/// <summary>
		/// Name of property to use.
		/// </summary>
		/// <remarks>
		/// <para>This is referring to a C# property, not a <see cref="UnityEditor.SerializedProperty"/>.</para>
		/// <para>This assumes the property returns a string. The property does not have to be public.</para>
		/// </remarks>
		public readonly string PropertyName;

		public readonly bool RepaintEveryFrame;

		/// <summary>
		/// </summary>
		/// <param name="propertyName">Name of the C# property.
		///
		/// Recommended to use the nameof operator if possible, so wrong spellings will come up as a syntax error.</param>
		/// <param name="repaintEveryFrame"></param>
		public MessagePropertyAttribute(string propertyName, bool repaintEveryFrame = false)
		{
			PropertyName = propertyName;
			RepaintEveryFrame = repaintEveryFrame;
		}
	}
}