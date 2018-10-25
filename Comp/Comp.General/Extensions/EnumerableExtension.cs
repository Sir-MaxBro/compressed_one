using System.Collections.Generic;
using System.Linq;

namespace Comp.General.Extensions
{
    public static class EnumerableExtension
    {
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int batchSize)
        {
            var batchNumber = 0;
            var sourceList = new List<T>(source);
            var sourceCount = sourceList.Count;
            while (batchSize * batchNumber < sourceCount)
            {
                yield return source.Skip(batchNumber * batchSize).Take(batchSize).ToList();
                batchNumber++;
            }
        }
    }
}
