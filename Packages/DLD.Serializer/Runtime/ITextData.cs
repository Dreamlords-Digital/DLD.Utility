// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

namespace DLD.Serializer
{
	/// <summary>
	/// Something that can be saved (serialized) to the computer storage as a text file.
	/// </summary>
	public interface ITextData
	{
		void PostLoad(string fullPath, string filename);
		void PrepareSave();
	}
}