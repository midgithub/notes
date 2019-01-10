
using XLua;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

namespace SG
{

    //主相机必须带有的
[Hotfix]
    public class CameraFollow : CameraBase
    {
        //protected Transform transform;
        public Transform m_target = null;

        //距离跟随目标的距离
        public float m_initDistance = 5f;
        public float m_distance = 5f;
        public float m_minDistance = 5f;

        //距离跟随目标的高度
        public float m_height = 10f;
        private float HEIGHTSLOPE = 11f;

        //和目标的偏移角度
        public float m_rotationAngle = 50f;
        public float m_rotationAngleX = 0;

        //public bool m_cameraShake = false;

        public bool m_horizontal = false;

        private EventMgr m_eventMgr;

        private bool m_bActive = false;

        Material ResaveMaterial = null;
        Material TransparentMaterial = null;

        GameObject m_GameObj = null;

        GameObject m_audioListerObj = null;

        //private bool m_bTouching = false;

        Dictionary<int, Vector2> m_lastFingerInfoDict = new Dictionary<int, Vector2>();
        Vector2 m_centerPos = new Vector2();
        List<Vector2> m_fingerPos = new List<Vector2>();
        List<Vector2> m_fingerMovePos = new List<Vector2>();

        Camera mCamera;

        /// <summary>
        /// 当前场景旋转。
        /// </summary>
        public static float CurRotationAngle = 0;

        /// <summary>
        /// 当前场景高度。
        /// </summary>
        public static float CurHeight = 0;

        void Awake()
        {
            m_eventMgr = CoreEntry.gEventMgr;
            mCamera = GetComponent<Camera>();
            RegisterEvent();
        }

        // Use this for initialization
        void Start()
        {
            //transform = this.transform;
            m_distance = m_initDistance;
            _nLayerMask = 1 << LayerMask.NameToLayer("tree");


            //创建音效监听器
            m_audioListerObj = new GameObject();
            m_audioListerObj.name = "audioListener";
            m_audioListerObj.transform.parent = transform;
            m_audioListerObj.transform.localRotation = new Quaternion(0, 0, 0, 0);
            m_audioListerObj.AddComponent<AudioListener>();



            //TransparentMaterial = new Material(Shader.Find(("SGShader/DynamicTransparentDiffuse")));

            //第一个条件是是否开启显示帧数或者调试模式
            if (true && this.gameObject.GetComponent<ShowFps>() == null)
            {
                this.gameObject.AddComponent<ShowFps>();
            }
            //GameObject Player = GameObject.Find("Player");

            //if (Player)
            //{
            //    moveToTargetBySpeed(Player.transform);
            //}

            if (ProfilerData.EnableProfilerSwitch)
            {
                this.gameObject.AddComponent<ProfilerGUI>();
            }
            CurRotationAngle = m_rotationAngle;
            CurHeight = m_height;
        }

        bool m_ignoreTimeScale = false;

        public bool IgnoreTimeScale
        {
            get { return m_ignoreTimeScale; }
            set { m_ignoreTimeScale = value; }
        }



        // Update is called once per frame
        void LateUpdate()
        {
            CameraUpdate();
        }

        void CameraUpdate()
        {
            //if (!m_target)
            //{
            //    return;
            //}

            if (m_cameraShake)
            {
                return;
            }

            if (m_cameraZoom)
            {
                return;
            }
            
            if (!m_bActive)
            {
                UpdateMainCameraTransform();
            }

            //障碍物变半透明检测没必要每帧都做 Physics.RaycastAll会返回一个数组对象 有GC
            m_CheckObstacleCount -= Time.deltaTime;
            if (m_CheckObstacleCount <= 0)
            {
                m_CheckObstacleCount = CheckObstacleGap;
                CheckObstacle();
            }            
        }

        /// <summary>
        /// 障碍物检测间隔。
        /// </summary>
        public static float CheckObstacleGap = 0.5f;

        /// <summary>
        /// 障碍物检测计数。
        /// </summary>
        float m_CheckObstacleCount = 0;

