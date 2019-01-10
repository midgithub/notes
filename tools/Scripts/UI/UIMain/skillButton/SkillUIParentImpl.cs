/**
* @file     : SkillUIImpl.cs
* @brief    : 
* @details  :  使用技能UI 
* @author   : 
* @date     : 
*/

using UnityEngine;
using System.Collections;
using SG;
using DG.Tweening;
using System.Collections.Generic;
using XLua;

[Hotfix]
public class SkillUIParentImpl : MonoBehaviour
{ 
    public Transform SillUIHiti;
    public Transform SillUI;

    public SkillButtonImpl[] SkillButton;
    public SkillButtonImpl[] SkillHetiButton;
    public SkillButtonImpl SkillCell_Morph;
    public SkillButtonImpl SkillCell_Moshen;
    public SkillButtonImpl SkillCell_WangzheVip;
    public int offsetX = 400;
    public float antiTime = 0.3f;
    public float delayTime = 4; //技能开启显示延时


    // Use this for initialization
    void Start () {
	}


    void OnEnable()
    { 
        Invoke("delayInit", 0.001f);
        if (!CoreEntry.gMorphMgr.IsMorphing)
        {
            ((RectTransform)SillUI).anchoredPosition = new UnityEngine.Vector2(0, 0);
            ((RectTransform)SillUIHiti).anchoredPosition = new UnityEngine.Vector2(400, 0);
        }
        else
        {
            ((RectTransform)SillUIHiti).anchoredPosition = new UnityEngine.Vector2(0, 0);
            ((RectTransform)SillUI).anchoredPosition = new UnityEngine.Vector2(400, 0);
        }


        CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_CHANGE_BEGIN, GE_SC_CHANGE_BEGIN);
        CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_CHANGE_END, GE_SC_CHANGE_END);
        CoreEntry.gEventMgr.AddListener(GameEvent.GE_SKILL_UPGRADE, GE_SKILL_UPGRADE);
        CoreEntry.gEventMgr.AddListener(GameEvent.GE_PLAYER_LV, GE_PLAYER_LV);
        CoreEntry.gEventMgr.AddListener(GameEvent.GE_PLAYER_VIP, GE_PLAYER_VIP);
        CoreEntry.gEventMgr.AddListener(GameEvent.GE_SKILL_ADD, GE_SKILL_ADD);
        CoreEntry.gEventMgr.AddListener(GameEvent.GE_SKILL_REMOVE, GE_SKILL_REMOVE);



    }

    void OnDisable()
    {
        CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_SC_CHANGE_BEGIN, GE_SC_CHANGE_BEGIN);
        CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_SC_CHANGE_END, GE_SC_CHANGE_END);
        CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_SKILL_UPGRADE, GE_SKILL_UPGRADE);
        CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_PLAYER_LV, GE_PLAYER_LV);
        CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_PLAYER_VIP, GE_PLAYER_VIP);
        CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_SKILL_ADD, GE_SKILL_ADD);
        CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_SKILL_REMOVE, GE_SKILL_REMOVE);

    }

    public void GE_SC_CHANGE_BEGIN(GameEvent ge, EventParameter param)
    {
        if (CoreEntry.gActorMgr.MainPlayer == null) return;
        if (CoreEntry.gActorMgr.MainPlayer.ServerID != param.longParameter) return;

        if (!CoreEntry.gMorphMgr.IsMorphing)
        {
            ((RectTransform)SillUI).anchoredPosition = new UnityEngine.Vector2(0, 0);
            ((RectTransform)SillUIHiti).anchoredPosition = new UnityEngine.Vector2(400, 0);
        }
        else
        {
            ((RectTransform)SillUIHiti).anchoredPosition = new UnityEngine.Vector2(0, 0);
            ((RectTransform)SillUI).anchoredPosition = new UnityEngine.Vector2(400, 0);
        }
    }
    public void GE_SC_CHANGE_END(GameEvent ge, EventParameter param)
    {
        if (CoreEntry.gActorMgr.MainPlayer == null) return;
        if (CoreEntry.gActorMgr.MainPlayer.ServerID != param.longParameter) return;
        if (!CoreEntry.gMorphMgr.IsMorphing)
        {
            ((RectTransform)SillUI).anchoredPosition = new UnityEngine.Vector2(0, 0);
            ((RectTransform)SillUIHiti).anchoredPosition = new UnityEngine.Vector2(400, 0);
        }
        else
        {
            ((RectTransform)SillUIHiti).anchoredPosition = new UnityEngine.Vector2(0, 0);
            ((RectTransform)SillUI).anchoredPosition = new UnityEngine.Vector2(400, 0);
        }
    }

    public void GE_SKILL_UPGRADE(GameEvent ge, EventParameter param)
    {
        if (CoreEntry.gActorMgr.MainPlayer != null)
        {
            CoreEntry.gActorMgr.MainPlayer.GetComponent<PlayerObj>().ReBindSkill();
            CoreEntry.gActorMgr.MainPlayer.GetComponent<PlayerObj>().RebindSkillButton();
            ModuleServer.MS.GSkillCastMgr.UpdateSkillCD();
        }		
    }
    public void GE_PLAYER_LV(GameEvent ge, EventParameter param)
    {
        int oldLevel = param.intParameter;
        int level = PlayerData.Instance.BaseAttr.Level;
        List<int> skillLockLevel = GetSkillLockLevel();
        int skillIndex = -1;
        for (int i = 0;i < skillLockLevel.Count; i++)
        {
            if(skillLockLevel[i] <= level && skillLockLevel[i] > oldLevel)
            {
                skillIndex = i;
                break;
            }
        }
        if (skillIndex == -1) return;

        CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SHOW_BATTER, null);

        PanelBase panel =  MainPanelMgr.Instance.ShowDialog("UISkillOpenTips");
        XLua.LuaTable tbl = panel.GetEnvTable();

        ShowInfo setFunc = null;
        if (null != tbl)
        {
            tbl.Get("ShowInfo", out setFunc);
        }

        if (null != setFunc)
        {
            setFunc(skillIndex);
        }
        Invoke("DelayReBindLock", delayTime);
         
        //Debug.LogError("skillIndex");

    } 
    public void GE_PLAYER_VIP(GameEvent ge, EventParameter param)
    {
        SkillCell_WangzheVip.transform.SetRenderActive(PlayerData.Instance.BaseAttr.VIPLevel > 0);
    }
    public void GE_SKILL_REMOVE(GameEvent ge, EventParameter param)
    {
        LuaTable mapConfig = ConfigManager.Instance.Map.GetMapConfig(MainRole.Instance.mapid);
        if (mapConfig != null)
        {
            if(mapConfig.Get<int>("type") != 51)
            {
                return;
            }
        }

        delayInit();
    }
    public void GE_SKILL_ADD(GameEvent ge, EventParameter param)
    {
        delayInit();
    }
    void delayInit()
    {
        //魔神小岛
        LuaTable mapConfig = ConfigManager.Instance.Map.GetMapConfig(MainRole.Instance.mapid);
        if (mapConfig != null)
        {
            SkillCell_Moshen.gameObject.SetActive(mapConfig.Get<int>("type") == 51);
        }

        if (CoreEntry.gActorMgr.MainPlayer != null)
        {
            SkillCell_Morph.ResetInfo();
            SkillCell_Moshen.ResetInfo();
            SkillCell_WangzheVip.ResetInfo();
            SkillCell_WangzheVip.transform.SetRenderActive(PlayerData.Instance.BaseAttr.VIPLevel > 0);
            for (int i = 0; i < SkillHetiButton.Length; i++)
            {
                if (SkillHetiButton[i])
                {
                    SkillHetiButton[i].ResetInfo();
                }
            }

            CoreEntry.gActorMgr.MainPlayer.ReBindSkill();
            CoreEntry.gActorMgr.MainPlayer.RebindSkillButton();
            ModuleServer.MS.GSkillCastMgr.UpdateSkillCD();
            ReBindLock();
        }		
    }

    public List<int>GetSkillLockLevel()
    {
        LuaTable skillLimitCfg = ConfigManager.Instance.Skill.GetSkillLimitConfig(PlayerData.Instance.BaseAttr.Prof);
        
        List<int> skillLockLevel = new List<int>();
        if (skillLimitCfg == null)
        {
            for (int i = 0; i < 4; i++)
            {
                skillLockLevel.Add(1);
            }
        }
        else
        {
            string[] levelArryTmp = skillLimitCfg.Get<string>("limit_levels").Split('#');
            if(levelArryTmp.Length >= 8)
            {
                skillLockLevel.Clear();
                for(int i = 4;i < 8; i++)
                {
                    int tmplevel = 1;
                    if (int.TryParse(levelArryTmp[i], out tmplevel))
                    {
                        skillLockLevel.Add(tmplevel);
                    }
                }
            }
        }
        return skillLockLevel;
    }
    public void DelayReBindLock()
    {
        MainPanelMgr.Instance.HideDialog("UISkillOpenTips");
        ReBindLock();
    }
    public void ReBindLock()
    {
        List<int> skillLockLevel = GetSkillLockLevel();
        for(int i =0; i < SkillButton.Length; i++)
        {
            SkillButton[i].m_ImageDisable.gameObject.SetActive(PlayerData.Instance.BaseAttr.Level < skillLockLevel[i]);
        }
    }
}

