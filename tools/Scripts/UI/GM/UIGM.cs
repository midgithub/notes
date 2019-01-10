/**
* @file     : UIGM.cs
* @brief    : GM窗口脚本
* @details  : 
* @author   :  
* @date     :  
*/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text;
using XLua;

namespace SG
{
    public enum DEBUG_TYPE
    {
        DEBUG_GM,
        DEBUG_LOG,

        DEBUG_ALL,
    }

    public enum LOG_TYPE
    {
        LOG_ERROR = 100,
        LOG_TRACE,
        LOG_NULL = -1,
    }

    // 快捷键结构定义
    public struct HotKeyDesc
    {
        public string strName;		// Lable
        public string strDesc;		// 描述
    };

[Hotfix]
    public class UIGM : MonoBehaviour
    {
        public Text mTextOutPut;
        private StringBuilder mStrOutPut = new StringBuilder();
        public StringBuilder mOutput
        {
            set
            {
                if (value == null)
                {
                    mStrOutPut = new StringBuilder();
                    mTextOutPut.text = "";
                    return;
                }
                mStrOutPut.Append(value);
                mTextOutPut.text = mStrOutPut.ToString();
            }
            get { return mStrOutPut; }
        }
        public InputField m_TimeScaleInput;
        public InputField mInput = null;
        Button mHotKeyBtn = null;
        Text mHotKeyLabel = null;

        int mHotKeyIndex = -1;
        GmMgr mGmMgr = null;
        Dictionary<int, HotKeyDesc> mHotKeyMap = new Dictionary<int, HotKeyDesc>();

        static int MAX_LOG_SIZE = 100;
        string mLogType = System.String.Empty;
        Queue<string> mLogGm = new Queue<string>();
        Queue<string> mLogTrace = new Queue<string>();
        Queue<string> mLogError = new Queue<string>();
        List<string> mLogAll = new List<string>();
#if !PUBLISH_RELEASE

        // Use this for initialization

        Text switchBtnText = null;

        void Start()
        {

            Transform transform = this.gameObject.transform.Find("GMPanel/SendBtn");
            if (transform != null)
            {
                uGUI.UIEventListener.Get(transform.gameObject).onPointerClick = SendBtnClick;
            }



            transform = this.gameObject.transform.Find("GMPanel/CloseBtn");
            if (transform != null)
            {
                uGUI.UIEventListener.Get(transform.gameObject).onPointerClick = CloseBtnClick;
            }
            transform = this.gameObject.transform.Find("GMPanel/LogClearBtn");
            if (transform != null)
            {
                uGUI.UIEventListener.Get(transform.gameObject).onPointerClick = LogClearBtnClick;
            }
            transform = this.gameObject.transform.Find("GMPanel/TimeSet/btnSetTime");
            if (transform != null)
            {
                uGUI.UIEventListener.Get(transform.gameObject).onPointerClick = OnBtnSetScaleTime;
            }
            transform = this.gameObject.transform.Find("GMPanel/InputField");
            if (transform != null)
            {
                mInput = transform.GetComponent<InputField>();
            }

            transform = this.gameObject.transform.Find("GMPanel/TimeSet/InputFieldTimeScale");
            if (transform != null)
            {
                m_TimeScaleInput = transform.GetComponent<InputField>();
            }

            transform = this.gameObject.transform.Find("GMPanel/OutPut");
            if (transform != null)
            {
                mTextOutPut = transform.GetComponent<Text>();
            }

            transform = this.gameObject.transform.Find("GMPanel/ProfilerBtn");
            if (transform != null)
            {
                uGUI.UIEventListener.Get(transform.gameObject).onPointerClick = OnMemInfoBtnClick;
            }

            transform = this.gameObject.transform.Find("GMPanel/SwitchQualityBtn");
            if (transform != null)
            {
                uGUI.UIEventListener.Get(transform.gameObject).onPointerClick = OnSwitchQualityBtnClick;
            }
            switchBtnText = this.gameObject.transform.Find("GMPanel/SwitchQualityBtn/Text").GetComponent<Text>();
            RefreshSwitchBtnText();

            // 初始化HotKey模块
            InitHotKey();
        }
        void RegisterHotKey(string name, string desc)
        {
            HotKeyDesc hotKeyDesc;
            hotKeyDesc.strName = name;
            hotKeyDesc.strDesc = desc;

            mHotKeyMap.Add(mHotKeyMap.Count, hotKeyDesc);
        }

