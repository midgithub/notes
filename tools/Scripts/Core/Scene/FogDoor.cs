/**
* @file     : FogDoor.cs
* @brief    : 
* @details  : 雾效门，用于实现区域雾效果
* @author   : 
* @date     : 2014-12-2
*/

using XLua;
using UnityEngine;
using System.Collections;

namespace SG
{
[Hotfix]
public class FogDoor : MonoBehaviour {
    private bool m_isEnter = false;

    //进入前的雾效
    private bool old_bFog = true;
    private Color old_heightFogColor = new Color(1.0f, 1.0f, 1.0f);
    private float old_top = 10.0f;
    private float old_bottom = 0.0f;
    private Color old_distanceFogColor = new Color(1.0f, 1.0f, 1.0f);
    private float old_near = 10.0f;
    private float old_far = 100.0f;

    //当前的雾效
    public Color heightFogColor = new Color(1.0f, 1.0f, 1.0f);
    public float top = 10.0f;
    public float bottom = 0.0f;
    public Color distanceFogColor = new Color(1.0f, 1.0f, 1.0f);
    public float near = 10.0f;
    public float far = 100.0f;

    private float m_startTime = 0;
    private float m_needTime = 2;
    private bool m_startChange = false;
    private bool m_curEnter = false;        

    RenderManager m_renderManager = null;

    void Awake()
    {
        MeshRenderer render = this.gameObject.GetComponent<MeshRenderer>();        
        if (render != null && render.enabled)
        {
            render.enabled = false;
        }                               
    }

	// Use this for initialization
    void Start () 
    {        
        GameObject obj = GameObject.FindGameObjectWithTag("fogManager");
        if (obj == null)
        {
            Destroy(this.gameObject);
        }   
        else
        {
            m_renderManager = obj.gameObject.GetComponent<RenderManager>();
        }              	
    }
	
	// Update is called once per frame
    void Update()
    {
        if (m_startChange)
        {            
            float curTime = Time.time;
            float diffTime = curTime - m_startTime;            
            float ratio = diffTime / m_needTime;

            if (diffTime >= m_needTime)
            {
                m_startChange = false;                                                               
            }

            if (m_curEnter)
            {
                //进入
                m_renderManager.bFog = true;
                m_renderManager.heightFogColor = Color.Lerp(old_heightFogColor, heightFogColor, ratio);
                m_renderManager.distanceFogColor = Color.Lerp(old_distanceFogColor, distanceFogColor, ratio);
                m_renderManager.bottom = Mathf.Lerp(old_bottom, bottom, ratio);
                m_renderManager.top = Mathf.Lerp(old_top, top, ratio);
                m_renderManager.near = Mathf.Lerp(old_near, near, ratio);
                m_renderManager.far = Mathf.Lerp(old_far, far, ratio);                                                                   
            }
            else
            {
                //退出，还原
                m_renderManager.bFog = old_bFog;
                m_renderManager.heightFogColor = Color.Lerp(heightFogColor, old_heightFogColor, ratio);
                m_renderManager.distanceFogColor = Color.Lerp(distanceFogColor, old_distanceFogColor, ratio);
                m_renderManager.bottom = Mathf.Lerp(bottom, old_bottom, ratio);
                m_renderManager.top = Mathf.Lerp(top, old_top, ratio);
                m_renderManager.near = Mathf.Lerp(near, old_near, ratio);
                m_renderManager.far = Mathf.Lerp(far, old_far, ratio);   
            }       
            
            if (!m_startChange)
            {
                //是否在不同的区域
                if (m_isEnter != m_curEnter)
                {
                    StartChange(m_isEnter);   
                } 
            }            
        }               
    }

    //触发器
    void OnTriggerEnter(Collider other)
    {        
        int playerLayer = LayerMask.NameToLayer("mainplayer");
        
        //碰到主角了
        if (other.transform.gameObject.layer == playerLayer)
        {
            m_isEnter = !m_isEnter;      

            //没有变化
            if (!m_startChange)
            {
                StartChange(m_isEnter);                                                
            }                                          
        }                            
    }    

    //开始变化
    void StartChange(bool isEnter)
    {        
        m_startChange = true;
        m_curEnter = isEnter;
        m_startTime = Time.time;    

        //进入区域
        if (isEnter)
        {
            //获取当前的参数
            old_bFog = m_renderManager.bFog;
            old_bottom = m_renderManager.bottom;
            old_top = m_renderManager.top;
            old_heightFogColor = m_renderManager.heightFogColor;
            old_distanceFogColor = m_renderManager.distanceFogColor;
            old_near = m_renderManager.near;
            old_far = m_renderManager.far;                    
        }             
    }

    //编辑器使用
    #if UNITY_EDITOR
        public void FogShowEditor()
        {
            LogMgr.UnityLog("FogShowEditor()");

            Shader.EnableKeyword("_FOG_ON");
            Shader.SetGlobalColor("_HeightFogColor", heightFogColor);
            Shader.SetGlobalColor("_DistanceFogColor", distanceFogColor);
            Vector4 fogParam = new Vector4(top, bottom, near, far);
            Shader.SetGlobalVector("_FogParam", fogParam);
        }
    #endif
}

};  //end SG

