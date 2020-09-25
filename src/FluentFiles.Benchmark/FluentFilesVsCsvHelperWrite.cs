using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using BenchmarkDotNet.Attributes;
using CsvHelper;
using CsvHelper.Configuration;
using FluentFiles.Benchmark.Entities;
using FluentFiles.Benchmark.Mapping;
using FluentFiles.Core;
using FluentFiles.Delimited.Implementation;

namespace FluentFiles.Benchmark
{
    public class FluentFilesVsCsvHelperWrite
    {
        private CsvConfiguration _csvConfig;
        private IFlatFileEngine _fluentEngine;

        private CustomObject[] _records;

        [Params(10, 100, 1000, 10000, 100000)]
        public int N;

        [GlobalSetup]
        public void Setup()
        {
            _csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture);
            _csvConfig.RegisterClassMap<CsvHelperMappingForCustomObject>();

            _fluentEngine = new DelimitedFileEngineFactory()
                .GetEngine(new FlatFileMappingForCustomObject());

            var fixture = new Fixture();
            _records = fixture.CreateMany<CustomObject>(N).ToArray();
        }

        [Benchmark(Baseline = true)]
        public async Task CsvHelper()
        {
            using var stream = new MemoryStream();
            using var streamWriter = new StreamWriter(stream);
            using var writer = new CsvWriter(streamWriter, _csvConfig);

            await writer.WriteRecordsAsync(_records).ConfigureAwait(false);
            await streamWriter.FlushAsync().ConfigureAwait(false);
        }

        [Benchmark]
        public async Task FluentFiles()
        {
            using var stream = new MemoryStream();
            using var streamWriter = new StreamWriter(stream);

            await _fluentEngine.WriteAsync(streamWriter, _records).ConfigureAwait(false);
        }
    }
}