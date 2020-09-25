using System.Collections.Generic;

namespace FluentFiles.Core.Extensions
{
    internal static class AsyncEnumerableExtensions
    {
        public static IEnumerable<T> ToEnumerable<T>(this IAsyncEnumerable<T> asyncEnumerable)
        {
            var enumerator = asyncEnumerable.GetAsyncEnumerator();
            try
            {
                while (enumerator.MoveNextAsync().GetAwaiter().GetResult())
                {
                    yield return enumerator.Current;
                }
            }
            finally
            {
                enumerator.DisposeAsync().GetAwaiter().GetResult();
            }
        }
    }
}
