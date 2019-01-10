/**
* @file     :  
* @brief    : 
* @details  :  
* @author   : 
* @date     : 
*/

using UnityEngine;
using System.Collections;
using XLua;
using System.Collections.Generic;
using System;


namespace SG
{


    
	// S->C 物品列表
    [LuaCallCSharp]
    [Hotfix]
    public class  MailItemVo 
    {
        public int itemid; // 附件物品id
        public long itemcount; // 附件物品数量
    }

    [LuaCallCSharp]
    [Hotfix]
    public class OpenMailResult
    {
        public byte[] contnet = new byte[512]; // 邮件内容 type=1 'param1:type1,param2:type2
        public MailItemVo[] items = new MailItemVo[8]; // 邮件列表

        public string GetContnet()
        {
            return CommonTools.BytesToString(contnet);
        }
        public int GetItemCount()
        {
            int j = 0;
            for (int i = 0; i < items.Length; i++ )
            {
                if (items[i] != null && items[i].itemid != 0 && items[i].itemcount != 0 )
                    j++;
            }
            return j;
        }

    }

    // S->C 邮件信息
    [LuaCallCSharp]
    [Hotfix]
    public class MailVo
    {
        public long id; // 邮件id
        public sbyte read; // 是否读过 0 - 未读， 1 - 已读
        public sbyte item; // 是否领取过附件0 - 没有附件， 1 - 未领取附件， 2 - 已领取附件
        public long sendTime; // 发件时间
        public long leftTime; // 剩余时间
        public byte[] mailtitle = new byte[50]; // 邮件标题
        public int mailTxtId; // 配表id
        public OpenMailResult openMail = null;
        public string GetTitle()
        {
             return   CommonTools.BytesToString(mailtitle);
        }
        public DateTime GetSendTime()
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));

            startTime = startTime.AddSeconds(sendTime);

            //startTime = startTime.AddHours(8);//转化为北京时间(北京时间=UTC时间+8小时 )            

            return startTime;        
        }

    }
    [LuaCallCSharp]
    [Hotfix]
    public class MailDataMgr
    {
        //private static MailDataMgr _instance = null;
        public static MailDataMgr Instance
        {
            get
            {
                return PlayerData.Instance.MailDataMgr;
            }
        }

        Dictionary<long, MailVo> mMail = new Dictionary<long, MailVo>();
        public Dictionary<long, MailVo> Mail
        {
            get { return mMail; }
            set { mMail = value; }
        } 
#region msg
        public void ResisterMsg()
        {
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_GetMailResult, GE_GetMailResult);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_OpenMailResult, GE_OpenMailResult);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_GetMailItemResult, GE_GetMailItemResult);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_DelMail, GE_DelMail);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_NotifyMail, GE_NotifyMail);
        }

        public void GE_GetMailResult(GameEvent ge, EventParameter param)
        {
            MsgData_sGetMailResult data = param.msgParameter as MsgData_sGetMailResult;
            for (int i = 0; i < data.items.Count; i++ )
            {
                UpdateMailVo(data.items[i]);
            }
        }
        public void GE_OpenMailResult(GameEvent ge, EventParameter param)
        {
            MsgData_sOpenMailResult data = param.msgParameter as MsgData_sOpenMailResult;
            MailVo item = GetMailVo(data.id);
            if (item != null)
            {
                item.openMail = new OpenMailResult();
                item.item = data.item;
                item.read = 1;
                Array.Copy(data.contnet, item.openMail.contnet, data.contnet.Length);
                for (int i = 0; i < item.openMail.items.Length; i++)
                {
                    if (data.items[i] != null)
                    {
                        item.openMail.items[i] = new MailItemVo();
                        item.openMail.items[i].itemid = data.items[i].itemid;
                        item.openMail.items[i].itemcount = data.items[i].itemcount;
                    }
                } 
            }
        }
        public void GE_GetMailItemResult(GameEvent ge, EventParameter param)
        {
            MsgData_sGetMailItemResult data = param.msgParameter as MsgData_sGetMailItemResult;
            for (int i = 0; i < data.items.Count; i++ )
            {
                if (data.items[i].result == -1) continue;
                MailVo item = GetMailVo(data.items[i].id);
                if (item != null)
                {
                    item.item = data.items[i].result;
                }
            }
        }
        public void GE_DelMail(GameEvent ge, EventParameter param)
        {
            MsgData_sDelMail data = param.msgParameter as MsgData_sDelMail;
            for (int i = 0; i < data.items.Count; i++)
            {
                mMail.Remove(data.items[i].id);
            }
        }
        public void GE_NotifyMail(GameEvent ge, EventParameter param)
        {
            MsgData_sNotifyMail data = param.msgParameter as MsgData_sNotifyMail;
            if (data.count > 0)
            {
                //NetLogicGame.Instance.SendReqGetMailList();
            }
        } 
#endregion
        //副本信息
        public void UpdateMailVo(MsgData_sMailVo item)
        {
            MailVo v = new MailVo();
            v.id = item.id;
            v.item = item.item;
            v.read = item.read;
            v.sendTime = item.sendTime;
            v.leftTime = item.leftTime;
            Array.Copy(item.mailtitle, v.mailtitle, v.mailtitle.Length);
            v.mailtitle = item.mailtitle;
            v.mailTxtId = item.mailTxtId;
            if (mMail.ContainsKey(v.id))
            {
                mMail[v.id] = v;
            }
            else
            {
                mMail.Add(v.id, v);
            }
        }
        public bool MainRed()
        {
            if (mMail == null)
                return false;
            bool bRes = false;
            Dictionary<long, MailVo>.Enumerator iter = mMail.GetEnumerator();
            while(iter.MoveNext())
            {
                MailVo item = iter.Current.Value;
                if(item.item == 1)
                {
                    bRes = true;
                    break;
                }
            } 
            return bRes;
        }

        public MailVo GetMailVo(long id)
        {
            MailVo v = null;
            if (mMail.ContainsKey(id))
            {
                v = mMail[id];
            }
            else
            {
                v = new MailVo(); 
                v.id = id;
                v.read = 0;
                v.sendTime = 0;
                v.leftTime = 0;
                v.mailTxtId = 0;
            }
            return v; 
        }
 

        public List<long> GetMail()
        {

            List<long> list = new List<long>(mMail.Keys);
            list.Sort(delegate (long a, long b) 
            {
                MailVo ia =MailDataMgr.Instance.Mail[a];
                MailVo ib = MailDataMgr.Instance.Mail[b];
 
                if(ia.read == ib.read)
                {
                    return -ia.sendTime.CompareTo(ib.sendTime);
                }
                else
                {
                    return ia.read.CompareTo(ib.read);                    
                }
            });
            return list; 
        }

        //public sbyte read; // 是否读过 0 - 未读， 1 - 已读
        //public sbyte item; // 是否领取过附件0 - 没有附件， 1 - 未领取附件， 2 - 已领取附件

        ///查询是否有未领取的附件
        public bool IsCanEnclosure()
        {
            foreach(var v in mMail.Values) {
                if (v.item == 1) return true;
            }
            return false;
        }
        ///查询是否有未读取的邮件
        public bool IsHaveUnRead()
        {
            foreach(var v in mMail.Values) {
                if (v.read == 0) return true;
            }
            return false;
        }
        ///查询某一封邮件是否已领取
        public bool IsOneEnclosure(long id)
        {
            if(mMail.ContainsKey(id)) {
                return mMail[id].item == 1;
            }
            return false;
        }

        ///重置邮件 用于重新登录
        public void ReSet()
        {
            mMail.Clear();
        }

    }
}