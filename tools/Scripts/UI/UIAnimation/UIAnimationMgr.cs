using UnityEngine;
using UnityEngine.UI;
using System;
using SG;
using XLua;

[LuaCallCSharp]

public class UIAnimationMgr 
{
    private static UIAnimationMgr s_instance = null;
    public static  UIAnimationMgr Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = new UIAnimationMgr();
            }
            return s_instance;
        }
    }

     

#region Animation

    [CSharpCallLua]
    public delegate string GetAnimationCfg(int type);
     
    public T GetAnimation<T>(int type)
    {
        string s = "";
        GetAnimationCfg func = LuaMgr.Instance.GetLuaEnv().Global.Get<GetAnimationCfg>("GetAnimationCfg");
        if (func != null)
        {
            s = func(type);
        }
        T t = default(T);
        GameObject obj = CoreEntry.gResLoader.Load(s) as GameObject;
        if (obj)
        {
            t = obj.GetComponent<T>();
        }
        return t;
    }

    public void PlayAnimation<T>(GameObject gameObject, System.Action action, int type = 1, float x = 0,float y = 0) where T : Component
    {
        T ani = gameObject.GetComponent<T>();
        if (ani == null)
        {
            ani = gameObject.AddComponent<T>();
        }
        if (ani)
        {
            var aniCfg = UIAnimationMgr.Instance.GetAnimation<T>(type);
            var v = ani as UIAnimationPopup;
            v.ScaleAnimation = (aniCfg as UIAnimationPopup).ScaleAnimation;
            v.AlphaAnimation = (aniCfg as UIAnimationPopup).AlphaAnimation;
            v.PosXAnimation = (aniCfg as UIAnimationPopup).PosXAnimation;
            v.PosYAnimaton = (aniCfg as UIAnimationPopup).PosYAnimaton;
            v.SetAnimationFrame();
            v.CallBack = action;
            v.originalPos = new Vector2(x, y);
            v.Play();
        }
    }
    //���ŶԻ��򶯻�
    //gameObject ���Ŷ������� action ���������ص�
    //type 1 �Ի������� 2 commonTopbar����  3 �����б�����
    public void PlayAni(GameObject gameObject, System.Action action, int type,float x =0,float y =0)
    {
        PlayAnimation<UIAnimationPopup>(gameObject, action, type,x,y);  
    }
    //���ŶԻ��򶯻�
    //gameObject ���Ŷ������� action ���������ص�
    public void PlayAnimation(GameObject gameObject, System.Action action,int type = 1,float x =0,float y =0)
    {
        PlayAnimation<UIAnimationPopup>(gameObject, action, type,x,y);
    }
   
#endregion

}

