using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using BenchmarkDotNet.Attributes;
using CsvHelper;
using FluentFiles.Benchmark.Entities;
using FluentFiles.Benchmark.Mapping;
using FluentFiles.Core;
using FluentFiles.Delimited.Implementation;
using Configuration = CsvHelper.Configuration.Configuration;

namespace FluentFiles.Benchmark
{
    public class FluentFilesVsCsvHelperWrite
    {
        private Configuration _csvConfig;
        private IFlatFileEngine _fluentEngine;

        private CustomObject[] _records;

        [Params(10, 100, 1000, 10000, 100000)]
        public int N;

        [GlobalSetup]
        public void Setup()
        {
            _csvConfig = new Configuration();
            _csvConfig.RegisterClassMap<CsvHelperMappingForCustomObject>();

            _fluentEngine = new DelimitedFileEngineFactory()
                .GetEngine(new FlatFileMappingForCustomObject());

            var fixture = new Fixture();
            _records = fixture.CreateMany<CustomObject>(N).ToArray();
        }

        [Benchmark(Baseline = true)]
        public void CsvHelper()
        {
            using (var stream = new MemoryStream())
            using (var streamWriter = new StreamWriter(stream))
            using (var writer = new CsvWriter(streamWriter, _csvConfig))
            {
                writer.WriteRecords(_records);
                streamWriter.Flush();
            }
        }

        [Benchmark]
        public async Task FluentFiles()
        {
            using (var stream = new MemoryStream())
            using (var streamWriter = new StreamWriter(stream))
            {
                await _fluentEngine.WriteAsync(streamWriter, _records).ConfigureAwait(false);
            }
        }
    }
}