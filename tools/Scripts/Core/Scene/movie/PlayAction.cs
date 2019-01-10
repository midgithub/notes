using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SG;

[System.AttributeUsage(System.AttributeTargets.Method)]
[Hotfix]
public sealed class ParamFunction : System.Attribute
{
    public string Desp { get; set; }
}


[Hotfix]
public class PlayAction : Common_Base{

    public bool IsNeedShowFlimScreen = true;
    public bool IsNeedMovieEndEvent = true;
    
    public bool IsNeedPauseGame = true;
 
    public ActionWrap m_Player1ActionWrap;
    public ActionWrap m_Player2ActionWrap;
    public ActionWrap m_Player3; 
    public ActionWrap m_Monster1ActionWrap;
    public ActionWrap m_Monster2;
    public ActionWrap m_Monster3;
    public ActionWrap m_Monster4;
    public ActionWrap m_Monster5;
    public ActionWrap m_Monster6;
    public ActionWrap m_Monster7;
    public ActionWrap m_Monster8;
    public ActionWrap m_Monster9;
    public ActionWrap m_Monster10;

    public bool IsOverrideTextSkipping = false;
    public Vector3 OffsetTextSkipping;
 
    Animator m_animator;
    SkipTheAnimation skiper = null;
    void Start () {
        m_animator = GetComponent<Animator>();

        if (IsNeedPauseGame || IsNeedShowFlimScreen)
        {
            IsNeedMovieEndEvent = true;
        }

        if (IsNeedMovieEndEvent)
        {
            //在动画最后加一个MovieEnd事件
            AnimationEventFromScript aefs = GetARequiredComponent<AnimationEventFromScript>(gameObject);
            if(aefs!=null)
             aefs.AddAnimationFinishedEvent<PlayAction>("MovieEnd");

            skiper = gameObject.AddComponent<SkipTheAnimation>();
            if (skiper != null)
            {
                skiper.SetPos(IsOverrideTextSkipping, OffsetTextSkipping);
            }
        }

       
	}

    //bool bAutoFight = false;
    bool needPauseGame = false;
    ActionWrap[] m_playerWraps;

    void OnEnable()
    {
        //bAutoFight = CoreEntry.gGameMgr.AutoFight; 

        //CoreEntry.gEventMgr.AddListener(GameEvent.GE_STORY_Guide_System_fighting_finish, OnDialogEnd );


        //战斗内
        if (CoreEntry.InFightScene)
        {
           
            if (IsNeedShowFlimScreen)
            { 
                needPauseGame = true;
            }

            if (IsNeedPauseGame)
            {
                needPauseGame = true;
            }

            if (needPauseGame)
            {
                CoreEntry.gGameMgr.pauseGame();
            }

            //绑定玩家 到动画
            //m_playerWraps = new ActionWrap[] { m_Player1ActionWrap, m_Player2ActionWrap, m_Player3 };
            //for (int i = 0; i < CoreEntry.gTeamMgr.playerList.Count; i++)
            //{
            //    GameObject go = CoreEntry.gTeamMgr.playerList[i];
            //    PlayerAgent angent = go.GetComponent<PlayerAgent>();
            //    if (angent != null)
            //    {
            //        angent.PlayerObj.StopMove(false);
            //    }
            //    //组队老板可能会加载6个武将
            //    if (i < m_playerWraps.Length)
            //    {
            //        if (m_playerWraps[i] != null)
            //        {
            //            m_playerWraps[i].boundModel(go);
            //        }
            //    }
            //}

        }

    }
    
    void OnDisable()
    {
        //CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_STORY_Guide_System_fighting_finish, OnDialogEnd);


        //战斗内
        if (CoreEntry.InFightScene )
        {
            if (IsNeedShowFlimScreen)
            { 
            }

            if (needPauseGame)
            {
                if (CoreEntry.GameStart == false)
                {
                    CoreEntry.gGameMgr.resumeGame();// = true;
                }
            }

            releasePlayers();
        }
    }

    void OnDialogEnd(GameEvent ge, EventParameter parameter)
    {
        if (skiper != null)
        {
            skiper.ShowTextIfHas(true);
        }

        m_animator.speed = 1;
        releasePlayers(); 
    }

