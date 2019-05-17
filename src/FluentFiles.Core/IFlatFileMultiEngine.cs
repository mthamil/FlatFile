namespace FluentFiles.Core
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// A file engine capable of handling files with multiple types of records.
    /// </summary>
    public interface IFlatFileMultiEngine : IFileWriter
    {
        /// <summary>
        /// Reads the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="cancellationToken">Cancels reading a file.</param>
        Task<ReadResult> ReadAsync(Stream stream, CancellationToken cancellationToken = default);

        /// <summary>
        /// Reads the specified text reader.
        /// </summary>
        /// <param name="reader">The text reader.</param>
        /// <param name="cancellationToken">Cancels reading a file.</param>
        Task<ReadResult> ReadAsync(TextReader reader, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets or sets a value indicating whether this instance has a file header.
        /// </summary>
        /// <value><c>true</c> if this instance has a file header; otherwise, <c>false</c>.</value>
        bool HasHeader { get; set; }
    }
}