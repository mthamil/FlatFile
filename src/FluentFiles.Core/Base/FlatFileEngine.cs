namespace FluentFiles.Core.Base
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using FluentFiles.Core;
    using FluentFiles.Core.Exceptions;
    using FluentFiles.Core.Extensions;

    /// <summary>
    /// Reads and writes file records.
    /// </summary>
    /// <typeparam name="TFieldSettings">The type of field configuration.</typeparam>
    /// <typeparam name="TLayoutDescriptor">The type of layout descriptor.</typeparam>
    public abstract class FlatFileEngine<TFieldSettings, TLayoutDescriptor> : FileEngineCore<TFieldSettings, TLayoutDescriptor>, IFlatFileEngine
        where TFieldSettings : IFieldSettings
        where TLayoutDescriptor : ILayoutDescriptor<TFieldSettings>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="FlatFileEngine{TFieldSettings, TLayoutDescriptor}"/>.
        /// </summary>
        /// <param name="handleEntryReadError">The file read error handler.</param>
        protected FlatFileEngine(FileReadErrorHandler? handleEntryReadError = null)
            : base(handleEntryReadError)
        {
        }

        /// <summary>
        /// Reads the specified stream.
        /// </summary>
        /// <typeparam name="TEntity">The type of record to read.</typeparam>
        /// <param name="stream">The stream.</param>
        /// <returns>Any records read and parsed from the stream.</returns>
        public virtual IEnumerable<TEntity> Read<TEntity>(Stream stream) where TEntity : class, new()
        {
            var reader = new StreamReader(stream);
            return Read<TEntity>(reader);
        }

        /// <summary>
        /// Reads from the specified text reader.
        /// </summary>
        /// <typeparam name="TEntity">The type of record to read.</typeparam>
        /// <param name="reader">The text reader.</param>
        /// <returns>Any records read and parsed from the reader.</returns>
        public virtual IEnumerable<TEntity> Read<TEntity>(TextReader reader) where TEntity : class, new() =>
            ReadAsync<TEntity>(reader).ToEnumerable();

        /// <summary>
        /// Reads records from a text reader asynchronously.
        /// </summary>
        /// <typeparam name="TEntity">The type of record to read.</typeparam>
        /// <param name="reader">The text reader.</param>
        /// <param name="cancellationToken">Can be used to cancel the read operation.</param>
        /// <returns>The records read and parsed from the reader.</returns>
        public async IAsyncEnumerable<TEntity> ReadAsync<TEntity>(TextReader reader, [EnumeratorCancellation] CancellationToken cancellationToken = default) where TEntity : class, new()
        {
            var layoutDescriptor = GetLayoutDescriptor(typeof(TEntity));
            if (layoutDescriptor.HasHeader)
            {
                ProcessHeader(reader);
            }

            string line;
            int lineNumber = 0;
            while ((line = await reader.ReadLineAsync().ConfigureAwait(false)) != null)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (string.IsNullOrEmpty(line) || string.IsNullOrEmpty(line.Trim())) continue;

                bool ignoreEntry = false;
                TEntity entry = (TEntity)layoutDescriptor.InstanceFactory();
                try
                {
                    if (!TryParseLine(line, lineNumber++, ref entry))
                    {
                        throw new ParseLineException("Impossible to parse line", line, lineNumber);
                    }
                }
                catch (Exception ex)
                {
                    if (HandleEntryReadError == null || !HandleEntryReadError(new FlatFileErrorContext(line, lineNumber, ex)))
                    {
                        throw;
                    }

                    ignoreEntry = true;
                }

                if (!ignoreEntry)
                {
                    yield return entry;
                }
            }
        }
    }
}