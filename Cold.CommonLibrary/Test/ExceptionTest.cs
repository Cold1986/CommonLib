using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class ExceptionTest
    {
        static void Main(string[] args)
        {
            try
            {
                mytest();
            }
            catch(BookOrderException e)
            {
                int t = 2;
            }
           
        }

        static void mytest()
        {
            test tt = new test();
            try
            {
                tt.name = "cold";
                tt.age = 11;
                throw new BookOrderException(tt);
            }
            catch (BookOrderException e2)
            {
                throw new BookOrderException(e2.btest);
            }
            catch (Exception e)
            {
                int t = 1;
            }
        }
    }
    class test
    {
        public string name { get; set; }
        public int age { get; set; }
    }

    class BookOrderException : Exception
    {
        public test btest;
        public BookOrderException()
            : base()
        {

        }

        public BookOrderException(test t)
            : base()
        {
            btest = t;
        }
    }
}