        //检测模型和射线
        void CheckModelTran()
        {
            //Vector3 vDir = Vector3.Normalize(getTargetPosition() - transform.position);
            if (CoreEntry.gCameraMgr.MainCamera == null)
            {
                return;
            }
            Ray ray = new Ray(transform.position, CoreEntry.gCameraMgr.MainCamera.transform.forward);

            int nLayerMask = 1 << LayerMask.NameToLayer("tree");

            RaycastHit rayCastHit;

            if (Physics.Raycast(ray, out rayCastHit, 1000.0f, nLayerMask))
            {
                GameObject obj = rayCastHit.transform.gameObject;
                if (m_GameObj != obj)
                {
                    m_GameObj = obj;
                    MeshRenderer render = obj.GetComponent<MeshRenderer>();
                    if (render != null)
                    {
                        Texture texture = render.material.mainTexture;
                        ResaveMaterial = render.material;
                        render.material = TransparentMaterial;
                        render.material.SetTexture(0, texture);
                    }
                }
            }
            else
            {
                if (m_GameObj != null)
                {
                    MeshRenderer mesh = m_GameObj.GetComponent<MeshRenderer>();
                    if (mesh != null)
                        mesh.material = ResaveMaterial;
                    //m_GameObj.collider.enabled = true;
                    m_GameObj = null;

                }

            }

        }

        // 所有障碍物的Renderer数组  
        private List<Renderer> _ObstacleCollider = new List<Renderer>();

        // 临时接收，用于存储  
        private Renderer _tempRenderer;

        //保存shader
        private Dictionary<Renderer, string> rendererDict = new Dictionary<Renderer, string>();

        private int _nLayerMask;

        //check Obstacle
        void CheckObstacle()
        {
            if (CoreEntry.gCameraMgr.MainCamera == null || m_target == null)
            {
                return;
            }

            Ray ray = new Ray(m_target.position, -CoreEntry.gCameraMgr.MainCamera.transform.forward);
            RaycastHit[] hit;
            hit = Physics.RaycastAll(ray, 1000.0f, _nLayerMask);

            //  如果碰撞信息数量大于0条 
            if (hit.Length > 0)
            {
                for (int i = 0; i < hit.Length; i++)
                {
                    _tempRenderer = hit[i].collider.gameObject.GetComponent<Renderer>();
                    if (rendererDict.ContainsKey(_tempRenderer))
                    {
                        return;
                    }

                    rendererDict[_tempRenderer] = _tempRenderer.material.shader.name;
                    //更改shader
                    ChangeMaterial(_tempRenderer);

                    if (_ObstacleCollider.Contains(_tempRenderer))
                    {
                        return;
                    }
                    _ObstacleCollider.Add(_tempRenderer);

                    float fadeIn = 1;
                    Tween t = DOTween.To(() => fadeIn, x => fadeIn = x, 0f, 1);
                    t.OnUpdate(() => SetMaterialsAlpha(_tempRenderer, fadeIn));

                }


            }// 恢复障碍物透明度为1  
            else
            {

                if (_ObstacleCollider.Count > 0 && _ObstacleCollider != null)
                {
                    for (int i = 0; i < _ObstacleCollider.Count; i++)
                    {
                        float fadeOut = 0;
                        _tempRenderer = _ObstacleCollider[i];
                        Tween t = DOTween.To(() => fadeOut, x => fadeOut = x, 1f, 1);
                        t.OnUpdate(() => SetMaterialsAlpha(_tempRenderer, fadeOut));
                        string shaderName = rendererDict[_tempRenderer];
                        t.OnComplete(() =>
                        {
                            bool stillNeedFade = rendererDict.ContainsKey(_tempRenderer);
                            if (!stillNeedFade)
                            {
                                ResetMaterial(_tempRenderer, shaderName);
                            }
                        });

                    }
                    _ObstacleCollider.Clear();
                    rendererDict.Clear();
                }
            }
        }


        void ChangeMaterial(Renderer render)
        {
            if (null == render)
                return;
            if (null == render.material)
                return;
            if (render.material.shader.name.Contains("Transparent/Diffuse"))
                return;

            render.material.shader = Shader.Find("Transparent/Diffuse");
        }

        // 修改障碍物的透明度  
        private void SetMaterialsAlpha(Renderer _renderer, float Transpa)
        {
            //Debug.Log("SetMaterialsAlpha " + Transpa);
            // 一个游戏物体的某个部分都可以有多个材质球  
            int materialsCount = _renderer.materials.Length;

            if (_renderer.material.shader.name != "Legacy Shaders/Transparent/Diffuse"
                && _renderer.material.shader.name != "Legacy Shaders/Transparent/Cutout/Diffuse")
            {
                return;
            }
            for (int i = 0; i < materialsCount; i++)
            {
                // 获取当前材质球颜色  
                Color color = _renderer.materials[i].color;

                // 设置透明度（0--1）  
                color.a = Transpa;

                // 设置当前材质球颜色（游戏物体上右键SelectShader可以看见属性名字为_Color）  
                _renderer.materials[i].SetColor("_Color", color);
            }
        }