        void InitHotKey()
        {
            // 注册HotKey
            RegisterHotKey("无敌", "#gm wudi");
            RegisterHotKey("秒怪", "#gm miaoguai");
            RegisterHotKey("1/4", "#gm shijian");
            RegisterHotKey("取消自动", "#gm qixiao auto");
            RegisterHotKey("技能范围", "#gm show skillscope");
            RegisterHotKey("不绘制", "#gm norender");
            RegisterHotKey("无攻击", "#gm wushang");
            RegisterHotKey("网络", "#gm recordnet");
            RegisterHotKey("音效", "#gm soundEffect");
            RegisterHotKey("AI", "#gm ai");

            // 注册Click事件
            for (int i = 0; i < mHotKeyMap.Count; i++)
            {
                string strIndex = "GMPanel/HotKey/" + i;
                Transform transform = this.gameObject.transform.Find(strIndex);
                if (transform)
                {
                    GameObject btnObj = transform.gameObject;
                    Text label = btnObj.GetComponentInChildren<Text>();
                    label.text = mHotKeyMap[i].strName;

                    //UIEventListener.Get(btnObj).onClick = HotKeyBtnClick;
                    uGUI.UIEventListener.Get(btnObj).onPointerClick = HotKeyBtnClick;
                    btnObj.GetComponent<Button>().image.color  = Color.white;
                }
            }

            // gm事件处理
            mGmMgr = ModuleServer.MS.GGmMgr;
        }
                // Send按钮单击事件
        public void SendBtnClick(GameObject btnObj)
        {
            // 获取内容
            if (mInput)
            {
                string strText = NGUIText.StripSymbols(mInput.text);
                if (!string.IsNullOrEmpty(strText))
                {
                    LogMgr.UnityLog("input: " + strText);

                    LogToBuff(DEBUG_TYPE.DEBUG_GM, LOG_TYPE.LOG_NULL, strText);

                    string[] cmdstr = strText.Split("\r\n".ToCharArray());
                    for (int i=0; i<cmdstr.Length; ++i)
                    {
                        string cmd = cmdstr[i].Trim();
                        if (!string.IsNullOrEmpty(cmd))
                        {
                            mGmMgr.GmReq(cmd);
                        }
                    }
                    mInput.text = "";
                    
                }
            }
        }
                // Close按钮单击事件
        public  void CloseBtnClick(GameObject btnObj)
        {
            MainPanelMgr.Instance.HideDialog("UIGM");
        }

                // LogLevel按钮单击事件
        public  void LevelPopupChange()
        {

        }

                // LogClear按钮单击事件
        public  void LogClearBtnClick(GameObject btnObj)
        {
            mOutput = null;
            mLogAll.Clear();
            mLogGm.Clear();
            mLogTrace.Clear();
            mLogError.Clear();

        }

        public void OnMemInfoBtnClick(GameObject btnObj)
        {
            ProfilerGUI.ShowMemo(!ProfilerData.IsProfiling);
        }

        public void OnSwitchQualityBtnClick(GameObject btnObj)
        {
            var b = GameGraphicSetting.IsLowQuality;
            GameGraphicSetting.IsLowQuality = !b;
            RefreshSwitchBtnText();
        }

        void RefreshSwitchBtnText()
        {
            if (GameGraphicSetting.IsLowQuality)
            {
                switchBtnText.text = "当前低画质";
            }
            else
            {
                switchBtnText.text = "当前高画质";
            }
        }

