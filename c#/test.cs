using System;
//使用 System 命名空间
//命名空间 可包括多个类
delegate int NumDel(int n);
namespace RectangleApplication
{
    //抽象类，抽象属性
    public abstract class Person 
    {
      public abstract string Name 
      { 
         //访问器
         get;
         set;
      }
    }

    class Student : Person
   {
    //抽象要实现
    public override string Name
      {
         get
         {
            return name;
         }
         set
         {
            name = value;
         }
      }
   }
    class Rectangle
    {
        // 成员变量
        double length;
        double width;
        //私有变量 obj.are 不行
        //Protected 访问修饰符允许子类访问它的基类的成员变量和成员函数
        //成员函数声明为 static。这样的函数只能访问静态变量。静态函数在对象被创建之前就已经存在。
        //静态变量用于定义常量，因为它们的值可以通过直接调用类而不需要创建类的实例来获取。
        private double are;
        public void Acceptdetails()
        {
            length = 4.5;    
            width = 3.5;
        }
        public double GetArea()
        {
            return length * width;
        }
        public void Display()
        {
            Console.WriteLine("Length: {0}", length);
            Console.WriteLine("Width: {0}", width);
            Console.WriteLine("Area: {0}", GetArea());
        }
    }

    class DelegateCtrl{
        public event NumDel numdel;
        public void show(int n)
        {
            numdel(n);
        }
    }
    
    class ExecuteRectangle:Rectangle
    {	
        void test()
    	{	
    		object obj = 100;
    		//对象类型变量的类型检查是在编译时发生的，而动态类型变量的类型检查是在运行时发生的。
    		dynamic d = 20;
    		//逐字字符串 = "C:\\Windows";
    		string str = @"C:\Windows";
    		string str1 = "sen"；
    		string re = str + str1;

    		int i = 75;
    		//"75"
    		Console.WriteLine(i.ToString()); 
    		//用户输入赋值
    		int num;
			num = Convert.ToInt32(Console.ReadLine());
			//void swap(ref int x, ref int y) 引用参数
			/*void getValue(out int x ) 外部传的值最后为5
	      	{
	         	int temp = 5;
	         	x = temp;
	      	}*/
    	}
    	void arrTest()
    	{
    		int [] arr = new int[10];
    		 for ( int i = 0; i < 10; i++ )
	         {
	            arr[i] = i + 100;
	         }
	         foreach (int j in n )
	         {
	            int i = j-100;
	            Console.WriteLine("Element[{0}] = {1}", i, j);
	         }
    	}
        public static int getNum(int n)
        {
            return n+1;
        }
        public static int getNum1(int n)
        {
            return n+1;
        }
        static void Main(string[] args)
        {   
            public event 
            Rectangle r = new Rectangle();
            r.Acceptdetails();
            r.Display();
            Console.ReadLine();

            NumDel de1 = new NumDel(getNum);
            de1 += getNum1;
            /* NumDel de1;
            de1 = getNum ;
            de1 += getNum1; 
            */
            //两函数顺序执行
            de1(1); 

            DelegateCtrl dctrl = new DelegateCtrl();
            dctrl.numdel += getNum;
            dctrl.numdel += getNum1;
            dctrl.show(1);
        }
    }
}