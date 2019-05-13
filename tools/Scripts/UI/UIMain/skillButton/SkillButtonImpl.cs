/**
* @file     : SkillButtonImpl.cs
* @brief    : 
* @details  :  使用技能按钮UI 
* @author   : 
* @date     : 
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using XLua;

[CSharpCallLua]
public delegate float GetHeroChangeTime();

enum enSkillIndex
{
    Player_att =1,
    Player_Skill1 = 2,
    Player_Skill2 = 3,
    Player_Skill3 = 4,
    Player_Skill4 = 5,
    Player_Skill_Beauty = 6,
    Player_Skill_WangzheVip = 907,
    Beauty_att = 1001,
    Beauty_skill1 = 1001,
    Beauty_skill2 = 1002,
    Beauty_skill3 = 1003,
    Beauty_skill4 = 1004,
    Beauty_skill5 = 1005,

}
namespace SG
{

[Hotfix]
    public class SkillButtonImpl : MonoBehaviour
    {
        public void OnPressDown(GameObject obj)
        {
            OnPress(true);
            //Debug.Log("index: " + buttonindex + " true");
        }
        public void OnPressUp(GameObject obj)
        {
            OnPress(false);
            //Debug.Log("index: " + buttonindex + " false");
        }
        void Awake()
        {
            m_TextCDCountDown = transform.FindChild("Label_CD").GetComponent<Text>();
            m_ImageDisable = transform.FindChild("Image_Disable").GetComponent<Image>();
            m_ImageSkillIcon = transform.FindChild("Image_SkillIcon").GetComponent<Image>();
            m_ImageCD = transform.FindChild("Image_Cd").GetComponent<Image>();
            BeautyEffect = transform.FindChild("CellEffect");
            if (BeautyEffect != null)
            {
                BeautyEffect.gameObject.SetActive(false);
            }

            wangzhevipEffect = transform.FindChild("uf_dazhaotixing");
            if (wangzhevipEffect != null)
            {
                wangzhevipEffect.gameObject.SetActive(false);
            }

            CooolEffect = transform.FindChild("CooolEffect");
            if (CooolEffect != null)
                CooolEffect.gameObject.SetActive(false);

            if (EnergyBall != null)
            {
                EnergyBall.gameObject.SetActive(false);
                //EnergyBallMaterial = EnergyBall.GetComponentInChildren<Renderer>();
            }

            uGUI.UIEventListener.Get(this.gameObject).onPointerClick = OnClick;
            uGUI.UIEventListener.Get(this.gameObject).onDragOut = OnDragOut;
            uGUI.UIEventListener.Get(this.gameObject).onDragBegin = onDragBegin;

            uGUI.UIEventListener.Get(this.gameObject).onDrag = OnDrag;
            uGUI.UIEventListener.Get(this.gameObject).onPointerDown = OnPressDown;
            uGUI.UIEventListener.Get(this.gameObject).onPointerUp = OnPressUp;

            if (m_ImageDisable != null && buttonindex >= (int)enSkillIndex.Player_Skill1 && buttonindex <= (int)enSkillIndex.Player_Skill4)
                m_ImageDisable.gameObject.SetActive(true);
            //m_eventMgr = CoreEntry.gEventMgr;
            if(buttonindex == (int)enSkillIndex.Player_Skill_Beauty)
            {
                LuaTable t = ConfigManager.Instance.Common.GetLevelOpen((int)FunctionType.HongYan);
                if(t != null)
                {
                    BeautyOpenLevel = t.Get<int>("open_level");
                }
            }
            //注册事件
            RegisterEvent();
        }

        public void ResetInfo()
        {
            defaultSkillID = 0;
            skillID = 0;
            float f = m_ImageCD.fillAmount;
            if (!CoreEntry.gMorphMgr.IsMorphing && BeautyEffect != null && BeautyEffect.gameObject.activeSelf)
            {
                BeautyEffect.gameObject.SetActive(false);
            }
            if (CooolEffect != null)
                CooolEffect.gameObject.SetActive(false);
            if (EnergyBall != null)
                EnergyBall.gameObject.SetActive(false);
            if (wangzhevipEffect != null)
                wangzhevipEffect.gameObject.SetActive(false);

            m_ImageDisable.gameObject.SetActive(true);
        }
        void Start()
        {
            if (buttonindex != (int)enSkillIndex.Player_Skill_Beauty)
            {
                if (m_ImageCD != null)
                {
                    m_ImageCD.enabled = false;
                }

                if (m_TextCDCountDown != null)
                {
                    m_TextCDCountDown.enabled = false;
                }
            }
        }
        void OnEnable()
        {
            //m_isClick = false;
            m_isPressed = false;
            m_isDrag = false;
            bCDCool = false;
            bPublicCDEnable = true;
            isDisable = false;
        }
        void OnDisable()
        {
        }

        void OnDestroy()
        {
            UnRegisterEvent();
        }

        public void OnClick(GameObject obj)
        {
            if (buttonindex == (int)enSkillIndex.Player_Skill_Beauty && PlayerData.Instance.BaseAttr.Level < BeautyOpenLevel)
            {
                //UITips.ShowTips(string.Format("魔神变身{0}级开放", BeautyOpenLevel));
                UITips.ShowUnHeroModelShow();
                return;
            }
            if (buttonindex == (int)enSkillIndex.Player_Skill_Beauty)
            {
                if (m_ImageDisable.gameObject.activeSelf)
                {
                     //   UITips.ShowTips(string.Format("您未获得魔神"));
                        UITips.ShowUnHeroModelShow();
                    return;
                }
                else
                {
                    if(guidewidget != null)
                    {
                        guidewidget.OnButtonClick();
                    }
                    if (m_ImageCD.fillAmount < 1)
                    {
                        string strTips = Localization.Get("技能冷却中");
                        if(strTips.Length > 0)
                        {
                            UITips.ShowTips(strTips);
                        }
                    }
                }
            }

            if (m_ImageDisable.gameObject.activeSelf)
            {
                if (buttonindex == (int)enSkillIndex.Player_Skill_WangzheVip)
                {
                    MainPanelMgr.Instance.ShowDialog("UIBaiYinVIP");
                    return;
                }

                UITips.ShowTips(string.Format("该技能尚未解锁"));
                return;
            }


            if (skillID == 0)
                return;
            if (CoreEntry.gActorMgr.MainPlayer != null)
            {
                if (buttonindex == (int)enSkillIndex.Player_att || buttonindex == (int)enSkillIndex.Beauty_att)
                {
                    CoreEntry.gActorMgr.MainPlayer.ReleaseCanNotBeControlledByInputFromNormalAttack();
                }
            }
            LuaTable skill = ConfigManager.Instance.Skill.GetSkillConfig(skillID);
            if (skill == null)
            {
                return;
            }

            if (SkillBase.GetSkillVersion(CoreEntry.gActorMgr.MainPlayer, skill) == 2 && CoreEntry.gActorMgr.MainPlayer.curActorState != ACTOR_STATE.AS_AIMING)
            {
                return;
            }

            SDKMgr.Instance.TrackGameOptLog(1, 500 + buttonindex);

            m_isDrag = false;
            //m_isClick = false;   // 按住不能放技能
            m_sendRatio = 0.05f;
            if (buttonindex == (int)enSkillIndex.Player_Skill_Beauty)
                LogMgr.UnityWarning("Player_Skill_Beauty:" + skillID);
            ModuleServer.MS.GSkillCastMgr.CastSkill(skillID);


        }
        public void OnDragOut(GameObject obj)
        {
            m_isDrag = false;
        }
        public void OnDrag(GameObject obj, PointerEventData data)
        {
        }
        public void onDragBegin(GameObject obj)
        {
            m_isDrag = true;
        }

        public void OnPress(bool isPressed)
        {
            BtnPressedTween(isPressed);
            if (isPressed == false)
            {
                if (m_isPressed && m_isDrag)
                 OnClick(this.gameObject);
                m_isPressed = false;
                //m_isClick = false;
                m_isDrag = false;
            }

            if (isPressed)
            {
                m_isPressed = true;
                m_isDrag = true;
                //m_sendRatioShowScope = 0.2f;

                if (CoreEntry.gActorMgr.MainPlayer != null)
                {
                    //普攻时不能移动
                    PreProcessWhenNormalAttack(CoreEntry.gActorMgr.MainPlayer);
                    if ((ModuleServer.MS.GSkillCastMgr.InCDCoolState(skillID) == false) && skillDesc != null && SkillBase.GetSkillVersion(CoreEntry.gActorMgr.MainPlayer, skillDesc) == 2)
                    {
                        StateParameter param = new StateParameter();
                        param.state = ACTOR_STATE.AS_AIMING;
                        param.skillID = skillID;
                        CoreEntry.gActorMgr.MainPlayer.RequestChangeState(param);
                    }
                }
            }
        }

        public void BtnPressedTween(bool isPressed)
        {
            if (m_ImageDisable.gameObject.activeSelf) return;
            if (isPressed)
            {
                this.transform.localScale = new Vector3(0.95f, 0.95f, 1.0f);
            }
            else
            {
                this.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
        }

        //////////////////////////////////////////////////////////////////////////

        public Text m_TextCDCountDown;
        public Image m_ImageDisable;
        public Image m_ImageSkillIcon;
        public Image m_ImageCD;
        private int m_LastShowCD = -1;          //上一帧显示的CD，防止Update内不断创建字符串导致GC

        private LuaTable skillDesc = null;

        public int skillID = 0;

        public bool isDisable = false;
        public float publicCD = 2.0f;
        public bool bPublicCDEnable = true;
        private bool bCDCool = false;
        public float totalTime = 5.0f;
        private float fBeginTime = 0;
        private float fDurTime = 0;

        private bool m_isPressed = false;
        private bool m_isDrag = false;
        private float m_sendRatio = 0.1f;

        public int buttonindex = 0;
        public int defaultSkillID = 0;

        public int m_nPlayerID = -1;

        //幻灵变身技能
        public float BeautyChangeTime = 30;
        public Transform BeautyEffect;
        public Transform CooolEffect = null;
        public Transform EnergyBall = null;
        public Renderer EnergyBallMaterial = null;
        public int BeautyOpenLevel = 0;
        public GuideWidget guidewidget = null;
        public Transform wangzhevipEffect;
        string GetSkillIcon(int iSkillID)
        {
            string skillIcon = null;

            LuaTable skilldesc = ConfigManager.Instance.Skill.GetSkillConfig(iSkillID);
            if (skilldesc != null)
            {
                skillIcon = skilldesc.Get<string>("icon");
            }

            return skillIcon;
        }

        void RegisterEvent()
        {
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_EU_ONSETSKILLID, OnEvent);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_EU_ONSKILLCOOLDOWN, OnEvent);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_EU_ONCANCELSKILLCOOL, OnEvent);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_EU_ONPRESS_SKILLBUTTON, OnEvent);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_EU_ONPROCESSING_SKILLBUTTON, OnEvent);
        }

        //删除注册事件，战斗内指引会用到 add by lzp
        public void UnRegisterEvent()
        {
            CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_EU_ONSETSKILLID, OnEvent);
            CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_EU_ONSKILLCOOLDOWN, OnEvent);
            CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_EU_ONCANCELSKILLCOOL, OnEvent);
            CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_EU_ONPRESS_SKILLBUTTON, OnEvent);
            CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_EU_ONPROCESSING_SKILLBUTTON, OnEvent);
        }

        void UpdateSkillIcon()
        {

            if ((int)enSkillIndex.Player_Skill_Beauty == buttonindex)
            {
                //Configs.changeConfig changCfg = CSVConfigManager.GetchangeConfig(skillID);
                LuaTable changeCfg = ConfigManager.Instance.Actor.GetChangeConfig(skillID);
                if (changeCfg != null)
                {
                    BeautyChangeTime = changeCfg.Get<float>("time");
                }
                GetHeroChangeTime fun =  LuaMgr.Instance.GetLuaEnv().Global.GetInPath<GetHeroChangeTime>("ModelManager.HeroModel.GetHeroChangeTime");
                if(fun != null)
                {
                    BeautyChangeTime = BeautyChangeTime + fun();
                }
            }
            string sIconName = GetSkillIcon(skillID);
            if (sIconName != null)
            {
                m_ImageSkillIcon.sprite = AtlasSpriteManager.Instance.GetSprite(sIconName);
                m_ImageSkillIcon.enabled = true;
            }
            else
            {
                LogMgr.LogError("技能没找到图标  skillID:" + skillID);
                //m_ImageDisable.gameObject.SetActive(true);
                m_ImageSkillIcon.gameObject.SetActive(false);
            }


            skillDesc = ConfigManager.Instance.Skill.GetSkillConfig(skillID);

            if (m_ImageDisable != null && (buttonindex <= (int)enSkillIndex.Beauty_att || buttonindex > (int)enSkillIndex.Player_Skill4))
            {
                m_ImageDisable.gameObject.SetActive(false);
                if (EnergyBall != null)
                    EnergyBall.gameObject.SetActive(true);

            }



            //重新更新技能CD时间
            if (CoreEntry.gActorMgr.MainPlayer.IsInCoolDownTime(skillID) == true) //
            {
                if (m_ImageCD != null)
                {
                    m_ImageCD.enabled = true;
                }
                if (m_TextCDCountDown != null)
                {
                    m_TextCDCountDown.enabled = true;
                }
            }
            else
            {
                fBeginTime = 0;
                fDurTime = 0;
                totalTime = 0.0f;
                bCDCool = false;

                m_ImageCD.fillAmount = 1f;
                if (EnergyBallMaterial != null)
                {
                    EnergyBallMaterial.material.SetFloat("_offect", 1);
                }

                if (buttonindex != (int)enSkillIndex.Player_Skill_Beauty)
                {
                    m_ImageCD.enabled = false;
                    m_TextCDCountDown.enabled = false;
                }

            }
             

        }
 
        void LateUpdate()
        {
            if (skillID == 0 || CoreEntry.gActorMgr.MainPlayer == null)
            {
                return;
            }
            //if (ShareCD == 0 && CD == 0 && m_TextCDCountDown.enabled == false) return;
            float ShareCD = ModuleServer.MS.GSkillCastMgr.QueryShareCDOverTime(skillID);
            float CD = ModuleServer.MS.GSkillCastMgr.QuerySkillCDOverTime(skillID);
            if (CD > ShareCD)
            {
                totalTime = ModuleServer.MS.GSkillCastMgr.GetSkillCDTime(skillID) / 1000;
                fBeginTime = ModuleServer.MS.GSkillCastMgr.GetSkillCDBegin(skillID);
            }
            else
            {
                totalTime = ModuleServer.MS.GSkillCastMgr.GetShareSkillCDTime(skillID) / 1000;
                fBeginTime = ModuleServer.MS.GSkillCastMgr.GetShareSkillCDBegin(skillID);
            }
            
            UpdateCD();            
            
            if ((ModuleServer.MS.GSkillCastMgr.InCDCoolState(skillID) == false) && m_isPressed && m_isDrag) // 普攻需要按住连续释放
            {
                m_sendRatio -= Time.deltaTime;
                if (m_sendRatio <= 0)
                {
                    m_sendRatio = 0.1f;

                    //如果硬直状态


                    LuaTable skill = ConfigManager.Instance.Skill.GetSkillConfig(skillID);

                    if (CoreEntry.gActorMgr.MainPlayer.IsInEndure())
                    {
                        return;
                    }

                    PreProcessWhenNormalAttack(CoreEntry.gActorMgr.MainPlayer);
                    //版本2的技能是手动指向型技能，不能自动释放
                    if (SkillBase.GetSkillVersion(CoreEntry.gActorMgr.MainPlayer, skill) != 2)
                    {
                        if (m_ImageDisable.gameObject.activeSelf == false)
                        {
                            ModuleServer.MS.GSkillCastMgr.CastSkill(skillID);
                        }
                    }
                }
            }
        }

        void UpdateCD()
        {
            fDurTime = Time.time - fBeginTime;
            if (fDurTime < totalTime)
            {
                bCDCool = true;
                if (m_ImageCD != null)
                {
                    if (m_TextCDCountDown.enabled == false)
                    {
                        m_TextCDCountDown.enabled = true;
                        m_ImageCD.enabled = true;
                        if (buttonindex == (int)enSkillIndex.Player_Skill_WangzheVip && wangzhevipEffect != null && wangzhevipEffect.gameObject.activeSelf)
                        {
                            wangzhevipEffect.gameObject.SetActive(false);
                        }
                    }

                    //幻灵变身技能
                    if (buttonindex != (int)enSkillIndex.Player_Skill_Beauty)
                    {
                        float f = m_ImageCD.fillAmount;

                        f = (totalTime - fDurTime) / totalTime;
                        
                        if ((m_ImageCD.fillAmount - f > 0 ? m_ImageCD.fillAmount - f : f - m_ImageCD.fillAmount) > 0.01f)
                        {
                            m_ImageCD.fillAmount = f;
                        }
                        //m_ImageCD.fillAmount = (totalTime - fDurTime) / totalTime;
                        int cdCountdown = (Mathf.RoundToInt(totalTime - fDurTime));
                        if (m_LastShowCD != cdCountdown)
                        {
                            m_LastShowCD = cdCountdown;
                            m_TextCDCountDown.text = cdCountdown.ToString();
                        }
                        if (buttonindex == (int)enSkillIndex.Player_Skill_WangzheVip && wangzhevipEffect != null && wangzhevipEffect.gameObject.activeSelf)
                        {
                            wangzhevipEffect.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        m_TextCDCountDown.enabled = false;
                        float f = m_ImageCD.fillAmount;
                        if (CoreEntry.gMorphMgr.IsMorphing)
                        {
                            f = (BeautyChangeTime - fDurTime) / BeautyChangeTime;                         
                        }
                        else
                        {
                            f = (fDurTime - BeautyChangeTime) / (totalTime - BeautyChangeTime);

                            if (BeautyEffect != null && BeautyEffect.gameObject.activeSelf)
                            {
                                BeautyEffect.gameObject.SetActive(false);
                            }
              
                            int CDTime = (Mathf.RoundToInt(totalTime - fDurTime));
                            if (CDTime > 0)
                            {
                                m_TextCDCountDown.enabled = true;
                                m_TextCDCountDown.text = CDTime.ToString();                                

                            }
                            else
                            {
                                m_TextCDCountDown.enabled = false;
                                m_TextCDCountDown.text = "";
                              
                            }
                            
                        }
                        if ((m_ImageCD.fillAmount - f > 0 ? m_ImageCD.fillAmount - f : f - m_ImageCD.fillAmount) > 0.01f)
                        {
                            if (EnergyBallMaterial != null)
                            {
                                EnergyBallMaterial.material.SetFloat("_offect", f);
                            }

                            m_ImageCD.fillAmount = f;
                        }
                    }
                }
            }
            else
            {
                if (bCDCool)
                {
                    if (CooolEffect != null)
                    {
                        CooolEffect.gameObject.SetActive(true);
                        Invoke("HideCoolEffect", 0.5f);
                    }
                }
                bCDCool = false;
                if (buttonindex == (int)enSkillIndex.Player_Skill_WangzheVip && wangzhevipEffect != null && !wangzhevipEffect.gameObject.activeSelf)
                {
                    wangzhevipEffect.gameObject.SetActive(true);
                }
                if (m_ImageCD != null && m_TextCDCountDown.enabled == true)
                {
                    //幻灵变身技能
                    if (buttonindex != (int)enSkillIndex.Player_Skill_Beauty)
                    {
                       if (m_ImageCD.fillAmount != 1)
                        {
                            m_ImageCD.fillAmount = 1f;
                        }
                        m_ImageCD.enabled = false;
                        m_TextCDCountDown.enabled = false;
                         
                    }
                    else
                    {
                        m_TextCDCountDown.enabled = false;
                        if (m_ImageCD.fillAmount != 1)
                        {
                            m_ImageCD.fillAmount = 1f;
                            if (EnergyBallMaterial != null)
                            {
                                EnergyBallMaterial.material.SetFloat("_offect", 1);
                            }
                        }
                        if (BeautyEffect != null && !BeautyEffect.gameObject.activeSelf)
                        {
                            BeautyEffect.gameObject.SetActive(true);
                        }
                    }
                }
            }
        }

        void HideCoolEffect()
        {
            if (CooolEffect != null)
                CooolEffect.gameObject.SetActive(false);
        }
        void PreProcessWhenNormalAttack(ActorObj actorBase)
        {
            //普攻时不能移动
            if (buttonindex == (int)enSkillIndex.Player_att || buttonindex == (int)enSkillIndex.Beauty_att)
            {
                actorBase.ReqirueCanNotBeControlledByInputFromNormalAttack();
            }
        }

        void OnEvent(GameEvent ge, EventParameter param)
        {
            int index = param.intParameter;
            int iSkillId = param.intParameter1;
            int nPlayerID = param.intParameter2;
            switch (ge)
            {
                case GameEvent.GE_EU_ONSETSKILLID:
                    {
                        if (buttonindex == index)
                        {
                            defaultSkillID = iSkillId;
                            skillID = iSkillId;
                            m_nPlayerID = nPlayerID;
                            skillDesc = ConfigManager.Instance.Skill.GetSkillConfig(skillID);
                            UpdateSkillIcon();
                        }
                        break;
                    }
                case GameEvent.GE_EU_ONSKILLCOOLDOWN:
                    {
                        if (iSkillId == skillID && nPlayerID == m_nPlayerID)
                        {
                            OnSkillCoolDown(nPlayerID);
                        }
                        else
                        {
                            OnSkillCoolDown(nPlayerID, iSkillId);
                        }
                        break;
                    }
                case GameEvent.GE_EU_ONCANCELSKILLCOOL:
                    {
                        if (iSkillId == skillID && nPlayerID == m_nPlayerID)
                        {
                            OnCancleSkillCool();
                        }
                        else
                        {
                            // OnCancleSkillCool( iSkillId);
                        }
                        break;
                    }
                case GameEvent.GE_EU_ONPRESS_SKILLBUTTON:
                    {
                        if (ModuleServer.MS.GSkillCastMgr.IsPressButtonHandled() == false)
                        {
                            if (buttonindex == index)
                            {
                                ModuleServer.MS.GSkillCastMgr.SetPressButtonHandled(true);
                                ModuleServer.MS.GSkillCastMgr.CastSkill(skillID);
                            }
                        }
                        break;
                    }
                case GameEvent.GE_EU_ONPROCESSING_SKILLBUTTON:
                    {
                        object obj = param.objParameter;
                        bool pressed = false;
                        if (obj != null && obj is bool)
                        {
                            pressed = (bool)obj;
                        }

                        if (buttonindex == index)
                        {
                            OnPress(pressed);
                        }
                        break;
                    }
            }
        }

        private void OnSkillCoolDown(int nEntityID, int nSkillID)
        {
            if (m_nPlayerID <= 0)
            {
                return;
            }
			if (nSkillID == 0)
				return;
			

            LuaTable ShareSkillCfg = ConfigManager.Instance.Skill.GetSkillConfig(skillID);

			if (ShareSkillCfg == null)
				return;

            if (m_nPlayerID == nEntityID && ShareSkillCfg.Get<int>("group_cd_id") == skillDesc.Get<int>("group_cd_id"))
            {
                totalTime = ShareSkillCfg.Get<float>("group_cd");
                m_LastShowCD = (int)totalTime;
                m_TextCDCountDown.text = m_LastShowCD.ToString();

                if (m_ImageCD != null)
                {
                    m_ImageCD.enabled = true;
                }
                if (m_TextCDCountDown != null)
                {
                    m_TextCDCountDown.enabled = true;
                }
                fBeginTime = Time.time;
            }
        }

        private void OnSkillCoolDown(int nEntityID)
        {
            SkillCastMgr.SkillCDData cdData;
            if (ModuleServer.MS.GSkillCastMgr.GetSkillCDData(m_nPlayerID, skillID, out cdData))
            {
                //玩家释放
                if (CoreEntry.gActorMgr.MainPlayer.EntityID == nEntityID)
                {
                    totalTime = cdData.iCDCoolTime / 1000;
                    m_LastShowCD = (int)totalTime;
                    m_TextCDCountDown.text = m_LastShowCD.ToString();

                    if (m_ImageCD != null)
                    {
                        m_ImageCD.enabled = true;
                    }
                    if (m_TextCDCountDown != null)
                    {
                        m_TextCDCountDown.enabled = true;
                    }
                }
                
                fBeginTime = Time.time;
                cdData.fBeginTime = fBeginTime;
            }
        }
        private void OnCancleSkillCool()
        { 

            SkillCastMgr.SkillCDData cdData;
            if (ModuleServer.MS.GSkillCastMgr.GetSkillCDData(m_nPlayerID, skillID, out cdData))
            {
                //玩家释放              
                totalTime = 0;
                m_LastShowCD = (int)totalTime;
                m_TextCDCountDown.text = m_LastShowCD.ToString();

                if (m_ImageCD != null)
                {
                    m_ImageCD.enabled = false;
                }
                if (m_TextCDCountDown != null)
                {
                    m_TextCDCountDown.enabled = false;
                }
            }
        }
    }
}

