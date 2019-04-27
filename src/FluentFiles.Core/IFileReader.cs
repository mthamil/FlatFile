namespace FluentFiles.Core
{
    using System.Collections.Generic;
    using System.IO;

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
        IEnumerable<TEntity> Read<TEntity>(Stream stream) where TEntity : class, new();

        /// <summary>
        /// Reads from the specified text reader.
        /// </summary>
        /// <typeparam name="TEntity">The type of record to read.</typeparam>
        /// <param name="reader">The text reader.</param>
        /// <returns>Any records read and parsed from the reader.</returns>
        IEnumerable<TEntity> Read<TEntity>(TextReader reader) where TEntity : class, new();
    }
}