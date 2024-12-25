// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;
using UnityEngine;

namespace DLD.Utility.Inspector
{
	/// <summary>
	/// Make this Serialized Field be shown only when the specified property returns true.
	/// </summary>
	/// <remarks>
	/// <para>This is referring to a C# property, not a <see cref="UnityEditor.SerializedProperty"/>.</para>
	/// <para>This assumes the property returns a boolean. The property does not have to be public.</para>
	/// </remarks>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public class ShowPropertyAttribute : PropertyAttribute
	{
		/// <summary>
		/// Name of property to use.
		/// </summary>
		/// <remarks>
		/// <para>This is referring to a C# property, not a <see cref="UnityEditor.SerializedProperty"/>.</para>
		/// <para>This assumes the property returns a boolean. The property does not have to be public.</para>
		/// </remarks>
		public readonly string PropertyName;

		/// <summary>
		/// Whether to completely hide the Serialized Field, or still show it but with read-only disabled controls.
		/// </summary>
		public HideType HideType { get; } = HideType.DoNotDraw;

		/// <summary>
		/// </summary>
		/// <param name="propertyName">Name of the C# property.
		///
		/// Recommended to use the nameof operator if possible, so wrong spellings will come up as a syntax error.</param>
		public ShowPropertyAttribute(string propertyName) => PropertyName = propertyName;

		/// <summary>
		/// </summary>
		/// <param name="propertyName">Name of the C# property.
		///
		/// Recommended to use the nameof operator if possible, so wrong spellings will come up as a syntax error.</param>
		/// <param name="hideType">Whether to completely hide the Serialized Field, or still show it but with read-only disabled controls.</param>
		public ShowPropertyAttribute(string propertyName, HideType hideType)
		{
			PropertyName = propertyName;
			HideType = hideType;
		}
	}
}