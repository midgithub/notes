/**
* @file     : MorphMgr
* @brief    : 
* @details  : 
* @author   : CW
* @date     : 2017-08-10
*/
using UnityEngine;
using System.Collections.Generic;
using XLua;

namespace SG
{
    [LuaCallCSharp]
[Hotfix]
    public class MorphMgr : MonoBehaviour
    {
        private bool bSceneLoaded = false;

        private int morphModelID = 0;
        private bool isMorphing = false;
        private float morphModelScale = 1.0f;
        private Dictionary<long, bool> mMorDict = new Dictionary<long, bool>();
        private Dictionary<long, int> mMorChangeDict = new Dictionary<long,int>();

        private string mUIEffectPath = "Effect/ui/uf_huanlingheti";
        private string mPlayerEffectPath = "Effect/skill/Fit/fx_huanlinght";

        /// <summary>
        /// ��ǰ�����Ƿ��ڱ�����
        /// </summary>
        public bool IsMorphing
        {
            get { return isMorphing; }
        }

        /// <summary>
        /// ����ģ��ID
        /// </summary>
        public int MorphModelID
        {
            get { return morphModelID; }
        }

        /// <summary>
        /// ����ģ������
        /// </summary>
        public float Scale
        {
            get { return morphModelScale; }
        }

        /// <summary>
        /// �����Ƿ����ڱ�����
        /// </summary>
        /// <param name="serverID"></param>
        /// <returns></returns>
        public bool IsInMorph(long serverID)
        {
            if (serverID == MainRole.Instance.serverID)
            {
                return isMorphing;
            }

            if (mMorDict.ContainsKey(serverID))
            {
                return mMorDict[serverID];
            }

            return false;
        }

        /// <summary>
        /// ���ñ���״̬��
        /// </summary>
        /// <param name="serverID"></param>
        public void SetInMorph(long serverID)
        {
            mMorDict[serverID] = true;
        }


        public float GetMorphWeaponScale(long serverID)
        {
            if (!IsInMorph(serverID))
            {
                return 1.0f;
            }

            if (!mMorChangeDict.ContainsKey(serverID))
            {
                return 1.0f;
            }

            int changeID = mMorChangeDict[serverID];
            LuaTable changeCfg = ConfigManager.Instance.Actor.GetChangeConfig(changeID);
            if (null == changeCfg)
            {
                return 1.0f;
            }

            return changeCfg.Get<float>("scale_hb");
        }

