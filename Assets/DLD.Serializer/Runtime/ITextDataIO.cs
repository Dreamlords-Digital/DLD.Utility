// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System.Collections.Generic;
using System.Threading.Tasks;
using DLD.Utility;

namespace DLD.Serializer
{

/// <summary>
/// Something that can save/load <see cref="ITextData"/> objects to/from different locations in the computer.
/// </summary>
public interface ITextDataIO
{
	public const string DEFAULT_HASH_ALGORITHM = "MD5";

	// ------------------------------------------------------------------------------------------------------
	// de/serializing to/from strings

	T FromSerializedString<T>(string fileText) where T : ITextData;
	string ToSerializedString<T>(T data) where T : ITextData;
	string ToSerializedString<T>(T data, out string hash) where T : ITextData;

	/// <summary>
	/// Generates a hash out of the binary form of the serializable fields and properties of the passed data.
	/// That means any variables that are configured to be not serialized are not included in the hash.
	/// The hash is then converted into a string for ease of use.
	/// </summary>
	/// <param name="data"></param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	string ToHash<T>(T data) where T : ITextData;

	// ------------------------------------------------------------------------------------------------------
	// save functions

#if UNITY_EDITOR
	void SaveToAssets<T>(string saveFileAssetsPath, T data) where T : ITextData;
	void SaveToAssets<T>(string saveFileAssetsPath, T data, out string hash, string hashAlgorithmName = DEFAULT_HASH_ALGORITHM) where T : ITextData;
#endif

	void SaveToLocal<T>(string saveFilePath, T data) where T : ITextData;
	void SaveToLocal<T>(string saveFilePath, T data, out string hash, string hashAlgorithmName = DEFAULT_HASH_ALGORITHM) where T : ITextData;
	void SaveBinaryToLocal<T>(string saveFilePath, T data) where T : IBinaryData;
	void SaveToStreamingAssets<T>(string saveFileAssetsPath, T data) where T : ITextData;
	void SaveToStreamingAssets<T>(string saveFileAssetsPath, T data, out string hash, string hashAlgorithmName = DEFAULT_HASH_ALGORITHM) where T : ITextData;

	// ======================================================================================================
	// load functions

	bool SetValuesFromLocal<T>(string filePath, T objectToBeGivenValues) where T : ITextData, IValuesSettableFrom<T>;

#if UNITY_EDITOR
	/// <summary>
	/// Load file from the project's "Assets/" folder. Only works while in the Unity Editor.
	/// </summary>
	/// <param name="fileAssetsPath"></param>
	/// <param name="resultingLoadedObject"></param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	(LoadResult result, string errorMessage) TryLoadFromAssets<T>(string fileAssetsPath, ref T resultingLoadedObject) where T : ITextData;
#endif

	/// <summary>
	/// Load an asset given the absolute path to it.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="filePath"></param>
	/// <param name="resultingLoadedObject"></param>
	/// <returns></returns>
	(LoadResult result, string errorMessage) TryLoadFromLocal<T>(string filePath, ref T resultingLoadedObject) where T : ITextData;

	/// <summary>
	/// Load an asset, with the specified path being relative to a Resources folder in the project.
	/// The file's file type should not be included in the specified <see cref="filePath"/>,
	/// as UnityEngine's Resources.<see cref="UnityEngine.Resources.Load{T}(string)"/> requires.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="filePath"></param>
	/// <param name="resultingLoadedObject"></param>
	/// <returns></returns>
	(LoadResult result, string errorMessage) TryLoadFromResources<T>(string filePath, ref T resultingLoadedObject) where T : ITextData;

	/// <summary>
	/// Load a binary file, with the specified path being relative to a Resources folder in the project.
	/// Loaded using <see cref="UnityEngine.TextAsset"/>, so the file in the project actually needs to
	/// have the ".bytes" file type (it's the only way Unity will interpret the file as a binary file),
	/// but the specified <see cref="filePath"/> should not end with ".bytes",
	/// as UnityEngine's Resources.<see cref="UnityEngine.Resources.Load{T}(string)"/> requires.
	/// </summary>
	/// <param name="filePath"></param>
	/// <param name="resultingLoadedObject"></param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	(LoadResult result, string errorMessage) TryLoadBinaryFromResources<T>(string filePath, ref T resultingLoadedObject) where T : IBinaryData;

	/// <summary>
	/// Load an asset, with the specified path being relative to the project's StreamingAssets path.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="filePath"></param>
	/// <param name="resultingLoadedObject"></param>
	/// <returns>true means load was successful</returns>
	(LoadResult result, string errorMessage) TryLoadFromStreamingAssets<T>(string filePath, ref T resultingLoadedObject) where T : ITextData;

	// ------------------------------------------------------------------------------------------------------
	// loading all files in a folder to a list

	/// <summary>
	/// Load all assets of a certain type found in a folder, given the absolute path to the folder.
	/// Results will be in the list specified. Note that the list will not be cleared beforehand,
	/// so results will be appended to the list.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="folderPath">Folder where serialized text files of the type is found.
	/// This folder will be searched through recursively, so sub-folders will also be looked at.</param>
	/// <param name="fileType">File type of serialized text file. This is used so it knows which files to attempt deserialization on.</param>
	/// <param name="outList">Loaded assets will be appended here</param>
	/// <returns></returns>
	void LoadAllFromLocal<T>(string folderPath, string fileType, List<T> outList)
		where T : ITextData;

	/// <summary>
	/// Load all assets of a certain type found, given the Resources folder to it.
	/// Results will be in the list specified. Note that the list will not be cleared beforehand,
	/// so results will be appended to the list.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="folderPath"></param>
	/// <param name="outList">Loaded assets will be appended here</param>
	/// <returns></returns>
	void LoadAllFromResourcesFolder<T>(string folderPath, List<T> outList)
		where T : ITextData;

	/// <summary>
	/// Load all assets of a given type, given the folder to it (must be relative to the StreamingAssets path)
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="folderPath"></param>
	/// <param name="fileType"></param>
	/// <param name="outList">Loaded assets will be appended here</param>
	/// <returns></returns>
	void LoadAllFromStreamingAssets<T>(string folderPath, string fileType, List<T> outList)
		where T : ITextData;
}

}