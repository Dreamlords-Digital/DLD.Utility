// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using DLD.JsonFx;
using DLD.Utility;
using UnityEngine;
using UnityEngine.Assertions;

namespace DLD.Serializer
{
	public class JsonFxTextDataIO : BaseTextDataIO
	{
		protected override string GetFileContents(string filePath)
		{
			return File.ReadAllText(filePath);
		}

		static readonly SerializationRuleType Rule = type =>
		{
			switch (type)
			{
				case FieldInfo field:
				{
					// To be serialized, either:
					// 1. public field that doesn't have [NotSerialized]
					// 2. private/protected field that has [Serialized]
					return (field.IsPublic && type.GetCustomAttributes(typeof(NotSerializedAttribute), true).Length == 0) ||
					       (!field.IsPublic && type.GetCustomAttributes(typeof(SerializedAttribute), true).Length > 0);
				}
				case PropertyInfo { CanRead: true, CanWrite: true } property:
				{
					// To be serialized, either:
					// 1. property whose get and set are both public, and doesn't have [NotSerialized]
					// 2. property whose get and/or set are private/protected, and has [Serialized]

					if (property.GetMethod.IsPublic && property.SetMethod.IsPublic)
					{
						// must not have the NotSerializedAttribute
						return type.GetCustomAttributes(typeof(NotSerializedAttribute), true).Length == 0;
					}
					else
					{
						// We reached here because either:
						// 1. get is private/protected, while set is public
						// 2. get is public, while set is private/protected
						// 3. both get and set are private/protected
						//
						// must have the SerializedAttribute
						return type.GetCustomAttributes(typeof(SerializedAttribute), true).Length > 0;
					}
				}
				default:
					// This delegate only gets called on fields and properties.
					// That means if we reached here, this is probably a property that is read-only or write-only.
					return false;
			}
		};

		static readonly SerializedNameType Name = memberInfo =>
		{
			var attribute =
				Attribute.GetCustomAttribute(memberInfo, typeof(SerializedAttribute)) as SerializedAttribute;
			return attribute?.Name;
		};

		readonly JsonReaderSettings _readerSettings = new JsonReaderSettings();
		readonly JsonWriterSettings _writerSettings = new JsonWriterSettings();

		bool _readerInitialized;
		bool _writerInitialized;

		void InitializeReaderIfNeeded()
		{
			if (!_readerInitialized)
			{
				InitializeReaderIfNeeded(_readerSettings);
				_readerInitialized = true;
			}
		}

		void InitializeReaderIfNeeded(JsonReaderSettings readerSettings)
		{
			readerSettings.AllowNullValueTypes = true;
			readerSettings.AllowUnquotedObjectKeys = true;
			readerSettings.TypeHintName = TYPE_HINT_NAME;
			readerSettings.SetFieldSerializationRule(Rule);
			readerSettings.SetFieldSerializedName(Name);
			AdditionalReaderInitialization(readerSettings);
		}

		protected virtual void AdditionalReaderInitialization(JsonReaderSettings readerSettings)
		{
		}

		void InitializeWriterIfNeeded()
		{
			if (!_writerInitialized)
			{
				_writerSettings.PrettyPrint = true;
				_writerSettings.Tab = "  ";
				_writerSettings.TypeHintName = TYPE_HINT_NAME;
				_writerSettings.TypeHintsOnlyWhenNeeded = true;
				_writerSettings.SetFieldSerializationRule(Rule);
				_writerSettings.SetFieldSerializedName(Name);

				_writerInitialized = true;
			}
		}

		protected override async Task<(T, LoadResult)> DeserializeObjectFromFileAsync<T>(string filePath)
		{
			Task<string> fileRead = ReadTextAsync(filePath);

			string fileText = await fileRead;

			if (string.IsNullOrEmpty(fileText))
			{
				return (default, LoadResult.FileIsEmpty);
			}

			InitializeReaderIfNeeded();
			// Note that this means the TypeCoercionUtility inside _readerSettings is shared among multiple threads.
			var reader = new JsonReader(fileText, _readerSettings);

			T returnValue;

			try
			{
				returnValue = (T)reader.Deserialize(typeof(T));
			}
			catch (JsonDeserializationException e)
			{
				if (FileUtil.IsFileAllNull(filePath))
				{
					return (default, LoadResult.FileIsAllNull);
				}

				Debug.LogError($"Error in {filePath} for {typeof(T).Name}:\n{e}");
				return (default, LoadResult.OtherError);
			}
			catch (Exception e)
			{
				if (FileUtil.IsFileAllNull(filePath))
				{
					return (default, LoadResult.FileIsAllNull);
				}

				Debug.LogError($"Error in {filePath} for {typeof(T).Name}:\n{e}");
				return (default, LoadResult.OtherError);
			}

			return (returnValue, LoadResult.Success);
		}

