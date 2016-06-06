using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Redis;
using Newtonsoft.Json;
using System.IO;

namespace Cold.CommonTools
{
    public class RedisHelper : IDisposable
    {
        /*copyright@2013 All Rights Reserved
         * Author:Mars
         * Date:2013.08.27
         * QQ:258248340
         * servicestack.redis为github中的开源项目
         * redis是一个典型的k/v型数据库
         * redis共支持五种类型的数据 string,list,hash,set,sortedset
         * 
         * string是最简单的字符串类型
         * 
         * list是字符串列表，其内部是用双向链表实现的，因此在获取/设置数据时可以支持正负索引
         * 也可以将其当做堆栈结构使用
         * 
         * hash类型是一种字典结构，也是最接近RDBMS的数据类型，其存储了字段和字段值的映射，但字段值只能是
         * 字符串类型，散列类型适合存储对象，建议使用对象类别和ID构成键名，使用字段表示对象属性，字
         * 段值存储属性值，例如：car:2 price 500 ,car:2  color black,用redis命令设置散列时，命令格式
         * 如下：HSET key field value，即key，字段名，字段值
         * 
         * set是一种集合类型，redis中可以对集合进行交集，并集和互斥运算
         *           
         * sorted set是在集合的基础上为每个元素关联了一个“分数”，我们能够
         * 获得分数最高的前N个元素，获得指定分数范围内的元素，元素是不同的，但是"分数"可以是相同的
         * set是用散列表和跳跃表实现的，获取数据的速度平均为o(log(N))
         * 
         * 需要注意的是，redis所有数据类型都不支持嵌套
         * redis中一般不区分插入和更新操作，只是命令的返回值不同
         * 在插入key时，如果不存在，将会自动创建
         * 
         * 在实际生产环境中，由于多线程并发的关系，建议使用连接池，本类只是用于测试简单的数据类型
         */

        /*
         * 以下方法为基本的设置数据和取数据
         */
        private static RedisClient redisCli = null;
        /// <summary>
        /// 建立redis长连接
        /// </summary>
        /// 将此处的IP换为自己的redis实例IP，如果设有密码，第三个参数为密码，string 类型
        public static void CreateClient(string hostIP, int port, string keyword)
        {
            if (redisCli == null)
            {
                redisCli = new RedisClient(hostIP, port, keyword);
            }

        }
        public static void CreateClient(string hostIP, int port)
        {
            if (redisCli == null)
            {
                redisCli = new RedisClient(hostIP, port);
            }

        }
        //private static RedisClient redisCli = new RedisClient("192.168.101.165", 6379, "123456");
        /// <summary>
        /// 获取key,返回string格式
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetStringValue(string key)
        {
            string value;
            try
            {
                value = redisCli.GetValue(key);
            }
            catch (Exception ex)
            {
                value = null;
            }
            return value;
        }

        public static void SetString(string key, string value, int expirySeconds = 60*60*24*30)
        {
            redisCli.SetValue(key, value, TimeSpan.FromSeconds(expirySeconds));
        }
        /// <summary>
        /// 获取key,返回byte[]格式
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>

        public static T Get<T>(string key) where T : class
        {
            T t = null;

            try
            {
                var value = redisCli.GetValue(key);
                JsonSerializer serializer = new JsonSerializer();
                StringReader sr = new StringReader(value);
                object o = serializer.Deserialize(new JsonTextReader(sr), typeof(T));
                t = o as T;
                return t;


                //t = redisCli.Get<T>(key);
            }
            catch (Exception ex)
            {
                t = null;
            }
            return t;
        }

        public static void Set<T>(string key, T value, int expirySeconds = 60*60*24*30) where T : class
        {
            var t = JsonConvert.SerializeObject(value);
            redisCli.SetValue(key, t, TimeSpan.FromSeconds(expirySeconds));

            //var r=redisCli.Set(key, value, TimeSpan.FromSeconds(expirySeconds));
        }

