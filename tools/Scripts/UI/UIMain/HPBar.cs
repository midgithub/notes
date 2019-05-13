/**
* @file     : HPBar.cs
* @brief    : 
* @details  : 文件用途说明
* @author   : 
* @date     : 
*/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using XLua;

namespace SG
{
    /// <summary>
    /// 血条。
    /// </summary>

    [CSharpCallLua]
    public delegate string GetHeroDesc(int id,int starlevel,string name,int qua,Text txt);
    [CSharpCallLua]
    public delegate string GetVipScColor(int vipLevel,long serverid);
    [CSharpCallLua]
    public delegate string GetNewLevel(int Level);

    

[Hotfix]
    public class HPBar : MonoBehaviour
    {
        public const string BIND_NAME = "E_top";                    //绑定节点名称

        public static string NormalRgb = "FFFFFF";
        public static string YellowRgb = "E6E09DFF";
        public static string GreenRgb = "00DE15";
        public static string GreyRgb = "ABABAB";
        public static string RedRgb = "F91515";
        public static string BlueRgb = "32CFF1";
        public static string OrangeRgb = "FF6100";
        public static string StrawberryRgb = "872657";

        public static string MonsterColor = "EEE048";
        public static string EliteColor = "EEE048";

        //物品品质特效
        public static readonly string[] QuaPfxs = {
            "",
            "",
            "UI/Prefabs/Bag/FirstRes/QualityEffect1",
            "UI/Prefabs/Bag/FirstRes/QualityEffect2",
            "UI/Prefabs/Bag/FirstRes/QualityEffect3",
            "UI/Prefabs/Bag/FirstRes/QualityEffect5",
            "UI/Prefabs/Bag/FirstRes/QualityEffect4",
            "UI/Prefabs/Bag/QualityEffect8",
            "",
            ""};

        public Image ValueImage;                                    //血量进度
        public Image MoveImage;                                     //用来播放进度动画
        public Text LordText;                                       //角色爵位(称号)名称
        public Text NameText;                                       //角色名称
        public Image PkLogo;                                        //PkLogo
        public Image ImageHeroTitle;                                //头顶称号
        public GameObject Owner;                                    //拥有血条的对象
        public Transform BindNode;                                  //绑定的节点
        public float AniTime = 0.2f;                                //血条动画时间(一管血动画时长)
        public float AniBaseTime = 0.3f;                            //血条动画基础时间(每次掉血都要加进去)

        public ActorType BarType;                                   //血条类型                
        public RectTransform SelfRT;

        protected float m_CurValue;                                 //当前血量比例    
        protected float m_MoveValue;                                //动画表现的比例
        protected float m_AniSpeed;                                 //动画播放速度

        private string lordName = "";
        private string VipName = "";
        private string playerName = "";
        private int playerLvl = 1;
        private bool self = false;

        void Awake()
        {
            SelfRT = transform as RectTransform;
        }