    void releasePlayers()
    {
        //解绑玩家
        //for (int i = 0; i < CoreEntry.gTeamMgr.playerList.Count; i++)
        //{
        //    GameObject go = CoreEntry.gTeamMgr.playerList[i];
        //    //组队老板可能会加载6个武将
        //    if (i < m_playerWraps.Length)
        //    {
        //        if (m_playerWraps[i] != null)
        //        {
        //            m_playerWraps[i].releaseModel();
        //        }
        //    }
        //}


        //CoreEntry.gGameMgr.AutoFight = true;
        //CoreEntry.gGameMgr.AutoFight = false ;


        //CoreEntry.gGameMgr.AutoFight = bAutoFight; 

    }


    int  m_triggerNextWaveTimes = 0 ; 
    public void MovieEnd()
    {
        TriggerRoot root = NGUITools.FindInParents<TriggerRoot>(gameObject);

        if (root != null && root.isTriggerNextWave)
        {
            if (m_triggerNextWaveTimes <= 0)
            {
                CoreEntry.gGameMgr.TriggerNextWave();
                m_triggerNextWaveTimes = 1;
            }
        }

        gameObject.SetActive(false);
    }


    public void showDialog(int id)
    {

        if (!CoreEntry.InFightScene)
        {
            return; 
        }

        //跳过动画，也跳过对话
        if (skiper != null )
        {
            if (skiper.IsSkiped)
            {
                return;
            }
            else
            {
                skiper.ShowTextIfHas(false);
            }
        }

        m_animator.speed = 0;

        EventParameter msg = EventParameter.Get();
        msg.intParameter1 = id;
        //CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_STORY_Guide_System_fighting, msg);

    }

    public void player1_stand()
    {
        if (m_Player1ActionWrap != null)
        {
            m_Player1ActionWrap.PlayAction("stand");
        }
    }
 

    public void player1_run()
    {
        if (m_Player1ActionWrap != null)
        {
            m_Player1ActionWrap.PlayAction("run");
        }
    }

    public void player1_die()
    {
        if (m_Player1ActionWrap != null)
        {
            m_Player1ActionWrap.PlayDie001();
        }
    }

    public void player1_Attack001()
    {
        if (m_Player1ActionWrap != null)
        {
            m_Player1ActionWrap.PlayAttack001();
        }
    }



    public void player2_stand()
    {
        if (m_Player2ActionWrap != null)
        {
            m_Player2ActionWrap.PlayAction("stand");
        }
    }

 
    public void Player2_run()
    {
        if (m_Player2ActionWrap != null)
        {
            m_Player2ActionWrap.PlayAction("run");
        }
    }

    public void Player2_die()
    {
        if (m_Player2ActionWrap != null)
        {
            m_Player2ActionWrap.PlayDie001();
        }
    }

    public void Player2_Attack001()
    {
        if (m_Player2ActionWrap != null)
        {
            m_Player2ActionWrap.PlayAttack001();
        }
    }

    public void Monster1_run()
    {
        if (m_Monster1ActionWrap != null)
        {
            m_Monster1ActionWrap.PlayAction("run");
        }
    }

    public void Monster1_die()
    {
        if (m_Monster1ActionWrap != null)
        {
            m_Monster1ActionWrap.PlayDie001();
        }
    }

    public void Monster1_Attack001()
    {
        if (m_Monster1ActionWrap != null)
        {
            m_Monster1ActionWrap.PlayAttack001();
        }
    }


    public void Monster1_stand()
    {
        if (m_Monster1ActionWrap != null)
        {
            m_Monster1ActionWrap.PlayAction("stand");
        }
    }


    public void player1Action(string actionName)
    {
        if (m_Player1ActionWrap != null)
        {
            m_Player1ActionWrap.PlayAction(actionName);
        }
    }
    public void player2Action(string actionName)
    {
        if (m_Player2ActionWrap != null)
        {
            m_Player2ActionWrap.PlayAction(actionName);
        }
    }
    public void player3Action(string actionName)
    {
        if (m_Player3  != null)
        {
            m_Player3 .PlayAction(actionName);
        }
    }

