using XLua;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Reflection;

namespace behaviac
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
[Hotfix]
    public class TypeMetaInfoAttribute : Attribute
    {
        public TypeMetaInfoAttribute(string displayName, string description)
        {
            this.displayName_ = displayName;
            this.desc_ = description;
        }

        public TypeMetaInfoAttribute()
        {
        }

        private string displayName_;
        private string desc_;

        public string DisplayName
        {
            get
            {
                return this.displayName_;
            }
        }

        public string Description
        {
            get
            {
                return this.desc_;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
[Hotfix]
    public class MemberMetaInfoAttribute : TypeMetaInfoAttribute
    {
        public MemberMetaInfoAttribute(string displayName, string description)
            : this(displayName, description, 1.0f)
        {
        }

		public MemberMetaInfoAttribute(string displayName, string description, float range)
			: base(displayName, description)
		{
			m_range = range;
		}

        public MemberMetaInfoAttribute()
        {
        }

        private static string getEnumName(object obj)
        {
            if (obj == null)
                return string.Empty;

            Type type = obj.GetType();
            if (!type.IsEnum)
            {
                return string.Empty;
            }

            string enumName = Enum.GetName(type, obj);
            if (string.IsNullOrEmpty(enumName))
            {
                return string.Empty;
            }

            return enumName;
        }

        public static string GetEnumDisplayName(object obj)
        {
            if (obj == null)
                return string.Empty;

            string enumName = getEnumName(obj);

            System.Reflection.FieldInfo fi = obj.GetType().GetField(obj.ToString());
            Attribute[] attributes = (Attribute[])fi.GetCustomAttributes(typeof(MemberMetaInfoAttribute), false);
            if (attributes.Length > 0)
                enumName = ((MemberMetaInfoAttribute)attributes[0]).DisplayName;

            return enumName;
        }

        public static string GetEnumDescription(object obj)
        {
            if (obj == null)
                return string.Empty;

            string enumName = getEnumName(obj);

            System.Reflection.FieldInfo fi = obj.GetType().GetField(obj.ToString());
            Attribute[] attributes = (Attribute[])fi.GetCustomAttributes(typeof(MemberMetaInfoAttribute), false);
            if (attributes.Length > 0)
                enumName = ((MemberMetaInfoAttribute)attributes[0]).Description;

            return enumName;
        }

		private float m_range = 1.0f;
		public float Range
		{
			get
			{
				return this.m_range;
			}
		}
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
[Hotfix]
    public class MethodMetaInfoAttribute : TypeMetaInfoAttribute
    {
        public MethodMetaInfoAttribute(string displayName, string description)
            : base(displayName, description)
        {
        }

        public MethodMetaInfoAttribute()
        {
        }

        public virtual bool IsNamedEvent
        {
            get
            {
                return false;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Delegate, AllowMultiple = false, Inherited = false)]
[Hotfix]
    public class EventMetaInfoAttribute : MethodMetaInfoAttribute
    {
        public EventMetaInfoAttribute(string displayName, string description)
            : base(displayName, description)
        {
        }

        public EventMetaInfoAttribute()
        {
        }

        public override bool IsNamedEvent
        {
            get
            {
                return true;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
[Hotfix]
    public class ParamMetaInfoAttribute : TypeMetaInfoAttribute
    {
        public ParamMetaInfoAttribute()
        {
        }

        public ParamMetaInfoAttribute(string displayName, string description, string defaultValue)
            : base(displayName, description)
        {
            defaultValue_ = defaultValue;
            rangeMin_ = float.MinValue;
            rangeMax_ = float.MaxValue;
        }

        public ParamMetaInfoAttribute(string displayName, string description, string defaultValue, float rangeMin, float rangeMax)
            : base(displayName, description)
        {
            defaultValue_ = defaultValue;
            rangeMin_ = rangeMin;
            rangeMax_ = rangeMax;
        }

        private string defaultValue_;
        public string DefaultValue
        {
            get { return defaultValue_; }
        }

        private float rangeMin_ = float.MinValue;
        public float RangeMin
        {
            get { return rangeMin_; }
        }

        private float rangeMax_ = float.MaxValue;
        public float RangeMax
        {
            get { return rangeMax_; }
        }
    }
}

