using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;
namespace SG
{
    public enum Collider_Type
    {
        CT_NONE = 0,
        CT_CAPSULE,
        CT_BOX
    }


    public enum ActorType
    {
        AT_NONE = 0,
        AT_LOCAL_PLAYER = 1,
        AT_MONSTER = 2,
        AT_BOSS = 3,
        AT_NPC = 4,
        AT_PVP_PLAYER = 5,
        AT_TRAP = 6,// 陷阱类型
        AT_BROKED = 7,//破碎类型
        AT_NON_ATTACK = 8, //不能被攻击的类型
        AT_AVATAR,          //分身
        AT_REMOTE_PLAYER,//网络中的其他玩家
        AT_SCENE_BUFF = 11,//场景里能吃得buff类型
        AT_MECHANICS = 12, //机械类
        AT_PET = 13,//宠物
    };


    //阵营
    public enum GroupType
    {
        GT_NONE = 0, 
        GT_ENEMY = 1,    //敌人
        GT_FRIEND = 2,   //友好
        GT_NEUTRAL = 3,  //中立

    }

    public enum SkillShowType
    {
        ST_MAGICKEY = 63, //法宝技能
        ST_HUANLING = 69,//幻灵红颜技能
        ST_BEAUTYMANHETI=73, //红颜合体技能
        ST_EQUIP_SKILL = 96,//装备技能
    }


    //object的基类组件
    [LuaCallCSharp]
[Hotfix]
    public class Entity : MonoBehaviour
    {
        //支持的组件
      //  protected ActorObj m_actor = null;

        //点光源
        GameObject m_pointLight = null;
        public UnityEngine.GameObject PointLight
        {
            get { return m_pointLight; }
            set { m_pointLight = value; }
        }

        public void setLightActive(bool isActive)
        {
            //if (CoreEntry.gGameMgr.CurMapDesc.TimeSystem == (int)Configs.MapDesc.TimeSystemEnum.NIGHTTIME)
            //{
            //    if (PointLight)
            //        PointLight.SetActive(isActive);
            //}
        }

        //影子
        protected BlobShadow m_blodShadow = null;
        /// <summary>
        /// 在调用基类Entity的Init方法前设置生效
        /// </summary>
        protected int m_shadowType = 1;


        protected SkinnedMeshRenderer[] m_allSkinnedMesh = null;
        protected MeshRenderer[] m_allMeshRender = null;
        protected Transform m_Bip001 = null;
        protected Transform m_hand_left = null;
        protected Transform m_hand_right = null;
        protected Transform m_E_Root = null;

        /// <summary>
        /// 移动速度缩放因子
        /// </summary>
        public float mSpeedScale = 1f;

        public float m_Speed = 1f;

 
        //id,编辑器使用
        //#if UNITY_EDITOR
        protected int m_resID = 0;

        //设置id 
        public int resid
        {
            get { return m_resID; }
            set { m_resID = value; }
        }

        protected int m_ConfigID = 0;
        public int ConfigID
        {
            get { return m_ConfigID; }
            set { m_ConfigID = value; }
        }

        protected int m_nEntity = 0;
        public int entityid
        {
            get { return m_nEntity; }
            set { m_nEntity = value; }
        }


        //服务器唯一ID       此参数服务器唯一ID，yuxj标记
        protected long serverID;

        public long ServerID
        {
            get { return serverID; }
            set { serverID = value; }
        }

        protected ActorType m_ActorType = ActorType.AT_NONE;

        public ActorType mActorType
        {
            get { return m_ActorType; }
            set { m_ActorType = value; }
        }

     //   public Transform m_transform = null;

        //入场动作
        protected string m_strEnterAction = "default";  //default 表示采用excel表格中入场动作
        public string enterAction
        {
            get { return m_strEnterAction; }
            set { m_strEnterAction = value; }
        }

        public Dictionary<int, string> dicEnterAction = new Dictionary<int, string>();

        //#endif    

        //public ActorObj actorBase
        //{
        //    get { return m_actor; }
        //}

