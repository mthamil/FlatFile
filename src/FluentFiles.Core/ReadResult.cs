namespace FluentFiles.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provides the results of a multi-record read operation.
    /// </summary>
    public class ReadResult
    {
        private readonly IReadOnlyDictionary<Type, IList<object>> _results;

        /// <summary>
        /// Initializes a new <see cref="ReadResult"/>.
        /// </summary>
        /// <param name="results">Parsed records mapped by type.</param>
        public ReadResult(IReadOnlyDictionary<Type, IList<object>> results)
        {
            _results = results ?? throw new ArgumentNullException(nameof(results));
        }

        /// <summary>
        /// Gets any records of type <typeparamref name="TRecord"/>.
        /// </summary>
        /// <typeparam name="TRecord">The type of record to retrieve.</typeparam>
        /// <returns>Any records of type <typeparamref name="TRecord"/> that were parsed.</returns>
        public IEnumerable<TRecord> GetRecords<TRecord>() =>
            _results.TryGetValue(typeof(TRecord), out var results)
                ? results.Cast<TRecord>()
                : Enumerable.Empty<TRecord>();
    }
}