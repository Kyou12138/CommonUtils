using CommonUtils.EmailUtils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace CommonUtils.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Demo()
        {
            EmailConfig config = new EmailConfig();
        }

        [TestMethod]
        public void Demo1()
        {
            LoggerHelper.Monitor("test");
        }

        [TestMethod]
        public void Demo2()
        {
            double result = Caculator.getConeVolume(100, 90);
        }

        private class Caculator
        {
            public static double getCircleArea(double r)
            {
                return Math.PI * r * r;
            }

            public static double getCylinderVolume(double r, double h)
            {
                double ca = getCircleArea(r);
                return ca * r;
            }

            public static double getConeVolume(double r, double h)
            {
                double cv = getCylinderVolume(r, h);
                return cv / 3;
            }
        }

        [TestMethod]
        public void Demo3()
        {
            var stu = new { name = "zzk", age = 18, date = DateTime.Now, count = 1, flag = true, isBoy = true };
            Console.WriteLine(stu.name);
            Console.WriteLine(stu.GetType().Name);
        }

        [TestMethod]
        public void Demo4()
        {
            new Student().Report();
            new CsStudent().Report();
        }

        private class Student
        {
            public string Name { get; set; }

            public void Report()
            {
                Console.WriteLine("I am a student.");
            }
        }

        private class CsStudent : Student
        {
            new public void Report()
            {
                Console.WriteLine("I am a CS student.");
            }
        }

        [TestMethod]
        public void Demo5()
        {
            uint x = uint.MaxValue;
            Console.WriteLine(x);

            var binStr = Convert.ToString(x, 2);
            Console.WriteLine(binStr);

            checked
            {
                try
                {
                    uint y = x + 1;
                    Console.WriteLine(y);
                }
                catch (Exception)
                {
                    Console.WriteLine("overflow");
                }
            }
        }

        [TestMethod]
        public void Demo6()
        {
            Console.WriteLine(sizeof(int));
            Console.WriteLine(sizeof(long));
            unsafe
            {
                Console.WriteLine(sizeof(Person));
            }
        }

        private struct Person
        {
            public int ID;
            public long Height;
        }

        [TestMethod]
        public void Demo7()
        {
            unsafe
            {
                Person p;
                p.ID = 1;
                p.Height = 180;

                Person* pp = &p;
                pp->Height = 179;
                Console.WriteLine(p.Height);
            }
        }

        [TestMethod]
        public void Demo8()
        {
            var x = int.MinValue;
            int y = -x;
            Console.WriteLine(x);
            Console.WriteLine(y);

            Console.WriteLine(Convert.ToString(x, 2).PadLeft(32, '0'));
            Console.WriteLine(Convert.ToString(y, 2).PadLeft(32, '0'));
        }

        [TestMethod]
        public void Demo9()
        {
            Student stu = new Student() { Name = "kyou" };
            something(stu);
            Console.WriteLine(stu.Name);
            Console.WriteLine(stu.GetHashCode());
        }

        private static void something(Student stu)
        {
            stu = new Student() { Name = "kyou" };
            Console.WriteLine(stu.Name);
            Console.WriteLine(stu.GetHashCode());
        }

        [TestMethod]
        public void Demo10()
        {
            Console.WriteLine("{0:C}", 12.5565);
            // 值类型： 结构体、枚举
            // 引用类型：接口、委托、类、数组
        }

        [TestMethod]
        public void Demo11()
        {
            var mc = new MyClass();
            mc.add(5, 6);
        }

        partial class MyClass
        {
            partial void PrintSum(int x, int y);

            public void add(int x, int y)
            {
                PrintSum(x, y);
            }
        }

        partial class MyClass
        {
            partial void PrintSum(int x, int y)
            {
                Console.WriteLine("Sum is {0}", x + y);
            }
        }

        [TestMethod]
        public void Demo12()
        {
            SecondClass sc = new SecondClass();
            BaseClass bc = (BaseClass)sc;
            bc.Print();
        }

        private class BaseClass
        {
            virtual public void Print()
            {
                Console.WriteLine("base class print");
            }
        }

        private class FirstClass : BaseClass
        {
            public FirstClass()
            {
                Console.WriteLine("first class constructor");
            }

            override public void Print()
            {
                Console.WriteLine("first class print");
            }
        }

        private class SecondClass : FirstClass
        {
            public SecondClass()
            {
                Console.WriteLine("second class constructor");
            }

            public SecondClass(int x) : this()
            {
                Console.WriteLine("second class constructor + {0}", x);
            }

            public new void Print()
            {
                Console.WriteLine("second class print");
            }
        }

        [TestMethod]
        public void Demo13()
        {
            new SecondClass(2);
        }

        [TestMethod]
        public void Demo14()
        {
            string test0 = "23";
            string test1 = "d";
            string test2 = test0 + test1;

            double test3 = Double.Parse(test0);
            double test4 = Double.Parse(test2);
            Console.WriteLine("debug");
        }

        [TestMethod]
        public void Demo15()
        {
            MyData md = new MyData(1, 4, 2);
            Console.WriteLine(md.Average());
        }

        [TestMethod]
        public void Demo16()
        {
            char c1 = '\x005C';
            Console.WriteLine(c1);
        }

        [TestMethod]
        public void Demo17()
        {
            string rst = " Print \x00A Multiple \u000A Lines";
            string vst = @" Print
Multiple
Lines";
            Console.WriteLine(rst);
            Console.WriteLine(vst);
        }

        [TestMethod]
        public void Demo18()
        {
            //隐式定义类型转换，显式：将implicit改为explicit
            LimitedInt li = 400;
            int i = li;
            Console.WriteLine("LimitedInt: {0}，int: {1}", li.Value, i);

            //重载运算符
            LimitedInt nli = -li;
            LimitedInt num = 44;
            LimitedInt end = li - num;
            LimitedInt sum = end + 2.3;
            Console.WriteLine("取负数{0}", nli.Value);
            Console.WriteLine("相减{0}", end.Value);
            Console.WriteLine("加Double{0}", sum.Value);
        }

        private class LimitedInt
        {
            private const int MaxValue = 100;
            private const int MinValue = 0;

            private int _theValue = 0;

            public int Value
            {
                get { return _theValue; }
                set
                {
                    if (value > MaxValue) _theValue = MaxValue;
                    else _theValue = MinValue < value ? value : MinValue;
                }
            }

            public static implicit operator int(LimitedInt li)
            {
                return li.Value;
            }

            public static implicit operator LimitedInt(int i)
            {
                LimitedInt li = new LimitedInt();
                li.Value = i;
                return li;
            }

            public static LimitedInt operator -(LimitedInt x)
            {
                LimitedInt li = new LimitedInt();
                li.Value = 0;
                return li;
            }

            public static LimitedInt operator -(LimitedInt x, LimitedInt y)
            {
                LimitedInt li = new LimitedInt();
                li.Value = x.Value - y.Value;
                return li;
            }

            public static LimitedInt operator +(LimitedInt x, double y)
            {
                LimitedInt li = new LimitedInt();
                li.Value = x.Value + (int)y;
                return li;
            }
        }

        [TestMethod]
        public void Demo19()
        {
            Simple s1 = new Simple();
            Console.WriteLine("{0},{1}", s1.x, s1.y);

            Simple s2 = new Simple(1, 3);

            Console.WriteLine("{0},{1}", s2.x, s2.y);
        }

        private struct Simple
        {
            public int x;
            public int y;

            public Simple(int a, int b)
            {
                x = a;
                y = b;
            }

            static Simple()
            {
            }
        }

        [TestMethod]
        public void Demo20()
        {
            TrafficLight t1 = TrafficLight.Green;
            TrafficLight t2 = TrafficLight.Yellow;
            TrafficLight t3 = TrafficLight.Red;
            Console.WriteLine("{0},\t{1}", t1, (int)t1);
            Console.WriteLine("{0},\t{1}", t2, (int)t2);
            Console.WriteLine("{0},\t{1}\n", t3, (int)t3);
        }

        private enum TrafficLight
        {
            Green,
            Yellow,
            Red
        }

        [TestMethod]
        public void Demo21()
        {
            CardDeckSettings ops;
            ops = CardDeckSettings.FancyNumbers;
            Console.WriteLine(ops.ToString());

            ops = CardDeckSettings.FancyNumbers | CardDeckSettings.Animation;
            Console.WriteLine(ops.ToString());
        }

        [Flags]
        private enum CardDeckSettings : uint
        {
            SingleDeck = 0x01,
            LargePictures = 0x02,
            FancyNumbers = 0x04,
            Animation = 0x08
        }

        [TestMethod]
        public void Demo22()
        {
            OptionClass oc = new OptionClass();
            CardDeckSettings ops = CardDeckSettings.SingleDeck | CardDeckSettings.Animation
                                    | CardDeckSettings.FancyNumbers;

            oc.SetOptions(ops);
            oc.PrintOptions();
        }

        private class OptionClass
        {
            private bool UseSingleDeck = false,
                 UseBigPics = false,
                 UseFancyNumbers = false,
                 UseAnimation = false,
                 UseAnimationAndFancyNumbers = false;

            public void SetOptions(CardDeckSettings ops)
            {
                UseSingleDeck = ops.HasFlag(CardDeckSettings.SingleDeck);
                UseBigPics = ops.HasFlag(CardDeckSettings.LargePictures);
                UseFancyNumbers = ops.HasFlag(CardDeckSettings.FancyNumbers);
                UseAnimation = ops.HasFlag(CardDeckSettings.Animation);
                CardDeckSettings testFlags = CardDeckSettings.Animation | CardDeckSettings.FancyNumbers;
                UseAnimationAndFancyNumbers = ops.HasFlag(testFlags);
            }

            public void PrintOptions()
            {
                Console.WriteLine("Option settings:");
                Console.WriteLine("Use Single Deck                 - {0}", UseSingleDeck);
                Console.WriteLine("Use Large Pictures              - {0}", UseBigPics);
                Console.WriteLine("Use Fancy Numbers               - {0}", UseFancyNumbers);
                Console.WriteLine("Show Animation                  - {0}", UseAnimation);
                Console.WriteLine("Show Animation and FancyNumbers - {0}", UseAnimationAndFancyNumbers);
            }
        }

        [TestMethod]
        public void Demo23()
        {
            var arr = new int[,] { { 0, 1, 2, 3 }, { 24, 13, 24, 33 } };
            for (int i = 0; i < 2; i++)
                for (int j = 0; j < 4; j++)
                {
                    Console.WriteLine("Element [ {0}, {1} ] is {2}", i, j, arr[i, j]);
                }
        }

        [TestMethod]
        public void Demo24()
        {
            int[][,] Arr = new int[3][,];
            Arr[0] = new int[,] { { 1, 2 }, { 23, 12 } };
            Arr[1] = new int[,] { { 1, 2, 2, 3 }, { 23, 12, 25, 32 } };
            Arr[2] = new int[,] { { 1, 2, 3 }, { 23, 12, 33 } };
            for (int i = 0; i < Arr.GetLength(0); i++)
            {
                for (int j = 0; j < Arr[i].GetLength(0); j++)
                {
                    for (int k = 0; k < Arr[i].GetLength(1); k++)
                        Console.WriteLine("[{0}][{1},{2}] = {3}", i, j, k, Arr[i][j, k]);
                    Console.WriteLine("");
                }
                Console.WriteLine("");
            }

            Console.WriteLine("start foreach");
            foreach (int[,] arr1 in Arr)
            {
                Console.WriteLine("new array");
                foreach (int item in arr1)
                {
                    Console.WriteLine(item);
                }
            }
        }

        [TestMethod]
        public void Demo25()
        {
            int[,] arr = { { 1, 2, 3, 4, 6 }, { 22, 32, 42, 63, 33 } };
            foreach (var item in arr)
            {
                Console.WriteLine(item);
            }
        }

        [TestMethod]
        public void Demo26()
        {
            Task t = DoWorkAsync();
            t.Wait();
        }

        private static int Get10()
        {
            return 10;
        }

        public static async Task DoWorkAsync()
        {
            Func<int> ten = new Func<int>(Get10);
            int a = await Task.Run(ten);

            int b = await Task.Run(new Func<int>(Get10));

            int c = await Task.Run(() => { return 10; });
            Console.WriteLine("{0} {1} {2}", a, b, c);
        }
    }

    internal static class ExtendMyData
    {
        public static double Average(this MyData md)
        {
            return (md.Sum() / 3);
        }
    }

    internal class MyData
    {
        private double D1;
        private double D2;
        private double D3;

        public MyData(double d1, double d2, double d3)
        {
            D1 = d1;
            D2 = d2;
            D3 = d3;
        }

        public Double Sum()
        {
            return D1 + D2 + D3;
        }
    }
}