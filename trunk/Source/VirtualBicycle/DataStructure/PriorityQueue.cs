using System;

namespace VirtualBicycle.DataStructure
{
    /// <summary>
    /// 泛型优先队列
    /// </summary>
    /// <typeparam name="T">实现IComparable&lt;T&gt;的类型</typeparam>
    public class PriorityQueue<T> where T : IComparable<T>
    {
        private const int defaultCapacity = 0x10;//默认容量为16

        public PriorityQueue()
            : this(false)
        {
        }
        public PriorityQueue(bool ascending)
        {
            buffer = new T[defaultCapacity];
            heapLength = 0;
            descending = ascending;
        }

        public bool Empty()
        {
            return heapLength == 0;
        }

        public T Top()
        {
            if (heapLength == 0) throw new OverflowException("优先队列为空时无法执行返回队首操作");
            return buffer[0];
        }

        public void Push(T obj)
        {
            if (heapLength == buffer.Length) expand();
            buffer[heapLength] = obj;
            Heap<T>.heapAdjustFromBottom(buffer, heapLength, descending);
            heapLength++;
        }

        public void Pop()
        {
            if (heapLength == 0) throw new OverflowException("优先队列为空时无法执行出队操作");
            --heapLength;
            swap(0, heapLength);
            Heap<T>.heapAdjustFromTop(buffer, 0, heapLength, descending);
        }

        private void expand()
        {
            Array.Resize<T>(ref buffer, buffer.Length * 2);
        }

        private void swap(int a, int b)
        {
            T tmp = buffer[a];
            buffer[a] = buffer[b];
            buffer[b] = tmp;
        }

        private bool descending;
        private int heapLength;
        private T[] buffer;
    }
}
