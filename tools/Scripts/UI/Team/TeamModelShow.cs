/**
* @file     : TeamModelShow.cs
* @brief    : 队伍模型展示
* @details  : 
* @author   : XuXiang
* @date     : 2017-11-23 11:09
*/

using UnityEngine;
using System.Collections;
using XLua;
using System.Collections.Generic;

namespace SG
{
    [LuaCallCSharp]
[Hotfix]
    public class TeamModelShow : MonoBehaviour
    {
        /// <summary>
        /// 显示对象预制件路径。
        /// </summary>
        public static string PrefabPath = "UI/Prefabs/Team/TeamModelShow";

        /// <summary>
        /// 当前模型显示对象
        /// </summary>
        private static TeamModelShow CurModeShow;

        /// <summary>
        /// 显示队伍模型。
        /// </summary>
        public static void ShowTeamModel()
        {
            if (CurModeShow == null)
            {
                GameObject prefab = (GameObject)CoreEntry.gResLoader.Load(PrefabPath, typeof(GameObject));
                if (prefab == null)
                {
                    return;
                }

                GameObject obj = Instantiate(prefab) as GameObject;
                CurModeShow = obj.GetComponent<TeamModelShow>();
                DontDestroyOnLoad(CurModeShow);
                CurModeShow.transform.localPosition = new Vector3(0, -1000, 0);
                CurModeShow.transform.localScale = Vector3.one;
                CurModeShow.transform.forward = Vector3.forward;
            }

            if (!CurModeShow.gameObject.activeSelf)
            {
                CurModeShow.gameObject.SetActive(true);
            }
            CurModeShow.Refresh();
        }

        /// <summary>
        /// 隐藏队伍模型。
        /// </summary>
        public static void HideTeamModel()
        {
            if (CurModeShow == null)
            {
                return;
            }

            if (CurModeShow.gameObject.activeSelf)
            {
                CurModeShow.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 旋转模型。
        /// </summary>
        public static void Rotate(int index, Vector3 r)
        {
            if (CurModeShow == null || !CurModeShow.gameObject.activeSelf || index < 0 || index >= CurModeShow.TeamMembers.Length)
            {
                return;
            }

            TeamMemberModelShow model = CurModeShow.TeamMembers[index];
            if (model.gameObject.activeSelf)
            {
                model.ModelParent.Rotate(r);
            }            
        }


        /// <summary>
        /// 队伍成员模型。
        /// </summary>
        public TeamMemberModelShow[] TeamMembers;

        /// <summary>
        /// 刷新队伍成员。
        /// </summary>
        public void Refresh()
        {
            if (TeamMembers == null)
            {
                return;
            }

            List<MsgData_sTeamRole> members = PlayerData.Instance.TeamData.Members;
            for (int i=0; i< TeamMembers.Length; ++i)
            {
                TeamMembers[i].gameObject.SetActive(i < members.Count);
                if (i < members.Count)
                {
                    TeamMembers[i].Init(members[i]);
                }
            }
        }
    }
}