        public int EntityID
        {
            get { return m_nEntity; }
        }

        

        public GameObject thisGameObject
        {
            get { return this.gameObject; }
        }


        public Vector3 m_CurveMovePos = Vector3.zero;

        public void SetPosition(float x, float z)
        {
            float y = CommonTools.GetTerrainHeight(new Vector2(x, z));
            SetPosition(new Vector3(x, y, z));
        }

        public void SetPosition(Vector3 position)
        {
            //  transform.position = position;
            BaseTool.SetPosition(transform, position);
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }



        //初始化.对外接口
        public virtual void Init(int resID, int ConfigID, long ServerID, string strEnterAction = "", bool isNpc = false)
        {
        
            this.ServerID = ServerID;

            m_resID = resID;

            m_ConfigID = ConfigID;


            //生成entityid
            m_nEntity = CoreEntry.gBaseTool.GetIntQueueValue();

         //   m_actor.Init(resID, entityID, m_cardAttr, strEnterAction);


            //增加影子
            if (m_shadowType != 0)
            {

                if (this.gameObject.GetComponent<BlobShadow>() == null)
                {
                    m_blodShadow = this.gameObject.AddComponent<BlobShadow>();
                }
                else
                {
                    m_blodShadow = this.gameObject.GetComponent<BlobShadow>();
                }

                m_blodShadow.Init(this, m_shadowType);
                m_blodShadow.ShowShadow();
                //todo:读表设置影子大小                
                float radius = GetColliderRadius();
                m_blodShadow.scaleX = radius / 0.5f;
                m_blodShadow.scaleY = radius / 0.5f;

            }


            //获取所有的skinndmesh
            m_allSkinnedMesh = this.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            m_allMeshRender = this.gameObject.GetComponentsInChildren<MeshRenderer>();

            m_Bip001 =  this.gameObject.transform.Find("Bip001");

            m_hand_left = this.gameObject.transform.Find("hand_left");
            m_hand_right = this.gameObject.transform.Find("hand_right");
            m_E_Root = this.gameObject.transform.Find("E_Root");
            
            ShowSelf();
        }

        //还原
        public virtual void UnInit() { }
        //复活处理
        public virtual void Recover() { }



        public virtual void NavigateTo(Vector3 pos)
        {

        }


        public virtual ActorObj GetSelTarget( )
        {
            return null;
        }

 
        //hp变化，主角，怪物，自己处理吧
        public virtual void DoDamage(int hp, bool bIsMainPlayer, BehitParam behitParam) 
        {
        }

        public virtual Vector3 GetBoxColliderSize()
        {
            Vector3 boxSize = Vector3.zero;
            return boxSize;
        }

        public virtual Collider GetCollider()
        {
            return null;
        }

        //获取碰撞体半径
        public virtual float GetColliderRadius()
        {
            return 0;
        }

        public void HideBlobShadow()
        {
            if (m_blodShadow != null)
            {
                m_blodShadow.HideShadow();
            }
        }

        public void ShowBlobShadow()
        {
            if (m_blodShadow != null)
            {
                m_blodShadow.ShowShadow();
            }
        }

        public virtual void SetSpeed(float speed)
        {
         
        }

        //隐身
        public virtual void HideSelf()
        {
            if (m_allSkinnedMesh == null)
            {
                return;
            }

            if(m_Bip001 != null)
            {
                m_Bip001.gameObject.SetActive(false);
            }
            if (m_hand_left != null)
            {
                m_hand_left.gameObject.SetActive(false);
            }
            if (m_hand_right != null)
            {
                m_hand_right.gameObject.SetActive(false);
            }
            if(m_E_Root != null)
            {
                m_E_Root.gameObject.SetActive(false);
            }

            for (int i = 0; i < m_allSkinnedMesh.Length; ++i)
            {
                if (m_allSkinnedMesh[i] != null)
                {
                    m_allSkinnedMesh[i].enabled = false;
                }
            }

            for (int i = 0; i < m_allMeshRender.Length; ++i)
            {
                if (m_allMeshRender[i] != null)
                {
                    m_allMeshRender[i].enabled = false;
                }
            }
        }

