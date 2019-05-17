using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FluentFiles.Core;
using FluentFiles.Core.Base;
using FluentFiles.FixedLength;
using FluentFiles.FixedLength.Implementation;
using FluentAssertions;
using Xunit;
using System.Threading.Tasks;

namespace FluentFiles.Tests.FixedLength
{
    public class FixedLengthMasterDetailCustomTests
    {
        private readonly IFlatFileMultiEngine engine;

        const string TestData = @"HHeader                     
MHeaderLine2                     
MHeaderLine3                     
D20150323Some Data                     
D20150512More Data                     
HAnotherHeader                     
D20150511FooBarBaz                     
SNonHeaderRecord                     
D20150512Standalone                     ";

        [Fact]
        public async Task ShouldAssociateDetailRecordsWithMasterRecord()
        {
            // Act.
            ReadResult result;
            using (var reader = new StringReader(TestData))
                result = await engine.ReadAsync(reader);

            var parents = result.GetRecords<HeaderRecord>().ToList();
            var continuations = result.GetRecords<HeaderRecordContinuation>().ToList();

            // Assert.
            var firstParent = parents.Single(r => r.Data == "Header");
            firstParent.Should().NotBeNull();

            var secondParent = continuations.Single(r => r.Data == "HeaderLine2");
            secondParent.Should().NotBeNull();

            var thirdParent = continuations.Single(r => r.Data == "HeaderLine3");
            thirdParent.Should().NotBeNull();

            firstParent.DetailRecords.Should().BeEmpty();
            secondParent.DetailRecords.Should().BeEmpty();
            thirdParent.DetailRecords.Should().HaveCount(2);

            var thirdParentChild = thirdParent.DetailRecords.Last();
            thirdParentChild.Should().NotBeNull();
            thirdParentChild.Data.Should().Be("20150512More Data", "It should preserve ordering");

            var anotherParent = parents.FirstOrDefault(r => r.Data == "AnotherHeader");
            anotherParent.Should().NotBeNull();
            anotherParent.DetailRecords.Should().HaveCount(1);

            var anotherChild = anotherParent.DetailRecords.First();
            anotherChild.Should().NotBeNull();
            anotherChild.Data.Should().Be("20150511FooBarBaz");

            result.GetRecords<DetailRecord>().Should().HaveCount(1, "Only unassociated detail records should be available.");
        }

        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
        public sealed class MasterAttribute : Attribute { }

        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
        public sealed class DetailAttribute : Attribute { }

        abstract class RecordBase
        {
            public char Type { get; set; }
            public string Data { get; set; }

            bool Equals(RecordBase other) { return Type == other.Type && string.Equals(Data, other.Data); }

            public override int GetHashCode() => HashCode.Combine(Type, Data);

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj.GetType() == GetType() && Equals((RecordBase)obj);
            }
        }

        [Master]
        class HeaderRecord : RecordBase
        {
            public IList<DetailRecord> DetailRecords { get; protected set; } = new List<DetailRecord>();
            public HeaderRecord()
            {
                Type = 'H';
            }
        }

        class HeaderRecordContinuation : HeaderRecord
        {
            public HeaderRecordContinuation()
            {
                Type = 'M';
            }
        }

        [Detail]
        class DetailRecord : RecordBase
        {
            public DetailRecord() { Type = 'D'; }
        }

        class StandaloneRecord : RecordBase
        {
            public StandaloneRecord() { Type = 'S'; }
        }

        abstract class RecordBaseLayout<T> : FixedLayout<T> where T : RecordBase
        {
            protected RecordBaseLayout()
            {
                this.WithMember(x => x.Type, c => c.WithLength(1))
                    .WithMember(x => x.Data, c => c.WithLength(20).WithRightPadding(' '));
            }
        }

        class HeaderLayout : RecordBaseLayout<HeaderRecord> { }

        class HeaderContinuationLayout : RecordBaseLayout<HeaderRecordContinuation> { }

        class DetailLayout : RecordBaseLayout<DetailRecord> { }

        class StandaloneLayout : RecordBaseLayout<StandaloneRecord> { }

        public FixedLengthMasterDetailCustomTests()
        {
            var layouts = new List<IFixedLengthLayoutDescriptor>
            {
                new HeaderLayout(),
                new HeaderContinuationLayout(),
                new DetailLayout(),
                new StandaloneLayout()
            };
            engine = new FixedLengthFileMultiEngine(layouts,
                                                    (s, i) =>
                                                    {
                                                        if (String.IsNullOrEmpty(s) || s.Length < 1) return null;

                                                        switch (s[0])
                                                        {
                                                            case 'H':
                                                                return typeof(HeaderRecord);
                                                            case 'M':
                                                                return typeof(HeaderRecordContinuation);
                                                            case 'S':
                                                                return typeof(StandaloneRecord);
                                                            case 'D':
                                                                return typeof(DetailRecord);
                                                            default:
                                                                return null;
                                                        }
                                                    },
                                                    new FixedLengthLineBuilderFactory(),
                                                    new FixedLengthLineParserFactory(),
                                                    new DelegatingMasterDetailStrategy(
                                                        x => x.GetType().GetCustomAttribute<MasterAttribute>(true) != null,
                                                        x => x.GetType().GetCustomAttribute<DetailAttribute>(true) != null,
                                                        (master, detail) => ((HeaderRecord)master).DetailRecords.Add((DetailRecord)detail)));
        }
    }
}