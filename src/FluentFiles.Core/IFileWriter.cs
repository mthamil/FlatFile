namespace FluentFiles.Core
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for writing files.
    /// </summary>
    public interface IFileWriter
    {
        /// <summary>
        /// Writes a record to the specified stream.
        /// </summary>
        /// <typeparam name="TEntity">The type of records to write.</typeparam>
        /// <param name="stream">The stream.</param>
        /// <param name="entries">The records to write.</param>
        /// <param name="cancellationToken">Cancels writing a file.</param>
        Task WriteAsync<TEntity>(Stream stream, IEnumerable<TEntity> entries, CancellationToken cancellationToken = default) where TEntity : class, new();

        /// <summary>
        /// Writes a record to the specified text writer.
        /// </summary>
        /// <typeparam name="TEntity">The type of records to write.</typeparam>
        /// <param name="writer">The text writer.</param>
        /// <param name="entries">The records to write.</param>
        /// <param name="cancellationToken">Cancels writing a file.</param>
        Task WriteAsync<TEntity>(TextWriter writer, IEnumerable<TEntity> entries, CancellationToken cancellationToken = default) where TEntity : class, new();
    }
}