        // HoteKey模块按钮单击事件
        public  void HotKeyBtnClick(GameObject btnObj)
        {

            mHotKeyIndex = int.Parse(btnObj.name);
            Text label = btnObj.GetComponentInChildren<Text>();
            if (label)
            {
                LogMgr.UnityLog("hotkey: index[" + mHotKeyIndex + "]," +
                          "name[" + mHotKeyMap[mHotKeyIndex].strName + "]," +
                          "desc[" + mHotKeyMap[mHotKeyIndex].strDesc + "]");
            }

            // 储存到显示Buff
            LogToBuff(DEBUG_TYPE.DEBUG_GM, LOG_TYPE.LOG_NULL, mHotKeyMap[mHotKeyIndex].strDesc);

            // 快捷按钮处理
            mHotKeyBtn = btnObj.GetComponent<Button>();
            mHotKeyLabel = btnObj.GetComponentInChildren<Text>();

            switch (mHotKeyIndex)
            {
                case 0:
                    OnWudi();
                    break;
                case 1:
                    OnMiaoGuai();
                    break;
                case 2:
                    OnTimeScale();
                    break;
                case 3:
                    OnCancelAtuoFight();
                    break;
                case 4:
                    OnShowSkillScope();
                    break;

                case 5:
                    CoreEntry.gGameMgr.RenderMeshSwith();
                    break;
                //case 6:
                //    CoreEntry.gGameMgr.RenderMeshSwith();
                //    break;
                case 6:
                    OnWuShang();
                    break;
                case 7:
                    OnRecordNet();
                    break;
                case 8:
                    OnCommon(mGmMgr.SwitchSoundEffect);
                    break;
                case 9:
                    OnCommon(mGmMgr.SwitchAI);
                    break;
                default:
                    break;
            }
        }
        void OnCommon(Func<bool> act)
        {
            string strResult = System.String.Empty;

            if (act())
            {
                if (mHotKeyBtn.image.color == Color.gray)
                {
                    mHotKeyBtn.image.color = Color.white;
                    strResult = "取消" + mHotKeyMap[mHotKeyIndex].strName + "成功";
                    mHotKeyLabel.text = mHotKeyMap[mHotKeyIndex].strName;
                }
                else
                {
                    mHotKeyBtn.image.color = Color.gray;
                    strResult = "设置" + mHotKeyMap[mHotKeyIndex].strName + "成功";
                    mHotKeyLabel.text = "取消" + mHotKeyMap[mHotKeyIndex].strName;
                }
            }
            else
            {
                strResult = mHotKeyMap[mHotKeyIndex].strName + "失败";
            }

            LogToBuff(DEBUG_TYPE.DEBUG_GM, LOG_TYPE.LOG_NULL, strResult);
        }

        void OnWudi()
        {
            string strResult = System.String.Empty;

            MsgData_sItemTips msg = new MsgData_sItemTips();
            msg.items = new List<MsgData_sItemTipsVO>();
            BagInfo bag =  PlayerData.Instance.BagData.GetBagInfo(BagType.ITEM_BAG_TYPE_COMMON);
            if(bag!=null)
            {
                foreach (KeyValuePair<int, ItemInfo> key in bag.ItemInfos)
                {
                    MsgData_sItemTipsVO itemdata = new MsgData_sItemTipsVO();
                    itemdata.type = 1;
                    itemdata.id = key.Value.ID;
                    itemdata.count = key.Value.Count;
                    msg.items.Add(itemdata);
                }
            }

            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_ITEMTIPS, EventParameter.Get(msg));
            //LuaTable tbl = LuaMgr.Instance.GetLuaEnv().Global.Get<LuaTable>("t_equip");
  
            if (mGmMgr.WuDi())
            {
                if (mHotKeyBtn.image.color == Color.gray)
                {
                    mHotKeyBtn.image.color = Color.white;
                    strResult = "取消" + mHotKeyMap[mHotKeyIndex].strName + "成功";
                    mHotKeyLabel.text = mHotKeyMap[mHotKeyIndex].strName;
                }
                else
                {
                    mHotKeyBtn.image.color = Color.gray;
                    strResult = "设置" + mHotKeyMap[mHotKeyIndex].strName + "成功";
                    mHotKeyLabel.text = "取消" + mHotKeyMap[mHotKeyIndex].strName;
                }
            }
            else
            {
                strResult = mHotKeyMap[mHotKeyIndex].strName + "失败";
            }