        private void ResetMaterial(Renderer tempRenderer, string name)
        {
            tempRenderer.material = new Material(tempRenderer.material);
            tempRenderer.material.shader = Shader.Find(name);
        }

        // 检测点是否在有效区域内
        bool IsPointInAvailableArea(Vector2 pos)
        {
            bool bIn = false;
            if (pos.x > Screen.width / 2)
            {
                bIn = true;
            }

            return bIn;
        }

        void UpdateInputCameraDistance()
        {
            float fInputChangeScale = QueryInputChangeScale();
            if ((fInputChangeScale > 0 && m_distance > m_minDistance) || (fInputChangeScale < 0 && m_distance < m_initDistance))
            {
                m_distance -= fInputChangeScale / 100;
            }
        }


        float QueryInputChangeScale()
        {
            m_fingerPos.Clear();
            m_fingerMovePos.Clear();
            m_centerPos = new Vector2(0.0f, 0.0f);
            int iFingerCount = 0;

            int count = Input.touchCount;
            for (int i = 0; i < count; i++)
            {
                Touch touch = Input.GetTouch(i);
                // 初始点击无效位置被排除
                if (TouchPhase.Began == touch.phase && IsPointInAvailableArea(touch.position) == false)
                {
                    continue;
                }

                Vector2 lastTouchPos;
                // 初始点击排除的点始终无效
                if (TouchPhase.Began != touch.phase && m_lastFingerInfoDict.TryGetValue(touch.fingerId, out lastTouchPos) == false)
                {
                    continue;
                }

                m_centerPos = m_centerPos + touch.position;
                iFingerCount++;

                //Debug.LogError("touch finger index : "+ i.ToString() + ", finger id : " + touch.fingerId.ToString()
                //               + ", phase : " + touch.phase.ToString());
                // 有效初始点击
                if (TouchPhase.Began == touch.phase && m_lastFingerInfoDict.TryGetValue(touch.fingerId, out lastTouchPos) == false)
                {
                    m_lastFingerInfoDict.Add(touch.fingerId, touch.position);
                    //Debug.LogError(" add finger! finger Count : " + m_lastFingerInfoDict.Count.ToString());
                }
                else if (m_lastFingerInfoDict.TryGetValue(touch.fingerId, out lastTouchPos) == true)
                {
                    if (TouchPhase.Moved == touch.phase)        //有效手指移动
                    {
                        Vector2 movePos = touch.position - lastTouchPos;
                        m_fingerMovePos.Add(movePos);
                        m_fingerPos.Add(touch.position);
                        m_lastFingerInfoDict[touch.fingerId] = touch.position;
                    }
                    else if (TouchPhase.Ended == touch.phase || TouchPhase.Canceled == touch.phase) // 手指松开
                    {
                        m_lastFingerInfoDict.Remove(touch.fingerId);
                        //Debug.LogError(" remove finger! finger Count : " + m_lastFingerInfoDict.Count.ToString());
                    }
                }
            }


            m_centerPos = new Vector2(m_centerPos.x / iFingerCount, m_centerPos.y / iFingerCount);
            float fInputChangeScale = 0.0f;
            for (int i = 0; i < m_fingerMovePos.Count; ++i)
            {
                Vector2 dir = (m_fingerPos[i] - m_centerPos).normalized;
                fInputChangeScale = fInputChangeScale + Vector2.Dot(m_fingerMovePos[i], dir);
                //Debug.LogError("move distance : " + Vector2.Dot(m_fingerMovePos[i], dir).ToString());
            }

            return fInputChangeScale;
        }

        public Vector3 getTargetPosition()
        {
            if (m_target == null)
            {
                return mLastCameraPosition;
            }
            return m_target.position + Vector3.up * 1f;
        }

