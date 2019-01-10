/**
* @file     : EventLoggerMgr
* @brief    : 
* @details  : 
* @author   : CW
* @date     : 2017-09-21
*/
using XLua;
using UnityEngine;
using System.Collections.Generic;

namespace SG
{
[Hotfix]
    public class EventLoggerMgr
    {
        private static EventLoggerMgr _instance = null;
        public static EventLoggerMgr Instance
        {
            get
            {
                if (null == _instance)
                    _instance = new EventLoggerMgr();

                return _instance;
            }
        }

        private List<IEventLogger> loggerList = new List<IEventLogger>();
        private EventLoggerMgr()
        {

        }

        /// <summary>
        /// ��ʼ��
        /// </summary>
        /// <param name="channelId"></param>
        public void Init(string channelId)
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    loggerList[i].Init(channelId);
                }
            }
        }

        /// <summary>
        /// ��¼
        /// </summary>
        public void Login()
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    //loggerList[i].Login();
                }
            }
        }

        /// <summary>
        /// ע��
        /// </summary>
        public void Register()
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    //loggerList[i].Login();
                }
            }
        }

        /// <summary>
        /// ������ɫ
        /// </summary>
        public void CreateRole()
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    //loggerList[i].CreateRole();
                }
            }
        }

        /// <summary>
        /// �ǳ�
        /// </summary>
        public void Logout()
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    //loggerList[i].Logout();
                }
            }
        }

        /// <summary>
        /// �˳���Ϸ
        /// </summary>
        public void Exit()
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    //loggerList[i].Exit();
                }
            }
        }

        /// <summary>
        /// ��Ϸ�汾��
        /// </summary>
        /// <param name="version"></param>
        public void Version(string version)
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    loggerList[i].Agent_SetVersion(version);
                }
            }
        }

        /// <summary>
        /// ��ɫ�ȼ�
        /// </summary>
        /// <param name="level"></param>
        public void Level(int level)
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    loggerList[i].SetLevel(level);
                }
            }
        }

        /// <summary>
        /// ��ʼ֧��
        /// </summary>
        public void PayStart()
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    
                }
            }
        }

        /// <summary>
        /// ֧���ɹ�
        /// </summary>
        public void LogPaySuccess()
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {

                }
            }
        }

        /// <summary>
        /// ֧��ʧ��
        /// </summary>
        public void LogPayFail()
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {

                }
            }
        }

        /// <summary>
        /// �Ƿ�ȡ��
        /// </summary>
        public void LogPayCanel()
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {

                }
            }
        }

        /// <summary>
        /// �Զ����¼�
        /// </summary>
        /// <param name="eventId">�¼�����</param>
        public void OnEvent(string eventId)
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    loggerList[i].OnEvent(eventId);
                }
            }
        }

        /// <summary>
        /// �Զ����¼�
        /// </summary>
        /// <param name="eventId">�¼�����</param>
        /// <param name="label">�¼�����</param>
        public void OnEvent(string eventId, string label)
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    loggerList[i].OnEvent(eventId, label);
                }
            }
        }

        /// <summary>
        /// �������Զ����¼�
        /// </summary>
        /// <param name="eventId">�¼�����</param>
        /// <param name="map">�¼���Ҫ��ע�����Լ���</param>
        /// HashMap map = new HashMap();
        ///map.put("item","skill book"); //������
        ///map.put("count","4"); //����
        ///DCEvent.onEvent("cash_gift" ,map);
        ///
        public void OnEvent(string eventId, System.Collections.Generic.Dictionary<string, string> map)
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    loggerList[i].OnEvent(eventId, map);
                }
            }
        }

        /// <summary>
        /// �Զ����¼�
        /// </summary>
        /// <param name="eventId">�¼�����</param>
        /// <param name="count">�¼���������</param>
        public void OnEventCount(string eventId, int count)
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    loggerList[i].OnEventCount(eventId, count);
                }
            }
        }

        /// <summary>
        /// ͳ���Զ����¼���ʹ��ʱ��
        /// </summary>
        /// <param name="eventId">�¼�����</param>
        /// <param name="duration">�¼�ʱ��������ָ�¼��������������ĵ�ʱ�䣬Ҳ����ָ�¼����������У�ĳһ�����̵�ʱ����������Ҫ������ʱ�Ķ��塣ʱ���ĵ�λ����</param>
        public void OnEventDuration(string eventId, long duration)
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    loggerList[i].OnEventDuration(eventId, duration);
                }
            }
        }

        /// <summary>
        /// ͳ���Զ����¼���ʹ��ʱ��
        /// </summary>
        /// <param name="eventId">�¼�����</param>
        /// <param name="label">�¼�����</param>
        /// <param name="duration">�¼�ʱ��������ָ�¼��������������ĵ�ʱ�䣬Ҳ����ָ�¼����������У�ĳһ�����̵�ʱ����������Ҫ������ʱ�Ķ��塣ʱ���ĵ�λ����</param>
        public void OnEventDuration(string eventId, string label, long duration)
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    loggerList[i].OnEventDuration(eventId, label, duration);
                }
            }
        }

        /// <summary>
        /// ͳ�ƶ������Զ����¼���ʹ��ʱ��
        /// </summary>
        /// <param name="eventId">�¼�����</param>
        /// <param name="map">�¼���Ҫ��ע�����Լ���</param>
        /// <param name="duration">�¼�ʱ��������ָ�¼��������������ĵ�ʱ�䣬Ҳ����ָ�¼����������У�ĳһ�����̵�ʱ����������Ҫ������ʱ�Ķ��塣ʱ���ĵ�λ����</param>
        public void OnEventDuration(string eventId, System.Collections.Generic.Dictionary<string, string> map, long duration)
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    loggerList[i].OnEventDuration(eventId, map, duration);
                }
            }
        }

        /// <summary>
        /// �Զ����¼���ʼ
        /// </summary>
        /// <param name="eventId">����Ϊ��Ҳ����Ϊ�մ�,�¼�����</param>
        public void OnEventBegin(string eventId)
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    loggerList[i].OnEventBegin(eventId);
                }
            }
        }

        /// <summary>
        /// �Զ����¼���ʼ
        /// </summary>
        /// <param name="eventId">�¼�����,����Ϊ��Ҳ����Ϊ�մ�</param>
        /// <param name="map">�¼���Ҫ��ע�����Լ���,����Ϊ��</param>
        public void OnEventBegin(string eventId, System.Collections.Generic.Dictionary<string, string> map)
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    loggerList[i].OnEventBegin(eventId, map);
                }
            }
        }

        /// <summary>
        /// �Զ����¼���ʼ,�����ӿ���Ҫ��DCEvent.onEventEnd(String eventId,String flag)����ʹ��
        /// </summary>
        /// <param name="eventId">�¼�����</param>
        /// <param name="map">�¼���Ҫ��ע�����Լ���</param>
        /// <param name="flag">��ͬһ��EventId���¼�ͬʱ������ʱ����SDKͨ��flag���ݽ������֣��Ա�������ͬʱ��¼������ͬEventId�¼������󡣸��ֶβ����ϱ�,����Ϊ�գ�����Ϊ�մ�</param>
        public void OnEventBegin(string eventId, System.Collections.Generic.Dictionary<string, string> map, string flag)
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    loggerList[i].OnEventBegin(eventId, map, flag);
                }
            }
        }

        /// <summary>
        /// �Զ����¼�����, ��Ҫ�ڵ���DCEvent.onEventBegin(String eventId)��DCEvent.onEventBegin(String eventId, Map map)�ӿ�֮���ſɵ��ñ��ӿڣ�����������Ч
        /// </summary>
        /// <param name="eventId">�¼�����</param>
        public void OnEventEnd(string eventId)
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    loggerList[i].OnEventEnd(eventId);
                }
            }
        }

        /// <summary>
        /// �Զ����¼�����, ��Ҫ�ڵ��ýӿ�DCEvent.onEventBegin(eventId, map, flag)֮�����øýӿڣ�����������Ч
        /// </summary>
        /// <param name="eventId">�¼�����</param>
        /// <param name="flag">��ͬһ��EventId���¼�ͬʱ������ʱ����SDKͨ��flag���ݽ������֣��Ա�������ͬʱ��¼������ͬEventId�¼������󡣸��ֶβ����ϱ�</param>
        public void OnEventEnd(string eventId, string flag)
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    loggerList[i].OnEventEnd(eventId, flag);
                }
            }
        }

        /// <summary>
        /// ��¼ǰ�Զ����¼�
        /// </summary>
        /// <param name="eventId">������</param>
        /// <param name="dic">�¼���Ҫ��ע�����Լ���</param>
        /// <param name="duration">�¼�ʱ��������ָ�¼��������������ĵ�ʱ�䣬Ҳ����ָ�¼����������У�ĳһ�����̵�ʱ����������Ҫ������ʱ�Ķ��塣ʱ���ĵ�λ����</param>
        public void OnEventBeforeLogin(string eventId, System.Collections.Generic.Dictionary<string, string> dic, long duration)
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    loggerList[i].OnEventBeforeLogin(eventId, dic, duration);
                }
            }
        }

        /// <summary>
        /// ���������˺����ͣ���Ϊ�罻�������˺�����
        /// </summary>
        /// <param name="type"></param>
        public void SetAccountType(DAAccountType type)
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    loggerList[i].SetAccountType(type);
                }
            }
        }

        /// <summary>
        /// ���������Ա�
        /// </summary>
        /// <param name="gender"></param>
        public void SetGender(DAGender gender)
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    loggerList[i].SetGender(gender);
                }
            }


        }

        /// <summary>
        /// ������������
        /// </summary>
        /// <param name="age"></param>
        public void SetAge(int age)
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    loggerList[i].SetAge(age);
                }
            }
        }

        /// <summary>
        /// ������Ϸ����
        /// </summary>
        /// <param name="gameServer">��������</param>
        public void SetGameServer(string gameServer)
        {
            if (gameServer != null)
            {
                for (int i = 0; i < loggerList.Count; ++i)
                {
                    if (loggerList[i] != null)
                    {
                        loggerList[i].SetGameServer(gameServer);
                    }
                }

            }
        }

        /// <summary>
        /// ���������±�ǩ
        /// </summary>
        /// <param name="tag">һ����ǩ,����Ϊ��</param>
        /// <param name="subTag">������ǩ,����Ϊ��</param>
        public void AddTag(string tag, string subTag)
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    loggerList[i].AddTag(tag, subTag);
                }
            }
        }

        /// <summary>
        /// �Ƴ����ұ�ǩ
        /// </summary>
        /// <param name="tag">һ����ǩ.����Ϊ�գ�����Ϊ�մ�</param>
        /// <param name="subTag">������ǩ,����Ϊ��</param>
        /// DCAccount.addTag("��������", "");
        /// DCAccount.removeTag("vip", "vip1");
        /// DCAccount.addTag("vip", "vip3");
        ///
        public void RemoveTag(string tag, string subTag)
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    loggerList[i].RemoveTag(tag, subTag);
                }
            }
        }

        /// <summary>
        /// ��Ϸ�ؿ���ʼ
        /// </summary>
        /// <param name="levelId"></param>
        public void Level_Begin(string levelId)
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    loggerList[i].Level_Begin(levelId);
                }
            }
        }

        /// <summary>
        /// ��Ϸ�ؿ�ͨ��
        /// </summary>
        /// <param name="levelId"></param>
        public void Level_Complete(string levelId, int newLv)
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    loggerList[i].Level_Complete(levelId);
                }
            }
        }

        /// <summary>
        /// ��Ϸ�ؿ�ͨ��ʧ��
        /// </summary>
        /// <param name="levelId"></param>
        /// <param name="reason"></param>
        public void Level_Fail(string levelId, string reason)
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    loggerList[i].Level_Fail(levelId, reason);
                }
            }
        }

        /// <summary>
        /// ������ʼ
        /// </summary>
        /// <param name="taskId">��Ϸ��������</param>
        /// <param name="type"></param>
        public void Task_Begin(string taskId, DATaskType type)
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    loggerList[i].Task_Begin(taskId, type);
                }
            }
        }

        /// <summary>
        /// �������ɣ���DCTask.begin����ʹ��
        /// </summary>
        /// <param name="taskId"></param>
        public void Task_Complete(string taskId, DATaskType type)
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    loggerList[i].Task_Complete(taskId, type);
                }
            }
        }

        /// <summary>
        /// ����ʧ�ܣ���DCTask.begin����ʹ��
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="reason"></param>
        public void Task_Fail(string taskId, string reason, DATaskType type)
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    loggerList[i].Task_Fail(taskId, reason, type);
                }
            }
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="itemId">���߱���</param>
        /// <param name="itemType">��������</param>
        /// <param name="itemCnt">�����ĵ�������</param>
        /// <param name="vituralCurrency">�����������ĵ�����������</param>
        /// <param name="currencyType">����������</param>
        /// <param name="consumePoint">���ѵ㣬����ʹ�ùؿ�����������ʶ�����߼�¼���ѳ���</param>
        public void Item_Buy(string itemId, string itemType, int itemCnt, int vituralCurrency, string currencyType, string consumePoint)
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    loggerList[i].Item_Buy(itemId, itemType, itemCnt, vituralCurrency, currencyType, consumePoint);
                }
            }
        }

        /// <summary>
        /// ���õ���
        /// </summary>
        /// <param name="itemId">���߱���</param>
        /// <param name="itemType">��������</param>
        /// <param name="itemCount">�����ĵ�������</param>
        /// <param name="reason">���õ��ߵ�ԭ��</param>
        public void Item_Get(string itemId, string itemType, int itemCount, string reason)
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    loggerList[i].Item_Get(itemId, itemType, itemCount, reason);
                }
            }
        }

        /// <summary>
        /// ���ĵ���
        /// </summary>
        /// <param name="itemId">���߱���</param>
        /// <param name="itemType">��������</param>
        /// <param name="itemCount">��������</param>
        /// <param name="reason">���ĵ��ߵ�ԭ��</param>
        public void Item_Use(string itemId, string itemType, int itemCount, string reason)
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    loggerList[i].Item_Use(itemId, itemType, itemCount, reason);
                }
            }
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="itemId">���߱���</param>
        /// <param name="itemType">��������</param>
        /// <param name="itemCnt">�����ĵ�������</param>
        /// <param name="vituralCurrency">�����������ĵ�����������</param>
        /// <param name="currencyType">����������</param>
        /// <param name="consumePoint">���ѵ㣬����ʹ�ùؿ�����������ʶ�����߼�¼���ѳ���</param>
        /// <param name="levelId">��ǰ�������ڹؿ�</param>
        public void Item_BuyInLevel(string itemId, string itemType, int itemCnt, int vituralCurrency, string currencyType, string consumePoint, string levelId)
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    loggerList[i].Item_BuyInLevel(itemId, itemType, itemCnt, vituralCurrency, currencyType, consumePoint, levelId);
                }
            }
        }

        /// <summary>
        /// �ؿ��ڻ��õ���
        /// </summary>
        /// <param name="itemId">���߱���</param>
        /// <param name="itemType">��������</param>
        /// <param name="itemCnt">�����ĵ�������</param>
        /// <param name="reason">���õ��ߵ�ԭ��</param>
        /// <param name="levelId">��ǰ�������ڹؿ�</param>
        public void Item_GetInLevel(string itemId, string itemType, int itemCnt, string reason, string levelId)
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    loggerList[i].Item_GetInLevel(itemId, itemType, itemCnt, reason, levelId);
                }
            }
        }

        /// <summary>
        /// �ؿ������ĵ���
        /// </summary>
        /// <param name="itemId">���߱���</param>
        /// <param name="itemType">��������</param>
        /// <param name="itemCnt">�����ĵ�������</param>
        /// <param name="reason">���õ��ߵ�ԭ��</param>
        /// <param name="levelId">��ǰ�������ڹؿ�</param>
        public void Item_UseInLevel(string itemId, string itemType, int itemCnt, string reason, string levelId)
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    loggerList[i].Item_UseInLevel(itemId, itemType, itemCnt, reason, levelId);
                }
            }
        }



        /// <summary>
        /// ��¼�������������Ĳ�ͬʱ��¼��������������
        /// </summary>
        /// <param name="reason">�������Ľ���ԭ��</param>
        /// <param name="coinType">����������</param>
        /// <param name="lost">�������ĵ��������������벻Ҫ���븺ֵ</param>
        /// <param name="left">���ҿ۳����ĺ�������������</param>
        public void Coin_Use(string reason, string coinType, long lost, long left)
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    loggerList[i].Coin_Use(reason, coinType, lost, left);
                }
            }
        }

        /// <summary>
        /// ��¼���������һ�ȡ��ͬʱ��¼��������������
        /// </summary>
        /// <param name="reason">���һ�ȡ����ԭ��</param>
        /// <param name="coinType">����������</param>
        /// <param name="gain">���һ�ȡ���������������벻Ҫ���븺ֵ</param>
        /// <param name="left">���Ҽ��ϻ�ȡ��������������</param>
        public void Coin_Get(string reason, string coinType, long gain, float itemTotalPrice, long left)
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    loggerList[i].Coin_Get(reason, coinType, gain, left, itemTotalPrice);
                }
            }
        }

        /// <summary>
        /// ��¼������������������
        /// </summary>
        /// <param name="coinNum">����������</param>
        /// <param name="coinType">����������</param>
        public void Coin_SetCoinNum(long coinNum, string coinType)
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    loggerList[i].Coin_SetCoinNum(coinNum, coinType);
                }
            }
        }

        /// <summary>
        /// �ؿ�������������
        /// </summary>
        /// <param name="reason">�������Ľ���ԭ��</param>
        /// <param name="coinType">����������</param>
        /// <param name="lost">�������ĵ��������������벻Ҫ���븺ֵ</param>
        /// <param name="left">���Ҽ��ϻ�ȡ��������������</param>
        /// <param name="levelId">��ǰ�������ڹؿ�</param>
        public void Coin_Use(string reason, string coinType, long lost, long left, string levelId)
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    loggerList[i].Coin_Use(reason, coinType, lost, left, levelId);
                }
            }
        }

        /// <summary>
        /// �ؿ��ڻ�ȡ������
        /// </summary>
        /// <param name="reason">���һ�ȡ����ԭ��</param>
        /// <param name="coinType">����������</param>
        /// <param name="gain">���һ�ȡ���������������벻Ҫ���븺ֵ</param>
        /// <param name="left">���Ҽ��ϻ�ȡ��������������</param>
        /// <param name="levelId">��ǰ�������ڹؿ�</param>
        public void Coin_Get(string reason, string coinType, long gain, long left, float itemTotalPrice, string levelId)
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    loggerList[i].Coin_Get(reason, coinType, gain, left, itemTotalPrice, levelId);
                }
            }
        }



        /// <summary>
        /// ����ConfigParamsUpdateListener��������������DCConfigParams.update�ӿ�ִ�����ɻ��ص�ConfigParamsUpdateListener��������callback�ӿ�.��Ҫ��DCConfigParams.update�ӿڵ���֮ǰ����.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="functionName"></param>
        public void ConfigParams_SetUpdateCallBack(string obj, string functionName)
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    loggerList[i].ConfigParams_SetUpdateCallBack(obj, functionName);
                }
            }
        }

        /// <summary>
        /// �������Դӷ�������ȡ���߲���,�����浽����.����������Ŀ�л�ȡĳ�������Ĳ������Ե���DCConfigParams.getParameterString(String key,String defaultValue);Key ��Ҫ������Dataeye��̨���ú��ſɻ�ȡ����Ҫ��SDK��ʼ��֮������
        /// </summary>
        public void ConfigParams_Update()
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    loggerList[i].ConfigParams_Update();
                }
            }
        }

        /// <summary>
        /// ��ȡ�ַ�����ֵ
        /// </summary>
        /// <param name="key">���߲����Ļ�ȡ��ֵ</param>
        /// <param name="defaultValue"></param>
        public void ConfigParams_GetParameterString(string key, string defaultValue)
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    loggerList[i].ConfigParams_GetParameterString(key, defaultValue);
                }
            }
        }

        /// <summary>
        /// ��ȡInt��ֵ
        /// </summary>
        /// <param name="key">���߲����Ļ�ȡ��ֵ</param>
        /// <param name="defaultValue"></param>
        public void ConfigParams_GetParameterInt(string key, int defaultValue)
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    loggerList[i].ConfigParams_GetParameterInt(key, defaultValue);
                }
            }
        }

        /// <summary>
        /// ��ȡ��������ֵ
        /// </summary>
        /// <param name="key">���߲����Ļ�ȡ��ֵ</param>
        /// <param name="defaultValue"></param>
        public void ConfigParams_GetParameterLong(string key, long defaultValue)
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    loggerList[i].ConfigParams_GetParameterLong(key, defaultValue);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">���߲����Ļ�ȡ��ֵ</param>
        /// <param name="defaultValue"></param>
        public void ConfigParams_GetParameterBoolean(string key, bool defaultValue)
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    loggerList[i].ConfigParams_GetParameterBoolean(key, defaultValue);
                }
            }
        }

        /// <summary>
        /// ��������Ч�����⹦��
        /// </summary>
        public void Agent_OpenAdTracking()
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    loggerList[i].Agent_OpenAdTracking();
                }
            }
        }






        /// <summary>
        /// �����Զ������ϱ�����
        /// </summary>
        /// <param name="second">�ϱ����ڼ���.��λ����.SDKĬ�ϵ��ϱ�������60��. �����÷�Χ��30�뵽12Сʱ(12*60*60��)</param>
        public void Agent_SetUploadInterval(uint second)
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    loggerList[i].Agent_SetUploadInterval(second);
                }
            }
        }

        /// <summary>
        /// ����ִ�������ϱ�
        /// </summary>
        public void Agent_UploadNow()
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    loggerList[i].Agent_UploadNow();
                }
            }
        }

        /// <summary>
        /// ��ȡ�豸����
        /// </summary>
        public void Agent_GetUID()
        {
            for (int i = 0; i < loggerList.Count; ++i)
            {
                if (loggerList[i] != null)
                {
                    loggerList[i].Agent_GetUID();
                }
            }
        }
    }
}

