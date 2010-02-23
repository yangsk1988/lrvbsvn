using System;
using System.Collections;
using System.Collections.Generic;

namespace VirtualBicycle.DataStructure
{
    /// <summary>
    /// 大顶堆
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Heap<T> where T : IComparable<T>
    {
        public static void HeapSort(T[] objects)
        {
            HeapSort(objects, false);
        }
        public static void HeapSort(T[] objects, bool descending)
        {
            for (int i = objects.Length / 2 - 1; i >= 0; --i)
                heapAdjustFromTop(objects, i, objects.Length, descending);
            for (int i = objects.Length - 1; i > 0; --i)
            {
                swap(objects, i, 0);
                heapAdjustFromTop(objects, 0, i, descending);
            }
        }

        public static void heapAdjustFromBottom(T[] objects, int n)
        {
            heapAdjustFromBottom(objects, n, false);
        }

        public static void heapAdjustFromBottom(T[] objects, int n, bool descending)
        {
            while (n > 0 && descending ^ objects[(n - 1) >> 1].CompareTo(objects[n]) < 0)
            {
                swap(objects, n, (n - 1) >> 1);
                n = (n - 1) >> 1;
            }
        }

        public static void heapAdjustFromTop(T[] objects, int n, int len)
        {
            heapAdjustFromTop(objects, n, len, false);
        }

        public static void heapAdjustFromTop(T[] objects, int n, int len, bool descending)
        {
            while ((n << 1) + 1 < len)
            {
                int m = (n << 1) + 1;
                if (m + 1 < len && descending ^ objects[m].CompareTo(objects[m + 1]) < 0)
                    ++m;
                if (descending ^ objects[n].CompareTo(objects[m]) > 0) return;
                swap(objects, n, m);
                n = m;
            }
        }

        private static void swap(T[] objects, int a, int b)
        {
            T tmp = objects[a];
            objects[a] = objects[b];
            objects[b] = tmp;
        }
    }
}
