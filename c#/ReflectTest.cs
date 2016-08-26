using UnityEngine;
using System.Collections;
using System.Reflection;

using System;

public class ReflectTest : MonoBehaviour {

	void Start () {
        reflectTest();
        //constructTest();
    }

    private void reflectTest()
    {
        //string n = "grayworm";
        //Type t = n.GetType();
        //Debug.Log(t);
        //int i = 1;
        //foreach (MemberInfo mi in t.GetMembers())
        //{
        //    Debug.Log(i+""+mi.MemberType); //MemberTypes: Method, Field...
        //    Debug.Log(i + "" + mi.Name);  
        //    i++;
        //}

        //查看类中的属性
        Debug.Log("GetProperties");
        Student st = new Student();
        Type tp = st.GetType();
        PropertyInfo[] pis = tp.GetProperties(); //属性 get,set
        int p = 1;
        foreach (PropertyInfo pi in pis)
        {
            Debug.Log(pi.Name);
            p++;
        }

        Debug.Log("GetMethods");
        int m = 1;
        MethodInfo[] mis = tp.GetMethods();  //public方法
        foreach (MethodInfo mi in mis)
        {
            Debug.Log(m + "" + mi.ReturnType);
            Debug.Log(m + "" + mi.Name);
            m++;
        }

        Debug.Log("GetFields");
        int f = 1;
        FieldInfo[] fis = tp.GetFields();  //public字段
        foreach (FieldInfo fi in fis)
        {
            Debug.Log(f + "" + fi.Name);
            f++;
        }
    }

    private void constructTest()
    {
        Student st = new Student();
        Type tp = st.GetType();
        Debug.Log(tp);
        ConstructorInfo[] ci = tp.GetConstructors();    //获取类的所有构造函数
        int i = 1;
        foreach (ConstructorInfo c in ci) //遍历构造函数
        {
            ParameterInfo[] ps = c.GetParameters();    //取出每个构造函数的所有参数
            foreach (ParameterInfo pi in ps)   //遍历该构造函数的所有参数
            {
                Debug.Log(i + "" + pi.ParameterType.ToString());
                Debug.Log(i + "" + pi.Name); 
            }
            i++;
        }

        Type[] pt = new Type[2];
        pt[0] = typeof(string);
        pt[1] = typeof(int);
        ConstructorInfo cmethod = tp.GetConstructor(pt); //根据参数类型获取构造函数 
        
        object[] obj = new object[2] { "sen", 23 }; //构造Object数组，作为构造函数的输入参数 
        object newStudent = cmethod.Invoke(obj); //调用构造函数生成对象 
        ((Student)newStudent).setId(32);

        //方法二
        object[] obj2 = new object[1] { "qing" };
        //用Activator的CreateInstance静态方法，生成新对象 
        object newStudent2 = Activator.CreateInstance(tp, obj2);
        ((Student)newStudent2).setId(37);

        object obj3 = Activator.CreateInstance(tp);
        //取得ID字段 
        FieldInfo fi = tp.GetField("_age");
        fi.SetValue(obj3, 25);

        //取得MyName属性 
        PropertyInfo pi1 = tp.GetProperty("Sex");
        pi1.SetValue(obj3, 1, null);

        //取得show方法 
        MethodInfo mi = tp.GetMethod("show");
        //调用show方法 
        mi.Invoke(obj3, null);
    }
}
