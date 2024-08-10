// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using UnityEngine.Assertions;

namespace DLD.Serializer
{
	/// <summary>
	/// Standard serializer for <see cref="ITextData"/>.
	/// Provides an instance that implements <see cref="ITextDataIO"/>.
	/// Currently uses JsonFx.
	/// </summary>
	public class DefaultSerializer
	{
		static ITextDataIO _defaultTextDataProvider;

		/// <summary>
		/// Standard serializer for <see cref="ITextData"/>.
		/// Currently uses JsonFx.
		/// </summary>
		public static ITextDataIO Instance
		{
			get
			{
				if (_defaultTextDataProvider == null)
				{
					Initialize();
					Assert.IsNotNull(_defaultTextDataProvider,
						"Could not initialize default text data provider. DefaultSerializer.Initialize() should have set it.");
				}

				return _defaultTextDataProvider;
			}
		}

		static void Initialize()
		{
			_defaultTextDataProvider = new JsonFxTextDataIO();
			//_defaultTextDataProvider = new JsonUtilityTextDataIO();
		}
	}
}