        public override Transform UpdateMainCameraTransform()
        {

            Vector3 newPosition;
            if (m_ignoreTimeScale == false)
            {
                m_cuveCurTime += Time.deltaTime;
            }
            else
            {
                m_cuveCurTime += RealTime.deltaTime;
            }

            float scale = mMoveCuve.Evaluate(m_cuveCurTime);
            if (m_cuveCurTime > m_cuveCountTime)
            {
                newPosition = getTargetPosition();
            }
            else
            {
                Vector3 distance = getTargetPosition() - m_beginMovePos;

                distance *= 1 - scale;

                newPosition = getTargetPosition() - distance;
            }

            mLastCameraPosition = newPosition;


            //Vector3 newPosition = getTargetPosition();

            if (m_horizontal == false)
            {
                float ainHeight = newPosition.y + m_height * (m_distance / HEIGHTSLOPE);

                Quaternion currentRotation = Quaternion.Euler(m_rotationAngleX, m_rotationAngle, 0);

                // Set the position of the camera on the x-z plane to:	    
                transform.position = newPosition;
                transform.position += currentRotation * Vector3.forward * m_distance * -1;

                // Set the height of the camera
                transform.position = new Vector3(transform.position.x, ainHeight, transform.position.z);

                // Always look at the target
                transform.LookAt(newPosition);

                m_audioListerObj.transform.position = newPosition;

            }
            else
            {
                float ainHeight = newPosition.y + m_height;

                Quaternion currentRotation = Quaternion.Euler(m_rotationAngleX, m_rotationAngle, 0);

                // Set the position of the camera on the x-z plane to:	    
                transform.position = newPosition;
                transform.position += currentRotation * Vector3.forward * m_distance * -1;

                // Set the height of the camera
                transform.position = new Vector3(transform.position.x, ainHeight + m_height, transform.position.z);
                //LogMgr.UnityLog("Camera position  x : " + m_transform.position.x.ToString() + ", y : " + m_transform.position.y.ToString()
                //	          + ", z : " + m_transform.position.z.ToString());

                // Always look at the target
                newPosition = new Vector3(newPosition.x, ainHeight, newPosition.z);

                transform.LookAt(newPosition);

                m_audioListerObj.transform.position = newPosition;

            }

            return transform;
        }

        //注册技能释放事件
        void RegisterEvent()
        {
            m_eventMgr.AddListener(GameEvent.GE_PLAYER_LOADING_OVER, EventFunction);

            m_eventMgr.AddListener(GameEvent.GE_CAMERA_EVENT_HEIGHT_ACTIVE, EventFunction);
            m_eventMgr.AddListener(GameEvent.GE_CAMERA_EVENT_HEIGHT_DISABLE, EventFunction);
        }

        void OnDestroy()
        {
            RemoveEvent();
        }

        void RemoveEvent()
        {
            m_eventMgr.RemoveListener(GameEvent.GE_PLAYER_LOADING_OVER, EventFunction);

            m_eventMgr.RemoveListener(GameEvent.GE_CAMERA_EVENT_HEIGHT_ACTIVE, EventFunction);
            m_eventMgr.RemoveListener(GameEvent.GE_CAMERA_EVENT_HEIGHT_DISABLE, EventFunction);
        }
        void OnEnable()
        {
            m_eventMgr.AddListener(GameEvent.GE_UPDATE_CAMERA_POSITION_IMMEDIATE, EventFunction);
            m_eventMgr.AddListener(GameEvent.GE_UI_OPEN, OnUIChange);
            m_eventMgr.AddListener(GameEvent.GE_UI_CLOSE, OnUIChange);
        }

        void OnDisable()
        {
            m_eventMgr.RemoveListener(GameEvent.GE_UPDATE_CAMERA_POSITION_IMMEDIATE, EventFunction);
            m_eventMgr.RemoveListener(GameEvent.GE_UI_OPEN, OnUIChange);
            m_eventMgr.RemoveListener(GameEvent.GE_UI_CLOSE, OnUIChange);
        }

        public void OnUIChange(GameEvent ge, EventParameter parameter)
        {
            if (MainPanelMgr.Instance.IsShow("TipShowUI"))
            {
                return;
            }

            //只处理全屏UI
            MainPanelUICfg cfg = MainPanelMgr.Instance.GetPanelConfig(parameter.stringParameter);
            if (!cfg.fullview)
            {
                return;
            }

            if (ge == GameEvent.GE_UI_OPEN)
            {
                mCamera.enabled = false;
            }
            else
            {
                if (!MainPanelMgr.Instance.IsHaveFullView())
                {
                    mCamera.enabled = true;
                }
            }
        }

        public void EventFunction(GameEvent ge, EventParameter parameter)
        {
            switch (ge)
            {
                case GameEvent.GE_PLAYER_LOADING_OVER:
                    {
                        //m_target = GameObject.FindGameObjectWithTag("player").GetComponent<Transform>();                    
                    }
                    break;

                case GameEvent.GE_CAMERA_EVENT_HEIGHT_ACTIVE:
                    {
                        m_bActive = true;
                    }
                    break;

                case GameEvent.GE_CAMERA_EVENT_HEIGHT_DISABLE:
                    {
                        m_bActive = false;
                    }
                    break;

                case GameEvent.GE_UPDATE_CAMERA_POSITION_IMMEDIATE:
                    {
                        UpdateMainCameraTransform();
                    }
                    break;

                default:
                    break;
            }
        }



