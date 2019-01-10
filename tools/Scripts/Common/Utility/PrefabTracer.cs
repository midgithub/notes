using XLua;
﻿using UnityEngine;
using System.Collections;
using SG;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
#endif

[ExecuteInEditMode]
[Hotfix]
public class PrefabTracer : MonoBehaviour
{
#if UNITY_EDITOR
    //public GameObject Prefab = null;
    public string PrefabPath = null;
    const string RootPath = "Assets/Resources/";


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    public static void Make(GameObject obj)
    {
        if (obj != null)
        {
            if (PrefabUtility.GetPrefabType(obj) == PrefabType.Prefab)
            {
                PrefabTracer pt = obj.GetComponent<PrefabTracer>();
                if (obj.GetComponent<PrefabTracer>() == null)
                {
                    pt = obj.AddComponent<PrefabTracer>();
                    
                }
                if (pt!= null)
                {
                    pt.PrefabPath = AssetDatabase.GetAssetPath(obj);
                    pt.PrefabPath = pt.PrefabPath.Remove(0, RootPath.Length);

                    LogMgr.Log( obj + " make prefab tracer successful. ");
                }
            }
            else
            {
                LogMgr.Log(obj + " is not a prefab, it is a " + PrefabUtility.GetPrefabType(obj).ToString());
            }
        }
    }

    public static void Refresh(GameObject obj, bool isSearchingChildren)
    {
        if (obj != null  )
        {

            GameObject realObj = obj;

            PrefabTracer pt = obj.GetComponent<PrefabTracer>();
            if (pt != null )
            {
               // PrefabTracer go = ;
                
                //UnityEngine.Object parentObject = EditorUtility.GetPrefabParent(go);
                GameObject org = (GameObject)AssetDatabase.LoadAssetAtPath(RootPath + pt.PrefabPath, typeof(GameObject));
                GameObject go = PrefabUtility.InstantiatePrefab(org) as GameObject;

                //GameObject go = PrefabUtility.InstantiatePrefab(pt.Prefab) as GameObject;

                //PrefabType type = (PrefabUtility.GetPrefabType(pt.Prefab));
                if (go != null)
                {
                    PrefabTracer newpt = go.GetComponent<PrefabTracer>();
                    if (newpt == null)
                    {
                        newpt = go.AddComponent<PrefabTracer>();
                    }
                    newpt.transform.parent = pt.transform.parent;
                    newpt.transform.localPosition = pt.transform.localPosition;
                    newpt.transform.localScale = pt.transform.localScale;
                    newpt.transform.localRotation = pt.transform.localRotation;
                    newpt.PrefabPath = pt.PrefabPath;
                    newpt.name = pt.name  ;

                    //支持对NGUI的sprite复制
                    UISprite oldSprite = obj.GetComponent<UISprite>();
                    if (oldSprite != null)
                    {
                        UISprite newSprite = go.GetComponent<UISprite>();
                        if (newSprite != null)
                        {
                            newSprite.depth = oldSprite.depth;
                            newSprite.SetDimensions(oldSprite.width, oldSprite.height);
                        }
                    }
                    LogMgr.Log(newpt.name);

                    DestroyImmediate(obj);
                    //根节点已经发生改变
                    realObj = go;

                }
            }

           

            if (isSearchingChildren)
            {
                
                //因为只在编辑器中运行，所以可以用foreach
                for (int i = realObj.transform.childCount - 1; i >= 0 ; --i)
                {
                    Transform tr = realObj.transform.GetChild(i);
                    if (tr != null)
                    {
                        LogMgr.Log(tr.name);
                        Refresh(tr.gameObject, isSearchingChildren);
                    }
                }
            }
        }
    }

#endif
}