            LogToBuff(DEBUG_TYPE.DEBUG_GM, LOG_TYPE.LOG_NULL, strResult);
        }

        void OnMiaoGuai()
        {
            string strResult = System.String.Empty;

            if (mGmMgr.MiaoGuai())
            {
                if (mHotKeyBtn.image.color == Color.gray)
                {
                    mHotKeyBtn.image.color = Color.white;
                    strResult = "取消" + mHotKeyMap[mHotKeyIndex].strName + "成功";
                    mHotKeyLabel.text = mHotKeyMap[mHotKeyIndex].strName;
                }
                else
                {
                    mHotKeyBtn.image.color = Color.gray;
                    strResult = "设置" + mHotKeyMap[mHotKeyIndex].strName + "成功";
                    mHotKeyLabel.text = "取消" + mHotKeyMap[mHotKeyIndex].strName;
                }
            }
            else
            {
                strResult = mHotKeyMap[mHotKeyIndex].strName + "失败";
            }

            LogToBuff(DEBUG_TYPE.DEBUG_GM, LOG_TYPE.LOG_NULL, strResult);
        }
        void OnWuShang()
        {
            string strResult = System.String.Empty;

            if (mGmMgr.WuShang())
            {
                if (mHotKeyBtn.image.color == Color.gray)
                {
                    mHotKeyBtn.image.color = Color.white;
                    strResult = "取消" + mHotKeyMap[mHotKeyIndex].strName + "成功";
                    mHotKeyLabel.text = mHotKeyMap[mHotKeyIndex].strName;
                }
                else
                {
                    mHotKeyBtn.image.color = Color.gray;
                    strResult = "设置" + mHotKeyMap[mHotKeyIndex].strName + "成功";
                    mHotKeyLabel.text = "取消" + mHotKeyMap[mHotKeyIndex].strName;
                }
            }
            else
            {
                strResult = mHotKeyMap[mHotKeyIndex].strName + "失败";
            }

            LogToBuff(DEBUG_TYPE.DEBUG_GM, LOG_TYPE.LOG_NULL, strResult);
        }

        void OnRecordNet()
        {
            Debug.LogWarning("已废弃");
            //string strResult = System.String.Empty;

            //if (mGmMgr.RecordNet())
            //{
            //    if (mHotKeyBtn.image.color == Color.gray)
            //    {
            //        mHotKeyBtn.image.color = Color.white;
            //        strResult = "取消" + mHotKeyMap[mHotKeyIndex].strName + "成功";
            //        mHotKeyLabel.text = mHotKeyMap[mHotKeyIndex].strName;
            //    }
            //    else
            //    {
            //        mHotKeyBtn.image.color = Color.gray;
            //        strResult = "设置" + mHotKeyMap[mHotKeyIndex].strName + "成功";
            //        mHotKeyLabel.text = "取消" + mHotKeyMap[mHotKeyIndex].strName;
            //    }
            //}
            //else
            //{
            //    strResult = mHotKeyMap[mHotKeyIndex].strName + "失败";
            //}

            //LogToBuff(DEBUG_TYPE.DEBUG_GM, LOG_TYPE.LOG_NULL, strResult);
        }

        void OnTimeScale()
        {
            if (!CoreEntry.gGameMgr.m_bScaleTime)
            {
                Time.timeScale = 0.05f;
            }
            else
            {
                TimeScaleCore.ResetValue();
            }
            CoreEntry.gGameMgr.m_bScaleTime = !CoreEntry.gGameMgr.m_bScaleTime;
        }

                //取消自动战斗
        void OnCancelAtuoFight()
        {

        }

        void OnShowSkillScope()
        {
            string strResult = System.String.Empty;
            if (mGmMgr.ShowSkillScope())
            {
                if (mHotKeyBtn.image.color == Color.gray)
                {
                    mHotKeyBtn.image.color = Color.white;
                    strResult = "取消" + mHotKeyMap[mHotKeyIndex].strName + "成功";
                    mHotKeyLabel.text = mHotKeyMap[mHotKeyIndex].strName;
                }
                else
                {
                    mHotKeyBtn.image.color = Color.gray;
                    strResult = "设置" + mHotKeyMap[mHotKeyIndex].strName + "成功";
                    mHotKeyLabel.text = "取消" + mHotKeyMap[mHotKeyIndex].strName;
                }
            }
            else
            {
                strResult = mHotKeyMap[mHotKeyIndex].strName + "失败";
            }

            LogToBuff(DEBUG_TYPE.DEBUG_GM, LOG_TYPE.LOG_NULL, strResult);
        }

