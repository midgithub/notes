/**
* @file     : ModuleServer.cs
* @brief    : 各系统模块逻辑管理
* @details  : 新增系统模块需要在这里加入和创建
* @author   : 
* @date     : 2014-9-24
*/

using XLua;
using UnityEngine;
using System.Collections;

namespace SG
{

[Hotfix]
	public class ModuleServer : IModuleServer {

		private static ModuleServer m_ModuleServer = null;
		
		private static bool bInited = false;
		
		public static ModuleServer MS
		{
			get
			{
				if( bInited == false )
				{
					Init();
				}
				
				return m_ModuleServer;
			}
		}
		
		private static void Init ()
		{
			m_ModuleServer = new ModuleServer();
			bInited = true;
		}


        bool isDoClassConstructorRegister = false ; 
		public void Initialize()
		{
            if (!isDoClassConstructorRegister)
            {
                GameObject go = new GameObject("ModuleRoot");
                GameObject.DontDestroyOnLoad(go);

                isDoClassConstructorRegister = true; 
                DoClassConstructorRegister(go);
                InitializeAllModules();
            }

		}


		//新增模块管理器在这里加入
		//public BloodMgr GBloodMgr 			= null;
		public SkillCastMgr GSkillCastMgr	= null;
		//public ShellMgr GShellMgr 			= null;
		//public JiesuanMgr GJiesuanMgr 		= null;
        public DropMgr GDropMgr             = null;
        public FlyTextManager GFlyTextMgr   = null;
        public FlyAttrManager GFlyAttrMgr   = null;
        public DropUIMgr GDropTextMgr       = null;
        public FixedFlyManager GFixedFlyMgr = null;
		//public LoginMgr GLoginMgr			= null;
     //   public ChatMgr GChatMgr = null;
        public GmMgr GGmMgr = null;

		//新增模块管理器在这里创建
		private bool DoClassConstructorRegister(GameObject go)
		{
//            ClassConstructorRegister BloodMgr_Register = new ClassConstructorRegister(BloodMgr.Newer, go);
//			ClassConstructorRegister SkillCastMgr_Register = new ClassConstructorRegister(SkillCastMgr.Newer, go);
//			ClassConstructorRegister ShellMgr_Register = new ClassConstructorRegister(ShellMgr.Newer, go);
//            ClassConstructorRegister JiesuanMgr_Register = new ClassConstructorRegister(JiesuanMgr.Newer, go);
//            ClassConstructorRegister DropMgr_Register = new ClassConstructorRegister(DropMgr.Newer, go);
//            ClassConstructorRegister FlyTextMgr_Register = new ClassConstructorRegister(FlyTextManager.Newer, go);
//            ClassConstructorRegister DropTextMgr_Register = new ClassConstructorRegister(DropUIMgr.Newer, go);
//            ClassConstructorRegister FixedFlyMgr_Register = new ClassConstructorRegister(FixedFlyManager.Newer, go);
//#if !PUBLISH_RELEASE
//            ClassConstructorRegister GmMgr_Register = new ClassConstructorRegister(GmMgr.Newer, go);
//#endif

            //By XuXiang 去警告
            //new ClassConstructorRegister(BloodMgr.Newer, go);
            new ClassConstructorRegister(SkillCastMgr.Newer, go);
            //new ClassConstructorRegister(ShellMgr.Newer, go);
            //new ClassConstructorRegister(JiesuanMgr.Newer, go);
            new ClassConstructorRegister(DropMgr.Newer, go);
            new ClassConstructorRegister(FlyTextManager.Newer, go);
            new ClassConstructorRegister(FlyAttrManager.Newer, go);
            new ClassConstructorRegister(DropUIMgr.Newer, go);
            new ClassConstructorRegister(FixedFlyManager.Newer, go);
#if !PUBLISH_RELEASE
            new ClassConstructorRegister(GmMgr.Newer, go);
#endif
            return true;
		}
	}

};//End SG

