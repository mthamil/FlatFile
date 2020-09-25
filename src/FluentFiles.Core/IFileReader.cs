namespace FluentFiles.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;

    /// <summary>
    /// Interface for reading files.
    /// </summary>
    public interface IFileReader
    {
        /// <summary>
        /// Reads the specified stream.
        /// </summary>
        /// <typeparam name="TEntity">The type of record to read.</typeparam>
        /// <param name="stream">The stream.</param>
        /// <returns>Any records read and parsed from the stream.</returns>
        [Obsolete("Use ReadAsync instead.")]
        IEnumerable<TEntity> Read<TEntity>(Stream stream) where TEntity : class, new();

        /// <summary>
        /// Reads from the specified text reader.
        /// </summary>
        /// <typeparam name="TEntity">The type of record to read.</typeparam>
        /// <param name="reader">The text reader.</param>
        /// <returns>Any records read and parsed from the reader.</returns>
        [Obsolete("Use ReadAsync instead.")]
        IEnumerable<TEntity> Read<TEntity>(TextReader reader) where TEntity : class, new();

        /// <summary>
        /// Reads records from a text reader asynchronously.
        /// </summary>
        /// <typeparam name="TEntity">The type of record to read.</typeparam>
        /// <param name="reader">The text reader.</param>
        /// <param name="cancellationToken">Can be used to cancel the read operation.</param>
        /// <returns>The records read and parsed from the reader.</returns>
        IAsyncEnumerable<TEntity> ReadAsync<TEntity>(TextReader reader, CancellationToken cancellationToken = default) where TEntity : class, new();
    }
}