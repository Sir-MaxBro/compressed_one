using System;
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

        public static IEnumerable<TResult> Mix<TResult>(this IEnumerable<TResult> source)
        {
            var itemsCount = source.Count();
            var mixedCollection = new TResult[itemsCount];
            var randomGenerator = new Random(DateTime.Now.Millisecond);

            foreach (var item in source)
            {
                bool isSetItem = false;
                do
                {
                    var nextIndex = randomGenerator.Next(0, itemsCount);
                    if (mixedCollection[nextIndex] == null)
                    {
                        mixedCollection[nextIndex] = item;
                        isSetItem = true;
                    }
                } while (!isSetItem);
            }

            return mixedCollection;
        }
    }
}
