/**
* @file     : HPBarBoss.cs
* @brief    : 
* @details  : Boss血条
* @author   : XuXiang
* @date     : 2017-06-03 11:40
*/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using XLua;

namespace SG
{
[Hotfix]
    public class HPBarBoss : HPBar
    {
        public Sprite[] BarSprite;                                  //血条循环图像

        public Image TopMoveImage;                                  //顶层动画血条(掉到下一管血的时候用)
        public Image BackImage;                                     //血条背景(下一管血的内容)
        public Text LastText;                                       //剩余多少管血
        public TransformAnimation HurtAni;                          //受击动画
        public Image IcoImage;                                      //Boss头像
        public Image BossType;                                      //boss 类型 人 魔 神
        private int m_TotalSection;                                 //用多少管血去表现

        string[] m_bossTypePath = new string[] { "Hero:text_ren", "Hero:text_shen", "Hero:text_mo", "Hero:texi_sheng" }; //type 路径

        public Text _dropText;

        private bool mainplayerDead = false;
        //更新血条显示
        protected override void UpdateHPShow()
        {
            if (m_MoveValue > m_CurValue)
            {
                float mv = m_AniSpeed * Time.deltaTime / m_TotalSection;
                m_MoveValue = Mathf.Max(m_MoveValue - mv, m_CurValue);

                int cs;
                float csv = GetSectionValue(m_CurValue, out cs);
                int ms;
                float msv = GetSectionValue(m_MoveValue, out ms);

                int s = ms - cs;        //当前表现进度与实际段数差异
                if (s > 0)
                {
                    //大于两段时，暂时不显示当前血条
                    //TopMoveImage.fillAmount = msv;
                    //MoveImage.fillAmount = 1;
                    SetProgress(TopMoveImage, msv);
                    SetProgress(MoveImage, 1);
                }
                else
                {
                    //ValueImage.fillAmount = csv;
                    //TopMoveImage.fillAmount = 0;
                    //MoveImage.fillAmount = msv;
                    SetProgress(ValueImage, csv);
                    SetProgress(TopMoveImage, 0);
                    SetProgress(MoveImage, msv);
                }
            }
        }

        protected override void UpdatePosition()
        {
            //BOSS血条不需要更新位置
        }

        /// <summary>
        /// 初始化血条。
        /// </summary>
        /// <param name="bname">BOSS名称。</param>
        /// <param name="num">血量管数。</param>
        /// <param name="value">默认血量比例。</param>
        public override void Init(GameObject owner, float value = 1, int num = 1, bool ani = false)
        {
            if (owner == null)
            {
                return;
            }
            Owner = owner;
            m_TotalSection = Mathf.Max(num, 1);
            SetValue(value, ani);

            ActorObj actor = Owner.GetComponent<ActorObj>();
            if (actor == null)     //血条测试时
            {
                return;
            }
            BaseAttr attr = actor.mBaseAttr;
            NameText.text = string.Format("{0} {1}级", attr.Name, attr.Level);
            LastText.enabled = num > 0;

            //显示头像
            string ico = (actor as MonsterObj).ModelConfig.Get<string>("head_icon");
            IcoImage.sprite = AtlasSpriteManager.Instance.GetSprite(ico);
            int _typeName = (actor as MonsterObj).MonsterConfig.Get<int>("race");
            int _type = 0;
            if (_typeName == 0)
            {
				BossType.enabled = false;
               // LogMgr.UnityLog("检查怪物配置表 race 字段 取到为空");
            }
            else
            {
				BossType.enabled = true;
                _type = _typeName - 1;
                if(0 <= _type && _type < m_bossTypePath.Length)
                {
                    BossType.sprite = AtlasSpriteManager.Instance.GetSprite(m_bossTypePath[_type]);
                }
                else
                {
                    BossType.enabled = false;
                }

            }

            int _isShowReward = (actor as MonsterObj).MonsterConfig.Get<int>("isShowReward");
            string _warkeyReward = (actor as MonsterObj).MonsterConfig.Get<string>("warkeyReward");

            SetDropInfo(_isShowReward, _warkeyReward);


        }

