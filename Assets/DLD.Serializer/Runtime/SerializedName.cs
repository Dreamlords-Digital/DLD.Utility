// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;

namespace DLD.Serializer
{
	[AttributeUsage(AttributeTargets.All)]
	public sealed class SerializedAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.All)]
	public sealed class SerializedNameAttribute : Attribute
	{
		public SerializedNameAttribute(string serializedName)
		{
			Name = serializedName;
		}

		public string Name { get; private set; }
	}
}