        public virtual void Init(GameObject owner, float value = 1, int num = 1, bool ani = false)
        {
            if (owner == null)
            {
                return;
            }
         
            Owner = owner;
            SetBindNode(Owner, BIND_NAME);
            SetValue(value, ani);

            ActorObj actor = Owner.GetComponent<ActorObj>();
            if (actor == null)     //血条测试时
            {
                return;
            }
            BaseAttr attr = actor.mBaseAttr;
           
            self = actor.mActorType == ActorType.AT_LOCAL_PLAYER;          //玩家自身不显示血条，但要显示不包含等级的名字
            
            if (ArenaMgr.Instance.IsArena)
            {
                self = true;
                if (actor.mActorType == ActorType.AT_PET)
                {
                    self = false;
                }
            }
            lordName = TitleMgr.Instance.GetCurLordText(attr.Lord, true);
            playerName = attr.Name;
            VipName = "";
            if(attr.VIPLevel > 0)
            {
                GetVipScColor fun = LuaMgr.Instance.GetLuaEnv().Global.GetInPath<GetVipScColor>("Common.GetVipScColor");
                if(fun!=null)
                {
                    VipName = fun(attr.VIPLevel, actor.ServerID);
                    //playerName = string.Format("{0} {1}", fun(attr.VIPLevel,actor.ServerID),attr.Name); 
                }
            }

            //Debug.LogError("pet.Starlevel: " + pet.Starlevel);
  
            playerLvl = attr.Level;

            if (null != ImageHeroTitle)
            {
                ImageHeroTitle.gameObject.SetActive(false);
            }

            if (self)
            {
                InitPKStatus(PlayerData.Instance.CurPKState);
               // SetSelfPkLogo(PlayerData.Instance.CurPKState);
                ValueImage.transform.parent.gameObject.SetActive(false);

                LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
                int id = G.GetInPath<int>("ModelManager.HeroTitleModel.IDWear");
                if(id != 0)
                {
                    LuaTable row = RawLuaConfig.Instance.GetRowData("t_title", id);
                    if(row!=null)
                    ChangeHeroTitle(row.Get<string>("icon"));
                }
            }
            else if (actor.mActorType == ActorType.AT_REMOTE_PLAYER)
            {
                InitPKStatus(actor as OtherPlayer);
              //  SetOtherPkLogo(actor as OtherPlayer);
                ValueImage.transform.parent.gameObject.SetActive(true);

                //LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
                int id = attr.HeroTitle;
                if (id != 0)
                {
                    LuaTable row = RawLuaConfig.Instance.GetRowData("t_title", id);
                    if (row != null)
                        ChangeHeroTitle(row.Get<string>("icon"));
                }
            }
            else if (actor.mActorType == ActorType.AT_MECHANICS)
            {
                InitPKStatus(actor as OtherPlayer);
                ValueImage.transform.parent.gameObject.SetActive(true);
            }
            else if (actor.mActorType == ActorType.AT_MONSTER)
            {
                LuaTable cfg = (actor as MonsterObj).MonsterConfig;
                NameText.text = string.Format("<color=#{2}>{0} {1}级</color>", attr.Name, attr.Level, cfg.Get<int>("type") == 2 ? EliteColor : MonsterColor);
            }
            else if (actor.mActorType == ActorType.AT_NPC)
            {
                NameText.text = attr.Name;
            }
            else if (actor.mActorType == ActorType.AT_PET)
            {
                SetPetLevel(actor);
            }
            else
            { 
                NameText.text = string.Format("{0} {1}级", attr.Name, attr.Level);
            }
        }

        void SetNameTextValue(string rgb)
        {
            string colorName = "";
            if (string.IsNullOrEmpty(playerName))
            {
                return;
            }


            //playerName = string.Format("{0} {1}", fun(attr.VIPLevel, actor.ServerID), attr.Name); 
            if (self)
            {
                if(MapMgr.Instance.IsCrossMap())
                {
                    colorName = string.Format("{0} <color=#{1}>[{2}]{3}</color>",VipName, rgb, Account.Instance.ServerId, playerName);
                }
                else
                {
                    colorName = string.Format("{0} <color=#{1}>{2}</color>",VipName, rgb, playerName);
                }
            }
            else
            {
                GetNewLevel fun = LuaMgr.Instance.GetLuaEnv().Global.GetInPath<GetNewLevel>("Common.GetNewLevel");
                if(fun != null)
                { 
                    colorName = string.Format("{0} <color=#{1}>{2} {3}级</color>", VipName, rgb, playerName, fun(playerLvl));
                }
                else
                {
                    colorName = string.Format("{0} <color=#{1}>{2} {3}级</color>", VipName, rgb, playerName, playerLvl); 
                }
            }
            //NameText.text = lordName + " " + colorName;

            var rbl = NameText.GetComponent<SG.RichLable>();
             
            if (rbl != null)
            {
                rbl.text = lordName + " " + colorName;
            }
            else
            {
                NameText.text = lordName + " " + colorName;
            } 


        }

        void SetHpImageType(bool isPk)
        {
            if(isPk)
            {
                ValueImage.sprite = AtlasSpriteManager.Instance.GetSprite("Hp:hp_bar"); //"Hp:hp_bar"
            }else
            {
                ValueImage.sprite = AtlasSpriteManager.Instance.GetSprite("Hp:hp_bar_player"); //"Hp:hp_bar"
            }
        }

