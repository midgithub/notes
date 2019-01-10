/**
* @file     : EnterSceneStruct.cs
* @brief    : 进入场景对象实体
* @details  : 进入场景对象实体
* @author   : CW
* @date     : 2017-06-14
*/
using XLua;
using UnityEngine;
using System.Collections;

namespace SG
{
[Hotfix]
    public class PatrolEnterStruct
    {
        public long Guid;
        public byte ObjType;
        public int PosX;
        public int PosY;
        public int Dir;

        public int ConfigID;
		public byte[] Man = new byte[32];
        public byte[] Woman = new byte[32];
    }

[Hotfix]
    public class DukeEnterStruct
    {
        public long Guid;
        public byte ObjType;
        public int PosX;
        public int PosY;
        public int Dir;

        public byte[] Rolename = new byte[32];
		public int Prof;  
		public byte Sex;
		public int Dress; 
		public int Arms;
		public int Head;
		public int Suit;
		public int Weapon;
		public int Level;
		public int WuHunId;
		public long GuildID;
        public byte[] GuildName = new byte[32];
		public int Ride;
		public byte Icon;
        public byte VipLv;
		public int ShenBin;
        public int Realm;
        public int Vplan;
        public int ActPet;
        public int Wing;
    }

[Hotfix]
    public class BiaoCheEnterStruct
    {
        public long Guid;
        public byte ObjType;
        public int PosX;
        public int PosY;
        public int Dir;

        public int Id;
		public int Camp;
		public int Level;
		public int Speed;
		public int Hp;
        public byte[] Name = new byte[32];
    }

    //public class MsgData_sSceneObjectEnterStaticObj
    //{
    //    public long Guid;
    //    public byte ObjType;
    //    public int PosX;
    //    public int PosY;
    //    public int Dir;

    //    public int ConfigID;
    //}
}