		protected override (LoadResult, string) DeserializeObjectFromFile<T>(string filePath, out T returnValue)
		{
			InitializeReaderIfNeeded();
			var streamReader = File.OpenText(filePath);
			var reader = new JsonReader(streamReader, _readerSettings);

			try
			{
				returnValue = (T)reader.Deserialize(typeof(T));
			}
			catch (JsonDeserializationException e)
			{
				returnValue = default;
				streamReader.Close();
				streamReader.Dispose();

				if (FileUtil.IsFileAllNull(filePath))
				{
					return (LoadResult.FileIsAllNull, null);
				}

				string errorMsg = e.ToString();
				Debug.LogError($"Error in {filePath} for {typeof(T).Name}:\n{errorMsg}");
				return (LoadResult.OtherError, errorMsg);
			}
			catch (Exception e)
			{
				returnValue = default;
				streamReader.Close();
				streamReader.Dispose();

				if (FileUtil.IsFileAllNull(filePath))
				{
					return (LoadResult.FileIsAllNull, null);
				}

				string errorMsg = e.ToString();
				Debug.LogError($"Error in {filePath} for {typeof(T).Name}:\n{errorMsg}");
				return (LoadResult.OtherError, errorMsg);
			}

			streamReader.Close();
			streamReader.Dispose();

			return (LoadResult.Success, null);
		}

		protected override void SerializeObjectToFile<T>(T data, string filePath)
		{
			InitializeWriterIfNeeded();
			using var writer = new JsonWriter(filePath, _writerSettings);
			writer.Write(data);
		}

		protected override void SerializeObjectToFile<T>(T data, string filePath, out string hash, string hashAlgorithmName = ITextDataIO.DEFAULT_HASH_ALGORITHM)
		{
			InitializeWriterIfNeeded();
			using var writer = new JsonWriterWithHasher(filePath, _writerSettings, hashAlgorithmName);
			writer.Write(data);

			byte[] hashBytes = writer.EndHash();
			Assert.AreEqual(16, hashBytes.Length,
				$"hashBytes.Length is {hashBytes.Length.ToString()}, should be 16");
			hash = hashBytes.Byte16ToString();
		}

		protected override T DeserializeObject<T>(string fileText)
		{
			InitializeReaderIfNeeded();
			var reader = new JsonReader(fileText, _readerSettings);

			return (T)reader.Deserialize(typeof(T));
		}

		protected override string SerializeObject<T>(T data)
		{
			InitializeWriterIfNeeded();
			TextWriter stringWriter = new StringWriter();
			var writer = new JsonWriter(stringWriter, _writerSettings);

			writer.Write(data);
			stringWriter.Flush();

			var returnValue = stringWriter.ToString();
			stringWriter.Dispose();

			return returnValue;
		}

		protected override string SerializeObject<T>(T data, out string hash, string hashAlgorithmName = ITextDataIO.DEFAULT_HASH_ALGORITHM)
		{
			InitializeWriterIfNeeded();
			TextWriter stringWriter = new StringWriter();
			var writer = new JsonWriterWithHasher(stringWriter, _writerSettings, hashAlgorithmName);

			writer.Write(data);

			byte[] hashBytes = writer.EndHash();
			Assert.AreEqual(16, hashBytes.Length,
				$"hashBytes.Length is {hashBytes.Length.ToString()}, should be 16");
			hash = hashBytes.Byte16ToString();

			stringWriter.Flush();

			var returnValue = stringWriter.ToString();
			stringWriter.Dispose();

			return returnValue;
		}

		/// <summary>
		/// Generates an MD5 hash out of the binary form of the serializable fields and properties of the passed data.
		/// That means any variables that are configured to be not serialized are not included in the hash.
		/// The hash is then converted into a string for ease of use.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="hashAlgorithmName"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		protected override string GetObjectHash<T>(T data, string hashAlgorithmName = ITextDataIO.DEFAULT_HASH_ALGORITHM)
		{
			var hashGenerator = new HashGenerator(_writerSettings, hashAlgorithmName);
			hashGenerator.Write(data);

			byte[] hashBytes = hashGenerator.EndHash();
			Assert.AreEqual(16, hashBytes.Length,
				$"hashBytes.Length is {hashBytes.Length.ToString()}, should be 16");
			return hashBytes.Byte16ToString();
		}
	}
}