    public void Monster1Action(string actionName)
    {
        if (m_Monster1ActionWrap != null)
        {
            m_Monster1ActionWrap.PlayAction(actionName);
        }
    }
    public void Monster2Action(string actionName)
    {
        if (m_Monster2 != null)
        {
            m_Monster2.PlayAction(actionName);
        }
    }
    public void Monster3Action(string actionName)
    {
        if (m_Monster3 != null)
        {
            m_Monster3.PlayAction(actionName);
        }
    }
    public void Monster4Action(string actionName)
    {
        if (m_Monster4 != null)
        {
            m_Monster4.PlayAction(actionName);
        }
    }
    public void Monster5Action(string actionName)
    {
        if (m_Monster5 != null)
        {
            m_Monster5.PlayAction(actionName);
        }
    }
    public void Monster6Action(string actionName)
    {
        if (m_Monster6 != null)
        {
            m_Monster6.PlayAction(actionName);
        }
    }

    public void Monster7Action(string actionName)
    {
        if (m_Monster7 != null)
        {
            m_Monster7.PlayAction(actionName);
        }
    }

    public void Monster8Action(string actionName)
    {
        if (m_Monster8 != null)
        {
            m_Monster8.PlayAction(actionName);
        }
    }

    public void Monster9Action(string actionName)
    {
        if (m_Monster9 != null)
        {
            m_Monster9.PlayAction(actionName);
        }
    }

    public void Monster10Action(string actionName)
    {
        if (m_Monster10 != null)
        {
            m_Monster10.PlayAction(actionName);
        }
    }


    //public void Monster1_idle()
    //{
    //    if (m_Monster1ActionWrap != null)
    //    {
    //        m_Monster1ActionWrap.PlayIdle();
    //    }
    //}




    [System.Serializable]
[Hotfix]
    public class PA_Param
    {
        public string ParamName;
        /// <summary>
        /// 描述信息
        /// </summary>
        public string Desp;
        public List<GameObject> ObjectList = new List<GameObject>();
        public List<float> FloatList = new List<float>();
        public List<string> StringList = new List<string>();
        public List<int> IntList = new List<int>();
        public List<Color> ColorList = new List<Color>();

        public GameObject GetGameObject(int pos)
        {
            if (ObjectList.Count > pos)
            {
                return ObjectList[pos];
            }

            return null;
        }


        public float GetFloat(int pos)
        {
            if (FloatList.Count > pos)
            {
                return FloatList[pos];
            }

            return 0.0f;
        }

        public int GetInt(int pos)
        {
            if (IntList.Count > pos)
            {
                return IntList[pos];
            }

            return 0;
        }

        public string GetString(int pos)
        {
            if (StringList.Count > pos)
            {
                return StringList[pos];
            }

            return "";
        }

        public Color GetColor(int pos)
        {
            if (ColorList.Count > pos)
            {
                return ColorList[pos];
            }

            return Color.white;
        }
    };

    public List<PA_Param> ParamList = new List<PA_Param>();

    public PA_Param FindParam(string paramName, bool reserved = true)
    {
        for (int i = 0; i < ParamList.Count; ++i)
        {
            if (ParamList[i].ParamName == paramName)
            {
                return ParamList[i];
            }
        }

        LogMgr.ErrorLog("can not find param with name: {0}, please check",paramName);
        return null;
    }

#if UNITY_EDITOR
    [ParamFunction(Desp = "object 0代表震动\r\n的中心点物体,\r\n object 1代表指定的摄像机，\r\n Float 0代表震动的范围。\r\n 特例：当中心点物体没有 或者 \r\n震动范围不设置时，\r\n代表整个震动范围无限。\r\n float 1代表振幅")]
#endif
    public void CameraShake(string paramName)
    {
        PA_Param param= FindParam(paramName);
        if (param == null)
        {
            return;
        }
        GameObject center = param.GetGameObject(0);
     
        //float radius = param.GetFloat(0);
        bool shouldStart = false;
      
        if (center != null  )
        {
            //if( CoreEntry.gTeamMgr.MainPlayer != null )
            //{
            //    if( radius <= 0 )
            //    {
            //        shouldStart = true;
            //    }
            //    else if ((CoreEntry.gTeamMgr.MainPlayer.transform.position - center.transform.position).magnitude <= radius)
            //    {
            //        shouldStart = true;
            //    }
            //}
        }
        else
        {
            shouldStart = true;
        }

        if( shouldStart )
        {
            Camera cam = GetCameraFromParam(param, 1);
            if (cam != null)
            {
                float scale = param.GetFloat(1);
                if (scale <= 0f)
                {
                    scale = 1f;
                }
                CameraEffect effect = GetARequiredComponent<CameraEffect>(cam.gameObject);
                if (effect != null)
                {
                    effect.Scale = scale;
                    effect.ShakeStart();
                }
            }
        }

    }

