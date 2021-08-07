using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NeuralNetBuilder
{
    public class NullableIntArray : IEnumerable<int?>
    {
        #region fields & ctor

        int?[] content;
        // int[] freqOfUsage;   // 0 = never used, 1 = accessed once etc
        int current;
        Random rnd;
        //T lastItem;

        public NullableIntArray()
        {
            // https://stackoverflow.com/a/8939958/10547243
            //if (Nullable.GetUnderlyingType(typeof(T)) == null)
            //    throw new ArgumentException($" {typeof(T)} is not nullable. The generic type T must be nullable to be used with {nameof(CustomArrayForNullableInt<T>)}");

            rnd = RandomProvider.GetThreadRandom();
            current = 0;
        }
        public NullableIntArray(int capacity)
            : base()
        {
            content = new int?[capacity];
        }
        public NullableIntArray(int?[] arr, decimal multiplier = 1)
            : base()
        {
            // var unmultipliedArray = new NullableIntArray(arr); 
            content = new int?[(int)(arr.Length * multiplier)];
            int arrLength = arr.Length;
            int x = 0;

            for (int i = 0; i < Math.Truncate(multiplier); i++)
            {
                x = i * arrLength;

                for (int k = 0; k <= arrLength; k++)
                {
                    content[x + k] = arr[k];
                }
            }

            int fraction = (int)(multiplier - Math.Truncate(multiplier) * 10);

            for (int i = 0; i < Math.Truncate(multiplier); i++)
            {
                content[x + i] = arr[i];
            }

            //arr.CopyTo(content, 0);
        }
        public NullableIntArray(IEnumerable<int?> collection, decimal multiplier = 1)
            : base()
        {
            content = collection.ToArray();
        }

        #endregion

        public int? this[int index]
        {
            get => content[index];
            set => content[index] = value;
        }

        public int Length => content.Length;
        public int CurrentIndex
        {
            get => current;
            set => current = value;
        }
        public int? CurrentItem => this[current];
        public int? NextItem
        {
            get
            {
                try
                {
                    return this[++current];
                }
                catch (Exception e)
                {
                    current = 0;
                    return this[current];
                }
            }
        }

        public int? GetNextItemAndSetItNull()
        {
            int? result = this[++current];
            SetNullAt(current);
            return result;
        }
        public int? GetRandomItem()
        {
            int rndIndex = rnd.Next(Length);
            current = rndIndex;
            return this[current];
        }
        public int? GetRandomItemAndSetItNull()
        {
            int? result = GetRandomItem();
            SetNullAt(current);
            return result;
        }
        public void SetNullAt(int index)
        {
            content[index] = null;
            current = index;
        }
        public void SetNullAll()
        {
            for (int i = 0; i < Length; i++)
            {
                content[i] = null;
            };
        }
        //public NullableIntArray Multiply(int v)
        //{
        //    content
        //}

        #region IEnumerable<int?>

        public IEnumerator<int?> GetEnumerator() => (IEnumerator<int?>)content.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => content.GetEnumerator();

        #endregion
    }
}
