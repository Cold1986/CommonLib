/*******************************
 * 
 * 选择排序的思想其实和冒泡排序有点类似，都是在一次排序后把最小的元素放到最前面。
 * 但是过程不同，冒泡排序是通过相邻的比较和交换。
 * 而选择排序是通过对整体的选择。
 * 举个栗子，对5,3,8,6,4这个无序序列进行简单选择排序，
 * 首先要选择5以外的最小数来和5交换，也就是选择3和5交换，一次排序后就变成了3,5,8,6,4.
 * 对剩下的序列一次进行选择和交换，最终就会得到一个有序序列。
 * 其实选择排序可以看成冒泡排序的优化，因为其目的相同，只是选择排序只有在确定了最小数的前提下才进行交换，大大减少了交换的次数。
 * 
 * 选择排序的时间复杂度为O(n^2)
 * 
 * ****************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cold.CommonLibrary
{
    /// <summary>
    /// 简单选择排序算法实现
    /// 选择排序的时间复杂度为O(n^2)
    /// </summary>
    public class SelectSort
    {
        public static void Sort(int[] arr)
        {
            if (arr == null || arr.Length == 0)
            {
                return;
            }

            int minIndex = 0;
            for (int i = 0; i < arr.Length - 1; i++)//只需要比较n-1次
            {
                minIndex = i;
                for (int j = i + 1; j < arr.Length; j++)
                {
                    if (arr[j] < arr[minIndex])
                    {
                        minIndex = j;
                    }
                }
                if (minIndex != i)//如果minIndex不为i,说明找到了更小的值，交换之。
                {
                    swap(arr, i, minIndex);
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