    const int DefaultLayer = 0;
    public void Replay()
    {
        Animator ani = m_animator;
        if (ani != null)
        {
            AnimatorStateInfo asi = ani.GetCurrentAnimatorStateInfo(DefaultLayer);

            ani.Play(asi.fullPathHash, DefaultLayer, 0f);
           
        }
       
    }

    public void SetPlayingSpeed(float v)
    {
        //Animator ani = m_animator;
        //if (ani != null)
        //{
        //    AnimatorStateInfo asi = ani.GetCurrentAnimatorStateInfo(DefaultLayer);


        //    ani.speed = v;

        //}

        CoreEntry.gTimeMgr.TimeScale = v;
    }

#if UNITY_EDITOR
    [ParamFunction(Desp = "object 0 为制定\r\n摄像机gameobject，\r\n float 0 位时间长度，\r\n color 0为要过渡到的颜色值")]
#endif
    public void RandomDelayReplay(string paramName)
    {
        PA_Param param = FindParam(paramName);
        if (param == null)
        {
            return;
        }

        float min = param.GetFloat(0);
        float max = param.GetFloat(1);
        if (max <= min)
        {
            Replay();
        }
        else
        {
            float v = Random.Range(min,max);
            Invoke("Replay",v);
        }
    }


    public void BeginFilmEffect()
    {
        

        SetStoryBackgroundHight(false);
    }

    public void EndFilmEffect()
    {
      

        SetStoryBackgroundHight(true);
    }

    #region Screen Color
    //Color destColor;
   // Color curColor;
    //FadeEffect fadeFffect = null;
   // float fadeDuration = 1.0f;

    public Camera GetCameraFromParam(PA_Param param, int index)
    {
        Camera cam = null;
        GameObject obj = param.GetGameObject(index);
        if (obj != null)
        {
            cam = obj.GetComponent<Camera>();
        }

        if (cam == null)
        {
            cam = Camera.main;
        }

        return cam;
    }

    [ParamFunction(Desp = "object 0 为制定摄\r\n像机gameobject，\r\n float 0 位时间长度，\r\n color 0为要过渡到的颜色值")]
    public void TweenColor(string paramName)
    {
        PA_Param param = FindParam(paramName);
        if (param == null)
        {
            return;
        }

        Camera cam = GetCameraFromParam(param, 0);
        
         
        if (cam != null)
        {
            FadeEffect fadeEffect = GetARequiredComponent<FadeEffect>(cam.gameObject);
            if (fadeEffect == null) return;

            Color destColor = param.GetColor(0);
            Color curColor = fadeEffect.blendColor;
            float duration = param.GetFloat(0);
            TweenColorInternal(fadeEffect, curColor, destColor, duration);
        }
    }


    public void FadeIn(float duration)
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
            FadeEffect fadeEffect = GetARequiredComponent<FadeEffect>(cam.gameObject);
            if (fadeEffect == null) return;

            Color curColor = fadeEffect.blendColor;
            Color destColor = Color.white;

            TweenColorInternal(fadeEffect, curColor, destColor, duration);

