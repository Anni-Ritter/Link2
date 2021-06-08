using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Link
{
    public delegate void PostToFirstWT(BitArray message);
    public delegate void PostToSecondWT(BitArray message);
    public class Program
    {
        static void Main(string[] args)
        {
            ConsoleHelper.WriteToConsole("Главный поток", "Введите Ваше сообщение...");
            var data = Console.ReadLine();
            Encoding encoding = Encoding.UTF8;

            Semaphore firstReceiveSemaphore = new Semaphore(0, 1);
            Semaphore secondReceiveSemaphore = new Semaphore(0, 1);

            FirstThread firstThread = new FirstThread(ref secondReceiveSemaphore, ref firstReceiveSemaphore);
            SecondThread secondThread = new SecondThread(ref firstReceiveSemaphore, ref secondReceiveSemaphore, encoding);

            Thread threadFirst = new Thread(new ParameterizedThreadStart(firstThread.FirstThreadMain));
            Thread threadSecond = new Thread(new ParameterizedThreadStart(secondThread.SecondThreadMain));

            PostToFirstWT postToFirstWt = new PostToFirstWT(firstThread.ReceiveData);
            PostToSecondWT postToSecondWt = new PostToSecondWT(secondThread.ReceiveData);

           var serializeMessage = Task.Factory.StartNew(() =>
           {
               var bitArray = new BitArray(encoding.GetBytes(data));
               var value = new bool[bitArray.Count];
               for (int m = 0; m < bitArray.Count; m++)
                   value[m] = bitArray[m];
               int j = 0;
               StaticFunction.Data = value.GroupBy(s => j++ / StaticFunction.PackLength).Select(g => g.ToArray()).ToArray();
           });

            threadFirst.Start(postToSecondWt);
            threadSecond.Start(postToFirstWt);

            Console.ReadLine();
        }
    }
}
