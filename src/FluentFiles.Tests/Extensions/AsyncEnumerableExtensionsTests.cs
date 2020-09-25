namespace FluentFiles.Tests.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentFiles.Core.Extensions;
    using Xunit;

    public class AsyncEnumerableExtensionsTests
    {
        [Fact]
        public void Test_ToEnumerable()
        {
            // Act
            var actual = AsyncEnumerableExtensions.ToEnumerable(GetIntegers()).Take(5);

            // Assert
            Assert.Equal(new[] { 0, 1, 2, 3, 4 }, actual);
        }

        private async IAsyncEnumerable<int> GetIntegers([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var i = 0;
            while (!cancellationToken.IsCancellationRequested)
                yield return i++;

            await Task.CompletedTask;
        }
    }
}