        public void OnEnable()
        {
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_OBJ_DEAD, OnObjectDead);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_REVIVE, OnRevive);
        }

        public void OnDisable()
        {
            CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_SC_OBJ_DEAD, OnObjectDead);
            CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_SC_REVIVE, OnRevive);
        }

        private void OnObjectDead(GameEvent ge, EventParameter parameter)
        {
            MsgData_sObjDeadInfo msg = parameter.msgParameter as MsgData_sObjDeadInfo;
            if (null == msg)
                return;

            ActorObj behitActor = CoreEntry.gActorMgr.GetActorByServerID(msg.ID);
            ActorObj attackActor = CoreEntry.gActorMgr.GetActorByServerID(msg.KillerID);
            if (behitActor == CoreEntry.gActorMgr.MainPlayer) {
                StopHurtAnimation();
                mainplayerDead = true;
                LogMgr.UnityLog("HPBarBoss StopHurtAnimation");
            }
        }

        private void OnRevive(GameEvent ge, EventParameter parameter)
        {
            MsgData_sRevive data = parameter.msgParameter as MsgData_sRevive;
            if (null == data)
                return;

            if (0 == data.Result )
            {
                ActorObj actor = CoreEntry.gActorMgr.GetActorByServerID(data.RoleID);
                if (actor == CoreEntry.gActorMgr.MainPlayer) {
                    LogMgr.UnityLog("HPBarBoss ShowHurtAnimation");
                    mainplayerDead = false;
                    ShowHurtAnimation();
                }
            }
        }

        /// <summary>
        /// 显示血条。
        /// </summary>
        /// <param name="visible">是否显示。</param>
        public void Show(bool visible = true)
        {
            gameObject.SetActive(visible);
        }

        /// <summary>
        /// 显示受击动画。
        /// </summary>
        public void ShowHurtAnimation()
        {
            if (mainplayerDead == false)
                HurtAni.Play();
        }
        /// <summary>
        /// stop HurtAnimation Curve
        /// </summary>
        public void StopHurtAnimation()
        {
            HurtAni.Stop();
        }
            
        /// <summary>
        /// 设置当前血量。
        /// </summary>
        /// <param name="v">血量比例值。</param>
        /// <param name="ani">是否带动画。</param>
        public override void SetValue(float v, bool ani = true)
        {
            float value = Mathf.Clamp01(v);
            int section;
            float svalue = GetSectionValue(value, out section);
            m_CurValue = value;
            SetLifeBarImage(section + 1);
            //ValueImage.fillAmount = svalue;
            //BackImage.fillAmount = section > 0 ? 1 : 0;
            SetProgress(ValueImage, svalue);
            SetProgress(BackImage, section > 0 ? 1 : 0);
            LastText.text = v > 0 ? string.Format("x{0}", section + 1) : string.Empty;

            if (ani)
            {
                if (m_MoveValue > m_CurValue)
                {
                    //如果一次掉血大于两管，则优化显示
                    int ms;
                    float msv = GetSectionValue(m_MoveValue, out ms);
                    int s = ms - section;
                    if (s >= 2)
                    {
                        m_MoveValue -= (s - 1) * 1.0f / m_TotalSection;         //表现上加速跳管
                        msv = GetSectionValue(m_MoveValue, out ms);
                    }

                    //根据掉血量计算动画速度
                    float mv = msv - svalue;
                    if (ms > section)
                    {
                        mv += 1;
                    }
                    float t = Mathf.Max(0.001f, mv * AniTime + AniBaseTime);        //防止除零
                    m_AniSpeed = mv / t;
                    ShowHurtAnimation();
                }
                else
                {
                    //回血表现没有过程，直接加上去
                    m_MoveValue = m_CurValue;
                    //TopMoveImage.fillAmount = 0;
                    //MoveImage.fillAmount = svalue;
                    SetProgress(TopMoveImage, 0);
                    SetProgress(MoveImage, svalue);
                }
            }
            else
            {
                m_MoveValue = m_CurValue;
                //TopMoveImage.fillAmount = 0;
                //MoveImage.fillAmount = svalue;
                SetProgress(TopMoveImage, 0);                
                SetProgress(MoveImage, svalue);
            }
        }

        /// <summary>
        /// 获取在一管内的比例。
        /// </summary>
        /// <param name="value">整体比例。</param>
        /// <param name="section">还有多少管满的。</param>
        /// <returns>一管内的比例。</returns>
        float GetSectionValue(float value, out int section)
        {
            float sv = 1.0f / m_TotalSection;                   //一管血占的比例
            section = (int)(value / sv);                       //当前还有多少管满血
            float lastvalue = value - sv * section;                //多出来的那部分比例
            return lastvalue / sv;                         //多出来那部分比例占一管的比例
        }

        /// <summary>
        /// 设置血条图像。
        /// </summary>
        /// <param name="section">当前还有多少管满的血。</param>
        void SetLifeBarImage(int section)
        {
            //设置血条图像
            int ci = (section - 1 + BarSprite.Length) % BarSprite.Length;
            ValueImage.sprite = BarSprite[ci];
            MoveImage.sprite = ValueImage.sprite;
            TopMoveImage.sprite = BarSprite[section % BarSprite.Length];
            BackImage.enabled = section > 1;
            if (section > 1)
            {
                int bi = (ci - 1 + BarSprite.Length) % BarSprite.Length;
                BackImage.sprite = BarSprite[bi];
            }
        }

        /// <summary>
        /// 设置进度。
        /// </summary>
        /// <param name="img">要设置的图像。</param>
        /// <param name="v">进度值。</param>
        private void SetProgress(Image img, float v)
        {
            img.fillAmount = v;

            //float x = m_BarWeight * (1 - Mathf.Clamp01(v));
            //RectTransform rt = img.GetComponent<RectTransform>();
            //rt.anchoredPosition = new Vector2(-x, rt.anchoredPosition.y);
        }
        /// <summary>
        /// 设置掉落
        /// </summary>
        void SetDropInfo(int isShowReward,string warkeyReward)
        {
            if(isShowReward == 0)
            {
                _dropText.gameObject.SetActive(false);
            }
            else if (isShowReward == 1)
            {
                string[] _wr = warkeyReward.Split('#');
                _dropText.gameObject.SetActive(true);
                for(int i=0;i<4;++i)
                {
                    GameObject objIcon = _dropText.transform.Find(i.ToString() + "/icon").gameObject;
                    Image _ig = _dropText.transform.Find(i.ToString()).GetComponent<Image>();
                    Image _igicon = objIcon.GetComponent<Image>();

                    if (i< _wr.Length)
                    {
                        _ig.gameObject.SetActive(true);
                        string[] sr = _wr[i].Split(',');
                        int _id = int.Parse(sr[0]);

                        LuaTable cfg = ConfigManager.Instance.BagItem.GetItemConfig(_id);
                        if (cfg == null)
                        {
                            continue;
                        }
                        
                        string _pathicon = cfg.Get<string>("icon");
                        int _quality = cfg.Get<int>("quality");

                        // 设置掉落物品特效
                        string _pfxName = "";
                        if(_quality < HPBar.QuaPfxs.Length)
                        {
                            _pfxName = HPBar.QuaPfxs[_quality];
                        }
                        for(int j = 0; j < objIcon.transform.childCount; ++j)
                        {
                            objIcon.transform.GetChild(j).gameObject.SetActive(false);
                        }
                        Transform trsPfx = objIcon.transform.Find("pfx" + _quality);
                        if(null == trsPfx)
                        {
                            GameObject pfx = CommonTools.AddSubChild(objIcon, _pfxName);
                            if(pfx != null)
                            {
                                trsPfx = pfx.transform;
                                trsPfx.name = "pfx" + _quality;

                                Vector2 size = _ig.GetComponent<RectTransform>().sizeDelta;
                                pfx.transform.localScale = new Vector3(size.x / 76, size.y / 76, 1);
                            }
                        }else
                        {
                            trsPfx.gameObject.SetActive(true);
                        }

                        _igicon.sprite = AtlasSpriteManager.Instance.GetSprite(_pathicon);
                        _ig.sprite = AtlasSpriteManager.Instance.GetSprite(ConfigManager.Instance.BagItem.GetQualityFrame(_quality));
                        _ig.GetComponent<Button>().onClick.AddListener(delegate ()
                        {
                            OnBtnDrop(_id);
                        });
                        continue;
                    }
                    _ig.gameObject.SetActive(false);
                }

            }
        }

        void OnBtnDrop(int id)
        {
            ShowItemTips fun = LuaMgr.Instance.GetLuaEnv().Global.GetInPath<ShowItemTips>("Common.ShowItemTips");
            if (fun != null)
            {
                fun(id);
            }
        }
    }
}

