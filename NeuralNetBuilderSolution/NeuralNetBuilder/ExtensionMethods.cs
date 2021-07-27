using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeuralNetBuilder
{
    public static class ExtensionMethods
    {
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
        /// <summary>
        /// Supports following enums: ActivationType, WeightInitType, CostType, ParameterName
        /// Other types will cause an exception throw.
        /// </summary>
        public static TEnum ToEnum<TEnum>(this string enumAsString, bool throwExceptionOnWrongParameter = true)
        {
            TEnum result = default;

            // Do I really need to restrict enum types?
            Type enumType = typeof(TEnum);
            //if (
            //    enumType != typeof(ActivationType) &&
            //    enumType != typeof(CostType) &&
            //    enumType != typeof(WeightInitType) &&
            //    enumType != typeof(ParameterName))
            //    throw new ArgumentException($"ToEnum(..) does not support type {enumType.Name}. \nSo far it only supports the following enums: ActivationType, WeightInitType, CostType");

            //var names = Enum.GetNames(enumType);
            var values = Enum.GetValues(enumType);
            int length = values.Length;

            for (int i = 0; i < length; i++)
            {
                result = (TEnum)values.GetValue(i);
                if (result.ToString() == enumAsString)
                    return result;
            }

            throw new ArgumentException($"{enumType.Name}.{enumAsString} does not exist.");
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
        public static async Task<IEnumerable<T>> ShuffleAsync<T>(this IEnumerable<T> collection)
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
