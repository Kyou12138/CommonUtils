#define DebugFlag

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommonUtils.Test
{
    [TestClass]
    public class UnitTest3
    {
        [TestMethod]
        public void demo()
        {
            Task<int> a = DoWorkAync();
            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(1);
                Console.WriteLine(i + "d");
            }
            Console.WriteLine(a.Result);
            Console.WriteLine("end");
        }

        public async Task<int> DoWorkAync()
        {
            return await Task.Run(() =>
            {
                int a = 0;
                for (int i = 0; i < 100; i++)
                {
                    a++;
                    Console.WriteLine(a + "w");
                    Thread.Sleep(2);
                }
                Console.WriteLine("task");
                return a;
            }
            );
        }

        private class MyClass
        {
            public int Get10()
            {
                return 10;
            }

            public async Task DoWorkAsync()
            {
                Func<int> ten = new Func<int>(Get10);
                int a = await Task.Run(ten);

                int b = await Task.Run(new Func<int>(Get10));

                int c = await Task.Run(() => { return 10; });

                Console.WriteLine("{0} {1} {2}", a, b, c);
            }
        }

        [TestMethod]
        public void demo1()
        {
            MyClass mc = new MyClass();
            Task t = mc.DoWorkAsync();
            t.Wait();
        }

        private class TestClass
        {
            public async Task RunAsync(CancellationToken ct)
            {
                if (ct.IsCancellationRequested)
                    return;
                await Task.Run(() => CycleMethod(ct));
            }

            private void CycleMethod(CancellationToken ct)
            {
                Console.WriteLine("starting CycleMethod");
                const int max = 5;
                for (int i = 0; i < max; i++)
                {
                    if (ct.IsCancellationRequested)
                        return;
                    Thread.Sleep(1000);
                    Console.WriteLine("     {0} of {1} iterations completed", i + 1, max);
                }
            }
        }

        [TestMethod]
        public void demo2()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken ct = cts.Token;

            TestClass tc = new TestClass();
            Task t = tc.RunAsync(ct);

            //Thread.Sleep(3000);
            //cts.Cancel();

            t.Wait();
            Console.WriteLine("Was Cancelled: {0}", ct.IsCancellationRequested);
        }

        private static async Task BadAsync()
        {
            try
            {
                await Task.Run(() => throw new Exception());
            }
            catch
            {
                Console.WriteLine("Exception in BadAsync");
            }
        }

        [TestMethod]
        public void demo3()
        {
            Task t = BadAsync();
            t.Wait();
            Console.WriteLine("Task status:{0}", t.Status);
            Console.WriteLine("Task IsFaulted: {0}", t.IsFaulted);
        }

        private class Simple
        {
            private Stopwatch sw = new Stopwatch();

            public void DoRun(CancellationToken ct)
            {
                Console.WriteLine("Caller: Before call");
                ShowDelyAsync(ct);
                Console.WriteLine("Caller: After call");
            }

            private async void ShowDelyAsync(CancellationToken ct)
            {
                sw.Start();
                Console.WriteLine("     Before Delay: {0}", sw.ElapsedMilliseconds);
                await Task.Delay(1000, ct);
                Console.WriteLine("     After Delay: {0}", sw.ElapsedMilliseconds);
            }
        }

        [TestMethod]
        public void demo4()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken ct = cts.Token;
            Simple s = new Simple();
            s.DoRun(ct);
            Thread.Sleep(500);
            cts.Cancel();
            Console.WriteLine("end");
            Thread.Sleep(2000);
        }

        private static class DoStuff
        {
            public static async Task<int> FindSeriesSum(int i1)
            {
                int sum = 0;
                for (int i = 0; i < i1; i++)
                {
                    sum += i;
                    Console.WriteLine(sum);
                    if (i % 1000 == 0) await Task.Yield();
                }
                return sum;
            }

            public static void CountBig(int p)
            {
                for (int i = 0; i < p; i++) Console.WriteLine("c-{0}", i); ;
            }
        }

        [TestMethod]
        public void demo5()
        {
            Task<int> value = DoStuff.FindSeriesSum(100000);
            DoStuff.CountBig(100000);
            DoStuff.CountBig(100000);
            DoStuff.CountBig(100000);
            DoStuff.CountBig(100000);
            Console.WriteLine("Sum: {0}", value.Result);
        }

        [TestMethod]
        public void demo6()
        {
            Parallel.For(0, 15, i => Console.WriteLine(i + "^2 = " + i * i));
        }

        private delegate long MyDel(int first, int second);

        private static long Sum(int x, int y)
        {
            Console.WriteLine("     Inside Sum");
            Thread.Sleep(100);
            return x + y;
        }

        [TestMethod]
        public void demo7()
        {
            MyDel myDel = new MyDel(Sum);

            Console.WriteLine("Before BeginInvoke");
            IAsyncResult iar = myDel.BeginInvoke(3, 4, null, null);
            Console.WriteLine("After BeginInvoke");
            Console.WriteLine("Doing stuff");

            long result = myDel.EndInvoke(iar);
            Console.WriteLine("After EndInvoke: {0}", result);
        }

        [TestMethod]
        public void demo8()
        {
            MyDel del = new MyDel(Sum);

            IAsyncResult iar = del.BeginInvoke(3, 4, null, null);
            Console.WriteLine("After BeginInvoke");

            while (!iar.IsCompleted)
            {
                Console.WriteLine("Not Done");

                for (int i = 0; i < 10000000; i++) ;
            }
            Console.WriteLine("Done");

            long result = del.EndInvoke(iar);
            Console.WriteLine("Result:{0}", result);
        }

        private static void CallWhenDone(IAsyncResult iar)
        {
            Console.WriteLine("         Inside CallWhenDone.");
            AsyncResult ar = (AsyncResult)iar;
            MyDel del = (MyDel)ar.AsyncDelegate;
            long result = del.EndInvoke(iar);
            Console.WriteLine("         The result: {0}", result);
        }

        [TestMethod]
        public void demo9()
        {
            MyDel del = new MyDel(Sum);
            Console.WriteLine("Before BeginInvoke");
            IAsyncResult iar = del.BeginInvoke(3, 4, new AsyncCallback(CallWhenDone), null);

            Console.WriteLine("Doing more work in Main.");
            Thread.Sleep(500);
            Console.WriteLine("Done with Main. Exiting.");
        }

        private int TimesCalled = 0;

        private void Display(object state)
        {
            Console.WriteLine("{0} - {1}", (string)state, ++TimesCalled);
        }

        [TestMethod]
        public void demo10()
        {
            Timer timer = new Timer(Display, "Processing timer event", 2000, 1000);
            Console.WriteLine("Timer started.");
            Thread.Sleep(5000);
        }

        private class DemoClass
        {
            public void A()
            {
                try
                {
                    B();
                }
                catch (NullReferenceException)
                {
                    Console.WriteLine("catch clause in A()");
                }
                finally
                {
                    Console.WriteLine("finally clause in A()");
                }
            }

            private void B()
            {
                int x = 10, y = 0;
                try
                {
                    x /= y;
                    Console.WriteLine(x);
                }
                catch (IndexOutOfRangeException)
                {
                    Console.WriteLine("catch clause in B()");
                }
                finally
                {
                    Console.WriteLine("finally clause in B()");
                }
            }
        }

        [TestMethod]
        public void demo11()
        {
            DemoClass dc = new DemoClass();
            try
            {
                dc.A();
            }
            catch (DivideByZeroException)
            {
                Console.WriteLine("catch clause in Main()");
            }
            finally
            {
                Console.WriteLine("finally clause in Main()");
            }
        }

        [TestMethod]
        public void demo12()
        {
#if DebugFlag
            Console.WriteLine("test");
#else
            Console.WriteLine("else");
#endif
            #region

            #endregion

            //#pragma warning disable
        }

        private class BaseClass
        {
            public int BaseField = 0;
        }

        private class DerivedClass : BaseClass
        {
            public int DerivedField = 0;
        }

        [TestMethod]
        public void demo13()
        {
            var bc = new BaseClass();
            var dc = new DerivedClass();
            BaseClass[] bca = new BaseClass[] { bc, dc };
            foreach (var item in bca)
            {
                Type t = item.GetType();
                Console.WriteLine("Object Type: {0}", t.Name);
                FieldInfo[] fi = t.GetFields();
                foreach (var f in fi)
                {
                    Console.WriteLine("     Field: {0}", f.Name);
                }
                Console.WriteLine();
            }
        }

        [Conditional("DebugFlag")]
        [Obsolete("Use method SuperPrintOut")]
        private static void PrintOut(string s)
        {
            Console.WriteLine(s);
        }

        [TestMethod]
        public void demo14()
        {
            PrintOut("dddd");
        }

        [DebuggerStepThrough]
        private static void MyTrace(string message,
                            [CallerFilePath] string fileName = "",
                            [CallerLineNumber] int lineNumber = 0,
                            [CallerMemberName] string callingMember = "")
        {
            Console.WriteLine("File:        {0}", fileName);
            Console.WriteLine("Line:        {0}", lineNumber);
            Console.WriteLine("Called From: {0}", callingMember);
            Console.WriteLine("Message:     {0}", message);
        }

        [TestMethod]
        public void demo15()
        {
            string start = "start";
            Console.WriteLine("debug-{0}", start);
            MyTrace("Simple message");
            string end = "end";
            Console.WriteLine("debug-{0}", end);
        }

        [TestMethod]
        public void demo16()
        {
            Type t = typeof(TestA);
            var a = t.GetCustomAttributes();
            foreach (var item in a)
            {
                ReviewCommentAttribute att = item as ReviewCommentAttribute;
                Console.WriteLine(att.TypeId);
                Console.WriteLine(att.Description);
                Console.WriteLine(att.VersionNumber);
                Console.WriteLine(att.ReviewerId + "end");
            }
        }

        [ReviewCommentAttribute("des", "1.0.1")]
        private class TestA
        {
        }

        [AttributeUsage(AttributeTargets.Class)]
        private sealed class ReviewCommentAttribute : Attribute
        {
            public string Description { get; set; }
            public string VersionNumber { get; set; }
            public string ReviewerId { get; set; }

            public ReviewCommentAttribute(string desc, string ver)
            {
                Description = desc;
                VersionNumber = ver;
            }
        }

        [TestMethod]
        public void demo17()
        {
            string s1 = "Hi there! This, is : a string.";
            char[] delimiters = { ' ', '!', ',', ':', '.' };
            string[] words = s1.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in words)
            {
                Console.WriteLine(word);
            }
        }

        [TestMethod]
        public void demo18()
        {
            StringBuilder sb = new StringBuilder("Hi, world!");
            Console.WriteLine(sb);
            sb.Replace("Hi", "Hello");
            Console.WriteLine(sb);
        }

        private class SomeClass
        {
            private int Field1 = 15;
            private int Field2 = 20;

            private MyNested mn = null;

            public void PrintMyMembers()
            {
                mn.PrintOuterMembers();
            }

            public SomeClass()
            {
                mn = new MyNested(this);
            }

            private class MyNested
            {
                private SomeClass sc = null;

                public MyNested(SomeClass SC)
                {
                    sc = SC;
                }

                public void PrintOuterMembers()
                {
                    Console.WriteLine("Field1: {0}", sc.Field1);
                    Console.WriteLine("Field2: {0}", sc.Field2);
                }
            }
        }

        [TestMethod]
        public void demo19()
        {
            SomeClass sc = new SomeClass();
            sc.PrintMyMembers();
        }

        private class MyClass1 : IDisposable
        {
            private bool disposed = false;

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            ~MyClass1()
            {
                Dispose(false);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!disposed)
                {
                    if (disposing)
                    {
                        //释放托管资源
                    }
                    //释放非托管资源
                }
                disposed = true;
            }
        }

        [TestMethod]
        public void demo20()
        {
            SheetTypeDic test = new SheetTypeDic();
            test.Add("无铅无卤板材系列");
            test.Add("Lead free");
            test.Add("Nor.Tg");
        }

        private class SheetTypeDic : List<string>
        {
        }
    }
}