        void Start()
        { 
 

            CoreEntry.gEventMgr.AddListener(GameEvent.GE_BEGIN_LOADSCENE, OnSceneLoadBegin);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_AFTER_LOADSCENE, OnSceneLoaded);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_CLEANUP_USER_DATA, OnCleanupUserData);
        } 
        public void OnCleanupUserData(GameEvent ge, EventParameter parameter)
        {
            isMorphing = false;
        }
 
        #region �¼�
        public void OnChangeBegin(MsgData msg)
        {
            MsgData_sChangeBegin data = msg as MsgData_sChangeBegin;
             
            DoMorphing(data.ID, (int)data.ChangeID);
        }

        public void OnChangeEnd(MsgData msg)
        {
            MsgData_sChangeEnd data = msg as MsgData_sChangeEnd;
           

            DoMorphingFinish(data.ID);
        }

        private void OnSceneLoadBegin(GameEvent ge, EventParameter parameter)
        {
            bSceneLoaded = false;

            //mMorphDataCache.Clear();
            //mMorphEndDataCache.Clear();
            mMorDict.Clear();
            mMorChangeDict.Clear();
        }

        private void OnSceneLoaded(GameEvent ge, EventParameter parameter)
        {
            bSceneLoaded = true;
        }
        #endregion
        private void DoMorphing(long serverID, int changeID)
        {
            LuaTable changeCfg = ConfigManager.Instance.Actor.GetChangeConfig(changeID);
            if (null == changeCfg)
            {
                return;
            }

            mMorChangeDict[serverID] = changeID;

            if (serverID == MainRole.Instance.serverID)
            {
                if (isMorphing)
                {
                    return;
                }
                isMorphing = true;
                morphModelID = changeCfg.Get<int>("model_id");
                morphModelScale = changeCfg.Get<float>("scale");
                PlayerObj actor = CoreEntry.gActorMgr.MainPlayer;
                if(actor != null)
                {
                    //StateParameter param = new StateParameter();
                    //param.state = ACTOR_STATE.AS_FIT;
                    //actor.RequestChangeState(param);
                    //this.Invoke("MeMorphingBegin", actor.GetActionLength("fit"));
                    MeMorphingBegin();
                }
            }
            else
            {
                mMorDict[serverID] = true;
                OtherMorphingBegin(serverID, changeID , changeCfg.Get<float>("scale"));
            }

            EventParameter parameter = EventParameter.Get();
            parameter.longParameter = serverID;
            parameter.intParameter = changeID;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_CHANGE_BEGIN, parameter);
        }

        public void DoMorphingFinish(long serverID)
        {
            mMorChangeDict[serverID] = 0;
            if (serverID == MainRole.Instance.serverID)
            {
                if (!isMorphing)
                {
                    return;
                }
                isMorphing = false;
                if (bSceneLoaded)
                {
                    MeMorphingFinish();
                }
            }
            else
            {
                mMorDict[serverID] = false;
                if (bSceneLoaded)
                {
                    OtherMorphFinish(serverID);
                }
            }
            
            EventParameter parameter = EventParameter.Get();
            parameter.longParameter = serverID;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_CHANGE_END, parameter);
        }

        #region ���Ǳ���
        private void MeMorphingBegin()
        {
            PlayerObj actor = CoreEntry.gActorMgr.MainPlayer;
            if (null == actor)
            {
                return;
            }
            Vector3 prePos = actor.transform.position;
            //var isWild = TaskMgr.IsWildTask;
            //var isAuto = AutoAIRunner.IsInAutoMode_Ex;
            //AutoAIRunner.StopAll();

            if (PlayerData.Instance.RideData.RideState == 1)
            {
                //PlayerData.Instance.RideData.mRideState = 0;
                PlayerData.Instance.RideData.SendChangeRideStateRequest(0);
                CoreEntry.gActorMgr.MainPlayer.GetDownHorse();
            }
            CoreEntry.gActorMgr.RemoveActor(actor);

            GameObject obj = CoreEntry.gSceneLoader.LoadPlayer(morphModelID);
            if (null != obj)
            {
                obj.transform.position = prePos;
                obj.transform.eulerAngles = actor.transform.eulerAngles;
                obj.transform.localScale = new Vector3(Scale, Scale, Scale);
            }

            //if (isAuto)
            //{
            //    AutoAIRunner.Init();
            //    TaskMgr.IsWildTask = isWild;
            //    AutoAIRunner.SetAutoFight(true, null);
            //}
            //else if (TaskMgr.LastClickTaskID > 0)
            //{
            //    AutoAIRunner.Init();
            //    AutoAIRunner.SetAutoTask(true);
            //}

            actor.RecycleObj();
            actor.HideBlobShadow();

            //ReSetSkills(0);
            CoreEntry.gActorMgr.MainPlayer.ReBindSkill();
            CoreEntry.gActorMgr.MainPlayer.RebindSkillButton();

            GameObject mEffectObj = CoreEntry.gGameObjPoolMgr.Instantiate(mUIEffectPath);
            if (null != mEffectObj)
            {
                SceneEfxPool efx = mEffectObj.GetComponent<SceneEfxPool>();
                if (efx == null)
                {
                    efx = mEffectObj.AddComponent<SceneEfxPool>();
                }
                if (efx != null)
                {
                    efx.Init(transform.position, 2.0f);
                }

                PanelBase mainPanel = MainPanelMgr.Instance.GetPanel("UIMain");
                if (mainPanel != null)
                {
                    mEffectObj.transform.SetParent(mainPanel.transform);
                    mEffectObj.transform.localPosition = Vector3.zero;
                    mEffectObj.transform.localScale = Vector3.one;
                }
            }
            mEffectObj = CoreEntry.gGameObjPoolMgr.Instantiate(mPlayerEffectPath);
            if (null != mEffectObj)
            {
                SceneEfxPool efx = mEffectObj.GetComponent<SceneEfxPool>();
                if (efx == null)
                {
                    efx = mEffectObj.AddComponent<SceneEfxPool>();
                }
                if (efx != null)
                {
                    efx.Init(transform.position, 2.0f);
                }

                if(null != obj)
                {
                    mEffectObj.transform.position = obj.transform.position;
                }
            }

            PlayerObj mainObj = CoreEntry.gActorMgr.MainPlayer;
            if (null != mainObj)
            {
                //mainObj.ChangeWeapon(PlayerData.Instance.GetWeaponModelID());
                StateParameter param = new StateParameter();
                param.state = ACTOR_STATE.AS_ENTER;
                mainObj.RequestChangeState(param);

            }

        }

        private void MeMorphingFinish()
        {
            ActorObj curObj = CoreEntry.gActorMgr.MainPlayer;
            //var isWild = TaskMgr.IsWildTask;
            //var isAuto = AutoAIRunner.IsInAutoMode_Ex;
            //AutoAIRunner.StopAll();

            CoreEntry.gActorMgr.RemoveActor(curObj);

            int modelID = PlayerData.Instance.GetDressModelID();
            GameObject obj = CoreEntry.gSceneLoader.LoadPlayer(modelID);
            if (null != obj)
            {
                obj.transform.localPosition = curObj.transform.position;
                obj.transform.eulerAngles = curObj.transform.eulerAngles;
                obj.transform.localScale = Vector3.one;
            }

            //if (isAuto)
            //{
            //    AutoAIRunner.Init();
            //    TaskMgr.IsWildTask = isWild;
            //    AutoAIRunner.SetAutoFight(true, null);
            //}
            //else if (TaskMgr.LastClickTaskID > 0)
            //{
            //    AutoAIRunner.Init();
            //    AutoAIRunner.SetAutoTask(true);
            //}

            if (curObj.IsDeath())
            {
                CoreEntry.gActorMgr.MainPlayer.OnDead(0, null, null, null);
            }
            if (((PlayerObj)curObj) != null)((PlayerObj)curObj).HideMoveArrow();
            curObj.RecycleObj();
            curObj.HideBlobShadow();

            //ReSetSkills(1);
            CoreEntry.gActorMgr.MainPlayer.ReBindSkill();
            CoreEntry.gActorMgr.MainPlayer.RebindSkillButton();
        }

        public void PreLoad()
        {
            GameObject effectObj = CoreEntry.gGameObjPoolMgr.Instantiate(mUIEffectPath);
            if (null != effectObj)
            {
                CoreEntry.gGameObjPoolMgr.Destroy(effectObj);
            }

            effectObj = CoreEntry.gGameObjPoolMgr.Instantiate(mPlayerEffectPath);
            if (null != effectObj)
            {
                CoreEntry.gGameObjPoolMgr.Destroy(effectObj);
            }
        }
        #endregion

        #region �������ұ���
        private void OtherMorphingBegin(long serverID, int changeID ,float scale)
        {
            OtherPlayer player = CoreEntry.gActorMgr.GetPlayerActorByServerID(serverID) as OtherPlayer;
            if (null == player)
            {
                return;
            }
            player.transform.localScale = Vector3.one;
            Vector3 pos = player.transform.position;
            Vector3 angle = player.transform.eulerAngles;

            MsgData_sSceneObjectEnterHuman humanStruct = CoreEntry.gSceneObjMgr.GetEntityData(serverID) as MsgData_sSceneObjectEnterHuman;
            if (null == humanStruct)
            {
                return;
            }

            //����
            humanStruct.Ride = 0;
            CoreEntry.gActorMgr.RemoveActor(player);
            player.RecycleObj();

            humanStruct.ChangeID = changeID;
            var other = CoreEntry.gSceneLoader.LoadOtherPlayer(humanStruct);
            other.transform.localScale = new Vector3(scale,scale,scale);
            other.transform.position = pos;
            other.transform.eulerAngles = angle;
        }

        private void OtherMorphFinish(long serverID)
        {
            OtherPlayer player = CoreEntry.gActorMgr.GetPlayerActorByServerID(serverID) as OtherPlayer;
            if (null == player)
            {
                return;
            }            

            MsgData_sSceneObjectEnterHuman humanStruct = CoreEntry.gSceneObjMgr.GetEntityData(serverID) as MsgData_sSceneObjectEnterHuman;
            if (null == humanStruct)
            {
                return;
            }

            Vector3 pos = player.transform.position;
            Vector3 angle = player.transform.eulerAngles;
            CoreEntry.gActorMgr.RemoveActor(player);
            player.RecycleObj();

            humanStruct.ChangeID = 0;
            var other = CoreEntry.gSceneLoader.LoadOtherPlayer(humanStruct);
            other.transform.position = pos;
            other.transform.eulerAngles = angle;
        }
        #endregion
    }
}

