/**
* @file     : a
* @brief    : b
* @details  : d
* @author   : a
* @date     : 2014-xx-xx
*/

using XLua;
using UnityEngine;
using System.Collections;

namespace SG
{
	//怪物波触发条件类型
	public enum WTC_TYPE{
		DEFAULT,
		LASTWAVEOVER,
		LASTWAVEBEGAN,
		ZONE,
	}

	//怪物波触发循环类型
	public enum WTL_TYPE{
		NONE = 0,
		MONSTERUPDATE,
		//TIMERANGE,
	}

	[ExecuteInEditMode, RequireComponent(typeof(RectangleZone))]
[Hotfix]
	public class SpawnWave : MonoBehaviour {

		public int sequence;
		public WTC_TYPE triggerType;
        public float turbulenceTime;
		public WTL_TYPE loopType;
		public int loopMonsterID;
		public float loopMonsterIntervalTime;
		public float loopTimeRange;
		public float arg1;
		public float arg2;
		public float arg3;
		public float arg4;
		public float arg5;
		public float arg6;

		static public WTC_TYPE DEFAULT_TRIGTYPE = WTC_TYPE.LASTWAVEOVER;
		static public float DEFAULT_INTERVALTIME = 1.0f;

		public bool selectInEditor;

		// Use this for initialization
		void Start ()
		{
			GetComponent<RectangleZone>().Hide();
		}

		// Update is called once per frame
		void Update () 
		{
			if (selectInEditor == true && triggerType == WTC_TYPE.ZONE)
			{
				GetComponent<RectangleZone>().Show();
			}
			else
			{
				GetComponent<RectangleZone>().Hide();
			}

			if (GetComponent<RectangleZone>().IsShown() == true)
			{
				GetComponent<RectangleZone>().SetZonePoint(new Vector3(arg1, arg2, arg3), new Vector3(arg4, arg5, arg6));
			}
		}
	}

};//End SG

