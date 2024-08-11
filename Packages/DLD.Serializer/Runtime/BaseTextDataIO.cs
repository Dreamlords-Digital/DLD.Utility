// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DLD.Utility;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DLD.Serializer
{
	public enum LoadResult
	{
		Success,
		EmptyPathGiven,
		FileDoesNotExist,
		FileIsEmpty,
		FileIsAllNull,
		OtherError,
	}

	public static class LoadResultUtility
	{
		public static string ToReadableString(this LoadResult loadResult)
		{
			switch (loadResult)
			{
				case LoadResult.Success:
					return "Success";
				case LoadResult.EmptyPathGiven:
					return "Empty Path Given";
				case LoadResult.FileDoesNotExist:
					return "File Does Not Exist";
				case LoadResult.FileIsEmpty:
					return "File Is Empty";
				case LoadResult.FileIsAllNull:
					return "File Is All Null";
				case LoadResult.OtherError:
					return "Other Error";
				default:
					return loadResult.ToString();
			}
		}

		public static string GetErrorMessage(this LoadResult loadResult, string errorMessage, string filePath)
		{
			switch (loadResult)
			{
				case LoadResult.EmptyPathGiven:
					return $"No path specified: {filePath}";
				case LoadResult.FileDoesNotExist:
					return $"File doesn't exist: {filePath}";
				case LoadResult.FileIsEmpty:
					return $"File is empty inside: {filePath}\nDelete it or use git to revert file to proper state.";
				case LoadResult.FileIsAllNull:
					return $"File is corrupted, all null bytes inside: {filePath}\nDelete it or use git to revert file to proper state.";
				case LoadResult.OtherError:
					return $"File has error: {filePath}\n{errorMessage}";
				default:
					return $"File has unknown error: {filePath}";
			}
		}
	}


	public abstract class BaseTextDataIO : ITextDataIO
	{
		public const string TYPE_HINT_NAME = "$type";

		protected abstract string GetFileContents(string filePath);

		protected abstract (LoadResult result, string errorMessage) DeserializeObjectFromFile<T>(string filePath, out T returnValue) where T : ITextData;
		protected abstract void SerializeObjectToFile<T>(T data, string filePath);

		protected abstract void SerializeObjectToFile<T>(T data, string filePath, out string hash, string hashAlgorithmName = ITextDataIO.DEFAULT_HASH_ALGORITHM);

		protected abstract T DeserializeObject<T>(string fileText) where T : ITextData;
		protected abstract string SerializeObject<T>(T data);

		protected abstract string SerializeObject<T>(T data, out string hash, string hashAlgorithmName = ITextDataIO.DEFAULT_HASH_ALGORITHM);

		/// <summary>
		/// Generates a hash out of the binary form of the serializable fields and properties of the passed data.
		/// That means any variables that are configured to be not serialized are not included in the hash.
		/// The hash is then converted into a string for ease of use.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="hashAlgorithmName">for other possible values, see:
		///	https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.hashalgorithm.create?view=net-5.0#system-security-cryptography-hashalgorithm-create(system-string)</param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		protected abstract string GetObjectHash<T>(T data, string hashAlgorithmName = ITextDataIO.DEFAULT_HASH_ALGORITHM);

		protected abstract Task<(T, LoadResult)> DeserializeObjectFromFileAsync<T>(string filePath);

		protected static async Task<string> ReadTextAsync(string filePath)
		{
			using var reader = File.OpenText(filePath);
			string fileText = await reader.ReadToEndAsync();
			return fileText;
		}

		static bool IsNullOrEmpty<T>(T obj)
		{
			return EqualityComparer<T>.Default.Equals(obj, default);
		}

		public (LoadResult, string) TryLoadFromLocal<T>(string filePath, ref T resultingLoadedObject) where T : ITextData
		{
			if (string.IsNullOrEmpty(filePath))
			{
				return (LoadResult.EmptyPathGiven, null);
			}

			if (!File.Exists(filePath))
			{
				return (LoadResult.FileDoesNotExist, null);
			}

			var (loadResult, errorMsg) = DeserializeObjectFromFile(filePath, out resultingLoadedObject);

			if (IsNullOrEmpty(resultingLoadedObject))
			{
				return (LoadResult.FileIsEmpty, null);
			}

			if (loadResult == LoadResult.Success)
			{
				string filename = Path.GetFileName(filePath);
				string path = Path.GetFullPath(filePath).Replace('\\', '/');

				resultingLoadedObject.PostLoad(path, filename);
				return (loadResult, null);
			}

			//Debug.LogWarningFormat("Error on loading file ({0}): \"{1}\"", loadResult.ToReadableString(), filePath);
			return (loadResult, errorMsg);
		}

		public bool SetValuesFromLocal<T>(string filePath, T objectToBeGivenValues) where T : ITextData, IValuesSettableFrom<T>
		{
			if (string.IsNullOrEmpty(filePath))
			{
				return false;
			}

			if (!File.Exists(filePath))
			{
				return false;
			}

			var loadResult = DeserializeObjectFromFile<T>(filePath, out var resultingLoadedObject);

			if (IsNullOrEmpty(resultingLoadedObject))
			{
				return false;
			}

			if (loadResult.result == LoadResult.Success)
			{
				string filename = Path.GetFileName(filePath);
				string path = Path.GetFullPath(filePath).Replace('\\', '/');

				resultingLoadedObject.PostLoad(path, filename);
				objectToBeGivenValues.SetValuesFrom(resultingLoadedObject);
				return true;
			}

			Debug.LogWarning($"Error on loading file ({loadResult.result.ToReadableString()}, {loadResult.errorMessage}): \"{filePath}\"");
			return false;
		}

#if UNITY_EDITOR
		public (LoadResult result, string errorMessage) TryLoadFromAssets<T>(string fileAssetsPath,
			ref T resultingLoadedObject) where T : ITextData
		{
			return TryLoadFromLocal($"{FileUtil.ProjectPath}/{fileAssetsPath}", ref resultingLoadedObject);
		}
#endif

		public (LoadResult result, string errorMessage) TryLoadFromStreamingAssets<T>(string fileAssetsPath,
			ref T resultingLoadedObject) where T : ITextData
		{
			return TryLoadFromLocal($"{Application.streamingAssetsPath}/{fileAssetsPath}", ref resultingLoadedObject);
		}

		public void LoadAllFromStreamingAssets<T>(string folderPath, string fileType, List<T> results) where T : ITextData
		{
			folderPath = $"{Application.streamingAssetsPath}/{folderPath}";

			LoadAllFromLocal(folderPath, fileType, results);
		}

#if UNITY_EDITOR
		readonly Dictionary<string, UnityEngine.Profiling.CustomSampler> _deserializeSampler = new Dictionary<string, UnityEngine.Profiling.CustomSampler>();
#endif

		/// <summary>
		/// Deserialize text file specified in <see cref="file"/>, into the type <see cref="T"/>, and add it to the <see cref="resultBag"/>.
		/// If the file has erroneous data that can't be resolved, it won't be added to the bag.
		/// </summary>
		/// <param name="file">Absolute path to the serialized text file.</param>
		/// <param name="resultBag">Where the deserialized object will be placed into.</param>
		/// <typeparam name="T">The type of object you want to deserialize into. Has to implement <see cref="ITextData"/>.</typeparam>
		/// <returns></returns>
		async Task DeserializeAndAddToBagAsync<T>(string file, ConcurrentBag<T> resultBag) where T : ITextData
		{
			var deserializeTask = DeserializeObjectFromFileAsync<T>(file);
			var (tempLoaded, loadResult) = await deserializeTask;

			if (loadResult != LoadResult.Success)
			{
				Debug.LogWarning($"File \"{file}\" has invalid data ({loadResult.ToReadableString()}). Skipping it.");
				return;
			}

			if (IsNullOrEmpty(tempLoaded))
			{
				Debug.LogWarning($"File \"{file}\" is empty ({loadResult.ToReadableString()}). Skipping it.");
				return;
			}

			var filename = Path.GetFileName(file);
			var path = Path.GetFullPath(file).Replace('\\', '/');

			tempLoaded.PostLoad(path, filename);

			resultBag.Add(tempLoaded);
		}

		public void LoadAllFromLocal<T>(string folderPath, string fileType, List<T> outList) where T : ITextData
		{
			// based on https://stackoverflow.com/a/19850142/1377948
			// https://docs.microsoft.com/en-us/dotnet/api/system.collections.concurrent.concurrentbag-1
			// https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task.run

#if UNITY_EDITOR
			if (!_deserializeSampler.ContainsKey(fileType))
			{
				_deserializeSampler.Add(fileType, UnityEngine.Profiling.CustomSampler.Create($"{fileType} Deserialize from thread"));
			}
			_deserializeSampler[fileType].Begin();
#endif

			// Deserialization of each file is its own task.
			// This method will wait for all of those tasks to finish.

			var taskList = new List<Task>();
			var resultBag = new ConcurrentBag<T>();

			UnityEngine.Profiling.Profiler.BeginSample($"{fileType} LoadAllFromLocal: Directory traversal");
			foreach (string file in Directory.EnumerateFiles(folderPath, $"*{fileType}"))
			{
				//Debug.Log("found file: " + Path.GetFileName(file));

				Task deserializeAndAddToBag = Task.Run(() => DeserializeAndAddToBagAsync(file, resultBag));
				taskList.Add(deserializeAndAddToBag);
			}
			UnityEngine.Profiling.Profiler.EndSample();

			if (taskList.Count == 0)
			{
				// nothing to load
				// that means no file of the specified type was found in the specified path
				return;
			}

			UnityEngine.Profiling.Profiler.BeginSample($"{fileType} LoadAllFromLocal: Waiting for tasks to complete");
			Task.WaitAll(taskList.ToArray());
			UnityEngine.Profiling.Profiler.EndSample();
			// Note: could maybe use Task all = Task.WhenAll(taskList);

			while (!resultBag.IsEmpty)
			{
				if (resultBag.TryTake(out var result))
				{
					outList.Add(result);
				}
				else
				{
					Debug.LogError("Could not take from resultBag");
				}
			}
#if UNITY_EDITOR
			_deserializeSampler[fileType].End();
#endif
		}

		public void LoadAllFromResourcesFolder<T>(string folderPath, List<T> results) where T : ITextData
		{
			Object[] loadedTextAssets = Resources.LoadAll(folderPath, typeof(TextAsset));

			if (loadedTextAssets == null || loadedTextAssets.Length == 0)
			{
				Debug.LogWarning("none found in " + folderPath);
				return;
			}

			for (int n = 0, len = loadedTextAssets.Length; n < len; ++n)
			{
				TextAsset loadedText = loadedTextAssets[n] as TextAsset;

				if (loadedText == null)
				{
					continue;
				}

				if (string.IsNullOrEmpty(loadedText.text))
				{
					Debug.LogWarning($"TextAsset \"{loadedText.name}\" has no data inside. Skipping it.");
					continue;
				}

				var tempLoaded = FromSerializedString<T>(loadedText.text);

				if (tempLoaded == null)
				{
					Debug.LogWarning($"TextAsset \"{loadedText.name}\" has invalid data. Skipping it.");
					continue;
				}

				results.Add(tempLoaded);
			}
		}

		public (LoadResult result, string errorMessage) TryLoadFromResources<T>(string filePath,
			ref T resultingLoadedObject) where T : ITextData
		{
			var textAsset = Resources.Load<TextAsset>(filePath);

			if (textAsset == null)
			{
				return (LoadResult.OtherError, null);
			}

			string fileText = textAsset.text;
			if (string.IsNullOrWhiteSpace(fileText))
			{
				Debug.LogWarning($"TextAsset \"{textAsset.name}\" has no data inside. Skipping it.");
				return (LoadResult.FileIsEmpty, null);
			}

			resultingLoadedObject = FromSerializedString<T>(fileText);

			return (LoadResult.Success, null);
		}

		public (LoadResult result, string errorMessage) TryLoadBinaryFromResources<T>(string filePath,
			ref T resultingLoadedObject) where T : IBinaryData
		{
			var textAsset = Resources.Load<TextAsset>(filePath);

			if (textAsset == null)
			{
				return (LoadResult.OtherError, null);
			}

			byte[] bytes = textAsset.bytes;
			if (bytes.AllNull())
			{
				Debug.LogWarning($"TextAsset \"{textAsset.name}\" has no data inside. Skipping it.");
				return (LoadResult.FileIsEmpty, null);
			}

			bool loadSuccess;
			string errorMessage;
			using (var stream = new MemoryStream(bytes))
			{
				using (var reader = new BinaryReader(stream))
				{
					(loadSuccess, errorMessage) = resultingLoadedObject.LoadFromStream(reader);
				}
			}

			if (loadSuccess)
			{
				return (LoadResult.Success, null);
			}

			return (LoadResult.OtherError, errorMessage);
		}

		// ----------------------------------------------------------------------------------------------

#if UNITY_EDITOR
		public void SaveToAssets<T>(string saveFileAssetsPath, T data) where T : ITextData
		{
			SaveToLocal($"{FileUtil.ProjectPath}/{saveFileAssetsPath}", data);
		}
#endif

		public void SaveToStreamingAssets<T>(string fileAssetsPath, T data) where T : ITextData
		{
			SaveToLocal($"{Application.streamingAssetsPath}/{fileAssetsPath}", data);
		}

		public void SaveToLocal<T>(string saveFilePath, T data) where T : ITextData
		{
			if (string.IsNullOrEmpty(saveFilePath))
			{
				return;
			}
			if (data == null)
			{
				return;
			}

			var folder = Path.GetDirectoryName(saveFilePath);
			//Debug.LogFormat("folder of {0} is {1}", saveFilePath, folder);

			if (!string.IsNullOrEmpty(folder) && !Directory.Exists(folder))
			{
				Directory.CreateDirectory(folder);
			}

#if UNITY_WEBPLAYER && !UNITY_EDITOR
			BetterDebug.LogError("Current build target is set to Web Player. Cannot perform file input/output when in Web Player.");
#else
			data.PrepareSave();

			SerializeObjectToFile(data, saveFilePath);
#endif
		}

		// ----------------------------------------------------------------------------------------------

#if UNITY_EDITOR
		public void SaveToAssets<T>(string saveFileAssetsPath, T data, out string hash, string hashAlgorithmName = ITextDataIO.DEFAULT_HASH_ALGORITHM) where T : ITextData
		{
			SaveToLocal($"{FileUtil.ProjectPath}/{saveFileAssetsPath}", data, out hash, hashAlgorithmName);
		}
#endif

		public void SaveToStreamingAssets<T>(string fileAssetsPath, T data, out string hash, string hashAlgorithmName = ITextDataIO.DEFAULT_HASH_ALGORITHM) where T : ITextData
		{
			SaveToLocal($"{Application.streamingAssetsPath}/{fileAssetsPath}", data, out hash, hashAlgorithmName);
		}

		public void SaveToLocal<T>(string saveFilePath, T data, out string hash, string hashAlgorithmName = ITextDataIO.DEFAULT_HASH_ALGORITHM) where T : ITextData
		{
			if (string.IsNullOrEmpty(saveFilePath))
			{
				hash = null;
				return;
			}
			if (data == null)
			{
				hash = null;
				return;
			}

			var folder = Path.GetDirectoryName(saveFilePath);
			//Debug.LogFormat("folder of {0} is {1}", saveFilePath, folder);

			if (!string.IsNullOrEmpty(folder) && !Directory.Exists(folder))
			{
				Directory.CreateDirectory(folder);
			}

#if UNITY_WEBPLAYER && !UNITY_EDITOR
			BetterDebug.LogError("Current build target is set to Web Player. Cannot perform file input/output when in Web Player.");
#else
			data.PrepareSave();

			SerializeObjectToFile(data, saveFilePath, out hash, hashAlgorithmName);
#endif
		}

		// ----------------------------------------------------------------------------------------------

		public void SaveBinaryToLocal<T>(string saveFileAbsolutePath, T data) where T : IBinaryData
		{
			if (string.IsNullOrEmpty(saveFileAbsolutePath))
			{
				return;
			}
			if (data == null)
			{
				return;
			}

			string folder = Path.GetDirectoryName(saveFileAbsolutePath);
			if (!string.IsNullOrEmpty(folder) && !Directory.Exists(folder))
			{
				Directory.CreateDirectory(folder);
			}

			using var writer = new BinaryWriter(File.Open(saveFileAbsolutePath, FileMode.Create));
			data.SaveToStream(writer);
		}

		// ----------------------------------------------------------------------------------------------

		public string ToSerializedString<T>(T data) where T : ITextData
		{
			if (data == null)
			{
				return null;
			}

			data.PrepareSave();

			return SerializeObject(data);
		}

		public string ToSerializedString<T>(T data, out string hash) where T : ITextData
		{
			if (data == null)
			{
				hash = null;
				return null;
			}

			data.PrepareSave();

			return SerializeObject(data, out hash);
		}

		public string ToHash<T>(T data) where T : ITextData
		{
			if (data == null)
			{
				return null;
			}

			data.PrepareSave();

			return GetObjectHash(data);
		}

		// ----------------------------------------------------------------------------------------------

		public T FromSerializedString<T>(string fileText) where T : ITextData
		{
			T result = _FromSerializedString<T>(fileText);

			if (result != null)
			{
				result.PostLoad(string.Empty, string.Empty);
			}

			return result;
		}

		T _FromSerializedString<T>(string fileText) where T : ITextData
		{
			if (string.IsNullOrEmpty(fileText))
			{
				Debug.LogError("null data received");
				return default;
			}

			T loadedObject = DeserializeObject<T>(fileText);

			return loadedObject;
		}

		// ----------------------------------------------------------------------------------------------
	}

} // end namespace DLD_Utility