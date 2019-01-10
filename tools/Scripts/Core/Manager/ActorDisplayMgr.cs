using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
[Hotfix]
    public class ActorDisplayMgr
    {
        private static ActorDisplayMgr _instance = null;
        public static ActorDisplayMgr Instance
        {
            get
            {
                if (null == _instance)
                {
                    _instance = new ActorDisplayMgr();
                }

                return _instance;
            }
        }

        private List<OtherPlayer>[] mOtherPlayers;

        public void Init()
        {
            mOtherPlayers = new List<OtherPlayer>[5];
            for (int i = 0; i < 5; i++)
            {
                mOtherPlayers[i] = new List<OtherPlayer>();
            }

            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SETTING_ACTORDISPLAYNUM, OnEvent);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_OTHERPLAYER_LOAD_OVER, OnEvent);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_OTHERPLAYER_LEAVE, OnEvent);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_TEAM_INFO, OnEvent);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_TEAM_EXIT, OnEvent);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_TEAM_JOIN, OnEvent);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_FRIEND_INFO, OnEvent);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_FRIEND_REMOVE_RELATION, OnEvent);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_GUILD_INFO, OnEvent);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_GUILD_DISMISS, OnEvent);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_GUILD_KICK_MEMBER, OnEvent);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_GUILD_QUIT, OnEvent);
        }

        private void OnEvent(GameEvent ge, EventParameter parameter)
        {
            RefreshDisplayActors();
        }

        private void RefreshDisplayActors()
        {
            int i = 0;
            for (i = 0; i < 5; i++)
            {
                mOtherPlayers[i].Clear();
            }

            int displayNum = SettingManager.Instance.ActorDisplayNum;
            bool showModel = CoreEntry.bModelShow;
            List<ActorObj> actors = CoreEntry.gActorMgr.GetAllPlayerActors();
            for (i = 0; i < actors.Count; i++)
            {
                ActorObj actor = actors[i];
                if (actor is PlayerObj)
                {
                    continue;
                }

                OtherPlayer otherActor = actor as OtherPlayer;
                if(otherActor != null && otherActor.mActorType == ActorType.AT_MECHANICS) //镖车类不隐藏
                {
                    continue;
                }

                if (null != PlayerData.Instance.TeamData.GetTeamRole(otherActor.ServerID))
                {
                    mOtherPlayers[0].Add(otherActor);

                    continue;
                }

                if (PlayerData.Instance.FriendData.IsEnemy(otherActor.ServerID))
                {
                    mOtherPlayers[1].Add(otherActor);

                    continue;
                }

                if (PlayerData.Instance.FriendData.IsFriend(otherActor.ServerID))
                {
                    mOtherPlayers[2].Add(otherActor);

                    continue;
                }

                if (PlayerData.Instance.GuildData.IsInGuild(otherActor.ServerID))
                {
                    mOtherPlayers[3].Add(otherActor);

                    continue;
                }

                mOtherPlayers[4].Add(otherActor);
            }

            displayNum--;

            int tmpNum = 0;
            int count = 0;
            for (i = 0; i < 5; i++)
            {
                List<OtherPlayer> list = mOtherPlayers[i];
                count = list.Count;
                tmpNum = displayNum < count ? displayNum : count;
                if (!showModel)
                {
                    tmpNum = 0;
                }
                displayNum -= tmpNum;
                int j = 0;
                for (j = 0; j < tmpNum; j++)
                {
                    list[j].ShowModel();
                }
                for (; j < count; j++)
                {
                    list[j].HideModel();
                }
            }
        }
    }
}

