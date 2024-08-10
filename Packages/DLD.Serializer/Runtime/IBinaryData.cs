// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System.IO;

namespace DLD.Serializer
{
	/// <summary>
	/// Something that can be saved (serialized) to the computer storage as a binary file.
	/// </summary>
	public interface IBinaryData
	{
		void SaveToStream(BinaryWriter writer);
		(bool loadSuccess, string errorMessage) LoadFromStream(BinaryReader reader);
	}
}