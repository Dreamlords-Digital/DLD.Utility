// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;
using DLD.JsonFx;

namespace DLD.Serializer
{
	[AttributeUsage(AttributeTargets.All)]
	public sealed class SerializedAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.All)]
	public sealed class SerializedNameAttribute : JsonNameAttribute
	{
		public SerializedNameAttribute(string serializedName) : base(serializedName)
		{
		}
	}
}