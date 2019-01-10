using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace SG
{
    //能量恢复组件
[Hotfix]
    public class PowerObject : MonoBehaviour
    {
    //    private int m_powerRecoverID = 0;
    //    private ActorObj m_ownerObject = null;
    //    private int m_maxMP = 0;
    //    private int m_initMP = 0;
    //    private int m_MP = 0;

    //    gamedb.PowerRecoverDesc m_powerRecoverDesc = null;


    //    public int maxMP
    //    {
    //        get { return m_maxMP; }
    //    }

    //    public int initMP
    //    {
    //        get { return m_initMP; }
    //    }

    //    public int curMP
    //    {
    //        get { return m_MP; }
    //    }


    //    void Awake()
    //    {
    //        //do nothing
    //    }

    //    void Start()
    //    {
    //        //do nothing
    //    }

    //    public void Init(int PowerRecoverID, ActorObj onwerObject, int maxMP, int initMP)
    //    {
    //        m_powerRecoverID = PowerRecoverID;
    //        m_ownerObject = onwerObject;
    //        m_maxMP = maxMP;
    //        m_initMP = initMP;
    //        m_MP = initMP;

            
    //        if (m_powerRecoverID > 0)
    //        {
    //            m_powerRecoverDesc = Res_FileDB.g_resFileDB.GetPowerReoverDescByID(m_powerRecoverID - 1);
    //        }
    //        else if (m_powerRecoverID == 0)
    //        {
    //            m_powerRecoverDesc = Res_FileDB.g_resFileDB.GetPowerReoverDescByID(m_powerRecoverID);
    //        }

    //        if (m_powerRecoverDesc != null)
    //        {
    //            InvokeRepeating("AutoRecover", 1, m_powerRecoverDesc.iAutoRecoverInterval);
    //        }

    //    }

    //    //自动恢复
    //    void AutoRecover()
    //    {
    //        if (m_powerRecoverDesc != null)
    //        {
    //            if (m_ownerObject)
    //            {
    //                if (m_MP < maxMP)
    //                {
    //                    m_MP += m_powerRecoverDesc.iAutoRecoverNum;
    //                    if (m_MP > maxMP)
    //                    {
    //                        m_MP = maxMP;
    //                    }

    //                    m_ownerObject.MagicChange(m_powerRecoverDesc.iAutoRecoverNum);
    //                }
    //            }
    //        }
    //    }

    //    public void MPChange(int MPValue)
    //    {
    //        if (m_ownerObject)
    //        {
    //            m_ownerObject.MagicChange(MPValue);
    //            m_MP += MPValue;
    //        }
    //    }

    //    public bool CanCastSkill(int SkillID)
    //    {
    //        gamedb.SkillDesc skillDesc = Res_FileDB.g_resFileDB.GetSkillDescByID(SkillID);   
    //        if (skillDesc != null)
    //        {
    //            if (m_MP < skillDesc.iPowerConsume && skillDesc.iPowerConsume != 0)
    //            {
    //                return false;
    //            }
    //        }
    //        return true;
    //    }

    //    public void ReInit()
    //    {
    //        m_MP = m_initMP;
    //    }


    };
};  //end SG

