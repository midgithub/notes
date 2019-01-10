using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using XLua;

namespace SG
{

    [LuaCallCSharp]
[Hotfix]
    public class NpcCameraShow : MonoBehaviour
    {

        [SerializeField]
        GameObject modelParent;
        // Use this for initialization

        /// <summary>
        /// 显示对象预制件路径。
        /// </summary>
        public static string PrefabPath = "UI/Prefabs/NPC/NpcTkCamera";

        private static NpcCameraShow CurModeShow;

        public static void ShowModel(string path)
        {
            if (CurModeShow == null)
            {
                GameObject prefab = (GameObject)CoreEntry.gResLoader.Load(PrefabPath, typeof(GameObject));
                if (prefab == null)
                {
                    return;
                }
                GameObject obj = Instantiate(prefab) as GameObject;
                CurModeShow = obj.GetComponent<NpcCameraShow>();
                DontDestroyOnLoad(CurModeShow);
                CurModeShow.transform.localPosition = new Vector3(0, -1000, 0);
                CurModeShow.transform.localScale = Vector3.one;
            //   CurModeShow.transform.forward = Vector3.forward;
            }

            if (!CurModeShow.gameObject.activeSelf)
            {
                CurModeShow.gameObject.SetActive(true);
            }
            CurModeShow.Show(path);
        }

        public void Show(string path)
        {
            UiUtil.ClearAllChild(modelParent.transform);
            GameObject prefab = (GameObject)CoreEntry.gResLoader.Load(path, typeof(GameObject));
            if(prefab == null)return;
            GameObject obj = Instantiate(prefab) as GameObject;
            obj.transform.SetParent(modelParent.transform);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            // prefab.transform.forward = modelParent.transform.forward;
            // prefab.GetComponent<Animation>().Play("stand");
        }


        public static void DestroyNpcCamera()
        {
            if (CurModeShow == null)
            {
                return;
            }
            Destroy(CurModeShow.gameObject);
            CurModeShow = null;
        }
    }

}

