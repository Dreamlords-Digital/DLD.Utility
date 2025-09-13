// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;

namespace DLD.Serializer
{
	[AttributeUsage(AttributeTargets.All)]
	[JetBrains.Annotations.MeansImplicitUse]
	public class SerializedAttribute : Attribute
	{
		/// <summary>
		///    Name of this field/property when serialized.
		/// </summary>
		public readonly string Name;

		public SerializedAttribute()
		{
		}

		public SerializedAttribute(string name)
		{
			Name = name;
		}
	}

	[AttributeUsage(AttributeTargets.All)]
	public class NotSerializedAttribute : Attribute
	{
		public NotSerializedAttribute()
		{
		}
	}
}