        /// <summary>
        /// 初始化PK状态。
        /// </summary>
        /// <param name="pkvalue">PK值。</param>
        /// <param name="pkstatus">PK状态。</param>
        public void InitPKStatus(int status)
        {
            if (status == PKStatus.PK_STATUS_RED)
            {
                SetNameTextValue(RedRgb);
                return;
            }
            if (status == PKStatus.PK_STATUS_GRAY)
            {
                SetNameTextValue(GreyRgb);
                return;
            }
            SetNameTextValue(YellowRgb);
        }

        /// <summary>
        /// 初始化PK状态。
        /// </summary>
        /// <param name="pkvalue">目标玩家。</param>
        public void InitPKStatus(OtherPlayer player)
        {
            //判定边境3v3或者边境远征
            int MeFact = PlayerData.Instance.Faction;
            int OtFact = player.Faction;
            if((MeFact == 11 || MeFact == 12 || MeFact == 14 || MeFact == 15 || MeFact == 16) 
                && (OtFact == 11 || OtFact == 12 || OtFact == 14 || OtFact == 15 || OtFact == 16))
            {
                if(player.Faction != PlayerData.Instance.Faction)
                {
                    int Fact = player.Faction - 13;
                    if(Fact > 0)
                    {
                        switch(Fact)
                        {
                            case 1: SetNameTextValue(BlueRgb); break;
                            case 2: SetNameTextValue(YellowRgb); break;
                            case 3: SetNameTextValue(OrangeRgb); break;
                            default: SetNameTextValue(StrawberryRgb); break;
                        }
                    }else
                    {
                        SetNameTextValue(RedRgb);
                    }
                    SetHpImageType(true);
                }
                else
                {
                    SetNameTextValue(GreenRgb);
                    SetHpImageType(false);
                }
                return;
            }
            if (PlayerData.Instance.CurPKMode == PKMode.PK_MODE_GUILD && !PlayerData.Instance.GuildData.IsInGuild(player.ServerID))
            {
                LuaTable mapConfig = ConfigManager.Instance.Map.GetMapConfig(MapMgr.Instance.EnterMapId);
                int mType = mapConfig.Get<int>("type");
                if(mType == (int)ESceneType.SCENE_TYPE_GUILD_WAR_ACTIVITY)
                {
                    SetNameTextValue(RedRgb);
                    SetHpImageType(true);
                    return;
                }
            }

            //红名的直接显示
                if (player.PKStatus == PKStatus.PK_STATUS_RED)
            {
                SetNameTextValue(RedRgb);
                SetHpImageType(true);
                return;
            }

            //可攻击对象显示红色(善恶看灰名除外)
            bool canattack = PlayerData.Instance.IsCanAttack(player);
            if (canattack)
            {
                //善恶模式看灰名时灰色
                bool grey = PlayerData.Instance.CurPKMode == PKMode.PK_MODE_EVIL && player.PKStatus == PKStatus.PK_STATUS_GRAY;
                SetNameTextValue(grey ? GreyRgb : RedRgb);
                SetHpImageType(true);
                return;
            }

            //灰名灰色，其它黄色
            SetNameTextValue(player.PKStatus == PKStatus.PK_STATUS_GRAY ? GreyRgb : (PlayerData.Instance.TeamData.IsInTeam(player.ServerID) ? GreenRgb : NormalRgb));
            bool isTeamAndGuild = PlayerData.Instance.TeamData.IsInTeam(player.ServerID) || PlayerData.Instance.GuildData.IsInGuild(player.ServerID);
            if (isTeamAndGuild)
            {
                SetHpImageType(false);
            }else
            {
                SetHpImageType(player.PKStatus == PKStatus.PK_STATUS_GRAY ? true : false);
            }          
        }

        /// <summary>
        /// 更新称号
        /// </summary>
        /// <param name="strIcon">称号图片-</param>
        public void ChangeHeroTitle(string strIcon)
        {
            if (null == ImageHeroTitle) return;

            if(null == strIcon) //卸载称号
            {
                ImageHeroTitle.gameObject.SetActive(false);
            }
            else
            {
                ImageHeroTitle.gameObject.SetActive(true);
                ImageHeroTitle.GetComponent<Image>().sprite = AtlasSpriteManager.Instance.GetSprite(strIcon);
            }
        }

