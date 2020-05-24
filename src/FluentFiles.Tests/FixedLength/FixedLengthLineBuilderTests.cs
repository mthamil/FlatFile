namespace FluentFiles.Tests.FixedLength
{
    using System;
    using System.Globalization;
    using Xunit;
    using FluentAssertions;
    using FluentFiles.Core.Conversion;
    using FluentFiles.FixedLength;
    using FluentFiles.FixedLength.Implementation;
    using FluentFiles.Tests.Base.Entities;
    
    public class FixedLengthLineBuilderTests
    {
        private readonly FixedLengthLineBuilder builder;
        private readonly IFixedLayout<TestObject> layout;

        public FixedLengthLineBuilderTests()
        {
            layout = new FixedLayout<TestObject>();

            builder = new FixedLengthLineBuilder(layout);
        }

        [Fact]
        public void BuilderShouldHandleMultipleFields()
        {
            layout.WithMember(o => o.Id, set => set.WithLength(4).WithConverter<IdHexConverter>())
                  .WithMember(o => o.Description, set => set.WithLength(7));

            var entry = new TestObject
            {
                Id = 48879,
                Description = "testing"
            };

            var line = builder.BuildLine(entry);

            line.Should().Be("BEEFtesting");
        }

        [Fact]
        public void BuilderShouldUseConverter()
        {
            layout.WithMember(o => o.Id, set => set.WithLength(4).WithConverter<IdHexConverter>());

            var entry = new TestObject
            {
                Id = 48879
            };

            var line = builder.BuildLine(entry);

            line.Should().Be("BEEF");
        }

        [Fact]
        public void BuilderShouldUseConversionFunction()
        {
            layout.WithMember(o => o.Id, set => set.WithLength(4).WithConversionToString((int id) => id.ToString("X")));

            var entry = new TestObject
            {
                Id = 48879
            };

            var line = builder.BuildLine(entry);

            line.Should().Be("BEEF");
        }

        [Fact]
        public void BuilderShouldFillIgnoredFieldWithSpacesByDefault()
        {
            layout.Ignore(5);

            var entry = new TestObject();

            var line = builder.BuildLine(entry);

            line.Should().Be("     ");
        }

        [Fact]
        public void BuilderShouldFillIgnoredFieldWithSelectedCharacter()
        {
            layout.Ignore(4, c => c.FillWith('0'));

            var entry = new TestObject();

            var line = builder.BuildLine(entry);

            line.Should().Be("0000");
        }

        class IdHexConverter : ConverterBase<int>
        {
            protected override int ParseValue(in FieldParsingContext context)
            {
                return Int32.Parse(context.Source, NumberStyles.AllowHexSpecifier);
            }

            protected override string FormatValue(in FieldFormattingContext<int> context)
            {
                return context.Source.ToString("X");
            }
        }
    }
}
