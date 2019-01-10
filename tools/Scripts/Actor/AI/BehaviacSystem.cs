/**
* @file     : BehaviacSystem.cs
* @brief    : 
* @details  : 行为系统，初始化工作
* @author   : 
* @date     : 2014-9-23 10:12
*/

using XLua;
using UnityEngine;
using System.Collections;

namespace SG
{

//全局唯一
[Hotfix]
public class BehaviacSystem : MonoBehaviour{
    
    void Awake()
    {
        string btExportPath = WorkspaceExportPath;     
        behaviac.Workspace.EFileFormat btFileFormat = behaviac.Workspace.EFileFormat.EFF_cs;
        
        //set workspace exportpath   
        behaviac.Workspace.SetWorkspaceSettings(btExportPath, btFileFormat);        

        //export meta file
        //string metaExportFile = Application.dataPath + "/BTWorkspace/xmlmeta/WPowerMeta.xml";
        //behaviac.Workspace.ExportMetas(metaExportFile);
        
        //for dedug     
        bool ret = behaviac.SocketUtils.SetupConnection(false);        
        if (!ret)
        {
            //Debug.LogWarning("can't connection!");
        }
                
        behaviac.Agent.SetIdMask(0xffffffff);

        behaviac.Config.IsLogging = true;
        behaviac.Config.IsSocketing = true;
    }

	// Use this for initialization
    //void Start () 
    //{
	
    //}
	
	// Update is called once per frame
	//void Update () {
	
	//}

    public static string WorkspaceExportPath
    {
        get{
            string path = "";
            if (Application.platform == RuntimePlatform.WindowsEditor
                || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                path = Application.dataPath + "/Resources/Data/BehaviacData/exported";
            }
            else
            {
                path = "Assets/Resources/Data/BehaviacData/exported";
            }

            return path;
        }
    }
}

};  //end SG

