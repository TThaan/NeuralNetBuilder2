using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NeuralNetBuilder
{
    public class NullableIntArray : IEnumerable<int?>
    {
        #region fields & ctor

        int?[] content;
        // int[] freqOfUsage;   // 0 = never used, 1 = accessed once etc
        int currentIndex;
        Random rnd;
        //T lastItem;
        int length;

        public NullableIntArray()
        {
            // https://stackoverflow.com/a/8939958/10547243
            //if (Nullable.GetUnderlyingType(typeof(T)) == null)
            //    throw new ArgumentException($" {typeof(T)} is not nullable. The generic type T must be nullable to be used with {nameof(CustomArrayForNullableInt<T>)}");

            rnd = RandomProvider.GetThreadRandom();
            currentIndex = 0;
        }
        public NullableIntArray(int capacity)
            : base()
        {
            content = new int?[capacity];
        }
        public NullableIntArray(int?[] arr, decimal multiplier = 1)
            : base()
        {
            MultiplyContent(arr, multiplier);
        }
        public NullableIntArray(IEnumerable<int?> collection, decimal multiplier = 1)
            : base()
        {
            MultiplyContent(collection, multiplier);
        }
        public NullableIntArray(IEnumerable<int?> collection, int replicatedSamples)
            : base()
        {
            ReplicateContent(collection, replicatedSamples);
        }

        #region helpers

        private void CreateContent(IEnumerable<int?> collection, decimal multiplier = 1)
        {
            length = collection.Count();

            if (collection == null || multiplier == 0)
            { 
                content = new int?[0];
                return;
            }
            else 
            {
                MultiplyContent(collection, multiplier);
            }
        }
        private void ReplicateContent(IEnumerable<int?> collection, int replicatedSamples, bool shuffleReplicatedCollection = false)
        {
            int origLength = collection.Count();
            length = replicatedSamples + origLength;
            int sourceIndex = 0;

            content = new int?[length];

            for (int i = 0; i < length; i++)
            {
                if (i != 0 && i % origLength == 0)
                {
                    sourceIndex = 0;
                    // collection.Shuffle();
                }
                content[currentIndex++] = collection.ElementAt(sourceIndex++);
            }
        }
        private void MultiplyContent(IEnumerable<int?> collection, decimal multiplier)
        {
            // wa Shuffle ?
            int origLength = collection.Count();
            length = (int)(multiplier * origLength);
            int sourceIndex = 0;

            content = new int?[length];

            for (int i = 0; i < length; i++)
            {
                if (i == origLength)
                    sourceIndex = 0;
                content[currentIndex++] = collection.ElementAt(sourceIndex++);
            }
        }

        #endregion

        #endregion

        public int? this[int index]
        {
            get => content[index];
            set => content[index] = value;
        }

        public int Length => content.Length;
        public int CurrentIndex
        {
            get => currentIndex;
            set => currentIndex = value;
        }
        public int? CurrentItem => this[currentIndex];
        public int? NextItem
        {
            get
            {
                try
                {
                    return this[++currentIndex];
                }
                catch (IndexOutOfRangeException)
                {
                    Debug.WriteLine("Handled!");
                    currentIndex = 0;
                    return this[currentIndex];
                }
            }
        }

        public int? GetNextItemAndSetItNull()
        {
            int? result = this[++currentIndex];
            SetNullAt(currentIndex);
            return result;
        }
        public int? GetRandomItem()
        {
            int rndIndex = rnd.Next(Length);
            currentIndex = rndIndex;
            return this[currentIndex];
        }
        public int? GetRandomItemAndSetItNull()
        {
            int? result = GetRandomItem();
            SetNullAt(currentIndex);
            return result;
        }
        public void SetNullAt(int index)
        {
            content[index] = null;
            currentIndex = index;
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

        public IEnumerator<int?> GetEnumerator()
        {
            foreach (int? item in content)
            {
                yield return item;
            }
        }
    IEnumerator IEnumerable.GetEnumerator() => content.GetEnumerator();

        #endregion
    }
}
