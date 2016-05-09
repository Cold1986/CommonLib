using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cold.CommonLibrary;

namespace Test
{
    class QuickSortTest
    {
        static void Main(string[] arg)
        {
            int[] arr = new int[]{5, 3, 8, 6, 4};

             QuickSort.Sort(arr);
        }
    }
}
