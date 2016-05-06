using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test
{
    class ThreadTest
    {
        bool breakThread = false;

        static void Main(string[] args)
        {
            var myThread=new Thread(Go);

            try
            {
                myThread.Start();
            }
            catch{

            }

            while (true)
            {
                if (myThread.ThreadState == ThreadState.Stopped)
                {
                    Console.WriteLine("线程结束");
                    break;
                }
                else if (myThread.ThreadState == ThreadState.Aborted)
                {
                    Console.WriteLine("线程已死");
                    //myThread.Start();
                    break;
                }
            }

            Console.ReadLine();
        }

        private static void Go()
        {
            //throw new Exception("test");
            
            //try
            //{
            //    //Thread.Sleep(2 * 1000);
            //    throw null; // 该异常将会被捕获

            //}
            //catch (Exception ex)
            //{
            //    // 异常日志记录，或者通知其他线程出现异常了

            //}
        }
    }
}