        /// <summary>
        /// 更改阵营，需要更改血条和名字颜色
        /// </summary>
        public void ChangeFaction(int iCamp)
        {
            if(!self && (iCamp == 11 || iCamp == 12 || iCamp == 14 || iCamp == 15 || iCamp == 16))
            {
                if(iCamp != 0 && iCamp != PlayerData.Instance.Faction)
                {
                    SetNameTextValue(RedRgb);
                    SetHpImageType(true);
                }
                else
                {
                    SetNameTextValue(YellowRgb);
                    SetHpImageType(false);
                }
            }
        }

        public void SetBindNode(GameObject owner, string name)
        {
            if (owner == null)
            {
                return;
            }

            Owner = owner;
            BindNode = Owner.transform.Find(name);
            if (BindNode == null)
            {
                LogMgr.UnityWarning(string.Format("{0} has no child {1}. Use self instead.", Owner.name, name));
                BindNode = Owner.transform;
            }
            UpdatePosition();
        }

        void Update()
        {
            UpdateHPShow();
        }

        private void LateUpdate()
        {
            UpdatePosition();
        }

        //更新血条显示
        protected virtual void UpdateHPShow()
        {
            if (m_MoveValue > m_CurValue)
            {
                float mv = m_AniSpeed * Time.deltaTime;
                m_MoveValue = Mathf.Max(m_MoveValue - mv, m_CurValue);
                MoveImage.fillAmount = m_MoveValue;
            }
        }

        protected virtual void UpdatePosition()
        {
            if (BindNode == null || CoreEntry.gCameraMgr.MainCamera == null)
            {
                return;
            }

            Vector3 sv = CoreEntry.gCameraMgr.MainCamera.WorldToScreenPoint(BindNode.position);
            SelfRT.position = MainPanelMgr.Instance.uiCamera.ScreenToWorldPoint(sv);
        }

        /// <summary>
        /// 设置当前血量。
        /// </summary>
        /// <param name="v">血量比例值。</param>
        /// <param name="ani">是否带动画。</param>
        public virtual void SetValue(float v, bool ani = true)
        {
            m_CurValue = Mathf.Clamp01(v);
            ValueImage.fillAmount = m_CurValue;

            if (ani)
            {
                if (m_MoveValue < m_CurValue)
                {
                    //回血表现没有过程，直接加上去
                    m_MoveValue = m_CurValue;
                    MoveImage.fillAmount = m_CurValue;
                }
                else
                {
                    //根据掉血量计算动画速度
                    float mv = m_MoveValue - m_CurValue;
                    float t = Mathf.Max(0.001f, mv * AniTime + AniBaseTime);        //防止除零
                    m_AniSpeed = mv / t;
                }
            }
            else
            {
                m_MoveValue = m_CurValue;
                MoveImage.fillAmount = m_CurValue;
            }
        }
        public virtual void SetPetLevel(ActorObj actor)
        {
            PetObj pet = (actor as PetObj);
            if (actor.mActorType == ActorType.AT_PET && pet != null)
            {
                LuaTable cfg = pet.PetConfig;
                //if (pet.m_MasterActor != null)
                {
                    GetHeroDesc fun = LuaMgr.Instance.GetLuaEnv().Global.GetInPath<GetHeroDesc>("ModelManager.HeroModel.GetHeroDesc");
                    //Debug.LogError("pet.Starlevel: " + pet.Starlevel);
                    if (fun != null)
                    {
                        NameText.text = fun(cfg.Get<int>("id"), pet.Starlevel, pet.OwnName, pet.Qua,LordText);
                    }
                    else
                    {
                        LordText.text = "";
                        NameText.text = string.Format("{0}\n{1}的魔神", cfg.Get<string>("name"), pet.OwnName);
                    }
                }
                //else
                //{
                //    NameText.text = string.Format("{0}", cfg.Get<string>("name"));
                //}
            }
        }

    }
}