        public void testUpDown()
        {
            StartCoroutine(testUpDownImpl());


        }

        public void SetHeight(float h)
        {
            m_height = h;
        }

        public void SetDistance(float d)
        {
            m_distance = d;
        }
        public void SetRotationY(float r)
        {
            m_rotationAngle = r;
        }






        IEnumerator testUpDownImpl()
        {
            TweenAttr.SetAttr fn = new TweenAttr.SetAttr(SetHeight);

            TweenAttr.Begin(gameObject, fn, 3, 10, 2);
            yield return new WaitForSeconds(3);



            TweenAttr.Begin(gameObject, fn, 3, 2, 10);
            yield return new WaitForSeconds(3);

            yield return true;
        }




        //////////////////////////////////////////切换角色  lmjedit////////////////////////////////////////////////////////

        public Vector3 mLastCameraPosition = new Vector3(0, 0, 0);

        GameObject mLookAtObj;
        Transform mLookAtTransform;
        public AnimationCurve mMoveCuve;
        Vector3 mMoveDist;

        float m_cuveCountTime;
        float m_cuveCurTime;
        Vector3 m_beginMovePos;

        public float getDistance(Transform targetTransform)
        {
            return (targetTransform.position - mLastCameraPosition).magnitude;

        }

        public float moveToTargetBySpeed(Transform targetTransform, float speed = 10)
        {
            if (CoreEntry.IsMobaGamePlay())
            {
                speed = 22;
            }

            float time;
            Vector3 dist = targetTransform.position - mLastCameraPosition;
            float distLen = dist.magnitude;
            if (speed < 0.001)
            {
                time = 0.001f;
            }
            else
            {
                time = distLen / speed;
            }

            moveToTarget(targetTransform, time);

            return time;
        }




        public static AnimationCurve CreateMoveCuve(float scaleTime)
        {
            Keyframe[] ks = new Keyframe[3];

            float scale = scaleTime / 10f;

            ks[0] = new Keyframe(0, 0, 1 / scaleTime, 1 / scaleTime);
            //ks[0].inTangent = 0;
            ks[1] = new Keyframe(8f * scale, 0.95f, 0.395f / scaleTime, 0.395f / scaleTime);
            //ks[2].inTangent = 45;
            ks[2] = new Keyframe(10f * scale, 1f, 0.295f / scaleTime, 0.295f / scaleTime);
            //ks[3].inTangent = 45;

            return new AnimationCurve(ks);
        }
        public static AnimationCurve CreateMoveCuveSlow(float scaleTime)
        {
            Keyframe[] ks = new Keyframe[3];

            float scale = scaleTime / 10f;

            ks[0] = new Keyframe(0, 0, 1 / scaleTime, 1 / scaleTime);
            //ks[0].inTangent = 0;
            ks[1] = new Keyframe(5f * scale, 0.8f, 0.395f / scaleTime, 0.395f / scaleTime);
            //ks[2].inTangent = 45;
            ks[2] = new Keyframe(10f * scale, 1f, 0.2f / scaleTime, 0.2f / scaleTime);
            //ks[3].inTangent = 45;

            return new AnimationCurve(ks);
        }

        public static string attachPointName = "E_Spine";

        public void moveToTarget(Transform targetTransform, float time = 1)
        {
            m_target = targetTransform;
            m_cuveCountTime = time;
            m_cuveCurTime = 0;
            mMoveCuve = CreateMoveCuve(m_cuveCountTime);

            LogMgr.UnityLog(string.Format("changeTarget     {0:00.000}    ", m_cuveCountTime));

            m_beginMovePos = mLastCameraPosition;



            if (mLookAtObj == null)
            {
                mLookAtObj = new GameObject(); //    GameObject.CreatePrimitive(PrimitiveType.Cube);
                mLookAtObj.transform.parent = m_target.parent;
                mLookAtObj.transform.position = getTargetPosition();
                // mLookAtObj.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
                mLastCameraPosition = getTargetPosition() + new Vector3(1f, 0, 0);
                mLookAtObj.layer = LayerMask.NameToLayer("mainplayer");

            }
        }

        public void jumpToTarget(Transform targetTransform)
        {

            m_target = targetTransform;
            mLastCameraPosition = m_target.transform.position;


        }
    }
}

