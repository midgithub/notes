/**
* @file     : FrameStrata.cs
* @brief    : Frame显示层级
* @details  : Frame按照不同类型需要有相对的显示层级关系，此层级关系优先级高于frame内控件之间的depth关系
* @author   : 
* @date     : 2014-10-13
*/

using XLua;
using UnityEngine;
using System.Collections;

namespace SG
{

	public enum FRAMESTRATA_T
	{
        FS_MIN = -10,

		FS_NO_DEFINED = 0,
		FS_SCENE = 20,
		FS_PARENT = 40,
		FS_BACKGROUND = 60,
		FS_LOW = 80,
		FS_MEDIUM = 100,
		FS_HIGH = 120,
		FS_DIALOG = 140,
		FS_FULLSCREEN = 160,
		FS_FULLSCREEN_DIALOG = 180,
		FS_TOOLTIP = 200,
		FS_MAX_STRATA = 220,
	}

};//End SG



