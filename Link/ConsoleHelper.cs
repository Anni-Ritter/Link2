using System;
using System.Collections.Generic;
using System.Text;

namespace Link
{
    class ConsoleHelper
    {
        public static object LockObject = new Object();
        public static void WriteToConsole(string info, string write)
        {
            lock (LockObject)
            {
                Console.WriteLine(info + " : " + write);
            }

        }
    }
}
