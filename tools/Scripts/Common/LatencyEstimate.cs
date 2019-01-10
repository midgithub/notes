using XLua;
﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{ 
[Hotfix]
    public class LatencyEstimate
    {
#region Property
        private UInt64 serverTime = 0 ;
        private UInt16 lastRTT = 0 ;
        private UInt16 currentRTT = 0 ;
        private float currentClientTime;
        public UInt64 ServerTime { get { return serverTime; } }
        public UInt16 RTT { get { return currentRTT; } }
        public UInt16 Latency { get { return (ushort)(currentRTT / 2); } }
#endregion

        public LatencyEstimate()
        {
        }
        public void Reset()
        {
            serverTime = 0;
            lastRTT = 0;
            currentRTT = 0;
        }
        public void ServerTimeRequest()
        {
            //todo send time-sync packet to server
            //record current local time
            currentClientTime = Time.time ;
        }

        public void ServerTimeResponse(UInt64 resposeTime)
        {
            float rtt = Time.time - currentClientTime;
            if (lastRTT == 0)
            {
                currentRTT = (UInt16)(rtt * 1000);
                lastRTT = currentRTT;
            }
            else
            {
                currentRTT = (UInt16)((rtt * 1000 * 0.8) +  lastRTT * 0.2);                
            }

            serverTime = resposeTime + Latency;
        }
    }
}

