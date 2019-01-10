using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
namespace SG
{
[Hotfix]
	public class WarningDecel : MonoBehaviour {		

		//持有贴花引用
		GameObject decalPerfab = null;

        //private int groundLayerMask;

		void Start () {

            

		}

        Decal getDecal()
        {
            return decalPerfab.GetComponentInChildren<Decal>();
        }

        /*
         * 设置贴花位置
         */
		public void SetPositionOnGround(Vector3 wordPositon)
		{
            //groundLayerMask = 1 << LayerMask.NameToLayer("ground");
			Ray l_Ray = new Ray(wordPositon, Vector3.down);

            RaycastHit GroudHit = new RaycastHit();
            if (Physics.Raycast(l_Ray, out GroudHit, Mathf.Infinity))
            {
                
                transform.position = GroudHit.point;
                
            }
            else
            {
                transform.position = wordPositon;
            }

      
		}

        /*
         * 隐藏贴花
         */
        public void HideDecal()
        {
            gameObject.SetActive(false);
            Destory();
        }

        /*
         * 显示贴花
         */
        public void ShowDecal()
        {
            gameObject.SetActive(true);
        }

        /*
         * 销毁
         */
        public void Destory()
        {
            Destroy(gameObject);
        }


        /*
         * 设置贴花类型
         */
        void SetDecalType(CLIPSHAPE_TYPE type)
        {
            getDecal().shapeType = type;
            getDecal().UpdateSelf();
        }

        /*
         * 设置角度
         */
        void SetAngle(float angle)
        {
            getDecal().sectorShapeAngle = angle;            
        }

		// Update is called once per frame
		void Update () {		

		}

        /*
         * 在player位置创建贴花
         */
        public static void CreateUnderPlayer()
        {
            var player = GameObject.FindGameObjectWithTag("player");
            if (player)
            {
                CreateRectangleDecal("Effect/skill/remain/yujingtiehua01",player.transform.position);
            }
            else
            {
                LogMgr.UnityWarning("nat find player");
            }
        }

        /*
         *  创建贴花
         */
        static WarningDecel Create(string DecelPerferb , Vector3 wordPositon)
        {
            GameObject go = (GameObject)Instantiate(CoreEntry.gResLoader.LoadResource(DecelPerferb));            
            if(go)
            {
                WarningDecel wd = go.AddComponent<WarningDecel>(); //go.GetComponent<WarningDecel>() as WarningDecel;
                if (wd)
                {
                    wd.Init(wordPositon,ref go);
                }
                return wd;
            }
            else
            {
                LogMgr.UnityError("[WarningDecal Create Failed]");
                return null;
            }
        }
        
        /*
         * 创建扇形贴花
         */
        public static WarningDecel CreateSectorDecal(string DecelPerferb, Vector3 wordPositon, float dscale = 5.0f, float angle = 90)
        {
            LogMgr.UnityWarning("WarningDecel"+DecelPerferb.ToString());
            WarningDecel wd = Create(DecelPerferb, wordPositon);
            if (wd)
            {
                wd.SetDecalType(CLIPSHAPE_TYPE.CST_SECTOR);
                wd.SetAngle(angle);
                wd.transform.localScale = new Vector3(dscale, 0.01f, dscale);
                return wd;
            }
            return null;
        }

        /*
         * 创建矩形贴花
         */
        public static WarningDecel CreateRectangleDecal(string DecelPerferb,Vector3 wordPositon, float xscale = 5.0f, float zscale = 5.0f)
        {
            WarningDecel wd = Create(DecelPerferb, wordPositon);
            if (wd)
            {
                wd.SetDecalType(CLIPSHAPE_TYPE.CST_SQUARE);
                wd.transform.localScale = new Vector3(xscale, 0.01f, zscale);
                return wd;      
            }
            return null;
        }
		
        /*
         *  初始化
         */
        public void Init(Vector3 wordPositon,ref GameObject decal)
        {
            //设置位置坐标
            SetPositionOnGround(wordPositon);

            //持有引用
            decalPerfab = decal;
        }
	}
}

