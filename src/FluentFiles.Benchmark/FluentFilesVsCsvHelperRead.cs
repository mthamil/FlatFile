using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using CsvHelper;
using CsvHelper.Configuration;
using FluentFiles.Benchmark.Entities;
using FluentFiles.Benchmark.Mapping;
using FluentFiles.Core;
using FluentFiles.Delimited.Implementation;

namespace FluentFiles.Benchmark
{
    public class FluentFilesVsCsvHelperRead
    {
        private CsvConfiguration _csvConfig;
        private IFlatFileEngine _fluentEngine;

        private string _records;

        [Params(10, 100, 1000, 10000, 100000)]
        public int N;

        [GlobalSetup]
        public void Setup()
        {
            _csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture);
            _csvConfig.RegisterClassMap<CsvHelperMappingForCustomObject>();

            _fluentEngine = new DelimitedFileEngineFactory()
                .GetEngine(new FlatFileMappingForCustomObject());

            var records = new StringBuilder(N * 80);
            records.AppendLine("String Column,Int Column,Guid Column,Custom Type Column");
            for (int i = 0; i < N; i++)
            {
                records.AppendLine($"\"{i + 1}\",{i + 1},{Guid.NewGuid():D},{i + 1}|{i + 2}|{i + 3}");
            }

            _records = records.ToString();
        }

        [Benchmark(Baseline = true)]
        public ValueTask<List<CustomObject>> CsvHelper()
        {
            using var streamReader = new StringReader(_records);
            using var reader = new CsvReader(streamReader, _csvConfig);

            return reader.GetRecordsAsync<CustomObject>().ToListAsync();
        }

        [Benchmark]
        public ValueTask<List<CustomObject>> FluentFiles()
        {
            using var streamReader = new StringReader(_records);
            return _fluentEngine.ReadAsync<CustomObject>(streamReader).ToListAsync();
        }
    }
}