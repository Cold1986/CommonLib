using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cold.CommonLibrary;

namespace Test
{
    class RedisTest
    {
        static void Main(string[] args)
        {
            RedisHelper.CreateClient("127.0.0.1", 6379);
            RedisHelper.SetString("test", "test2", 1000);

            People Cold = new People();
            Cold.Name = "测试";
            Cold.age = 22;

            RedisHelper.Set<People>("testT", Cold, 1000);

            var tt = RedisHelper.Get<People>("testT");



            RedisHelper.SetHashField("testHash", "1", "hashTest1");
            RedisHelper.SetHashField("testHash", "2", "hashTest2");
            RedisHelper.SetHashField("testHash", "1", "hashTest12");

            var t = RedisHelper.GetHashField("testHash", "1");

            Console.WriteLine(t);
            Console.ReadKey();
        }

        class People
        {
            public string Name;
            public int age;
        }
    }
}
