using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeuralNetBuilder
{
    public static class ExtensionMethods
    {
        internal static string ToCollectionString<T>(this IEnumerable<T> collection)
        {
            return string.Join(",", collection.Select(x => x.ToString()));
        }
        internal static async Task<IEnumerable<T>> ShuffleAsync<T>(this IEnumerable<T> collection)
        {
            return await Task.Run(() =>
            {
                Random rnd = new Random();

                T[] result = collection.ToArray();
                int count = collection.Count();

                for (int index = 0; index < count; index++)
                {
                    int newIndex = rnd.Next(count);

                    // Exchange arr[n] with arr[k]

                    T item = result[index];
                    result[index] = result[newIndex];
                    result[newIndex] = item;
                }

                return result;
            });
        }
    }
}
