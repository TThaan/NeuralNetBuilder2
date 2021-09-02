using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeuralNet_Core
{
    public static class ExtensionMethods
    {
        #region fields

        static Random rnd = RandomProvider.GetThreadRandom();

        #endregion

        /// <summary>
        /// Cast a generic IEnumerable to a string. 
        /// Optionally set a separator and a line break each or every n-th item (0 = no line break).
        /// </summary>
        public static string ToStringFromCollection<T>(this IEnumerable<T> collection, string separator = ", ", int lineBreakAfter = 0, int spacesInNewLine = 0)
        {
            List<object> collectionWithLineBreaks = Enumerable.Cast<object>(collection.ToList()).ToList();

            if (lineBreakAfter > 0)
            {
                collectionWithLineBreaks = collection.Select((x, i) =>
                {
                    if (i != 0 && i % lineBreakAfter == 0)
                        return $"\n{string.Join(string.Empty, Enumerable.Repeat(' ', spacesInNewLine))}" + (object)(x.ToString());
                    else
                        return (object)x.ToString();
                }).ToList();
            }
            return string.Join(separator, collectionWithLineBreaks.Select(x => x.ToString()));
        }
        public static TEnum ToEnum<TEnum>(this string enumAsString)
        {
            TEnum result;

            Type enumType = typeof(TEnum);
            
            var values = Enum.GetValues(enumType);
            int length = values.Length;

            for (int i = 0; i < length; i++)
            {
                result = (TEnum)values.GetValue(i);
                if (result.ToString() == enumAsString)
                    return result;
            }

            throw new ArgumentException($"'{enumAsString}' does not exist for {enumType.Name}.");
        }
        // Implement multiple dimensions:
        public static List<T> ToList<T>(this Array arr)
        {
            var result = new List<T>();

            for (int i = 0; i < arr.Length; i++)
            {
                result.Add((T)arr.GetValue(i));
            }

            return result;
        }
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> collection)
        {
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
        }
        public static async Task<IEnumerable<T>> ShuffleAsync<T>(this IEnumerable<T> collection)
        {
            return await Task.Run(() => collection.Shuffle());
        }
        public static async Task<T> GetRandomItemAndRemoveIt<T>(this List<T> list)   where T : class
        {
            return await Task.Run(() =>
            {
                T result = null;

                // Get next item at or after random index

                int rndIndex = rnd.Next(list.Count);
                for (int i = rndIndex; i < list.Count; i++)
                {
                    if (list[i] != null)
                    {
                        result = list[i];
                        list.RemoveAt(i);
                    }
                }

                // if there is no item at or after random index get the first existing item in the list
                if (result == null)
                {
                    for (int i = 0; i < rndIndex; i++)
                    {
                        if (list[i] != null)
                        {
                            result = list[i];
                            list.RemoveAt(i);
                        }
                    }
                }

                return result;
            });
        }        
        // Rename GetMaxValue!
        public static T GetMaximum<T>(this IEnumerable<T> collection)
        {
            return collection.OrderByDescending(x => x).First();
        }
        public static T GetMaximum<T>(this IEnumerable iEnum)
        {
            var result = Enumerable.Cast<T>(iEnum);
            return result.OrderByDescending(x => x).First();
        }
        public static T GetMinimum<T>(this IEnumerable<T> collection)
        {
            return collection.OrderBy(x => x).First();
        }
        public static T GetMinimum<T>(this IEnumerable iEnum)
        {
            var result = Enumerable.Cast<T>(iEnum);
            return result.OrderBy(x => x).First();
        }

        internal static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action(item);
        }
    }
}
