using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FakeItEasy;
using FluentFiles.FixedLength;
using FluentFiles.FixedLength.Implementation;
using Xunit;

namespace FluentFiles.Tests.FixedLength
{
    public class FixedLengthFileEngineWriteTests
    {
        private readonly IFixedLengthLineBuilder _lineBuilder = A.Fake<IFixedLengthLineBuilder>();
        private readonly IFixedLengthLineBuilderFactory _lineBuilderFactory;
        private readonly FixedLengthFileEngine _fileEngine;

        private readonly IFixture _fixture = new Fixture();

        public FixedLengthFileEngineWriteTests()
        {
            A.CallTo(() => _lineBuilder.BuildLine(A<TestRecord>.Ignored))
                .ReturnsLazily((TestRecord r) => r.ToString());

            _lineBuilderFactory = A.Fake<IFixedLengthLineBuilderFactory>();
            A.CallTo(() => _lineBuilderFactory.GetBuilder(A<IFixedLengthLayoutDescriptor>.Ignored))
                .Returns(_lineBuilder);

            _fileEngine = new FixedLengthFileEngine(
                A.Fake<IFixedLengthLayoutDescriptor>(),
                _lineBuilderFactory,
                new FixedLengthLineParserFactory());
        }

        [Fact]
        public async Task WriteIsCancellable()
        {
            // Arrange.
            var writeCount = 0;
            var cancellationTokenSource = new CancellationTokenSource();

            A.CallTo(() => _lineBuilder.BuildLine(A<TestRecord>.Ignored))
                .Invokes(() =>
                {
                    if (writeCount++ > 3)
                        cancellationTokenSource.Cancel();
                })
                .ReturnsLazily((TestRecord r) => r.ToString());

            var records = _fixture.CreateMany<TestRecord>(10);

            // Act.
            var stringBuilder = new StringBuilder();
            using (var writer = new StringWriter(stringBuilder))
                await Assert.ThrowsAsync<OperationCanceledException>(() => _fileEngine.WriteAsync(writer, records, cancellationTokenSource.Token));

            // Assert.
            var writtenLines = stringBuilder.ToString().Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            Assert.Equal(writtenLines, records.Take(5).Select(r => r.ToString()));
        }

        private class TestRecord
        {
            public int Id { get; set; }

            public override string ToString() => Id.ToString();
        }
    }
}
