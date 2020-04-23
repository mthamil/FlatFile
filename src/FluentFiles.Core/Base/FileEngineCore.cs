namespace FluentFiles.Core.Base
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentFiles.Core;

    /// <summary>
    /// Reads and writes file records.
    /// </summary>
    /// <typeparam name="TFieldSettings">The type of field configuration.</typeparam>
    /// <typeparam name="TLayoutDescriptor">The type of layout descriptor.</typeparam>
    public abstract class FileEngineCore<TFieldSettings, TLayoutDescriptor> : IFileWriter
        where TFieldSettings : IFieldSettings
        where TLayoutDescriptor : ILayoutDescriptor<TFieldSettings>
    {
        /// <summary>
        /// Handles file read errors.
        /// </summary>
        protected FileReadErrorHandler? HandleEntryReadError { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="FileEngineCore{TFieldSettings, TLayoutDescriptor}"/>.
        /// </summary>
        /// <param name="handleEntryReadError">The file read error handler.</param>
        protected FileEngineCore(FileReadErrorHandler? handleEntryReadError)
        {
            HandleEntryReadError = handleEntryReadError;
        }

        /// <summary>
        /// Gets the layout descriptor for a record type.
        /// </summary>
        /// <param name="recordType">The record type.</param>
        /// <returns>The layout descriptor.</returns>
        protected abstract TLayoutDescriptor GetLayoutDescriptor(Type recordType);

        /// <summary>
        /// Gets a line builder for a record type.
        /// </summary>
        /// <param name="layoutDescriptor">The layout descriptor.</param>
        /// <returns>The line builder.</returns>
        protected abstract ILineBuilder GetLineBuilder(TLayoutDescriptor layoutDescriptor);

        /// <summary>
        /// Gets a line parser for a record type.
        /// </summary>
        /// <param name="layoutDescriptor">The layout descriptor.</param>
        /// <returns>The line parser.</returns>
        protected abstract ILineParser GetLineParser(TLayoutDescriptor layoutDescriptor);

        /// <summary>
        /// Processes the header.
        /// </summary>
        /// <param name="reader">The reader.</param>
        protected virtual void ProcessHeader(TextReader reader) => reader.ReadLine();

        /// <summary>
        /// Tries to parse a line.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="line">The line.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="entity">The entity.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected virtual bool TryParseLine<TEntity>(string line, int lineNumber, ref TEntity entity) 
            where TEntity : notnull, new()
        {
            var layoutDescriptor = GetLayoutDescriptor(entity.GetType());
            var parser = GetLineParser(layoutDescriptor);
            parser.ParseLine(line.AsSpan(), entity);

            return true;
        }

        /// <summary>
        /// Writes a record to the specified stream.
        /// </summary>
        /// <typeparam name="TEntity">The type of records to write.</typeparam>
        /// <param name="stream">The stream.</param>
        /// <param name="entries">The records to write.</param>
        /// <param name="cancellationToken">Cancels writing a file.</param>
        public virtual Task WriteAsync<TEntity>(Stream stream, IEnumerable<TEntity> entries, CancellationToken cancellationToken = default) 
            where TEntity : notnull, new()
        {
            var writer = new StreamWriter(stream);
            return WriteAsync(writer, entries, cancellationToken);
        }

        /// <summary>
        /// Writes a record to the specified text writer.
        /// </summary>
        /// <typeparam name="TEntity">The type of records to write.</typeparam>
        /// <param name="writer">The text writer.</param>
        /// <param name="entries">The records to write.</param>
        /// <param name="cancellationToken">Cancels writing a file.</param>
        public async Task WriteAsync<TEntity>(TextWriter writer, IEnumerable<TEntity> entries, CancellationToken cancellationToken = default) 
            where TEntity : notnull, new()
        {
            await WriteHeaderAsync(writer).ConfigureAwait(false);

            int lineNumber = 0;
            foreach (var entry in entries)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await WriteEntryAsync(writer, lineNumber, entry).ConfigureAwait(false);

                lineNumber += 1;
            }

            await WriteFooterAsync(writer).ConfigureAwait(false);

            await writer.FlushAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Writes an entry.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="writer">The writer.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="entity">The entity.</param>
        protected virtual Task WriteEntryAsync<TEntity>(TextWriter writer, int lineNumber, TEntity entity)
            where TEntity : notnull
        {
            var layoutDescriptor = GetLayoutDescriptor(entity.GetType());
            var builder = GetLineBuilder(layoutDescriptor);
            var line = builder.BuildLine(entity);

            return writer.WriteLineAsync(line);
        }

        /// <summary>
        /// Writes the header.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected virtual Task WriteHeaderAsync(TextWriter writer) => Task.CompletedTask;

        /// <summary>
        /// Writes the footer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected virtual Task WriteFooterAsync(TextWriter writer) => Task.CompletedTask;
    }
}