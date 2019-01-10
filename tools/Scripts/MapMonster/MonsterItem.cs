using UnityEngine;
using SG;
using System;
using System.Collections.Generic;
using XLua;

[Hotfix]
public class MonsterItem : MonoBehaviour
{

    Action back;
    public MonsterInfo info;
    public GameObject area;
    public GameObject parentTranObj;
    public GameObject pointObj;   //路点obj
    public TextMesh pointText;  //路点文字
    private PointMapArea pointDraw; //路点关联绘制

   [HideInInspector]
    public GameObject obj;

    public List<GameObject> childModels = new List<GameObject>();

    public GameObject modelPrefab;
    public void RefreshData(MonsterInfo _info, Action callback = null)
    {
        SetV(_info);
        Transform tt = this.gameObject.transform;
        tt.position = info.ve3;
        tt.eulerAngles = new Vector3(0, info.eulerAnglesY, 0);
        if (_info.enumType != MonsterType.point)   //路点类型
        {
            CreatModel();
        }
        else
        {
            pointObj.SetActive(true);   //路点抽象， 没有配置表可读，临时找个模型 ，供策划编辑地图使用
            pointText.text = _info.index.ToString();
            DrawRalationLine();
        }
    }

    public void DrawRalationLine()
    {
#if UNITY_EDITOR
        if (pointDraw == null)
        {
            pointDraw = pointText.GetComponent<PointMapArea>();
        }
        pointDraw.ClearAll();
        foreach (var item in info.pointRalations)
        {
            Vector3 vv = MonsterLoad.instance.GetMemberPositon(MonsterType.point, item);
            if (vv != Vector3.zero)
            {
                pointDraw.SetArea(item, vv);
            }
        }
#endif
    }
    /*
    public void RemoveRalationLine()
    {
        if (pointDraw == null)
        {
            pointDraw = pointObj.GetComponent<PointMapArea>();
        }

    }
    */
    public void SetV(MonsterInfo _info)
    {    
        info = _info;
        obj = this.gameObject;
        info.itemObj = obj;
        area.transform.localScale = new Vector3(info.r * 2, 0, info.r * 2);
        area.SetActive(info.bShowArea);
        //       EnemyLoad();
    }
    List<Vector3> allPos = new List<Vector3>();

    public void ChangeModelShow()
    {
        CreatModel();
    }

    void CreatModel()
    {
        if (modelPrefab.transform.childCount > 0)
        {
            foreach (Transform item in modelPrefab.transform)
            {
                if (item != null)
                {
                    DestroyImmediate(item.gameObject);
                }
            }
        }
        string modelSkl = "";
        if (info.enumType == MonsterType.birth)
        {
            modelSkl = "Effect/scence/cs_zc_02/cs_zc_03chuansongmen";
        }
        else
        {
            if (Application.isPlaying)
            {
                LuaTable cfg = ConfigManager.Instance.Common.GetModelConfig(info.modelId);
                if (cfg != null)
                {
                    modelSkl = cfg.Get<string>("skl");
                }
            }
            else
            {
#if UNITY_EDITOR
                MonsterLoad.CacheModelSkn.TryGetValue(info.modelId, out modelSkl);
#endif
            }
        }
        if (string.IsNullOrEmpty(modelSkl))
        {
            return;
        }
        GameObject p = (GameObject)UnityEngine.Object.Instantiate(CoreEntry.gResLoader.Load(modelSkl));
        p.transform.parent = modelPrefab.transform;
        p.transform.localPosition = Vector3.zero;
        p.transform.localEulerAngles = Vector3.zero;
    }

    private void EnemyLoad()
    {
        //if (modelPrefab == null)
        //{
        //    modelPrefab = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("xx/xxx"));
        //    modelPrefab.transform.position = Vector3.zero;
        //}
        if (childModels.Count < info.count)
        {
            for (int i = 0; i < info.count; i++)
            {
                if (childModels.Count < info.count)
                {
                    //Transform tt = parentTranObj.transform.GetChild(i);
                    //if(tt != null)
                    //{
                    //    tt.gameObject.SetActive(true);
                    //    childModels.Add(tt.gameObject);
                    //}else
                    //{
                    //    GameObject t = Instantiate(modelPrefab);
                    //    t.transform.parent = parentTranObj.transform;
                    //    t.transform.position = Vector3.zero;
                    //    t.name = i.ToString();
                    //    childModels.Add(t);
                    //}

                    GameObject t = Instantiate(modelPrefab);
                    t.transform.parent = parentTranObj.transform;
                    t.transform.position = Vector3.zero;
                    t.name = i.ToString();
                    childModels.Add(t);
                }
            }
        }
        int max = childModels.Count;
        for (int i = info.count + 1; i <= max; i++)
        {
            childModels.RemoveAt(i);
            Transform tt = parentTranObj.transform.GetChild(i);
            if (tt != null)
            {
                tt.gameObject.SetActive(false);
            }
        }

        //        SetEnemyPos();

    }
    private void SetEnemyPos()
    {
        if (allPos.Count < childModels.Count)
        {
            for (int i = 0; i < 100; i++)
            {
                if (allPos.Count >= childModels.Count)
                {
                    break;
                }
                System.Random rnd = new System.Random();
                int _x = rnd.Next(Convert.ToInt32(info.ve3.x - info.r), Convert.ToInt32(info.ve3.x + info.r));
                int _z = rnd.Next(Convert.ToInt32(info.ve3.z - info.r), Convert.ToInt32(info.ve3.z + info.r));
                Vector3 pos = new Vector3(_x, info.ve3.y, _z);
                if (!allPos.Contains(pos))
                {
                    allPos.Add(pos);
                }
            }
        }
        for (int i = 0; i < childModels.Count; i++)
        {
            childModels[i].transform.position = allPos[i];
        }
    }

}