        //显示
        public virtual void ShowSelf()
        {
            if (m_allSkinnedMesh == null)
            {
                return;
            }
            if (m_Bip001 != null)
            {
                m_Bip001.gameObject.SetActive(true);
            }
            if (m_hand_left != null)
            {
                m_hand_left.gameObject.SetActive(true);
            }
            if (m_hand_right != null)
            {
                m_hand_right.gameObject.SetActive(true);
            }
            if (m_E_Root != null)
            {
                m_E_Root.gameObject.SetActive(true);
            }


            for (int i = 0; i < m_allSkinnedMesh.Length; ++i)
            {
                if (m_allSkinnedMesh[i] != null)
                {
                    m_allSkinnedMesh[i].enabled = true;
                }
            }

            for (int i = 0; i < m_allMeshRender.Length; ++i)
            {
                if (m_allMeshRender[i] != null)
                {
                    m_allMeshRender[i].enabled = true;
                }
            }
        }


        //友方看到的半透隐身状态
        public void StealthSelf(bool bStealth)
        {
            if (bStealth)
                SetMaterialShader("DZSMobile/CharacterAlpha", "_BodyAlpha", 0.5f);
            else
                SetMaterialShader("DZSMobile/Character", null, 0);
        }
 

        public void SetMaterialShader(string mat, string key, float val)
        {
            if (m_allSkinnedMesh == null)
            {
                return;
            }
            for (int i = 0; i < m_allSkinnedMesh.Length; ++i)
            {
                if (m_allSkinnedMesh[i].gameObject.layer == 22)//翅膀不改
                {
                    continue;
                }
                if (m_allSkinnedMesh[i] != null)
                {
                    if (mat != null && mat != "")
                        m_allSkinnedMesh[i].material.shader = CoreEntry.gResLoader.LoadShader(mat);

                    if (key != null && key != "")
                    {
                        if (key.IndexOf("Color") != -1)
                        {
                            Color color = UiUtil.IntToColor((int)val);
                            m_allSkinnedMesh[i].material.SetColor(key, color);
                        }
                        else
                        {
                            m_allSkinnedMesh[i].material.SetFloat(key, val);
                        }
                    }
                }
            }
        }

        public void SetMaterialColor(string mat, string key, Color color)
        {
            if (m_allSkinnedMesh == null)
            {
                return;
            }
            for (int i = 0; i < m_allSkinnedMesh.Length; ++i)
            {
                if (m_allSkinnedMesh[i] != null)
                {
                    if (m_allSkinnedMesh[i].gameObject.layer == 22)//翅膀不改
                    {
                        continue;
                    }
                    if (m_allSkinnedMesh[i].materials.Length > 1)
                    {
                        if (mat != null && mat != "")
                            m_allSkinnedMesh[i].materials[1].shader = CoreEntry.gResLoader.LoadShader(mat);

                        if (key != null && key != "")
                        {
                            m_allSkinnedMesh[i].materials[1].SetColor(key, color);
                        }
                    }
                    else
                    {
                        if (mat != null && mat != "")
                            m_allSkinnedMesh[i].material.shader = CoreEntry.gResLoader.LoadShader(mat);

                        if (key != null && key != "")
                        {
                            m_allSkinnedMesh[i].material.SetColor(key, color);
                        }

                    }
                }
            }
        }

        /// <summary>
        /// 忽略条件，强制复活
        /// </summary>
        public virtual void ForceToRevive()
        {

        }

        public virtual void ForceToRebirth()
        {

        }

        public virtual void Awake()
        {
           // m_transform = this.transform;

        }

        /// <summary>
        /// 回收缓存对象
        /// </summary>
        public virtual void RecycleObj()
        {
            CoreEntry.gObjPoolMgr.RecycleObject(resid, gameObject);
        }

    }

};  //end SG