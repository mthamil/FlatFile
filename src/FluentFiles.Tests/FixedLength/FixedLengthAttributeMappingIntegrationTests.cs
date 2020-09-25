namespace FluentFiles.Tests.FixedLength
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using FakeItEasy;
    using FluentAssertions;
    using FluentFiles.Core;
    using FluentFiles.Core.Base;
    using FluentFiles.Core.Conversion;
    using FluentFiles.FixedLength;
    using FluentFiles.FixedLength.Attributes;
    using FluentFiles.FixedLength.Implementation;
    using FluentFiles.Tests.Base.Entities;
    using Xunit;

    public class FixedLengthAttributeMappingIntegrationTests : FixedLengthIntegrationTests
    {
        readonly IFlatFileEngineFactory<IFixedLengthLayoutDescriptor, IFixedFieldSettingsContainer> fileEngineFactory;

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

        public FixedLengthAttributeMappingIntegrationTests()
        {
            fileEngineFactory = new FixedLengthFileEngineFactory();
        }

        protected override IFlatFileEngine Engine => fileEngineFactory.GetEngine<TestObject>();

        [Fact]
        public async Task EngineShouldCallTypeConverterWhenConverterAttributeIsPresent()
        {
            var converter = new StubConverter();

            var attribute = A.Fake<IFixedFieldSettings>();
            A.CallTo(() => attribute.Index).Returns(1);
            A.CallTo(() => attribute.Length).Returns(1);
            A.CallTo(() => attribute.Converter).Returns(converter);

            var properties = typeof (ConverterTestObject)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .ToDictionary(info => info.Name);

            var fields = new FieldCollection<IFixedFieldSettingsContainer>();
            fields.AddOrUpdate(new FixedFieldSettings(properties["Foo"], attribute));

            var descriptor = new FixedLayout<ConverterTestObject>(new FixedFieldSettingsBuilderFactory(), fields) {HasHeader = false};

            var engine = fileEngineFactory.GetEngine(descriptor);

            using var stream = new MemoryStream();
            using var writer = new StreamWriter(stream);
            await writer.WriteLineAsync("A");
            await writer.FlushAsync();
            stream.Seek(0, SeekOrigin.Begin);

            // Capture first result to force enumerable to be iterated
            using var reader = new StreamReader(stream);
            var result = await engine.ReadAsync<ConverterTestObject>(reader).FirstOrDefaultAsync();

            result.Foo.Should().Be("foo");
        }
    }
}