            //AEffect();
            //GetARequiredComponent<DepthEffect>(cam.gameObject);
           
        }
    }

    public void FadeOut(float duration)
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
            FadeEffect fadeEffect = GetARequiredComponent<FadeEffect>(cam.gameObject);
            if (fadeEffect == null) return;
            Color curColor = fadeEffect.blendColor;
            Color destColor = Color.black;


            TweenColorInternal(fadeEffect, curColor, destColor, duration);
        }
    }


    [ParamFunction(Desp = "object 0 为制定摄像机gameobject，\r\n float 0 位时间长度")]
    public void FadeInToGivenCamera(string paramName)
    {
        PA_Param param = FindParam(paramName);
        if (param == null)
        {
            return;
        }

        Camera cam = GetCameraFromParam(param, 0);
        if (cam != null)
        {
            FadeEffect fadeEffect = GetARequiredComponent<FadeEffect>(cam.gameObject);
            if (fadeEffect == null) return;
            Color curColor = fadeEffect.blendColor;
            Color destColor = Color.white;
            float duration = param.GetFloat(0);

            TweenColorInternal(fadeEffect, curColor, destColor, duration);

        }
    }

    [ParamFunction(Desp = "object 0 为制定摄\r\n像机gameobject，\r\n float 0 位时间长度")]
    public void FadeOutToGivenCamera(string paramName)
    {
        PA_Param param = FindParam(paramName);
        if (param == null)
        {
            return;
        }

        Camera cam = GetCameraFromParam(param, 0);
        if (cam != null)
        {
            FadeEffect fadeEffect = GetARequiredComponent<FadeEffect>(cam.gameObject);
            if (fadeEffect == null) return;

            Color curColor = fadeEffect.blendColor;
            Color destColor = Color.black;
            float duration = param.GetFloat(0);

            TweenColorInternal(fadeEffect, curColor, destColor, duration);
        }
    }

    public void TweenColorInternal(FadeEffect fadeEffect,Color curColor, Color destColor, float duration)
    {
        StopCoroutine(FadeScreen(fadeEffect, curColor, destColor, duration));
        if (duration == 0)
        {
            if (fadeEffect != null)
            {
                fadeEffect.blendColor = destColor;
            }
        }
        else
        {
            StartCoroutine(FadeScreen(fadeEffect, curColor, destColor, duration));
        }
    }

    IEnumerator FadeScreen(FadeEffect fadeEffect, Color curColor,Color destColor,float fadeDuration)
    {
        float timer = 0f;
        int times = 20;
        float deltaTime = 1f / times;
        while (timer < 1)
        {
            timer += deltaTime;
            curColor = Color.Lerp(curColor, destColor, timer );
            if (fadeEffect != null)
            {
                fadeEffect.blendColor = curColor;
            }
            yield return new WaitForSeconds(fadeDuration * deltaTime);
        }
    }

    public void ShowUI(int v)
    {
       

    }

#endregion


    public void PlayAnimationWithPath(string path)
    {
        Object orgObj = CoreEntry.gResLoader.Load(path);
        if(orgObj!=null)
        {
            PlayAnimationInPrefab(orgObj);
            orgObj = null;
        }

        //Resources.UnloadUnusedAssets();

   }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="orgObj"></param>
    /// <param name="reserved">仅仅不让他在animation的event窗口出现</param>
    protected void PlayAnimationInPrefab(Object orgObj,bool reserved = true)
    {
        if (orgObj != null)
        {
            Transform tr = transform;
            while (tr.parent != null)
            {
                tr = tr.parent;
                if (tr.name == "ObjectRoot")
                {
                    break;
                }
            }

            GameObject obj = Instantiate(orgObj) as GameObject;

            if (obj != null)
            {
                obj.transform.parent = tr;
                //不设置true，这样如果prefab不是active状态，可以默认不会播放该动画，方便测试
                obj.SetActive(true);
            }
        }
    }

#if UNITY_EDITOR
    [ParamFunction(Desp = "object 0 为需要播放动\r\n画的prefab文件")]
