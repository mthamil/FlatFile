namespace FluentFiles.Tests.Delimited
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using FakeItEasy;
    using FluentFiles.Core;
    using FluentFiles.Core.Base;
    using FluentFiles.Core.Conversion;
    using FluentFiles.Delimited;
    using FluentFiles.Delimited.Attributes;
    using FluentFiles.Delimited.Implementation;
    using FluentFiles.Tests.Base.Entities;
    using Xunit;

    public class DelimitedAttributeMappingIntegrationTests : DelimitedIntegrationTests
    {
        private readonly IFlatFileEngineFactory<IDelimitedLayoutDescriptor, IDelimitedFieldSettingsContainer> _fileEngineFactory;

        public DelimitedAttributeMappingIntegrationTests()
        {
            _fileEngineFactory = new DelimitedFileEngineFactory();
        }

        protected override IFlatFileEngine Engine => _fileEngineFactory.GetEngine<TestObject>();

        class ConverterTestObject
        {
            public string Foo { get; set; }
        }

        class StubConverter : IFieldValueConverter
        {
            public bool CanParse(Type to) => true;
            public bool CanFormat(Type from) => true;
            public object Parse(in FieldParsingContext context) => "foo";
            public string Format(in FieldFormattingContext context) => context.Source.ToString();
        }

        [Fact]
        public async Task EngineShouldCallTypeConverterWhenConverterAttributeIsPresent()
        {
            // a converter to convert "A" to "foo"
            var converter = new StubConverter();

            // an attribute to assign the property
            var attribute = A.Fake<IDelimitedFieldSettings>();
            A.CallTo(() => attribute.Index).Returns(1);
            A.CallTo(() => attribute.Converter).Returns(converter);

            // the properties of the class
            var properties = typeof(ConverterTestObject).GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(info => info.Name);

            // assign the attribute to the Foo property
            var fields = new FieldCollection<IDelimitedFieldSettingsContainer>();
            fields.AddOrUpdate(new DelimitedFieldSettings(properties["Foo"], attribute));

            var layout = new DelimitedLayout<ConverterTestObject>(new DelimitedFieldSettingsBuilderFactory(), fields);
            var engine = _fileEngineFactory.GetEngine(layout);

            // write "A" to the stream and verify it is converted to "foo"
            using var stream = new MemoryStream();
            using var writer = new StreamWriter(stream);
            await writer.WriteLineAsync("A");
            await writer.FlushAsync();
            stream.Seek(0, SeekOrigin.Begin);

            // Capture first result to force enumerable to be iterated
            using var reader = new StreamReader(stream);
            var result = await engine.ReadAsync<ConverterTestObject>(reader).FirstOrDefaultAsync();
            Assert.Equal("foo", result.Foo);
        }
    }
}