using System;
using System.Collections.Generic;
using System.Threading;

namespace TreeTest.ProductAndCustomer
{
    public class AsyncStack
    {
        private Queue<bool> _arrDelegate = new Queue<bool>();

        public void Push(bool handler)
        {
            try
            {
                Monitor.Enter(this);
                //try
                //{
                //    Monitor.Wait(this);
                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine(ex.Message);
                //}
                Monitor.Pulse(this);
                _arrDelegate.Enqueue(handler);
            }
            finally
            {
                Monitor.Exit(this);
            }
        }

        public bool IsHaveProduction()
        {
            return _arrDelegate.Count > 0;
        }

        public bool Pop()
        {
            try
            {
                Monitor.Enter(this);
                while (_arrDelegate.Count == 0)
                {
                    try
                    {
                        Monitor.Wait(this);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                Monitor.Pulse(this);
                return _arrDelegate.Dequeue();
            }
            finally
            {
                Monitor.Exit(this);
            }
        }
    }
}