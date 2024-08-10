// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using DLD.JsonFx;
using NUnit.Framework;

namespace DLD.Serializer.Tests
{
	[TestFixture]
	public class TextDataJsonFxTests : BaseTextDataTests
	{
		class TestableJsonFxTextDataIO : JsonFxTextDataIO
		{
			protected override void AdditionalReaderInitialization(JsonReaderSettings readerSettings)
			{
				string thisAssemblyName = GetType().Assembly.GetName().Name;
				readerSettings.AssemblyNamesToSearchThroughIfNotFound.Add(thisAssemblyName);
			}
		}

		protected override ITextDataIO GetTextDataIOInstance()
		{
			return new TestableJsonFxTextDataIO();
		}
	}
}