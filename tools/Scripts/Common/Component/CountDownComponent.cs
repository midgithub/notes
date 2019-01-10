using XLua;
﻿using UnityEngine;
using System.Collections;
using System;

namespace SG
{

[Hotfix]
    public class CountDownData
    {
        public object Target = null;
        public float Time = 0f;
        public Action<object, GameObject> Callback = null;
    }
    /// <summary>
    /// 倒计时组件
    /// </summary>
[Hotfix]
    public class CountDownComponent : MonoBehaviour
    {
        CountDownData data = new CountDownData();
        CountDownData data2 = new CountDownData();
        public void InitTimer(float time, object target, Action<object,GameObject> callback)
        {
            data.Time = time;
            data.Target = target;
            data.Callback = callback;

            Invoke("Timeout", data.Time);
        }

        public void InitTimer2(float time, object target, Action<object, GameObject> callback)
        {
            data2.Time = time;
            data2.Target = target;
            data2.Callback = callback;

            Invoke("Timeout2", data2.Time);
        }

        void Timeout()
        {
            if (data.Callback != null)
            {
                data.Callback(data.Target, gameObject);
            }
        }

        void Timeout2()
        {
            if (data2.Callback != null)
            {
                data2.Callback(data2.Target, gameObject);
            }
        }

        public void ClearTimer()
        {
            CancelInvoke("Timeout");
            CancelInvoke("Timeout2");
        }

    }


};  //end SG

