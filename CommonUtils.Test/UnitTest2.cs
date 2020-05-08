using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace CommonUtils.Test
{
    [TestClass]
    public class UnitTest2
    {
        private delegate void MyDel(int value);

        private void PrintLow(int value)
        {
            Console.WriteLine("{0} - Low Value", value);
        }

        private void PrintHigh(int value)
        {
            Console.WriteLine("{0} - High Value", value);
        }

        private void PrintDoubleNum(int value)
        {
            Console.WriteLine("{0} - Double Value", value * 2);
        }

        [TestMethod]
        public void demo()
        {
            MyDel del;
            Random rand = new Random();
            int randNum = rand.Next(99);
            del = randNum < 50 ? new MyDel(PrintLow) : new MyDel(PrintHigh);
            del += PrintDoubleNum;
            del += PrintDoubleNum;
            del -= PrintDoubleNum;
            del(randNum);
        }

        private class Test
        {
            public void Print1()
            {
                Console.WriteLine("Print1 -- instance");
            }

            public static void Print2()
            {
                Console.WriteLine("Print2 -- static");
            }
        }

        private delegate void PrintFunction();

        [TestMethod]
        public void demo1()
        {
            Test t = new Test();
            PrintFunction pf;
            pf = t.Print1;
            pf += Test.Print2;
            pf += t.Print1;
            pf += Test.Print2;

            if (null != pf)
                pf();
            else
                Console.WriteLine("delegate is empty");
        }

        private delegate int TheDel();

        private class MyClass
        {
            private int IntValue = 5;

            public int add2()
            {
                IntValue += 2; return IntValue;
            }

            public int add3()
            {
                IntValue += 3; return IntValue;
            }
        }

        [TestMethod]
        public void demo2()
        {
            MyClass mc = new MyClass();
            TheDel del = mc.add2;
            del += mc.add3;
            del += mc.add2;
            del += delegate () { return 1; };
            del += () => 2;
            Console.WriteLine("Value: {0}", del());
        }

        public class IncrementerEventArgs : EventArgs
        {
            public int IterationCount { get; set; }
        }

        private class Incrementer
        {
            public event EventHandler<IncrementerEventArgs> CoutedADozen;

            public void DoCount()
            {
                IncrementerEventArgs args = new IncrementerEventArgs();
                for (int i = 1; i < 100; i++)
                {
                    if (i % 12 == 0 && CoutedADozen != null)
                    {
                        args.IterationCount = i;
                        CoutedADozen(this, args);
                    }
                }
            }
        }

        private class Dozens
        {
            public int DozensCount { get; private set; }

            public Dozens(Incrementer incrementer)
            {
                DozensCount = 0;
                incrementer.CoutedADozen += IncrementDozensCount;
            }

            public Dozens()
            {
                DozensCount = 0;
            }

            public void IncrementDozensCount(object send, IncrementerEventArgs e)
            {
                Console.WriteLine("Incremented at iteration:{0} in {1}", e.IterationCount, send.ToString());
                DozensCount++;
            }
        }

        [TestMethod]
        public void demo3()
        {
            Incrementer incrementer = new Incrementer();
            //Dozens dozensCounter = new Dozens(incrementer);
            Dozens dozensCounter = new Dozens();
            incrementer.CoutedADozen += dozensCounter.IncrementDozensCount;
            incrementer.DoCount();
            Console.WriteLine("dozensCount:{0}", dozensCounter.DozensCount);
        }

        private class Publisher
        {
            public event EventHandler SimpleEvent;

            public void RaiseTheEvent()
            {
                SimpleEvent(this, null);
            }
        }

        private class Subscriber
        {
            public void MethodA(object o, EventArgs e)
            {
                Console.WriteLine("AAA");
            }

            public void MethodB(object o, EventArgs e)
            {
                Console.WriteLine("BBB");
            }
        }

        [TestMethod]
        public void demo4()
        {
            Publisher p = new Publisher();
            Subscriber s = new Subscriber();
            p.SimpleEvent += s.MethodA;
            p.SimpleEvent += s.MethodB;
            p.RaiseTheEvent();

            Console.WriteLine("\r\nRemove MethodB");
            p.SimpleEvent -= s.MethodB;
            p.RaiseTheEvent();
        }

        private interface IInfo
        {
            string GetName();

            string GetAge();
        }

        private class CA : IInfo
        {
            public string Name;
            public int Age;

            public string GetName()
            {
                return Name;
            }

            public string GetAge()
            {
                return Age.ToString();
            }
        }

        private class CB : IInfo
        {
            public string First;
            public string Last;
            public double PersonsAge;

            public string GetName()
            {
                return First + " " + Last;
            }

            public string GetAge()
            {
                return PersonsAge.ToString();
            }
        }

        private static void PrintInfo(IInfo info)
        {
            Console.WriteLine("name: {0}, age: {1}", info.GetName(), info.GetAge());
        }

        [TestMethod]
        public void demo5()
        {
            CA ca = new CA() { Name = "Jhon Doe", Age = 35 };
            CB cb = new CB() { First = "Jhon", Last = "Doe", PersonsAge = 35 };

            PrintInfo(ca);
            PrintInfo(cb);
        }

        private class MyCompClass : IComparable
        {
            public int Num { get; set; }

            public int CompareTo(object o)
            {
                MyCompClass mc = (MyCompClass)o;
                if (Num > mc.Num) return 1;
                if (Num < mc.Num) return -1;
                return 0;
            }
        }

        private static void PrintMyCompClassArray(string s, MyCompClass[] mc)
        {
            Console.Write(s);
            for (int i = 0; i < mc.Length; i++)
            {
                Console.Write("{0} ", mc[i].Num);
            }
            Console.WriteLine(" ");
        }

        [TestMethod]
        public void demo6()
        {
            int[] arry = new[] { 20, 12, 3, 9, 13 };

            MyCompClass[] mcArr = new MyCompClass[arry.Length];
            for (int i = 0; i < arry.Length; i++)
            {
                mcArr[i] = new MyCompClass();
                mcArr[i].Num = arry[i];
            }
            PrintMyCompClassArray("Initial Order: ", mcArr);

            Array.Sort(mcArr);

            PrintMyCompClassArray("Sorted Order: ", mcArr);
        }

        private interface IIfc1
        { void PrintOut(string s); }

        private interface IIfc2
        { void PrintOut(string t); }

        private class ClassIn : IIfc1, IIfc2
        {
            void IIfc1.PrintOut(string s)
            {
                Console.WriteLine("IIfc1: {0}", s);
            }

            void IIfc2.PrintOut(string t)
            {
                Console.WriteLine("IIfc2: {0}", t);
            }
        }

        [TestMethod]
        public void demo7()
        {
            ClassIn ci = new ClassIn();
            IIfc1 ifc1 = (IIfc1)ci;
            ifc1.PrintOut("interface 1");

            IIfc2 ifc2 = (IIfc2)ci;
            ifc2.PrintOut("interface 2");
        }

        private interface ILiveBirth
        {
            string BabyCalled();
        }

        private class Animal
        { }

        private class Cat : Animal, ILiveBirth
        {
            string ILiveBirth.BabyCalled()
            {
                return "kitten";
            }
        }

        private class Dog : Animal, ILiveBirth
        {
            string ILiveBirth.BabyCalled()
            {
                return "puppy";
            }
        }

        private class Bird : Animal
        {
        }

        [TestMethod]
        public void demo8()
        {
            Animal[] animals = new Animal[3];
            animals[0] = new Cat();
            animals[1] = new Bird();
            animals[2] = new Dog();
            foreach (Animal animal in animals)
            {
                ILiveBirth live = animal as ILiveBirth;
                if (null != live) Console.WriteLine("Baby is called: {0}", live.BabyCalled());
            }
        }

        [TestMethod]
        public void demo9()
        {
            ushort sh = 10;
            byte sb = (byte)sh;
            Console.WriteLine("sb: {0} = 0x{0:X} ", sb);

            sh = 1365;
            sb = (byte)sh;
            Console.WriteLine("sb: {0} = 0x{0:X}", sb);

            //decimal d = 1365.122M; //checked不checked都会抛异常
            //sb = (byte)d;
            //Console.WriteLine("sb: {0} = 0x{0:X}", sb);
        }

        private class Person
        {
            public string Name;
            public int Age;

            public Person()
            {
            }

            public Person(string name, int age)
            {
                Name = name;
                Age = age;
            }

            public static implicit operator int(Person p)
            {
                return p.Age;
            }

            public static implicit operator Person(int i)
            {
                return new Person("Nemo", i);
            }

            public static implicit operator string(Person p)
            {
                return p.Name;
            }
        }

        [TestMethod]
        public void demo10()
        {
            Person bill = new Person("Bill", 12);
            int age = bill;

            Console.WriteLine("Name: {0}, Age: {1}", bill.Name, bill.Age);

            Person nemo = 13;
            Console.WriteLine("Name: {0}, Age: {1}", nemo.Name, nemo.Age);
        }

        private class Employee : Person
        { }

        [TestMethod]
        public void demo12()
        {
            Employee e = new Employee();
            e.Name = "William";
            e.Age = 23;
            float fVar = e;
            Console.WriteLine("Person Info: {0}, {1}", e.Name, fVar);
        }

        private class Simple
        {
            static public void ReverseAndPoint<T>(T[] arr)
            {
                Array.Reverse(arr);
                foreach (T item in arr)
                {
                    Console.Write("{0}, ", item.ToString());
                }
                Console.WriteLine("");
            }
        }

        [TestMethod]
        public void demo13()
        {
            var intArr = new int[] { 2, 3, 4, 6, 9 };
            var strArr = new string[] { "f", "s", "t" };
            var doubleArr = new Double[] { 2.3, 2.442, 4.22 };

            Simple.ReverseAndPoint(intArr);
            Simple.ReverseAndPoint<int>(intArr);
            Simple.ReverseAndPoint(strArr);
            Simple.ReverseAndPoint<string>(strArr);
            Simple.ReverseAndPoint(doubleArr);
            Simple.ReverseAndPoint<double>(doubleArr);
        }

        public delegate TR Func<T1, T2, TR>(T1 t1, T2 t2);

        private class Simple1
        {
            static public string PrintString(int p1, int p2)
            {
                int total = p1 + p2;
                return total.ToString();
            }
        }

        [TestMethod]
        public void demo14()
        {
            Func<int, int, string> func = new Func<int, int, string>(Simple1.PrintString);
            Console.WriteLine(func(12, 34));
        }

        private class Animal1
        { public int Legs = 4; }

        private class Dog1 : Animal1
        { }

        private delegate T Factory<out T>();

        private static Dog1 MakeDog()
        {
            return new Dog1();
        }

        //协变
        [TestMethod]
        public void demo15()
        {
            Factory<Dog1> dogMaker = MakeDog;
            Factory<Animal1> anMaker = dogMaker;
            Console.WriteLine(anMaker().Legs.ToString());
        }

        private delegate void Action<in T>(T a);

        private static void ActionOnAnimal(Animal1 a)
        { Console.WriteLine(a.Legs.ToString()); }

        //逆变
        [TestMethod]
        public void demo16()
        {
            Action<Animal1> anMaker = ActionOnAnimal;
            Action<Dog1> dogMaker = anMaker;
            dogMaker(new Dog1());
        }

        private class Food
        { public string Name; }

        private class Apple : Food
        { }

        private interface IMyIfc<out T>
        { T GetList(); }

        private class SimpleReturn<T> : IMyIfc<T>
        {
            public T[] items = new T[2];

            public T GetList()
            {
                return items[0];
            }
        }

        private static void PrintName(IMyIfc<Food> returner)
        {
            Console.WriteLine(returner.GetList().Name);
        }

        [TestMethod]
        public void demo17()
        {
            SimpleReturn<Apple> aReturn = new SimpleReturn<Apple>();
            aReturn.items[0] = new Apple() { Name = "apple" };

            IMyIfc<Food> fReturner = aReturn;

            PrintName(fReturner);
        }

        [TestMethod]
        public void demo18()
        {
            int[] arr = { 1, 2, 3, 4 };
            IEnumerator enumerator = arr.GetEnumerator();
            while (enumerator.MoveNext())
            {
                int i = (int)enumerator.Current;
                Console.WriteLine(i);
            }
        }

        private class ColorEnumerator : IEnumerator
        {
            private string[] _colors;
            private int _position = -1;

            public ColorEnumerator(string[] theColors)
            {
                _colors = new string[theColors.Length];
                for (int i = 0; i < theColors.Length; i++)
                {
                    _colors[i] = theColors[i];
                }
            }

            public object Current
            {
                get
                {
                    if (_position == -1) throw new InvalidOperationException();
                    if (_position >= _colors.Length) throw new InvalidOperationException();
                    return _colors[_position];
                }
            }

            public bool MoveNext()
            {
                if (_position < _colors.Length - 1)
                {
                    _position++;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public void Reset()
            {
                _position = -1;
            }
        }

        private class Spectrum : IEnumerable
        {
            private string[] Colors = { "violet", "blue", "cyan", "green", "yellow", "orange", "red" };

            public IEnumerator GetEnumerator()
            {
                //return BlackAndWhite();
                return new ColorEnumerator(Colors);
            }

            public IEnumerable<string> GetEnumerable()
            {
                yield return "violet";
                yield return "cyan";
                yield return "red";
            }
        }

        [TestMethod]
        public void demo19()
        {
            Spectrum spectrum = new Spectrum();
            foreach (var item in spectrum)
            {
                Console.WriteLine(item);
            }

            foreach (var item in spectrum.GetEnumerable())
            {
                Console.Write("{0} ", item);
            }
        }

        public static System.Collections.Generic.IEnumerator<string> BlackAndWhite()
        {
            string[] theColors = { "black", "gray", "white" };
            for (int i = 0; i < theColors.Length; i++)
                yield return theColors[i];
        }

        private class Spectrum1
        {
            private bool _listFromUVtoIR;
            private string[] colors = { "violet", "blue", "cyan", "green", "yellow", "orange", "red" };

            public Spectrum1(bool listFromUVtoIR)
            {
                _listFromUVtoIR = listFromUVtoIR;
            }

            public IEnumerator<string> GetEnumerator()
            {
                return _listFromUVtoIR ? UVtoRIR : RIPtoUV;
            }

            public IEnumerator<string> UVtoRIR
            {
                get
                {
                    for (int i = 0; i < colors.Length; i++)
                    {
                        yield return colors[i];
                    }
                }
            }

            public IEnumerator<string> RIPtoUV
            {
                get
                {
                    for (int i = colors.Length - 1; i >= 0; i--)
                    {
                        yield return colors[i];
                    }
                }
            }
        }

        //迭代器
        [TestMethod]
        public void demo20()
        {
            Spectrum1 startUV = new Spectrum1(true);
            Spectrum1 startIR = new Spectrum1(false);
            foreach (var item in startUV)
            {
                Console.Write("{0} ", item);
            }
            Console.WriteLine("");
            foreach (var item in startIR)
            {
                Console.Write("{0} ", item);
            }
        }

        [TestMethod]
        public void demo21()
        {
            int[] numbers = { 2, 12, 5, 15 };
            IEnumerable<int> lowNums = from n in numbers where n < 10 select n;
            foreach (var item in lowNums)
            {
                Console.WriteLine(item);
            }
        }

        //linq
        [TestMethod]
        public void demo22()
        {
            int[] numbers = { 2, 5, 28, 31, 17, 16, 42 };
            var numsQuery = from n in numbers
                            where n < 20
                            select n;
            var numsMethod = numbers.Where(x => x < 20);

            int numsCount = (from n in numbers
                             where n < 20
                             select n).Count();

            foreach (var item in numsQuery)
            {
                Console.Write("{0}, ", item);
            }
            Console.WriteLine("");
            foreach (var item in numsMethod)
            {
                Console.Write("{0}, ", item);
            }
            Console.WriteLine("");
            Console.WriteLine(numsCount);
        }

        private class Student
        {
            public int StID;
            public string LastName;
        }

        private class CourseStudent
        {
            public string CourseName;
            public int StID;
        }

        private static Student[] students = new Student[]
        {
            new Student(){ StID = 1, LastName = "Carson" },
            new Student(){ StID = 2, LastName = "Klassen" },
            new Student(){ StID = 3, LastName = "Fleming" }
        };

        private static CourseStudent[] courses = new CourseStudent[]
        {
            new CourseStudent(){ CourseName = "Art", StID = 1 },
            new CourseStudent(){ CourseName = "Art", StID = 2 },
            new CourseStudent(){ CourseName = "History", StID = 1 },
            new CourseStudent(){ CourseName = "History", StID = 3 },
            new CourseStudent(){ CourseName = "Physics", StID = 3 }
        };

        [TestMethod]
        public void demo23()
        {
            var query = from stu in students
                        join c in courses on stu.StID equals c.StID
                        where c.CourseName == "Art"
                        select stu.LastName;
            foreach (var item in query)
            {
                Console.WriteLine("Art:{0}", item);
            }
        }

        [TestMethod]
        public void demo24()
        {
            var groupA = new[] { 3, 4, 5, 6 };
            var groupB = new[] { 6, 7, 8, 9 };
            //var someints = from a in groupA
            //              from b in groupB
            //              where a > 4 && b <= 8
            //              select new { a, b, sum = a + b };
            //var someints = from a in groupA
            //               from b in groupB
            //               let sum = a + b
            //               where sum == 12
            //               select new { a, b, sum };
            var someints = from a in groupA
                           from b in groupB
                           let sum = a + b
                           where sum >= 11
                           where a == 4
                           select new { a, b, sum };
            foreach (var item in someints)
            {
                Console.WriteLine(item);
            }
        }

        [TestMethod]
        public void demo25()
        {
            var students = new[]
            {
                new { Name = "Jhon", Age = 18, No = 2 },
                new { Name = "Tom", Age = 18, No = 1 },
                new { Name = "Bob", Age = 19, No = 3 },
                new { Name = "Smith", Age = 18, No = 4 }
            };
            var query = from stu in students
                        orderby stu.Age, stu.No
                        select stu;
            foreach (var item in query)
            {
                Console.WriteLine(item);
            }
        }

        [TestMethod]
        public void demo26()
        {
            var students = new[]
            {
                new { Name = "Jhon", Major="History" },
                new { Name = "Smith", Major="Art" },
                new { Name = "Tom", Major="Art" },
                new { Name = "Fleming", Major="History" }
            };

            var query = from stu in students
                        group stu by stu.Major;
            foreach (var s in query)
            {
                Console.WriteLine(s.Key);
                foreach (var t in s)
                {
                    Console.WriteLine("     {0}", t.Name);
                }
            }
        }

        [TestMethod]
        public void demo27()
        {
            int[] arr = { 1, 2, 3, 4, 5 };

            Console.WriteLine(arr.Count(n => n % 2 == 1));
        }

        [TestMethod]
        public void demo28()
        {
            XDocument empDoc = new XDocument(
                new XElement("Employees",
                    new XElement("Employee",
                        new XElement("Name", "Bob Smith"),
                        new XElement("PhoneNumber", "1234")
                    ),
                    new XElement("Employee",
                        new XElement("Name", "Sally Jones"),
                        new XElement("PhoneNumber", "12344"),
                        new XElement("PhoneNumber", "12345")
                    )
                )
            );
            XElement root = empDoc.Element("Employees");
            IEnumerable<XElement> emps = root.Elements();

            foreach (XElement emp in emps)
            {
                XElement empNameNode = emp.Element("Name");
                Console.WriteLine(empNameNode.Value);

                IEnumerable<XElement> empPhones = emp.Elements("PhoneNumber");
                foreach (var phone in empPhones)
                {
                    Console.WriteLine("     {0}", phone.Value);
                }
            }
        }

        [TestMethod]
        public void demo29()
        {
            XDocument xd = new XDocument(
                new XElement("root", new XElement("first"))
            );

            Console.WriteLine(xd);
            Console.WriteLine("");
            XElement root = xd.Element("root");
            root.Add(new XElement("second"));
            root.Add(
                new XElement("third"),
                new XComment("important comment"),
                new XElement("fourth")
            );
            Console.WriteLine(xd);
        }

        [TestMethod]
        public void demo30()
        {
            XDocument xd = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XComment("This is a comment"),
                new XProcessingInstruction("xml-stylesheet", @"href=""stories"" type=""text/css"""),
                new XElement("root",
                    new XElement("first"),
                    new XElement("second"),
                    new XAttribute("name", "k"),
                    new XAttribute("des", "myroot")
                )
            );

            Console.WriteLine(xd);
        }

        [TestMethod]
        public void demo31()
        {
            XDocument xd = new XDocument(
                new XElement("MyElements",
                    new XElement("first",
                        new XAttribute("color", "red"),
                        new XAttribute("size", "small")
                    ),
                    new XElement("second",
                        new XAttribute("color", "red"),
                        new XAttribute("size", "medium")
                    ),
                    new XElement("third",
                        new XAttribute("color", "blue"),
                        new XAttribute("size", "large")
                    )
                )
            );
            Console.WriteLine(xd);
            xd.Save("SimpleSample.xml");
        }

        [TestMethod]
        public void demo32()
        {
            XDocument xd = XDocument.Load("SimpleSample.xml");
            XElement root = xd.Element("MyElements");
            var xyz = from e in root.Elements()
                      where e.Name.ToString().Length == 5
                      select e;
            foreach (XElement element in xyz)
            {
                Console.WriteLine(element.Name.ToString());
            }
            Console.WriteLine();
            foreach (XElement element in xyz)
            {
                Console.WriteLine("Name: {0}, color: {1}, size: {2}", element.Name.ToString(),
                    element.Attribute("color").Value, element.Attribute("size").Value
                    );
            }

            var query = from e in root.Elements()
                        select new { e.Name, color = e.Attribute("color").Value };
            foreach (var item in query)
            {
                Console.WriteLine("name: {0, -6}, color: {1, -7}", item.Name, item.color);
            }
        }

        [TestMethod]
        public void demo33()
        {
        }
    }
}