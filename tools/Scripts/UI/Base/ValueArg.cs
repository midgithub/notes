/**
* @file     : ValueArg.cs
* @brief    : 数值或字符串等不确定类型数据
* @details  : 设置数据：SetValue() 获取数据：GetValue()
* @author   : 
* @date     : 2014-9-24
*/

using XLua;
using UnityEngine;
using System.Collections;

namespace SG
{

	public struct ValueArg {
		
		private NumUnion mNumValue;
		private ObjUnion mObjValue;
		private ValueType mValueType;
		
		[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)]
		private struct NumUnion
		{
			[System.Runtime.InteropServices.FieldOffset(0)]
			public bool mBool;

			[System.Runtime.InteropServices.FieldOffset(0)]
			public int mInt;
			
			[System.Runtime.InteropServices.FieldOffset(0)]
			public float mFloat;
			
			[System.Runtime.InteropServices.FieldOffset(0)]
			public double mNumber;
		}

		[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)]
		private struct ObjUnion
		{
			[System.Runtime.InteropServices.FieldOffset(0)]
			public string mStr;
			
			[System.Runtime.InteropServices.FieldOffset(0)]
			public GameObject mGameObj;

            [System.Runtime.InteropServices.FieldOffset(0)]
            public object mObj; 
		}
		
		private enum ValueType
		{
			NoValue,
			BoolValue,
			IntValue,
			FloatValue,
			DoubleValue,
			StringValue,
			GameObjectValue,
            OtherObject,
		}

		public void SetValue(bool value)
		{
			mValueType = ValueType.BoolValue;
			mNumValue.mBool = value;
		}

		public void SetValue(int value)
		{
			mValueType = ValueType.IntValue;
			mNumValue.mInt = value;
		}
		
		public void SetValue(float value)
		{
			mValueType = ValueType.FloatValue;
			mNumValue.mFloat = value;
		}
		
		public void SetValue(double value)
		{
			mValueType = ValueType.DoubleValue;
			mNumValue.mNumber = value;
		}
		
		public void SetValue(string value)
		{
			mValueType = ValueType.StringValue;
			mObjValue.mStr = value;
		}

		public void SetValue(GameObject value)
		{
			mValueType = ValueType.GameObjectValue;
			mObjValue.mGameObj = value;
		}

        public void SetValue(object value)
        {
            mValueType = ValueType.OtherObject;
            mObjValue.mObj = value;
        }
		
		public object GetValue()
		{
			switch (mValueType)
			{
			case ValueType.BoolValue:
				return mNumValue.mBool;
			case ValueType.IntValue:
				return mNumValue.mInt;
			case ValueType.FloatValue:
				return mNumValue.mFloat;
			case ValueType.DoubleValue:
				return mNumValue.mNumber;
			case ValueType.StringValue:
				return mObjValue.mStr;
			case ValueType.GameObjectValue:
				return mObjValue.mGameObj;

            case ValueType.OtherObject:
                return mObjValue.mObj; 

			default:
				return "NoValue";
			}
		}
	}

};//End SG



