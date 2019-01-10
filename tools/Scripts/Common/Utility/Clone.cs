using XLua;
﻿using UnityEngine;
using System.Collections;
using System;
using System.Reflection;
using SG;

[Hotfix]
public class Clone  {

    public static  object Copy( object obj)
    {
        System.Object targetDeepCopyObj;
        Type targetType = obj.GetType();
        //值类型
        if (targetType.IsValueType == true)
        {
            targetDeepCopyObj = obj;
        }
        //引用类型 
        else
        {
            targetDeepCopyObj = System.Activator.CreateInstance(targetType);   //创建引用对象 
            System.Reflection.MemberInfo[] memberCollection = obj.GetType().GetMembers();

            foreach (System.Reflection.MemberInfo member in memberCollection)
            {
                if (member.MemberType == System.Reflection.MemberTypes.Field)
                {
                    System.Reflection.FieldInfo field = (System.Reflection.FieldInfo)member;
                    System.Object fieldValue = field.GetValue(obj);
                    try
                    {

                        if (fieldValue is ICloneable)
                        {
                            field.SetValue(targetDeepCopyObj, (fieldValue as ICloneable).Clone());
                        }
                        else
                        {
                            field.SetValue(targetDeepCopyObj, Copy(fieldValue));
                        }

                    }
                    catch (Exception e)
                    {
                        LogMgr.UnityError(e.ToString()); 
                    }

                }
                else if (member.MemberType == System.Reflection.MemberTypes.Property)
                {
                    System.Reflection.PropertyInfo myProperty = (System.Reflection.PropertyInfo)member;
                    MethodInfo info = myProperty.GetSetMethod(false);
                    if (info != null)
                    {
                        object propertyValue = myProperty.GetValue(obj, null);
                        if (propertyValue is ICloneable)
                        {
                            myProperty.SetValue(targetDeepCopyObj, (propertyValue as ICloneable).Clone(), null);
                        }
                        else
                        {
                            myProperty.SetValue(targetDeepCopyObj, Copy(propertyValue), null);
                        }
                    }

                }
            }
        }
        return targetDeepCopyObj;
    }
	 






}

