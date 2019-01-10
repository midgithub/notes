using XLua;
﻿using UnityEngine;
using System.Collections;

namespace SG
{

[Hotfix]
public class CameraShake : ISkillCell {
    private Transform camTransform;    
    
    private CameraBase m_cameraFollow;

    public bool bIsNeedVibrate = false;


    //编辑器中设置曲线
    public AnimationCurve m_animationCurve = new AnimationCurve();
            
    private float m_beginTime = 0;

    public override void Init(ISkillCellData cellData, SkillBase skillBase)
    {
        if (m_cameraFollow != null && m_cameraFollow.IsDisableCameraShake() == false)
        {
            Invoke("ShakeStart", ((CameraShakeCellDesc)cellData).playTime + 0.000002f);  // 这里加个0.000002f 让ShakeStart函数在Start后执行
        }

         //active the camera blur
         if(((CameraShakeCellDesc)cellData).fBlurTime > 0.0f)
            {
                GameObject obj = GameObject.FindGameObjectWithTag("MainCamera");
                if(obj)
                {
                    DeemoRadialBlur blur = obj.GetComponent<DeemoRadialBlur>();

                    if(blur)
                    {
                        //  GameObject blurObj = blur.;
                        //  blurObj.SetActive(true);

                        blur.enabled = true;

                        Invoke("Endblur", ((CameraShakeCellDesc)cellData).fBlurTime);
                    }
                   
                }

            }
        }


    void Endblur()
        {
            CancelInvoke("Endblur");

            GameObject obj = GameObject.FindGameObjectWithTag("MainCamera");
            if (obj)
            {
                DeemoRadialBlur blur = obj.GetComponent<DeemoRadialBlur>();

                if (blur)
                {
                    //  GameObject blurObj = blur.gameObject;
                    //  blurObj.SetActive(false);     

                    blur.enabled = false;
                }
            }

        }



    // PoolManager debug
        void OnDisable()
    {
        if (m_cameraFollow != null)
        {
            m_cameraFollow.m_cameraShake = false;
            m_cameraFollow = null;
        }
        CancelInvoke("Start");
        CancelInvoke("ShakeStart");
        CancelInvoke("ShakeEnd");
    }

    // PoolManager
    void OnEnable()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("MainCamera");
        if (obj != null)
        {
            m_cameraFollow = obj.GetComponent<CameraBase>();
            camTransform = obj.transform;
        }

        CancelInvoke("Start");
        Invoke("Start", 0.000001f);
    }

    void Start()
    {
        CancelInvoke("Start");


     
        //Invoke("ShakeStart", m_delayStartTime);
    }

    void Update()
    {
        if (m_cameraFollow!=null&&m_cameraFollow.m_cameraShake)
        {
			//更新主相机当前位置
			camTransform = m_cameraFollow.UpdateMainCameraTransform();

			//相机抖动的值
            float posY = m_animationCurve.Evaluate(Time.time - m_beginTime);            

			camTransform.position = camTransform.position + new Vector3(0, posY, 0);                                   
        }        
    }

    void OnDestroy()
    {
        if(m_cameraFollow!=null)
            m_cameraFollow.m_cameraShake = false;
    }

    void ShakeStart()
    {
        CancelInvoke("ShakeStart");

        if (m_cameraFollow != null && m_cameraFollow.IsDisableCameraShake() == true)
        {
            return;
        }
        
        if (m_cameraFollow!=null)
        {
            m_cameraFollow.m_cameraShake = true;

            m_beginTime = Time.time;

            float length = m_animationCurve[m_animationCurve.length - 1].time;


            //震动
         //   if (bIsNeedVibrate)
            //    Handheld.Vibrate();



            Invoke("ShakeEnd", length);
        }
    }

    void ShakeEnd()
    {
        CancelInvoke("ShakeEnd");    
        m_cameraFollow.m_cameraShake = false;
        
        //Destroy(this.gameObject);
    }   
}

}; //end SG

