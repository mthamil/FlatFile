namespace FluentFiles.Tests.Base
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using FluentFiles.Core;
    using FluentFiles.Tests.Base.Entities;
    using Xunit;

    public abstract class IntegrationTests<TFieldSettings, TConstructor, TLayout>
        where TLayout : ILayout<TestObject, TFieldSettings, TConstructor, TLayout>
        where TFieldSettings : IFieldSettingsContainer
        where TConstructor : IFieldSettingsBuilder<TConstructor, TFieldSettings>
    {
        protected abstract TLayout Layout { get; }

        protected IList<TestObject> Objects { get; set; }

        protected abstract IFlatFileEngine Engine { get; }

        public abstract string TestSource { get; }

        protected IntegrationTests()
        {
            Objects = new List<TestObject>();

            for (int i = 1; i <= 10; i++)
            {
                Objects.Add(new TestObject
                {
                    Id = i,
                    Description = "Description " + i,
                    NullableInt = i % 5 == 0 ? null : (int?)3
                });
            }
        }

        [Fact]
        public virtual Task CountOfTheObjectsAfterWriteReadShouldBeTheSame()
        {
            return InvokeWriteTest(async (engine, stream) =>
            {
                using var reader = new StreamReader(stream);
                var objectsAfterRead = await engine.ReadAsync<TestObject>(reader).ToListAsync();

                objectsAfterRead.Should().HaveCount(Objects.Count);
            });
        }

        [Fact]
        public virtual Task AllDeclaredPropertiesOfTheObjectsAfterWriteReadShouldBeTheSame()
        {
            return InvokeWriteTest(async (engine, stream) =>
            {
                using var reader = new StreamReader(stream);
                var objectsAfterRead = await engine.ReadAsync<TestObject>(reader).ToListAsync();

                objectsAfterRead.Should().BeEquivalentTo(Objects, options => options.IncludingAllDeclaredProperties());
            });
        }

        [Fact]
        public void AllDeclaredPropertiesOfTheObjectsAfterReadFromSourceShouldBeTheSame()
        {
            InvokeReadTest(async (engine, stream) =>
            {
                using var reader = new StreamReader(stream);
                var objectsAfterRead = await engine.ReadAsync<TestObject>(reader).ToListAsync();

                objectsAfterRead.Should().BeEquivalentTo(Objects, options => options.IncludingAllDeclaredProperties());

            }, TestSource);
        }

        protected virtual async Task InvokeWriteTest(Action<IFlatFileEngine, MemoryStream> action, CancellationToken cancellationToken = default)
        {
            using var memory = new MemoryStream();
            await Engine.WriteAsync(memory, Objects, cancellationToken);

            memory.Seek(0, SeekOrigin.Begin);
            action(Engine, memory);
        }

        protected virtual void InvokeReadTest(Action<IFlatFileEngine, MemoryStream> action, string textSource)
        {
            using var memory = new MemoryStream(Encoding.UTF8.GetBytes(textSource));
            action(Engine, memory);
        }
    }
}