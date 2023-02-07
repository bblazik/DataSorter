using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSorter
{
    public partial class Algorithms
    {
        public static void QuickSort(List<Record> records, int left, int right)
        {
            if (left < right)
            {
                int pivotIndex = Partition(records, left, right);
                QuickSort(records, left, pivotIndex - 1);
                QuickSort(records, pivotIndex + 1, right);
            }
        }

        private static int Partition(List<Record> records, int left, int right)
        {
            int pivotIndex = left + (right - left) / 2;
            Record pivotValue = records[pivotIndex];
            Swap(records, pivotIndex, right);

            int storeIndex = left;
            for (int i = left; i < right; i++)
            {
                if (records[i].CompareTo(pivotValue) < 0)
                {
                    Swap(records, storeIndex, i);
                    storeIndex++;
                }
            }

            Swap(records, storeIndex, right);
            return storeIndex;
        }

        private static void Swap(List<Record> records, int a, int b)
        {
            Record temp = records[a];
            records[a] = records[b];
            records[b] = temp;
        }
    }
}
