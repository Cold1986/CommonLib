using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class DelegateTest
    {
        private delegate int Test(int a, int b);

        static void Main(string[] args)
        {
            Test t = test1;
            var a=t(1, 2);
            var aa=t.Invoke(1, 2);
            Console.ReadLine();
        }

        static int test1(int a, int b)
        {
            return a + b;
        }

        static int test2(int a, int b)
        {
            return a * b;
        }
    }
}