        float m_timeScale = 1;

        public void OnBtnSetScaleTime(GameObject obj)
        {
            float val = 0;

            if (!float.TryParse(m_TimeScaleInput.text, out val))
            {
                return;
            }
            Debug.Log("Time Scale: "+ val);

            Time.timeScale = val;
            m_timeScale = val;
            CancelInvoke();

            StopAllCoroutines();
            StartCoroutine("LoopSetTimeScale");

        }

        public IEnumerator LoopSetTimeScale()
        {
            while (true)
            {
                Time.timeScale = m_timeScale;
                yield return new WaitForEndOfFrame();
            }
        }

        void OnEvent()
        {
            if (EventToUI.sEvent.CompareTo("EU_DEBUGLOGOUT") == 0)
            {
                LOG_TYPE type = (LOG_TYPE)EventToUI.GetArg(UIEventArg.Arg1);
                string strMsg = (string)EventToUI.GetArg(UIEventArg.Arg2);

                LogToBuff(DEBUG_TYPE.DEBUG_LOG, type, strMsg);
            }
            else if (EventToUI.sEvent.CompareTo("EU_GMOP_OK") == 0)
            {
                outPutAppend("gm操作成功!");
            }

        }
        public void Log(string msg)
        {
            if (mLogTrace.Count == MAX_LOG_SIZE)
            {
                mLogTrace.Dequeue();
            }
            mLogTrace.Enqueue(msg);


            outPutAppend(msg);

        }

        public void outPutAppend(string msg)
        {
            mOutput.Append(msg);
            mOutput.Append('\n');
            mTextOutPut.text = mStrOutPut.ToString();
        }
        void LogToBuff(DEBUG_TYPE eDebugType, LOG_TYPE eLogType, string strMsg)
        {
            string strTime = System.DateTime.Now.ToString("HH:mm:ss");
            string strBuffer = System.String.Empty;

            if (eDebugType == DEBUG_TYPE.DEBUG_GM)
            {
                string strType = eDebugType.ToString();
                string[] strArray = strType.Split('_');

                mLogType = strArray[1];

                strBuffer = string.Format("[{0}][{1}]:[{2}]", strTime, strArray[1], strMsg);

                if (mLogGm.Count == MAX_LOG_SIZE)
                {
                    mLogGm.Dequeue();
                }
                mLogGm.Enqueue(strBuffer);

                if (mLogType.ToLower() == "gm"
                    || mLogType.ToLower() == "all")
                {
                    outPutAppend(strBuffer); 
                }
            }
            else if (eDebugType == DEBUG_TYPE.DEBUG_LOG)
            {
                string strType = eLogType.ToString();
                string[] strArray = strType.Split('_');

                strBuffer = string.Format("[{0}][{1}]:[{2}]", strTime, strArray[1], strMsg);

                if (eLogType == LOG_TYPE.LOG_TRACE)
                {
                    if (mLogTrace.Count == MAX_LOG_SIZE)
                    {
                        mLogTrace.Dequeue();
                    }
                    mLogTrace.Enqueue(strBuffer);

                    if (mLogType == "trace"
                        || mLogType == "all")
                    {
                        outPutAppend(strBuffer);
                    }
                }
                else if (eLogType == LOG_TYPE.LOG_ERROR)
                {
                    if (mLogError.Count == MAX_LOG_SIZE)
                    {
                        mLogError.Dequeue();
                    }
                    mLogError.Enqueue(strBuffer);

                    if (mLogType == "error"
                        || mLogType == "all")
                    {
                        outPutAppend(strBuffer);
                    }
                }
            }
        }


#else
        void Start()
        {
            if (gameObject != null)
            {
                gameObject.SetActive(false);
            }
        }
#endif
    }

};//End SG 


