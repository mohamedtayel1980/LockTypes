using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MutexSample
{
    class BankAccount
    {
        public object padlock = new object();
        public int Balance { get; private set; }

        public void Deposit(int amount)
        {



            Balance += amount;

        }

        public void Withdraw(int amount)
        {

            Balance -= amount;

        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var tasks = new List<Task>();
            var ba = new BankAccount();
            Mutex mutex = new Mutex();
            Console.WriteLine("Mutex Start");
            for (int i = 0; i < 10; ++i)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; ++j)
                    {
                        var lockTaken = mutex.WaitOne();
                        try
                        {
                          
                            ba.Deposit(100);
                        }
                        finally
                        {
                            if (lockTaken) mutex.ReleaseMutex();
                        }




                    }
                }));
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; ++j)
                    {
                        var lockTaken = mutex.WaitOne();
                        try
                        {

                           
                            ba.Withdraw(100);
                        }
                        finally
                        {
                            if (lockTaken) mutex.ReleaseMutex();
                        }



                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());

            Console.WriteLine($"Final balance is {ba.Balance}.");
            Console.WriteLine("Mutex End");

            Console.WriteLine("All done here.");
        }
    }
}
