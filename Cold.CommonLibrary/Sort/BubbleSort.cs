/*****************************************
 * 
 * 冒泡排序是最简单的排序之一了，其大体思想就是通过与相邻元素的比较和交换来把小的数交换到最前面。
 * 这个过程类似于水泡向上升一样，因此而得名。
 * 举个栗子，对5,3,8,6,4这个无序序列进行冒泡排序。
 * 首先从后向前冒泡，4和6比较，把4交换到前面，序列变成5,3,8,4,6。
 * 同理4和8交换，变成5,3,4,8,6,3和4无需交换。
 * 5和3交换，变成3,5,4,8,6,3.这样一次冒泡就完了，把最小的数3排到最前面了。
 * 对剩下的序列依次冒泡就会得到一个有序序列。
 * 
 * 冒泡排序的时间复杂度为O(n^2)。
 * 
 * *************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cold.CommonLibrary
{
    /// <summary>
    /// 冒泡排序算法实现
    /// 冒泡排序的时间复杂度为O(n^2)。
    /// </summary>
    public class BubbleSort
    {
        public static void Sort(int[] arr)
        {
            if (arr == null || arr.Length == 0)
            {
                return;
            }

            for (int i = 0; i < arr.Length; i++)
            {
                for (int j = arr.Length - 1; j > i; j--)
                {
                    if (arr[j] < arr[j - 1])
                    {
                        swap(arr, j - 1, j);
                    }
                }
            }
        }

        private static void swap(int[] arr, int i, int j)
        {
            int temp = arr[i];
            arr[i] = arr[j];
            arr[j] = temp;
        }

    }
}