        public static byte[] GetByteValue(string key)
        {
            byte[] value = redisCli.Get(key);
            return value;
        }
        /// <summary>
        /// 获得某个hash型key下的所有字段
        /// </summary>
        /// <param name="hashId"></param>
        /// <returns></returns>
        public static List<string> GetHashFields(string hashId)
        {
            List<string> hashFields = redisCli.GetHashKeys(hashId);
            return hashFields;
        }
        /// <summary>
        /// 获得某个hash型key下的所有值
        /// </summary>
        /// <param name="hashId"></param>
        /// <returns></returns>
        public static List<string> GetHashValues(string hashId)
        {
            List<string> hashValues = redisCli.GetHashKeys(hashId);
            return hashValues;
        }
        /// <summary>
        /// 获得hash型key某个字段的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        public static string GetHashField(string key, string field)
        {
            string value = redisCli.GetValueFromHash(key, field);
            return value;
        }
        /// <summary>
        /// 设置hash型key某个字段的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        public static void SetHashField(string key, string field, string value)
        {
            redisCli.SetEntryInHash(key, field, value);
        }
        /// <summary>
        ///使某个字段增加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public static void SetHashIncr(string key, string field, long incre)
        {
            redisCli.IncrementValueInHash(key, field, incre);

        }
        /// <summary>
        /// 向list类型数据添加成员，向列表底部(右侧)添加
        /// </summary>
        /// <param name="Item"></param>
        /// <param name="list"></param>
        public static void AddItemToListRight(string list, string item)
        {
            redisCli.AddItemToList(list, item);
        }
        /// <summary>
        /// 向list类型数据添加成员，向列表顶部(左侧)添加
        /// </summary>
        /// <param name="list"></param>
        /// <param name="item"></param>
        public static void AddItemToListLeft(string list, string item)
        {
            redisCli.LPush(list, Encoding.Default.GetBytes(item));
        }
        /// <summary>
        /// 从list类型数据读取所有成员
        /// </summary>
        public static List<string> GetAllItems(string list)
        {
            List<string> listMembers = redisCli.GetAllItemsFromList(list);
            return listMembers;
        }
        /// <summary>
        /// 从list类型数据指定索引处获取数据，支持正索引和负索引
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string GetItemFromList(string list, int index)
        {
            string item = redisCli.GetItemFromList(list, index);
            return item;
        }
        /// <summary>
        /// 向列表底部（右侧）批量添加数据
        /// </summary>
        /// <param name="list"></param>
        /// <param name="values"></param>
        public static void GetRangeToList(string list, List<string> values)
        {
            redisCli.AddRangeToList(list, values);
        }
        /// <summary>
        /// 向集合中添加数据
        /// </summary>
        /// <param name="item"></param>
        /// <param name="set"></param>
        public static void GetItemToSet(string item, string set)
        {
            redisCli.AddItemToSet(item, set);
        }
        /// <summary>
        /// 获得集合中所有数据
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        public static HashSet<string> GetAllItemsFromSet(string set)
        {
            HashSet<string> items = redisCli.GetAllItemsFromSet(set);
            return items;
        }
        /// <summary>
        /// 获取fromSet集合和其他集合不同的数据
        /// </summary>
        /// <param name="fromSet"></param>
        /// <param name="toSet"></param>
        /// <returns></returns>
        public static HashSet<string> GetSetDiff(string fromSet, params string[] toSet)
        {
            HashSet<string> diff = redisCli.GetDifferencesFromSet(fromSet, toSet);
            return diff;
        }
        /// <summary>
        /// 获得所有集合的并集
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        public static HashSet<string> GetSetUnion(params string[] set)
        {
            HashSet<string> union = redisCli.GetUnionFromSets(set);
            return union;
        }
        /// <summary>
        /// 获得所有集合的交集
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        public static HashSet<string> GetSetInter(params string[] set)
        {
            HashSet<string> inter = redisCli.GetIntersectFromSets(set);
            return inter;
        }
        /// <summary>
        /// 向有序集合中添加元素
        /// </summary>
        /// <param name="set"></param>
        /// <param name="value"></param>
        /// <param name="score"></param>
        public static void AddItemToSortedSet(string set, string value, long score)
        {
            redisCli.AddItemToSortedSet(set, value, score);
        }
        /// <summary>
        /// 获得某个值在有序集合中的排名，按分数的降序排列
        /// </summary>
        /// <param name="set"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long GetItemIndexInSortedSetDesc(string set, string value)
        {
            long index = redisCli.GetItemIndexInSortedSetDesc(set, value);
            return index;
        }
        /// <summary>
        /// 获得某个值在有序集合中的排名，按分数的升序排列
        /// </summary>
        /// <param name="set"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long GetItemIndexInSortedSet(string set, string value)
        {
            long index = redisCli.GetItemIndexInSortedSet(set, value);
            return index;
        }
        /// <summary>
        /// 获得有序集合中某个值得分数
        /// </summary>
        /// <param name="set"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double GetItemScoreInSortedSet(string set, string value)
        {
            double score = redisCli.GetItemScoreInSortedSet(set, value);
            return score;
        }
        /// <summary>
        /// 获得有序集合中，某个排名范围的所有值
        /// </summary>
        /// <param name="set"></param>
        /// <param name="beginRank"></param>
        /// <param name="endRank"></param>
        /// <returns></returns>
        public static List<string> GetRangeFromSortedSet(string set, int beginRank, int endRank)
        {
            List<string> valueList = redisCli.GetRangeFromSortedSet(set, beginRank, endRank);
            return valueList;
        }
        /// <summary>
        /// 获得有序集合中，某个分数范围内的所有值，升序
        /// </summary>
        /// <param name="set"></param>
        /// <param name="beginScore"></param>
        /// <param name="endScore"></param>
        /// <returns></returns>
        public static List<string> GetRangeFromSortedSet(string set, double beginScore, double endScore)
        {
            List<string> valueList = redisCli.GetRangeFromSortedSetByHighestScore(set, beginScore, endScore);
            return valueList;
        }
        /// <summary>
        /// 获得有序集合中，某个分数范围内的所有值，降序
        /// </summary>
        /// <param name="set"></param>
        /// <param name="beginScore"></param>
        /// <param name="endScore"></param>
        /// <returns></returns>
        public static List<string> GetRangeFromSortedSetDesc(string set, double beginScore, double endScore)
        {
            List<string> vlaueList = redisCli.GetRangeFromSortedSetByLowestScore(set, beginScore, endScore);
            return vlaueList;
        }
        public void Dispose()
        {
            redisCli.Dispose();
        }

    }
}
