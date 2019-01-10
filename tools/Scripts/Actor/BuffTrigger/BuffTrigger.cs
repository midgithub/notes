using UnityEngine;
using System.Collections;
using SG;
using System.Collections.Generic;
using XLua;

[Hotfix]
public class BuffTrigger : Entity 
{
    List<int> MergerArray(int[] First, int[] Second)
    {
        List<int> re = new List<int>();
        int[] result = new int[First.Length + Second.Length];
        First.CopyTo(result, 0);
        Second.CopyTo(result, First.Length);

        for (int i = 0; i < result.Length; ++i)
        {
            if (result[i] > 0)
            {
                re.Add(result[i]);
            }
        }
        return re;
    }

    GameObject m_AnimationRoot;

    //string[] m_ModelPath = {
    //                           "Animation/monster/Buff/fuzi",
    //                        "Animation/monster/Buff/yifu",
    //                        "Animation/monster/Buff/xie"
    //                       };
    //List<int> m_BuffIdList;

    int m_CurrentBuffId = 0;
    bool m_Actived = false;

    //Configs.CreatureDesc CreatureDesc;
    LuaTable SkillBuffDesc;


    public override void Awake()
    {
        m_AnimationRoot = transform.FindChild("ModelRoot").FindChild("AnimationRoot").gameObject;
        BoxCollider collider = GetComponent<BoxCollider>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
    }

    void Start()
    {
        //CreatureDesc = CsvMgr.GetRecordCreatureDesc(m_resID);
        //if (CreatureDesc != null)
        //{
        //    m_BuffIdList = MergerArray(CreatureDesc.SpecialSkillDescSkillID, CreatureDesc.SpecialSkillDescRatio);
        //    ClearModel();
        //    if (m_BuffIdList.Count < 1)
        //    {
        //        Debug.LogError("Creature id " + m_resID + ":请在CreatureDesc表里至少填充一个buff id");
        //        return;
        //    }

        //    //如果有填开始时间，那么延迟刷新
        //    if (CreatureDesc.startTime > 0)
        //    {
        //        Invoke("RefreshBuff", CreatureDesc.startTime);
        //    }
        //    else
        //    {
        //        RefreshBuff();
        //    }
        //}

    }

    void ClearModel()
    {
        Transform myTrans = m_AnimationRoot.transform;
        BetterList<Transform> list = new BetterList<Transform>();

        for (int i = 0; i < myTrans.childCount; ++i)
        {
            Transform t = myTrans.GetChild(i);
            list.Add(t);
        }

        for (int i = 0; i < list.size; i++)
        {
            GameObject.Destroy(list[i].gameObject);
        }
    }

    void RefreshBuff()
    {
        ClearModel();

        //int index = Random.Range(0, m_BuffIdList.Count);
        //m_CurrentBuffId = m_BuffIdList[index];
        SkillBuffDesc = ConfigManager.Instance.Skill.GetBuffConfig(m_CurrentBuffId);
        if (SkillBuffDesc == null)
        {
            return;
        }
       
        string prefabPath = SkillBuffDesc.Get<string>("buff_param3");

        GameObject prefab = CoreEntry.gResLoader.Load(prefabPath) as GameObject;
        if (prefab == null)
        {
            LogMgr.LogError("skillBuff id " + m_CurrentBuffId + ":SkillBuff里的模型(特效)路径有问题");
            return;
        }
        GameObject model = (GameObject)Instantiate(prefab);
        if (model == null)
        {
            LogMgr.LogError("skillBuff id " + m_CurrentBuffId + ":实例化模型(特效)失败");
            return;
        }
        model.transform.parent = m_AnimationRoot.transform;
        model.transform.localPosition = Vector3.zero;
        model.transform.localScale = new Vector3(1, 1, 1);

        m_Actived = true;
    }


    void OnTriggerEnter(Collider other)
    {
        ActorObj actorBase = other.transform.root.gameObject.GetComponent<ActorObj>();
        if (actorBase == null)
        {
            actorBase = other.transform.gameObject.GetComponent<ActorObj>();

        }

        if (actorBase == null)
        {
            return;
        }

        if (!m_Actived)
        {
            return;
        }

        if (actorBase.mActorType == ActorType.AT_LOCAL_PLAYER || actorBase.mActorType == ActorType.AT_PVP_PLAYER || actorBase.mActorType == ActorType.AT_REMOTE_PLAYER)
        {
           // ClearModel();
           // m_Actived = false;

           // //增加BUFF的逻辑
           // actorBase.Addbuff(m_CurrentBuffId, actorBase, 0);

           //// 如果有填cd时间，按这个时间刷新，否则彻底删除
           // if (CreatureDesc.repeatTime > 0)
           // {
           //     Invoke("RefreshBuff", CreatureDesc.repeatTime);
           // }
           // else
           // {
           //     GameObject.Destroy(gameObject);
           // }

        }

    }

    void OnDisable()
    {
        CancelInvoke("RefreshBuff");
    }
}

