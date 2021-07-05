using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeuralNetBuilder
{
    public static class ExtensionMethods
    {
        public static string ToStringFromCollection<T>(this IEnumerable<T> collection)
        {
            return string.Join(", ", collection.Select(x => x.ToString()));
        }
        /// <summary>
        /// Supports following enums: ActivationType, WeightInitType, CostType
        /// Other types will cause an exception throw.
        /// </summary>
        public static TEnum ToEnum<TEnum>(this string enumAsString, bool throwExceptionOnWrongParameter = true)
        {
            TEnum result = default;

            // Do I really need to restrict enum types?
            Type enumType = typeof(TEnum);
            if (
                enumType != typeof(ActivationType) ||
                enumType != typeof(CostType) ||
                enumType != typeof(WeightInitType))
                throw new ArgumentException($"ToEnum(..) does not support type {enumType.Name}. \nSo far it only supports the following enums: ActivationType, WeightInitType, CostType");

            var names = Enum.GetNames(enumType);
            var values = Enum.GetValues(enumType);
            int length = names.Length;

            for (int i = 0; i < length; i++)
            {
                if (names[i] == enumAsString)
                    result = (TEnum)values.GetValue(i);

                if (i == length - 1 && throwExceptionOnWrongParameter)
                    throw new ArgumentException($"{enumType.Name}.{enumAsString} does not exist.");
            }
            
            return result;
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
