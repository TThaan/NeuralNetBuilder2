using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NeuralNet_Core
{
    public class CustomArray<T> : IEnumerable<T>
    {
        #region fields & ctor

        T[] content;
        // int[] freqOfUsage;   // 0 = never used, 1 = accessed once etc
        int current;
        Random rnd;
        //T lastItem;

        public CustomArray()
        {
            rnd = RandomProvider.GetThreadRandom();
            current = 0;
        }
        public CustomArray(int capacity)
            : base()
        {
            content = new T[capacity];
        }
        public CustomArray(T[] arr)
            : base()
        {
            arr.CopyTo(content, 0);
        }
        public CustomArray(IEnumerable<T> collection)
            : base()
        {
            content = collection.ToArray();
        }

        #endregion

        public T this[int index]
        {
            get => content[index];
            set => content[index] = value;
        }

        public int Length => content.Length;
        public int CurrentIndex => current;
        public T CurrentItem => this[current];
        public T NextItem => this[++current];

        public T GetNextItemAndSetItNull()
        {
            T result = this[++current];
            SetNullAt(current);
            return result;
        }
        public T GetRandomItem()
        {
            int rndIndex = rnd.Next(Length);
            current = rndIndex;
            return this[current];
        }
        public T GetRandomItemAndSetItNull()
        {
            T result = GetRandomItem();
            SetNullAt(current);
            return result;
        }
        public void SetNullAt(int index)
        {
            content[index] = default;
            current = index;
        }
        public void SetNullAll()
        {
            for (int i = 0; i < Length; i++)
            {
                content[i] = default;
            };
        }

        #region IEnumerable<T>

        public IEnumerator<T> GetEnumerator() => (IEnumerator<T>)content.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => content.GetEnumerator();

        #endregion
    }
}
