using UnityEngine;
using System.Collections;
using XLua;
using System;
using System.Collections.Generic;

namespace SG
{
    [LuaCallCSharp]
[Hotfix]
    public class DeviceUtil
    {
        /// <summary>
        /// 获取电量。
        /// </summary>
        /// <returns>电量比例。0-100</returns>
        public static int GetBatteryLevel()
        { 
            return Miscellaneous.GetBatteryLevel();
        }

        /// <summary>
        /// 获取网络状态。
        /// </summary>
        /// <returns>网络状态。0无网络 1无线 2流量</returns>
        public static int GetNetState()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                return 0;
            }
            else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
            {
                return 1;
            }
            else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
            {
                return 2;
            }
            return 0;
        }

        /// <summary>
        /// 获取网络延时。
        /// </summary>
        /// <returns>网络延时。(ms)</returns>
        public static int GetNetDelay()
        {
            List<int> delays = CoreEntry.netMgr.HearBeatDelay;
            if (delays.Count <= 0)
            {
                return 0;
            }

            int delay = 0;
            for (int i=0; i< delays.Count; ++i)
            {
                delay += delays[i];
            }
            return delay / delays.Count;
        }
    }
}

