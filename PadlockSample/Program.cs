using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PadlockSample
{
    class BankAccount
    {
        public object padlock = new object();
        private int _balance;

        public int Balance { get => _balance; private set => _balance = value; }

        public void Deposit(int amount)
        {
            // += is really two operations
            // op1 is temp <- get_Balance() + amount
            // op2 is set_Balance(temp)
            // something can happen _between_ op1 and op2
            Interlocked.Add(ref _balance, amount);

            //p1
            //p2
            Interlocked.MemoryBarrier();
            //p3



        }

        public void Withdraw(int amount)
        {
            Interlocked.Add(ref _balance, -amount);
              
            
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var tasks = new List<Task>();
            var ba = new BankAccount();

            for (int i = 0; i < 10; ++i)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; ++j)
                        ba.Deposit(100);
                }));
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; ++j)
                        ba.Withdraw(100);
                }));
            }

            Task.WaitAll(tasks.ToArray());

            Console.WriteLine($"Final balance is {ba.Balance}.");


            Console.WriteLine("All done here.");
        }
    }
}
