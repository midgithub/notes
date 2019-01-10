/**
* @file     : IModuleServer.cs
* @brief    : 负责各系统模块加载和初始化
* @details  : 
* @author   : 
* @date     : 2014-9-24
*/

using XLua;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
[Hotfix]
	public class IModule : MonoBehaviour {

		public virtual bool LoadSrv(IModuleServer IModuleSrv) { return false; }

		public virtual void InitializeSrv() { }
	}

	public delegate IModule ClassNewer(GameObject go);
	
[Hotfix]
	public class ClassConstructorRegister {
		
		public ClassConstructorRegister(ClassNewer newer, GameObject go)
		{
			IModule module = newer(go);
			if (module != null)
			{
				ModuleServer.MS.RegisterClassFactory(module);
			}
		}
	}

[Hotfix]
	public class IModuleServer {
			
		private List<IModule> mLogicModules = new List<IModule>();

		// 注册模块
		public void RegisterClassFactory(IModule module)
		{
			if (module.LoadSrv(this) == true)
			{
				// 注册模块成功
				mLogicModules.Add(module);
			}
		}

		// 初始化所有模块
		public void InitializeAllModules()
		{
			foreach (IModule module in mLogicModules)
			{
				module.InitializeSrv();
			}
		}
	}

}

