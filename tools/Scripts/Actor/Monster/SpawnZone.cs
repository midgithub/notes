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

	[ExecuteInEditMode, RequireComponent(typeof(RectangleZone))]
[Hotfix]
	public class SpawnZone : MonoBehaviour {

		public int zoneID;
        public int doorID;
		public Vector3 minimumPos;
		public Vector3 maximumPos;


		public bool selectInEditor;
		
		// Use this for initialization
		void Start ()
		{
			GetComponent<RectangleZone>().Hide();
		}
		
		// Update is called once per frame
		void Update () 
		{
			if (selectInEditor == true)
			{
				GetComponent<RectangleZone>().Show();
			}
			else
			{
				GetComponent<RectangleZone>().Hide();
			}
			
			if (GetComponent<RectangleZone>().IsShown() == true)
			{
				GetComponent<RectangleZone>().SetZonePoint(
					new Vector3(minimumPos.x, minimumPos.y, minimumPos.z), 
					new Vector3(maximumPos.x, maximumPos.y, maximumPos.z));
			}
		}
	}

};//End SG

