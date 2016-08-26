using UnityEngine;
using System.Collections;

public class Student  {
    private string _name;
    private int _id;

    public int _age;

    public Student()
    {
        Debug.Log("默认构造函数");
    }

    public Student(int id)
    {
        _id = id;
    }

    public Student(string name)
    {
        _name = name;
    }

    public Student(string name,int id)
    {
        _name = name;
        _id = id;
    }

    public void setName(string name)
    {
        _name = name;
        Debug.Log(name);
    }

    public void setId(int id)
    {
        _id = id;
        Debug.Log(id);
    }

    private void setAge( )
    {
        Debug.Log("setAge");
    }

    public int Sex
    {
        get;
        set;
    }

    public void show()
    {
        Debug.Log("this is" + Sex);
    }
}
