using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSorter
{
    public class MinHeap<T> where T : IComparable<T>
    {
        private List<T> _heap;
        private int _size;

        public MinHeap()
        {
            _heap = new List<T>();
            _size = 0;
        }

        public MinHeap(int initialCapacity)
        {
            _heap = new List<T>(initialCapacity);
            _size = 0;
        }

        public int Size
        {
            get { return _size; }
        }

        public bool IsEmpty
        {
            get { return _size == 0; }
        }

        private int Parent(int i)
        {
            return (i - 1) / 2;
        }

        private int LeftChild(int i)
        {
            return 2 * i + 1;
        }

        private int RightChild(int i)
        {
            return 2 * i + 2;
        }

        private void Swap(int i, int j)
        {
            T temp = _heap[i];
            _heap[i] = _heap[j];
            _heap[j] = temp;
        }

        private void SiftUp(int i)
        {
            while (i > 0 && _heap[Parent(i)].CompareTo(_heap[i]) > 0)
            {
                Swap(i, Parent(i));
                i = Parent(i);
            }
        }

        private void SiftDown(int i)
        {
            int minIndex = i;
            int leftChild = LeftChild(i);
            if (leftChild < _size && _heap[leftChild].CompareTo(_heap[minIndex]) < 0)
            {
                minIndex = leftChild;
            }

            int rightChild = RightChild(i);
            if (rightChild < _size && _heap[rightChild].CompareTo(_heap[minIndex]) < 0)
            {
                minIndex = rightChild;
            }

            if (i != minIndex)
            {
                Swap(i, minIndex);
                SiftDown(minIndex);
            }
        }

        public void Add(T value)
        {
            _heap.Add(value);
            _size++;
            SiftUp(_size - 1);
        }

        public T ExtractMin()
        {
            T result = _heap[0];
            _heap[0] = _heap[_size - 1];
            _size--;
            _heap.RemoveAt(_size);
            SiftDown(0);
            return result;
        }
    }
}