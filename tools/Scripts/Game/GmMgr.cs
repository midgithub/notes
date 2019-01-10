/**
* @file     : GmMgr.cs
* @brief    : GM命令管理
* @details  : 
* @author   : nakewang
* @date     : 2014-12-08
*/

using XLua;
using UnityEngine;
using System.Collections;

namespace SG
{
[Hotfix]
    public class GmMgr : IModule
    {
#if !PUBLISH_RELEASE
        //----------- 每个管理器必须写的方法 ----------
        public override bool LoadSrv(IModuleServer IModuleSrv)
        {
            ModuleServer moduleSrv = (ModuleServer)IModuleSrv;
            moduleSrv.GGmMgr = this;

            return true;
        }

        public override void InitializeSrv()
        {
            //LogMgr.UnityLog("call initialize GmMgr!");
        }

        public static IModule Newer(GameObject go)
        {
            IModule module = go.AddComponent<GmMgr>();
            return module;

        }
        //-------------------------------------------

        public bool WuDi()
        {
            LogMgr.UnityLog("call GmMgr WuDi");
            return true;
        }

        public bool MiaoGuai()
        {
            LogMgr.UnityLog("call GmMgr MiaoGuai");

            SkillMgr gSkillMgr = CoreEntry.gSkillMgr;
            if (gSkillMgr.m_bMiaoGuai)
            {
                gSkillMgr.m_bMiaoGuai = false;
            }
            else
            {
                gSkillMgr.m_bMiaoGuai = true;
            }

            return true;
        }

        public bool WuShang()
        {
            LogMgr.UnityLog("call GmMgr WuShang");

            SkillMgr gSkillMgr = CoreEntry.gSkillMgr;
            if (gSkillMgr.m_bWuShang)
            {
                gSkillMgr.m_bWuShang = false;
            }
            else
            {
                gSkillMgr.m_bWuShang = true;
            }



            return true;
        }

        //public bool RecordNet()
        //{
        //    LogMgr.UnityLog("call GmMgr RecordNet");

        //    if (CoreEntry.netMgr.m_recordNet)
        //    {
        //        CoreEntry.netMgr.StopRecordingNet();
        //    }
        //    else
        //    {
        //        CoreEntry.netMgr.StartRecordingNet();
        //    }



        //    return true;
        //}

        public bool SwitchSoundEffect()
        {
            LogMgr.UnityLog("call GmMgr Sound Effect");

            if (CoreEntry.SoundEffectMgr.OnlyPlayOneSample)
            {
                CoreEntry.SoundEffectMgr.OnlyPlayOneSample = false;
            }
            else
            {
                CoreEntry.SoundEffectMgr.OnlyPlayOneSample = true;
            }



            return true;
        }

        //bool closeAI = false;
        public bool SwitchAI()
        {
            LogMgr.UnityLog("call GmMgr Switch ai");
            return false;
        }

        public bool ShowSkillScope()
        {
            LogMgr.UnityLog("call GmMgr ShowSkillScope");

            SkillMgr gSkillMgr = CoreEntry.gSkillMgr;
            if (gSkillMgr.m_bShowSkillScope)
            {
                gSkillMgr.m_bShowSkillScope = false;
            }
            else
            {
                gSkillMgr.m_bShowSkillScope = true;
            }

            return true;
        }

        public void GmReq(string strGMCmd)
        {
            //LogMgr.UnityLog("Do GM:" + strGMCmd);
            // mGmReq.szContent = System.Text.Encoding.Default.GetBytes(strGMCmd);
            //  CoreEntry.networkMgr.Send(mReq);

            SendGMCommand((ulong)CoreEntry.gActorMgr.MainPlayer.ServerID, strGMCmd);
        }

        public void SendGMCommand(ulong uid, string command)
        {

            MsgData_GMCommand data = new MsgData_GMCommand();
            data.uid = uid;
            var bytes = System.Text.Encoding.UTF8.GetBytes(command);
            data.length = bytes.Length;
            data.command.AddRange(bytes);
            CoreEntry.netMgr.send(NetMsgDef.C_GMCOMMAND, data);
        }
#endif

    }
}

