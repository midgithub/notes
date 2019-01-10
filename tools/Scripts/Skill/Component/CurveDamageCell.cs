/**
* @file     : CurveDamageCell.cs
* @brief    : 
* @details  : 曲线伤害元素
* @author   : 
* @date     : 2014-12-5
*/

using XLua;
using UnityEngine;
using System.Collections;

namespace SG
{

[Hotfix]
    public class CurveDamageCell : ISkillCell
    {
        SkillBase m_skillBase = null;
        CurveDamageCellDesc m_curveDamageCellDesc = null;
        Transform m_transform = null;
        int m_curIndex = 0;
        //Vector3 pos = Vector3.zero;

        //Transform m_parent = null;    

        void Awake()
        {
            m_transform = this.transform;
        }

        public override void Init(ISkillCellData cellData, SkillBase skillBase)
        {
            m_skillBase = skillBase;
            m_curveDamageCellDesc = (CurveDamageCellDesc)cellData;
            m_curIndex = 0;
        }

        // PoolManager
        void OnEnable()
        {
            CancelInvoke("Start");
            Invoke("Start", 0.000001f);
        }

        void OnDisable()
        {
            CancelInvoke("Start");
        }

        // Use this for initialization
        void Start()
        {
            CancelInvoke("Start");

            if (m_skillBase == null || m_skillBase.m_actor == null)
                return;

            //m_parent = m_transform.parent;

            ////伤害计算
            //m_skillBase = this.transform.root.gameObject.GetComponent<SkillBase>();

            //SkillClassDisplayDesc skillClass = CoreEntry.gGameDBMgr.GetSkillClassDisplayDesc(m_skillBase.m_skillID);	
            if (m_curveDamageCellDesc == null)
            {
                return;
            }

            if (m_curveDamageCellDesc.curveDamageInfoList.Count <= 0)
            {
                return;
            }

            m_transform.parent = null;

            //第一段开始
            float delayTime = m_curveDamageCellDesc.curveDamageInfoList[0].hitTime;
            m_curIndex = 0;

            Invoke("CalculateDamage", delayTime);

            //pos = m_transform.position; 
        }

        // Update is called once per frame
        void Update()
        {
            //Debug.DrawLine(pos, m_transform.position, Color.green);
        }

        void CalculateDamage()
        {
            CancelInvoke("CalculateDamage");

            OneCurveDamageInfo curveInfo = m_curveDamageCellDesc.curveDamageInfoList[m_curIndex];

            //旋转偏移        
            m_transform.Rotate(Vector3.up, curveInfo.offsetAngle * Mathf.Deg2Rad);

            //debug
#if UNITY_EDITOR
            GameObject efxObj = Instantiate(
                            CoreEntry.gResLoader.LoadResource("Skill/Cube")) as GameObject;
            efxObj.transform.position = m_transform.position;
            efxObj.transform.rotation = m_transform.rotation;
            efxObj.name = m_curIndex.ToString();

            SceneEfx efx = efxObj.AddComponent<SceneEfx>();
            efx.Init(m_transform.position, 5);
#endif

            //计算伤害monster
            System.Collections.Generic.List<ActorObj> actors = CoreEntry.gActorMgr.GetAllMonsterActors();
            foreach (ActorObj obj in actors)
            {
                //对IOS出现怪物不动 报错的异常  进行错误处理
                if (GameLogicMgr.checkValid(obj.gameObject) == false)
                {
                    continue;
                }
                ActorObj actorBase = obj;

                //临时判断
                if (actorBase.mActorType == m_skillBase.m_actor.mActorType)
                {
                    continue;
                }


                if (!CoreEntry.gGameMgr.IsPvpState() && !m_skillBase.m_actor.IsAimActorType(actorBase))
                {
                    continue;
                }

                bool isSkillSuccess = BaseTool.instance.IsPointInRectangleXZ(m_transform.position,
                    m_transform.rotation.eulerAngles, curveInfo.damageLength,
                    curveInfo.damageWidth, obj.transform.position);

                //伤害对象
                if (isSkillSuccess)
                {
                    //纠正被击表现
                    DamageParam damageParam = new DamageParam();
                    damageParam.skillID = m_skillBase.m_skillID;
                    damageParam.attackActor = m_skillBase.m_actor;
                    damageParam.behitActor = actorBase;

                    CoreEntry.gSkillMgr.OnSkillDamage(damageParam);
                }
            }

            //场景物件，主角破坏                
            if (m_skillBase.m_actor.mActorType == ActorType.AT_LOCAL_PLAYER)
            {
                GameObject[] brokedObjs = CoreEntry.gSceneMgr.brokedObjArray;
                for (int i = 0; i < brokedObjs.Length; ++i)
                {
                    bool isSkillSuccess = BaseTool.instance.IsPointInRectangleXZ(m_transform.position,
                    m_transform.rotation.eulerAngles, curveInfo.damageLength,
                    curveInfo.damageWidth, brokedObjs[i].transform.position);

                    //伤害对象
                    if (isSkillSuccess)
                    {
                        Broked broked = brokedObjs[i].GetComponent<Broked>();
                        int weight = 3;//m_skillBase.m_skillDesc.weight;

                        broked.DoBroked(m_skillBase.m_actor.thisGameObject, weight);
                    }
                }
            }

            if (m_curIndex < m_curveDamageCellDesc.curveDamageInfoList.Count - 1)
            {
                m_curIndex++;

                //设置位置           
                m_transform.position += m_transform.forward.normalized * curveInfo.length;

                Invoke("CalculateDamage", m_curveDamageCellDesc.curveDamageInfoList[m_curIndex].hitTime);
            }
        }

        //结束的时候，显示
        void OnDestroy()
        {

        }
    }
}

