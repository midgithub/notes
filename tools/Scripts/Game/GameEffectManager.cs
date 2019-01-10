using XLua;
﻿using UnityEngine;
using System.Collections;

namespace SG
{
[Hotfix]
    public class GameEffectManager
    {

        public ImbededAnimation BossDeadEffect = new ImbededAnimation();
        public PlayAction BossDeadEffect_PlayAction = null;


        public void Load()
        {
            if (MapMgr.Instance.InMainCity == false)
            {
                if (BossDeadEffect.GameObj != null)
                {
                    BossDeadEffect.Load("Animation/effect/BossDeadEffect", (a, b) =>
                        {
                            BossDeadEffect.GameObj.SetActive(false);
                        });

                    //在GetComponentInChildren之前先判断是否active，如果不是的话，先打开
                    if (BossDeadEffect.GameObj.activeInHierarchy == false)
                    {
                        BossDeadEffect.GameObj.SetActive(true);
                    }

                    Camera cam = BossDeadEffect.GameObj.GetComponentInChildren<Camera>();
                    if (cam != null && null != CoreEntry.gCameraMgr.MainCamera)
                    {
                        cam.CopyFrom(CoreEntry.gCameraMgr.MainCamera);
                        BaseTool.ResetTransform(cam.transform);

                    }
                    if (null != CoreEntry.gCameraMgr.MainCamera)
                    {
                        BossDeadEffect.GameObj.transform.parent = CoreEntry.gCameraMgr.MainCamera.transform;
                    }
                    BaseTool.ResetTransform(BossDeadEffect.GameObj.transform);

                    //不让动画自动播放
                    BossDeadEffect.GameObj.SetActive(false);

                    BossDeadEffect_PlayAction = BossDeadEffect.GameObj.GetComponent<PlayAction>();
                }
            }
        }
    }
}

