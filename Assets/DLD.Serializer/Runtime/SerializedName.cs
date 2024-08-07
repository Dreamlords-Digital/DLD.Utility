// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;

namespace DLD.Serializer
{
	[AttributeUsage(AttributeTargets.All)]
	public sealed class SerializedAttribute : Attribute
	{
		public readonly string Name;

		public SerializedAttribute()
		{
		}

		public SerializedAttribute(string name)
		{
			Name = name;
		}
	}
}