#endif
    public void PlayAnimation(string paramName)
    {
        PA_Param param = FindParam(paramName);
        if (param == null)
        {
            return;
        }

        GameObject obj = param.GetGameObject(0);
        PlayAnimationInPrefab(obj);
    }

    public void HideTeamPlayers()
    {
        //for (int i = 0; i < CoreEntry.gTeamMgr.playerList.Count; ++i)
        //{
        //    ActorObj actorbase =  CoreEntry.gTeamMgr.playerList[i].GetComponent<ActorObj>();
        //    if (actorbase != null)
        //    {
        //        actorbase.gameObject.SetActive(false);
        //        actorbase.DisableInWorld();
        //    }
        //}
    }

    public void ShowTeamPlayers()
    {
        //for (int i = 0; i < CoreEntry.gTeamMgr.playerList.Count; ++i)
        //{
        //    ActorObj actorbase = CoreEntry.gTeamMgr.playerList[i].GetComponent<ActorObj>();
        //    if (actorbase != null)
        //    {
        //        actorbase.gameObject.SetActive(true);
        //        actorbase.EnableInWorld();

        //        StateParameter param = new StateParameter();
        //        param.state = ACTOR_STATE.AS_STAND;
        //        actorbase.RequestChangeState(param);
                
        //    }
        //}
    }

    public void Pause(float v)
    {
        if (m_animator != null)
        {
            m_animator.speed = 0f;
            CancelInvoke("Resume");
            Invoke("Resume",v);
        }
    }

    public void Resume()
    {
        if (m_animator != null)
        {
            m_animator.speed = 1f;
        }
    }

    public void AEffect()
    {
        
        GameObject root= GameObject.FindGameObjectWithTag("Passable");
        if (root != null)
        {

            ApplyChild(root.transform);

        }
    }

#if UNITY_EDITOR
    [ParamFunction(Desp = "object中为所有需\r\n要参与动画的物体，\r\n string 0 代表播放动画的名称,\r\n float 0 代表朝向最小值，\r\n float 1 代表朝向最大值")]
#endif
    public void RandomFaceActorPlay(string paramName)
    {
        PA_Param param = FindParam(paramName);
        if (param == null)
        {
            return;
        }

        float min = param.GetFloat(0);
        float max = param.GetFloat(1);
        if (min < max)
        {
            for (int i = 0; i < param.ObjectList.Count; ++i)
            {
                float v = Random.Range(min, max);
                Transform tr = param.ObjectList[i].GetComponent<Transform>();
                if (tr != null)
                {
                    tr.Rotate(Vector3.up, v);
                }
            }
        }
        
        ActorPlay(paramName);
        
    }

#if UNITY_EDITOR
    [ParamFunction(Desp = "object中为所有需\r\n要参与动画的物体，\r\n string 0 代表播放动画的名称")]
#endif
    public void ActorPlay(string paramName)
    {
        PA_Param param = FindParam(paramName);
        if (param == null)
        {
            return;
        }

        string actionName = param.GetString(0);
        if( actionName != "" )
        {

            for (int i = 0; i < param.ObjectList.Count; ++i)
            {
                Animation anim = param.ObjectList[i].GetComponent<Animation>();
                if (anim != null)
                {
                    anim.Play(actionName);
                }
            }
        }
    }

#if UNITY_EDITOR
    [ParamFunction(Desp = "object 0 中为所有需要\r\n参与动画的物体的父物体，\r\n string 0 代表播放动画的名称,\r\n float 0 代表朝向最小值，\r\nfloat 1 代表朝向最大值")]
#endif
    public void RandomFaceActorPlayWithRoot(string paramName)
    {
        PA_Param param = FindParam(paramName);
        if (param == null)
        {
            return;
        }

        float min = param.GetFloat(0);
        float max = param.GetFloat(1);
        if (min < max)
        {
            GameObject root = param.GetGameObject(0);
            if (root == null) return;
            Transform[] list = root.GetComponentsInChildren<Transform>();
            for (int i = 0; i < list.Length; ++i)
            {
                float v = Random.Range(min, max);
                if (list[i] != null)
                {
                    list[i].Rotate(Vector3.up, v);
                }
            }
        }
    }

#if UNITY_EDITOR
    [ParamFunction(Desp = "object 0 中为所有需要\r\n参与动画的物体的父物体，\r\n string 0 代表播放动画的名称")]
