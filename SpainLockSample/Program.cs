using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SpainLockSample
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
            SpinLock spinLock = new SpinLock();
            Console.WriteLine("spinLock started");
            for (int i = 0; i < 10; ++i)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; ++j)
                    {
                        var lockTaken = false;
                        try
                        {
                            spinLock.Enter(ref lockTaken);
                            ba.Deposit(100);
                        }
                        finally
                        {
                            if (lockTaken) spinLock.Exit(true);
                        }
                        
                          
                        
                        
                    }
                }));
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; ++j)
                    {
                        var lockTaken = false;
                        try
                        {
                          
                            spinLock.Enter(ref lockTaken);
                            ba.Withdraw(100);
                        }
                        finally
                        {
                            if (lockTaken) spinLock.Exit(true);
                        }
                       
                        
                       
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());

            Console.WriteLine($"Final balance is {ba.Balance}.");
            Console.WriteLine("spinLock End");

            Console.WriteLine("All done here.");
        }
    }
}