#endif
    public void ActorPlayWithRoot(string paramName)
    {
        PA_Param param = FindParam(paramName);
        if (param == null)
        {
            return;
        }

        string actionName = param.GetString(0);
        if (actionName != "")
        {
            GameObject root = param.GetGameObject(0);
            if (root == null) return;

            Animation[] animList = root.GetComponentsInChildren<Animation>();
            for (int i = 0; i < animList.Length; ++i)
            {
                if (animList[i] != null)
                {
                    animList[i].Play(actionName);
                }
            }
        }
    }

    public void ApplyChild(Transform tr)
    {
        for (int i = 0; i < tr.childCount; ++i)
        {
            Transform t = tr.GetChild(i);
            if (t != null)
            {
                ApplyChild(t);
            }
        }

        if (tr.GetComponent<Renderer>() != null)
        {
            if (tr.GetComponent<Renderer>().material.HasProperty("_Color"))
            {
                tr.GetComponent<Renderer>().material.color =  Color.blue;
            }

            
        }
    }
    ////新手引导关进入场景剧情 
    //public void guideStory()
    //{
    //    //战斗前
    //    if (CoreEntry.gGameMgr.GuidScence)
    //    {
    //        CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_STORY_TIGGER_FightFront, null);
    //    }

    //}

    //设置剧情背景亮度
    public void SetStoryBackgroundHight(bool v)
    {
        
    }

#if UNITY_EDITOR
    [ParamFunction(Desp = "object 0 中为需要转向的物体\r\n float 0旋转的时长\n int 0 注视种类")]
#endif
    public void TurnToBoss(string paramName)
    {
        PA_Param param = FindParam(paramName);
        if (param == null)
        {
            return;
        }

        GameObject obj = param.GetGameObject(0);
        if (obj != null)
        {
            GameObject tmpBoss = GetBoss();
        
            Transform chestSocket = tmpBoss.GetComponent<ActorObj>().GetChildTransform("E_Spine");
            float time = param.GetFloat(0);
            StartCoroutine(TuningToObject(obj, chestSocket, time));
          
        }
    }

    public void StopTurnToBoss()
    {
        StopCoroutine("LookingAtBoss");
    }

    GameObject boss = null;
    public void SetBoss(GameObject v)
    {
        boss = v;
    }

    GameObject GetBoss()
    {
        GameObject tmpBoss = boss;
        if (tmpBoss == null)
        {
            tmpBoss = CoreEntry.gGameMgr.GetBossActorObject();
        }

        return tmpBoss;
    }

    public IEnumerator TuningToObject(GameObject obj, Transform target, float duration)
    {
        Quaternion start = obj.transform.localRotation;
        obj.transform.LookAt(target.position);
        Quaternion end = obj.transform.localRotation;
        obj.transform.localRotation = start;

        float timer = 0f;
        int times = 20;
        float deltaTime = 1f / times;
        while (timer < 1)
        {
            timer += deltaTime;
            obj.transform.localRotation = Quaternion.Slerp(start, end, timer);
            yield return new WaitForSeconds(duration * deltaTime);
        }
        StartCoroutine(LookingAtObject(obj, target));
    }

    public IEnumerator LookingAtObject(GameObject obj, Transform target)
    {
        if (target != null)
        {
            while (true)
            {
                obj.transform.LookAt(target.position);
                yield return null;
            }
        }
    }

#if UNITY_EDITOR
    [ParamFunction(Desp = "object 0 是需要还原的物体")]
#endif
    public void ResetRotation(string paramName)
    {
        PA_Param param = FindParam(paramName);
        if (param == null)
        {
            return;
        }

        GameObject obj = param.GetGameObject(0);
        if (obj != null)
        {
            obj.transform.localRotation = Quaternion.identity;
        }
    }

    public void StopBossAnimation(int v)
    {
        GameObject tmpBoss = GetBoss();
        ActorAnimation aa = GetARequiredComponent<ActorAnimation>(tmpBoss);
        if (aa != null)
        {
            aa.DisableSelfWhenOnceActionFinished = true;
            if (v == 0)
            {
                aa.IsStop = false;
            }
            else
            {
                aa.enabled = true;
                aa.UpdateToCurrentAnImation();
                aa.IsStop = true;
            }
        }
    }

    public void SetMainCameraFollowPlayer(int v)
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
			var cameraComponent = Camera.main.GetComponent<CameraFollow>();
			if(cameraComponent==null)
				return;
            if (v == 0)
            {
                Camera.main.GetComponent<CameraFollow>().enabled = false;
            }
            else
            {
                Camera.main.GetComponent<CameraFollow>().enabled = true;
            }
        }
    }
         
}

