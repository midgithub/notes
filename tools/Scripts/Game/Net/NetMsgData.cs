//NetMsgData.cs
//协议工具自动生成

using System;
using System.Text;
using System.Security.Cryptography;
using System.Net;
using System.Collections.Generic;
using UnityEngine;


namespace SG
{
	// C->S 心跳 msgId:2136;
	public class MsgData_cHeartBeat : MsgData
	{
		public long Time;	// 时间

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(Time);
		}
	}

	// S->C 心跳 msgId:7136;
	public class MsgData_sHeartBeat : MsgData
	{
		public long Time;	// 时间

		public override void unpack(NetReadBuffer buffer)
		{
			Time = buffer.ReadInt64();
		}
	}

	// C->S GM命令 msgId:23;
	public class MsgData_GMCommand : MsgData
	{
		public ulong uid;	// 角色guid
		public int length;	// 命令长度
		public List<byte> command = new List<byte>();	// 命令

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteUInt64(uid);
			buffer.WriteInt32(length);
			buffer.WriteBytes(command);
		}
	}

	// C->S c->s 登录协议(CL_CONN_SRV)
	public class MsgData_cLogin : MsgData
	{
		public byte[] Account = new byte[64];	// 玩家帐号
		public byte[] Platform = new byte[32];	// 平台
		public byte[] GameName = new byte[32];	// 游戏名字
		public uint ServerID;	// 区服ID
		public uint ClientTime;	// 客户端时间
		public uint IsAdult;	// 防沉迷标记
		public byte[] Exts = new byte[64];	// 扩展信息
		public byte[] Sign = new byte[64];	// 签名信息
		public byte[] Mac = new byte[32];	// 本地mac地址
		public byte[] Version = new byte[33];	// 协议版本号
		public byte[] Channel = new byte[64];	// 渠道标识

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteBytes(Account);
			buffer.WriteBytes(Platform);
			buffer.WriteBytes(GameName);
			buffer.WriteUInt32(ServerID);
			buffer.WriteUInt32(ClientTime);
			buffer.WriteUInt32(IsAdult);
			buffer.WriteBytes(Exts);
			buffer.WriteBytes(Sign);
			buffer.WriteBytes(Mac);
			buffer.WriteBytes(Version);
			buffer.WriteBytes(Channel);
		}
	}

	// C->S 请求登录(麟游) msgId:1904
	public class MsgData_cLogin_LY : MsgData
	{
		public byte[] Mac = new byte[32];	// 本地mac地址
		public byte[] Version = new byte[33];	// 协议版本号
		public int serverid;	// // 区服ID
		public uint dataInfo;	// 
		public List<byte> data = new List<byte>();	// 数据长度

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteBytes(Mac);
			buffer.WriteBytes(Version);
			buffer.WriteInt32(serverid);
			buffer.WriteUInt32(dataInfo);
			buffer.WriteBytes(data);
		}
	}

	// S->C 返回西游数据检验数据 msgId:6007
	public class MsgData_sVerifyAccount : MsgData
	{
		public int dataSize;	// 数据长度
		public List<byte> data = new List<byte>();	// json数据

		public override void unpack(NetReadBuffer buffer)
		{
			dataSize = buffer.ReadInt32();
			buffer.ReadBytes(data, (int)dataSize);
		}
	}

	// C->S 请求登录(西游) msgId:1905
	public class MsgData_cLogin_XY : MsgData
	{
		public sbyte Platform;	// 平台 1.android 2.ios
		public sbyte flag;	// 重连标识 0 正常登录 1重连登录
		public byte[] Account = new byte[64];	// 玩家帐号
		public int serverid;	// // 区服ID
		public byte[] ipadd = new byte[24];	// ip地址
		public byte[] Channel = new byte[64];	// 渠道标识
		public byte[] iemi = new byte[33];	// IMEI原始值
		public byte[] mac = new byte[33];	// mac原始值(转大写)
		public byte[] openuuid = new byte[33];	// openuuid原始值ios
		public byte[] idfa = new byte[33];	// idfa原始值ios
		public byte[] androiduuid = new byte[33];	// 安卓原始值
		public byte[] DeviceBrand = new byte[33];	// 设备品牌
		public byte[] DeviceType = new byte[33];	// 设备型号
		public byte[] Version = new byte[33];	// 协议版本号
		public uint dataInfo;	// 
		public List<byte> data = new List<byte>();	// 数据长度

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt8(Platform);
			buffer.WriteInt8(flag);
			buffer.WriteBytes(Account);
			buffer.WriteInt32(serverid);
			buffer.WriteBytes(ipadd);
			buffer.WriteBytes(Channel);
			buffer.WriteBytes(iemi);
			buffer.WriteBytes(mac);
			buffer.WriteBytes(openuuid);
			buffer.WriteBytes(idfa);
			buffer.WriteBytes(androiduuid);
			buffer.WriteBytes(DeviceBrand);
			buffer.WriteBytes(DeviceType);
			buffer.WriteBytes(Version);
			buffer.WriteUInt32(dataInfo);
			buffer.WriteBytes(data);
		}
	}

	// C->S 请求登录(西游) msgId:1906
	public class MsgData_cLogin_XYList : MsgData
	{
		public sbyte Platform;	// 平台 1.android 2.ios
		public int serverid;	// // 区服ID
		public byte[] ipadd = new byte[24];	// ip地址
		public byte[] data = new byte[65];	// 数据长度

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt8(Platform);
			buffer.WriteInt32(serverid);
			buffer.WriteBytes(ipadd);
			buffer.WriteBytes(data);
		}
	}

	// C->S 请求创建支付订单 msgId:2915
	public class MsgData_cGetRechargeorder : MsgData
	{
		public int ItemID;	// 购买物品ID
		public int ItemNum;	// 购买数量
		public long ServerID;	// 服务器ID
		public sbyte PidSize;	// pid长度
		public sbyte ServerNameSize;	// 服务器名长度
		public byte[] Pid = new byte[128];	// pid
		public byte[] ServerName = new byte[128];	// 服务器名

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(ItemID);
			buffer.WriteInt32(ItemNum);
			buffer.WriteInt64(ServerID);
			buffer.WriteInt8(PidSize);
			buffer.WriteInt8(ServerNameSize);
			buffer.WriteBytes(Pid);
			buffer.WriteBytes(ServerName);
		}
	}

	// C->S C->S 请求（第一拨）创建支付订单 msgId:2925
	public class MsgData_cGetRechargeorder_DYB : MsgData
	{
		public int ItemID;	// 购买物品ID
		public int ItemNum;	// 购买数量
		public long ServerID;	// 服务器ID
		public sbyte PidSize;	// pid长度
		public sbyte ServerNameSize;	// 服务器名长度
		public byte[] Pid = new byte[128];	// pid
		public byte[] ServerName = new byte[128];	// 服务器名

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(ItemID);
			buffer.WriteInt32(ItemNum);
			buffer.WriteInt64(ServerID);
			buffer.WriteInt8(PidSize);
			buffer.WriteInt8(ServerNameSize);
			buffer.WriteBytes(Pid);
			buffer.WriteBytes(ServerName);
		}
	}

	// C->S C->S 请求（第一拨）创建支付订单 msgId:2924
	public class MsgData_cGetRechargeorder_SQW : MsgData
	{
		public int ItemID;	// 购买物品ID
		public int ItemNum;	// 购买数量
		public long ServerID;	// 服务器ID
		public sbyte PidSize;	// pid长度
		public sbyte ServerNameSize;	// 服务器名长度
		public byte[] Pid = new byte[128];	// pid
		public byte[] ServerName = new byte[128];	// 服务器名

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(ItemID);
			buffer.WriteInt32(ItemNum);
			buffer.WriteInt64(ServerID);
			buffer.WriteInt8(PidSize);
			buffer.WriteInt8(ServerNameSize);
			buffer.WriteBytes(Pid);
			buffer.WriteBytes(ServerName);
		}
	}

	// S->C 返回:创建订单 msgId:7913
	public class MsgData_sGetRechargeorder : MsgData
	{
		public int ItemID;	// 购买物品ID
		public int dataSize;	// 数据长度
		public List<byte> data = new List<byte>();	// pid

		public override void unpack(NetReadBuffer buffer)
		{
			ItemID = buffer.ReadInt32();
			dataSize = buffer.ReadInt32();
			buffer.ReadBytes(data, (int)dataSize);
		}
	}

	// S->C S->C 返回:创建订单 msgId:7930
	public class MsgData_sGetRechargeorder_DYB : MsgData
	{
		public int ItemID;	// 购买物品ID
		public int dataSize;	// 数据长度
		public List<byte> data = new List<byte>();	// pid

		public override void unpack(NetReadBuffer buffer)
		{
			ItemID = buffer.ReadInt32();
			dataSize = buffer.ReadInt32();
			buffer.ReadBytes(data, (int)dataSize);
		}
	}

	// S->C S->C 返回:创建订单 msgId:7931
	public class MsgData_sGetRechargeorder_SQW : MsgData
	{
		public int ItemID;	// 购买物品ID
		public int dataSize;	// 数据长度
		public List<byte> data = new List<byte>();	// pid

		public override void unpack(NetReadBuffer buffer)
		{
			ItemID = buffer.ReadInt32();
			dataSize = buffer.ReadInt32();
			buffer.ReadBytes(data, (int)dataSize);
		}
	}

	// S->C S->C 登录返回 msgId:6001(LC_CONN_RESP)
	public class MsgData_sLogin : MsgData
	{
		public int ResultCode;	// 返回：0:OK -1:Create Role -2:时间戳错误 -3:签名不匹配 -4:封停 -5:协议不一致 -6:MAC封禁
		public int TxCode;	// 如果是TX接口失败,返回TX的错误码
		public byte[] Account = new byte[64];	// 玩家帐号
		public long GUID;	// guid
		public int ForbbidenTime;	// 封禁时间
		public long ServerTime;	// 服务器时间
		public byte[] Exts = new byte[64];	// 扩展信息
		public uint ServerID;	// 区服ID
		public long AccountGUID;	// 帐号guid
		public sbyte RoleCount;	// 角色个数

		public override void unpack(NetReadBuffer buffer)
		{
			ResultCode = buffer.ReadInt32();
			TxCode = buffer.ReadInt32();
			buffer.ReadBytes(Account);
			GUID = buffer.ReadInt64();
			ForbbidenTime = buffer.ReadInt32();
			ServerTime = buffer.ReadInt64();
			buffer.ReadBytes(Exts);
			ServerID = buffer.ReadUInt32();
			AccountGUID = buffer.ReadInt64();
			RoleCount = buffer.ReadInt8();
		}
	}

	// S->C 登录时返回的角色概要信息
	public class MsgData_sLoginRole : MsgData
	{
		public long ID;	// 角色ID
		public byte[] Name = new byte[32];	// 角色名字
		public int MapID;	// 地图ID
		public int Job;	// 角色职业
		public int Level;	// 角色等级
		public int FightPoint;	// 战斗力
		public int VipLevel;	// Vip等级
		public int Weapon;	// 武器
		public int MagicWeapon;	// 神兵
		public int Dress;	// 衣服
		public int FashionHead;	// 时装头
		public int FashionWeapon;	// 时装武器
		public int FashionDress;	// 时装衣服
		public int Wing;	// 翅膀
		public long LastLoginTime;	// 最近登录时间
		public int ForbbidenTime;	// 封禁结束时间
		public int EquipStarMin;	// 装备升星最小星级
		public long CreateTime;	// 创建角色登录时间

		public override void unpack(NetReadBuffer buffer)
		{
			ID = buffer.ReadInt64();
			buffer.ReadBytes(Name);
			MapID = buffer.ReadInt32();
			Job = buffer.ReadInt32();
			Level = buffer.ReadInt32();
			FightPoint = buffer.ReadInt32();
			VipLevel = buffer.ReadInt32();
			Weapon = buffer.ReadInt32();
			MagicWeapon = buffer.ReadInt32();
			Dress = buffer.ReadInt32();
			FashionHead = buffer.ReadInt32();
			FashionWeapon = buffer.ReadInt32();
			FashionDress = buffer.ReadInt32();
			Wing = buffer.ReadInt32();
			LastLoginTime = buffer.ReadInt64();
			ForbbidenTime = buffer.ReadInt32();
			EquipStarMin = buffer.ReadInt32();
			CreateTime = buffer.ReadInt64();
		}
	}

	// S->C 返回登录角色信息 msgId:6003
	public class MsgData_sRoleInfo : MsgData
	{
		public sbyte SelectIndex;	// 选中的角色索引
		public sbyte RoleCount;	// 角色个数
		public List<MsgData_sLoginRole> Roles = new List<MsgData_sLoginRole>();	// 角色数组

		public override void unpack(NetReadBuffer buffer)
		{
			SelectIndex = buffer.ReadInt8();
			RoleCount = buffer.ReadInt8();
			Roles = new List<MsgData_sLoginRole>();
			for (int i = 0; i < RoleCount; i++)
			{
				MsgData_sLoginRole __item = new MsgData_sLoginRole();
				__item.unpack(buffer);
				Roles.Add(__item);
			}
		}
	}

	// C->S c->s 创建角色(CL_CREATE_ROLE_REQ msgId:1002)
	public class MsgData_cCreateRole : MsgData
	{
		public byte[] RoleName = new byte[32];	// 角色名字
		public int Job;	// 角色职业
		public int Icon;	// 角色ICON
		public byte[] Channel = new byte[32];	// 渠道
		public byte[] Exts = new byte[64];	// 扩展信息
		public long AccountGUID;	// 帐号guid
		public long CurrentRoleID;	// 当前选中的角色guid，没有则填0

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteBytes(RoleName);
			buffer.WriteInt32(Job);
			buffer.WriteInt32(Icon);
			buffer.WriteBytes(Channel);
			buffer.WriteBytes(Exts);
			buffer.WriteInt64(AccountGUID);
			buffer.WriteInt64(CurrentRoleID);
		}
	}

	// S->C s->c 创建角色(LC_CREATE_ROLE_RESP msgId:6002)
	public class MsgData_sCreateRole : MsgData
	{
		public int Result;	// 结果,0成功,-1名字冲突,-2名字不合法 -3其他Error
		public int Job;	// 角色职业
		public long ID;	// 角色ID
		public byte[] Name = new byte[32];	// 角色名
		public long Externsion;	// 扩展ID，客户端无用
		public long createtime;	// 创建角色时间

		public override void unpack(NetReadBuffer buffer)
		{
			Result = buffer.ReadInt32();
			Job = buffer.ReadInt32();
			ID = buffer.ReadInt64();
			buffer.ReadBytes(Name);
			Externsion = buffer.ReadInt64();
			createtime = buffer.ReadInt64();
		}
	}

	// C->S c->s 请求进入游戏(CW_ENTER_GAME msgId:2001)
	public class MsgData_cEnterGame : MsgData
	{
		public byte[] Account = new byte[64];	// 玩家ID
		public byte[] IP = new byte[32];	// Client IP
		public byte[] MAC = new byte[32];	// 物理地址
		public byte[] OpenKey = new byte[64];	// Open Key
		public byte[] Channel = new byte[64];	// 渠道
		public byte[] Exts = new byte[64];	// 扩展信息
		public int ServerID;	// 服务器ID
		public int LoginType;	// 登录类型:0web,1微端
		public int ActivityID;	// 活动ID
		public int PID;	// pid
		public long SelectedRole;	// 选中的角色guid
		public long AccountGUID;	// 角色帐号guid

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteBytes(Account);
			buffer.WriteBytes(IP);
			buffer.WriteBytes(MAC);
			buffer.WriteBytes(OpenKey);
			buffer.WriteBytes(Channel);
			buffer.WriteBytes(Exts);
			buffer.WriteInt32(ServerID);
			buffer.WriteInt32(LoginType);
			buffer.WriteInt32(ActivityID);
			buffer.WriteInt32(PID);
			buffer.WriteInt64(SelectedRole);
			buffer.WriteInt64(AccountGUID);
		}
	}

	// S->C s->c 进入游戏返回(WC_ENTER_GAME msgId:7001)
	public class MsgData_sEnterGame : MsgData
	{
		public int ResultCode;	// 失败原因 1:服务器未启动 2:玩家已在线

		public override void unpack(NetReadBuffer buffer)
		{
			ResultCode = buffer.ReadInt32();
		}
	}

	// S->C s->c 主角接收自己的信息(SC_SCENE_SHOW_ME_INFO msgId:8002)
	public class MsgData_sMeInfo : MsgData
	{
		public long RoleID;	// 角色ID
		public byte[] RoleName = new byte[32];	// 角色名字
		public sbyte Gender;	// 性别
		public int Job;	// 职业
		public int Icon;	// 头像
		public int Dress;	// 衣服
		public int Weapon;	// 武器
		public int FashionWing;	// 时装翅膀(原时装头)
		public int FashionWeapon;	// 时装武器
		public int FashionDress;	// 时装衣服
		public int WuHun;	// 武魂
		public int FaBao;	// 法宝
		public int ShenBing;	// 神兵
		public sbyte ZhenYing;	// 阵营
		public int JingJie;	// 境界0无
		public int Pets;	// 萌宠
		public int Wing;	// 翅膀
		public int PiFengLevel;	// 披风使用等级
		public int SuitFlag;	// 套装标识
		public int Footprints;	// 脚印
		public int LoryLevel;	// 修为等级
		public sbyte MilitaryRank;	// 军衔ID
		public int FabaoQuality;	// 法宝品质
		public int GuanZhi;	// 官职
		public int BaoJiaLevel;	// 宝甲等级
		public int TianGangLevel;	// 天罡等级
		public int ZhanNuLevel;	// 战弩等级
		public int MingLunLevel;	// 命轮等级
		public int HunQiLevel;	// 魂器等级
		public int ShengQiLevel;	// 圣器等级
		public int FaBaoAwake;	// 法宝觉醒
		public int TransferID;	// 转职ID
		public int ZhenFaID;	// 阵法ID
		public sbyte ShenWuID;	// 神武ID
		public int FeiShengLevel;	// 飞升等级
		public int QiLinBiLevel;	// 麒麟臂等级
		public int JianYuLevel;	// 剑域等级
		public int FashionState;	// 是否隐藏时装 0显示 1隐藏
		public int EquipStarMin;	// 装备升星最小星级

		public override void unpack(NetReadBuffer buffer)
		{
			RoleID = buffer.ReadInt64();
			buffer.ReadBytes(RoleName);
			Gender = buffer.ReadInt8();
			Job = buffer.ReadInt32();
			Icon = buffer.ReadInt32();
			Dress = buffer.ReadInt32();
			Weapon = buffer.ReadInt32();
			FashionWing = buffer.ReadInt32();
			FashionWeapon = buffer.ReadInt32();
			FashionDress = buffer.ReadInt32();
			WuHun = buffer.ReadInt32();
			FaBao = buffer.ReadInt32();
			ShenBing = buffer.ReadInt32();
			ZhenYing = buffer.ReadInt8();
			JingJie = buffer.ReadInt32();
			Pets = buffer.ReadInt32();
			Wing = buffer.ReadInt32();
			PiFengLevel = buffer.ReadInt32();
			SuitFlag = buffer.ReadInt32();
			Footprints = buffer.ReadInt32();
			LoryLevel = buffer.ReadInt32();
			MilitaryRank = buffer.ReadInt8();
			FabaoQuality = buffer.ReadInt32();
			GuanZhi = buffer.ReadInt32();
			BaoJiaLevel = buffer.ReadInt32();
			TianGangLevel = buffer.ReadInt32();
			ZhanNuLevel = buffer.ReadInt32();
			MingLunLevel = buffer.ReadInt32();
			HunQiLevel = buffer.ReadInt32();
			ShengQiLevel = buffer.ReadInt32();
			FaBaoAwake = buffer.ReadInt32();
			TransferID = buffer.ReadInt32();
			ZhenFaID = buffer.ReadInt32();
			ShenWuID = buffer.ReadInt8();
			FeiShengLevel = buffer.ReadInt32();
			QiLinBiLevel = buffer.ReadInt32();
			JianYuLevel = buffer.ReadInt32();
			FashionState = buffer.ReadInt32();
			EquipStarMin = buffer.ReadInt32();
		}
	}

	// S->C s->c 通知玩家进入场景(SC_SCENE_ENTER_GAME msgId:8001)
	public class MsgData_sEnterScene : MsgData
	{
		public int Result;	// 结果
		public int LineID;	// 线
		public double PosX;	// X坐标
		public double PosY;	// Y坐标
		public double Dir;	// 方向
		public int MapID;	// 地图ID
		public int FubenID;	// 地图ID
		public sbyte EnterType;	// 0:登录游戏 1:切换场景
		public long ServerTime;	// 开服时间,时间戳,秒
		public long MegerServerTime;	// 合服时间,时间戳,秒

		public override void unpack(NetReadBuffer buffer)
		{
			Result = buffer.ReadInt32();
			LineID = buffer.ReadInt32();
			PosX = buffer.ReadDouble();
			PosY = buffer.ReadDouble();
			Dir = buffer.ReadDouble();
			MapID = buffer.ReadInt32();
			FubenID = buffer.ReadInt32();
			EnterType = buffer.ReadInt8();
			ServerTime = buffer.ReadInt64();
			MegerServerTime = buffer.ReadInt64();
		}
	}

	// C->S c->s 主角进入场景(CS_SCENE_ENTER_SCENE msgId:3003)
	public class MsgData_cMainPlayerEnterScene : MsgData
	{
		public int InitGame;	// 默认0，取值1为切换地图状态

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(InitGame);
		}
	}

	// S->C s->c 主角进入场景(SC_SCENE_ENTER_SCENE_RET msgId:8003)
	public class MsgData_sMainPlayerEnterScene : MsgData
	{

		public override void unpack(NetReadBuffer buffer)
		{
		}
	}

	// C->S 登出协议(msgId:1903)
	public class MsgData_cLogout : MsgData
	{
		public long AccountGUID;	// 账号GUID

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(AccountGUID);
		}
	}

	// C->S 离开游戏协议(msgId:2906)
	public class MsgData_cLeaveGame : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// S->C 服务器返回离开游戏(msgId:7906)
	public class MsgData_sLeaveGame : MsgData
	{

		public override void unpack(NetReadBuffer buffer)
		{
		}
	}

	// C->S 重连协议(msgId:1902)
	public class MsgData_cReconnect : MsgData
	{
		public byte[] Account = new byte[64];	// 账号GUID
		public byte[] Cookie = new byte[64];	// cookie
		public sbyte Status;	// 0 表示 在选角色状态， 1 表示在游戏场景中 ,2表示在跨服场景中
		public int ServerID;	// server id

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteBytes(Account);
			buffer.WriteBytes(Cookie);
			buffer.WriteInt8(Status);
			buffer.WriteInt32(ServerID);
		}
	}

	// S->C 返回重连协议(msgId:6006)
	public class MsgData_sReconnect : MsgData
	{
		public byte Result;	// 成功返回0 ，cookie认证失败返回1, 连接失败返回2

		public override void unpack(NetReadBuffer buffer)
		{
			Result = buffer.ReadUInt8();
		}
	}

	// S->C Cookie更新Cookie协议(msgId:6005)
	public class MsgData_sCookieUpdate : MsgData
	{
		public uint ExpiredTime;	// linux timtstamp
		public byte[] Cookie = new byte[64];	// cookie

		public override void unpack(NetReadBuffer buffer)
		{
			ExpiredTime = buffer.ReadUInt32();
			buffer.ReadBytes(Cookie);
		}
	}

	// C->S c->s 主角请求移动(CS_SCENE_MOVE_TO msgId:3004)
	public class MsgData_cMoveTo : MsgData
	{
		public int SrcX;	// 源X
		public int SrcY;	// 源Y
		public int DestX;	// 目的X
		public int DestY;	// 目的Y

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(SrcX);
			buffer.WriteInt32(SrcY);
			buffer.WriteInt32(DestX);
			buffer.WriteInt32(DestY);
		}
	}

	// S->C s->c 收到其他玩家移动通知(SC_SCENE_OBJ_MOVE_TO_NOTIFY msgId:8007)
	public class MsgData_sOtherMoveTo : MsgData
	{
		public long RoleID;	// 角色ID
		public sbyte ObjType;	// 类型
		public int SrcX;	// 源X
		public int SrcY;	// 源Y
		public int DestX;	// 目的X
		public int DestY;	// 目的Y

		public override void unpack(NetReadBuffer buffer)
		{
			RoleID = buffer.ReadInt64();
			ObjType = buffer.ReadInt8();
			SrcX = buffer.ReadInt32();
			SrcY = buffer.ReadInt32();
			DestX = buffer.ReadInt32();
			DestY = buffer.ReadInt32();
		}
	}

	// C->S c->s 客户端主动去请求转向(CS_REQ_CHANGE_DIR msgId:3036)
	public class MsgData_cChangeDir : MsgData
	{
		public double Dir;	// 朝向

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteDouble(Dir);
		}
	}

	// S->C s->c 广播给玩家，有人修改朝向(SC_CHAR_CHANGE_DIR msgId:8036)
	public class MsgData_sOtherChangeDir : MsgData
	{
		public long GUID;	// 角色ID
		public sbyte ObjType;	// 类型
		public double Dir;	// 朝向

		public override void unpack(NetReadBuffer buffer)
		{
			GUID = buffer.ReadInt64();
			ObjType = buffer.ReadInt8();
			Dir = buffer.ReadDouble();
		}
	}

	// C->S c->s 主角请求停止移动(CS_SCENE_MOVE_STOP msgId:3006)
	public class MsgData_cMoveStop : MsgData
	{
		public int StopX;	// 停止位置X
		public int StopY;	// 停止位置Y
		public int Dir;	// 朝向

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(StopX);
			buffer.WriteInt32(StopY);
			buffer.WriteInt32(Dir);
		}
	}

	// S->C s->c 广播收到其他玩家/怪物停止移动(SC_SCENE_OBJ_MOVE_STOP_NOTIFY msgId:8008)
	public class MsgData_sOtherMoveStop : MsgData
	{
		public long RoleID;	// 角色ID
		public sbyte ObjType;	// 类型
		public int StopX;	// 停止位置X
		public int StopY;	// 停止位置Y
		public int Dir;	// 朝向

		public override void unpack(NetReadBuffer buffer)
		{
			RoleID = buffer.ReadInt64();
			ObjType = buffer.ReadInt8();
			StopX = buffer.ReadInt32();
			StopY = buffer.ReadInt32();
			Dir = buffer.ReadInt32();
		}
	}

	// S->C s->c 收到其他怪物移动通知(SC_SCENE_MONSTER_MOVE_TO_NOTIFY msgId:8551)
	public class MsgData_sMonsterMoveTo : MsgData
	{
		public long RoleID;	// 角色ID
		public int DestX;	// 目的X
		public int DestY;	// 目的Y

		public override void unpack(NetReadBuffer buffer)
		{
			RoleID = buffer.ReadInt64();
			DestX = buffer.ReadInt32();
			DestY = buffer.ReadInt32();
		}
	}

	// C->S 客户端请求: 主角施放技能 msgId:3050
	public class MsgData_cCastMagic : MsgData
	{
		public int SkillID;	// 技能ID
		public long TargetID;	// 目标ID
		public double PosX;	// 如果是位置施法，坐标X
		public double PosY;	// 如果是位置施法，坐标Y
		public float SelfPosX;	// 自身坐标X
		public float SelfPosY;	// 自身坐标Y
		public float Dir;	// 自身方向
		public sbyte TargetCount;	// 多目标时的目标个数
		public List<long> TargetList = new List<long>();	// 目标id列表

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(SkillID);
			buffer.WriteInt64(TargetID);
			buffer.WriteDouble(PosX);
			buffer.WriteDouble(PosY);
			buffer.WriteFloat(SelfPosX);
			buffer.WriteFloat(SelfPosY);
			buffer.WriteFloat(Dir);
			buffer.WriteInt8(TargetCount);
			for (int i = 0; i < TargetCount; i++)
			{
				buffer.WriteInt64(TargetList[i]);
			}
		}
	}

	// S->C 服务端通知: 释放技能返回结果 msgId:8099
	public class MsgData_sCastMagic : MsgData
	{
		public int SkillID;	// 技能ID
		public int ResultCode;	// 释放技能返回结果 0:成功 -1:目标或坐标不对 -2:技能不存在 -3:CD -10:目标不存在 -11:消耗检查 -21 目标状态不对 -31/32/33:距离不对 -61施法者状态不对

		public override void unpack(NetReadBuffer buffer)
		{
			SkillID = buffer.ReadInt32();
			ResultCode = buffer.ReadInt32();
		}
	}

	// C->S 客户端请求: 主角打断施法 msgId:3056
	public class MsgData_cInterruptCast : MsgData
	{
		public int SkillID;	// 技能ID

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(SkillID);
		}
	}

	// S->C 服务端通知: 法术冷却信息 msgId:8058
	public class MsgData_sCooldown : MsgData
	{
		public long CasterID;	// 施法者ID
		public int SkillID;	// 技能ID
		public int CD;	// 冷却时间
		public int GroupID;	// 技能CD组
		public int GroupCD;	// 技能CD组时间

		public override void unpack(NetReadBuffer buffer)
		{
			CasterID = buffer.ReadInt64();
			SkillID = buffer.ReadInt32();
			CD = buffer.ReadInt32();
			GroupID = buffer.ReadInt32();
			GroupCD = buffer.ReadInt32();
		}
	}

	// S->C 服务端通知: 普通施法开始（全屏广播） msgId:8050
	public class MsgData_sCastBegan : MsgData
	{
		public long CasterID;	// 施法者ID
		public int SkillID;	// 技能ID
		public long TargetID;	// 目标ID
		public int PosX;	// 如果是位置施法，坐标X
		public int PosY;	// 如果是位置施法，坐标Y
		public int CasterPosX;	// 施法者坐标X
		public int CasterPosY;	// 施法者坐标Y

		public override void unpack(NetReadBuffer buffer)
		{
			CasterID = buffer.ReadInt64();
			SkillID = buffer.ReadInt32();
			TargetID = buffer.ReadInt64();
			PosX = buffer.ReadInt32();
			PosY = buffer.ReadInt32();
			CasterPosX = buffer.ReadInt32();
			CasterPosY = buffer.ReadInt32();
		}
	}

	// S->C 服务端通知: 普通施法结束（全屏广播） msgId:8051
	public class MsgData_sCastEnd : MsgData
	{
		public long CasterID;	// 施法者ID
		public int SkillID;	// 技能ID

		public override void unpack(NetReadBuffer buffer)
		{
			CasterID = buffer.ReadInt64();
			SkillID = buffer.ReadInt32();
		}
	}

	// S->C 服务端通知: 施法蓄力开始（全屏广播） msgId:8052
	public class MsgData_sCastPrepBegan : MsgData
	{
		public long CasterID;	// 施法者ID
		public int SkillID;	// 技能ID
		public int PrepTime;	// 蓄力持续时间（毫秒）
		public long TargetID;	// 目标ID
		public double PosX;	// 如果是位置施法，坐标X
		public double PosY;	// 如果是位置施法，坐标Y

		public override void unpack(NetReadBuffer buffer)
		{
			CasterID = buffer.ReadInt64();
			SkillID = buffer.ReadInt32();
			PrepTime = buffer.ReadInt32();
			TargetID = buffer.ReadInt64();
			PosX = buffer.ReadDouble();
			PosY = buffer.ReadDouble();
		}
	}

	// S->C 服务端通知: 蓄力施法结束（全屏广播） msgId:8053
	public class MsgData_sCastPrepEnd : MsgData
	{
		public long CasterID;	// 施法者ID
		public int SkillID;	// 技能ID
		public sbyte IsEnd;	// 0:中断 1:蓄力满

		public override void unpack(NetReadBuffer buffer)
		{
			CasterID = buffer.ReadInt64();
			SkillID = buffer.ReadInt32();
			IsEnd = buffer.ReadInt8();
		}
	}

	// S->C 服务端通知: 施法引导结束（全屏广播） msgId:8054
	public class MsgData_sCastChanBegan : MsgData
	{
		public long CasterID;	// 施法者ID
		public int SkillID;	// 技能ID
		public long TargetID;	// 目标ID
		public double PosX;	// 如果是位置施法，坐标X
		public double PosY;	// 如果是位置施法，坐标Y

		public override void unpack(NetReadBuffer buffer)
		{
			CasterID = buffer.ReadInt64();
			SkillID = buffer.ReadInt32();
			TargetID = buffer.ReadInt64();
			PosX = buffer.ReadDouble();
			PosY = buffer.ReadDouble();
		}
	}

	// S->C 服务端通知: 普通施法结束（全屏广播） msgId:8055
	public class MsgData_sCastChanEnd : MsgData
	{
		public long CasterID;	// 施法者ID
		public int SkillID;	// 技能ID

		public override void unpack(NetReadBuffer buffer)
		{
			CasterID = buffer.ReadInt64();
			SkillID = buffer.ReadInt32();
		}
	}

	// S->C 服务端通知: 施法中断通知（全屏广播） msgId:8056;
	public class MsgData_sInterruptCast : MsgData
	{
		public long CasterID;	// 施法者ID
		public int SkillID;	// 技能ID

		public override void unpack(NetReadBuffer buffer)
		{
			CasterID = buffer.ReadInt64();
			SkillID = buffer.ReadInt32();
		}
	}

	// S->C 服务端通知: 施法效果结算（全屏广播） msgId:8057
	public class MsgData_sCastEffect : MsgData
	{
		public long CasterID;	// 施法者ID
		public int SkillID;	// 技能ID
		public int EffectID;	// 效果ID
		public long TargetID;	// 目标ID
		public int Flags;	// 目标标记
		public int DamagaType;	// 伤害类型
		public double Damage;	// 目标伤害值

		public override void unpack(NetReadBuffer buffer)
		{
			CasterID = buffer.ReadInt64();
			SkillID = buffer.ReadInt32();
			EffectID = buffer.ReadInt32();
			TargetID = buffer.ReadInt64();
			Flags = buffer.ReadInt32();
			DamagaType = buffer.ReadInt32();
			Damage = buffer.ReadDouble();
		}
	}

	// S->C 服务端通知:击退 msgId:8081
	public class MsgData_sKnockBack : MsgData
	{
		public long CasterID;	// 施法者ID
		public long TargetID;	// 目标ID
		public int MotionSpeed;	// 速度
		public int MotionTime;	// 动作持续时间
		public double PosX;	// 如果是位置施法，坐标X
		public double PosY;	// 如果是位置施法，坐标Y

		public override void unpack(NetReadBuffer buffer)
		{
			CasterID = buffer.ReadInt64();
			TargetID = buffer.ReadInt64();
			MotionSpeed = buffer.ReadInt32();
			MotionTime = buffer.ReadInt32();
			PosX = buffer.ReadDouble();
			PosY = buffer.ReadDouble();
		}
	}

	// S->C 服务端通知:增加BUFF msgId:8082
	public class MsgData_sAddBuffer : MsgData
	{
		public long TargetID;	// 目标ID
		public long BufferInstanceID;	// Buffer实例ID
		public int BufferTemplateID;	// Buffer配置ID
		public int Life;	// Buffer持续时间
		public int Param;	// Buffer附带参数
		public long CasterID;	// 施法者ID

		public override void unpack(NetReadBuffer buffer)
		{
			TargetID = buffer.ReadInt64();
			BufferInstanceID = buffer.ReadInt64();
			BufferTemplateID = buffer.ReadInt32();
			Life = buffer.ReadInt32();
			Param = buffer.ReadInt32();
			CasterID = buffer.ReadInt64();
		}
	}

	// S->C 服务端通知:更新BUFF msgId:8083
	public class MsgData_sUpdateBuffer : MsgData
	{
		public long BufferInstanceID;	// Buffer实例ID
		public int Life;	// Buffer持续时间
		public int Count;	// Buffer叠加层数
		public int[] Param = new int[3];	// Buffer参数

		public override void unpack(NetReadBuffer buffer)
		{
			BufferInstanceID = buffer.ReadInt64();
			Life = buffer.ReadInt32();
			Count = buffer.ReadInt32();
			for (int i = 0; i < 3; i++)
			{
				int __item = buffer.ReadInt32();
				Param[i]=__item;
			}
		}
	}

	// S->C 服务端通知:删除BUFF msgId:8084
	public class MsgData_sDelBuffer : MsgData
	{
		public long BufferInstanceID;	// Buffer实例ID
		public long TargetID;	// 目标ID
		public int BufferTemplateID;	// Buffer配置ID

		public override void unpack(NetReadBuffer buffer)
		{
			BufferInstanceID = buffer.ReadInt64();
			TargetID = buffer.ReadInt64();
			BufferTemplateID = buffer.ReadInt32();
		}
	}

	// S->C 服务端通知:状态位改变 msgId:8085
	public class MsgData_sStateChanged : MsgData
	{
		public long RoleID;	// 角色id
		public sbyte State;	// 状态位
		public sbyte IsSet;	// 0表示False,1表示是set状态

		public override void unpack(NetReadBuffer buffer)
		{
			RoleID = buffer.ReadInt64();
			State = buffer.ReadInt8();
			IsSet = buffer.ReadInt8();
		}
	}

	// S->C 服务端通知:场景中玩家形象改变 msgId:8027
	public class MsgData_sPlayerShowChanged : MsgData
	{
		public long RoleID;	// 角色id
		public int Type;	// 类型:1Face,2Hair 参照EModelChange
		public int Value;	// 值

		public override void unpack(NetReadBuffer buffer)
		{
			RoleID = buffer.ReadInt64();
			Type = buffer.ReadInt32();
			Value = buffer.ReadInt32();
		}
	}

	// S->C 服务端通知: 连击持续时间开始（全屏广播） msgId:8086
	public class MsgData_sComboBegan : MsgData
	{
		public long CasterID;	// 施法者ID
		public int SkillID;	// 技能ID
		public int ComboTime;	// 连击持续时间
		public long TargetID;	// 目标ID

		public override void unpack(NetReadBuffer buffer)
		{
			CasterID = buffer.ReadInt64();
			SkillID = buffer.ReadInt32();
			ComboTime = buffer.ReadInt32();
			TargetID = buffer.ReadInt64();
		}
	}

	// S->C 服务端通知: 连击持续时间结束（全屏广播） msgId:8087
	public class MsgData_sComboEnded : MsgData
	{
		public long CasterID;	// 施法者ID
		public int SkillID;	// 技能ID

		public override void unpack(NetReadBuffer buffer)
		{
			CasterID = buffer.ReadInt64();
			SkillID = buffer.ReadInt32();
		}
	}

	// S->C 地图Buffer
	public class MsgData_sBuffer : MsgData
	{
		public long BufferInstanceID;	// Buffer实例ID
		public int BufferTemplateID;	// Buffer模板ID
		public int Life;	// Buffer持续时间

		public override void unpack(NetReadBuffer buffer)
		{
			BufferInstanceID = buffer.ReadInt64();
			BufferTemplateID = buffer.ReadInt32();
			Life = buffer.ReadInt32();
		}
	}

	// S->C 服务端通知:增加BUFF列表 msgId:8364
	public class MsgData_sAddBufferList : MsgData
	{
		public long TargetID;	// 目标ID
		public uint BufferCount;	// Buffer个数
		public List<MsgData_sBuffer> BufferList = new List<MsgData_sBuffer>();	// Buffer列表

		public override void unpack(NetReadBuffer buffer)
		{
			TargetID = buffer.ReadInt64();
			BufferCount = buffer.ReadUInt32();
			BufferList = new List<MsgData_sBuffer>();
			for (int i = 0; i < BufferCount; i++)
			{
				MsgData_sBuffer __item = new MsgData_sBuffer();
				__item.unpack(buffer);
				BufferList.Add(__item);
			}
		}
	}

	// S->C Object进入场景 msgId:8012
	public class MsgData_sSceneObjectEnterNotify : MsgData
	{
		public uint DataSize;	// 消息数据长度
		public List<byte> Data = new List<byte>();	// 消息数据

		public override void unpack(NetReadBuffer buffer)
		{
			DataSize = buffer.ReadUInt32();
			buffer.ReadBytes(Data, (int)DataSize);
		}
	}

	// S->C 服务器返回:场景中红颜等阶和星级,（包括自己和别人的红颜) msgId:8849
	public class MsgData_sSceneObjHoneYanLevel : MsgData
	{
		public long RoleID;	// 红颜唯一标识
		public int Level;	// 红颜等级(即星级)
		public int Qua;	// 红颜等级(提品品质)
		public byte[] RoleName = new byte[32];	// 拥有者名字

		public override void unpack(NetReadBuffer buffer)
		{
			RoleID = buffer.ReadInt64();
			Level = buffer.ReadInt32();
			Qua = buffer.ReadInt32();
			buffer.ReadBytes(RoleName);
		}
	}

	// S->C Object进入场景数据头
	public class MsgData_sSceneObjectEnterHead : MsgData
	{
		public uint DataSize;	// 消息数据长度
		public long Guid;	// 唯一标识
		public byte ObjType;	// 对象类型
		public int PosX;	// X坐标
		public int PosY;	// Y坐标(unity为Z坐标)
		public int Dir;	// 朝向

		public override void unpack(NetReadBuffer buffer)
		{
			DataSize = buffer.ReadUInt32();
			Guid = buffer.ReadInt64();
			ObjType = buffer.ReadUInt8();
			PosX = buffer.ReadInt32();
			PosY = buffer.ReadInt32();
			Dir = buffer.ReadInt32();
		}
	}

	// S->C 玩家进入场景数据
	public class MsgData_sSceneObjectEnterHuman : MsgData
	{
		public uint DataSize;	// 消息数据长度
		public long Guid;	// 唯一标识
		public byte ObjType;	// 对象类型
		public int PosX;	// X坐标
		public int PosY;	// Y坐标(unity为Z坐标)
		public int Dir;	// 朝向
		public int Speed;	// 速度
		public byte[] RoleName = new byte[32];	// 角色名字
		public int Prof;	// 职业
		public byte Sex;	// 性别
		public int Dress;	// 衣服
		public int Weapon;	// 武器
		public int FashionWing;	// 时装翅膀
		public int FashionDress;	// 时装衣服
		public int FashionWeapon;	// 时装武器
		public int Level;	// 等级
		public long CurHp;	// 当前血量
		public long MaxHp;	// 最大血量
		public int WuHunId;	// 武魂编号
		public long TeamID;	// 队伍编号
		public long GuildID;	// 公会编号
		public byte[] GuildName = new byte[32];	// 公会名字
		public int GuildPos;	// 公会职位
		public int Ride;	// 坐骑
		public int ZazenID;	// 
		public byte ZazenIdx;	// 
		public int[] Title = new int[3];	// 
		public byte Icon;	// 头像
		public byte VipLv;	// VIP等级
		public byte PKStatus;	// PK状态
		public int Bits;	// 状态位
		public int ShenBin;	// 神兵
		public byte Faction;	// 
		public int LingZhi;	// 
		public int Realm;	// 
		public int Vplan;	// 
		public int ActPet;	// 
		public int Wing;	// 翅膀
		public int GroupSuitFlag;	// 
		public int Footprints;	// 
		public long Power;	// 战斗力
		public int MagicKey;	// 法宝
		public int GlorLEvel;	// 
		public int MagicKeyQuality;	// 
		public int PiFeng;	// 披风
		public int GuanZhi;	// 
		public int Treasure;	// 
		public int GroupId;	// 
		public byte MilitaryLv;	// 
		public int TxHuang;	// 
		public int TxBlue;	// 
		public int TianGangLevel;	// 
		public int ZhanNuLevel;	// 
		public byte[] MateName = new byte[32];	// 
		public int HunQiLevel;	// 
		public int MingLun;	// 
		public int ShengQi;	// 
		public int MagickeyAwake;	// 
		public int ZhuanZhi;	// 
		public int ZhenfaId;	// 
		public byte ShenWu;	// 
		public int FeiShengId;	// 
		public int JianYu;	// 
		public long ChangeID;	// 
		public int FashionState;	// 
		public int EquipStarMin;	// 
		public int MagicKeyStar;	// 
		public long MoShenID;	// 

		public override void unpack(NetReadBuffer buffer)
		{
			DataSize = buffer.ReadUInt32();
			Guid = buffer.ReadInt64();
			ObjType = buffer.ReadUInt8();
			PosX = buffer.ReadInt32();
			PosY = buffer.ReadInt32();
			Dir = buffer.ReadInt32();
			Speed = buffer.ReadInt32();
			for (int i = 0; i < 32; i++)
			{
				byte __item = buffer.ReadUInt8();
				RoleName[i]=__item;
			}
			Prof = buffer.ReadInt32();
			Sex = buffer.ReadUInt8();
			Dress = buffer.ReadInt32();
			Weapon = buffer.ReadInt32();
			FashionWing = buffer.ReadInt32();
			FashionDress = buffer.ReadInt32();
			FashionWeapon = buffer.ReadInt32();
			Level = buffer.ReadInt32();
			CurHp = buffer.ReadInt64();
			MaxHp = buffer.ReadInt64();
			WuHunId = buffer.ReadInt32();
			TeamID = buffer.ReadInt64();
			GuildID = buffer.ReadInt64();
			for (int i = 0; i < 32; i++)
			{
				byte __item = buffer.ReadUInt8();
				GuildName[i]=__item;
			}
			GuildPos = buffer.ReadInt32();
			Ride = buffer.ReadInt32();
			ZazenID = buffer.ReadInt32();
			ZazenIdx = buffer.ReadUInt8();
			for (int i = 0; i < 3; i++)
			{
				int __item = buffer.ReadInt32();
				Title[i]=__item;
			}
			Icon = buffer.ReadUInt8();
			VipLv = buffer.ReadUInt8();
			PKStatus = buffer.ReadUInt8();
			Bits = buffer.ReadInt32();
			ShenBin = buffer.ReadInt32();
			Faction = buffer.ReadUInt8();
			LingZhi = buffer.ReadInt32();
			Realm = buffer.ReadInt32();
			Vplan = buffer.ReadInt32();
			ActPet = buffer.ReadInt32();
			Wing = buffer.ReadInt32();
			GroupSuitFlag = buffer.ReadInt32();
			Footprints = buffer.ReadInt32();
			Power = buffer.ReadInt64();
			MagicKey = buffer.ReadInt32();
			GlorLEvel = buffer.ReadInt32();
			MagicKeyQuality = buffer.ReadInt32();
			PiFeng = buffer.ReadInt32();
			GuanZhi = buffer.ReadInt32();
			Treasure = buffer.ReadInt32();
			GroupId = buffer.ReadInt32();
			MilitaryLv = buffer.ReadUInt8();
			TxHuang = buffer.ReadInt32();
			TxBlue = buffer.ReadInt32();
			TianGangLevel = buffer.ReadInt32();
			ZhanNuLevel = buffer.ReadInt32();
			for (int i = 0; i < 32; i++)
			{
				byte __item = buffer.ReadUInt8();
				MateName[i]=__item;
			}
			HunQiLevel = buffer.ReadInt32();
			MingLun = buffer.ReadInt32();
			ShengQi = buffer.ReadInt32();
			MagickeyAwake = buffer.ReadInt32();
			ZhuanZhi = buffer.ReadInt32();
			ZhenfaId = buffer.ReadInt32();
			ShenWu = buffer.ReadUInt8();
			FeiShengId = buffer.ReadInt32();
			JianYu = buffer.ReadInt32();
			ChangeID = buffer.ReadInt64();
			FashionState = buffer.ReadInt32();
			EquipStarMin = buffer.ReadInt32();
			MagicKeyStar = buffer.ReadInt32();
			MoShenID = buffer.ReadInt64();
		}
	}

	// S->C 怪物进入场景数据
	public class MsgData_sSceneObjectEnterMonster : MsgData
	{
		public uint DataSize;	// 消息数据长度
		public long Guid;	// 唯一标识
		public byte ObjType;	// 对象类型
		public int PosX;	// X坐标
		public int PosY;	// Y坐标(unity为Z坐标)
		public int Dir;	// 朝向
		public int Speed;	// 速度
		public int ConfigID;	// 配置数据ID
		public double CurHp;	// 当前血量
		public double MaxHp;	// 最大血量
		public int Bits;	// 状态位
		public byte Flags;	// 标志
		public byte Faction;	// 派系
		public int BelongType;	// 所属类型
		public long BelongID;	// 所属对象ID

		public override void unpack(NetReadBuffer buffer)
		{
			DataSize = buffer.ReadUInt32();
			Guid = buffer.ReadInt64();
			ObjType = buffer.ReadUInt8();
			PosX = buffer.ReadInt32();
			PosY = buffer.ReadInt32();
			Dir = buffer.ReadInt32();
			Speed = buffer.ReadInt32();
			ConfigID = buffer.ReadInt32();
			CurHp = buffer.ReadDouble();
			MaxHp = buffer.ReadDouble();
			Bits = buffer.ReadInt32();
			Flags = buffer.ReadUInt8();
			Faction = buffer.ReadUInt8();
			BelongType = buffer.ReadInt32();
			BelongID = buffer.ReadInt64();
		}
	}

	// S->C NPC进入场景数据
	public class MsgData_sSceneObjectEnterNPC : MsgData
	{
		public uint DataSize;	// 消息数据长度
		public long Guid;	// 唯一标识
		public byte ObjType;	// 对象类型
		public int PosX;	// X坐标
		public int PosY;	// Y坐标(unity为Z坐标)
		public int Dir;	// 朝向
		public int ConfigID;	// 配置数据ID

		public override void unpack(NetReadBuffer buffer)
		{
			DataSize = buffer.ReadUInt32();
			Guid = buffer.ReadInt64();
			ObjType = buffer.ReadUInt8();
			PosX = buffer.ReadInt32();
			PosY = buffer.ReadInt32();
			Dir = buffer.ReadInt32();
			ConfigID = buffer.ReadInt32();
		}
	}

	// S->C 物品进入场景数据
	public class MsgData_sSceneObjectEnterItem : MsgData
	{
		public uint DataSize;	// 消息数据长度
		public long Guid;	// 唯一标识
		public byte ObjType;	// 对象类型
		public int PosX;	// X坐标
		public int PosY;	// Y坐标(unity为Z坐标)
		public int Dir;	// 朝向
		public int ConfigID;	// 配置数据ID
		public long Owner;	// 拥有者
		public int Count;	// 物品数量
		public long Source;	// 掉落来源
		public byte Flags;	// 标记

		public override void unpack(NetReadBuffer buffer)
		{
			DataSize = buffer.ReadUInt32();
			Guid = buffer.ReadInt64();
			ObjType = buffer.ReadUInt8();
			PosX = buffer.ReadInt32();
			PosY = buffer.ReadInt32();
			Dir = buffer.ReadInt32();
			ConfigID = buffer.ReadInt32();
			Owner = buffer.ReadInt64();
			Count = buffer.ReadInt32();
			Source = buffer.ReadInt64();
			Flags = buffer.ReadUInt8();
		}
	}

	// S->C 采集物进入场景数据
	public class MsgData_sSceneObjectEnterCollection : MsgData
	{
		public uint DataSize;	// 消息数据长度
		public long Guid;	// 唯一标识
		public byte ObjType;	// 对象类型
		public int PosX;	// X坐标
		public int PosY;	// Y坐标(unity为Z坐标)
		public int Dir;	// 朝向
		public int ConfigID;	// 配置数据ID
		public byte Flags;	// 标记

		public override void unpack(NetReadBuffer buffer)
		{
			DataSize = buffer.ReadUInt32();
			Guid = buffer.ReadInt64();
			ObjType = buffer.ReadUInt8();
			PosX = buffer.ReadInt32();
			PosY = buffer.ReadInt32();
			Dir = buffer.ReadInt32();
			ConfigID = buffer.ReadInt32();
			Flags = buffer.ReadUInt8();
		}
	}

	// S->C 陷阱进入场景数据
	public class MsgData_sSceneObjectEnterTrap : MsgData
	{
		public uint DataSize;	// 消息数据长度
		public long Guid;	// 唯一标识
		public byte ObjType;	// 对象类型
		public int PosX;	// X坐标
		public int PosY;	// Y坐标(unity为Z坐标)
		public int Dir;	// 朝向
		public int ConfigID;	// 配置数据ID
		public long Owner;	// 拥有者
		public byte Flags;	// 标记

		public override void unpack(NetReadBuffer buffer)
		{
			DataSize = buffer.ReadUInt32();
			Guid = buffer.ReadInt64();
			ObjType = buffer.ReadUInt8();
			PosX = buffer.ReadInt32();
			PosY = buffer.ReadInt32();
			Dir = buffer.ReadInt32();
			ConfigID = buffer.ReadInt32();
			Owner = buffer.ReadInt64();
			Flags = buffer.ReadUInt8();
		}
	}

	// S->C 宠物进入场景数据
	public class MsgData_sSceneObjectEnterPet : MsgData
	{
		public uint DataSize;	// 消息数据长度
		public long Guid;	// 唯一标识
		public byte ObjType;	// 对象类型
		public int PosX;	// X坐标
		public int PosY;	// Y坐标(unity为Z坐标)
		public int Dir;	// 朝向
		public int ConfigID;	// 配置数据ID
		public long Owner;	// 拥有者
		public int Speed;	// 速度
		public int Bits;	// 
		public byte Flags;	// 标记
		public byte Type;	// 0 灵兽(红颜) 1.神灵
		public int Level;	// 等级
		public int LevelStar;	// 星级
		public byte Job;	// 携带职业类型
		public int Qua;	// 品质
		public byte[] RoleName = new byte[32];	// 拥有者名字

		public override void unpack(NetReadBuffer buffer)
		{
			DataSize = buffer.ReadUInt32();
			Guid = buffer.ReadInt64();
			ObjType = buffer.ReadUInt8();
			PosX = buffer.ReadInt32();
			PosY = buffer.ReadInt32();
			Dir = buffer.ReadInt32();
			ConfigID = buffer.ReadInt32();
			Owner = buffer.ReadInt64();
			Speed = buffer.ReadInt32();
			Bits = buffer.ReadInt32();
			Flags = buffer.ReadUInt8();
			Type = buffer.ReadUInt8();
			Level = buffer.ReadInt32();
			LevelStar = buffer.ReadInt32();
			Job = buffer.ReadUInt8();
			Qua = buffer.ReadInt32();
			buffer.ReadBytes(RoleName);
		}
	}

	// S->C 镖车进入场景数据
	public class MsgData_sSceneObjectEnterBiaoChe : MsgData
	{
		public uint DataSize;	// 消息数据长度
		public long Guid;	// 唯一标识
		public byte ObjType;	// 对象类型
		public int PosX;	// X坐标
		public int PosY;	// Y坐标(unity为Z坐标)
		public int Dir;	// 朝向
		public int DartID;	// 镖车ID
		public int Camp;	// 所属阵营
		public int Level;	// 团队成员最大等级
		public int Speed;	// 速度
		public int HP;	// 血量
		public byte[] LeaderName = new byte[32];	// 队长名字

		public override void unpack(NetReadBuffer buffer)
		{
			DataSize = buffer.ReadUInt32();
			Guid = buffer.ReadInt64();
			ObjType = buffer.ReadUInt8();
			PosX = buffer.ReadInt32();
			PosY = buffer.ReadInt32();
			Dir = buffer.ReadInt32();
			DartID = buffer.ReadInt32();
			Camp = buffer.ReadInt32();
			Level = buffer.ReadInt32();
			Speed = buffer.ReadInt32();
			HP = buffer.ReadInt32();
			for (int i = 0; i < 32; i++)
			{
				byte __item = buffer.ReadUInt8();
				LeaderName[i]=__item;
			}
		}
	}

	// S->C 静态物品进入场景数据
	public class MsgData_sSceneObjectEnterStaticObj : MsgData
	{
		public uint DataSize;	// 消息数据长度
		public long Guid;	// 唯一标识
		public byte ObjType;	// 对象类型
		public int PosX;	// X坐标
		public int PosY;	// Y坐标(unity为Z坐标)
		public int Dir;	// 朝向
		public int ConfigID;	// 配置数据ID

		public override void unpack(NetReadBuffer buffer)
		{
			DataSize = buffer.ReadUInt32();
			Guid = buffer.ReadInt64();
			ObjType = buffer.ReadUInt8();
			PosX = buffer.ReadInt32();
			PosY = buffer.ReadInt32();
			Dir = buffer.ReadInt32();
			ConfigID = buffer.ReadInt32();
		}
	}

	// S->C Object离开场景 msgid:8013
	public class MsgData_sSceneObjectLeaveNotify : MsgData
	{
		public ulong ObjectID;	// OBJECT ID
		public sbyte ObjectType;	// OBJECT类型

		public override void unpack(NetReadBuffer buffer)
		{
			ObjectID = buffer.ReadUInt64();
			ObjectType = buffer.ReadInt8();
		}
	}

	// S->C Object消失 msgId:11193
	public class MsgData_sSceneObjectDISAPPEA : MsgData
	{
		public uint count;	// 
		public List<long> ObjectID = new List<long>();	// OBJECT ID

		public override void unpack(NetReadBuffer buffer)
		{
			count = buffer.ReadUInt32();
			ObjectID = new List<long>();
			for (int i = 0; i < count; i++)
			{
				long __item = buffer.ReadInt64();
				ObjectID.Add(__item);
			}
		}
	}

	// S->C 属性内容
	public class MsgData_sClientAttr : MsgData
	{
		public sbyte AttrType;	// 属性类型
		public double AttrValue;	// 属性值

		public override void unpack(NetReadBuffer buffer)
		{
			AttrType = buffer.ReadInt8();
			AttrValue = buffer.ReadDouble();
		}
	}

	// S->C 角色属性改变 msgid:8011
	public class MsgData_sRoleAttrInfoNotify : MsgData
	{
		public long RoleID;	// 角色ID
		public sbyte ObjectType;	// OBJECT类型
		public uint AttrCount;	// 属性个数
		public List<MsgData_sClientAttr> AttrList = new List<MsgData_sClientAttr>();	// 属性列表

		public override void unpack(NetReadBuffer buffer)
		{
			RoleID = buffer.ReadInt64();
			ObjectType = buffer.ReadInt8();
			AttrCount = buffer.ReadUInt32();
			AttrList = new List<MsgData_sClientAttr>();
			for (int i = 0; i < AttrCount; i++)
			{
				MsgData_sClientAttr __item = new MsgData_sClientAttr();
				__item.unpack(buffer);
				AttrList.Add(__item);
			}
		}
	}

	// S->C 服务端通知:返回模块战斗力变化 msgid:9936
	public class MsgData_sModleFightChange : MsgData
	{
		public int ModleType;	// 战斗力类型
		public long Value;	// 改变后的值

		public override void unpack(NetReadBuffer buffer)
		{
			ModleType = buffer.ReadInt32();
			Value = buffer.ReadInt64();
		}
	}

	// S->C 特色坐骑信息
	public class MsgData_sFeatureRide : MsgData
	{
		public int RideID;	// 坐骑编号
		public long Time;	// 坐骑限时

		public override void unpack(NetReadBuffer buffer)
		{
			RideID = buffer.ReadInt32();
			Time = buffer.ReadInt64();
		}
	}

	// S->C 初始化坐骑信息 msgid:8117
	public class MsgData_sRideInfo : MsgData
	{
		public int Stage;	// 坐骑阶位
		public int StarProgress;	// 星级进度
		public int PillNum;	// 属性丹数量
		public int PillNumPercent;	// 资质丹数量
		public int RideID;	// 当前坐骑ID
		public int RideState;	// 骑乘状态 0下马 1上马
		public int UpLevelNum;	// 疲劳值 PS:好奇怪的字段
		public uint FeatureRideCount;	// 特色坐骑数
		public List<MsgData_sFeatureRide> FeatureRideList = new List<MsgData_sFeatureRide>();	// 特色坐骑列表

		public override void unpack(NetReadBuffer buffer)
		{
			Stage = buffer.ReadInt32();
			StarProgress = buffer.ReadInt32();
			PillNum = buffer.ReadInt32();
			PillNumPercent = buffer.ReadInt32();
			RideID = buffer.ReadInt32();
			RideState = buffer.ReadInt32();
			UpLevelNum = buffer.ReadInt32();
			FeatureRideCount = buffer.ReadUInt32();
			FeatureRideList = new List<MsgData_sFeatureRide>();
			for (int i = 0; i < FeatureRideCount; i++)
			{
				MsgData_sFeatureRide __item = new MsgData_sFeatureRide();
				__item.unpack(buffer);
				FeatureRideList.Add(__item);
			}
		}
	}

	// C->S 客户端请求：更改乘骑状态 msgid:3122
	public class MsgData_cChangeRideState : MsgData
	{
		public int RideState;	// 乘骑状态

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(RideState);
		}
	}

	// S->C 回复更改乘骑状态 msgid:8122
	public class MsgData_sChangeRideState : MsgData
	{
		public int RideState;	// 乘骑状态

		public override void unpack(NetReadBuffer buffer)
		{
			RideState = buffer.ReadInt32();
		}
	}

	// C->S 客户端请求：使用属性丹 msgId:3120
	public class MsgData_cUseAttrDan : MsgData
	{
		public int Type;	// 属性丹类型 E_AttrDanType

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(Type);
		}
	}

	// S->C 服务器通知：返回使用属性丹 msgId:8120
	public class MsgData_sUseAttrDan : MsgData
	{
		public int Type;	// 属性丹类型 E_AttrDanType
		public int Result;	// 使用结果 0=成功 -1=未开启 -2达到上限 -3数量不足
		public int PillNumber;	// 属性丹数量

		public override void unpack(NetReadBuffer buffer)
		{
			Type = buffer.ReadInt32();
			Result = buffer.ReadInt32();
			PillNumber = buffer.ReadInt32();
		}
	}

	// C->S 客户端请求：坐骑进阶 msgId:3118
	public class MsgData_cRideLvlUp : MsgData
	{
		public int Type;	// 消耗类型 0进阶石 1灵力
		public int AutoBuy;	// 0 自动购买道具,1 不自动购买

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(Type);
			buffer.WriteInt32(AutoBuy);
		}
	}

	// S->C 服务器通知：返回坐骑进阶进度 msgId:8118
	public class MsgData_sRideLvlUpInfo : MsgData
	{
		public int Result;	// 0=成功 -1=未开启 -2最大等级 -3金币不足 -4进阶石不足 -5元宝不足 -6材料不足
		public int RideLevel;	// 坐骑阶位
		public int StarProgress;	// 星级进度
		public int UpType;	// 1 普通成长,2 双倍成长
		public int UpLevelNum;	// 疲劳值

		public override void unpack(NetReadBuffer buffer)
		{
			Result = buffer.ReadInt32();
			RideLevel = buffer.ReadInt32();
			StarProgress = buffer.ReadInt32();
			UpType = buffer.ReadInt32();
			UpLevelNum = buffer.ReadInt32();
		}
	}

	// C->S 客户端请求：更改坐骑 msgId:3121
	public class MsgData_cChangeRide : MsgData
	{
		public int ID;	// 坐骑id

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(ID);
		}
	}

	// S->C 服务器通知：返回更改坐骑 msgId:8121
	public class MsgData_sChangeRideId : MsgData
	{
		public int ID;	// 坐骑id
		public int State;	// 骑乘状态,0下马,1上马

		public override void unpack(NetReadBuffer buffer)
		{
			ID = buffer.ReadInt32();
			State = buffer.ReadInt32();
		}
	}

	// C->S 客户端请求：请求UI信息 msgId:3759
	public class MsgData_cZhiYuanFb : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// C->S 客户端请求：请求挑战 msgId:3760
	public class MsgData_cZhiYuanFbChallenge : MsgData
	{
		public int ID;	// 副本ID

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(ID);
		}
	}

	// C->S 客户端请求：领取宝箱 msgId:3761
	public class MsgData_cZhiYuanFbRequestReward : MsgData
	{
		public int ID;	// 副本ID

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(ID);
		}
	}

	// C->S 客户端请求：请求退出 msgId:3762
	public class MsgData_cZhiYuanFbQuit : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// C->S 客户端请求：请求扫荡 msgId:3764
	public class MsgData_cZhiYuanFbWipe : MsgData
	{
		public int ID;	// 副本ID
		public int Num;	// 扫荡次数

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(ID);
			buffer.WriteInt32(Num);
		}
	}

	// C->S 客户端请求：请求购买次数 msgId:3765
	public class MsgData_cZhiYuanFbVigor : MsgData
	{
		public int CID;	// 章节ID

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(CID);
		}
	}

	// C->S 客户端请求：请求领取宝箱奖励 msgId:3766
	public class MsgData_cZhiYuanFbBoxReward : MsgData
	{
		public int ID;	// 宝箱ID

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(ID);
		}
	}

	// C->S 客户端请求：请求一键扫荡 msgId:3769
	public class MsgData_cZhiYuanFbImmediately : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// S->C 章节信息
	public class MsgData_sVeilvo : MsgData
	{
		public int RewardState;	// 宝箱状态: 0未领取 1已领取

		public override void unpack(NetReadBuffer buffer)
		{
			RewardState = buffer.ReadInt32();
		}
	}

	// S->C 副本信息
	public class MsgData_sStagevo : MsgData
	{
		public int Num;	// 剩余挑战次数
		public int ID;	// ID
		public int State;	// 0 没扫荡 1在扫荡
		public long TimeNum;	// 扫荡剩余时间0 待领取倒计时
		public int MaxNum;	// 总次数
		public int Evaluate;	// 评价1-3星
		public int RewardType;	// 0:未开启1：可领取2：已领取

		public override void unpack(NetReadBuffer buffer)
		{
			Num = buffer.ReadInt32();
			ID = buffer.ReadInt32();
			State = buffer.ReadInt32();
			TimeNum = buffer.ReadInt64();
			MaxNum = buffer.ReadInt32();
			Evaluate = buffer.ReadInt32();
			RewardType = buffer.ReadInt32();
		}
	}

	// S->C 服务器通知：返回UI信息 msgId:8759
	public class MsgData_sZhiYuanFbData : MsgData
	{
		public sbyte[] EnterNumList = new sbyte[128];	// 可挑战次数
		public MsgData_sVeilvo[] VeilList = new MsgData_sVeilvo[11];	// 章节list
		public uint StageListCount;	// 副本信息数量
		public List<MsgData_sStagevo> StageList = new List<MsgData_sStagevo>();	// 副本信息列表

		public override void unpack(NetReadBuffer buffer)
		{
			for (int i = 0; i < 128; i++)
			{
				sbyte __item = buffer.ReadInt8();
				EnterNumList[i]=__item;
			}
			for (int i = 0; i < 11; i++)
			{
				MsgData_sVeilvo __item = new MsgData_sVeilvo();
				__item.unpack(buffer);
				VeilList[i]=__item;
			}
			StageListCount = buffer.ReadUInt32();
			StageList = new List<MsgData_sStagevo>();
			for (int i = 0; i < StageListCount; i++)
			{
				MsgData_sStagevo __item = new MsgData_sStagevo();
				__item.unpack(buffer);
				StageList.Add(__item);
			}
		}
	}

	// S->C 服务器通知：刷新 msgId:8760
	public class MsgData_sZhiYuanFbUpDate : MsgData
	{
		public int Num;	// 剩余挑战次数
		public int State;	// 0 没扫荡 1在扫荡
		public long TimeNum;	// 扫荡剩余时间0 待领取倒计时
		public int ID;	// ID
		public int RewardType;	// 0:未开启1：可领取2：已领取

		public override void unpack(NetReadBuffer buffer)
		{
			Num = buffer.ReadInt32();
			State = buffer.ReadInt32();
			TimeNum = buffer.ReadInt64();
			ID = buffer.ReadInt32();
			RewardType = buffer.ReadInt32();
		}
	}

	// S->C 服务器通知：返回挑战 msgId:8761
	public class MsgData_sBackZhiYuanFbChallenge : MsgData
	{
		public int Result;	// 进入结果返回: 0失败 1成功
		public int ID;	// 挑战编号

		public override void unpack(NetReadBuffer buffer)
		{
			Result = buffer.ReadInt32();
			ID = buffer.ReadInt32();
		}
	}

	// S->C 服务器通知：返回退出 msgId:8762
	public class MsgData_sBackZhiYuanFbQuit : MsgData
	{
		public int Result;	// 退出结果返回: 0失败 1成功

		public override void unpack(NetReadBuffer buffer)
		{
			Result = buffer.ReadInt32();
		}
	}

	// S->C 服务器通知：返回追踪信息 msgId:8763
	public class MsgData_sBackZhiYuanFbInfo : MsgData
	{
		public int Num;	// 待定~~

		public override void unpack(NetReadBuffer buffer)
		{
			Num = buffer.ReadInt32();
		}
	}

	// S->C 服务器通知：返回扫荡 msgId:8764
	public class MsgData_sBackZhiYuanFbWipe : MsgData
	{
		public int Result;	// 扫荡结果返回: 0失败 1成功
		public int ID;	// 扫荡编号
		public int Num;	// 返回扫荡次数

		public override void unpack(NetReadBuffer buffer)
		{
			Result = buffer.ReadInt32();
			ID = buffer.ReadInt32();
			Num = buffer.ReadInt32();
		}
	}

	// S->C 服务器通知：返回购买次数 msgId:8765
	public class MsgData_sBackZhiYuanFbVigor : MsgData
	{
		public int Result;	// 购买结果返回: 0成功 1失败
		public int CID;	// 章节ID

		public override void unpack(NetReadBuffer buffer)
		{
			Result = buffer.ReadInt32();
			CID = buffer.ReadInt32();
		}
	}

	// S->C 服务器通知：返回领取宝箱奖励 msgId:8766
	public class MsgData_sBackZhiYuanFbBoxReward : MsgData
	{
		public int Result;	// 领取结果返回: 0失败 1成功
		public int ID;	// 领取编号

		public override void unpack(NetReadBuffer buffer)
		{
			Result = buffer.ReadInt32();
			ID = buffer.ReadInt32();
		}
	}

	// S->C 服务器通知：返回通关 msgId:8767
	public class MsgData_sBackZhiYuanFbEnd : MsgData
	{
		public int Level;	// 评定
		public int Result;	// 结果返回: 0失败 1成功

		public override void unpack(NetReadBuffer buffer)
		{
			Level = buffer.ReadInt32();
			Result = buffer.ReadInt32();
		}
	}

	// S->C 服务器通知：返回扫荡结束 msgId:8768
	public class MsgData_sBackZhiYuanFbMopupEnd : MsgData
	{
		public int ID;	// 扫荡完成的ID
		public int Result;	// 扫荡结果
		public int Num;	// 扫荡次数

		public override void unpack(NetReadBuffer buffer)
		{
			ID = buffer.ReadInt32();
			Result = buffer.ReadInt32();
			Num = buffer.ReadInt32();
		}
	}

	// S->C 服务器通知：返回挑战首通结果 msgId:8769
	public class MsgData_sZhiYuanFbFirstChallenge : MsgData
	{
		public int ID;	// 挑战ID

		public override void unpack(NetReadBuffer buffer)
		{
			ID = buffer.ReadInt32();
		}
	}

	// C->S 客户端请求：发送定义的PK规则 msgId:3172
	public class MsgData_cSetPKRule : MsgData
	{
		public int ID;	// PK规则
		public int SelfPK;	// 自己定义的PK规则

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(ID);
			buffer.WriteInt32(SelfPK);
		}
	}

	// S->C 服务器通知：返回定义的PK规则 msgId:8172
	public class MsgData_sSetPKRule : MsgData
	{
		public int ID;	// PK规则
		public int SelfPK;	// 自己定义的PK规则
		public int State;	// 当前PK的状态

		public override void unpack(NetReadBuffer buffer)
		{
			ID = buffer.ReadInt32();
			SelfPK = buffer.ReadInt32();
			State = buffer.ReadInt32();
		}
	}

	// C->S 客户端请求：请求队伍信息,不在队伍中不返回 msgId:2007
	public class MsgData_cTeamInfo : MsgData
	{
		public long TeamID;	// 队伍id

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(TeamID);
		}
	}

	// S->C 队伍成员信息
	public class MsgData_sTeamRole : MsgData
	{
		public long RoleID;	// 角色ID
		public byte[] RoleName = new byte[32];	// 角色名字
		public int Line;	// 分线
		public int MapID;	// 地图
		public int Prof;	// 职业
		public int Level;	// 等级
		public int HP;	// 当前血量
		public int MaxHP;	// 血量上限
		public int Mp;	// 当前法力
		public int MaxMP;	// 法力上限
		public long Power;	// 战斗力
		public byte[] GuildName = new byte[32];	// 帮会名
		public sbyte TeamPos;	// 职位,0成员,1队长
		public sbyte Online;	// 在线状态
		public int Icon;	// 玩家头像
		public int Arms;	// 武器
		public int Dress;	// 衣服
		public int FashionsHead;	// 时装头
		public int FashionsArms;	// 时装武器
		public int FashionsDress;	// 时装衣服
		public int WuhunID;	// 武魂id
		public int Wing;	// 翅膀
		public int SuitFlag;	// 套装标识
		public int MagicWeapon;	// 神兵ID
		public int VIPLevel;	// VIP等级
		public int RoomType;	// 准备状态

		public override void unpack(NetReadBuffer buffer)
		{
			RoleID = buffer.ReadInt64();
			buffer.ReadBytes(RoleName);
			Line = buffer.ReadInt32();
			MapID = buffer.ReadInt32();
			Prof = buffer.ReadInt32();
			Level = buffer.ReadInt32();
			HP = buffer.ReadInt32();
			MaxHP = buffer.ReadInt32();
			Mp = buffer.ReadInt32();
			MaxMP = buffer.ReadInt32();
			Power = buffer.ReadInt64();
			buffer.ReadBytes(GuildName);
			TeamPos = buffer.ReadInt8();
			Online = buffer.ReadInt8();
			Icon = buffer.ReadInt32();
			Arms = buffer.ReadInt32();
			Dress = buffer.ReadInt32();
			FashionsHead = buffer.ReadInt32();
			FashionsArms = buffer.ReadInt32();
			FashionsDress = buffer.ReadInt32();
			WuhunID = buffer.ReadInt32();
			Wing = buffer.ReadInt32();
			SuitFlag = buffer.ReadInt32();
			MagicWeapon = buffer.ReadInt32();
			VIPLevel = buffer.ReadInt32();
			RoomType = buffer.ReadInt32();
		}
	}

	// S->C 服务器通知：返回队伍信息(登录时推一次) msgId:7007
	public class MsgData_sTeamInfo : MsgData
	{
		public long TeamID;	// 队伍id
		public uint RoleListCount;	// 队伍成员数量
		public List<MsgData_sTeamRole> RoleList = new List<MsgData_sTeamRole>();	// 队伍成员列表

		public override void unpack(NetReadBuffer buffer)
		{
			TeamID = buffer.ReadInt64();
			RoleListCount = buffer.ReadUInt32();
			RoleList = new List<MsgData_sTeamRole>();
			for (int i = 0; i < RoleListCount; i++)
			{
				MsgData_sTeamRole __item = new MsgData_sTeamRole();
				__item.unpack(buffer);
				RoleList.Add(__item);
			}
		}
	}

	// C->S 客户端请求：请求创建队伍(结果走公告聊天) msgId:2008
	public class MsgData_cTeamCreate : MsgData
	{
		public long TargetRoleID;	// 被邀请人,没有直接创建队伍,有创建队伍并邀请
		public int targetID;	// 组队目标ID。默认填0

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(TargetRoleID);
			buffer.WriteInt32(targetID);
		}
	}

	// C->S 客户端请求：申请入队(结果走公告聊天) msgId:2009
	public class MsgData_cTeamApply : MsgData
	{
		public long TeamID;	// 队伍id

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(TeamID);
		}
	}

	// C->S 客户端请求：邀请入队(结果走公告聊天) msgId:2010
	public class MsgData_cTeamInvite : MsgData
	{
		public long TargetRoleID;	// 被邀请人
		public int Type;	// 邀请类型 0:正常组队 1:魔域深渊

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(TargetRoleID);
			buffer.WriteInt32(Type);
		}
	}

	// C->S 客户端请求：请求退出队伍(结果走公告聊天) msgId:2011
	public class MsgData_cTeamQuit : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// S->C 服务器通知：广播,有人退出队伍 msgId:7016
	public class MsgData_sTeamRoleExit : MsgData
	{
		public long RoleID;	// 角色ID

		public override void unpack(NetReadBuffer buffer)
		{
			RoleID = buffer.ReadInt64();
		}
	}

	// C->S 客户端请求：请求转让队长(结果走公告聊天) msgId:2012
	public class MsgData_cTeamTransfer : MsgData
	{
		public long TargetRoleID;	// 转让目标

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(TargetRoleID);
		}
	}

	// C->S 客户端请求：请求开除(结果走公告聊天) msgId:2013
	public class MsgData_cTeamFire : MsgData
	{
		public long TargetRoleID;	// 开除目标

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(TargetRoleID);
		}
	}

	// C->S 客户端请求：入队审批 msgId:2017
	public class MsgData_cTeamJoinApprove : MsgData
	{
		public long TargetRoleID;	// 目标
		public int Operate;	// 1同意0拒绝

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(TargetRoleID);
			buffer.WriteInt32(Operate);
		}
	}

	// S->C 服务器通知：入队请求(仅队长) msgId:7017
	public class MsgData_sTeamJoinRequest : MsgData
	{
		public long RoleID;	// 角色ID
		public byte[] RoleName = new byte[32];	// 角色名字
		public int Power;	// 战力
		public int Level;	// 等级
		public int Prof;	// 职业

		public override void unpack(NetReadBuffer buffer)
		{
			RoleID = buffer.ReadInt64();
			buffer.ReadBytes(RoleName);
			Power = buffer.ReadInt32();
			Level = buffer.ReadInt32();
			Prof = buffer.ReadInt32();
		}
	}

	// C->S 客户端请求：入队邀请反馈 msgId:2018
	public class MsgData_cTeamInviteApprove : MsgData
	{
		public long TeamID;	// 队伍id
		public int Operate;	// 1同意0拒绝
		public int Type;	// 邀请类型 0:正常组队 1:魔域深渊

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(TeamID);
			buffer.WriteInt32(Operate);
			buffer.WriteInt32(Type);
		}
	}

	// S->C 服务器通知：入队邀请 msgId:7018
	public class MsgData_sTeamInviteRequest : MsgData
	{
		public long TeamID;	// 队伍id
		public byte[] LeaderName = new byte[32];	// 队长名字
		public int Type;	// 邀请类型 0:正常组队 1:魔域深渊

		public override void unpack(NetReadBuffer buffer)
		{
			TeamID = buffer.ReadInt64();
			buffer.ReadBytes(LeaderName);
			Type = buffer.ReadInt32();
		}
	}

	// C->S 客户端请求：组队设置 msgId:2019
	public class MsgData_cTeamSetting : MsgData
	{
		public int AutoTeam;	// 自己,自动组队1, 需询问0
		public int AutoAgreeEnter;	// 队长,自动同意进入队伍1, 需询问0

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(AutoTeam);
			buffer.WriteInt32(AutoAgreeEnter);
		}
	}

	// C->S 客户端请求：请求附近队伍 msgId:2020
	public class MsgData_cTeamNearbyTeam : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// S->C 队伍情况
	public class MsgData_sTeamCase : MsgData
	{
		public long TeamID;	// 队伍ID
		public byte[] LeaderName = new byte[32];	// 队长名字
		public int MaxRoleLevel;	// 最高等级
		public int AverageRoleLevel;	// 平均等级
		public long MaxRolePower;	// 最高战斗力
		public long AverageRolePower;	// 平均战斗力
		public int RoleNum;	// 成员数量

		public override void unpack(NetReadBuffer buffer)
		{
			TeamID = buffer.ReadInt64();
			buffer.ReadBytes(LeaderName);
			MaxRoleLevel = buffer.ReadInt32();
			AverageRoleLevel = buffer.ReadInt32();
			MaxRolePower = buffer.ReadInt64();
			AverageRolePower = buffer.ReadInt64();
			RoleNum = buffer.ReadInt32();
		}
	}

	// S->C 服务器通知：返回附近队伍 msgId:7020
	public class MsgData_sTeamNearbyTeam : MsgData
	{
		public uint TeamListCount;	// 队伍数量
		public List<MsgData_sTeamCase> TeamList = new List<MsgData_sTeamCase>();	// 队伍列表

		public override void unpack(NetReadBuffer buffer)
		{
			TeamListCount = buffer.ReadUInt32();
			TeamList = new List<MsgData_sTeamCase>();
			for (int i = 0; i < TeamListCount; i++)
			{
				MsgData_sTeamCase __item = new MsgData_sTeamCase();
				__item.unpack(buffer);
				TeamList.Add(__item);
			}
		}
	}

	// C->S 客户端请求：请求附近玩家 msgId:2021
	public class MsgData_cTeamNearbyRole : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// S->C 附加玩家信息
	public class MsgData_sTeamNearbyRoleInfo : MsgData
	{
		public long RoleID;	// 角色ID
		public byte[] RoleName = new byte[32];	// 角色名字
		public int Level;	// 等级
		public int Prof;	// 职业
		public int TeamState;	// 组队状态 0未组队,1已组队
		public byte[] GuildName = new byte[32];	// 公会名称
		public int GuildPos;	// 公会职位
		public int Power;	// 战斗力

		public override void unpack(NetReadBuffer buffer)
		{
			RoleID = buffer.ReadInt64();
			buffer.ReadBytes(RoleName);
			Level = buffer.ReadInt32();
			Prof = buffer.ReadInt32();
			TeamState = buffer.ReadInt32();
			buffer.ReadBytes(GuildName);
			GuildPos = buffer.ReadInt32();
			Power = buffer.ReadInt32();
		}
	}

	// S->C 服务器通知：返回附近玩家 msgId:7021
	public class MsgData_sTeamNearbyRole : MsgData
	{
		public uint RoleListCount;	// 玩家数量
		public List<MsgData_sTeamNearbyRoleInfo> RoleList = new List<MsgData_sTeamNearbyRoleInfo>();	// 玩家列表

		public override void unpack(NetReadBuffer buffer)
		{
			RoleListCount = buffer.ReadUInt32();
			RoleList = new List<MsgData_sTeamNearbyRoleInfo>();
			for (int i = 0; i < RoleListCount; i++)
			{
				MsgData_sTeamNearbyRoleInfo __item = new MsgData_sTeamNearbyRoleInfo();
				__item.unpack(buffer);
				RoleList.Add(__item);
			}
		}
	}

	// S->C 服务器通知：广播,有人进入队伍 msgId:7014
	public class MsgData_sTeamRoleJoin : MsgData
	{
		public long TeamID;	// 队伍id
		public long RoleID;	// 角色ID
		public byte[] RoleName = new byte[32];	// 角色名字
		public int Line;	// 分线
		public int MapID;	// 地图
		public int Prof;	// 职业
		public int Level;	// 等级
		public int HP;	// 当前血量
		public int MaxHP;	// 血量上限
		public int Mp;	// 当前法力
		public int MaxMP;	// 法力上限
		public long Power;	// 战斗力
		public byte[] GuildName = new byte[32];	// 帮会名,无帮会则发空字符串
		public sbyte TeamPos;	// 职位,0成员,1队长
		public sbyte Online;	// 在线状态1:在线, 0:不在线
		public int Icon;	// 玩家头像
		public int Arms;	// 武器
		public int Dress;	// 衣服
		public int FashionsHead;	// 时装头
		public int FashionsArms;	// 时装武器
		public int FashionsDress;	// 时装衣服
		public int WuhunID;	// 武魂id
		public int Wing;	// 翅膀
		public int SuitFlag;	// 套装标识
		public int MagicWeapon;	// 神兵ID
		public int VIPLevel;	// VIP等级
		public int RoomType;	// 准备状态

		public override void unpack(NetReadBuffer buffer)
		{
			TeamID = buffer.ReadInt64();
			RoleID = buffer.ReadInt64();
			buffer.ReadBytes(RoleName);
			Line = buffer.ReadInt32();
			MapID = buffer.ReadInt32();
			Prof = buffer.ReadInt32();
			Level = buffer.ReadInt32();
			HP = buffer.ReadInt32();
			MaxHP = buffer.ReadInt32();
			Mp = buffer.ReadInt32();
			MaxMP = buffer.ReadInt32();
			Power = buffer.ReadInt64();
			buffer.ReadBytes(GuildName);
			TeamPos = buffer.ReadInt8();
			Online = buffer.ReadInt8();
			Icon = buffer.ReadInt32();
			Arms = buffer.ReadInt32();
			Dress = buffer.ReadInt32();
			FashionsHead = buffer.ReadInt32();
			FashionsArms = buffer.ReadInt32();
			FashionsDress = buffer.ReadInt32();
			WuhunID = buffer.ReadInt32();
			Wing = buffer.ReadInt32();
			SuitFlag = buffer.ReadInt32();
			MagicWeapon = buffer.ReadInt32();
			VIPLevel = buffer.ReadInt32();
			RoomType = buffer.ReadInt32();
		}
	}

	// S->C 服务器通知：广播,队伍成员信息变化 msgId:7015
	public class MsgData_sTeamRoleUpdate : MsgData
	{
		public long RoleID;	// 角色ID
		public byte[] RoleName = new byte[32];	// 角色名字
		public int Line;	// 分线
		public int MapID;	// 地图
		public int Prof;	// 职业
		public int Level;	// 等级
		public byte[] GuildName = new byte[32];	// 帮会名,无帮会则发空字符串
		public sbyte TeamPos;	// 职位,0成员,1队长
		public sbyte Online;	// 在线状态1:在线, 0:不在线
		public int Icon;	// 玩家头像
		public int Arms;	// 武器
		public int Dress;	// 衣服
		public int FashionsHead;	// 时装头
		public int FashionsArms;	// 时装武器
		public int FashionsDress;	// 时装衣服
		public int WuhunID;	// 武魂id
		public int Wing;	// 翅膀
		public int SuitFlag;	// 套装标识
		public int MagicWeapon;	// 神兵ID
		public int VIPLevel;	// VIP等级
		public int RoomType;	// 准备状态

		public override void unpack(NetReadBuffer buffer)
		{
			RoleID = buffer.ReadInt64();
			buffer.ReadBytes(RoleName);
			Line = buffer.ReadInt32();
			MapID = buffer.ReadInt32();
			Prof = buffer.ReadInt32();
			Level = buffer.ReadInt32();
			buffer.ReadBytes(GuildName);
			TeamPos = buffer.ReadInt8();
			Online = buffer.ReadInt8();
			Icon = buffer.ReadInt32();
			Arms = buffer.ReadInt32();
			Dress = buffer.ReadInt32();
			FashionsHead = buffer.ReadInt32();
			FashionsArms = buffer.ReadInt32();
			FashionsDress = buffer.ReadInt32();
			WuhunID = buffer.ReadInt32();
			Wing = buffer.ReadInt32();
			SuitFlag = buffer.ReadInt32();
			MagicWeapon = buffer.ReadInt32();
			VIPLevel = buffer.ReadInt32();
			RoomType = buffer.ReadInt32();
		}
	}

	// S->C 服务器通知：广播,队伍成员(hp, mp)更新 msgId:7025
	public class MsgData_sTeamRoleUpdateInfo : MsgData
	{
		public long RoleID;	// 角色ID
		public int HP;	// 当前血量
		public int MaxHP;	// 血量上限
		public int Mp;	// 当前法力
		public int MaxMP;	// 法力上限
		public long Power;	// 战斗力

		public override void unpack(NetReadBuffer buffer)
		{
			RoleID = buffer.ReadInt64();
			HP = buffer.ReadInt32();
			MaxHP = buffer.ReadInt32();
			Mp = buffer.ReadInt32();
			MaxMP = buffer.ReadInt32();
			Power = buffer.ReadInt64();
		}
	}

	// C->S 客户端请求：队员回复是否同意加入组队副本 msgId:2037
	public class MsgData_cReplyTeamDungeon : MsgData
	{
		public int Reply;	// 答复结果:1同意，0拒绝

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(Reply);
		}
	}

	// S->C 附加玩家信息
	public class MsgData_sDungeonTeamStatus : MsgData
	{
		public long RoleID;	// 角色ID
		public int Status;	// 玩家状态 0:已同意，1:等待确认中，2:已拒绝

		public override void unpack(NetReadBuffer buffer)
		{
			RoleID = buffer.ReadInt64();
			Status = buffer.ReadInt32();
		}
	}

	// S->C 服务器通知：更新组队副本确认的队员列表 msgId:7037
	public class MsgData_sTeamDungeonUpdate : MsgData
	{
		public int DungeonId;	// 副本ID
		public int Line;	// 队长所在线
		public uint RoleListCount;	// 玩家数量
		public List<MsgData_sDungeonTeamStatus> RoleList = new List<MsgData_sDungeonTeamStatus>();	// 玩家列表

		public override void unpack(NetReadBuffer buffer)
		{
			DungeonId = buffer.ReadInt32();
			Line = buffer.ReadInt32();
			RoleListCount = buffer.ReadUInt32();
			RoleList = new List<MsgData_sDungeonTeamStatus>();
			for (int i = 0; i < RoleListCount; i++)
			{
				MsgData_sDungeonTeamStatus __item = new MsgData_sDungeonTeamStatus();
				__item.unpack(buffer);
				RoleList.Add(__item);
			}
		}
	}

	// C->S 客户端请求：返回请求进入组队活动状态 msgId:2246
	public class MsgData_cEnterDulpPrepare : MsgData
	{
		public int Prepare;	// 准备状态 0 true 1 false
		public int Type;	// 邀请类型 0:正常组队 1:魔域深渊  2:奇遇 3:公会组队

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(Prepare);
			buffer.WriteInt32(Type);
		}
	}

	// S->C 服务器通知：广播,队伍成员信息变化 msgId:7248
	public class MsgData_sUpdateTeamPreparte : MsgData
	{
		public long RoleID;	// 角色ID
		public int RoomType;	// 准备状态 0 true 1 false
		public int Type;	// 邀请类型 0:正常组队 1:魔域深渊  2:奇遇 3:公会组队

		public override void unpack(NetReadBuffer buffer)
		{
			RoleID = buffer.ReadInt64();
			RoomType = buffer.ReadInt32();
			Type = buffer.ReadInt32();
		}
	}

	// C->S 客户端请求：请求发布招募 msgId:2247
	public class MsgData_cTeamSecreZhaoMu : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// C->S 客户端请求：目标队伍列表 msgId:2911
	public class MsgData_cTargetTeamList : MsgData
	{
		public int TargetID;	// 目标id

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(TargetID);
		}
	}

	// S->C 服务器通知：目标队伍信息
	public class MsgData_sTargetTeamInfo : MsgData
	{
		public long TeamID;	// 队伍ID
		public byte[] LeaderName = new byte[32];	// 队长名字
		public int LeaderProf;	// 队长职业
		public int TargetID;	// 目标ID
		public int AverageLevel;	// 平均等级
		public long AveragePower;	// 平均战斗力
		public int RoleNum;	// 成员数量

		public override void unpack(NetReadBuffer buffer)
		{
			TeamID = buffer.ReadInt64();
			buffer.ReadBytes(LeaderName);
			LeaderProf = buffer.ReadInt32();
			TargetID = buffer.ReadInt32();
			AverageLevel = buffer.ReadInt32();
			AveragePower = buffer.ReadInt64();
			RoleNum = buffer.ReadInt32();
		}
	}

	// S->C 服务器通知：目标队伍列表 msgId:7908
	public class MsgData_sTargetTeamList : MsgData
	{
		public uint TeamListCount;	// 队伍数量
		public List<MsgData_sTargetTeamInfo> TeamList = new List<MsgData_sTargetTeamInfo>();	// 队伍列表

		public override void unpack(NetReadBuffer buffer)
		{
			TeamListCount = buffer.ReadUInt32();
			TeamList = new List<MsgData_sTargetTeamInfo>();
			for (int i = 0; i < TeamListCount; i++)
			{
				MsgData_sTargetTeamInfo __item = new MsgData_sTargetTeamInfo();
				__item.unpack(buffer);
				TeamList.Add(__item);
			}
		}
	}

	// C->S 客户端请求：设置自动组队标志 msgId:2912
	public class MsgData_cSetAutoTeam : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// C->S 客户端请求：自动匹配队伍 msgId:2913
	public class MsgData_cAutoTeam : MsgData
	{
		public int Type;	// 1匹配 2停止匹配

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(Type);
		}
	}

	// S->C 服务器通知：自动组队设置 msgId:7910
	public class MsgData_sAutoTeamSetting : MsgData
	{
		public int Setting;	// 自动组队状态

		public override void unpack(NetReadBuffer buffer)
		{
			Setting = buffer.ReadInt32();
		}
	}

	// S->C 服务器通知：自动匹配队伍结果 msgId:7914
	public class MsgData_sAutoTeamResult : MsgData
	{
		public int Result;	// 

		public override void unpack(NetReadBuffer buffer)
		{
			Result = buffer.ReadInt32();
		}
	}

	// C->S 服务器通知：请求推荐好友列表 msgId：2030
	public class MsgData_cAskRecommendList : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// C->S 服务器通知：查找好友 msgId：2025
	public class MsgData_cFindFriend : MsgData
	{
		public byte[] RoleName = new byte[32];	// 角色名字

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteBytes(RoleName);
		}
	}

	// C->S 添加推荐好友信息
	public class MsgData_cAddFriendVO : MsgData
	{
		public long RoleID;	// 角色ID

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(RoleID);
		}
	}

	// C->S 服务器通知：添加推荐好友 msgId：2029
	public class MsgData_cAddFriendRecommend : MsgData
	{
		public uint AddFriendCount;	// 玩家数量
		public List<MsgData_cAddFriendVO> AddFriendList = new List<MsgData_cAddFriendVO>();	// 玩家列表

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteUInt32(AddFriendCount);
			for (int i = 0; i < AddFriendCount; i++)
			{
				MsgData_cAddFriendVO __item = AddFriendList[i];
				__item.pack(buffer);
			}
		}
	}

	// C->S 目标添加好友反馈信息
	public class MsgData_cFriendApproveVO : MsgData
	{
		public int Agree;	// 0-同意， 1-不同意
		public long RoleID;	// 角色ID

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(Agree);
			buffer.WriteInt64(RoleID);
		}
	}

	// C->S 服务器通知：目标添加好友反馈 msgId：2026
	public class MsgData_cFriendApproveList : MsgData
	{
		public uint ApproveCount;	// 玩家数量
		public List<MsgData_cFriendApproveVO> ApproveList = new List<MsgData_cFriendApproveVO>();	// 玩家列表

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteUInt32(ApproveCount);
			for (int i = 0; i < ApproveCount; i++)
			{
				MsgData_cFriendApproveVO __item = ApproveList[i];
				__item.pack(buffer);
			}
		}
	}

	// C->S 客户端请求：添加到黑名单 msgId：2027
	public class MsgData_cAddBlackList : MsgData
	{
		public long RoleID;	// 角色ID

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(RoleID);
		}
	}

	// C->S 客户端请求：删除关系 msgId：2028
	public class MsgData_cRemoveRelation : MsgData
	{
		public long RoleID;	// 角色ID
		public int RelationType;	// 关系类型 好友1，仇人2，黑名单3，最近联系人4

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(RoleID);
			buffer.WriteInt32(RelationType);
		}
	}

	// C->S 客户端请求：请求关系改变列表 msgId：2031
	public class MsgData_cRelationChangeList : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// C->S 客户端请求：请求领取好友礼包 msgId：2146
	public class MsgData_cFriendRewardGet : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// S->C 推荐好友信息
	public class MsgData_sFriendRecommendVO : MsgData
	{
		public long RoleID;	// 角色ID
		public byte[] RoleName = new byte[32];	// 角色名字
		public int Level;	// 等级
		public int Icon;	// 头像
		public int VIPLevel;	// VIP等级
		public long Power;	// 战斗力

		public override void unpack(NetReadBuffer buffer)
		{
			RoleID = buffer.ReadInt64();
			buffer.ReadBytes(RoleName);
			Level = buffer.ReadInt32();
			Icon = buffer.ReadInt32();
			VIPLevel = buffer.ReadInt32();
			Power = buffer.ReadInt64();
		}
	}

	// S->C 服务器通知：返回推荐好友列表 msgId：7030
	public class MsgData_sFriendRecommendList : MsgData
	{
		public uint RoleCount;	// 玩家数量
		public List<MsgData_sFriendRecommendVO> RoleList = new List<MsgData_sFriendRecommendVO>();	// 玩家列表

		public override void unpack(NetReadBuffer buffer)
		{
			RoleCount = buffer.ReadUInt32();
			RoleList = new List<MsgData_sFriendRecommendVO>();
			for (int i = 0; i < RoleCount; i++)
			{
				MsgData_sFriendRecommendVO __item = new MsgData_sFriendRecommendVO();
				__item.unpack(buffer);
				RoleList.Add(__item);
			}
		}
	}

	// S->C 服务器通知：查找好友请求返回 msgId：7027
	public class MsgData_sFindFriendTarget : MsgData
	{
		public long RoleID;	// 角色ID
		public byte[] RoleName = new byte[32];	// 角色名字
		public int Level;	// 等级

		public override void unpack(NetReadBuffer buffer)
		{
			RoleID = buffer.ReadInt64();
			buffer.ReadBytes(RoleName);
			Level = buffer.ReadInt32();
		}
	}

	// S->C 服务器通知：更新在线状态 msgId：7031
	public class MsgData_sRelationOnLineStatus : MsgData
	{
		public long RoleID;	// 角色ID
		public long LastLoginTime;	// 0在线 不为0表示最后登录时间

		public override void unpack(NetReadBuffer buffer)
		{
			RoleID = buffer.ReadInt64();
			LastLoginTime = buffer.ReadInt64();
		}
	}

	// S->C 服务器通知：返回可领取好友礼包 msgId：7145
	public class MsgData_sFriendReward : MsgData
	{
		public long RoleID;	// 角色ID
		public byte[] RoleName = new byte[32];	// 角色名字
		public int Level;	// 等级

		public override void unpack(NetReadBuffer buffer)
		{
			RoleID = buffer.ReadInt64();
			buffer.ReadBytes(RoleName);
			Level = buffer.ReadInt32();
		}
	}

	// S->C 服务器通知：返回领取好友礼包 msgId：7146
	public class MsgData_sFriendRewardGet : MsgData
	{
		public int Result;	// 结果：0成功

		public override void unpack(NetReadBuffer buffer)
		{
			Result = buffer.ReadInt32();
		}
	}

	// S->C 移除玩家信息
	public class MsgData_sRemoveRelationVO : MsgData
	{
		public long RoleID;	// 角色ID
		public sbyte RelationType;	// 关系类型 好友1，仇人2，黑名单3，最近联系人4

		public override void unpack(NetReadBuffer buffer)
		{
			RoleID = buffer.ReadInt64();
			RelationType = buffer.ReadInt8();
		}
	}

	// S->C 服务器通知：删除关系返回 msgId:7029
	public class MsgData_sRemoveRelation : MsgData
	{
		public uint RemoveCount;	// 玩家数量
		public List<MsgData_sRemoveRelationVO> RemoveList = new List<MsgData_sRemoveRelationVO>();	// 玩家列表

		public override void unpack(NetReadBuffer buffer)
		{
			RemoveCount = buffer.ReadUInt32();
			RemoveList = new List<MsgData_sRemoveRelationVO>();
			for (int i = 0; i < RemoveCount; i++)
			{
				MsgData_sRemoveRelationVO __item = new MsgData_sRemoveRelationVO();
				__item.unpack(buffer);
				RemoveList.Add(__item);
			}
		}
	}

	// S->C 玩家关系信息
	public class MsgData_sRelationVO : MsgData
	{
		public long RoleID;	// 角色ID
		public int Prof;	// 职业
		public sbyte RelationFlag;	// 关系标识
		public byte[] RoleName = new byte[32];	// 角色名字
		public int RelationDegree;	// 亲密度
		public int BeKillNum;	// 被杀次数
		public int Level;	// 等级
		public int Icon;	// 头像
		public int VIPLevel;	// VIP等级
		public long OnlineStatus;	// 在线状态 0不在线 1在线 2三天未登录
		public long TeamID;	// 队伍ID
		public long GuildID;	// 公会ID
		public sbyte GuildPos;	// 公会职务
		public long RecentTime;	// 最近联系时间
		public long KillTime;	// 击杀时间
		public long Power;	// 战斗力
		public int GloryLevel;	// 修为等级
		public int TXHFlag;	// 黄钻标识
		public int TXBFlag;	// 蓝钻标识
		public byte[] GuildName = new byte[32];	// 公会名字
		public int MapID;	// 当前地图

		public override void unpack(NetReadBuffer buffer)
		{
			RoleID = buffer.ReadInt64();
			Prof = buffer.ReadInt32();
			RelationFlag = buffer.ReadInt8();
			buffer.ReadBytes(RoleName);
			RelationDegree = buffer.ReadInt32();
			BeKillNum = buffer.ReadInt32();
			Level = buffer.ReadInt32();
			Icon = buffer.ReadInt32();
			VIPLevel = buffer.ReadInt32();
			OnlineStatus = buffer.ReadInt64();
			TeamID = buffer.ReadInt64();
			GuildID = buffer.ReadInt64();
			GuildPos = buffer.ReadInt8();
			RecentTime = buffer.ReadInt64();
			KillTime = buffer.ReadInt64();
			Power = buffer.ReadInt64();
			GloryLevel = buffer.ReadInt32();
			TXHFlag = buffer.ReadInt32();
			TXBFlag = buffer.ReadInt32();
			buffer.ReadBytes(GuildName);
			MapID = buffer.ReadInt32();
		}
	}

	// S->C 服务器通知：关系列表 msgId:7028
	public class MsgData_sRelationList : MsgData
	{
		public uint RelationCount;	// 玩家数量
		public List<MsgData_sRelationVO> RelationList = new List<MsgData_sRelationVO>();	// 玩家列表

		public override void unpack(NetReadBuffer buffer)
		{
			RelationCount = buffer.ReadUInt32();
			RelationList = new List<MsgData_sRelationVO>();
			for (int i = 0; i < RelationCount; i++)
			{
				MsgData_sRelationVO __item = new MsgData_sRelationVO();
				__item.unpack(buffer);
				RelationList.Add(__item);
			}
		}
	}

	// S->C 服务器通知：添加好友申请 msgId:7332
	public class MsgData_sAddFriendApply : MsgData
	{
		public long RoleID;	// 角色ID
		public byte[] RoleName = new byte[32];	// 角色名字
		public int Level;	// 等级

		public override void unpack(NetReadBuffer buffer)
		{
			RoleID = buffer.ReadInt64();
			buffer.ReadBytes(RoleName);
			Level = buffer.ReadInt32();
		}
	}

	// C->S 客户端请求：查看其他人信息 msgId:3217
	public class MsgData_cOtherHumanInfo : MsgData
	{
		public long RoleID;	// 角色ID
		public int Type;	// 查看类型 1:基本信息 2:详细信息 4:坐骑 8:武魂 16:装备宝石 32:卓越孔信息 64:身上道具 128:灵阵 256:神兵

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(RoleID);
			buffer.WriteInt32(Type);
		}
	}

	// S->C 服务器通知：查看其他人信息结果 msgId:8217
	public class MsgData_sOtherHumanInfoRet : MsgData
	{
		public int Result;	// 结果: 0成功 1失败

		public override void unpack(NetReadBuffer buffer)
		{
			Result = buffer.ReadInt32();
		}
	}

	// S->C 服务器通知：神武技能数据
	public class MsgData_sShenwuSkill : MsgData
	{
		public int SkillID;	// 神武技能id

		public override void unpack(NetReadBuffer buffer)
		{
			SkillID = buffer.ReadInt32();
		}
	}

	// S->C 服务器通知：物品装备信息卓越属性列表
	public class MsgData_sOtherHumanItemEquipSuperVO : MsgData
	{
		public long UID;	// 属性uid
		public int ID;	// 卓越id
		public int Val;	// 值1

		public override void unpack(NetReadBuffer buffer)
		{
			UID = buffer.ReadInt64();
			ID = buffer.ReadInt32();
			Val = buffer.ReadInt32();
		}
	}

	// S->C 服务器通知：物品装备信息新卓越属性列表
	public class MsgData_sOtherHumanItemEquipNewSuperVO : MsgData
	{
		public int ID;	// 新卓越id
		public int Wash;	// 洗练值

		public override void unpack(NetReadBuffer buffer)
		{
			ID = buffer.ReadInt32();
			Wash = buffer.ReadInt32();
		}
	}

	// S->C 服务器通知：物品装备信息
	public class MsgData_sOtherHumanItemEquipVO : MsgData
	{
		public int TID;	// 装备tid
		public sbyte Bind;	// 0 未绑定，1 已绑定
		public int StrenLevel;	// 强化等级
		public int StrenVal;	// 强化值
		public int RefinLevel;	// 炼化等级
		public int AttrAddLevel;	// 追加属性等级
		public int GroupId;	// 套装id
		public int GroupId2;	// 套装id2
		public int GroupLevel;	// 套装等级
		public int SuperNum;	// 卓越数量
		public int GodLevel;	// 神化等级
		public int BlessLevel;	// 神偌等级
		public int grades;	// 品阶
		public MsgData_sOtherHumanItemEquipSuperVO[] SuperList = new MsgData_sOtherHumanItemEquipSuperVO[7];	// 卓越属性列表
		public MsgData_sOtherHumanItemEquipNewSuperVO[] NewSuperList = new MsgData_sOtherHumanItemEquipNewSuperVO[6];	// 新卓越属性列表

		public override void unpack(NetReadBuffer buffer)
		{
			TID = buffer.ReadInt32();
			Bind = buffer.ReadInt8();
			StrenLevel = buffer.ReadInt32();
			StrenVal = buffer.ReadInt32();
			RefinLevel = buffer.ReadInt32();
			AttrAddLevel = buffer.ReadInt32();
			GroupId = buffer.ReadInt32();
			GroupId2 = buffer.ReadInt32();
			GroupLevel = buffer.ReadInt32();
			SuperNum = buffer.ReadInt32();
			GodLevel = buffer.ReadInt32();
			BlessLevel = buffer.ReadInt32();
			grades = buffer.ReadInt32();
			for (int i = 0; i < 7; i++)
			{
				MsgData_sOtherHumanItemEquipSuperVO __item = new MsgData_sOtherHumanItemEquipSuperVO();
				__item.unpack(buffer);
				SuperList[i]=__item;
			}
			for (int i = 0; i < 6; i++)
			{
				MsgData_sOtherHumanItemEquipNewSuperVO __item = new MsgData_sOtherHumanItemEquipNewSuperVO();
				__item.unpack(buffer);
				NewSuperList[i]=__item;
			}
		}
	}

	// S->C 服务器通知：返回他人信息 msgId:8218
	public class MsgData_sOtherHumanBSInfoRet : MsgData
	{
		public int ServerType;	// 是否全服排行信息 1=是 其余都不是
		public int Type;	// 1== 详细信息，2 == 排行榜详细信息
		public long RoleID;	// 角色ID
		public byte[] RoleName = new byte[32];	// 角色名字
		public int Prof;	// 职业
		public int Level;	// 等级
		public long HP;	// 当前血量
		public long MaxHP;	// 血量上限
		public int MP;	// 当前法力
		public int MaxMP;	// 法力上限
		public long Power;	// 战力
		public byte[] GuildName = new byte[32];	// 公会名字
		public int GuildPos;	// 公会职业
		public int VIPLevel;	// VIP等级
		public sbyte Sex;	// 性别
		public int Dress;	// 衣服
		public int Weapon;	// 武器
		public int ShenBing;	// 神兵
		public int FashionWing;	// 时装翅膀
		public int FashionWeapon;	// 时装武器
		public int FashionDress;	// 时装衣服
		public int MountState;	// 0未开启状态，1已开启状态
		public int WuhunState;	// 0未开启状态，1已开启状态
		public int WuhunId;	// 武魂id
		public int Wing;	// 翅膀
		public int PifengLevel;	// 披风使用等级
		public int SuitFlag;	// 套装标识
		public int GuanZhi;	// 官职
		public int Att;	// 攻击
		public int Def;	// 防御
		public int Hit;	// 命中
		public int Crit;	// 暴击
		public int Dodge;	// 闪避
		public int DefCri;	// 韧性
		public int AttSpeed;	// 攻击速度
		public int MoveSpeed;	// 移动速度
		public int HL;	// 魂力
		public int TP;	// 体魄
		public int SF;	// 身法
		public int JS;	// 精神
		public byte[] LoveName = new byte[32];	// 配偶名字
		public int BaoJiaLevel;	// 宝甲等级
		public int TianGangLevel;	// 天罡等级
		public int ZhanNuLevel;	// 战弩等级
		public int MingLunLevel;	// 命轮等级
		public int HunQiLevel;	// 魂器等级
		public int ShengQiLevel;	// 圣器等级
		public int ReincarnateType;	// 0没有专职，1=1转
		public int Shenwu;	// 神武等级 * 10000 + 神武星级
		public int Shenwuext;	// 神武淬炼等级 * 10000 + 神武神武淬炼星级
		public int QilinbiLevel;	// 麒麟臂等级
		public int JianyuLevel;	// 剑域等级
		public MsgData_sShenwuSkill[] ShenwuSkills = new MsgData_sShenwuSkill[3];	// 神武技能
		public uint ItemEquipCount;	// 装备物品数量
		public List<MsgData_sOtherHumanItemEquipVO> ItemEquipList = new List<MsgData_sOtherHumanItemEquipVO>();	// 装备物品列表

		public override void unpack(NetReadBuffer buffer)
		{
			ServerType = buffer.ReadInt32();
			Type = buffer.ReadInt32();
			RoleID = buffer.ReadInt64();
			buffer.ReadBytes(RoleName);
			Prof = buffer.ReadInt32();
			Level = buffer.ReadInt32();
			HP = buffer.ReadInt64();
			MaxHP = buffer.ReadInt64();
			MP = buffer.ReadInt32();
			MaxMP = buffer.ReadInt32();
			Power = buffer.ReadInt64();
			buffer.ReadBytes(GuildName);
			GuildPos = buffer.ReadInt32();
			VIPLevel = buffer.ReadInt32();
			Sex = buffer.ReadInt8();
			Dress = buffer.ReadInt32();
			Weapon = buffer.ReadInt32();
			ShenBing = buffer.ReadInt32();
			FashionWing = buffer.ReadInt32();
			FashionWeapon = buffer.ReadInt32();
			FashionDress = buffer.ReadInt32();
			MountState = buffer.ReadInt32();
			WuhunState = buffer.ReadInt32();
			WuhunId = buffer.ReadInt32();
			Wing = buffer.ReadInt32();
			PifengLevel = buffer.ReadInt32();
			SuitFlag = buffer.ReadInt32();
			GuanZhi = buffer.ReadInt32();
			Att = buffer.ReadInt32();
			Def = buffer.ReadInt32();
			Hit = buffer.ReadInt32();
			Crit = buffer.ReadInt32();
			Dodge = buffer.ReadInt32();
			DefCri = buffer.ReadInt32();
			AttSpeed = buffer.ReadInt32();
			MoveSpeed = buffer.ReadInt32();
			HL = buffer.ReadInt32();
			TP = buffer.ReadInt32();
			SF = buffer.ReadInt32();
			JS = buffer.ReadInt32();
			buffer.ReadBytes(LoveName);
			BaoJiaLevel = buffer.ReadInt32();
			TianGangLevel = buffer.ReadInt32();
			ZhanNuLevel = buffer.ReadInt32();
			MingLunLevel = buffer.ReadInt32();
			HunQiLevel = buffer.ReadInt32();
			ShengQiLevel = buffer.ReadInt32();
			ReincarnateType = buffer.ReadInt32();
			Shenwu = buffer.ReadInt32();
			Shenwuext = buffer.ReadInt32();
			QilinbiLevel = buffer.ReadInt32();
			JianyuLevel = buffer.ReadInt32();
			for (int i = 0; i < 3; i++)
			{
				MsgData_sShenwuSkill __item = new MsgData_sShenwuSkill();
				__item.unpack(buffer);
				ShenwuSkills[i]=__item;
			}
			ItemEquipCount = buffer.ReadUInt32();
			ItemEquipList = new List<MsgData_sOtherHumanItemEquipVO>();
			for (int i = 0; i < ItemEquipCount; i++)
			{
				MsgData_sOtherHumanItemEquipVO __item = new MsgData_sOtherHumanItemEquipVO();
				__item.unpack(buffer);
				ItemEquipList.Add(__item);
			}
		}
	}

	// S->C 服务器通知：返回其他人详细信息 msgId:8219
	public class MsgData_sOtherHumanXXInfoRet : MsgData
	{
		public long RoleID;	// 角色ID
		public int Dodge;	// 闪避
		public int DefCri;	// 韧性
		public int CriValue;	// 爆伤*1000
		public int SubCri;	// 免爆*1000
		public int AbsAtt;	// 真实伤害
		public int Parry;	// 格挡率*1000
		public int ParryValue;	// 格挡值
		public int AddDamage;	// 伤害增强*1000
		public int SubDamage;	// 伤害减免*1000
		public int PKValue;	// pk值
		public int PKHonor;	// 荣誉
		public int KillValue;	// 杀戮值
		public int SubDef;	// 破防刺穿
		public double Super;	// 卓越一击几率
		public double SuperValue;	// 卓越一击伤害
		public double DefSuper;	// 抗卓越一击几率
		public double DefSuperValue;	// 抗卓越一击伤害
		public double Vertigo;	// 眩晕
		public double DefVertigo;	// 抗眩晕
		public double ShenWei;	// 神威

		public override void unpack(NetReadBuffer buffer)
		{
			RoleID = buffer.ReadInt64();
			Dodge = buffer.ReadInt32();
			DefCri = buffer.ReadInt32();
			CriValue = buffer.ReadInt32();
			SubCri = buffer.ReadInt32();
			AbsAtt = buffer.ReadInt32();
			Parry = buffer.ReadInt32();
			ParryValue = buffer.ReadInt32();
			AddDamage = buffer.ReadInt32();
			SubDamage = buffer.ReadInt32();
			PKValue = buffer.ReadInt32();
			PKHonor = buffer.ReadInt32();
			KillValue = buffer.ReadInt32();
			SubDef = buffer.ReadInt32();
			Super = buffer.ReadDouble();
			SuperValue = buffer.ReadDouble();
			DefSuper = buffer.ReadDouble();
			DefSuperValue = buffer.ReadDouble();
			Vertigo = buffer.ReadDouble();
			DefVertigo = buffer.ReadDouble();
			ShenWei = buffer.ReadDouble();
		}
	}

	// S->C 服务器通知：返回其他人坐骑装备信息
	public class MsgData_sOtherMountItemEquipVO : MsgData
	{
		public int ID;	// 装备tid
		public int GroupID;	// 套装id
		public sbyte PillNum;	// 0 未绑定，1 已绑定

		public override void unpack(NetReadBuffer buffer)
		{
			ID = buffer.ReadInt32();
			GroupID = buffer.ReadInt32();
			PillNum = buffer.ReadInt8();
		}
	}

	// S->C 服务器通知：返回其他人坐骑技能信息
	public class MsgData_sOtherMountSkillVO : MsgData
	{
		public int ID;	// 技能编号
		public int Level;	// 等级

		public override void unpack(NetReadBuffer buffer)
		{
			ID = buffer.ReadInt32();
			Level = buffer.ReadInt32();
		}
	}

	// S->C 服务器通知：返回其他人坐骑属性信息
	public class MsgData_sOtherMountAttrVO : MsgData
	{
		public int Type;	// 属性
		public double ValX;	// 属性百分比

		public override void unpack(NetReadBuffer buffer)
		{
			Type = buffer.ReadInt32();
			ValX = buffer.ReadDouble();
		}
	}

	// S->C 服务器通知：返回其他人坐骑信息 msgId:8220
	public class MsgData_sOtherMountInfoRet : MsgData
	{
		public int Type;	// 1== 详细信息，2 == 排行榜详细信息
		public long RoleID;	// 角色ID
		public int RideLevel;	// 坐骑阶位
		public int StarProgress;	// 星级进度
		public int RideSelect;	// 选中坐骑
		public int PillNum;	// 属性丹数量
		public MsgData_sOtherMountItemEquipVO[] EquipList = new MsgData_sOtherMountItemEquipVO[4];	// 坐骑装备
		public MsgData_sOtherMountSkillVO[] SkillList = new MsgData_sOtherMountSkillVO[6];	// 坐骑装备
		public double FightPower;	// 战斗力
		public uint AttrCount;	// 属性数量
		public List<MsgData_sOtherMountAttrVO> AttrList = new List<MsgData_sOtherMountAttrVO>();	// 属性列表

		public override void unpack(NetReadBuffer buffer)
		{
			Type = buffer.ReadInt32();
			RoleID = buffer.ReadInt64();
			RideLevel = buffer.ReadInt32();
			StarProgress = buffer.ReadInt32();
			RideSelect = buffer.ReadInt32();
			PillNum = buffer.ReadInt32();
			for (int i = 0; i < 4; i++)
			{
				MsgData_sOtherMountItemEquipVO __item = new MsgData_sOtherMountItemEquipVO();
				__item.unpack(buffer);
				EquipList[i]=__item;
			}
			for (int i = 0; i < 6; i++)
			{
				MsgData_sOtherMountSkillVO __item = new MsgData_sOtherMountSkillVO();
				__item.unpack(buffer);
				SkillList[i]=__item;
			}
			FightPower = buffer.ReadDouble();
			AttrCount = buffer.ReadUInt32();
			AttrList = new List<MsgData_sOtherMountAttrVO>();
			for (int i = 0; i < AttrCount; i++)
			{
				MsgData_sOtherMountAttrVO __item = new MsgData_sOtherMountAttrVO();
				__item.unpack(buffer);
				AttrList.Add(__item);
			}
		}
	}

	// S->C 服务器通知：返回其他人道具信息
	public class MsgData_sOtherBodyToolVO : MsgData
	{
		public int Wing;	// 翅膀itemid
		public int WingState;	// 绑定状态
		public int Val1;	// 翅膀时代表过期时间
		public int Val2;	// 翅膀时代表是否特殊属性

		public override void unpack(NetReadBuffer buffer)
		{
			Wing = buffer.ReadInt32();
			WingState = buffer.ReadInt32();
			Val1 = buffer.ReadInt32();
			Val2 = buffer.ReadInt32();
		}
	}

	// S->C 服务器通知：返回身上道具信息 msgId:8452
	public class MsgData_sOtherBodyTool : MsgData
	{
		public int ServerType;	// 是否全服排行信息 1=是 其余都不是
		public int Type;	// 1== 详细信息，2 == 排行榜详细信息
		public long RoleID;	// 角色ID
		public double FightPower;	// 战斗力
		public uint BodyToolCount;	// 属性数量
		public List<MsgData_sOtherBodyToolVO> BodyToolList = new List<MsgData_sOtherBodyToolVO>();	// 属性列表

		public override void unpack(NetReadBuffer buffer)
		{
			ServerType = buffer.ReadInt32();
			Type = buffer.ReadInt32();
			RoleID = buffer.ReadInt64();
			FightPower = buffer.ReadDouble();
			BodyToolCount = buffer.ReadUInt32();
			BodyToolList = new List<MsgData_sOtherBodyToolVO>();
			for (int i = 0; i < BodyToolCount; i++)
			{
				MsgData_sOtherBodyToolVO __item = new MsgData_sOtherBodyToolVO();
				__item.unpack(buffer);
				BodyToolList.Add(__item);
			}
		}
	}

	// C->S 请求公会列表 msgId:2038
	public class MsgData_cQueryGuildList : MsgData
	{
		public int Page;	// 页数
		public int PageSize;	// 每页数量
		public int OnlyAutoAgree;	// 1 - 仅显示自动同意, 0 - 都显示

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(Page);
			buffer.WriteInt32(PageSize);
			buffer.WriteInt32(OnlyAutoAgree);
		}
	}

	// C->S 请求自己公会信息 msgId:2039
	public class MsgData_cQueryMyGuildInfo : MsgData
	{
		public int m_i4type;	// 0s是自己所有信息 1 是战力

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(m_i4type);
		}
	}

	// C->S 请求自己公会成员列表 msgId:2040
	public class MsgData_cQueryMyGuildMems : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// C->S 请求自己公会事件 msgId:2041
	public class MsgData_cQueryMyGuildEvent : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// C->S 请求创建公会 msgId:2042
	public class MsgData_cCreateGuild : MsgData
	{
		public byte[] Name = new byte[32];	// 公会名称
		public byte[] Notice = new byte[128];	// 公会公告

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteBytes(Name);
			buffer.WriteBytes(Notice);
		}
	}

	// C->S 退出公会 msgId:2043
	public class MsgData_cQuitGuild : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// C->S 解散公会 msgId:2044
	public class MsgData_cDismissGuild : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// C->S 升级公会 msgId:2045
	public class MsgData_cLvUpGuild : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// C->S 开启某组公会技能 msgId:2046
	public class MsgData_cLvUpGuildSkill : MsgData
	{
		public int GroupId;	// 技能ID组

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(GroupId);
		}
	}

	// C->S 改变职位 msgId:2047
	public class MsgData_cChangeGuildPos : MsgData
	{
		public long MemGid;	// 玩家ID
		public int Pos;	// 职位

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(MemGid);
			buffer.WriteInt32(Pos);
		}
	}

	// C->S 改变公会公告 msgId:2048
	public class MsgData_cChangeGuildNotice : MsgData
	{
		public byte[] Notice = new byte[128];	// 公告内容

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteBytes(Notice);
		}
	}

	// C->S 审核申请
	public class MsgData_cReqGuildApplyVo : MsgData
	{
		public long MemGid;	// 玩家ID

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(MemGid);
		}
	}

	// C->S 审核申请 msgId:2049
	public class MsgData_cVerifyGuildApply : MsgData
	{
		public int Verify;	// 是否同意0 - 同意，1 - 拒绝
		public int Size;	// 数组 size 
		public List<MsgData_cReqGuildApplyVo> GuildApplyList = new List<MsgData_cReqGuildApplyVo>();	// 数组 size 

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(Verify);
			buffer.WriteInt32(Size);
			for (int i = 0; i < Size; i++)
			{
				MsgData_cReqGuildApplyVo __item = GuildApplyList[i];
				__item.pack(buffer);
			}
		}
	}

	// C->S 踢出公会成员 msgId:2050
	public class MsgData_cKickGuildMem : MsgData
	{
		public long MemGid;	// 玩家ID

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(MemGid);
		}
	}

	// C->S 申请加入公会 msgId:2051
	public class MsgData_cApplyGuild : MsgData
	{
		public long GuildId;	// 玩家ID
		public int Apply;	// 0-取消， 1-申请

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(GuildId);
			buffer.WriteInt32(Apply);
		}
	}

	// C->S 邀请加入公会 msgId:2052
	public class MsgData_cInviteToGuild : MsgData
	{
		public long RoleId;	// 玩家ID

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(RoleId);
		}
	}

	// C->S 同意拒绝邀请加入公会 msgId:2053
	public class MsgData_cInviteToGuildResult : MsgData
	{
		public long InviterId;	// 邀请人ID
		public int Result;	// 结果 0 - 同意，1 - 不同意

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(InviterId);
			buffer.WriteInt32(Result);
		}
	}

	// C->S 设置自动申请审核 msgId:2055
	public class MsgData_cSetAutoVerify : MsgData
	{
		public int Auto;	// 结果 0 - 不自动 1 - 自动
		public int Level;	// 档数

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(Auto);
			buffer.WriteInt32(Level);
		}
	}

	// C->S 请求其他公会信息 msgId:2056
	public class MsgData_cQueryOtherGuildInfo : MsgData
	{
		public long GuildId;	// 公会id

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(GuildId);
		}
	}

	// C->S 请求自己公会申请 msgId:2057
	public class MsgData_cQueryMyGuildApplys : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// C->S 请求禅让帮主 msgId:2058
	public class MsgData_cChangeLeader : MsgData
	{
		public long MemGid;	// 玩家id

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(MemGid);
		}
	}

	// C->S  请求捐献 msgId:2059
	public class MsgData_cGuildContribute : MsgData
	{
		public int ItemId;	// 资源id
		public int Count;	// 数量

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(ItemId);
			buffer.WriteInt32(Count);
		}
	}

	// C->S 客户端请求：升级自身公会技能 msgId:2060
	public class MsgData_cLevelUpMyGuildSkill : MsgData
	{
		public int GroupId;	// 技能组ID

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(GroupId);
		}
	}

	// C->S 查找公会 msgId:2061
	public class MsgData_cSearchGuild : MsgData
	{
		public int Type;	//  查找类型, (0 -公会名， 1- 帮主名)
		public int PageSize;	// 每页数量
		public byte[] Name = new byte[32];	// 名称

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(Type);
			buffer.WriteInt32(PageSize);
			buffer.WriteBytes(Name);
		}
	}

	// C->S 客户端请求：请求加持属性 msgId:2071
	public class MsgData_cReqAidInfo : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// C->S 客户端请求：请求加持洗炼 msgId:2072
	public class MsgData_cReqUnionBapAid : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// C->S 客户端请求：请求加持升级 msgId:2073
	public class MsgData_cReqAidUpLevel : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// C->S 客户端请求：请求清除上次洗炼数据或保存本次 msgId:2069
	public class MsgData_cReqClearBapAidInfo : MsgData
	{
		public int State;	// 0 清除， 1 保存

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(State);
		}
	}

	// C->S 客户端请求：请求获得公会祈福信息 msgId:2139
	public class MsgData_cReqGetUnionPray : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// C->S 客户端请求：请求公会祈福 msgId:2141
	public class MsgData_cReqUnionPray : MsgData
	{
		public int PrayID;	// 祈福类型

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(PrayID);
		}
	}

	// C->S 请求创建公会同盟 msgId:2064
	public class MsgData_cCreateAlliance : MsgData
	{
		public long GuildId;	// 公会ID

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(GuildId);
		}
	}

	// C->S 请求公会同盟申请列表 msgId:2066
	public class MsgData_cQueryGuildAllianceApplys : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// C->S 请求公会同盟信息 msgId:2067
	public class MsgData_cQueryAllianceGuildInfo : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// C->S 公会同盟列表
	public class MsgData_cReqGuildAllianceApplyVo : MsgData
	{
		public long GuildId;	// 公会ID

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(GuildId);
		}
	}

	// C->S 审核公会同盟申请 msgId:2068
	public class MsgData_cGuildAllianceVerify : MsgData
	{
		public int Verify;	// 是否同意0 - 同意，1 - 拒绝
		public int Size;	// Size
		public List<MsgData_cReqGuildAllianceApplyVo> GuildAllianceApplyList = new List<MsgData_cReqGuildAllianceApplyVo>();	// 公会同盟列表

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(Verify);
			buffer.WriteInt32(Size);
			for (int i = 0; i < Size; i++)
			{
				MsgData_cReqGuildAllianceApplyVo __item = GuildAllianceApplyList[i];
				__item.pack(buffer);
			}
		}
	}

	// S->C 公会数据
	public class MsgData_sRespGuildVo : MsgData
	{
		public long GuildId;	// 公会ID
		public int Rank;	// 排名
		public int Level;	// 公会等级
		public int MemeCnt;	// 成员数量
		public int ExtendNum;	// 扩展人数
		public long Power;	// 战斗力
		public sbyte AppleFlag;	// 申请标识(1-已经申请,0-未申请)
		public byte[] GuildName = new byte[32];	// 公会名称
		public byte[] GuildMasterName = new byte[32];	// 帮主名称

		public override void unpack(NetReadBuffer buffer)
		{
			GuildId = buffer.ReadInt64();
			Rank = buffer.ReadInt32();
			Level = buffer.ReadInt32();
			MemeCnt = buffer.ReadInt32();
			ExtendNum = buffer.ReadInt32();
			Power = buffer.ReadInt64();
			AppleFlag = buffer.ReadInt8();
			buffer.ReadBytes(GuildName);
			buffer.ReadBytes(GuildMasterName);
		}
	}

	// S->C  返回公会列表 msgId:7038
	public class MsgData_sGuildList : MsgData
	{
		public long RecomendGuid;	// 推荐公会ID
		public int Page;	// 页数
		public uint Size;	// 数量
		public List<MsgData_sRespGuildVo> GuildAllianceApplyList = new List<MsgData_sRespGuildVo>();	// 

		public override void unpack(NetReadBuffer buffer)
		{
			RecomendGuid = buffer.ReadInt64();
			Page = buffer.ReadInt32();
			Size = buffer.ReadUInt32();
			GuildAllianceApplyList = new List<MsgData_sRespGuildVo>();
			for (int i = 0; i < Size; i++)
			{
				MsgData_sRespGuildVo __item = new MsgData_sRespGuildVo();
				__item.unpack(buffer);
				GuildAllianceApplyList.Add(__item);
			}
		}
	}

	// S->C 公会资源列表
	public class MsgData_sRespGuildResVo : MsgData
	{
		public int ItemId;	// 资源ID
		public int Count;	// 数量

		public override void unpack(NetReadBuffer buffer)
		{
			ItemId = buffer.ReadInt32();
			Count = buffer.ReadInt32();
		}
	}

	// S->C 公会技能列表
	public class MsgData_sRespGuildSkillVo : MsgData
	{
		public int SkillId;	// 技能ID
		public int OpenFlag;	// 是否开启(1 - 开启， 0 - 未开启)

		public override void unpack(NetReadBuffer buffer)
		{
			SkillId = buffer.ReadInt32();
			OpenFlag = buffer.ReadInt32();
		}
	}

	// S->C 公会信息返回 msgId:7039
	public class MsgData_sQueryMyGuildInfo : MsgData
	{
		public long GuidId;	// 公会ID
		public long AllianceGuildId;	// 同盟公会ID
		public int Rank;	// 排名
		public int Level;	// 公会等级
		public int MemCnt;	// 成员数量
		public int ExtendNum;	// 扩展人数
		public double Captial;	// 公会资金
		public int Liveness;	// 公会活跃度
		public sbyte Pos;	// 职位
		public sbyte Autoagree;	// 0-不自动， 自动档位
		public int Contribution;	// 当前贡献
		public int TotalContribution;	// 累计贡献
		public int Loyalty;	// 忠诚度
		public long Power;	// 战斗力
		public byte[] GuildName = new byte[32];	// 公会名称
		public int ApplyNum;	// 申请数量
		public byte[] GuildMasterName = new byte[32];	// 帮主名称
		public byte[] GuildNotice = new byte[128];	// 帮会公告
		public long RewardTime;	// 领奖剩余时间
		public int AverageLevel;	// 前5名平均等级
		public MsgData_sRespGuildResVo[] GuildResList = new MsgData_sRespGuildResVo[3];	// 帮会资源
		public MsgData_sRespGuildSkillVo[] GuildSkillList = new MsgData_sRespGuildSkillVo[8];	// 帮会技能

		public override void unpack(NetReadBuffer buffer)
		{
			GuidId = buffer.ReadInt64();
			AllianceGuildId = buffer.ReadInt64();
			Rank = buffer.ReadInt32();
			Level = buffer.ReadInt32();
			MemCnt = buffer.ReadInt32();
			ExtendNum = buffer.ReadInt32();
			Captial = buffer.ReadDouble();
			Liveness = buffer.ReadInt32();
			Pos = buffer.ReadInt8();
			Autoagree = buffer.ReadInt8();
			Contribution = buffer.ReadInt32();
			TotalContribution = buffer.ReadInt32();
			Loyalty = buffer.ReadInt32();
			Power = buffer.ReadInt64();
			buffer.ReadBytes(GuildName);
			ApplyNum = buffer.ReadInt32();
			buffer.ReadBytes(GuildMasterName);
			buffer.ReadBytes(GuildNotice);
			RewardTime = buffer.ReadInt64();
			AverageLevel = buffer.ReadInt32();
			for (int i = 0; i < 3; i++)
			{
				MsgData_sRespGuildResVo __item = new MsgData_sRespGuildResVo();
				__item.unpack(buffer);
				GuildResList[i]=__item;
			}
			for (int i = 0; i < 8; i++)
			{
				MsgData_sRespGuildSkillVo __item = new MsgData_sRespGuildSkillVo();
				__item.unpack(buffer);
				GuildSkillList[i]=__item;
			}
		}
	}

	// S->C 工会每日福利，领取奖励 msgID 2956
	public class MsgData_cGuildReward : MsgData
	{

		public override void unpack(NetReadBuffer buffer)
		{
		}
	}

	// S->C 工会每日福利，返回奖励领取结果 msgId:7961
	public class MsgData_sGuildReward : MsgData
	{
		public int result;	// 0成功,1失败

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt32();
		}
	}

	// S->C 公会成员数据
	public class MsgData_sRespGuildMemsVo : MsgData
	{
		public long Gid;	// 角色gid
		public byte[] Name = new byte[32];	// 名称
		public long Time;	// 最后登陆时间
		public long Jointime;	// 加入公会时间
		public int Level;	// 等级
		public int VipLevel;	// VIP等级
		public int Contribute;	// 当前贡献
		public int AllContribute;	// 累积贡献
		public int Loyalty;	// 忠诚度
		public long Power;	// 战斗力
		public sbyte Pos;	// 职位
		public sbyte Online;	// 1-在线，0-不在线
		public int Prof;	// 玩家职业
		public int VFlag;	// V计划
		public int MapID;	// 地图ID
		public int LineId;	// 线
		public int GloryLevel;	// 修为等级
		public int TxhFlag;	// 黄钻标识
		public int TxbFlag;	// 蓝钻标识

		public override void unpack(NetReadBuffer buffer)
		{
			Gid = buffer.ReadInt64();
			buffer.ReadBytes(Name);
			Time = buffer.ReadInt64();
			Jointime = buffer.ReadInt64();
			Level = buffer.ReadInt32();
			VipLevel = buffer.ReadInt32();
			Contribute = buffer.ReadInt32();
			AllContribute = buffer.ReadInt32();
			Loyalty = buffer.ReadInt32();
			Power = buffer.ReadInt64();
			Pos = buffer.ReadInt8();
			Online = buffer.ReadInt8();
			Prof = buffer.ReadInt32();
			VFlag = buffer.ReadInt32();
			MapID = buffer.ReadInt32();
			LineId = buffer.ReadInt32();
			GloryLevel = buffer.ReadInt32();
			TxhFlag = buffer.ReadInt32();
			TxbFlag = buffer.ReadInt32();
		}
	}

	// S->C 返回公会成员列表 msgId:7040
	public class MsgData_sQueryMyGuildMems : MsgData
	{
		public long TimeNow;	// 当前时间
		public int Size;	// 数组大小
		public List<MsgData_sRespGuildMemsVo> GuildMemList = new List<MsgData_sRespGuildMemsVo>();	// 当前时间

		public override void unpack(NetReadBuffer buffer)
		{
			TimeNow = buffer.ReadInt64();
			Size = buffer.ReadInt32();
			GuildMemList = new List<MsgData_sRespGuildMemsVo>();
			for (int i = 0; i < Size; i++)
			{
				MsgData_sRespGuildMemsVo __item = new MsgData_sRespGuildMemsVo();
				__item.unpack(buffer);
				GuildMemList.Add(__item);
			}
		}
	}

	// S->C 公会事件列表
	public class MsgData_sRespGuildEventVo : MsgData
	{
		public int Id;	// 时间ID
		public long Time;	// 时间
		public byte[] Param = new byte[64];	// 参数

		public override void unpack(NetReadBuffer buffer)
		{
			Id = buffer.ReadInt32();
			Time = buffer.ReadInt64();
			buffer.ReadBytes(Param);
		}
	}

	// S->C 返回公会事件 msgId:7041
	public class MsgData_sQueryMyGuildEvent : MsgData
	{
		public int Size;	//  size 
		public List<MsgData_sRespGuildEventVo> GuildEventList = new List<MsgData_sRespGuildEventVo>();	// 数组大小

		public override void unpack(NetReadBuffer buffer)
		{
			Size = buffer.ReadInt32();
			GuildEventList = new List<MsgData_sRespGuildEventVo>();
			for (int i = 0; i < Size; i++)
			{
				MsgData_sRespGuildEventVo __item = new MsgData_sRespGuildEventVo();
				__item.unpack(buffer);
				GuildEventList.Add(__item);
			}
		}
	}

	// S->C 创建公会返回 msgId:7042
	public class MsgData_sCreateGuildRet : MsgData
	{
		public sbyte Result;	//  创建公会返回结果 0- 成功 1-失败 

		public override void unpack(NetReadBuffer buffer)
		{
			Result = buffer.ReadInt8();
		}
	}

	// S->C 更新公会信息 msgId:7043
	public class MsgData_sUpdateGuildInfo : MsgData
	{
		public long GuildId;	// 公会id
		public long AllianceGuildId;	// 同盟公会id
		public int Rank;	// 排名
		public int Level;	// 公会等级
		public int MemCnt;	// 成员数量
		public int ExtendNum;	// 扩展人数
		public double Captial;	// 公会资金
		public int Liveness;	// 公会活跃度
		public long Power;	// 战斗力
		public MsgData_sRespGuildResVo[] GuildResList = new MsgData_sRespGuildResVo[3];	// 帮会资源

		public override void unpack(NetReadBuffer buffer)
		{
			GuildId = buffer.ReadInt64();
			AllianceGuildId = buffer.ReadInt64();
			Rank = buffer.ReadInt32();
			Level = buffer.ReadInt32();
			MemCnt = buffer.ReadInt32();
			ExtendNum = buffer.ReadInt32();
			Captial = buffer.ReadDouble();
			Liveness = buffer.ReadInt32();
			Power = buffer.ReadInt64();
			for (int i = 0; i < 3; i++)
			{
				MsgData_sRespGuildResVo __item = new MsgData_sRespGuildResVo();
				__item.unpack(buffer);
				GuildResList[i]=__item;
			}
		}
	}

	// S->C 更新公会帮主 msgId:7044
	public class MsgData_sUpdateGuildMasterName : MsgData
	{
		public byte[] GuildMasterName = new byte[32];	// 帮主名称

		public override void unpack(NetReadBuffer buffer)
		{
			buffer.ReadBytes(GuildMasterName);
		}
	}

	// S->C 更新公会公告 msgId:7045
	public class MsgData_sUpdateGuildNotice : MsgData
	{
		public byte[] GuildNotice = new byte[128];	// 帮主名称

		public override void unpack(NetReadBuffer buffer)
		{
			buffer.ReadBytes(GuildNotice);
		}
	}

	// S->C 通知公会邀请 msgId:7046
	public class MsgData_sNotifyBeInvitedGuild : MsgData
	{
		public byte[] Name = new byte[32];	// 邀请人名称
		public long InviterId;	// 邀请人ID
		public byte[] GuildName = new byte[32];	// 公会名称

		public override void unpack(NetReadBuffer buffer)
		{
			buffer.ReadBytes(Name);
			InviterId = buffer.ReadInt64();
			buffer.ReadBytes(GuildName);
		}
	}

	// S->C 返回其他公会信息 msgId:7047
	public class MsgData_sQueryOtherGuildInfo : MsgData
	{
		public long GuildId;	// 公会id
		public int Rank;	// 排名
		public int Level;	// 公会等级
		public int MemCnt;	// 成员数量
		public int ExtendNum;	// 扩展人数
		public long Captial;	// 公会资金
		public long Power;	// 战斗力
		public byte[] GuildName = new byte[32];	// 公会名称
		public byte[] GuildMasterName = new byte[32];	// 帮主名称
		public byte[] GuildNotice = new byte[128];	// 公会公告

		public override void unpack(NetReadBuffer buffer)
		{
			GuildId = buffer.ReadInt64();
			Rank = buffer.ReadInt32();
			Level = buffer.ReadInt32();
			MemCnt = buffer.ReadInt32();
			ExtendNum = buffer.ReadInt32();
			Captial = buffer.ReadInt64();
			Power = buffer.ReadInt64();
			buffer.ReadBytes(GuildName);
			buffer.ReadBytes(GuildMasterName);
			buffer.ReadBytes(GuildNotice);
		}
	}

	// S->C 公会申请列表
	public class MsgData_sRespGuildApplysVo : MsgData
	{
		public long Gid;	// 角色ID
		public byte[] Name = new byte[32];	// 名称
		public long Time;	// 申请时间
		public int Level;	// 等级
		public long Power;	// 战斗力

		public override void unpack(NetReadBuffer buffer)
		{
			Gid = buffer.ReadInt64();
			buffer.ReadBytes(Name);
			Time = buffer.ReadInt64();
			Level = buffer.ReadInt32();
			Power = buffer.ReadInt64();
		}
	}

	// S->C 返回公会申请列表 msgId:7048
	public class MsgData_sQueryMyGuildApplys : MsgData
	{
		public int Size;	// 数量
		public List<MsgData_sRespGuildApplysVo> GuildApplysList = new List<MsgData_sRespGuildApplysVo>();	// 公会成员列表

		public override void unpack(NetReadBuffer buffer)
		{
			Size = buffer.ReadInt32();
			GuildApplysList = new List<MsgData_sRespGuildApplysVo>();
			for (int i = 0; i < Size; i++)
			{
				MsgData_sRespGuildApplysVo __item = new MsgData_sRespGuildApplysVo();
				__item.unpack(buffer);
				GuildApplysList.Add(__item);
			}
		}
	}

	// S->C 公会申请人数 msgId:7149
	public class MsgData_sGuildReplyCountTip : MsgData
	{
		public int ReplyNum;	// 公会申请人数

		public override void unpack(NetReadBuffer buffer)
		{
			ReplyNum = buffer.ReadInt32();
		}
	}

	// S->C 申请加入返回公会 msgId:7049
	public class MsgData_sApplyGuild : MsgData
	{
		public long GuildId;	// 公会id
		public sbyte Apply;	// 0-取消， 1-申请
		public sbyte ApplyFlag;	// 申请标识(1-已经申请,0-未申请)

		public override void unpack(NetReadBuffer buffer)
		{
			GuildId = buffer.ReadInt64();
			Apply = buffer.ReadInt8();
			ApplyFlag = buffer.ReadInt8();
		}
	}

	// S->C 公会成员列表
	public class MsgData_sRespGuildApplyVo : MsgData
	{
		public long MemGid;	// 玩家ID
		public sbyte Result;	//  0-成功，1失败

		public override void unpack(NetReadBuffer buffer)
		{
			MemGid = buffer.ReadInt64();
			Result = buffer.ReadInt8();
		}
	}

	// S->C 审核申请返回 msgId:7050
	public class MsgData_sVerifyGuildApply : MsgData
	{
		public int Verify;	// 是否同意0 - 同意，1 - 拒绝
		public int Size;	// 数量
		public List<MsgData_sRespGuildApplyVo> GuildApplyList = new List<MsgData_sRespGuildApplyVo>();	// 公会成员列表

		public override void unpack(NetReadBuffer buffer)
		{
			Verify = buffer.ReadInt32();
			Size = buffer.ReadInt32();
			GuildApplyList = new List<MsgData_sRespGuildApplyVo>();
			for (int i = 0; i < Size; i++)
			{
				MsgData_sRespGuildApplyVo __item = new MsgData_sRespGuildApplyVo();
				__item.unpack(buffer);
				GuildApplyList.Add(__item);
			}
		}
	}

	// S->C 返回退出公会 msgId:7051
	public class MsgData_sQuitGuild : MsgData
	{
		public int Result;	// 0-成功，-1失败

		public override void unpack(NetReadBuffer buffer)
		{
			Result = buffer.ReadInt32();
		}
	}

	// S->C 返回解散公会 msgId:7052
	public class MsgData_sDismissGuild : MsgData
	{
		public int Result;	// 0-成功，-1失败

		public override void unpack(NetReadBuffer buffer)
		{
			Result = buffer.ReadInt32();
		}
	}

	// S->C 返回升级公会 msgId:7053
	public class MsgData_sLevelUpGuild : MsgData
	{
		public int Level;	// 公会等级
		public int Result;	// 0-成功，-1失败

		public override void unpack(NetReadBuffer buffer)
		{
			Level = buffer.ReadInt32();
			Result = buffer.ReadInt32();
		}
	}

	// S->C 返回开启某组公会技能 msgId:7054
	public class MsgData_sLvUpGuildSkill : MsgData
	{
		public int GroupId;	// 技能组ID
		public int Result;	// 0-成功，-1失败

		public override void unpack(NetReadBuffer buffer)
		{
			GroupId = buffer.ReadInt32();
			Result = buffer.ReadInt32();
		}
	}

	// S->C 返回改变职位 msgId:7055
	public class MsgData_sChangeGuildPos : MsgData
	{
		public long MemGid;	// 玩家ID
		public sbyte Pos;	// 职位
		public int Result;	// 0-成功，-1失败

		public override void unpack(NetReadBuffer buffer)
		{
			MemGid = buffer.ReadInt64();
			Pos = buffer.ReadInt8();
			Result = buffer.ReadInt32();
		}
	}

	// S->C 返回踢出公会成员 msgId:7056
	public class MsgData_sKickGuildMemeber : MsgData
	{
		public long MemGid;	// 玩家ID
		public int Result;	// 0-成功，-1失败

		public override void unpack(NetReadBuffer buffer)
		{
			MemGid = buffer.ReadInt64();
			Result = buffer.ReadInt32();
		}
	}

	// S->C 返回禅让帮主 msgId:7057
	public class MsgData_sChangeLeader : MsgData
	{
		public long OldId;	// 老帮主ID
		public long NewId;	// 新帮主ID
		public sbyte Pos;	// 老帮主职位
		public int Result;	// 0-成功，-1失败

		public override void unpack(NetReadBuffer buffer)
		{
			OldId = buffer.ReadInt64();
			NewId = buffer.ReadInt64();
			Pos = buffer.ReadInt8();
			Result = buffer.ReadInt32();
		}
	}

	// S->C 返回公会捐献 msgId:7058
	public class MsgData_sGuildContribute : MsgData
	{
		public double Captial;	// 公会资金
		public int Contribute;	// 当前贡献
		public MsgData_sRespGuildResVo[] GuildResList = new MsgData_sRespGuildResVo[3];	// 公会资源列表

		public override void unpack(NetReadBuffer buffer)
		{
			Captial = buffer.ReadDouble();
			Contribute = buffer.ReadInt32();
			for (int i = 0; i < 3; i++)
			{
				MsgData_sRespGuildResVo __item = new MsgData_sRespGuildResVo();
				__item.unpack(buffer);
				GuildResList[i]=__item;
			}
		}
	}

	// S->C 服务器返回：返回升级自身公会技能 msgId:7059
	public class MsgData_sLevelUpMyGuildSkill : MsgData
	{
		public int Id;	// 技能组ID
		public int Result;	// 0-成功，-1失败

		public override void unpack(NetReadBuffer buffer)
		{
			Id = buffer.ReadInt32();
			Result = buffer.ReadInt32();
		}
	}

	// S->C 服务器返回：返回加持属性 msgId:7071
	public class MsgData_sBackAidInfo : MsgData
	{
		public int AidLevel;	// 加持等级
		public int Att;	// 加持攻击
		public int Def;	// 加持防御
		public int MaxHP;	// 加持生命
		public int SubDef;	// 加持破防

		public override void unpack(NetReadBuffer buffer)
		{
			AidLevel = buffer.ReadInt32();
			Att = buffer.ReadInt32();
			Def = buffer.ReadInt32();
			MaxHP = buffer.ReadInt32();
			SubDef = buffer.ReadInt32();
		}
	}

	// S->C 服务器返回：返回洗炼属性 msgId:7072
	public class MsgData_sBackAidBapInfo : MsgData
	{
		public int Att;	// 加持攻击
		public int Def;	// 加持防御
		public int MaxHP;	// 加持生命
		public int SubDef;	// 加持破防

		public override void unpack(NetReadBuffer buffer)
		{
			Att = buffer.ReadInt32();
			Def = buffer.ReadInt32();
			MaxHP = buffer.ReadInt32();
			SubDef = buffer.ReadInt32();
		}
	}

	// S->C 服务器返回：返回加持升级 msgId:7073
	public class MsgData_sBackAidUpLevelInfo : MsgData
	{
		public int Result;	// 0 成功， 1 失败

		public override void unpack(NetReadBuffer buffer)
		{
			Result = buffer.ReadInt32();
		}
	}

	// S->C 更新自己公会信息 msgId:7074
	public class MsgData_sUpdateMyGuildMemInfo : MsgData
	{
		public sbyte Pos;	// 职位
		public int Con;	// 当前贡献
		public int TotalCon;	// 累计贡献
		public int Loyalty;	// 忠诚度

		public override void unpack(NetReadBuffer buffer)
		{
			Pos = buffer.ReadInt8();
			Con = buffer.ReadInt32();
			TotalCon = buffer.ReadInt32();
			Loyalty = buffer.ReadInt32();
		}
	}

	// S->C 祈福信息
	public class MsgData_sPrayVo : MsgData
	{
		public long Time;	// 祈福时间
		public byte[] Name = new byte[32];	// 名称
		public int PrayID;	// 祈福类型

		public override void unpack(NetReadBuffer buffer)
		{
			Time = buffer.ReadInt64();
			buffer.ReadBytes(Name);
			PrayID = buffer.ReadInt32();
		}
	}

	// S->C 获得公会祈福信息 msgId:7139
	public class MsgData_sGetUnionPray : MsgData
	{
		public int Pray1;	// 普通祈福 0未祈福，1已祈福
		public int Pray2;	// 高级祈福 0未祈福，1已祈福
		public int Pray3;	// 至尊祈福 0未祈福，1已祈福
		public int PrayCount;	// 祈福数量
		public List<MsgData_sPrayVo> PrayList = new List<MsgData_sPrayVo>();	// 祈福列表

		public override void unpack(NetReadBuffer buffer)
		{
			Pray1 = buffer.ReadInt32();
			Pray2 = buffer.ReadInt32();
			Pray3 = buffer.ReadInt32();
			PrayCount = buffer.ReadInt32();
			PrayList = new List<MsgData_sPrayVo>();
			for (int i = 0; i < PrayCount; i++)
			{
				MsgData_sPrayVo __item = new MsgData_sPrayVo();
				__item.unpack(buffer);
				PrayList.Add(__item);
			}
		}
	}

	// S->C 服务器返回：公会祈福结果 msgId:7141
	public class MsgData_sUnionPray : MsgData
	{
		public sbyte Result;	// 0:成功(成功获得),1:失败
		public int PrayID;	// 祈福类型

		public override void unpack(NetReadBuffer buffer)
		{
			Result = buffer.ReadInt8();
			PrayID = buffer.ReadInt32();
		}
	}

	// S->C 返回创建公会同盟 msgId:7066
	public class MsgData_sCreateAlliance : MsgData
	{
		public int GuildId;	// 公会id
		public int Result;	// 0-成功，-1失败

		public override void unpack(NetReadBuffer buffer)
		{
			GuildId = buffer.ReadInt32();
			Result = buffer.ReadInt32();
		}
	}

	// S->C 返回解散公会同盟 msgId:7067
	public class MsgData_sDismissGuildAlliance : MsgData
	{
		public int GuildId;	// 公会id

		public override void unpack(NetReadBuffer buffer)
		{
			GuildId = buffer.ReadInt32();
		}
	}

	// S->C 返回公会同盟申请列表 msgId:7068
	public class MsgData_sRespGuildAllianceApplysVo : MsgData
	{
		public long Id;	// 公会Id
		public byte[] Name = new byte[32];	// 公会名称
		public long Time;	// 申请时间
		public long Power;	// 战斗力
		public int Level;	// 等级
		public int MemCnt;	// 成员数量
		public int ExtendNum;	// 扩展人数

		public override void unpack(NetReadBuffer buffer)
		{
			Id = buffer.ReadInt64();
			buffer.ReadBytes(Name);
			Time = buffer.ReadInt64();
			Power = buffer.ReadInt64();
			Level = buffer.ReadInt32();
			MemCnt = buffer.ReadInt32();
			ExtendNum = buffer.ReadInt32();
		}
	}

	// S->C 返回公会同盟申请列表 msgId:7068
	public class MsgData_sQueryGuildAllianceApplys : MsgData
	{
		public int Size;	// 数组
		public List<MsgData_sRespGuildAllianceApplysVo> GuildAllianceApplysList = new List<MsgData_sRespGuildAllianceApplysVo>();	// 同盟 公会列表

		public override void unpack(NetReadBuffer buffer)
		{
			Size = buffer.ReadInt32();
			GuildAllianceApplysList = new List<MsgData_sRespGuildAllianceApplysVo>();
			for (int i = 0; i < Size; i++)
			{
				MsgData_sRespGuildAllianceApplysVo __item = new MsgData_sRespGuildAllianceApplysVo();
				__item.unpack(buffer);
				GuildAllianceApplysList.Add(__item);
			}
		}
	}

	// S->C 公会成员列表
	public class MsgData_sRespAllianceGuildMemsVo : MsgData
	{
		public int Gid;	// Gid
		public byte[] Name = new byte[32];	// 名称
		public long Time;	// 最后登录时间
		public int Level;	// 等级
		public long Power;	// 战斗力
		public sbyte Pos;	// 职位
		public sbyte Online;	// 1-在线，0-不在线

		public override void unpack(NetReadBuffer buffer)
		{
			Gid = buffer.ReadInt32();
			buffer.ReadBytes(Name);
			Time = buffer.ReadInt64();
			Level = buffer.ReadInt32();
			Power = buffer.ReadInt64();
			Pos = buffer.ReadInt8();
			Online = buffer.ReadInt8();
		}
	}

	// S->C 返回公会同盟信息 msgId:7069
	public class MsgData_sQueryAllianceGuildInfo : MsgData
	{
		public int Rank;	// 排名
		public int Level;	// 公会等级
		public int MemCnt;	// 成员数量
		public long ExtendNum;	// 扩展人数
		public int Power;	// 战斗力
		public byte[] GuildName = new byte[32];	// 公会名称
		public int Size;	// 数量
		public List<MsgData_sRespAllianceGuildMemsVo> GuildMemList = new List<MsgData_sRespAllianceGuildMemsVo>();	// 公会成员列表

		public override void unpack(NetReadBuffer buffer)
		{
			Rank = buffer.ReadInt32();
			Level = buffer.ReadInt32();
			MemCnt = buffer.ReadInt32();
			ExtendNum = buffer.ReadInt64();
			Power = buffer.ReadInt32();
			buffer.ReadBytes(GuildName);
			Size = buffer.ReadInt32();
			GuildMemList = new List<MsgData_sRespAllianceGuildMemsVo>();
			for (int i = 0; i < Size; i++)
			{
				MsgData_sRespAllianceGuildMemsVo __item = new MsgData_sRespAllianceGuildMemsVo();
				__item.unpack(buffer);
				GuildMemList.Add(__item);
			}
		}
	}

	// S->C 公会同盟数据
	public class MsgData_sReqGuildAllianceApplyVo : MsgData
	{
		public long GuildId;	// 公会ID
		public sbyte Result;	// 0-成功，1失败

		public override void unpack(NetReadBuffer buffer)
		{
			GuildId = buffer.ReadInt64();
			Result = buffer.ReadInt8();
		}
	}

	// S->C 审核公会同盟返回 msgId:7070
	public class MsgData_sGuildAllianceVerify : MsgData
	{
		public int Verify;	// 是否同意0 - 同意，1 - 拒绝
		public int Size;	// 公会同盟列表 数量
		public List<MsgData_sReqGuildAllianceApplyVo> GuildAllianceApplyList = new List<MsgData_sReqGuildAllianceApplyVo>();	// 公会同盟列表

		public override void unpack(NetReadBuffer buffer)
		{
			Verify = buffer.ReadInt32();
			Size = buffer.ReadInt32();
			GuildAllianceApplyList = new List<MsgData_sReqGuildAllianceApplyVo>();
			for (int i = 0; i < Size; i++)
			{
				MsgData_sReqGuildAllianceApplyVo __item = new MsgData_sReqGuildAllianceApplyVo();
				__item.unpack(buffer);
				GuildAllianceApplyList.Add(__item);
			}
		}
	}

	// S->C 请求公会战力返回 msgId:7922
	public class MsgData_sGuildMilitary : MsgData
	{
		public long m_i8power;	// 战力数值

		public override void unpack(NetReadBuffer buffer)
		{
			m_i8power = buffer.ReadInt64();
		}
	}

	// C->S 请求复活 msgId:3077
	public class MsgData_cRevive : MsgData
	{
		public int ReviveType;	// 复活类型
		public int MoneyType;	// 物品不足时使用的元宝类型,1元宝,2绑定元宝

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(ReviveType);
			buffer.WriteInt32(MoneyType);
		}
	}

	// S->C 复活信息 msgId:8077
	public class MsgData_sRevive : MsgData
	{
		public int Result;	// 复活结果, 0:成功 -1:5s限制时间 -2:其他错误,人没死等 -3:道具不足 -4:元宝不足 -5:红名不可原地复活
		public long RoleID;	// 角色id
		public int ReviveType;	// 复活类型
		public double PosX;	// 复活位置坐标x
		public double PosY;	// 复活位置坐标y

		public override void unpack(NetReadBuffer buffer)
		{
			Result = buffer.ReadInt32();
			RoleID = buffer.ReadInt64();
			ReviveType = buffer.ReadInt32();
			PosX = buffer.ReadDouble();
			PosY = buffer.ReadDouble();
		}
	}

	// C->S 客户端请求任务列表 msgid:3059
	public class MsgData_cQueryQuest : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// C->S 客户端点击计数(如功能指引任务) msgid:3061
	public class MsgData_cQuestClick : MsgData
	{
		public int questId;	// 任务Id

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(questId);
		}
	}

	// C->S 客户端请求接取任务 msgid:3062
	public class MsgData_cAcceptQuest : MsgData
	{
		public int questId;	// 任务Id

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(questId);
		}
	}

	// C->S 客户端请求放弃任务 msgid:3063
	public class MsgData_cGiveUpQuest : MsgData
	{
		public int questId;	// 任务Id

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(questId);
		}
	}

	// C->S 客户端请求完成任务 msgid:3064
	public class MsgData_cFinishQuest : MsgData
	{
		public int questId;	// 任务Id
		public int multipIndex;	// 1.免费领1倍，2.银两领双倍，3.元宝领三倍

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(questId);
			buffer.WriteInt32(multipIndex);
		}
	}

	// S->C 任务目标列表成员信息
	public class MsgData_sQuestGoalInfo : MsgData
	{
		public int goalId;	// 目标id
		public int curCount;	// 当前任务计数

		public override void unpack(NetReadBuffer buffer)
		{
			goalId = buffer.ReadInt32();
			curCount = buffer.ReadInt32();
		}
	}

	// S->C 任务列表成员信息
	public class MsgData_sQuestInfo : MsgData
	{
		public int id;	// 任务id
		public int state;	// 任务状态 0未接 1进行中 2可交
		public int flag;	// 日环任务时，高16位环数 中8位星级 低8位倍率
		public MsgData_sQuestGoalInfo[] goalsList = new MsgData_sQuestGoalInfo[3];	// 目标列表

		public override void unpack(NetReadBuffer buffer)
		{
			id = buffer.ReadInt32();
			state = buffer.ReadInt32();
			flag = buffer.ReadInt32();
			for (int i = 0; i < 3; i++)
			{
				MsgData_sQuestGoalInfo __item = new MsgData_sQuestGoalInfo();
				__item.unpack(buffer);
				goalsList[i]=__item;
			}
		}
	}

	// S->C 服务端任务列表反馈 msgid:8059
	public class MsgData_sQueryQuestResult : MsgData
	{
		public uint questListCount;	// 表成员个数
		public List<MsgData_sQuestInfo> questList = new List<MsgData_sQuestInfo>();	// 任务列表

		public override void unpack(NetReadBuffer buffer)
		{
			questListCount = buffer.ReadUInt32();
			questList = new List<MsgData_sQuestInfo>();
			for (int i = 0; i < questListCount; i++)
			{
				MsgData_sQuestInfo __item = new MsgData_sQuestInfo();
				__item.unpack(buffer);
				questList.Add(__item);
			}
		}
	}

	// S->C 服务端增加一个任务 msgid:8060
	public class MsgData_sQueryAdd : MsgData
	{
		public uint id;	// 任务id
		public int state;	// 任务状态 0未接 1进行中 2可交
		public int flag;	// 日环任务时，高16位环数 中8位星级 低8位倍率
		public int goalListCount;	// 目标列表成员个数
		public List<MsgData_sQuestGoalInfo> goalsList = new List<MsgData_sQuestGoalInfo>();	// 目标列表

		public override void unpack(NetReadBuffer buffer)
		{
			id = buffer.ReadUInt32();
			state = buffer.ReadInt32();
			flag = buffer.ReadInt32();
			goalListCount = buffer.ReadInt32();
			goalsList = new List<MsgData_sQuestGoalInfo>();
			for (int i = 0; i < goalListCount; i++)
			{
				MsgData_sQuestGoalInfo __item = new MsgData_sQuestGoalInfo();
				__item.unpack(buffer);
				goalsList.Add(__item);
			}
		}
	}

	// S->C 服务端任务更新 msgid:8061
	public class MsgData_sQueryUpdate : MsgData
	{
		public uint id;	// 任务id
		public int state;	// 任务状态 0未接 1进行中 2可交
		public int flag;	// 日环任务时，高16位环数 中8位星级 低8位倍率
		public int goalListCount;	// 目标列表成员个数
		public List<MsgData_sQuestGoalInfo> goalsList = new List<MsgData_sQuestGoalInfo>();	// 目标列表

		public override void unpack(NetReadBuffer buffer)
		{
			id = buffer.ReadUInt32();
			state = buffer.ReadInt32();
			flag = buffer.ReadInt32();
			goalListCount = buffer.ReadInt32();
			goalsList = new List<MsgData_sQuestGoalInfo>();
			for (int i = 0; i < goalListCount; i++)
			{
				MsgData_sQuestGoalInfo __item = new MsgData_sQuestGoalInfo();
				__item.unpack(buffer);
				goalsList.Add(__item);
			}
		}
	}

	// S->C 服务端接受任务反馈 msgid:8062
	public class MsgData_sAcceptQuestResult : MsgData
	{
		public uint result;	// 0:成功

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadUInt32();
		}
	}

	// S->C 服务端放弃任务反馈 msgid:8063
	public class MsgData_sGiveupQuestResult : MsgData
	{
		public uint result;	// 0:成功
		public uint id;	// 任务id

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadUInt32();
			id = buffer.ReadUInt32();
		}
	}

	// S->C 服务端完成任务反馈 msgid:8064
	public class MsgData_sFinishQuestResult : MsgData
	{
		public uint result;	// 0:成功
		public uint id;	// 任务id

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadUInt32();
			id = buffer.ReadUInt32();
		}
	}

	// S->C 服务端任务删除 msgid:8065
	public class MsgData_sQuestDele : MsgData
	{
		public uint id;	// 任务id

		public override void unpack(NetReadBuffer buffer)
		{
			id = buffer.ReadUInt32();
		}
	}

	// S->C 物品信息
	public class MsgData_sItemInfo : MsgData
	{
		public long UID;	// 唯一编号
		public int ID;	// 物品编号
		public int Count;	// 物品数量
		public int Bag;	// 所属背包
		public int Pos;	// 格子位置
		public int UseCount;	// 当前使用次数PS:可能是总使用次数。
		public int TodayUse;	// 当天使用次数
		public long CanUseVal;	// 可使用的值
		public long Flag;	// 标志位 第1位:绑定, 第2位:交易绑定
		public int TimeLimt;	// 倒计时

		public override void unpack(NetReadBuffer buffer)
		{
			UID = buffer.ReadInt64();
			ID = buffer.ReadInt32();
			Count = buffer.ReadInt32();
			Bag = buffer.ReadInt32();
			Pos = buffer.ReadInt32();
			UseCount = buffer.ReadInt32();
			TodayUse = buffer.ReadInt32();
			CanUseVal = buffer.ReadInt64();
			Flag = buffer.ReadInt64();
			TimeLimt = buffer.ReadInt32();
		}
	}

	// S->C 服务端通知: 下发物品列表 msgId:8019
	public class MsgData_sQueryItemResult : MsgData
	{
		public long ID;	// 编号
		public int Bag;	// 背包类型
		public int Size;	// 已开格子数
		public int OpenLastTime;	// 自动开启下一格子剩余时间秒
		public uint ItemCount;	// 物品数量
		public List<MsgData_sItemInfo> ItemInfoList = new List<MsgData_sItemInfo>();	// 物品列表

		public override void unpack(NetReadBuffer buffer)
		{
			ID = buffer.ReadInt64();
			Bag = buffer.ReadInt32();
			Size = buffer.ReadInt32();
			OpenLastTime = buffer.ReadInt32();
			ItemCount = buffer.ReadUInt32();
			ItemInfoList = new List<MsgData_sItemInfo>();
			for (int i = 0; i < ItemCount; i++)
			{
				MsgData_sItemInfo __item = new MsgData_sItemInfo();
				__item.unpack(buffer);
				ItemInfoList.Add(__item);
			}
		}
	}

	// S->C 服务端通知: 整理背包反馈 msgId:8024
	public class MsgData_sPackItemResult : MsgData
	{
		public int Result;	// 整理结果 0成功
		public int BagType;	// 背包类型

		public override void unpack(NetReadBuffer buffer)
		{
			Result = buffer.ReadInt32();
			BagType = buffer.ReadInt32();
		}
	}

	// C->S 客户端请求: 整理背包 msgId:3024
	public class MsgData_cPackItem : MsgData
	{
		public int BagType;	// 背包类型

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(BagType);
		}
	}

	// C->S 客户端请求: 获取物品列表 msgId:3019
	public class MsgData_cQueryItem : MsgData
	{
		public long RoleID;	// 角色编号
		public int BagType;	// 背包类型

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(RoleID);
			buffer.WriteInt32(BagType);
		}
	}

	// S->C 服务端通知: 加物品 msgId:8014
	public class MsgData_sItemAdd : MsgData
	{
		public long UID;	// 唯一编号
		public int ID;	// 物品编号
		public int Count;	// 物品数量
		public int Bag;	// 所属背包
		public int Pos;	// 格子位置
		public int UseCount;	// 当前使用次数PS:可能是总使用次数。
		public int TodayUse;	// 当天使用次数
		public long CanUseVal;	// 可使用的值
		public long Flag;	// 标志位 第1位:绑定, 第2位:交易绑定
		public int TimeLimt;	// 倒计时

		public override void unpack(NetReadBuffer buffer)
		{
			UID = buffer.ReadInt64();
			ID = buffer.ReadInt32();
			Count = buffer.ReadInt32();
			Bag = buffer.ReadInt32();
			Pos = buffer.ReadInt32();
			UseCount = buffer.ReadInt32();
			TodayUse = buffer.ReadInt32();
			CanUseVal = buffer.ReadInt64();
			Flag = buffer.ReadInt64();
			TimeLimt = buffer.ReadInt32();
		}
	}

	// S->C 服务端通知: 删除物品 msgId:8015
	public class MsgData_sItemDel : MsgData
	{
		public int Bag;	// 所属背包
		public int Pos;	// 格子位置

		public override void unpack(NetReadBuffer buffer)
		{
			Bag = buffer.ReadInt32();
			Pos = buffer.ReadInt32();
		}
	}

	// S->C 服务端通知: 更新物品 msgId:8016
	public class MsgData_sItemUpdate : MsgData
	{
		public long UID;	// 唯一编号
		public int ID;	// 物品编号
		public int Count;	// 物品数量
		public int Bag;	// 所属背包
		public int Pos;	// 格子位置
		public int UseCount;	// 当前使用次数PS:可能是总使用次数。
		public int TodayUse;	// 当天使用次数
		public long CanUseVal;	// 可使用的值
		public long Flag;	// 标志位 第1位:绑定, 第2位:交易绑定
		public int TimeLimt;	// 倒计时

		public override void unpack(NetReadBuffer buffer)
		{
			UID = buffer.ReadInt64();
			ID = buffer.ReadInt32();
			Count = buffer.ReadInt32();
			Bag = buffer.ReadInt32();
			Pos = buffer.ReadInt32();
			UseCount = buffer.ReadInt32();
			TodayUse = buffer.ReadInt32();
			CanUseVal = buffer.ReadInt64();
			Flag = buffer.ReadInt64();
			TimeLimt = buffer.ReadInt32();
		}
	}

	// C->S 客户端请求: 丢弃一件物品 msgId:3020
	public class MsgData_cDiscardItem : MsgData
	{
		public int Bag;	// 所属背包
		public int Pos;	// 格子位置

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(Bag);
			buffer.WriteInt32(Pos);
		}
	}

	// S->C 服务端通知: 丢弃物品反馈 msgId:8020
	public class MsgData_sDiscardItemResult : MsgData
	{
		public int Result;	// 结果 0:成功
		public int Bag;	// 所属背包
		public int Pos;	// 格子位置

		public override void unpack(NetReadBuffer buffer)
		{
			Result = buffer.ReadInt32();
			Bag = buffer.ReadInt32();
			Pos = buffer.ReadInt32();
		}
	}

	// S->C 服务端通知: 背包可扩充 msgId:8098
	public class MsgData_sExpandBagTips : MsgData
	{
		public int Bag;	// 背包类型

		public override void unpack(NetReadBuffer buffer)
		{
			Bag = buffer.ReadInt32();
		}
	}

	// S->C 服务端通知: 背包扩充反馈 msgId:8025
	public class MsgData_sExpandBagResult : MsgData
	{
		public int Result;	// 结果 0:成功
		public int Bag;	// 所属背包
		public int NewSize;	// 新格子数

		public override void unpack(NetReadBuffer buffer)
		{
			Result = buffer.ReadInt32();
			Bag = buffer.ReadInt32();
			NewSize = buffer.ReadInt32();
		}
	}

	// C->S 客户端请求: 背包容量扩充 msgId:3025
	public class MsgData_cExpandBag : MsgData
	{
		public int Bag;	// 所属背包
		public int NewSize;	// 新格子数
		public int MoneyType;	// 物品不足时使用的元宝类型,1元宝,2绑定元宝

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(Bag);
			buffer.WriteInt32(NewSize);
			buffer.WriteInt32(MoneyType);
		}
	}

	// S->C 服务端通知: 交换物品反馈 msgId:8021
	public class MsgData_sSwapItemResult : MsgData
	{
		public int Result;	// 结果 0:成功
		public int SrcBag;	// 源背包
		public int DstBag;	// 目标背包
		public int SrcPos;	// 源格子
		public int DstPos;	// 目标格子

		public override void unpack(NetReadBuffer buffer)
		{
			Result = buffer.ReadInt32();
			SrcBag = buffer.ReadInt32();
			DstBag = buffer.ReadInt32();
			SrcPos = buffer.ReadInt32();
			DstPos = buffer.ReadInt32();
		}
	}

	// C->S 客户端请求: 交换两个物品 msgId:3021
	public class MsgData_cSwapItem : MsgData
	{
		public int SrcBag;	// 源背包
		public int DstBag;	// 目标背包
		public int SrcPos;	// 源格子
		public int DstPos;	// 目标格子

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(SrcBag);
			buffer.WriteInt32(DstBag);
			buffer.WriteInt32(SrcPos);
			buffer.WriteInt32(DstPos);
		}
	}

	// S->C 服务端通知: 交一键穿戴魔神装备结果
	public class MsgData_sMoShenItem : MsgData
	{
		public int SrcBag;	// 源背包
		public int DstBag;	// 目标背包
		public int SrcPos;	// 源格子
		public int DstPos;	// 目标格子

		public override void unpack(NetReadBuffer buffer)
		{
			SrcBag = buffer.ReadInt32();
			DstBag = buffer.ReadInt32();
			SrcPos = buffer.ReadInt32();
			DstPos = buffer.ReadInt32();
		}
	}

	// S->C 服务器推送：一键穿戴魔神装备结果 msgId:11181
	public class MsgData_sMoShenItemResult : MsgData
	{
		public int Result;	// 结果 0:成功
		public uint Count;	// 数量
		public List<MsgData_sMoShenItem> ResultList = new List<MsgData_sMoShenItem>();	// 格子位置列表

		public override void unpack(NetReadBuffer buffer)
		{
			Result = buffer.ReadInt32();
			Count = buffer.ReadUInt32();
			ResultList = new List<MsgData_sMoShenItem>();
			for (int i = 0; i < Count; i++)
			{
				MsgData_sMoShenItem __item = new MsgData_sMoShenItem();
				__item.unpack(buffer);
				ResultList.Add(__item);
			}
		}
	}

	// S->C 服务端通知: 出售物品反馈 msgId:8023
	public class MsgData_sSellItemResult : MsgData
	{
		public int Result;	// 结果 0:成功
		public long UID;	// 唯一编号
		public int ID;	// 物品编号
		public int Count;	// 物品数量
		public long Flag;	// 标志位 第1位:绑定, 第2位:交易绑定

		public override void unpack(NetReadBuffer buffer)
		{
			Result = buffer.ReadInt32();
			UID = buffer.ReadInt64();
			ID = buffer.ReadInt32();
			Count = buffer.ReadInt32();
			Flag = buffer.ReadInt64();
		}
	}

	// C->S 客户端请求: 出售背包物品 msgId:3023
	public class MsgData_cSellItem : MsgData
	{
		public int Bag;	// 所属背包
		public int Pos;	// 格子位置

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(Bag);
			buffer.WriteInt32(Pos);
		}
	}

	// S->C 服务端通知: 使用物品反馈 msgId:8022
	public class MsgData_sUseItemResult : MsgData
	{
		public int Result;	// 结果 0:成功
		public int Bag;	// 所属背包
		public int Pos;	// 格子位置
		public int Count;	// 使用数量
		public int ID;	// 物品ID PS:黑人问号???

		public override void unpack(NetReadBuffer buffer)
		{
			Result = buffer.ReadInt32();
			Bag = buffer.ReadInt32();
			Pos = buffer.ReadInt32();
			Count = buffer.ReadInt32();
			ID = buffer.ReadInt32();
		}
	}

	// C->S 客户端请求: 使用一个物品 msgId:3022
	public class MsgData_cUseItem : MsgData
	{
		public int Bag;	// 所属背包
		public int Pos;	// 格子位置
		public int Count;	// 使用数量
		public int XuanZiID;	// 选择的物品Id:礼包中物品ID PS:黑人问号???

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(Bag);
			buffer.WriteInt32(Pos);
			buffer.WriteInt32(Count);
			buffer.WriteInt32(XuanZiID);
		}
	}

	// S->C 服务端通知: 拆分物品反馈 msgId:8026
	public class MsgData_sSplitItemResult : MsgData
	{
		public int Result;	// 结果 0:成功 -1:不可拆分

		public override void unpack(NetReadBuffer buffer)
		{
			Result = buffer.ReadInt32();
		}
	}

	// C->S 客户端请求: 拆分一个物品 msgId:3026
	public class MsgData_cSplitItem : MsgData
	{
		public int Bag;	// 所属背包
		public int Pos;	// 格子位置
		public int Count;	// 拆分数量

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(Bag);
			buffer.WriteInt32(Pos);
			buffer.WriteInt32(Count);
		}
	}

	// C->S 客户端请求: 批量出售背包物品 msgId:4931
	public class MsgData_cSellItemList : MsgData
	{
		public int Bag;	// 所属背包
		public int Count;	// 出售数量
		public List<int> PosList = new List<int>();	// 格子位置列表

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(Bag);
			buffer.WriteInt32(Count);
			for (int i = 0; i < Count; i++)
			{
				buffer.WriteInt32(PosList[i]);
			}
		}
	}

	// S->C 服务端通知: 批量出售物品反馈 msgId:9931
	public class MsgData_sSellItemListResult : MsgData
	{
		public int Count;	// 出售数量
		public List<MsgData_sSellItemResult> ResultList = new List<MsgData_sSellItemResult>();	// 格子位置列表

		public override void unpack(NetReadBuffer buffer)
		{
			Count = buffer.ReadInt32();
			ResultList = new List<MsgData_sSellItemResult>();
			for (int i = 0; i < Count; i++)
			{
				MsgData_sSellItemResult __item = new MsgData_sSellItemResult();
				__item.unpack(buffer);
				ResultList.Add(__item);
			}
		}
	}

	// C->S 客户端请求: 请求传送 msgId:3200
	public class MsgData_cTeleport : MsgData
	{
		public int Type;	// 1世界地图传送 2日环传送 3剧情传送 4悬赏传送 5世界boss传送 6主线传送 7远距离主线任务免费传送(根据任务表配的teleportMap地图id)
		public int MapID;	// 地图id
		public int X;	// 地图X坐标
		public int Y;	// 地图Y坐标

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(Type);
			buffer.WriteInt32(MapID);
			buffer.WriteInt32(X);
			buffer.WriteInt32(Y);
		}
	}

	// S->C 服务端通知: 返回传送结果 msgId:8200
	public class MsgData_sTeleportResult : MsgData
	{
		public sbyte Type;	// 1世界地图传送 2日环传送 3剧情传送 4悬赏传送 5世界boss传送 6主线传送 7远距离主线任务免费传送(根据任务表配的teleportMap地图id)
		public sbyte Result;	// 结果 0:成功,1:ID错误 2:当前场景错误 3:目标地图错误 4:等级 5:PKing 6:同地图 7:钱不够 8:修为等级 9:待定义

		public override void unpack(NetReadBuffer buffer)
		{
			Type = buffer.ReadInt8();
			Result = buffer.ReadInt8();
		}
	}

	// C->S 客户端请求: 请求技能列表 msgId:3028
	public class MsgData_cSkillList : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// C->S 客户端请求: 请求学习技能 msgId:3029
	public class MsgData_cSkillLearn : MsgData
	{
		public int ID;	// 技能id

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(ID);
		}
	}

	// C->S 客户端请求: 请求升级技能 msgId:3030
	public class MsgData_cSkillLevelUp : MsgData
	{
		public int ID;	// 技能id

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(ID);
		}
	}

	// C->S 客户端请求: 请求一键升级技能 msgId:4960
	public class MsgData_cSkillFastLevelUp : MsgData
	{
		public int ID;	// 技能id

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(ID);
		}
	}

	// S->C 技能信息
	public class MsgData_sSkillInfo : MsgData
	{
		public int ID;	// 技能编号

		public override void unpack(NetReadBuffer buffer)
		{
			ID = buffer.ReadInt32();
		}
	}

	// S->C 服务端通知: 返回技能列表 msgId:8028
	public class MsgData_sSkillListResult : MsgData
	{
		public uint Count;	// 技能数量
		public List<MsgData_sSkillInfo> Skills = new List<MsgData_sSkillInfo>();	// 技能信息列表

		public override void unpack(NetReadBuffer buffer)
		{
			Count = buffer.ReadUInt32();
			Skills = new List<MsgData_sSkillInfo>();
			for (int i = 0; i < Count; i++)
			{
				MsgData_sSkillInfo __item = new MsgData_sSkillInfo();
				__item.unpack(buffer);
				Skills.Add(__item);
			}
		}
	}

	// S->C 服务端通知: 返回学习技能 msgId:8029
	public class MsgData_sSkillLearnResult : MsgData
	{
		public int Result;	// 反馈结果,0成功,1失败
		public int ID;	// 技能id

		public override void unpack(NetReadBuffer buffer)
		{
			Result = buffer.ReadInt32();
			ID = buffer.ReadInt32();
		}
	}

	// S->C 服务端通知: 返回升级技能 msgId:8031
	public class MsgData_sSkillLevelUp : MsgData
	{
		public int Result;	// 反馈结果,0成功,1失败
		public int OldID;	// 学习前技能id
		public int NewID;	// 学习后技能id

		public override void unpack(NetReadBuffer buffer)
		{
			Result = buffer.ReadInt32();
			OldID = buffer.ReadInt32();
			NewID = buffer.ReadInt32();
		}
	}

	// S->C 服务端通知: 增加一个技能(特殊技能，如武魂技能) msgId:8033
	public class MsgData_sSkillAdd : MsgData
	{
		public int ID;	// 技能id

		public override void unpack(NetReadBuffer buffer)
		{
			ID = buffer.ReadInt32();
		}
	}

	// S->C 服务端通知: 删除一个技能(特殊技能，如武魂技能) msgId:8034
	public class MsgData_sSkillRemove : MsgData
	{
		public int ID;	// 技能id

		public override void unpack(NetReadBuffer buffer)
		{
			ID = buffer.ReadInt32();
		}
	}

	// S->C 技能list
	public class MsgData_sSkillCDListVO : MsgData
	{
		public int skillID;	// 技能ID
		public int cdTime;	// 冷却时间

		public override void unpack(NetReadBuffer buffer)
		{
			skillID = buffer.ReadInt32();
			cdTime = buffer.ReadInt32();
		}
	}

	// S->C 登录同步技能CD msgId:8451
	public class MsgData_sSkillCDList : MsgData
	{
		public uint count;	// 数量
		public List<MsgData_sSkillCDListVO> items = new List<MsgData_sSkillCDListVO>();	// 技能list

		public override void unpack(NetReadBuffer buffer)
		{
			count = buffer.ReadUInt32();
			items = new List<MsgData_sSkillCDListVO>();
			for (int i = 0; i < count; i++)
			{
				MsgData_sSkillCDListVO __item = new MsgData_sSkillCDListVO();
				__item.unpack(buffer);
				items.Add(__item);
			}
		}
	}

	// C->S 拾取 msgId:3076
	public class MsgData_cPickUpItem : MsgData
	{
		public uint DataSize;	// 数据长度
		public List<long> IDs = new List<long>();	// 物品ID

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteUInt32(DataSize);
			for (int i = 0; i < DataSize; i++)
			{
				buffer.WriteInt64(IDs[i]);
			}
		}
	}

	// S->C 拾取结果 msgId:8076
	public class PickUpItem_Result : MsgData
	{
		public int Result;	// 结果 0:成功 -1:物品不存在 -2:不属于该玩家 -3:背包已满;
		public long ID;	// ID

		public override void unpack(NetReadBuffer buffer)
		{
			Result = buffer.ReadInt32();
			ID = buffer.ReadInt64();
		}
	}

	// S->C 拾取结果 msgId:8076
	public class MsgData_sPickUpItem : MsgData
	{
		public uint DataSize;	// 数据长度
		public List<PickUpItem_Result> Data = new List<PickUpItem_Result>();	// 返回结果

		public override void unpack(NetReadBuffer buffer)
		{
			DataSize = buffer.ReadUInt32();
			Data = new List<PickUpItem_Result>();
			for (int i = 0; i < DataSize; i++)
			{
				PickUpItem_Result __item = new PickUpItem_Result();
				__item.unpack(buffer);
				Data.Add(__item);
			}
		}
	}

	// C->S 客户端请求：装备位升级 msgId:3690
	public class MsgData_cEquipPosLevelUp : MsgData
	{
		public int pos;	// 装备位
		public int keepLvl;	// 是否使用升星符：1使用 0不用
		public int autobuy;	// 自动购买:0自动,非零不自动

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(pos);
			buffer.WriteInt32(keepLvl);
			buffer.WriteInt32(autobuy);
		}
	}

	// C->S 客户端请求：装备位升星 msgId:4955
	public class MsgData_cEquipPosStarLevelUp : MsgData
	{
		public int pos;	// 装备位
		public int autobuy;	// 自动购买:0自动,非零不自动

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(pos);
			buffer.WriteInt32(autobuy);
		}
	}

	// S->C 服务器返回:装备位升级 msgId:8690
	public class MsgData_sEquipPosLevelUp : MsgData
	{
		public sbyte result;	// 0-强化成功, 1-失败; 2升星成功，3VIP打造成功 4狂化成功
		public int pos;	// 装备位
		public int level;	// 等级
		public int exp;	// 经验
		public int starLevel;	// 星级
		public int vip1;	// 钻石VIP打造 ，1已打造，0未打造
		public int vip2;	// 王者VIP打造 ，1已打造，0未打造
		public int vip3;	// 至尊VIP打造 ，1已打造，0未打造
		public int madness;	// 0未狂化， 1已狂化

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt8();
			pos = buffer.ReadInt32();
			level = buffer.ReadInt32();
			exp = buffer.ReadInt32();
			starLevel = buffer.ReadInt32();
			vip1 = buffer.ReadInt32();
			vip2 = buffer.ReadInt32();
			vip3 = buffer.ReadInt32();
			madness = buffer.ReadInt32();
		}
	}

	// S->C 单个装备信息
	public class MsgData_sEquipData : MsgData
	{
		public int pos;	// 装备位
		public int level;	// 等级
		public int exp;	// 经验
		public int starLevel;	// 星级
		public int vip1;	// 钻石VIP打造 ，1已打造，0未打造
		public int vip2;	// 王者VIP打造 ，1已打造，0未打造
		public int vip3;	// 至尊VIP打造 ，1已打造，0未打造
		public int madness;	// 0未狂化， 1已狂化

		public override void unpack(NetReadBuffer buffer)
		{
			pos = buffer.ReadInt32();
			level = buffer.ReadInt32();
			exp = buffer.ReadInt32();
			starLevel = buffer.ReadInt32();
			vip1 = buffer.ReadInt32();
			vip2 = buffer.ReadInt32();
			vip3 = buffer.ReadInt32();
			madness = buffer.ReadInt32();
		}
	}

	// S->C 服务器返回:装备位信息 msgId:8691
	public class MsgData_sEquipPosInfo : MsgData
	{
		public int selectstar;	// 当前选中星级
		public uint count;	// 数量
		public List<MsgData_sEquipData> items = new List<MsgData_sEquipData>();	// 装备位列表

		public override void unpack(NetReadBuffer buffer)
		{
			selectstar = buffer.ReadInt32();
			count = buffer.ReadUInt32();
			items = new List<MsgData_sEquipData>();
			for (int i = 0; i < count; i++)
			{
				MsgData_sEquipData __item = new MsgData_sEquipData();
				__item.unpack(buffer);
				items.Add(__item);
			}
		}
	}

	// C->S 客户端请求：选择装备位星级 msgId:4986
	public class MsgData_cEquipPosStarSelect : MsgData
	{
		public int star;	// 星级

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(star);
		}
	}

	// S->C 服务器返回:选择装备位星级 msgId:11011
	public class MsgData_sEquipPosStarSelect : MsgData
	{
		public sbyte result;	// 返回结束 0 成功 1失败
		public int star;	// 星级

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt8();
			star = buffer.ReadInt32();
		}
	}

	// S->C 返回装备位信息 msgId:8692;
	public class MsgData_sOtherEquipPos : MsgData
	{
		public int serverType;	// 是否全服排行信息 1=是 其余都不是
		public int type;	// 1== 详细信息，2 == 排行榜详细信息
		public long roleID;	// 角色ID
		public uint count;	// 数量
		public List<MsgData_sEquipData> items = new List<MsgData_sEquipData>();	// 装备位列表

		public override void unpack(NetReadBuffer buffer)
		{
			serverType = buffer.ReadInt32();
			type = buffer.ReadInt32();
			roleID = buffer.ReadInt64();
			count = buffer.ReadUInt32();
			items = new List<MsgData_sEquipData>();
			for (int i = 0; i < count; i++)
			{
				MsgData_sEquipData __item = new MsgData_sEquipData();
				__item.unpack(buffer);
				items.Add(__item);
			}
		}
	}

	// C->S 客户端请求：穿戴时装 msgId:3226
	public class MsgData_cDressFashion : MsgData
	{
		public int ID;	// 时装id
		public int OP;	// 操作类型 1:穿 0:脱

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(ID);
			buffer.WriteInt32(OP);
		}
	}

	// S->C 服务器返回：返回穿戴时装结果 msgId:8226;
	public class MsgData_sDressFashion : MsgData
	{
		public int Result;	// 结果 0 - 成功，1 - 失败
		public int ID;	// 时装id
		public int OP;	// 操作类型 1:穿 0:脱

		public override void unpack(NetReadBuffer buffer)
		{
			Result = buffer.ReadInt32();
			ID = buffer.ReadInt32();
			OP = buffer.ReadInt32();
		}
	}

	// S->C 时装信息
	public class MsgData_sFashionVO : MsgData
	{
		public int ID;	// 时装id
		public int Time;	// 时装剩余时间

		public override void unpack(NetReadBuffer buffer)
		{
			ID = buffer.ReadInt32();
			Time = buffer.ReadInt32();
		}
	}

	// S->C 服务器返回:返回时装列表 msgId:8225
	public class MsgData_sFashionsInfo : MsgData
	{
		public uint Count;	// 数量
		public List<MsgData_sFashionVO> Fashions = new List<MsgData_sFashionVO>();	// 时装列表

		public override void unpack(NetReadBuffer buffer)
		{
			Count = buffer.ReadUInt32();
			Fashions = new List<MsgData_sFashionVO>();
			for (int i = 0; i < Count; i++)
			{
				MsgData_sFashionVO __item = new MsgData_sFashionVO();
				__item.unpack(buffer);
				Fashions.Add(__item);
			}
		}
	}

	// C->S 客户端请求：设置时装状态 msgId:4938
	public class MsgData_cUpdateFashionState : MsgData
	{
		public int State;	// 0 显示 1隐藏

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(State);
		}
	}

	// C->S 客户端请求:触发静物 msgId:3070;
	public class MsgData_cStructDef : MsgData
	{
		public long cID;	// 静物id

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(cID);
		}
	}

	// S->C 服务端通知:触发静物反馈 msgId:8070;
	public class MsgData_sStructDefResult : MsgData
	{
		public int result;	// 范围结果，0成功，1失败，2等级不足， 3境界不足，5其他人正在采集该物品，6本次活动已用完
		public long cID;	// 静物id

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt32();
			cID = buffer.ReadInt64();
		}
	}

	// C->S 等待分解的装备
	public class MsgData_cEquipData : MsgData
	{
		public long guid;	// 装备guid

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(guid);
		}
	}

	// C->S 请求分解装备 msgId:3388
	public class MsgData_cEquipDecompose : MsgData
	{
		public uint count;	// 数量
		public List<MsgData_cEquipData> items = new List<MsgData_cEquipData>();	// 等待分解的装备list

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteUInt32(count);
			for (int i = 0; i < count; i++)
			{
				MsgData_cEquipData __item = items[i];
				__item.pack(buffer);
			}
		}
	}

	// S->C 分解的装备碎片
	public class MsgData_sEquipChip : MsgData
	{
		public int num;	// 碎片数量
		public int cid;	// 物品id

		public override void unpack(NetReadBuffer buffer)
		{
			num = buffer.ReadInt32();
			cid = buffer.ReadInt32();
		}
	}

	// S->C 返回分解结果 msgId:8388;
	public class MsgData_sEquipDecompose : MsgData
	{
		public int result;	// 结果  0:成功
		public uint count;	// 数量
		public List<MsgData_sEquipChip> items = new List<MsgData_sEquipChip>();	// 分解的碎片list

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt32();
			count = buffer.ReadUInt32();
			items = new List<MsgData_sEquipChip>();
			for (int i = 0; i < count; i++)
			{
				MsgData_sEquipChip __item = new MsgData_sEquipChip();
				__item.unpack(buffer);
				items.Add(__item);
			}
		}
	}

	// S->C 获得失去提示物品
	public class MsgData_sItemTipsVO : MsgData
	{
		public sbyte type;	// 类型,1:获得,0失去
		public int id;	// 物品id
		public int count;	// 物品数量

		public override void unpack(NetReadBuffer buffer)
		{
			type = buffer.ReadInt8();
			id = buffer.ReadInt32();
			count = buffer.ReadInt32();
		}
	}

	// S->C 物品获得失去提示 msgId:8142;
	public class MsgData_sItemTips : MsgData
	{
		public uint count;	// 数量
		public List<MsgData_sItemTipsVO> items = new List<MsgData_sItemTipsVO>();	// 获得失去提示物品

		public override void unpack(NetReadBuffer buffer)
		{
			count = buffer.ReadUInt32();
			items = new List<MsgData_sItemTipsVO>();
			for (int i = 0; i < count; i++)
			{
				MsgData_sItemTipsVO __item = new MsgData_sItemTipsVO();
				__item.unpack(buffer);
				items.Add(__item);
			}
		}
	}

	// S->C 卓越属性列表
	public class MsgData_sSuperVO : MsgData
	{
		public long uid;	// 属性uid
		public int id;	// 卓越id
		public int val;	// 值1

		public override void unpack(NetReadBuffer buffer)
		{
			uid = buffer.ReadInt64();
			id = buffer.ReadInt32();
			val = buffer.ReadInt32();
		}
	}

	// S->C 新卓越属性列表;
	public class MsgData_sNewSuperVO : MsgData
	{
		public int id;	// 新卓越id;
		public int wash;	// // 洗练值;

		public override void unpack(NetReadBuffer buffer)
		{
			id = buffer.ReadInt32();
			wash = buffer.ReadInt32();
		}
	}

	// S->C 返回添加装备附加信息 msgId:8149
	public class MsgData_sEquipAdd : MsgData
	{
		public long uid;	// 装备cid;
		public int strenLvl;	// 强化等级;
		public int strenVal;	// 强化值
		public int attrAddLvl;	// 追加属性等级;
		public int groupId;	// 套装id;
		public int groupId2;	// 套装id2;
		public int groupId2Bind;	//  套装id2绑定状态0 未绑定，1 已绑定
		public int group2Level;	// 套装等级
		public int superNum;	// 卓越数量
		public int isyuangu;	// 是否远古
		public int godLevel;	// 神化等级
		public int blessLevel;	// 神佑等级
		public int grades;	// 品阶
		public MsgData_sSuperVO[] Superitems = new MsgData_sSuperVO[7];	// 卓越属性列表
		public MsgData_sNewSuperVO[] NewSuperitems = new MsgData_sNewSuperVO[6];	// 新卓越属性列表

		public override void unpack(NetReadBuffer buffer)
		{
			uid = buffer.ReadInt64();
			strenLvl = buffer.ReadInt32();
			strenVal = buffer.ReadInt32();
			attrAddLvl = buffer.ReadInt32();
			groupId = buffer.ReadInt32();
			groupId2 = buffer.ReadInt32();
			groupId2Bind = buffer.ReadInt32();
			group2Level = buffer.ReadInt32();
			superNum = buffer.ReadInt32();
			isyuangu = buffer.ReadInt32();
			godLevel = buffer.ReadInt32();
			blessLevel = buffer.ReadInt32();
			grades = buffer.ReadInt32();
			for (int i = 0; i < 7; i++)
			{
				MsgData_sSuperVO __item = new MsgData_sSuperVO();
				__item.unpack(buffer);
				Superitems[i]=__item;
			}
			for (int i = 0; i < 6; i++)
			{
				MsgData_sNewSuperVO __item = new MsgData_sNewSuperVO();
				__item.unpack(buffer);
				NewSuperitems[i]=__item;
			}
		}
	}

	// S->C 装备list
	public class MsgData_sEquipSuperListVO : MsgData
	{
		public long uid;	// 装备cid
		public int superNum;	// 卓越数量
		public MsgData_sSuperVO[] items = new MsgData_sSuperVO[7];	// 卓越属性列表

		public override void unpack(NetReadBuffer buffer)
		{
			uid = buffer.ReadInt64();
			superNum = buffer.ReadInt32();
			for (int i = 0; i < 7; i++)
			{
				MsgData_sSuperVO __item = new MsgData_sSuperVO();
				__item.unpack(buffer);
				items[i]=__item;
			}
		}
	}

	// S->C 返回装备卓越信息 msgId:8239
	public class MsgData_sEquipSuper : MsgData
	{
		public uint count;	// 数量
		public List<MsgData_sEquipSuperListVO> items = new List<MsgData_sEquipSuperListVO>();	// 装备list

		public override void unpack(NetReadBuffer buffer)
		{
			count = buffer.ReadUInt32();
			items = new List<MsgData_sEquipSuperListVO>();
			for (int i = 0; i < count; i++)
			{
				MsgData_sEquipSuperListVO __item = new MsgData_sEquipSuperListVO();
				__item.unpack(buffer);
				items.Add(__item);
			}
		}
	}

	// S->C 列表;
	public class MsgData_sEquipExtraVO : MsgData
	{
		public long uid;	// 装备cid
		public int level;	// 追加属性等级

		public override void unpack(NetReadBuffer buffer)
		{
			uid = buffer.ReadInt64();
			level = buffer.ReadInt32();
		}
	}

	// S->C 返回装备追加属性信息 msgId:8246;
	public class MsgData_sEquipExtra : MsgData
	{
		public uint count;	// 数量
		public List<MsgData_sEquipExtraVO> items = new List<MsgData_sEquipExtraVO>();	// 装备list

		public override void unpack(NetReadBuffer buffer)
		{
			count = buffer.ReadUInt32();
			items = new List<MsgData_sEquipExtraVO>();
			for (int i = 0; i < count; i++)
			{
				MsgData_sEquipExtraVO __item = new MsgData_sEquipExtraVO();
				__item.unpack(buffer);
				items.Add(__item);
			}
		}
	}

	// S->C 装备list
	public class MsgData_sEquipNewSuperListVO : MsgData
	{
		public long uid;	// 装备cid
		public MsgData_sNewSuperVO[] items = new MsgData_sNewSuperVO[6];	// 卓越属性列表

		public override void unpack(NetReadBuffer buffer)
		{
			uid = buffer.ReadInt64();
			for (int i = 0; i < 6; i++)
			{
				MsgData_sNewSuperVO __item = new MsgData_sNewSuperVO();
				__item.unpack(buffer);
				items[i]=__item;
			}
		}
	}

	// S->C 返回装备新卓越信息 msgId:8447
	public class MsgData_sEquipNewSuper : MsgData
	{
		public uint count;	// 数量
		public List<MsgData_sEquipNewSuperListVO> items = new List<MsgData_sEquipNewSuperListVO>();	// 新卓越属性列表;

		public override void unpack(NetReadBuffer buffer)
		{
			count = buffer.ReadUInt32();
			items = new List<MsgData_sEquipNewSuperListVO>();
			for (int i = 0; i < count; i++)
			{
				MsgData_sEquipNewSuperListVO __item = new MsgData_sEquipNewSuperListVO();
				__item.unpack(buffer);
				items.Add(__item);
			}
		}
	}

	// S->C  装备附加信息 装备list;
	public class MsgData_sItemEquipVO : MsgData
	{
		public long uid;	// 装备cid;
		public int strenLvl;	// 强化等级;
		public int strenVal;	// 强化值
		public int groupId;	// 套装id;
		public int groupId2;	// 套装id2;
		public int groupId2Bind;	//  套装id2绑定状态0 未绑定，1 已绑定
		public int group2Level;	// 套装等级
		public int godLevel;	// 神化等级
		public int blessLevel;	// 神佑等级

		public override void unpack(NetReadBuffer buffer)
		{
			uid = buffer.ReadInt64();
			strenLvl = buffer.ReadInt32();
			strenVal = buffer.ReadInt32();
			groupId = buffer.ReadInt32();
			groupId2 = buffer.ReadInt32();
			groupId2Bind = buffer.ReadInt32();
			group2Level = buffer.ReadInt32();
			godLevel = buffer.ReadInt32();
			blessLevel = buffer.ReadInt32();
		}
	}

	// S->C  返回装备附加信息 msgId:8131
	public class MsgData_sEquipInfo : MsgData
	{
		public uint count;	// 数量
		public List<MsgData_sItemEquipVO> items = new List<MsgData_sItemEquipVO>();	// 装备附加信息 装备list

		public override void unpack(NetReadBuffer buffer)
		{
			count = buffer.ReadUInt32();
			items = new List<MsgData_sItemEquipVO>();
			for (int i = 0; i < count; i++)
			{
				MsgData_sItemEquipVO __item = new MsgData_sItemEquipVO();
				__item.unpack(buffer);
				items.Add(__item);
			}
		}
	}

	// C->S 客户端请求:灵兽墓地信息 msgId:3743;
	public class MsgData_cHunLingXianYuInfo : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// C->S 客户端请求:重置灵兽墓地 msgId:3744;
	public class MsgData_cResetHunLingXianYu : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// C->S 客户端请求:挑战灵兽墓地 msgId:3745;
	public class MsgData_cChallHunLingXianYu : MsgData
	{
		public int layer;	// 挑战层数

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(layer);
		}
	}

	// C->S 客户端请求:扫荡灵兽墓地 msgId:3747;
	public class MsgData_cCanHunLingXianYu : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// C->S 客户端请求:退出灵兽墓地 msgId:3748;
	public class MsgData_cHunLingXianYuQuit : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// C->S 客户端请求:获得奖励 msgId:3749;
	public class MsgData_cLicaiInfoGetAward : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// S->C 服务器通知:灵兽墓地信息 msgId:8743;
	public class MsgData_sHunLingXianYuInfo : MsgData
	{
		public int finishLayer;	// 已通关的层数
		public int mymaxlayer;	// 我的最好层数
		public int resetNum;	// 重置次数
		public byte[] firstRoleName = new byte[32];	// 第一名角色名字
		public int firsttheadId;	// 头像id
		public int firstLayer;	// 第一名最高层数

		public override void unpack(NetReadBuffer buffer)
		{
			finishLayer = buffer.ReadInt32();
			mymaxlayer = buffer.ReadInt32();
			resetNum = buffer.ReadInt32();
			buffer.ReadBytes(firstRoleName);
			firsttheadId = buffer.ReadInt32();
			firstLayer = buffer.ReadInt32();
		}
	}

	// S->C 返回重置灵兽墓地结果 msgId:8744;
	public class MsgData_sResetHunLingXianYu : MsgData
	{
		public sbyte result;	// 0成功，1失败
		public int finishLayer;	// 已通关的层数
		public int resetNum;	// 重置次数

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt8();
			finishLayer = buffer.ReadInt32();
			resetNum = buffer.ReadInt32();
		}
	}

	// S->C 返回进入灵兽墓地结果 msgId:8745;
	public class MsgData_sChallHunLingXianYu : MsgData
	{
		public sbyte result;	// 0成功，1失败
		public int mLayer;	// 挑战层数

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt8();
			mLayer = buffer.ReadInt32();
		}
	}

	// S->C 挑战灵兽墓地结果 msgId:8746;
	public class MsgData_sChallHunLingXianYuResult : MsgData
	{
		public sbyte result;	// 0成功，1失败

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt8();
		}
	}

	// S->C 扫荡灵兽墓地结果 msgId:8747;
	public class MsgData_sCanHunLingXianYuResult : MsgData
	{
		public sbyte result;	// 0成功，1失败

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt8();
		}
	}

	// S->C 退出灵兽墓地结果 msgId:8748;
	public class MsgData_sHunLingXianYuQuit : MsgData
	{
		public sbyte result;	// 0成功，1失败

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt8();
		}
	}

	// S->C 获得灵兽墓地结果 msgId:8749;
	public class MsgData_sHunLingXianYuGetAward : MsgData
	{
		public sbyte result;	// 0成功，1失败

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt8();
		}
	}

	// S->C 灵兽墓地怪物波数信息 msgId:8750;
	public class MsgData_sHunLingXianYuMonsterInfo : MsgData
	{
		public int index;	// 怪物波数
		public int monId;	// 怪物Id
		public int monNum;	// 怪物个数

		public override void unpack(NetReadBuffer buffer)
		{
			index = buffer.ReadInt32();
			monId = buffer.ReadInt32();
			monNum = buffer.ReadInt32();
		}
	}

	// C->S 客户端请求:灵兽墓地信息 msgId:3421;
	public class MsgData_cLingShouMuDiInfo : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// C->S 客户端请求:重置灵兽墓地 msgId:3422;
	public class MsgData_cResetLingShouMuDi : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// C->S 客户端请求:挑战灵兽墓地 msgId:3423;
	public class MsgData_cChallLingShouMuDi : MsgData
	{
		public int layer;	// 挑战层数

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(layer);
		}
	}

	// C->S 客户端请求:扫荡灵兽墓地 msgId:3424;
	public class MsgData_cCanLingShouMuDi : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// C->S 客户端请求:获得奖励 msgId:3425;
	public class MsgData_cLingShouMuDiGetAward : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// C->S 客户端请求:退出灵兽墓地 msgId:3432;
	public class MsgData_cLingShouMuDiQuit : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// S->C  灵兽墓地信息前3排名;
	public class MsgData_sLingShouMuDiInfoTop : MsgData
	{
		public byte[] name = new byte[32];	// 角色名字;
		public int maxLayer;	// 最高层数
		public int prof;	// 职业
		public int totalTime;	// 用时(秒)

		public override void unpack(NetReadBuffer buffer)
		{
			buffer.ReadBytes(name);
			maxLayer = buffer.ReadInt32();
			prof = buffer.ReadInt32();
			totalTime = buffer.ReadInt32();
		}
	}

	// S->C 服务器通知:灵兽墓地信息 msgId:8421;
	public class MsgData_sLingShouMuDiInfo : MsgData
	{
		public int finishLayer;	// 已通关的层数
		public int mymaxlayer;	// 我的最好层数
		public int resetNum;	// 重置次数
		public MsgData_sLingShouMuDiInfoTop[] topList = new MsgData_sLingShouMuDiInfoTop[3];	// 前3排名信息

		public override void unpack(NetReadBuffer buffer)
		{
			finishLayer = buffer.ReadInt32();
			mymaxlayer = buffer.ReadInt32();
			resetNum = buffer.ReadInt32();
			for (int i = 0; i < 3; i++)
			{
				MsgData_sLingShouMuDiInfoTop __item = new MsgData_sLingShouMuDiInfoTop();
				__item.unpack(buffer);
				topList[i]=__item;
			}
		}
	}

	// S->C 返回重置灵兽墓地结果 msgId:8423;
	public class MsgData_sResetLingShouMuDi : MsgData
	{
		public sbyte result;	// 0成功，1失败
		public int finishLayer;	// 已通关的层数
		public int resetNum;	// 重置次数

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt8();
			finishLayer = buffer.ReadInt32();
			resetNum = buffer.ReadInt32();
		}
	}

	// S->C 返回进入灵兽墓地结果 msgId:8422;
	public class MsgData_sChallLingShouMuDi : MsgData
	{
		public sbyte result;	// 0成功，1失败
		public int mLayer;	// 挑战层数

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt8();
			mLayer = buffer.ReadInt32();
		}
	}

	// S->C 挑战灵兽墓地结果 msgId:8424;
	public class MsgData_sChallLingShouMuDiResult : MsgData
	{
		public sbyte result;	// 0成功，1失败

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt8();
		}
	}

	// S->C 扫荡灵兽墓地结果 msgId:8425;
	public class MsgData_sCanLingShouMuDiResult : MsgData
	{
		public sbyte result;	// 0成功，1失败

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt8();
		}
	}

	// S->C 退出灵兽墓地结果 msgId:8432;
	public class MsgData_sLingShouMuDiQuit : MsgData
	{
		public sbyte result;	// 0成功，1失败

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt8();
		}
	}

	// S->C 获得灵兽墓地结果 msgId:8426;
	public class MsgData_sLingShouMuDiGetAward : MsgData
	{
		public sbyte result;	// 0成功，1失败

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt8();
		}
	}

	// S->C 灵兽墓地怪物波数信息 msgId:8650;
	public class MsgData_sLingShouMuDiMonsterInfo : MsgData
	{
		public int index;	// 怪物波数
		public int monId;	// 怪物Id
		public int monNum;	// 怪物个数

		public override void unpack(NetReadBuffer buffer)
		{
			index = buffer.ReadInt32();
			monId = buffer.ReadInt32();
			monNum = buffer.ReadInt32();
		}
	}

	// C->S 查看荣耀等级面板数据 msgId:3646;
	public class MsgData_cDianfengInfo : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// C<->S 返回荣耀点数分配列表
	public class DianfengInfo_Attr : MsgData
	{
		public int ID;	// 荣耀等级表节点ID
		public int Num;	// 分配的点数

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(ID);
			buffer.WriteInt32(Num);
		}
		public override void unpack(NetReadBuffer buffer)
		{
			ID = buffer.ReadInt32();
			Num = buffer.ReadInt32();
		}
	}

	// S->C 返回荣耀等级面板结果 msgId:8646;
	public class MsgData_sDianfengInfo : MsgData
	{
		public uint ListSize;	// 数据长度
		public List<DianfengInfo_Attr> Data = new List<DianfengInfo_Attr>();	// 荣耀点数分配列表

		public override void unpack(NetReadBuffer buffer)
		{
			ListSize = buffer.ReadUInt32();
			Data = new List<DianfengInfo_Attr>();
			for (int i = 0; i < ListSize; i++)
			{
				DianfengInfo_Attr __item = new DianfengInfo_Attr();
				__item.unpack(buffer);
				Data.Add(__item);
			}
		}
	}

	// C->S 保存荣耀点数 msgId:3644;
	public class MsgData_cDianfengSave : MsgData
	{
		public uint ListSize;	// 数据长度
		public List<DianfengInfo_Attr> Data = new List<DianfengInfo_Attr>();	// 荣耀点数分配列表

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteUInt32(ListSize);
			for (int i = 0; i < ListSize; i++)
			{
				DianfengInfo_Attr __item = Data[i];
				__item.pack(buffer);
			}
		}
	}

	// S->C 返回保存结果 msgId:8644;
	public class MsgData_sDianfengSave : MsgData
	{
		public int Result;	// 0成功 1失败

		public override void unpack(NetReadBuffer buffer)
		{
			Result = buffer.ReadInt32();
		}
	}

	// C->S 重置荣耀点数 msgId:3645;
	public class MsgData_cDianfengReset : MsgData
	{
		public int Type;	// 属性类型

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(Type);
		}
	}

	// S->C 返回重置结果 msgId:8645;
	public class MsgData_sDianfengReset : MsgData
	{
		public int Result;	// 0成功 1失败

		public override void unpack(NetReadBuffer buffer)
		{
			Result = buffer.ReadInt32();
		}
	}

	// C->S 提高翅膀合成成功率的道具list
	public class MsgData_cWingHeChengItemList : MsgData
	{
		public int id;	// 道具id

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(id);
		}
	}

	// C->S 客户端请求:翅膀合成 msgId:3464;
	public class MsgData_cWingHeCheng : MsgData
	{
		public int wingId;	// 翅膀id
		public uint listCount;	// 成员个数
		public List<MsgData_cWingHeChengItemList> wingList = new List<MsgData_cWingHeChengItemList>();	// 提高翅膀合成成功率的道具list

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(wingId);
			buffer.WriteUInt32(listCount);
			for (int i = 0; i < listCount; i++)
			{
				MsgData_cWingHeChengItemList __item = wingList[i];
				__item.pack(buffer);
			}
		}
	}

	// C->S 客户端请求:使用原始翅膀 msgId:3698;
	public class MsgData_cUseWingOrginal : MsgData
	{
		public int type;	// 1翅膀 2皮肤

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(type);
		}
	}

	// C->S 客户端请求:激活皮肤 msgId:3699;
	public class MsgData_cActivateSkin : MsgData
	{
		public int wingId;	// 皮肤Id

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(wingId);
		}
	}

	// C->S 客户端请求:使用皮肤 msgId:3700;
	public class MsgData_cUseSkin : MsgData
	{
		public int wingId;	// 皮肤Id

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(wingId);
		}
	}

	// C->S 客户端请求:翅膀扩展激活 msgId:3978;
	public class MsgData_cWingExtActive : MsgData
	{
		public int id;	// id

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(id);
		}
	}

	// S->C 翅膀list
	public class MsgData_sWingInfoVO : MsgData
	{
		public long id;	// 道具cid
		public long time;	// 到期时间, -1无限
		public sbyte attrFlag;	// 是否有特殊属性，1有

		public override void unpack(NetReadBuffer buffer)
		{
			id = buffer.ReadInt64();
			time = buffer.ReadInt64();
			attrFlag = buffer.ReadInt8();
		}
	}

	// S->C 返回翅膀信息 msgId:8527;
	public class MsgData_sWingInfo : MsgData
	{
		public uint listCount;	// 成员个数
		public List<MsgData_sWingInfoVO> wingList = new List<MsgData_sWingInfoVO>();	// 翅膀list

		public override void unpack(NetReadBuffer buffer)
		{
			listCount = buffer.ReadUInt32();
			wingList = new List<MsgData_sWingInfoVO>();
			for (int i = 0; i < listCount; i++)
			{
				MsgData_sWingInfoVO __item = new MsgData_sWingInfoVO();
				__item.unpack(buffer);
				wingList.Add(__item);
			}
		}
	}

	// S->C 返回翅膀合成结果 msgId:8464;
	public class MsgData_sWingHeCheng : MsgData
	{
		public sbyte result;	// 0成功，1错误，2 材料不足 3银两不足 4背包已满

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt8();
		}
	}

	// S->C 使用原始翅膀 msgId:8698;
	public class MsgData_sUseWingOrginal : MsgData
	{
		public int wingId;	// 翅膀id
		public int result;	// 0成功，1失败

		public override void unpack(NetReadBuffer buffer)
		{
			wingId = buffer.ReadInt32();
			result = buffer.ReadInt32();
		}
	}

	// S->C 激活翅膀(皮肤) msgId:8699;
	public class MsgData_sActivateSkin : MsgData
	{
		public int wingId;	// 翅膀id
		public int result;	// 0成功，1失败

		public override void unpack(NetReadBuffer buffer)
		{
			wingId = buffer.ReadInt32();
			result = buffer.ReadInt32();
		}
	}

	// S->C 使用皮肤 msgId:8700;
	public class MsgData_sUseSkin : MsgData
	{
		public int wingId;	// 皮肤id
		public int result;	// 0成功，1失败

		public override void unpack(NetReadBuffer buffer)
		{
			wingId = buffer.ReadInt32();
			result = buffer.ReadInt32();
		}
	}

	// S->C 激活的皮肤列表
	public class MsgData_sSkinList : MsgData
	{
		public int id;	// 皮肤id

		public override void unpack(NetReadBuffer buffer)
		{
			id = buffer.ReadInt32();
		}
	}

	// S->C 获取翅膀(皮肤)相关信息 msgId:8701;
	public class MsgData_sWinAndSkinInfo : MsgData
	{
		public uint listCount;	// 成员个数
		public List<MsgData_sSkinList> wingList = new List<MsgData_sSkinList>();	// 激活的皮肤列表

		public override void unpack(NetReadBuffer buffer)
		{
			listCount = buffer.ReadUInt32();
			wingList = new List<MsgData_sSkinList>();
			for (int i = 0; i < listCount; i++)
			{
				MsgData_sSkinList __item = new MsgData_sSkinList();
				__item.unpack(buffer);
				wingList.Add(__item);
			}
		}
	}

	// S->C 返回：翅膀扩展激活 msgId:8978;
	public class MsgData_sWingExtActive : MsgData
	{
		public int result;	// 0成功，1其他， 2条件不足
		public int id;	// id

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt32();
			id = buffer.ReadInt32();
		}
	}

	// S->C 激活的翅膀扩展列表
	public class MsgData_sWinExtActiveList : MsgData
	{
		public int id;	// id
		public int isActive;	// 1激活, 非1没激活

		public override void unpack(NetReadBuffer buffer)
		{
			id = buffer.ReadInt32();
			isActive = buffer.ReadInt32();
		}
	}

	// S->C 返回 翅膀扩展激活列表 msgId:8979;
	public class MsgData_sWingExtActiveInfo : MsgData
	{
		public uint listCount;	// 成员个数
		public List<MsgData_sWinExtActiveList> wingList = new List<MsgData_sWinExtActiveList>();	// 翅膀扩展激活list

		public override void unpack(NetReadBuffer buffer)
		{
			listCount = buffer.ReadUInt32();
			wingList = new List<MsgData_sWinExtActiveList>();
			for (int i = 0; i < listCount; i++)
			{
				MsgData_sWinExtActiveList __item = new MsgData_sWinExtActiveList();
				__item.unpack(buffer);
				wingList.Add(__item);
			}
		}
	}

	// S->C 计数的翅膀扩展列表
	public class MsgData_sWinExtCountList : MsgData
	{
		public int id;	// id
		public int type;	// type
		public int num;	// 数量

		public override void unpack(NetReadBuffer buffer)
		{
			id = buffer.ReadInt32();
			type = buffer.ReadInt32();
			num = buffer.ReadInt32();
		}
	}

	// S->C 返回 翅膀扩展计数列表 msgId:8980;
	public class MsgData_sWingExtCountInfo : MsgData
	{
		public uint listCount;	// 成员个数
		public List<MsgData_sWinExtCountList> wingList = new List<MsgData_sWinExtCountList>();	// 翅膀扩展计数list

		public override void unpack(NetReadBuffer buffer)
		{
			listCount = buffer.ReadUInt32();
			wingList = new List<MsgData_sWinExtCountList>();
			for (int i = 0; i < listCount; i++)
			{
				MsgData_sWinExtCountList __item = new MsgData_sWinExtCountList();
				__item.unpack(buffer);
				wingList.Add(__item);
			}
		}
	}

	// C->S 客户端请求:发送聊天 msgId:2002;
	public class MsgData_cChat : MsgData
	{
		public int Channel;	// 频道,1全部,2世界,3区域,3军团,4阵营,5公会,6组队,7喇叭,8私聊
		public long ToID;	// 私聊时,接受者的ID
		public uint TextSize;	// 内容长度
		public List<byte> Text = new List<byte>();	// 内容

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(Channel);
			buffer.WriteInt64(ToID);
			buffer.WriteUInt32(TextSize);
			buffer.WriteBytes(Text);
		}
	}

	// C->S 客户端请求:设置是否接收对方的私聊信息 msgId:2004;
	public class MsgData_cSetPrivateChatState : MsgData
	{
		public long RoleID;	// 对方ID
		public int State;	// 私聊状态,1接受对方私聊,0关闭私聊

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(RoleID);
			buffer.WriteInt32(State);
		}
	}

	// S->C 服务器通知:收到聊天 msgId:7002;
	public class MsgData_sChat : MsgData
	{
		public long SenderID;	// 发送者ID
		public byte[] SenderName = new byte[32];	// 发送者名字
		public long SenderTeamID;	// 发送者队伍id
		public long SenderGuildID;	// 发送者公会id
		public sbyte SenderGuildPos;	// 发送者公会职务
		public int SenderVIP;	// 发送者VIP信息，按位存，前三位依次表示钻石、黄金、白银VIP是否开通，后29位存vip等级
		public int SenderLevel;	// 发送者等级
		public sbyte SenderIcon;	// 发送者头像
		public int SenderFlag;	// 发送者标示
		public sbyte SenderCityPos;	// 发送者王城职位
		public int SenderVFlag;	// 发送者V计划标示
		public int SenderTxhFlag;	// 黄钻标识
		public int SenderTxbFlag;	// 蓝钻标识
		public sbyte SenderIsGM;	// 是否GM标示
		public sbyte SenderCamp;	// 跨服场景时，发送者阵营
		public long SenderTime;	// 发送时间
		public int HornID;	// 喇叭时,喇叭id
		public int Channel;	// 频道
		public sbyte APP;	// app
		public uint TextSize;	// 内容长度
		public List<byte> Text = new List<byte>();	// 内容

		public override void unpack(NetReadBuffer buffer)
		{
			SenderID = buffer.ReadInt64();
			buffer.ReadBytes(SenderName);
			SenderTeamID = buffer.ReadInt64();
			SenderGuildID = buffer.ReadInt64();
			SenderGuildPos = buffer.ReadInt8();
			SenderVIP = buffer.ReadInt32();
			SenderLevel = buffer.ReadInt32();
			SenderIcon = buffer.ReadInt8();
			SenderFlag = buffer.ReadInt32();
			SenderCityPos = buffer.ReadInt8();
			SenderVFlag = buffer.ReadInt32();
			SenderTxhFlag = buffer.ReadInt32();
			SenderTxbFlag = buffer.ReadInt32();
			SenderIsGM = buffer.ReadInt8();
			SenderCamp = buffer.ReadInt8();
			SenderTime = buffer.ReadInt64();
			HornID = buffer.ReadInt32();
			Channel = buffer.ReadInt32();
			APP = buffer.ReadInt8();
			TextSize = buffer.ReadUInt32();
			buffer.ReadBytes(Text, (int)TextSize);
		}
	}

	// S->C 服务器通知:收到私聊通知 msgId:7003;
	public class MsgData_sPrivateChatNotice : MsgData
	{
		public long SenderID;	// 发送者ID
		public byte[] SenderName = new byte[32];	// 发送者名字
		public int Num;	// 未读私聊数量
		public sbyte SenderVIP;	// 发送者VIP等级
		public int SenderLevel;	// 发送者等级
		public sbyte SenderIcon;	// 发送者头像

		public override void unpack(NetReadBuffer buffer)
		{
			SenderID = buffer.ReadInt64();
			buffer.ReadBytes(SenderName);
			Num = buffer.ReadInt32();
			SenderVIP = buffer.ReadInt8();
			SenderLevel = buffer.ReadInt32();
			SenderIcon = buffer.ReadInt8();
		}
	}

	// S->C 服务器通知:聊天,系统通知 msgId:7005;
	public class MsgData_sChatSysNotice : MsgData
	{
		public int ID;	// 通知ID
		public uint ParamSize;	// 内容长度
		public List<byte> Param = new List<byte>();	// 内容

		public override void unpack(NetReadBuffer buffer)
		{
			ID = buffer.ReadInt32();
			ParamSize = buffer.ReadUInt32();
			buffer.ReadBytes(Param, (int)ParamSize);
		}
	}

	// S->C 服务器通知:公告 msgId:7006;
	public class MsgData_sNotice : MsgData
	{
		public int ID;	// 通知ID
		public uint ParamSize;	// 内容长度
		public List<byte> Param = new List<byte>();	// 内容

		public override void unpack(NetReadBuffer buffer)
		{
			ID = buffer.ReadInt32();
			ParamSize = buffer.ReadUInt32();
			buffer.ReadBytes(Param, (int)ParamSize);
		}
	}

	// S->C 对象死亡通知 msgId:8075;
	public class MsgData_sObjDeadInfo : MsgData
	{
		public long ID;	// 死亡对象id
		public byte[] KillerName = new byte[32];	// 击杀者名字
		public int KillerLevel;	// 击杀者等级
		public int KillerType;	// 击杀者类型
		public sbyte ObjectType;	// 对象类型
		public long KillerID;	// 击杀者id
		public int KillerSkillID;	// 击杀者技能id

		public override void unpack(NetReadBuffer buffer)
		{
			ID = buffer.ReadInt64();
			buffer.ReadBytes(KillerName);
			KillerLevel = buffer.ReadInt32();
			KillerType = buffer.ReadInt32();
			ObjectType = buffer.ReadInt8();
			KillerID = buffer.ReadInt64();
			KillerSkillID = buffer.ReadInt32();
		}
	}

	// S->C 成就列表:UI显示的阶段ID及进度
	public class MsgData_sAchievementVO : MsgData
	{
		public int type;	// 类型
		public sbyte value;	// 值
		public sbyte state;	// 领取状态

		public override void unpack(NetReadBuffer buffer)
		{
			type = buffer.ReadInt32();
			value = buffer.ReadInt8();
			state = buffer.ReadInt8();
		}
	}

	// S->C 返回 成就信息 msgId:8372;
	public class MsgData_sBackAchievementInfo : MsgData
	{
		public sbyte poingIndex;	// 点数阶段
		public uint listCount;	// 成员个数
		public List<MsgData_sAchievementVO> list = new List<MsgData_sAchievementVO>();	// 成就列表:UI显示的阶段ID及进度

		public override void unpack(NetReadBuffer buffer)
		{
			poingIndex = buffer.ReadInt8();
			listCount = buffer.ReadUInt32();
			list = new List<MsgData_sAchievementVO>();
			for (int i = 0; i < listCount; i++)
			{
				MsgData_sAchievementVO __item = new MsgData_sAchievementVO();
				__item.unpack(buffer);
				list.Add(__item);
			}
		}
	}

	// S->C 刷新成就进度 msgId:8479;
	public class MsgData_sAchievementUpData : MsgData
	{
		public int type;	// 类型
		public int value;	// 值
		public sbyte state;	// 领取状态

		public override void unpack(NetReadBuffer buffer)
		{
			type = buffer.ReadInt32();
			value = buffer.ReadInt32();
			state = buffer.ReadInt8();
		}
	}

	// S->C 刷新成就进度 传送信息 msgId:8096
	public class MsgData_sChangePos : MsgData
	{
		public long RoleID;	// 玩家guid
		public double PosX;	// 传送位置坐标x
		public double PosY;	// 传送位置坐标y

		public override void unpack(NetReadBuffer buffer)
		{
			RoleID = buffer.ReadInt64();
			PosX = buffer.ReadDouble();
			PosY = buffer.ReadDouble();
		}
	}

	// S->C 服务器通知:技能目标列表 msgId:9932;
	public class MsgData_sSkillTargetList : MsgData
	{
		public long CasterID;	// 施法者ID
		public int SkillID;	// 技能ID
		public sbyte TargetCount;	// 目标个数
		public List<long> Targets = new List<long>();	// 目标列表

		public override void unpack(NetReadBuffer buffer)
		{
			CasterID = buffer.ReadInt64();
			SkillID = buffer.ReadInt32();
			TargetCount = buffer.ReadInt8();
			Targets = new List<long>();
			for (int i = 0; i < TargetCount; i++)
			{
				long __item = buffer.ReadInt64();
				Targets.Add(__item);
			}
		}
	}

	// C->S 分解法宝列表
	public class MsgData_cMagicKey : MsgData
	{
		public long id;	// 法宝ID

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(id);
		}
	}

	// C->S 请求法宝分解 msgId:3655
	public class MsgData_cResMagicKeyDecompose : MsgData
	{
		public uint count;	// 数量
		public List<MsgData_cMagicKey> items = new List<MsgData_cMagicKey>();	// 分解列表

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteUInt32(count);
			for (int i = 0; i < count; i++)
			{
				MsgData_cMagicKey __item = items[i];
				__item.pack(buffer);
			}
		}
	}

	// S->C 返回法宝分解结果 msgId:8667
	public class MsgData_sResMagicKeyDecompose : MsgData
	{
		public int result;	//  0成功，-1功能未开启，-2法宝不存在，-3，-4无该法宝配置，-5装备中，-6扣法宝失败

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt32();
		}
	}

	// S->C 法宝信息
	public class MsgData_sMagickeyinfo : MsgData
	{
		public long guid;	// 物品实例id
		public int magickeyID;	// 法宝配置ID
		public int wuxing;	// 悟性值
		public int level;	// 等级
		public double totalExp;	// 总经验值
		public int starCount;	// 法宝星级
		public int starProgress;	// 法宝星级进度
		public int passiveskill1;	// 被动技能1
		public int passiveskill2;	// 被动技能2
		public int passiveskill3;	// 被动技能3
		public int passiveskill4;	// 被动技能4
		public int passiveskill5;	// 被动技能5
		public int passiveskill6;	// 被动技能6
		public int passiveskill7;	// 被动技能7
		public int passiveskill8;	// 被动技能8
		public int passiveskill9;	// 被动技能9
		public int passiveskill10;	// 被动技能10
		public int passiveskill11;	// 被动技能11
		public int passiveskill12;	// 被动技能12
		public int awakeCount;	// 觉醒次数 0、未觉醒 1、觉醒一次 2、觉醒二次
		public int feisheng;	// 飞升等级
		public int feishengext;	// 当前飞升经验
		public int FightPower;	// 战斗力
		public int[] AttrType = new int[3];	// 属性类型
		public int[] AttrValue = new int[3];	// 属性值

		public override void unpack(NetReadBuffer buffer)
		{
			guid = buffer.ReadInt64();
			magickeyID = buffer.ReadInt32();
			wuxing = buffer.ReadInt32();
			level = buffer.ReadInt32();
			totalExp = buffer.ReadDouble();
			starCount = buffer.ReadInt32();
			starProgress = buffer.ReadInt32();
			passiveskill1 = buffer.ReadInt32();
			passiveskill2 = buffer.ReadInt32();
			passiveskill3 = buffer.ReadInt32();
			passiveskill4 = buffer.ReadInt32();
			passiveskill5 = buffer.ReadInt32();
			passiveskill6 = buffer.ReadInt32();
			passiveskill7 = buffer.ReadInt32();
			passiveskill8 = buffer.ReadInt32();
			passiveskill9 = buffer.ReadInt32();
			passiveskill10 = buffer.ReadInt32();
			passiveskill11 = buffer.ReadInt32();
			passiveskill12 = buffer.ReadInt32();
			awakeCount = buffer.ReadInt32();
			feisheng = buffer.ReadInt32();
			feishengext = buffer.ReadInt32();
			FightPower = buffer.ReadInt32();
			for (int i = 0; i < 3; i++)
			{
				int __item = buffer.ReadInt32();
				AttrType[i]=__item;
			}
			for (int i = 0; i < 3; i++)
			{
				int __item = buffer.ReadInt32();
				AttrValue[i]=__item;
			}
		}
	}

	// S->C 更新法宝信息 msgId:8628
	public class MsgData_sUpdateMagicKeyInfo : MsgData
	{
		public uint count;	// 数量
		public List<MsgData_sMagickeyinfo> items = new List<MsgData_sMagickeyinfo>();	// 法宝信息例表

		public override void unpack(NetReadBuffer buffer)
		{
			count = buffer.ReadUInt32();
			items = new List<MsgData_sMagickeyinfo>();
			for (int i = 0; i < count; i++)
			{
				MsgData_sMagickeyinfo __item = new MsgData_sMagickeyinfo();
				__item.unpack(buffer);
				items.Add(__item);
			}
		}
	}

	// S->C 返回法宝培养结果 msgId:8633
	public class MsgData_sTrainMagicKey : MsgData
	{
		public int result;	// 法宝培养结果0成功，-1功能未开启，-2没有该法宝，-3已达最高级，-4道具不足，-5扣道具失败，-6配置表中无该法宝

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt32();
		}
	}

	// S->C 法宝打造结果
	public class MsgData_sMakeresult : MsgData
	{
		public long guid;	// 物品实例id

		public override void unpack(NetReadBuffer buffer)
		{
			guid = buffer.ReadInt64();
		}
	}

	// S->C 返回法宝打造结果 msgId:8627
	public class MsgData_sMakeMagicKey : MsgData
	{
		public int result;	// 法宝打造结果
		public uint count;	// 数量
		public List<MsgData_sMakeresult> items = new List<MsgData_sMakeresult>();	// 法宝打造信息例表

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt32();
			count = buffer.ReadUInt32();
			items = new List<MsgData_sMakeresult>();
			for (int i = 0; i < count; i++)
			{
				MsgData_sMakeresult __item = new MsgData_sMakeresult();
				__item.unpack(buffer);
				items.Add(__item);
			}
		}
	}

	// S->C 法宝祝福值列表
	public class MsgData_sWashMakeresult : MsgData
	{
		public int MagicKeyMakeOrder;	// 打造的等阶ID
		public int MagicKeyWashValue;	// 现有的打造祝福值
		public int MagicKeyMaxWashValue;	// 最大的打造祝福值;

		public override void unpack(NetReadBuffer buffer)
		{
			MagicKeyMakeOrder = buffer.ReadInt32();
			MagicKeyWashValue = buffer.ReadInt32();
			MagicKeyMaxWashValue = buffer.ReadInt32();
		}
	}

	// S->C  返回法宝祝福值信息 msgId:8818;
	public class MsgData_sMagicKeyWashInfo : MsgData
	{
		public uint count;	// 数量
		public List<MsgData_sWashMakeresult> items = new List<MsgData_sWashMakeresult>();	//  法宝祝福值列表;

		public override void unpack(NetReadBuffer buffer)
		{
			count = buffer.ReadUInt32();
			items = new List<MsgData_sWashMakeresult>();
			for (int i = 0; i < count; i++)
			{
				MsgData_sWashMakeresult __item = new MsgData_sWashMakeresult();
				__item.unpack(buffer);
				items.Add(__item);
			}
		}
	}

	// S->C 仙灵列表
	public class MsgData_sGodInfosList : MsgData
	{
		public long guid;	// 物品实例id
		public int wuxing;	// 悟性值
		public int id;	// 配置ID
		public long magicGuid;	// 法宝实例ID

		public override void unpack(NetReadBuffer buffer)
		{
			guid = buffer.ReadInt64();
			wuxing = buffer.ReadInt32();
			id = buffer.ReadInt32();
			magicGuid = buffer.ReadInt64();
		}
	}

	// S->C 返回法宝仙灵列表 msgId:8697
	public class MsgData_sReturnMagicKeyGodInfos : MsgData
	{
		public uint count;	// 数量
		public List<MsgData_sGodInfosList> items = new List<MsgData_sGodInfosList>();	// 仙灵列表

		public override void unpack(NetReadBuffer buffer)
		{
			count = buffer.ReadUInt32();
			items = new List<MsgData_sGodInfosList>();
			for (int i = 0; i < count; i++)
			{
				MsgData_sGodInfosList __item = new MsgData_sGodInfosList();
				__item.unpack(buffer);
				items.Add(__item);
			}
		}
	}

	// S->C 返回仙灵穿戴结果 msgId:8709
	public class MsgData_sResMaigcKeyGodInset : MsgData
	{
		public long guid;	// 物品实例id
		public long magicGuid;	// 法宝实例ID

		public override void unpack(NetReadBuffer buffer)
		{
			guid = buffer.ReadInt64();
			magicGuid = buffer.ReadInt64();
		}
	}

	// S->C 被动技能列表
	public class MsgData_sPassSkill : MsgData
	{
		public int skillID;	// 被动技能

		public override void unpack(NetReadBuffer buffer)
		{
			skillID = buffer.ReadInt32();
		}
	}

	// S->C 返回法宝技能 msgId:8736
	public class MsgData_sMagicKeyInsetSkill : MsgData
	{
		public long guid;	// 法宝实例ID
		public uint count;	// 数量
		public List<MsgData_sPassSkill> items = new List<MsgData_sPassSkill>();	// 法宝实例列表

		public override void unpack(NetReadBuffer buffer)
		{
			guid = buffer.ReadInt64();
			count = buffer.ReadUInt32();
			items = new List<MsgData_sPassSkill>();
			for (int i = 0; i < count; i++)
			{
				MsgData_sPassSkill __item = new MsgData_sPassSkill();
				__item.unpack(buffer);
				items.Add(__item);
			}
		}
	}

	// S->C 返回:法宝飞升升经验 msgId:9037
	public class MsgData_sMagicKeyFeiSheng : MsgData
	{
		public int result;	//  0成功 1材料不足
		public long guid;	// 法宝实例ID
		public int feishengId;	// 飞升Id
		public int feishengext;	// 飞升经验

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt32();
			guid = buffer.ReadInt64();
			feishengId = buffer.ReadInt32();
			feishengext = buffer.ReadInt32();
		}
	}

	// S->C 返回法宝被动技能领悟 msgId:8879
	public class MsgData_sReturnMagickeySKillLingwu : MsgData
	{
		public sbyte result;	//  结果,0成功;
		public sbyte chengeIndex;	// 变化的槽位

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt8();
			chengeIndex = buffer.ReadInt8();
		}
	}

	// C->S 客户端请求：法宝升星 msgId:3735
	public class MsgData_cMagicKeyStarLevelUp : MsgData
	{
		public long guid;	// 法宝实例ID

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(guid);
		}
	}

	// C->S 损失矿石
	public class MsgData_cTrainMagicKeyItem : MsgData
	{
		public long TrainDanguid;	// 法宝培养丹ID
		public int TrainDanNum;	// 法宝培养丹数量

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(TrainDanguid);
			buffer.WriteInt32(TrainDanNum);
		}
	}

	// C->S 请求法宝培养 msgId:3626;
	public class MsgData_cTrainMagicKey : MsgData
	{
		public long guid;	// 法宝的GUID
		public uint count;	// 数量
		public List<MsgData_cTrainMagicKeyItem> items = new List<MsgData_cTrainMagicKeyItem>();	// 损失矿石列表

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(guid);
			buffer.WriteUInt32(count);
			for (int i = 0; i < count; i++)
			{
				MsgData_cTrainMagicKeyItem __item = items[i];
				__item.pack(buffer);
			}
		}
	}

	// C->S 请求法宝打造 msgId:3623
	public class MsgData_cMakeMagicKey : MsgData
	{
		public int MakeOrder;	// 法宝打造品阶
		public int MakeCount;	// 法宝打造数量
		public int bindFlag;	// 材料绑定标志 1 只使用绑定 2 只使用非绑定 3 两者都用，优先绑定
		public int IsVIP;	// 1VIP打造, 0普通打造 2 领取法宝

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(MakeOrder);
			buffer.WriteInt32(MakeCount);
			buffer.WriteInt32(bindFlag);
			buffer.WriteInt32(IsVIP);
		}
	}

	// C->S 已锁住的技能槽位
	public class MsgData_cSuoList : MsgData
	{
		public int indexNum;	// id

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(indexNum);
		}
	}

	// C->S 请求法宝被动技能领悟 msgId:3879
	public class MsgData_cRequestMagickeySkillLingwu : MsgData
	{
		public long magickId;	// ID
		public long itemID;	// 道具ID
		public uint count;	// 数量
		public List<MsgData_cSuoList> items = new List<MsgData_cSuoList>();	// 已锁住的技能槽位

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(magickId);
			buffer.WriteInt64(itemID);
			buffer.WriteUInt32(count);
			for (int i = 0; i < count; i++)
			{
				MsgData_cSuoList __item = items[i];
				__item.pack(buffer);
			}
		}
	}

	// C->S 客户端请求：法宝仙灵穿戴 msgId:3708;
	public class MsgData_cReqMaigcKeyGodInset : MsgData
	{
		public long guid;	// 实例ID
		public long magicGuid;	// 法宝实例ID
		public int type;	//  0、穿上  1、卸下

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(guid);
			buffer.WriteInt64(magicGuid);
			buffer.WriteInt32(type);
		}
	}

	// S->C 返回升星结果 msgId:8735
	public class MsgData_sMagicKeyStarLevelUp : MsgData
	{
		public int progressNum;	// 法宝星级每次进度数量
		public int result;	// 结果 0-成功 1-失败

		public override void unpack(NetReadBuffer buffer)
		{
			progressNum = buffer.ReadInt32();
			result = buffer.ReadInt32();
		}
	}

	// C->S 客户端请求：宝石孔位信息 msgId:3738;
	public class MsgData_cReqGemOpenInfo : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// S->C 孔位信息
	public class MsgData_sGemOpenItem : MsgData
	{
		public int Pos;	// 空位
		public int IsOpen;	// 是否开启1开启

		public override void unpack(NetReadBuffer buffer)
		{
			Pos = buffer.ReadInt32();
			IsOpen = buffer.ReadInt32();
		}
	}

	// S->C 服务器返回: 宝石孔位信息 msgId:8738
	public class MsgData_sGemOpenInfo : MsgData
	{
		public uint Size;	// Size
		public List<MsgData_sGemOpenItem> Data = new List<MsgData_sGemOpenItem>();	// Data

		public override void unpack(NetReadBuffer buffer)
		{
			Size = buffer.ReadUInt32();
			Data = new List<MsgData_sGemOpenItem>();
			for (int i = 0; i < Size; i++)
			{
				MsgData_sGemOpenItem __item = new MsgData_sGemOpenItem();
				__item.unpack(buffer);
				Data.Add(__item);
			}
		}
	}

	// C->S 客户端请求：开启宝石孔位 msgId:3739;
	public class MsgData_cReqGemSlotOpen : MsgData
	{
		public int Pos;	// 空位

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(Pos);
		}
	}

	// S->C 服务器返回: 开启宝石孔位信息 msgId:8739;
	public class MsgData_sReqGemSlotOpen : MsgData
	{
		public int Result;	// 结果
		public int Pos;	// 位置

		public override void unpack(NetReadBuffer buffer)
		{
			Result = buffer.ReadInt32();
			Pos = buffer.ReadInt32();
		}
	}

	// C->S 请求装备宝石操作 msgId:3610;
	public class MsgData_cReqEquipGemInset : MsgData
	{
		public int Pos;	// 空位
		public long Guid;	// Guid
		public int Type;	// 操作类型 1镶嵌, 2卸下 3 一键镶嵌;

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(Pos);
			buffer.WriteInt64(Guid);
			buffer.WriteInt32(Type);
		}
	}

	// S->C 宝石镶嵌信息
	public class MsgData_sGemInsetItem : MsgData
	{
		public int Pos;	// 空位
		public long Guid;	// Guid

		public override void unpack(NetReadBuffer buffer)
		{
			Pos = buffer.ReadInt32();
			Guid = buffer.ReadInt64();
		}
	}

	// S->C 返回装备宝石操作 msgId:8611;
	public class MsgData_sReqEquipGemInset : MsgData
	{
		public uint Size;	// Size
		public List<MsgData_sGemInsetItem> Data = new List<MsgData_sGemInsetItem>();	// Data

		public override void unpack(NetReadBuffer buffer)
		{
			Size = buffer.ReadUInt32();
			Data = new List<MsgData_sGemInsetItem>();
			for (int i = 0; i < Size; i++)
			{
				MsgData_sGemInsetItem __item = new MsgData_sGemInsetItem();
				__item.unpack(buffer);
				Data.Add(__item);
			}
		}
	}

	// C->S 装备宝石升级 msgId:3157;
	public class MsgData_cEquipGemUpLevel : MsgData
	{
		public int ID;	// ID
		public int AutoBuy;	// 是否自动购买0=true;

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(ID);
			buffer.WriteInt32(AutoBuy);
		}
	}

	// S->C 返回宝石升级信息 msgId:8157;
	public class MsgData_sEquipGemUpLevel : MsgData
	{
		public int Result;	// Result
		public int ID;	// ID
		public int Level;	// Level

		public override void unpack(NetReadBuffer buffer)
		{
			Result = buffer.ReadInt32();
			ID = buffer.ReadInt32();
			Level = buffer.ReadInt32();
		}
	}

	// S->C 服务器通知:免费传送次数 msgId:8412
	public class MsgData_sTeleportFreeTime : MsgData
	{
		public int Time;	// 免费传送次数(玩家初始有n次, 低于n后每小时恢复1次,最大累积n次,每日0点重置为n次.n在常量表配置)

		public override void unpack(NetReadBuffer buffer)
		{
			Time = buffer.ReadInt32();
		}
	}

	// C->S 请求剧情副本NPC对话结束 msgId:3101;
	public class MsgData_cDungeonNpcTalkEnd : MsgData
	{
		public int id;	// 剧情副本stepid, 1 = 任务

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(id);
		}
	}

	// C->S 请求进入剧情副本 msgId:3102;
	public class MsgData_cEnterDungeon : MsgData
	{
		public int flag;	// 1.重新开始，  2.延续上一次中途退出的副本
		public int id;	// 副本id

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(flag);
			buffer.WriteInt32(id);
		}
	}

	// C->S 请求退出剧情副本 msgId:3103;
	public class MsgData_cLeaveDungeon : MsgData
	{
		public int id;	// 副本id

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(id);
		}
	}

	// C->S 剧情播放完成 msgId:3104;
	public class MsgData_cStoryEnd : MsgData
	{
		public int type;	// 1.对话框   2.任务：其他是剧情
		public int id;	// 剧情id

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(type);
			buffer.WriteInt32(id);
		}
	}

	// C->S 请求副本组列表 msgId:3137;
	public class MsgData_cDungeonGrup : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// C->S 放弃副本(副本进行中退出后倒计时,点击确定放弃) msgId:3140;
	public class MsgData_cDungeonAbstain : MsgData
	{
		public int id;	// 副本ID

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(id);
		}
	}

	// C->S 领取奖励 msgId:3141;
	public class MsgData_cDungeonGetAward : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// S->C 进入副本返回结果 msgId:8102
	public class MsgData_sEnterDungeonResult : MsgData
	{
		public int result;	// 0成功，1错误，2副本已关闭， 3不在同一线， 4当前场景不对
		public int id;	// 副本id
		public int stepId;	// 步骤id
		public int time;	// 副本进行时间
		public int bossId;	// boss变异id
		public int bossStar;	// boss星级

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt32();
			id = buffer.ReadInt32();
			stepId = buffer.ReadInt32();
			time = buffer.ReadInt32();
			bossId = buffer.ReadInt32();
			bossStar = buffer.ReadInt32();
		}
	}

	// S->C 离开副本返回结果 msgId:8103
	public class MsgData_sLeaveDungeonResult : MsgData
	{
		public int result;	// 0成功

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt32();
		}
	}

	// S->C 剧情完成返回结果 msgId:8104
	public class MsgData_sStoryEndResult : MsgData
	{
		public int result;	// 0成功

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt32();
		}
	}

	// S->C 副本剧情步骤 msgId:8105
	public class MsgData_sStoryStep : MsgData
	{
		public int stepId;	// 步骤id
		public int id;	// 副本id

		public override void unpack(NetReadBuffer buffer)
		{
			stepId = buffer.ReadInt32();
			id = buffer.ReadInt32();
		}
	}

	// S->C 副本组各难度得分
	public class MsgData_sDungeonDifficulty : MsgData
	{
		public int time;	// 通关最快时间

		public override void unpack(NetReadBuffer buffer)
		{
			time = buffer.ReadInt32();
		}
	}

	// S->C 副本组列表
	public class MsgData_sDungeonGroupList : MsgData
	{
		public int group;	// 副本组:各个难度组成一组
		public sbyte useTimes;	// 已用次数
		public sbyte usePayTimese;	// 已用付费次数
		public sbyte curDiff;	// 对应5个难度:1,2,3,4,5
		public MsgData_sDungeonDifficulty[] sourceList = new MsgData_sDungeonDifficulty[5];	// 副本组各难度得分

		public override void unpack(NetReadBuffer buffer)
		{
			group = buffer.ReadInt32();
			useTimes = buffer.ReadInt8();
			usePayTimese = buffer.ReadInt8();
			curDiff = buffer.ReadInt8();
			for (int i = 0; i < 5; i++)
			{
				MsgData_sDungeonDifficulty __item = new MsgData_sDungeonDifficulty();
				__item.unpack(buffer);
				sourceList[i]=__item;
			}
		}
	}

	// S->C 副本组列表更新 msgId:8137
	public class MsgData_sDungeonGroupUpdate : MsgData
	{
		public int listCount;	// 成员个数
		public List<MsgData_sDungeonGroupList> groupList = new List<MsgData_sDungeonGroupList>();	// 副本组列表

		public override void unpack(NetReadBuffer buffer)
		{
			listCount = buffer.ReadInt32();
			groupList = new List<MsgData_sDungeonGroupList>();
			for (int i = 0; i < listCount; i++)
			{
				MsgData_sDungeonGroupList __item = new MsgData_sDungeonGroupList();
				__item.unpack(buffer);
				groupList.Add(__item);
			}
		}
	}

	// S->C 开始副本关闭倒计时 msgId:8140
	public class MsgData_sDungeonCountDown : MsgData
	{
		public int id;	// 副本id
		public int line;	// 线
		public int time;	// 倒计时时间

		public override void unpack(NetReadBuffer buffer)
		{
			id = buffer.ReadInt32();
			line = buffer.ReadInt32();
			time = buffer.ReadInt32();
		}
	}

	// S->C 副本过关结果 msgId:8141
	public class MsgData_sDungeonPassResult : MsgData
	{
		public int id;	// 副本id
		public int result;	// 1.胜利   0失败

		public override void unpack(NetReadBuffer buffer)
		{
			id = buffer.ReadInt32();
			result = buffer.ReadInt32();
		}
	}

	// C->S 请求进入任务副本 msgId:4933;
	public class MsgData_cDungeonQuestEnter : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// C->S 请求退出任务副本 msgId:4934;
	public class MsgData_cDungeonQuestQuit : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// S->C 任务副本状态更新 msgId:9935;
	public class MsgData_sDungeonQuestStateUpdate : MsgData
	{
		public uint id;	// 副本ID
		public byte state;	// 副本状态 0成功，1失败， 2 正在进行中
		public byte index;	// 怪物波数
		public byte num;	// 死亡怪物个数

		public override void unpack(NetReadBuffer buffer)
		{
			id = buffer.ReadUInt32();
			state = buffer.ReadUInt8();
			index = buffer.ReadUInt8();
			num = buffer.ReadUInt8();
		}
	}

	// C->S 日环任务升到5星 msgId:3152;
	public class MsgData_cDailyQuestStar : MsgData
	{
		public int id;	// 任务ID

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(id);
		}
	}

	// C->S 日环任务一键完成 msgId:3153;
	public class MsgData_cDailyQuestFinish : MsgData
	{
		public int type;	// 1.免费领一倍   2.元宝领

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(type);
		}
	}

	// C->S 请求日环任务结果 msgId:3154;
	public class MsgData_cDailyQuestResult : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// C->S 日环任务抽奖 msgId:3155;
	public class MsgData_cDailyDraw : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// C->S 日环任务抽奖确认 msgId:3156;
	public class MsgData_cDailyDrawConfirm : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// C->S 讨伐日环任务升到5星 msgId:3702;
	public class MsgData_cAgainstQuestStar : MsgData
	{
		public int id;	// 任务ID

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(id);
		}
	}

	// C->S 讨伐任务一键完成 msgId:3703;
	public class MsgData_cAgainstQuestFinish : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// C->S 请求讨伐任务结果 msgId:3704;
	public class MsgData_cAgainstQuestResult : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// C->S 讨伐任务抽奖 msgId:3705;
	public class MsgData_cAgainstDraw : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// C->S 讨伐跳环领奖 msgId:3706;
	public class MsgData_cGetAgainstDraw : MsgData
	{
		public int type;	// 1.免费领一倍   2.银两领双倍  3.元宝领三倍

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(type);
		}
	}

	// C->S 讨伐任务抽奖确认 msgId:3709;
	public class MsgData_cDAgainstDrawConfirm : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// S->C 日环任务升5星结果 msgId:8152
	public class MsgData_sDailyQuestStar : MsgData
	{
		public sbyte result;	// 0成功 1钱不够  2目标任务不存在

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt8();
		}
	}

	// S->C 日环抽奖列表
	public class MsgData_sDailyQuestRewardlist : MsgData
	{
		public int id;	// 物品id
		public int num;	// 数量

		public override void unpack(NetReadBuffer buffer)
		{
			id = buffer.ReadInt32();
			num = buffer.ReadInt32();
		}
	}

	// S->C 一键完成的日环任务列表
	public class MsgData_sDailyQuestList : MsgData
	{
		public int id;	// 任务id
		public int star;	// 星级
		public int num;	// 倍率

		public override void unpack(NetReadBuffer buffer)
		{
			id = buffer.ReadInt32();
			star = buffer.ReadInt32();
			num = buffer.ReadInt32();
		}
	}

	// S->C 返回日环任务一键完成奖励信息 msgId:8153
	public class MsgData_sDailyQuestFinish : MsgData
	{
		public sbyte result;	// 0成功 1钱不够  2VIP等级不够  3等级不足
		public int level;	// 每日完成奖励等级
		public MsgData_sDailyQuestRewardlist[] rewardList = new MsgData_sDailyQuestRewardlist[4];	// 日环抽奖列表
		public uint count;	// 完成的日环任务列表个数
		public List<MsgData_sDailyQuestList> questList = new List<MsgData_sDailyQuestList>();	// 一键完成的日环任务列表

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt8();
			level = buffer.ReadInt32();
			for (int i = 0; i < 4; i++)
			{
				MsgData_sDailyQuestRewardlist __item = new MsgData_sDailyQuestRewardlist();
				__item.unpack(buffer);
				rewardList[i]=__item;
			}
			count = buffer.ReadUInt32();
			questList = new List<MsgData_sDailyQuestList>();
			for (int i = 0; i < count; i++)
			{
				MsgData_sDailyQuestList __item = new MsgData_sDailyQuestList();
				__item.unpack(buffer);
				questList.Add(__item);
			}
		}
	}

	// S->C 返回日环任务结果 msgId:8154
	public class MsgData_sDailyQuestResult : MsgData
	{
		public MsgData_sDailyQuestList[] questList = new MsgData_sDailyQuestList[20];	// 日环任务列表
		public MsgData_sDailyQuestRewardlist[] rewardList = new MsgData_sDailyQuestRewardlist[4];	// 日环抽奖列表

		public override void unpack(NetReadBuffer buffer)
		{
			for (int i = 0; i < 20; i++)
			{
				MsgData_sDailyQuestList __item = new MsgData_sDailyQuestList();
				__item.unpack(buffer);
				questList[i]=__item;
			}
			for (int i = 0; i < 4; i++)
			{
				MsgData_sDailyQuestRewardlist __item = new MsgData_sDailyQuestRewardlist();
				__item.unpack(buffer);
				rewardList[i]=__item;
			}
		}
	}

	// S->C 返回日环任务抽奖 msgId:8155
	public class MsgData_sDailyDraw : MsgData
	{
		public int rewardIndex;	// 奖励索引
		public int doubleIndex;	// 倍数索引

		public override void unpack(NetReadBuffer buffer)
		{
			rewardIndex = buffer.ReadInt32();
			doubleIndex = buffer.ReadInt32();
		}
	}

	// S->C 讨伐任务升5星结果 msgId:8702
	public class MsgData_sAgainstQuestStar : MsgData
	{
		public sbyte result;	// 0成功 1钱不够  2目标任务不存在

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt8();
		}
	}

	// S->C 一键完成的讨伐任务列表
	public class MsgData_sAgainstQuestList : MsgData
	{
		public int id;	// 任务id

		public override void unpack(NetReadBuffer buffer)
		{
			id = buffer.ReadInt32();
		}
	}

	// S->C 返回讨伐任务一键完成奖励信息 msgId:8703
	public class MsgData_sAgainstQuestFinish : MsgData
	{
		public sbyte result;	// 0成功 1钱不够  2VIP等级不够  3等级不足
		public int level;	// 每日完成奖励等级
		public uint count;	// 完成的日讨伐任务列表个数
		public List<MsgData_sAgainstQuestList> questList = new List<MsgData_sAgainstQuestList>();	// 一键完成的讨伐任务列表

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt8();
			level = buffer.ReadInt32();
			count = buffer.ReadUInt32();
			questList = new List<MsgData_sAgainstQuestList>();
			for (int i = 0; i < count; i++)
			{
				MsgData_sAgainstQuestList __item = new MsgData_sAgainstQuestList();
				__item.unpack(buffer);
				questList.Add(__item);
			}
		}
	}

	// S->C 返回讨伐任务结果 msgId:8704
	public class MsgData_sAgainstQuestResult : MsgData
	{
		public MsgData_sDailyQuestList[] questList = new MsgData_sDailyQuestList[20];	// 日环任务列表
		public MsgData_sDailyQuestRewardlist[] rewardList = new MsgData_sDailyQuestRewardlist[4];	// 日环抽奖列表

		public override void unpack(NetReadBuffer buffer)
		{
			for (int i = 0; i < 20; i++)
			{
				MsgData_sDailyQuestList __item = new MsgData_sDailyQuestList();
				__item.unpack(buffer);
				questList[i]=__item;
			}
			for (int i = 0; i < 4; i++)
			{
				MsgData_sDailyQuestRewardlist __item = new MsgData_sDailyQuestRewardlist();
				__item.unpack(buffer);
				rewardList[i]=__item;
			}
		}
	}

	// S->C 返回讨伐任务抽奖 msgId:8705
	public class MsgData_sAgainstQuestDraw : MsgData
	{
		public int rewardIndex;	// 奖励索引
		public int doubleIndex;	// 倍数索引

		public override void unpack(NetReadBuffer buffer)
		{
			rewardIndex = buffer.ReadInt32();
			doubleIndex = buffer.ReadInt32();
		}
	}

	// S->C 服务端通知:讨伐跳环领奖 msgId:8706
	public class MsgData_sAgainstQSkipReward : MsgData
	{
		public sbyte result;	// 日环跳环领取结果:0 成功

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt8();
		}
	}

	// S->C 服务端返回跳环结果 msgId:8707
	public class MsgData_sAgainstQuestSkipResult : MsgData
	{
		public int rount;	// 跳到第几环
		public uint count;	// 列表数量
		public List<MsgData_sDailyQuestList> AgainstQuestList = new List<MsgData_sDailyQuestList>();	// 跳过的任务列表，跳环均以5星结算

		public override void unpack(NetReadBuffer buffer)
		{
			rount = buffer.ReadInt32();
			count = buffer.ReadUInt32();
			AgainstQuestList = new List<MsgData_sDailyQuestList>();
			for (int i = 0; i < count; i++)
			{
				MsgData_sDailyQuestList __item = new MsgData_sDailyQuestList();
				__item.unpack(buffer);
				AgainstQuestList.Add(__item);
			}
		}
	}

	// S->C 服务端通知是否抽奖，每环结束都要发 msgId:8708
	public class MsgData_sAgainstQDrawNotice : MsgData
	{
		public sbyte result;	// 0抽奖， 1不抽奖

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt8();
		}
	}

	// S->C 服务端通知：任务下发的扫荡奖励信息
	public class MsgData_sQuestReward : MsgData
	{
		public long itemId;	// 物品id
		public int count;	// 物品数量

		public override void unpack(NetReadBuffer buffer)
		{
			itemId = buffer.ReadInt64();
			count = buffer.ReadInt32();
		}
	}

	// S->C 服务端通知：任务下发的扫荡奖励 msgId:9940
	public class MsgData_sQuestRewardResult : MsgData
	{
		public int type;	// 2.跑环奖励   3讨伐奖励   4 通缉奖励
		public long exp;	// 经验奖励
		public int money;	// 金币奖励
		public int zhiqi;	// 灵气奖励
		public int count;	// 完成的奖励列表个数
		public List<MsgData_sQuestReward> itemList = new List<MsgData_sQuestReward>();	// 物品奖励列表

		public override void unpack(NetReadBuffer buffer)
		{
			type = buffer.ReadInt32();
			exp = buffer.ReadInt64();
			money = buffer.ReadInt32();
			zhiqi = buffer.ReadInt32();
			count = buffer.ReadInt32();
			itemList = new List<MsgData_sQuestReward>();
			for (int i = 0; i < count; i++)
			{
				MsgData_sQuestReward __item = new MsgData_sQuestReward();
				__item.unpack(buffer);
				itemList.Add(__item);
			}
		}
	}

	// C->S 请求合成道具 msgId:3204;
	public class MsgData_cItemCompose : MsgData
	{
		public int ID;	// 合成分解的道具Id;
		public int Type;	// 合成分解类型 1:合成 2:分解;
		public int Count;	// 合成分解的道具数量;

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(ID);
			buffer.WriteInt32(Type);
			buffer.WriteInt32(Count);
		}
	}

	// S->C 服务端通知:返回道具合成信息 msgId:8204;
	public class MsgData_sItemCompose : MsgData
	{
		public int Result;	// 返回结果;
		public int ID;	// 合成分解的道具Id;
		public int Type;	// 合成分解类型 1:合成 2:分解;

		public override void unpack(NetReadBuffer buffer)
		{
			Result = buffer.ReadInt32();
			ID = buffer.ReadInt32();
			Type = buffer.ReadInt32();
		}
	}

	// S->C 服务端广播通知:  开始变身 msgid: 9936
	public class MsgData_sChangeBegin : MsgData
	{
		public long ID;	// ID
		public uint ChangeID;	// 

		public override void unpack(NetReadBuffer buffer)
		{
			ID = buffer.ReadInt64();
			ChangeID = buffer.ReadUInt32();
		}
	}

	// S->C 服务端广播通知:  结束变身 msgid: 9937
	public class MsgData_sChangeEnd : MsgData
	{
		public long ID;	// ID
		public uint ChangeID;	// 

		public override void unpack(NetReadBuffer buffer)
		{
			ID = buffer.ReadInt64();
			ChangeID = buffer.ReadUInt32();
		}
	}

	// C->S 客户端请求：红颜操作 msgId:3754;
	public class MsgData_cHongyanAct : MsgData
	{
		public int ID;	// 红颜ID
		public int Type;	// 1好感度，2升级, 3自动购买升星升阶;

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(ID);
			buffer.WriteInt32(Type);
		}
	}

	// S->C 服务器返回：激活返回结果 msgId:8756;
	public class MsgData_sHongyanAct : MsgData
	{
		public int ID;	// 红颜ID
		public int Result;	// 请求激活结果 0:成功
		public int State;	// 激活类型 0-未激活 1-激活;
		public int Jiedian;	// 当前节点
		public int JiedianLevel;	// 节点对应的等级

		public override void unpack(NetReadBuffer buffer)
		{
			ID = buffer.ReadInt32();
			Result = buffer.ReadInt32();
			State = buffer.ReadInt32();
			Jiedian = buffer.ReadInt32();
			JiedianLevel = buffer.ReadInt32();
		}
	}

	// C->S 客户端请求：红颜出战类型变化 msgId:3755;
	public class MsgData_cHongyanFight : MsgData
	{
		public int ID;	// 红颜ID
		public int State;	// 1 -未出战 2-已出战

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(ID);
			buffer.WriteInt32(State);
		}
	}

	// S->C 服务器返回：红颜出战类型变化 msgId:8758;
	public class MsgData_sHongyanFight : MsgData
	{
		public int Result;	// 出战状态改变 0-成功 -1-失败
		public int ID;	// 红颜ID
		public int State;	// 1 -未出战 2-已出战

		public override void unpack(NetReadBuffer buffer)
		{
			Result = buffer.ReadInt32();
			ID = buffer.ReadInt32();
			State = buffer.ReadInt32();
		}
	}

	// C->S 请求:红颜使用属性丹 msgId:3804;
	public class MsgData_cBeautyWomanUseAtt : MsgData
	{
		public int ID;	// 红颜ID

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(ID);
		}
	}

	// S->C 服务器返回：升阶返回结果 msgId:8757
	public class MsgData_sBeautyWomanLevelUp : MsgData
	{
		public int ID;	// 红颜ID
		public int result;	// 请求升阶结果 0:成功
		public int State;	// 升阶类型 0-升星 1-升阶
		public int starNum;	// 星数
		public int currentExp;	// 星数经验
		public int gradeNum;	// 等阶
		public int radExp;	// 祝福值

		public override void unpack(NetReadBuffer buffer)
		{
			ID = buffer.ReadInt32();
			result = buffer.ReadInt32();
			State = buffer.ReadInt32();
			starNum = buffer.ReadInt32();
			currentExp = buffer.ReadInt32();
			gradeNum = buffer.ReadInt32();
			radExp = buffer.ReadInt32();
		}
	}

	// S->C 美人信息
	public class MsgData_sBeautyWomanVo : MsgData
	{
		public int ID;	// 红颜ID
		public int actState;	// 激活类型 0-未激活 1-已激活 2-已出战
		public int jiedian;	// 当前节点
		public int jiedianlevel;	// 节点对应的等级
		public int gradeState;	// 升阶类型 0-升星 1-升阶
		public int gradeNum;	// 等阶
		public int starNum;	// 星数
		public int currentExp;	// 星数经验
		public int gradExp;	// 祝福值
		public int count;	// 已使用的数量
		public int fighting;	// 战斗力

		public override void unpack(NetReadBuffer buffer)
		{
			ID = buffer.ReadInt32();
			actState = buffer.ReadInt32();
			jiedian = buffer.ReadInt32();
			jiedianlevel = buffer.ReadInt32();
			gradeState = buffer.ReadInt32();
			gradeNum = buffer.ReadInt32();
			starNum = buffer.ReadInt32();
			currentExp = buffer.ReadInt32();
			gradExp = buffer.ReadInt32();
			count = buffer.ReadInt32();
			fighting = buffer.ReadInt32();
		}
	}

	// S->C 更新红颜战斗力信息  msgid: 9941
	public class MsgData_sBeautyWomanFightingUpdate : MsgData
	{
		public int ID;	// 红颜ID
		public int fighting;	// 红颜战斗力

		public override void unpack(NetReadBuffer buffer)
		{
			ID = buffer.ReadInt32();
			fighting = buffer.ReadInt32();
		}
	}

	// S->C 服务器通知:红颜信息(玩家登陆;每天更新) msgId:8755
	public class MsgData_sBeautyWomanInfo : MsgData
	{
		public int ID;	// 红颜ID
		public uint count;	// 列表数量
		public List<MsgData_sBeautyWomanVo> items = new List<MsgData_sBeautyWomanVo>();	// 美人信息列表

		public override void unpack(NetReadBuffer buffer)
		{
			ID = buffer.ReadInt32();
			count = buffer.ReadUInt32();
			items = new List<MsgData_sBeautyWomanVo>();
			for (int i = 0; i < count; i++)
			{
				MsgData_sBeautyWomanVo __item = new MsgData_sBeautyWomanVo();
				__item.unpack(buffer);
				items.Add(__item);
			}
		}
	}

	// S->C 返回: 修为等级信息 msgId:8804;
	public class MsgData_sBeautyWomanUseAtt : MsgData
	{
		public int result;	// 使用结果 0-成功
		public int ID;	// 红颜ID
		public int count;	// 已使用的数量

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt32();
			ID = buffer.ReadInt32();
			count = buffer.ReadInt32();
		}
	}

	// C->S 请求:神兵进阶 msgId:3250;
	public class MsgData_cMagicWeaponLevelUp : MsgData
	{
		public int autobuy;	// 道具不足时是否自动购买，0自动购买

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(autobuy);
		}
	}

	// C->S 请求:神兵换模型 msgId:3456;
	public class MsgData_cMagicWeaponChangeModel : MsgData
	{
		public int level;	// 请求更换模型的等阶(即配表ID)

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(level);
		}
	}

	// S->C 服务器通知:神兵信息 msgId:8248
	public class MsgData_sMagicWeaponInfo : MsgData
	{
		public int level;	// 神兵等阶
		public int modelLevel;	// 神兵使用模型的等阶(神兵等阶)
		public int proficiency;	// 神兵熟练度
		public int lvlProficiency;	// 熟练度等级
		public int pillNum;	// 属性丹数量
		public int pillNumPercent;	// 资质丹数量
		public int uplevelNum;	// 疲劳值

		public override void unpack(NetReadBuffer buffer)
		{
			level = buffer.ReadInt32();
			modelLevel = buffer.ReadInt32();
			proficiency = buffer.ReadInt32();
			lvlProficiency = buffer.ReadInt32();
			pillNum = buffer.ReadInt32();
			pillNumPercent = buffer.ReadInt32();
			uplevelNum = buffer.ReadInt32();
		}
	}

	// S->C 服务器通知:神兵熟练度 msgId:8249
	public class MsgData_sMagicWeaponProficiency : MsgData
	{
		public int proficiency;	// 神兵熟练度

		public override void unpack(NetReadBuffer buffer)
		{
			proficiency = buffer.ReadInt32();
		}
	}

	// S->C 服务器通知:神兵进阶 msgId:8250
	public class MsgData_sMagicWeaponLevelUp : MsgData
	{
		public sbyte result;	// 0 成功，2 神兵未解锁，3已达到等级上限，4熟练度不够，5金币不够，6道具数量不足
		public int proficiency;	// 神兵熟练度
		public int lvlProficiency;	// 熟练度等级
		public int uplevelNum;	// 疲劳值

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt8();
			proficiency = buffer.ReadInt32();
			lvlProficiency = buffer.ReadInt32();
			uplevelNum = buffer.ReadInt32();
		}
	}

	// S->C 服务器通知:神兵换模型结果 msgId:8456
	public class MsgData_sMagicWeaponChangeModel : MsgData
	{
		public int level;	// 成功时:更换后的模型等阶   失败时:-1等级不够，-2其他

		public override void unpack(NetReadBuffer buffer)
		{
			level = buffer.ReadInt32();
		}
	}

	// C->S 请求UI信息 msgId:3390;
	public class MsgData_cDominateRoute : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// C->S 请求挑战 msgId:3392;
	public class MsgData_cDominateRouteChallenge : MsgData
	{
		public int id;	// 挑战ID

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(id);
		}
	}

	// C->S 请求退出 msgId:3393;
	public class MsgData_cDominateRouteQuit : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// C->S 领取宝箱 msgId:3394;
	public class MsgData_cDominateRouteRequestReward : MsgData
	{
		public int id;	// 挑战ID

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(id);
		}
	}

	// C->S 请求扫荡 msgId:3395
	public class MsgData_cDominateRouteWipe : MsgData
	{
		public int id;	// 扫荡ID
		public int num;	// 扫荡次数

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(id);
			buffer.WriteInt32(num);
		}
	}

	// S->C 返回UI信息 msgId:8390
	public class MsgData_sDominateRouteData : MsgData
	{
		public int enterNum;	// 剩余挑战次数
		public MsgData_sVeilvo[] veillist = new MsgData_sVeilvo[11];	// 剩余挑战次数
		public uint count;	// 数量
		public List<MsgData_sStagevo> items = new List<MsgData_sStagevo>();	// 数量

		public override void unpack(NetReadBuffer buffer)
		{
			enterNum = buffer.ReadInt32();
			for (int i = 0; i < 11; i++)
			{
				MsgData_sVeilvo __item = new MsgData_sVeilvo();
				__item.unpack(buffer);
				veillist[i]=__item;
			}
			count = buffer.ReadUInt32();
			items = new List<MsgData_sStagevo>();
			for (int i = 0; i < count; i++)
			{
				MsgData_sStagevo __item = new MsgData_sStagevo();
				__item.unpack(buffer);
				items.Add(__item);
			}
		}
	}

	// S->C 刷新 msgId:8391
	public class MsgData_sDominateRouteUpDate : MsgData
	{
		public int num;	// 剩余次数
		public int state;	// 0 没扫荡 1在扫荡
		public long time;	// 扫荡剩余时间0 待领取倒计时
		public int id;	// id
		public int rewardType;	// 0:未开启1：可领取2：已领取

		public override void unpack(NetReadBuffer buffer)
		{
			num = buffer.ReadInt32();
			state = buffer.ReadInt32();
			time = buffer.ReadInt64();
			id = buffer.ReadInt32();
			rewardType = buffer.ReadInt32();
		}
	}

	// S->C 返回挑战 msgId:8392
	public class MsgData_sBackDominateRouteChallenge : MsgData
	{
		public int result;	// 进入结果返回: 0失败 1成功
		public int id;	// 返回进入挑战

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt32();
			id = buffer.ReadInt32();
		}
	}

	// S->C 返回退出 msgId:8393;
	public class MsgData_sBackDominateRouteQuit : MsgData
	{
		public int result;	// 进入结果返回: 0失败 1成功

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt32();
		}
	}

	// S->C 返回追踪信息 msgId:8394;
	public class MsgData_sBackDominateRouteInfo : MsgData
	{
		public int num;	// 待定~~~

		public override void unpack(NetReadBuffer buffer)
		{
			num = buffer.ReadInt32();
		}
	}

	// S->C 返回扫荡 msgId:8395
	public class MsgData_sBackDominateRouteWipe : MsgData
	{
		public int result;	// 进入结果返回: 0失败 1成功
		public int id;	// 返回进入挑战
		public int num;	// 待定返回扫荡次数

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt32();
			id = buffer.ReadInt32();
			num = buffer.ReadInt32();
		}
	}

	// S->C 返回购买精力 msgId:8396;
	public class MsgData_sBackDominateRouteVigor : MsgData
	{
		public int result;	// 进入结果返回: 0失败 1成功

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt32();
		}
	}

	// S->C 返回领取宝箱奖励 msgId:8397;
	public class MsgData_sBackDominateRouteBoxReward : MsgData
	{
		public int result;	// 购买领取结果: 0失败 1成功
		public int id;	// 返回领取id

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt32();
			id = buffer.ReadInt32();
		}
	}

	// S->C 返回通关 msgId:8398;
	public class MsgData_sBackDominateRouteEnd : MsgData
	{
		public int level;	// 评定
		public int result;	// 购买领取结果: 0失败 1成功

		public override void unpack(NetReadBuffer buffer)
		{
			level = buffer.ReadInt32();
			result = buffer.ReadInt32();
		}
	}

	// S->C 返回扫荡结束 msgId:8399
	public class MsgData_sBackDominateRouteMopupEnd : MsgData
	{
		public int id;	// 扫荡完成的ID
		public int result;	// 扫荡结果
		public int num;	// 扫荡次数

		public override void unpack(NetReadBuffer buffer)
		{
			id = buffer.ReadInt32();
			result = buffer.ReadInt32();
			num = buffer.ReadInt32();
		}
	}

	// C->S 请求:圣盾进阶 msgId:3724;
	public class MsgData_cPiFengLevelUp : MsgData
	{
		public int autobuy;	// 道具不足时是否自动购买, 1自动购买
		public int uptype;	//  1激活，2升级;

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(autobuy);
			buffer.WriteInt32(uptype);
		}
	}

	// C->S 请求:圣盾换模型 msgId:3725;
	public class MsgData_cPiFengChangeModel : MsgData
	{
		public int level;	// 请求更换圣盾等阶(即配表ID)

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(level);
		}
	}

	// S->C 服务器通知:圣盾信息 msgId:8723
	public class MsgData_sPiFengInfo : MsgData
	{
		public int level;	// 圣盾等阶
		public int wish;	// 祝福值
		public int useLevel;	// 使用圣盾
		public int pillNum;	// 属性丹数量
		public int pillNumPercent;	// 属性丹数量(加百分比)
		public int star;	// 等阶星数

		public override void unpack(NetReadBuffer buffer)
		{
			level = buffer.ReadInt32();
			wish = buffer.ReadInt32();
			useLevel = buffer.ReadInt32();
			pillNum = buffer.ReadInt32();
			pillNumPercent = buffer.ReadInt32();
			star = buffer.ReadInt32();
		}
	}

	// S->C 服务器通知:圣盾进阶 msgId:8724
	public class MsgData_sPiFengLevelUp : MsgData
	{
		public int result;	// 0 0:成功(成功获得祝福值),1:其他错误, 2:圣盾未解锁, 3:已达等级上限, 4:金币不够, 5:道具数量不足，8:自动购买失败，9:道具扣除失败，10:金币扣除失败
		public int level;	// 圣盾等阶
		public int wish;	// 祝福值
		public int star;	// 星数

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt32();
			level = buffer.ReadInt32();
			wish = buffer.ReadInt32();
			star = buffer.ReadInt32();
		}
	}

	// S->C 服务器通知:圣盾换模型结果 msgId:8725
	public class MsgData_sPiFengChangeModel : MsgData
	{
		public int result;	// 0:成功 1:失败
		public int level;	// 圣盾等阶

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt32();
			level = buffer.ReadInt32();
		}
	}

	// C->S 请求:拥有王冠信息 msgId:4936;
	public class MsgData_cReqCrownInfo : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// S->C 返回:王冠数据
	public class MsgData_sCrownInfo : MsgData
	{
		public int ID;	// 王冠ID
		public int SkillID;	// 技能ID

		public override void unpack(NetReadBuffer buffer)
		{
			ID = buffer.ReadInt32();
			SkillID = buffer.ReadInt32();
		}
	}

	// S->C 返回:拥有王冠信息 msgId:9942;
	public class MsgData_sResCrownInfo : MsgData
	{
		public short Size;	// 数据长度
		public List<MsgData_sCrownInfo> List = new List<MsgData_sCrownInfo>();	// 数据

		public override void unpack(NetReadBuffer buffer)
		{
			Size = buffer.ReadInt16();
			List = new List<MsgData_sCrownInfo>();
			for (int i = 0; i < Size; i++)
			{
				MsgData_sCrownInfo __item = new MsgData_sCrownInfo();
				__item.unpack(buffer);
				List.Add(__item);
			}
		}
	}

	// C->S 请求:王冠技能升级 msgId:4937;
	public class MsgData_cReqCrownSkillUp : MsgData
	{
		public int ID;	// 王冠ID
		public int SkillID;	// 技能ID

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(ID);
			buffer.WriteInt32(SkillID);
		}
	}

	// S->C 返回:王冠技能升级 msgId:9943
	public class MsgData_sResCrownSkillUp : MsgData
	{
		public short Result;	// 升级结果反馈：0成功，其它失败
		public int ID;	// 王冠ID
		public int SkillID;	// 技能ID

		public override void unpack(NetReadBuffer buffer)
		{
			Result = buffer.ReadInt16();
			ID = buffer.ReadInt32();
			SkillID = buffer.ReadInt32();
		}
	}

	// S->C 返回:王冠激活 msgId:9944
	public class MsgData_sResCrownActive : MsgData
	{
		public int ID;	// 王冠ID
		public int SkillID;	// 技能ID

		public override void unpack(NetReadBuffer buffer)
		{
			ID = buffer.ReadInt32();
			SkillID = buffer.ReadInt32();
		}
	}

	// C->S 客户端请求进入王冠副本 msgId:4946
	public class MsgData_cReqCrownFight : MsgData
	{
		public int id;	// crownID

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(id);
		}
	}

	// C->S 客户端请求离开王冠副本 msgId:4947
	public class MsgData_cReqCrownFightLeave : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// S->C 返回:王冠副本结算 msgId:9984
	public class MsgData_sResCrownFightResult : MsgData
	{
		public int Result;	// 结果 0：成功，-1：失败

		public override void unpack(NetReadBuffer buffer)
		{
			Result = buffer.ReadInt32();
		}
	}

	// C->S 客户端请求进入个人BOSS副本 msgId:3561
	public class MsgData_cEnterPersonalBoss : MsgData
	{
		public int id;	// bossID

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(id);
		}
	}

	// S->C BOSS信息
	public class MsgData_sPersonalBossVO : MsgData
	{
		public int id;	// BOSSID
		public int num;	// 已进入次数
		public int isfirst;	// 是否已每日首通 0已首通
		public int remainTime;	// 大于0剩余秒数 等于0到期 -1错误
		public int passFlag;	// 通关标识 0未通关 1通关过
		public int leftNum;	// 剩余次数
		public int m_nBuyNum;	// 购买次数
		public long m_i8NextTimeStamp;	// //下次进入的时间戳

		public override void unpack(NetReadBuffer buffer)
		{
			id = buffer.ReadInt32();
			num = buffer.ReadInt32();
			isfirst = buffer.ReadInt32();
			remainTime = buffer.ReadInt32();
			passFlag = buffer.ReadInt32();
			leftNum = buffer.ReadInt32();
			m_nBuyNum = buffer.ReadInt32();
			m_i8NextTimeStamp = buffer.ReadInt64();
		}
	}

	// S->C 服务器返回BOSS挑战列表 msgId:8560
	public class MsgData_sPersonalBossList : MsgData
	{
		public int itemEnterNum;	// 道具进入次数(已进入次数)
		public uint count;	// 数量
		public List<MsgData_sPersonalBossVO> items = new List<MsgData_sPersonalBossVO>();	// 数量

		public override void unpack(NetReadBuffer buffer)
		{
			itemEnterNum = buffer.ReadInt32();
			count = buffer.ReadUInt32();
			items = new List<MsgData_sPersonalBossVO>();
			for (int i = 0; i < count; i++)
			{
				MsgData_sPersonalBossVO __item = new MsgData_sPersonalBossVO();
				__item.unpack(buffer);
				items.Add(__item);
			}
		}
	}

	// S->C 服务器返回进入个人BOSS结果 msgId:8561
	public class MsgData_sBackEnterResultPersonalBoss : MsgData
	{
		public int result;	// 结果 0 成功 -1等级不足 -2次数不足 -3非VIP -4组队 -5异常 -6时间没到
		public int id;	//  id 进入的ID
		public int type;	//  进入形式 是否是道具 0是道具进入
		public int num;	// 已进入免费次数
		public int itemEnterNum;	// 已进入道具次数

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt32();
			id = buffer.ReadInt32();
			type = buffer.ReadInt32();
			num = buffer.ReadInt32();
			itemEnterNum = buffer.ReadInt32();
		}
	}

	// S->C 服务器:退出个人BOSS结果 msgId:8562
	public class MsgData_sBackQuitPersonalBoss : MsgData
	{
		public int result;	// 结果 0 成功 -1失败

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt32();
		}
	}

	// S->C 服务器:挑战个人BOSS结果 msgId:8563
	public class MsgData_sPersonalBossResult : MsgData
	{
		public int result;	// 结果 0 成功 -1失败
		public int isfirst;	// 结果 0 是

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt32();
			isfirst = buffer.ReadInt32();
		}
	}

	// C->S 客户端请求通缉列表 msgId:3164
	public class MsgData_cTongJiInfo : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// C->S 难度刷新 msgId:3165;
	public class MsgData_cTongJiLvlRefresh : MsgData
	{
		public int type;	//  刷新类型 0:客户端申请列表 1:银两 2:元宝;

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(type);
		}
	}

	// C->S 接受通缉任务 msgId:3166;
	public class MsgData_cAcceptTongJi : MsgData
	{
		public int id;	//  通缉id;

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(id);
		}
	}

	// C->S 领取通缉奖励 msgId:3168;
	public class MsgData_cGetTongJiReward : MsgData
	{
		public int id;	//  通缉id;
		public int type;	//  类型 0、普通，1、银两，2、元宝;

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(id);
			buffer.WriteInt32(type);
		}
	}

	// C->S 放弃通缉 msgId:3169;
	public class MsgData_cGiveupTongJi : MsgData
	{
		public int id;	//  通缉id;

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(id);
		}
	}

	// C->S 获取通缉宝箱奖励 msgId:3171;
	public class MsgData_cGetTongJiBox : MsgData
	{
		public int boxId;	//  宝箱等级

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(boxId);
		}
	}

	// C->S 刷新悬赏状态 msgId:3522;
	public class MsgData_cTongJiRefreshState : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// S->C 已领奖宝箱列表
	public class MsgData_sScoreBoxVO : MsgData
	{
		public int boxId;	// 宝箱id;

		public override void unpack(NetReadBuffer buffer)
		{
			boxId = buffer.ReadInt32();
		}
	}

	// S->C 服务端通知:返回通缉信息 msgId:8164;
	public class MsgData_sTongJiInfo : MsgData
	{
		public int Group;	//  当前选择的通缉组id;
		public int id;	//  当前可选择的通缉id;
		public int state;	//  活动状态 0:未接受,1已接受,2可领奖,3已领奖;
		public int finishCount;	//  今日完成次数;
		public int buyCount;	//  已购买次数
		public int curScore;	//  当前积分;
		public int coolTime;	//  剩余冷却时间;
		public uint count;	// 列表数量
		public List<MsgData_sScoreBoxVO> boxList = new List<MsgData_sScoreBoxVO>();	// 已领奖宝箱列表

		public override void unpack(NetReadBuffer buffer)
		{
			Group = buffer.ReadInt32();
			id = buffer.ReadInt32();
			state = buffer.ReadInt32();
			finishCount = buffer.ReadInt32();
			buyCount = buffer.ReadInt32();
			curScore = buffer.ReadInt32();
			coolTime = buffer.ReadInt32();
			count = buffer.ReadUInt32();
			boxList = new List<MsgData_sScoreBoxVO>();
			for (int i = 0; i < count; i++)
			{
				MsgData_sScoreBoxVO __item = new MsgData_sScoreBoxVO();
				__item.unpack(buffer);
				boxList.Add(__item);
			}
		}
	}

	// S->C 服务器通知：返回难度刷新 msgId:8165;
	public class MsgData_sTongJiLvlRefreshResult : MsgData
	{
		public int result;	// 结果 0 成功 -1失败
		public int id;	// 当前可选择的通缉id;
		public int Group;	// 当前选择的通缉组id;

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt32();
			id = buffer.ReadInt32();
			Group = buffer.ReadInt32();
		}
	}

	// S->C 服务器通知：返回接受通缉任务 msgId:8166;
	public class MsgData_sAcceptTongJiResult : MsgData
	{
		public int id;	// 通缉id;

		public override void unpack(NetReadBuffer buffer)
		{
			id = buffer.ReadInt32();
		}
	}

	// S->C 服务器通知：通缉活动可领奖 msgId:8167;
	public class MsgData_sFinishTongJi : MsgData
	{
		public int id;	// 通缉id;

		public override void unpack(NetReadBuffer buffer)
		{
			id = buffer.ReadInt32();
		}
	}

	// S->C 服务器返回结果：返回领取通缉奖励结果 msgId:8168;
	public class MsgData_sGetTongJiReward : MsgData
	{
		public int result;	// 结果 0 成功 1银两不足  2元宝不足
		public int curScore;	// 当前积分;
		public int id;	// 通缉id

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt32();
			curScore = buffer.ReadInt32();
			id = buffer.ReadInt32();
		}
	}

	// S->C 服务器通知：返回放弃通缉结果 msgId:8169;
	public class MsgData_sGiveupTongJiResult : MsgData
	{
		public int result;	// 结果 0 成功
		public int id;	// 通缉id

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt32();
			id = buffer.ReadInt32();
		}
	}

	// S->C 服务器通知：自动刷新通缉列表 msgId:8170;
	public class MsgData_sRefreshTongJiList : MsgData
	{
		public int Group;	// 当前选择的通缉组id;
		public int id;	// 当前可选择的通缉id

		public override void unpack(NetReadBuffer buffer)
		{
			Group = buffer.ReadInt32();
			id = buffer.ReadInt32();
		}
	}

	// S->C 服务器通知：返回获取通缉宝箱结果 msgId:8171;
	public class MsgData_sGetTongJiBoxResult : MsgData
	{
		public int result;	// 结果 0 成功
		public int boxId;	// 宝箱id

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt32();
			boxId = buffer.ReadInt32();
		}
	}

	// S->C 服务器通知：刷新悬赏状态结果 msgId:8522;
	public class MsgData_sTongJiRefreshState : MsgData
	{
		public int result;	// 结果 0.成功  1.玩家等级/vip等级不足  2.通缉任务状态错误,  3.元宝不足

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt32();
		}
	}

	// C->S 请求打开邮件 msgId:2033;
	public class MsgData_cOpenMail : MsgData
	{
		public long id;	//  邮件id

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(id);
		}
	}

	// C->S 邮件id
	public class MsgData_cMailReqItemVo : MsgData
	{
		public long id;	//  邮件id

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(id);
		}
	}

	// C->S 请求领取附件 msgId:2034
	public class MsgData_cGetMailItem : MsgData
	{
		public uint count;	// 数量
		public List<MsgData_cMailReqItemVo> items = new List<MsgData_cMailReqItemVo>();	// 列表

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteUInt32(count);
			for (int i = 0; i < count; i++)
			{
				MsgData_cMailReqItemVo __item = items[i];
				__item.pack(buffer);
			}
		}
	}

	// C->S 邮件id
	public class MsgData_cReqMailDelVo : MsgData
	{
		public long id;	//  邮件id

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(id);
		}
	}

	// C->S 请求删除邮件 msgId:2035;
	public class MsgData_cDelMail : MsgData
	{
		public uint count;	// 数量
		public List<MsgData_cReqMailDelVo> items = new List<MsgData_cReqMailDelVo>();	// 列表

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteUInt32(count);
			for (int i = 0; i < count; i++)
			{
				MsgData_cReqMailDelVo __item = items[i];
				__item.pack(buffer);
			}
		}
	}

	// S->C 邮件信息
	public class MsgData_sMailVo : MsgData
	{
		public long id;	// 邮件id
		public sbyte read;	// 是否读过 0 - 未读， 1 - 已读
		public sbyte item;	// 是否领取过附件0 - 没有附件， 1 - 未领取附件， 2 - 已领取附件
		public long sendTime;	// 发件时间
		public long leftTime;	// 剩余时间
		public byte[] mailtitle = new byte[50];	// 邮件标题
		public int mailTxtId;	// 配表id

		public override void unpack(NetReadBuffer buffer)
		{
			id = buffer.ReadInt64();
			read = buffer.ReadInt8();
			item = buffer.ReadInt8();
			sendTime = buffer.ReadInt64();
			leftTime = buffer.ReadInt64();
			buffer.ReadBytes(mailtitle);
			mailTxtId = buffer.ReadInt32();
		}
	}

	// S->C 返回邮件列表 msgId:7032
	public class MsgData_sGetMailResult : MsgData
	{
		public uint count;	// 列表数量
		public List<MsgData_sMailVo> items = new List<MsgData_sMailVo>();	// 邮件列表

		public override void unpack(NetReadBuffer buffer)
		{
			count = buffer.ReadUInt32();
			items = new List<MsgData_sMailVo>();
			for (int i = 0; i < count; i++)
			{
				MsgData_sMailVo __item = new MsgData_sMailVo();
				__item.unpack(buffer);
				items.Add(__item);
			}
		}
	}

	// S->C 物品列表
	public class MsgData_sMailItemVo : MsgData
	{
		public int itemid;	// 附件物品id
		public long itemcount;	// 附件物品数量

		public override void unpack(NetReadBuffer buffer)
		{
			itemid = buffer.ReadInt32();
			itemcount = buffer.ReadInt64();
		}
	}

	// S->C 返回打开邮件 msgId:7033
	public class MsgData_sOpenMailResult : MsgData
	{
		public long id;	// 邮件id
		public sbyte item;	// 是否领取过附件0 - 没有附件， 1 - 未领取附件， 2 - 已领取附件
		public byte[] contnet = new byte[512];	// 邮件内容 type=1 'param1:type1,param2:type2
		public MsgData_sMailItemVo[] items = new MsgData_sMailItemVo[8];	// 邮件列表

		public override void unpack(NetReadBuffer buffer)
		{
			id = buffer.ReadInt64();
			item = buffer.ReadInt8();
			buffer.ReadBytes(contnet);
			for (int i = 0; i < 8; i++)
			{
				MsgData_sMailItemVo __item = new MsgData_sMailItemVo();
				__item.unpack(buffer);
				items[i]=__item;
			}
		}
	}

	// S->C  
	public class MsgData_sMailRespItemVo : MsgData
	{
		public long id;	// 邮件id
		public sbyte result;	// 领取邮件附件结果 0- 成功 1-失败

		public override void unpack(NetReadBuffer buffer)
		{
			id = buffer.ReadInt64();
			result = buffer.ReadInt8();
		}
	}

	// S->C 请求领取附件返回 msgId:7034
	public class MsgData_sGetMailItemResult : MsgData
	{
		public uint count;	// 列表数量
		public List<MsgData_sMailRespItemVo> items = new List<MsgData_sMailRespItemVo>();	// 附件列表

		public override void unpack(NetReadBuffer buffer)
		{
			count = buffer.ReadUInt32();
			items = new List<MsgData_sMailRespItemVo>();
			for (int i = 0; i < count; i++)
			{
				MsgData_sMailRespItemVo __item = new MsgData_sMailRespItemVo();
				__item.unpack(buffer);
				items.Add(__item);
			}
		}
	}

	// S->C  
	public class MsgData_sRespMailDelVo : MsgData
	{
		public long id;	// 邮件id

		public override void unpack(NetReadBuffer buffer)
		{
			id = buffer.ReadInt64();
		}
	}

	// S->C 请求删除邮件返回 msgId:7035
	public class MsgData_sDelMail : MsgData
	{
		public uint count;	// 列表数量
		public List<MsgData_sRespMailDelVo> items = new List<MsgData_sRespMailDelVo>();	// 列表

		public override void unpack(NetReadBuffer buffer)
		{
			count = buffer.ReadUInt32();
			items = new List<MsgData_sRespMailDelVo>();
			for (int i = 0; i < count; i++)
			{
				MsgData_sRespMailDelVo __item = new MsgData_sRespMailDelVo();
				__item.unpack(buffer);
				items.Add(__item);
			}
		}
	}

	// S->C 邮件提醒 msgId:7036
	public class MsgData_sNotifyMail : MsgData
	{
		public int count;	// 邮件数量

		public override void unpack(NetReadBuffer buffer)
		{
			count = buffer.ReadInt32();
		}
	}

	// S->C 消息包描述
	public class MsgData_sFramePacketDesc : MsgData
	{
		public ushort cmd;	// 命令码
		public ushort len;	// body长度
		public List<byte> body = new List<byte>();	// body

		public override void unpack(NetReadBuffer buffer)
		{
			cmd = buffer.ReadUInt16();
			len = buffer.ReadUInt16();
			buffer.ReadBytes(body, (int)len);
		}
	}

	// S->C scene server每帧消息包 msgId:9945
	public class MsgData_sFrameMsg : MsgData
	{
		public ushort len;	// 数据长度
		public ushort packet_count;	// 数据长度
		public List<byte> content = new List<byte>();	// 内容

		public override void unpack(NetReadBuffer buffer)
		{
			len = buffer.ReadUInt16();
			packet_count = buffer.ReadUInt16();
			buffer.ReadBytes(content, (int)len);
		}
	}

	// C->S 请求退出竞技场 msgId:3192;
	public class MsgData_cReqExitArena : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// S->C 竞技场玩家
	public class MsgData_sResArenaMemVo : MsgData
	{
		public long RoleID;	// 玩家id
		public byte[] Name = new byte[32];	// 角色名字
		public long Power;	// 战斗力
		public int Level;	// 玩家等级
		public int Job;	// 职业
		public int Dress;	// 衣服
		public int Weapon;	// 武器
		public int FashionHead;	// 时装头
		public int FashionWeapon;	// 时装武器
		public int FashionDress;	// 时装衣服
		public int WuHunID;	// 灵兽ID
		public int ShenBing;	// 神兵ID
		public int Gender;	// 性别
		public int Icon;	// 头像
		public int Wing;	// 翅膀
		public int SuitFlag;	// 套装标识
		public double Atk;	// atk
		public double HP;	// hp
		public double SubDef;	// subdef
		public double Def;	// def
		public double Cri;	// cri
		public double CriValue;	// crivalue
		public double AbsAtk;	// absatk
		public double DefCri;	// defcri
		public double SubCri;	// subcri
		public double DmgSub;	// dmgsub
		public double DmgAdd;	// dmgadd
		public int[] SkillIDs = new int[10];	// 技能

		public override void unpack(NetReadBuffer buffer)
		{
			RoleID = buffer.ReadInt64();
			buffer.ReadBytes(Name);
			Power = buffer.ReadInt64();
			Level = buffer.ReadInt32();
			Job = buffer.ReadInt32();
			Dress = buffer.ReadInt32();
			Weapon = buffer.ReadInt32();
			FashionHead = buffer.ReadInt32();
			FashionWeapon = buffer.ReadInt32();
			FashionDress = buffer.ReadInt32();
			WuHunID = buffer.ReadInt32();
			ShenBing = buffer.ReadInt32();
			Gender = buffer.ReadInt32();
			Icon = buffer.ReadInt32();
			Wing = buffer.ReadInt32();
			SuitFlag = buffer.ReadInt32();
			Atk = buffer.ReadDouble();
			HP = buffer.ReadDouble();
			SubDef = buffer.ReadDouble();
			Def = buffer.ReadDouble();
			Cri = buffer.ReadDouble();
			CriValue = buffer.ReadDouble();
			AbsAtk = buffer.ReadDouble();
			DefCri = buffer.ReadDouble();
			SubCri = buffer.ReadDouble();
			DmgSub = buffer.ReadDouble();
			DmgAdd = buffer.ReadDouble();
			for (int i = 0; i < 10; i++)
			{
				int __item = buffer.ReadInt32();
				SkillIDs[i]=__item;
			}
		}
	}

	// S->C 服务器通知进入竞技场 msgId:8192;
	public class MsgData_sResEnterArena : MsgData
	{
		public long guidVirtualPlayer;	// 虚拟玩家ID
		public long guidMoShen;	// 魔神ID

		public override void unpack(NetReadBuffer buffer)
		{
			guidVirtualPlayer = buffer.ReadInt64();
			guidMoShen = buffer.ReadInt64();
		}
	}

	// C->S 请求当前角色竞技场信息 msgId:2100;
	public class MsgData_cReqMeArenaInfo : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// S->C 服务器返回当前角色竞技场信息 msgId:7100;
	public class MsgData_sResMeArenaInfo : MsgData
	{
		public int Rank;	// 排名
		public int RankAt0;	// 0点排名
		public int ChallengeTimes;	// 挑战次数
		public int CD;	// 冷却时间
		public int IsRewarded;	// 1未领取，1领取
		public int WinTimes;	// 连胜次数
		public int TotalMoney;	// 整点在线累计奖励金币
		public int TotalHonor;	// 整点在线累计奖励荣誉
		public int MaxChallengeTimes;	// 挑战最大次数
		public long RewardTime;	// 领取奖励时间

		public override void unpack(NetReadBuffer buffer)
		{
			Rank = buffer.ReadInt32();
			RankAt0 = buffer.ReadInt32();
			ChallengeTimes = buffer.ReadInt32();
			CD = buffer.ReadInt32();
			IsRewarded = buffer.ReadInt32();
			WinTimes = buffer.ReadInt32();
			TotalMoney = buffer.ReadInt32();
			TotalHonor = buffer.ReadInt32();
			MaxChallengeTimes = buffer.ReadInt32();
			RewardTime = buffer.ReadInt64();
		}
	}

	// C->S 请求挑战列表 msgId:2101;
	public class MsgData_cReqArenaList : MsgData
	{
		public int Type;	// 类型，0-123名，1-挑战对象

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(Type);
		}
	}

	// S->C 竞技场玩家
	public class MsgData_sResArenaRoleVo : MsgData
	{
		public long RoleID;	// 玩家id
		public byte[] Name = new byte[32];	// 角色名字
		public long Power;	// 战斗力
		public int Rank;	// 玩家排行
		public int Job;	// 职业
		public int Weapon;	// 衣服
		public int Dress;	// 武器
		public int FashionHead;	// 时装头
		public int FashionWeapon;	// 时装武器
		public int FashionDress;	// 时装衣服
		public int WuHunID;	// 灵兽ID
		public int Wing;	// 翅膀
		public int SuitFlag;	// 套装标识
		public int ShenBing;	// 神兵

		public override void unpack(NetReadBuffer buffer)
		{
			RoleID = buffer.ReadInt64();
			buffer.ReadBytes(Name);
			Power = buffer.ReadInt64();
			Rank = buffer.ReadInt32();
			Job = buffer.ReadInt32();
			Weapon = buffer.ReadInt32();
			Dress = buffer.ReadInt32();
			FashionHead = buffer.ReadInt32();
			FashionWeapon = buffer.ReadInt32();
			FashionDress = buffer.ReadInt32();
			WuHunID = buffer.ReadInt32();
			Wing = buffer.ReadInt32();
			SuitFlag = buffer.ReadInt32();
			ShenBing = buffer.ReadInt32();
		}
	}

	// S->C 返回挑战列表 msgId:7101;
	public class MsgData_sResArenaList : MsgData
	{
		public int Type;	// 类型，0-123名，1-挑战对象
		public uint Size;	// 长度
		public List<MsgData_sResArenaRoleVo> List = new List<MsgData_sResArenaRoleVo>();	// 列表

		public override void unpack(NetReadBuffer buffer)
		{
			Type = buffer.ReadInt32();
			Size = buffer.ReadUInt32();
			List = new List<MsgData_sResArenaRoleVo>();
			for (int i = 0; i < Size; i++)
			{
				MsgData_sResArenaRoleVo __item = new MsgData_sResArenaRoleVo();
				__item.unpack(buffer);
				List.Add(__item);
			}
		}
	}

	// C->S 请求挑战 msgId:2102;
	public class MsgData_cReqArenaChallenge : MsgData
	{
		public int Rank;	// 挑战排名

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(Rank);
		}
	}

	// S->C 返回挑战 msgId:7102;
	public class MsgData_sResArenaChallenge : MsgData
	{
		public int Result;	// 0成功，1失败
		public int Rank;	// 挑战排名
		public int Exp;	// 经验获得
		public int Honor;	// 荣誉获得
		public int Diamond;	// 钻石获得

		public override void unpack(NetReadBuffer buffer)
		{
			Result = buffer.ReadInt32();
			Rank = buffer.ReadInt32();
			Exp = buffer.ReadInt32();
			Honor = buffer.ReadInt32();
			Diamond = buffer.ReadInt32();
		}
	}

	// C->S 请求领取奖励 msgId:2103;
	public class MsgData_cReqArenaReward : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// S->C 返回领取奖励 msgId:7103;
	public class MsgData_sResArenaReward : MsgData
	{
		public int Result;	// 0成功，1失败

		public override void unpack(NetReadBuffer buffer)
		{
			Result = buffer.ReadInt32();
		}
	}

	// C->S 请求竞技场战报 msgId:2104;
	public class MsgData_cReqArenaRecord : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// S->C 战报信息
	public class MsgData_sResArenaRecordItem : MsgData
	{
		public int ID;	// ID
		public long Time;	// 时间
		public byte[] Param = new byte[64];	// 参数

		public override void unpack(NetReadBuffer buffer)
		{
			ID = buffer.ReadInt32();
			Time = buffer.ReadInt64();
			buffer.ReadBytes(Param);
		}
	}

	// S->C 返回竞技场战报 msgId:7104;
	public class MsgData_sResArenaRecord : MsgData
	{
		public uint Size;	// 长度
		public List<MsgData_sResArenaRecordItem> List = new List<MsgData_sResArenaRecordItem>();	// 列表

		public override void unpack(NetReadBuffer buffer)
		{
			Size = buffer.ReadUInt32();
			List = new List<MsgData_sResArenaRecordItem>();
			for (int i = 0; i < Size; i++)
			{
				MsgData_sResArenaRecordItem __item = new MsgData_sResArenaRecordItem();
				__item.unpack(buffer);
				List.Add(__item);
			}
		}
	}

	// C->S 请求购买竞技场次数 msgId:2105;
	public class MsgData_cReqBuyArenaTimes : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// S->C 返回购买竞技场次数 msgId:7105;
	public class MsgData_sResBuyArenaTimes : MsgData
	{
		public int Result;	// 0成功，1失败,2购买上限
		public int Times;	// 次数

		public override void unpack(NetReadBuffer buffer)
		{
			Result = buffer.ReadInt32();
			Times = buffer.ReadInt32();
		}
	}

	// C->S 请求购买竞技场CD msgId:2106;
	public class MsgData_cReqBuyArenaCD : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// S->C 返回购买竞技场CD msgId:7106;
	public class MsgData_sResBuyArenaCD : MsgData
	{
		public int Result;	// 0成功，1失败
		public int CD;	// CD

		public override void unpack(NetReadBuffer buffer)
		{
			Result = buffer.ReadInt32();
			CD = buffer.ReadInt32();
		}
	}

	// S->C 返回竞技场排名变化CD msgId:7909;
	public class MsgData_sResArenaRankChange : MsgData
	{
		public int Flag;	// 0排名改变, 其它:未变化

		public override void unpack(NetReadBuffer buffer)
		{
			Flag = buffer.ReadInt32();
		}
	}

	// C->S 客户端发送对象列表给服务器 msgId:4935;
	public class MsgData_cSkillTargets : MsgData
	{
		public int SkillID;	// skill id
		public int EffectID;	// effect id
		public byte TargetCount;	// effect id
		public long[] TargetList = new long[10];	// effect id

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(SkillID);
			buffer.WriteInt32(EffectID);
			buffer.WriteUInt8(TargetCount);
			for (int i = 0; i < 10; i++)
			{
				buffer.WriteInt64(TargetList[i]);
			}
		}
	}

	// C->S  客户端请求：官职升级 msgId:3688;
	public class MsgData_cGuanZhiLevelUp : MsgData
	{
		public int type;	// 1上报，2升级

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(type);
		}
	}

	// C->S  客户端请求：官职信息 msgId:3689;
	public class MsgData_cGuanZhiInfo : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// C->S   请求服务器功勋 msgId:3810;
	public class MsgData_cInterServiceContValue : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// S->C 服务器返回:官职升级 msgId:8688;
	public class MsgData_sGuanZhiLevelUp : MsgData
	{
		public sbyte result;	// 0成功，1失败
		public int id;	// 官职ID;;
		public int val;	// 已上报功勋值;

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt8();
			id = buffer.ReadInt32();
			val = buffer.ReadInt32();
		}
	}

	// S->C 服务器返回:官职信息 msgId:8689;
	public class MsgData_sGuanZhiInfo : MsgData
	{
		public int id;	// 官职ID;;
		public int val;	// 已上报功勋值;

		public override void unpack(NetReadBuffer buffer)
		{
			id = buffer.ReadInt32();
			val = buffer.ReadInt32();
		}
	}

	// S->C 返回服务器功勋 msgId:8810;
	public class MsgData_sInterServiceContValue : MsgData
	{
		public int contValue;	// 功勋数量;

		public override void unpack(NetReadBuffer buffer)
		{
			contValue = buffer.ReadInt32();
		}
	}

	// C->S  客户端请求：活跃奖励 msgId:3262;
	public class MsgData_cHuoYueReward : MsgData
	{
		public int id;	// 活跃奖励id;

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(id);
		}
	}

	// S->C 返回获取活跃奖励结果 msgId:8262;
	public class MsgData_sHuoYueReward : MsgData
	{
		public int result;	// 0成功，1失败
		public int id;	// 活跃任务id;

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt32();
			id = buffer.ReadInt32();
		}
	}

	// S->C 返回活跃度任务完成一次 msgId:8261;
	public class MsgData_sHuoYueDuFinish : MsgData
	{
		public int id;	// 活跃任务id;
		public int num;	// 该活跃任务完成总数量;
		public int vitality;	// 当天活跃度总数;

		public override void unpack(NetReadBuffer buffer)
		{
			id = buffer.ReadInt32();
			num = buffer.ReadInt32();
			vitality = buffer.ReadInt32();
		}
	}

	// S->C 已领取活跃度奖励列表
	public class MsgData_sHuoYueDuRewardVo : MsgData
	{
		public int id;	// 已领取奖励id 

		public override void unpack(NetReadBuffer buffer)
		{
			id = buffer.ReadInt32();
		}
	}

	// S->C 已领取活跃度奖励列表
	public class MsgData_sHuoYueDuTaskVo : MsgData
	{
		public int id;	// 活跃任务id 
		public int num;	// 活跃度完成数 

		public override void unpack(NetReadBuffer buffer)
		{
			id = buffer.ReadInt32();
			num = buffer.ReadInt32();
		}
	}

	// S->C 返回活跃度信息 msgId:8260;
	public class MsgData_sHuoYueDu : MsgData
	{
		public int vitality;	// 总功勋值;
		public MsgData_sHuoYueDuRewardVo[] rewardList = new MsgData_sHuoYueDuRewardVo[10];	// 已领取活跃度奖励列表
		public uint count;	// 长度
		public List<MsgData_sHuoYueDuTaskVo> list = new List<MsgData_sHuoYueDuTaskVo>();	// 活跃度列表

		public override void unpack(NetReadBuffer buffer)
		{
			vitality = buffer.ReadInt32();
			for (int i = 0; i < 10; i++)
			{
				MsgData_sHuoYueDuRewardVo __item = new MsgData_sHuoYueDuRewardVo();
				__item.unpack(buffer);
				rewardList[i]=__item;
			}
			count = buffer.ReadUInt32();
			list = new List<MsgData_sHuoYueDuTaskVo>();
			for (int i = 0; i < count; i++)
			{
				MsgData_sHuoYueDuTaskVo __item = new MsgData_sHuoYueDuTaskVo();
				__item.unpack(buffer);
				list.Add(__item);
			}
		}
	}

	// S->C 服务器返回:进入流水副本结果 msgId:8439
	public class MsgData_sWaterDungeonEnterResult : MsgData
	{
		public sbyte result;	// 0:成功 1:功能未开启 2: 副本次数已用完;
		public int rightTime;	// 任务剩余时间（秒）

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt8();
			rightTime = buffer.ReadInt32();
		}
	}

	// S->C 服务器返回:退出流水副本结果 msgId:8440
	public class MsgData_sWaterDungeonExitResult : MsgData
	{
		public sbyte result;	// 0:成功  

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt8();
		}
	}

	// S->C 服务器返回:流水副本信息 msgId:8434
	public class MsgData_sWaterDungeonInfo : MsgData
	{
		public int wave;	// 我的最佳波数
		public double exp;	// 我的最高经验
		public sbyte time;	// 已用次数
		public int buyCount;	// 购买次数
		public int monster;	// 我的最多杀怪
		public double moreExp;	// 今天可额外领取的经验
		public sbyte moreReward;	// 0不可以额外领取,1可以额外领取

		public override void unpack(NetReadBuffer buffer)
		{
			wave = buffer.ReadInt32();
			exp = buffer.ReadDouble();
			time = buffer.ReadInt8();
			buyCount = buffer.ReadInt32();
			monster = buffer.ReadInt32();
			moreExp = buffer.ReadDouble();
			moreReward = buffer.ReadInt8();
		}
	}

	// S->C 服务器返回:流水副本进度 msgId:8436
	public class MsgData_sWaterDungeonProgress : MsgData
	{
		public int wave;	// 当前波数
		public int monster;	// 当前波杀怪数
		public double exp;	// 累计获得经验

		public override void unpack(NetReadBuffer buffer)
		{
			wave = buffer.ReadInt32();
			monster = buffer.ReadInt32();
			exp = buffer.ReadDouble();
		}
	}

	// S->C 服务器返回:流水副本结算 msgId:8437
	public class MsgData_sWaterDungeonResult : MsgData
	{
		public int wave;	// 累计波数
		public double exp;	// 累计获得经验
		public int result;	// 刷副本结果: 0成功 1时间到失败 2主动离开

		public override void unpack(NetReadBuffer buffer)
		{
			wave = buffer.ReadInt32();
			exp = buffer.ReadDouble();
			result = buffer.ReadInt32();
		}
	}

	// C->S  客户端请求：冲穴 msgId：3666;
	public class MsgData_cOpenNode : MsgData
	{
		public int id;	// 经脉ID

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(id);
		}
	}

	// S->C 服务器返回：冲穴结果 msgId：8666
	public class MsgData_sOpenNode : MsgData
	{
		public sbyte result;	// 结果 0成功 1失败 2道具不足
		public int id;	// 经脉的ID
		public int NodeId;	// 最后冲穴成功的穴位ID
		public int Level;	// 穴位当前的等级

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt8();
			id = buffer.ReadInt32();
			NodeId = buffer.ReadInt32();
			Level = buffer.ReadInt32();
		}
	}

	// S->C 穴位列表
	public class MsgData_sNeiGonginset : MsgData
	{
		public int id;	// 经脉的ID； 
		public int NodeId;	// 最后冲穴成功的穴位ID 
		public int Level;	// 穴位当前的等级

		public override void unpack(NetReadBuffer buffer)
		{
			id = buffer.ReadInt32();
			NodeId = buffer.ReadInt32();
			Level = buffer.ReadInt32();
		}
	}

	// S->C 返回活跃度信息 msgId:8260;
	public class MsgData_sNeiGongInfo : MsgData
	{
		public uint count;	// 数量
		public List<MsgData_sNeiGonginset> Items = new List<MsgData_sNeiGonginset>();	// 活跃度列表

		public override void unpack(NetReadBuffer buffer)
		{
			count = buffer.ReadUInt32();
			Items = new List<MsgData_sNeiGonginset>();
			for (int i = 0; i < count; i++)
			{
				MsgData_sNeiGonginset __item = new MsgData_sNeiGonginset();
				__item.unpack(buffer);
				Items.Add(__item);
			}
		}
	}

	// S->C 开启新功能 msgId:8110;
	public class MsgData_sFunctionOpen : MsgData
	{
		public ushort functionID;	// 功能id ,对应funcopen表里的id

		public override void unpack(NetReadBuffer buffer)
		{
			functionID = buffer.ReadUInt16();
		}
	}

	// S->C 功能开启列表 msgId:8111;
	public class MsgData_sFunctionList : MsgData
	{
		public ushort count;	// 功能个数
		public List<ushort> functionList = new List<ushort>();	// 功能列表

		public override void unpack(NetReadBuffer buffer)
		{
			count = buffer.ReadUInt16();
			functionList = new List<ushort>();
			for (int i = 0; i < count; i++)
			{
				ushort __item = buffer.ReadUInt16();
				functionList.Add(__item);
			}
		}
	}

	// C->S  请求进入单人秘境副本 msgId:3628;
	public class MsgData_cReqEnterSimpleSecrectDupl : MsgData
	{
		public int id;	// 副本ID
		public int itemId;	// 钥匙ID
		public int enterType;	// 进入类型 1、单人进入组队秘境 2、请求开始进入

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(id);
			buffer.WriteInt32(itemId);
			buffer.WriteInt32(enterType);
		}
	}

	// C->S  请求购买组队或次数 msgId:3642;
	public class MsgData_cReqBuySecrectDuplTili : MsgData
	{
		public int type;	// 类型 1组队购买次数 3个人购买次数

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(type);
		}
	}

	// C->S  请求激活 msgId:3643;
	public class MsgData_cReqJiHuoSecrectDupl : MsgData
	{
		public int stepid;	// 阶数
		public int id;	// 套装ID

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(stepid);
			buffer.WriteInt32(id);
		}
	}

	// C->S 请求:个人秘境副本扫荡 msgId:3967;
	public class MsgData_cSecretDungeonSweep : MsgData
	{
		public int id;	// 层数id
		public int num;	// 扫荡次数

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(id);
			buffer.WriteInt32(num);
		}
	}

	// C->S 请求:个人秘境副本扫荡领奖励 msgId:3968;
	public class MsgData_cSecretDungeonSweepReward : MsgData
	{
		public int id;	// 层数id

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(id);
		}
	}

	// S->C 返回单人秘境副本面板信息 msgId:8637;
	public class MsgData_sResSimpleSecrectDuplInfo : MsgData
	{
		public int tili;	// 组队剩余进入次数
		public int counts;	// 剩余进入次数
		public int vlaTag;	// 个人已购买次数
		public int vlaTagTeam;	// 组队已购买次数
		public int simpleMaxLayerCount;	// 单人秘境最高层数

		public override void unpack(NetReadBuffer buffer)
		{
			tili = buffer.ReadInt32();
			counts = buffer.ReadInt32();
			vlaTag = buffer.ReadInt32();
			vlaTagTeam = buffer.ReadInt32();
			simpleMaxLayerCount = buffer.ReadInt32();
		}
	}

	// S->C 秘境副本刷怪区域ID
	public class MsgData_sResMakeresult : MsgData
	{
		public int id;	// 区域ID

		public override void unpack(NetReadBuffer buffer)
		{
			id = buffer.ReadInt32();
		}
	}

	// S->C 返回进入单人秘境副本 msgId:8638
	public class MsgData_sResEnterSimpleSecrectDupl : MsgData
	{
		public int result;	// -的是错误结果 0、请求进入成功 1、请求开始进入成功
		public int id;	// 进入副本ID
		public int type;	// 进入类型 0、个人 1、组队 2、跨服
		public uint count;	// 列表数量
		public List<MsgData_sResMakeresult> items = new List<MsgData_sResMakeresult>();	// 秘境副本刷怪区域ID列表

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt32();
			id = buffer.ReadInt32();
			type = buffer.ReadInt32();
			count = buffer.ReadUInt32();
			items = new List<MsgData_sResMakeresult>();
			for (int i = 0; i < count; i++)
			{
				MsgData_sResMakeresult __item = new MsgData_sResMakeresult();
				__item.unpack(buffer);
				items.Add(__item);
			}
		}
	}

	// S->C 秘境副本追踪面饭信息 msgId:8639;
	public class MsgData_sSimpleSecrectDuplTrace : MsgData
	{
		public int num;	// 已击杀数量
		public int value;	// 当前产生的值
		public int id;	// 区域ID

		public override void unpack(NetReadBuffer buffer)
		{
			num = buffer.ReadInt32();
			value = buffer.ReadInt32();
			id = buffer.ReadInt32();
		}
	}

	// S->C 单人秘境副本结算 msgId:8640
	public class MsgData_sSimpleSecrectDuplCom : MsgData
	{
		public int result;	// 0成功 1失败

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt32();
		}
	}

	// S->C 返回退出单人秘境副本 msgId:8641
	public class MsgData_sResExitSimpleSecrectDupl : MsgData
	{

		public override void unpack(NetReadBuffer buffer)
		{
		}
	}

	// S->C 更新组队或次数 msgId:8642
	public class MsgData_sUpdateSecrectDuplTili : MsgData
	{
		public int type;	// 类型 1组队购买次数 3个人购买次数
		public int param;	// 参数
		public int vlaTag;	// 已购买次数

		public override void unpack(NetReadBuffer buffer)
		{
			type = buffer.ReadInt32();
			param = buffer.ReadInt32();
			vlaTag = buffer.ReadInt32();
		}
	}

	// S->C 激活列表
	public class MsgData_sJiHuoMakeresult : MsgData
	{
		public int taozhuangID;	// 套装ID
		public int equipID;	//  装备ID

		public override void unpack(NetReadBuffer buffer)
		{
			taozhuangID = buffer.ReadInt32();
			equipID = buffer.ReadInt32();
		}
	}

	// S->C 返回激活结果 msgId:8643
	public class MsgData_sResJiHuoSecrectDupl : MsgData
	{
		public uint count;	// 列表数量
		public List<MsgData_sJiHuoMakeresult> items = new List<MsgData_sJiHuoMakeresult>();	// 激活列表

		public override void unpack(NetReadBuffer buffer)
		{
			count = buffer.ReadUInt32();
			items = new List<MsgData_sJiHuoMakeresult>();
			for (int i = 0; i < count; i++)
			{
				MsgData_sJiHuoMakeresult __item = new MsgData_sJiHuoMakeresult();
				__item.unpack(buffer);
				items.Add(__item);
			}
		}
	}

	// S->C 返回:个人秘境副本扫荡 msgId:8967
	public class MsgData_sSecretDungeonSweep : MsgData
	{
		public int result;	// 结果:0成功 1其他 2跨服中 3等级不足 4次数不足 5还在扫荡中 6道具不足
		public int id;	//  层数id
		public int num;	//  扫荡次数
		public int second;	//  剩余秒数

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt32();
			id = buffer.ReadInt32();
			num = buffer.ReadInt32();
			second = buffer.ReadInt32();
		}
	}

	// S->C 返回:个人秘境副本扫荡领奖励 msgId:8968
	public class MsgData_sSecretDungeonSweepReward : MsgData
	{
		public int result;	// 结果:0成功 1其他 2还在扫荡中
		public int id;	//  层数id
		public int num;	//  扫荡次数

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt32();
			id = buffer.ReadInt32();
			num = buffer.ReadInt32();
		}
	}

	// S->C 返回:个人秘境副本扫荡 msgId:8969
	public class MsgData_sSecretDungeonSweepInfo : MsgData
	{
		public int id;	//  层数id
		public int num;	//  扫荡次数
		public int second;	//  剩余秒数

		public override void unpack(NetReadBuffer buffer)
		{
			id = buffer.ReadInt32();
			num = buffer.ReadInt32();
			second = buffer.ReadInt32();
		}
	}

	// C->S c->s 请求世界列表(CW_WorldBoss msgId:2063)
	public class MsgData_cWorldBoss : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// S->C 世界Boss列表
	public class MsgData_sWorldBossList : MsgData
	{
		public int id;	// bossId
		public int line;	//  活动所在线
		public int state;	// 0活着， 1死亡
		public long roleId;	//  上次击杀者roleId
		public byte[] roleName = new byte[32];	// 上次击杀者名字
		public double hp;	// 剩余血量
		public double maxHp;	// 总血量

		public override void unpack(NetReadBuffer buffer)
		{
			id = buffer.ReadInt32();
			line = buffer.ReadInt32();
			state = buffer.ReadInt32();
			roleId = buffer.ReadInt64();
			buffer.ReadBytes(roleName);
			hp = buffer.ReadDouble();
			maxHp = buffer.ReadDouble();
		}
	}

	// S->C s->c 返回世界BOSS列表，刷新时推单个(WC_WorldBoss msgId:7064)
	public class MsgData_sWorldBoss : MsgData
	{
		public uint count;	// Boss列表数量
		public List<MsgData_sWorldBossList> items = new List<MsgData_sWorldBossList>();	// 世界Boss列表

		public override void unpack(NetReadBuffer buffer)
		{
			count = buffer.ReadUInt32();
			items = new List<MsgData_sWorldBossList>();
			for (int i = 0; i < count; i++)
			{
				MsgData_sWorldBossList __item = new MsgData_sWorldBossList();
				__item.unpack(buffer);
				items.Add(__item);
			}
		}
	}

	// S->C 活动状态列表
	public class MsgData_sActivityStateList : MsgData
	{
		public int id;	// 活动id
		public int state;	// 0关闭， 1开启
		public long time;	// 开启或关闭时间
		public int line;	//  活动所在线
		public int mapId;	//  活动所在地图
		public int forecast;	//  活动预告开启标志 0：关闭， 1：开启

		public override void unpack(NetReadBuffer buffer)
		{
			id = buffer.ReadInt32();
			state = buffer.ReadInt32();
			time = buffer.ReadInt64();
			line = buffer.ReadInt32();
			mapId = buffer.ReadInt32();
			forecast = buffer.ReadInt32();
		}
	}

	// S->C s->c 返回活动状态，刷新时推单个(WC_ActivityState msgId:7065)
	public class MsgData_sActivityState : MsgData
	{
		public uint count;	// 数量
		public List<MsgData_sActivityStateList> items = new List<MsgData_sActivityStateList>();	// 活动状态列表

		public override void unpack(NetReadBuffer buffer)
		{
			count = buffer.ReadUInt32();
			items = new List<MsgData_sActivityStateList>();
			for (int i = 0; i < count; i++)
			{
				MsgData_sActivityStateList __item = new MsgData_sActivityStateList();
				__item.unpack(buffer);
				items.Add(__item);
			}
		}
	}

	// C->S 请求:进入活动 msgId:3159;
	public class MsgData_cActivityEnter : MsgData
	{
		public int id;	// 活动id
		public int param1;	// 参数1 打宝塔时传层数id,其他活动传0

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(id);
			buffer.WriteInt32(param1);
		}
	}

	// C->S 请求:退出活动 msgId:3160;
	public class MsgData_cActivityQuit : MsgData
	{
		public int id;	// 活动id

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(id);
		}
	}

	// S->C 活动列表list
	public class MsgData_sActivityList : MsgData
	{
		public int id;	// 活动id
		public int dailyCount;	// 今天已参加次数

		public override void unpack(NetReadBuffer buffer)
		{
			id = buffer.ReadInt32();
			dailyCount = buffer.ReadInt32();
		}
	}

	// S->C 登录返回活动列表 msgId:8158
	public class MsgData_sActivity : MsgData
	{
		public uint count;	// 数量
		public List<MsgData_sActivityList> list = new List<MsgData_sActivityList>();	// 活动列表list

		public override void unpack(NetReadBuffer buffer)
		{
			count = buffer.ReadUInt32();
			list = new List<MsgData_sActivityList>();
			for (int i = 0; i < count; i++)
			{
				MsgData_sActivityList __item = new MsgData_sActivityList();
				__item.unpack(buffer);
				list.Add(__item);
			}
		}
	}

	// S->C 返回:进入活动 msgId:8159
	public class MsgData_sActivityEnter : MsgData
	{
		public int result;	// 结果:0成功  -1配置等系统性错误   -2或其他 为条件不满足错误
		public int id;	//  活动id
		public int param1;	//  参数1

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt32();
			id = buffer.ReadInt32();
			param1 = buffer.ReadInt32();
		}
	}

	// S->C 返回:退出活动 msgId:8160
	public class MsgData_sActivityQuit : MsgData
	{
		public int result;	// 结果:0成功
		public int id;	//  活动id

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt32();
			id = buffer.ReadInt32();
		}
	}

	// S->C 返回:活动结束(活动内玩家) msgId:8161
	public class MsgData_sActivityFinish : MsgData
	{
		public int id;	//  活动id

		public override void unpack(NetReadBuffer buffer)
		{
			id = buffer.ReadInt32();
		}
	}

	// S->C 返回玩家累计伤害 msgId:8162
	public class MsgData_sWorldBossDamage : MsgData
	{
		public double damage;	// 伤害总量

		public override void unpack(NetReadBuffer buffer)
		{
			damage = buffer.ReadDouble();
		}
	}

	// S->C (活动内)世界BOSS伤害排行(前5名)
	public class MsgData_sWorldBossHurtList : MsgData
	{
		public long roleId;	// roleId
		public byte[] roleName = new byte[32];	// roleName
		public double hurt;	// 造成伤害

		public override void unpack(NetReadBuffer buffer)
		{
			roleId = buffer.ReadInt64();
			buffer.ReadBytes(roleName);
			hurt = buffer.ReadDouble();
		}
	}

	// S->C 返回世界BOSS伤害信息(活动内) msgId:8163
	public class MsgData_sWorldBossHurt : MsgData
	{
		public double hp;	// hp
		public double maxHp;	// maxHp
		public MsgData_sWorldBossHurtList[] hurtList = new MsgData_sWorldBossHurtList[5];	// 世界BOSS伤害排行(前5名)

		public override void unpack(NetReadBuffer buffer)
		{
			hp = buffer.ReadDouble();
			maxHp = buffer.ReadDouble();
			for (int i = 0; i < 5; i++)
			{
				MsgData_sWorldBossHurtList __item = new MsgData_sWorldBossHurtList();
				__item.unpack(buffer);
				hurtList[i]=__item;
			}
		}
	}

	// C->S 请求:挖宝列表信息 msgId:3806;
	public class MsgData_cWaBaoList : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// S->C boss挖宝列表
	public class MsgData_sBossWabaoList : MsgData
	{
		public int bossId;	// bossId
		public int num;	// 可领取次数
		public int state;	// 领取状态:0:不可领取， 1:可领取  2:已领取

		public override void unpack(NetReadBuffer buffer)
		{
			bossId = buffer.ReadInt32();
			num = buffer.ReadInt32();
			state = buffer.ReadInt32();
		}
	}

	// S->C 返回Boss挖宝信息 msgId:8807
	public class MsgData_sWaBaoList : MsgData
	{
		public uint count;	// 数量
		public List<MsgData_sBossWabaoList> list = new List<MsgData_sBossWabaoList>();	// boss挖宝列表

		public override void unpack(NetReadBuffer buffer)
		{
			count = buffer.ReadUInt32();
			list = new List<MsgData_sBossWabaoList>();
			for (int i = 0; i < count; i++)
			{
				MsgData_sBossWabaoList __item = new MsgData_sBossWabaoList();
				__item.unpack(buffer);
				list.Add(__item);
			}
		}
	}

	// C->S 请求:领取对应BOSS奖励 msgId:3805;
	public class MsgData_cGetWaBaoReward : MsgData
	{
		public int bossId;	// bossId
		public int index;	// 领取类型 0:1次 , 1:多次

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(bossId);
			buffer.WriteInt32(index);
		}
	}

	// S->C 领取对应BOSS奖励 msdId:8808
	public class MsgData_sGetWaBaoReward : MsgData
	{
		public int bossId;	// bossId
		public int num;	// 可领取次数
		public int result;	// 使用结果 0成功

		public override void unpack(NetReadBuffer buffer)
		{
			bossId = buffer.ReadInt32();
			num = buffer.ReadInt32();
			result = buffer.ReadInt32();
		}
	}

	// S->C 陷阱触发 msdId:9947
	public class MsgData_sTriggerTrap : MsgData
	{
		public ulong TrapRoleID;	// Trap ID
		public byte TrapType;	// 0 buffer，光环，1 trap

		public override void unpack(NetReadBuffer buffer)
		{
			TrapRoleID = buffer.ReadUInt64();
			TrapType = buffer.ReadUInt8();
		}
	}

	// S->C 返回世界等级 msdId:8740
	public class MsgData_sWorldLevel : MsgData
	{
		public int level;	// 世界等级

		public override void unpack(NetReadBuffer buffer)
		{
			level = buffer.ReadInt32();
		}
	}

	// S->C 返回野外金币BOSS信息 msdId:8737
	public class MsgData_sFieldBoss : MsgData
	{
		public int id;	// bossId
		public long damage;	// 伤害值

		public override void unpack(NetReadBuffer buffer)
		{
			id = buffer.ReadInt32();
			damage = buffer.ReadInt64();
		}
	}

	// C->S 请求:珍宝阁数据 msgId:3113;
	public class MsgData_cZhenBaoGe : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// C->S 珍宝阁提交道具list
	public class MsgData_cZhenBaoGeSubmitList : MsgData
	{
		public int id;	// 珍宝阁id

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(id);
		}
	}

	// C->S 珍宝阁一键提交道具 msgId:3114;
	public class MsgData_cZhenBaoGeSubmit : MsgData
	{
		public uint count;	// 数量
		public List<MsgData_cZhenBaoGeSubmitList> list = new List<MsgData_cZhenBaoGeSubmitList>();	// 珍宝阁提交道具list

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteUInt32(count);
			for (int i = 0; i < count; i++)
			{
				MsgData_cZhenBaoGeSubmitList __item = list[i];
				__item.pack(buffer);
			}
		}
	}

	// C->S 珍宝阁提交特殊道具 msgId:3115;
	public class MsgData_cZhenBaoGeSpeItem : MsgData
	{
		public int id;	// 珍宝阁id
		public int itemId;	// 特殊道具id

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(id);
			buffer.WriteInt32(itemId);
		}
	}

	// S->C 珍宝阁数据list
	public class MsgData_sZhenBaoGeList : MsgData
	{
		public int id;	// 珍宝阁id
		public int submitTimes;	// 已提交次数
		public int submitNum;	// 当前次数已提交数量
		public int itemNum1;	// 特殊道具1
		public int itemNum2;	// 特殊道具2
		public int itemNum3;	// 特殊道具3

		public override void unpack(NetReadBuffer buffer)
		{
			id = buffer.ReadInt32();
			submitTimes = buffer.ReadInt32();
			submitNum = buffer.ReadInt32();
			itemNum1 = buffer.ReadInt32();
			itemNum2 = buffer.ReadInt32();
			itemNum3 = buffer.ReadInt32();
		}
	}

	// S->C 返回珍宝阁数据,数据刷新时也返回这个 msgID:8113
	public class MsgData_sZhenBaoGe : MsgData
	{
		public uint count;	// 数量
		public List<MsgData_sZhenBaoGeList> list = new List<MsgData_sZhenBaoGeList>();	// 珍宝阁数据list

		public override void unpack(NetReadBuffer buffer)
		{
			count = buffer.ReadUInt32();
			list = new List<MsgData_sZhenBaoGeList>();
			for (int i = 0; i < count; i++)
			{
				MsgData_sZhenBaoGeList __item = new MsgData_sZhenBaoGeList();
				__item.unpack(buffer);
				list.Add(__item);
			}
		}
	}

	// C->S 客户端请求：寻宝任务接取 msgId:3476
	public class MsgData_cFindTreasure : MsgData
	{
		public int quality;	// 寻宝图quality

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(quality);
		}
	}

	// C->S 请求打宝塔增加时间 msgId:3676
	public class MsgData_cReqTreasureAddTime : MsgData
	{
		public int id;	// 物品ID
		public int count;	// 物品数量

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(id);
			buffer.WriteInt32(count);
		}
	}

	// S->C 寻宝信息 msgId:8475
	public class MsgData_sFindTreasureInfo : MsgData
	{
		public int mapid;	// 地图点1
		public int mapid2;	// 地图点2
		public int wabaoId;	// wabao表ID
		public int getlvl;	// 接取时玩家等级
		public int lastNum;	// 剩余次数
		public int lookPoint;	// 看过的点。没有发0

		public override void unpack(NetReadBuffer buffer)
		{
			mapid = buffer.ReadInt32();
			mapid2 = buffer.ReadInt32();
			wabaoId = buffer.ReadInt32();
			getlvl = buffer.ReadInt32();
			lastNum = buffer.ReadInt32();
			lookPoint = buffer.ReadInt32();
		}
	}

	// S->C 服务器返回:寻宝任务接取 结果 msgId:8476
	public class MsgData_sFindTreasureResult : MsgData
	{
		public int result;	// 结果=0成功，1=失败
		public int mapid;	// 地图点1
		public int mapid2;	// 地图点2
		public int wabaoId;	// wabao表ID
		public int getlvl;	// 接取时玩家等级
		public int lastNum;	// 剩余次数

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt32();
			mapid = buffer.ReadInt32();
			mapid2 = buffer.ReadInt32();
			wabaoId = buffer.ReadInt32();
			getlvl = buffer.ReadInt32();
			lastNum = buffer.ReadInt32();
		}
	}

	// S->C 服务器返回:取消寻宝任务 msgId:8477
	public class MsgData_sFindTreasureCancel : MsgData
	{
		public int result;	// 结果=0成功，1=失败

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt32();
		}
	}

	// S->C 服务器返回:接取结果 msgId:8478;
	public class MsgData_sFindTreasureCollect : MsgData
	{
		public int result;	// 0=真，1=假
		public int mapid;	// 当前辨别的id
		public int resType;	// 1=宝箱，2=妖怪
		public int resId;	// 挖到的id

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt32();
			mapid = buffer.ReadInt32();
			resType = buffer.ReadInt32();
			resId = buffer.ReadInt32();
		}
	}

	// S->C 返回今日已经购买的时间 msgId:8676
	public class MsgData_sTreasureTodayAddedTimer : MsgData
	{
		public int addedTimer;	// 今日已经购买的时间

		public override void unpack(NetReadBuffer buffer)
		{
			addedTimer = buffer.ReadInt32();
		}
	}

	// S->C 打宝塔剩余时间 msgId:8677
	public class MsgData_sTreasureRemainTime : MsgData
	{
		public int remainTime;	// 剩余时间

		public override void unpack(NetReadBuffer buffer)
		{
			remainTime = buffer.ReadInt32();
		}
	}

	// S->C boss状态列表
	public class MsgData_sTreasurebossList : MsgData
	{
		public int bossState;	// 0、已刷新 1、未刷新
		public int remainTimer;	// 未刷新需要剩余时间
		public int id;	// BossID

		public override void unpack(NetReadBuffer buffer)
		{
			bossState = buffer.ReadInt32();
			remainTimer = buffer.ReadInt32();
			id = buffer.ReadInt32();
		}
	}

	// S->C 更新BOSS状态信息 msgId:8678
	public class MsgData_sTreasureUpdateBoss : MsgData
	{
		public uint count;	// 数量
		public List<MsgData_sTreasurebossList> items = new List<MsgData_sTreasurebossList>();	// 数量

		public override void unpack(NetReadBuffer buffer)
		{
			count = buffer.ReadUInt32();
			items = new List<MsgData_sTreasurebossList>();
			for (int i = 0; i < count; i++)
			{
				MsgData_sTreasurebossList __item = new MsgData_sTreasurebossList();
				__item.unpack(buffer);
				items.Add(__item);
			}
		}
	}

	// C->S 组队秘境创建房间 msgId:2156
	public class MsgData_cCreateRoom : MsgData
	{
		public int dungeonIndex;	// 副本层数对应ID
		public int gloryLevelLimit;	// 修为等级
		public int attrLimit;	// 战力需求
		public int autoStart;	// 自动开始，0是true

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(dungeonIndex);
			buffer.WriteInt32(gloryLevelLimit);
			buffer.WriteInt32(attrLimit);
			buffer.WriteInt32(autoStart);
		}
	}

	// S->C 组队秘境，自己房间信息 msgId:7154
	public class MsgData_sTimeDungeonRoomInfo : MsgData
	{
		public int dungeonIndex;	// 副本层数对应ID
		public sbyte isLock;	// 锁
		public int lockAttrNum;	// 战斗力限制值
		public int autoStart;	// 自动开始，0是true

		public override void unpack(NetReadBuffer buffer)
		{
			dungeonIndex = buffer.ReadInt32();
			isLock = buffer.ReadInt8();
			lockAttrNum = buffer.ReadInt32();
			autoStart = buffer.ReadInt32();
		}
	}

	// C->S 试炼秘境队伍请求进入组队活动 msgId:2907
	public class MsgData_cSecretTeamStart : MsgData
	{
		public int dungeonID;	// 副本层数对应ID

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(dungeonID);
		}
	}

	// S->C 进入组队副本提示准备消息 msgId:7245
	public class MsgData_sEnterDulpPrepare : MsgData
	{
		public int line;	// 队长所在线
		public int type;	// 邀请类型 0:正常组队 1:魔域深渊 2:奇遇 3:公会组队

		public override void unpack(NetReadBuffer buffer)
		{
			line = buffer.ReadInt32();
			type = buffer.ReadInt32();
		}
	}

	// C->S 客户端请求：查询指定点是否有怪 msgId:4939
	public class MsgData_cQueryMonsterByPosition : MsgData
	{
		public int x;	// x
		public int y;	// y

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(x);
			buffer.WriteInt32(y);
		}
	}

	// S->C 服务器返回:查询指定点有多少怪 msgId:9948
	public class MsgData_sQueryMonsterByPosition : MsgData
	{
		public int x;	// x
		public int y;	// y
		public sbyte num;	// 怪物数量

		public override void unpack(NetReadBuffer buffer)
		{
			x = buffer.ReadInt32();
			y = buffer.ReadInt32();
			num = buffer.ReadInt8();
		}
	}

	// C->S 客户端请求：设置装备套装 msgId:3465
	public class MsgData_cEquipGroup : MsgData
	{
		public long equipUid;	// 装备UID
		public long itemUid;	// 物品UID

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(equipUid);
			buffer.WriteInt64(itemUid);
		}
	}

	// S->C 服务器返回:设置装备套装 msgId:8465
	public class MsgData_sEquipGroup : MsgData
	{
		public sbyte result;	// 结果:0成功，1装备不存在，2道具不存在 3物品无法使用
		public long equipUid;	// 装备UID
		public long itemUid;	// 物品UID
		public int itemId;	// 物品ID

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt8();
			equipUid = buffer.ReadInt64();
			itemUid = buffer.ReadInt64();
			itemId = buffer.ReadInt32();
		}
	}

	// C->S 客户端请求：设置装备套装2 msgId:3564
	public class MsgData_cEquipGroupTwo : MsgData
	{
		public long equipUid;	// 装备UID
		public int groupId;	// 套装tid
		public int isBind;	// 是否绑定  0非绑定  1绑定

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(equipUid);
			buffer.WriteInt32(groupId);
			buffer.WriteInt32(isBind);
		}
	}

	// S->C 服务器返回:设置装备套装2 msgId:8565
	public class MsgData_sEquipGroupTwo : MsgData
	{
		public sbyte result;	// 结果:0成功，1装备不存在，2道具不存在 3物品无法使用  4已有套装
		public long equipUid;	// 装备UID
		public int groupId;	// 套装tid
		public int isBind;	// 是否绑定  0未绑定  1已绑定

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt8();
			equipUid = buffer.ReadInt64();
			groupId = buffer.ReadInt32();
			isBind = buffer.ReadInt32();
		}
	}

	// C->S 客户端请求：剥离装备套装 msgId:3558
	public class MsgData_cEquipGroupPeel : MsgData
	{
		public long equipUid;	// 装备UID

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(equipUid);
		}
	}

	// S->C 服务器返回:剥离装备套装 msgId:8558
	public class MsgData_sEquipGroupPeel : MsgData
	{
		public sbyte result;	// 结果:0成功，1装备不存在，2套装不存在
		public long equipUid;	// 装备UID

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt8();
			equipUid = buffer.ReadInt64();
		}
	}

	// C->S 客户端请求：套装升级 msgId:3685
	public class MsgData_cEquipGroupLvlUp : MsgData
	{
		public long equipUid;	// 装备UID
		public int isBind;	// 使用绑定材料:  0 非绑定材料   1 绑定材料    2不限

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(equipUid);
			buffer.WriteInt32(isBind);
		}
	}

	// S->C 服务器返回:套装升级 msgId:8685
	public class MsgData_sEquipGroupLvlUp : MsgData
	{
		public sbyte result;	// 结果:0成功，1装备不存在，2道具不足   3已满级   4无套装属性
		public long equipUid;	// 装备UID
		public int equipLevel;	// 套装等级

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt8();
			equipUid = buffer.ReadInt64();
			equipLevel = buffer.ReadInt32();
		}
	}

	// C->S 客户端请求：装备套装交换 msgId:3868
	public class MsgData_cNewEquipGroupSwap : MsgData
	{
		public long equipId1;	// 装备ID1
		public long equipId2;	// 装备ID2

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(equipId1);
			buffer.WriteInt64(equipId2);
		}
	}

	// S->C 服务器返回:装备套装交换 msgId:8868
	public class MsgData_sNewEquipGroupSwap : MsgData
	{
		public int result;	// 结果:0成功，1失败
		public long equipId1;	// 装备ID1
		public int groupId1;	// 装备groupId1
		public int groupLvl1;	// 装备grouplvl1
		public sbyte bind1;	// 0未绑定   1已绑定
		public long equipId2;	// 装备ID2
		public int groupId2;	// 装备groupId2
		public int groupLvl2;	// 装备grouplvl2
		public sbyte bind2;	// 0未绑定   1已绑定

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt32();
			equipId1 = buffer.ReadInt64();
			groupId1 = buffer.ReadInt32();
			groupLvl1 = buffer.ReadInt32();
			bind1 = buffer.ReadInt8();
			equipId2 = buffer.ReadInt64();
			groupId2 = buffer.ReadInt32();
			groupLvl2 = buffer.ReadInt32();
			bind2 = buffer.ReadInt8();
		}
	}

	// C->S 客户端请求：解锁套装孔 msgId:3943
	public class MsgData_cEquipGroupOpenPos : MsgData
	{
		public int pos;	// 装备位
		public int index;	// 套装位

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(pos);
			buffer.WriteInt32(index);
		}
	}

	// S->C 服务器返回:解锁套装孔 msgId:8944
	public class MsgData_sEquipGroupOpenPos : MsgData
	{
		public int result;	// 结果:0成功，1道具不足  2不可解锁  3已达可解锁上限
		public int pos;	// 装备位
		public int index;	// 套装位

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt32();
			pos = buffer.ReadInt32();
			index = buffer.ReadInt32();
		}
	}

	// C->S 客户端请求：激活套装id msgId:3944
	public class MsgData_cEquipGroupOpenSet : MsgData
	{
		public int pos;	// 装备位
		public int index;	// 套装位

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(pos);
			buffer.WriteInt32(index);
		}
	}

	// S->C 服务器返回:激活套装id msgId:8945
	public class MsgData_sEquipGroupOpenSet : MsgData
	{
		public int result;	// 结果:0成功，1道具不足  2不可开启
		public int pos;	// 装备位
		public int index;	// 套装位

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt32();
			pos = buffer.ReadInt32();
			index = buffer.ReadInt32();
		}
	}

	// C->S 客户端请求：升级套装 msgId:3945
	public class MsgData_cEquipGroupUpLvl : MsgData
	{
		public int pos;	// 装备位
		public int index;	// 套装位

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(pos);
			buffer.WriteInt32(index);
		}
	}

	// S->C 服务器返回: 升级套装  msgId:8946
	public class MsgData_sEquipGroupUpLvl : MsgData
	{
		public int result;	// 结果:0成功，1道具不足  2不可开启
		public int pos;	// 装备位
		public int index;	// 套装位
		public int lvl;	// 等级

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt32();
			pos = buffer.ReadInt32();
			index = buffer.ReadInt32();
			lvl = buffer.ReadInt32();
		}
	}

	// C->S 熔炼道具list
	public class MsgData_cEquipSmeltList : MsgData
	{
		public long uid;	// 熔炼道具guid

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(uid);
		}
	}

	// C->S 客户端请求:装备熔炼 msgId:3514;
	public class MsgData_cEquipSmelt : MsgData
	{
		public int flags;	// 熔炼品质
		public uint count;	// 数量
		public List<MsgData_cEquipSmeltList> list = new List<MsgData_cEquipSmeltList>();	// 熔炼道具list

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(flags);
			buffer.WriteUInt32(count);
			for (int i = 0; i < count; i++)
			{
				MsgData_cEquipSmeltList __item = list[i];
				__item.pack(buffer);
			}
		}
	}

	// S->C 服务器返回:装备熔炼结果 msgId:8923
	public class MsgData_sEquipSmelt : MsgData
	{
		public int result;	// 结果:0成功
		public long equipUid;	// 装备uid

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt32();
			equipUid = buffer.ReadInt64();
		}
	}

	// C->S 客户端请求：进入宝塔秘境 msgId:4940
	public class MsgData_cEnterTreasureDupl : MsgData
	{
		public int id;	// 副本id

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(id);
		}
	}

	// S->C 服务器返回: 进入宝塔秘境  msgId:9949
	public class MsgData_sEnterTreasureDupl : MsgData
	{
		public int result;	// 结果:0成功 其它失败
		public int id;	// 副本id

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt32();
			id = buffer.ReadInt32();
		}
	}

	// S->C 服务器返回: 退了宝塔秘境  msgId:9950
	public class MsgData_sQuitTreasureDupl : MsgData
	{
		public int result;	// 结果:0成功 其它失败

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt32();
		}
	}

	// S->C 服务器返回: 组队目标数据  msgId:7907
	public class MsgData_sTeamTargetData : MsgData
	{
		public int teamTargetID;	// 组队目标ID
		public int powerLimit;	// 战力限制
		public int gloryLevelLimit;	// 修为限制

		public override void unpack(NetReadBuffer buffer)
		{
			teamTargetID = buffer.ReadInt32();
			powerLimit = buffer.ReadInt32();
			gloryLevelLimit = buffer.ReadInt32();
		}
	}

	// C->S 客户端请求: 更新组队目标  msgId:2908
	public class MsgData_cUpdateTeamTarget : MsgData
	{
		public int teamTargetID;	// 组队目标ID

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(teamTargetID);
		}
	}

	// C->S 客户端请求: 更新组队限制条件  msgId:2909
	public class MsgData_cUpdateTeamLimit : MsgData
	{
		public int teamTargetID;	// 组队目标ID
		public int powerLimit;	// 战力限制
		public int gloryLevelLimit;	// 修为限制

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(teamTargetID);
			buffer.WriteInt32(powerLimit);
			buffer.WriteInt32(gloryLevelLimit);
		}
	}

	// C->S 装备传承 msgId:3148
	public class MsgData_cEquipInherit : MsgData
	{
		public long srcid;	// 源装备cid
		public long tarid;	// 目标装备cid
		public int autoBuy;	// 自动购买材料 1:true

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(srcid);
			buffer.WriteInt64(tarid);
			buffer.WriteInt32(autoBuy);
		}
	}

	// S->C 返回装备传承 msgId:8150
	public class MsgData_sEquipInherit : MsgData
	{
		public long srcid;	// 源装备cid
		public long tarid;	// 目标装备cid
		public int result;	//  结果,0成功, -1失败

		public override void unpack(NetReadBuffer buffer)
		{
			srcid = buffer.ReadInt64();
			tarid = buffer.ReadInt64();
			result = buffer.ReadInt32();
		}
	}

	// C->S 公会任务一键完成 msgId:3942
	public class MsgData_cUnionQuestFinish : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// S->C 一键完成的公会任务列表
	public class MsgData_sUnionQuestList : MsgData
	{
		public int id;	// 任务id

		public override void unpack(NetReadBuffer buffer)
		{
			id = buffer.ReadInt32();
		}
	}

	// S->C 返回公会任务一键完成奖励信息 msgId:8942
	public class MsgData_sUnionQuestFinish : MsgData
	{
		public sbyte result;	// 0成功 1钱不够  2VIP等级不够  3等级不足
		public int level;	// 每日完成奖励等级
		public uint count;	// 完成的日讨伐任务列表个数
		public List<MsgData_sUnionQuestList> questList = new List<MsgData_sUnionQuestList>();	// 一键完成的公会任务列表

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt8();
			level = buffer.ReadInt32();
			count = buffer.ReadUInt32();
			questList = new List<MsgData_sUnionQuestList>();
			for (int i = 0; i < count; i++)
			{
				MsgData_sUnionQuestList __item = new MsgData_sUnionQuestList();
				__item.unpack(buffer);
				questList.Add(__item);
			}
		}
	}

	// C->S 请求培养 msgid:3850
	public class MsgData_cEquipPeiYang : MsgData
	{
		public long id;	// 装备cid

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(id);
		}
	}

	// C->S 请求培养保存属性 msgid:3851
	public class MsgData_cEquipPeiYangSet : MsgData
	{
		public long id;	// 装备cid

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(id);
		}
	}

	// S->C 返回请求培养 msgid:8850
	public class MsgData_sEquipPeiYang : MsgData
	{
		public int result;	// 错误类型 0成功  -1 材料不足 -2装备不存在
		public long id;	// 装备cid
		public uint count;	// 数量
		public List<MsgData_sNewSuperVO> items = new List<MsgData_sNewSuperVO>();	// 数据list

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt32();
			id = buffer.ReadInt64();
			count = buffer.ReadUInt32();
			items = new List<MsgData_sNewSuperVO>();
			for (int i = 0; i < count; i++)
			{
				MsgData_sNewSuperVO __item = new MsgData_sNewSuperVO();
				__item.unpack(buffer);
				items.Add(__item);
			}
		}
	}

	// S->C 返回培养保存属性 msgid:8851
	public class MsgData_sEquipPeiYangSet : MsgData
	{
		public int result;	// 错误类型 0成功  -1装备不存在
		public long id;	// 装备cid

		public override void unpack(NetReadBuffer buffer)
		{
			result = buffer.ReadInt32();
			id = buffer.ReadInt64();
		}
	}

	// S->C 开启的日常活动成员信息
	public class MsgData_sDailyActivyItem : MsgData
	{
		public int id;	// 日常活动表id
		public int state;	// 0-未开启，1-开启
		public int count;	// 完成次数

		public override void unpack(NetReadBuffer buffer)
		{
			id = buffer.ReadInt32();
			state = buffer.ReadInt32();
			count = buffer.ReadInt32();
		}
	}

	// S->C 开启的日常活动列表 msgId:9953
	public class MsgData_sDailyActivyList : MsgData
	{
		public uint count;	// 开启的日常活动个数
		public List<MsgData_sDailyActivyItem> list = new List<MsgData_sDailyActivyItem>();	// 开启的日常活动id列表

		public override void unpack(NetReadBuffer buffer)
		{
			count = buffer.ReadUInt32();
			list = new List<MsgData_sDailyActivyItem>();
			for (int i = 0; i < count; i++)
			{
				MsgData_sDailyActivyItem __item = new MsgData_sDailyActivyItem();
				__item.unpack(buffer);
				list.Add(__item);
			}
		}
	}

	// S->C 服务端通知:位移技能效果 msgId:8089
	public class MsgData_sCastMoveEffect : MsgData
	{
		public long caseterID;	// 释放者
		public long targetID;	// 目标者
		public int skillID;	// 技能ID
		public int skillEffectID;	// 技能效果ID
		public int moveTime;	// 移动时间
		public double posX;	// 位置X
		public double posY;	// 位置Y

		public override void unpack(NetReadBuffer buffer)
		{
			caseterID = buffer.ReadInt64();
			targetID = buffer.ReadInt64();
			skillID = buffer.ReadInt32();
			skillEffectID = buffer.ReadInt32();
			moveTime = buffer.ReadInt32();
			posX = buffer.ReadDouble();
			posY = buffer.ReadDouble();
		}
	}

	// C->S 请求七彩灵石操作 msgid:3681
	public class MsgData_cReqColorGemOperation : MsgData
	{
		public int Type;	// 装4镶嵌 5卸载
		public int ID;	// ID
		public int Pos;	// 位置

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(Type);
			buffer.WriteInt32(ID);
			buffer.WriteInt32(Pos);
		}
	}

	// C->S 请求七彩灵石升级 msgid:3682
	public class MsgData_cReqColorGemLevelup : MsgData
	{
		public int Type;	// 1激活 2升级 3突破
		public int ID;	// ID

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(Type);
			buffer.WriteInt32(ID);
		}
	}

	// S->C 服务端通知:数据
	public class MsgData_sResColorGemItem : MsgData
	{
		public int ID;	// ID
		public int Stage;	// 宝石段位
		public int Level;	// 宝石等级
		public int Pos;	// 镶嵌位置

		public override void unpack(NetReadBuffer buffer)
		{
			ID = buffer.ReadInt32();
			Stage = buffer.ReadInt32();
			Level = buffer.ReadInt32();
			Pos = buffer.ReadInt32();
		}
	}

	// S->C 服务端通知:返回灵石列表 msgId:8671
	public class MsgData_sResColorGemInfo : MsgData
	{
		public uint Size;	// list size
		public List<MsgData_sResColorGemItem> InsetList = new List<MsgData_sResColorGemItem>();	// 所有数据

		public override void unpack(NetReadBuffer buffer)
		{
			Size = buffer.ReadUInt32();
			InsetList = new List<MsgData_sResColorGemItem>();
			for (int i = 0; i < Size; i++)
			{
				MsgData_sResColorGemItem __item = new MsgData_sResColorGemItem();
				__item.unpack(buffer);
				InsetList.Add(__item);
			}
		}
	}

	// S->C 服务端通知:返回灵石操作结果 msgId:8672
	public class MsgData_sResColorGemILevelup : MsgData
	{
		public int Type;	// 1激活 2升级 3突破 4穿戴 5卸下
		public int Result;	// 0成功 1失败

		public override void unpack(NetReadBuffer buffer)
		{
			Type = buffer.ReadInt32();
			Result = buffer.ReadInt32();
		}
	}

	// S->C 服务端通知:数据
	public class MsgData_sResOtherColorGemItem : MsgData
	{
		public int Pos;	// 镶嵌位置
		public int ID;	// ID
		public int Level;	// 宝石等级
		public int Stage;	// 宝石段位

		public override void unpack(NetReadBuffer buffer)
		{
			Pos = buffer.ReadInt32();
			ID = buffer.ReadInt32();
			Level = buffer.ReadInt32();
			Stage = buffer.ReadInt32();
		}
	}

	// S->C 返回装备七彩灵石信息 msgId:8351（其他人的，响应cs消息3217，m_i4type:512）
	public class MsgData_sResOtherColorGemInfo : MsgData
	{
		public int ServerType;	// 是否全服排行信息 1=是 其余都不是
		public int Type;	// 1== 详细信息，2 == 排行榜详细信息
		public long RoleID;	// 角色ID
		public uint Size;	// list size
		public List<MsgData_sResOtherColorGemItem> InsetList = new List<MsgData_sResOtherColorGemItem>();	// 所有数据

		public override void unpack(NetReadBuffer buffer)
		{
			ServerType = buffer.ReadInt32();
			Type = buffer.ReadInt32();
			RoleID = buffer.ReadInt64();
			Size = buffer.ReadUInt32();
			InsetList = new List<MsgData_sResOtherColorGemItem>();
			for (int i = 0; i < Size; i++)
			{
				MsgData_sResOtherColorGemItem __item = new MsgData_sResOtherColorGemItem();
				__item.unpack(buffer);
				InsetList.Add(__item);
			}
		}
	}

	// C->S //发送设备token信息 msgID 2951
	public class MsgData_cDeviceTokenInfo : MsgData
	{
		public byte[] m_szToken = new byte[64];	// 设备token

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteBytes(m_szToken);
		}
	}

	// S->C s->c 经验信息 msgId:11180
	public class MsgData_sAddExpInfo : MsgData
	{
		public double exp;	// 总经验包含addexp
		public double addexp;	// 增加的经验值
		public double rate;	// 增加比例
		public int src;	// 来源：143怪物来源 277魔神buff

		public override void unpack(NetReadBuffer buffer)
		{
			exp = buffer.ReadDouble();
			addexp = buffer.ReadDouble();
			rate = buffer.ReadDouble();
			src = buffer.ReadInt32();
		}
	}

	// S->C //服务器推送：充值结果 msgId:7960
	public class MsgData_WC_RechargeRet : MsgData
	{
		public byte[] m_szOrderID = new byte[64];	// //订单ID
		public int m_nRet;	// //返回值 0成功 1订单重复 -2账号不存在 -3没有角色 -5保存订单失败
		public int m_nMoneys;	// //支付金额，单位元
		public int m_nNum;	// //购买数量
		public int m_nProductID;	// //商品ID
		public byte[] m_szProductName = new byte[256];	// //商品名称

		public override void unpack(NetReadBuffer buffer)
		{
			buffer.ReadBytes(m_szOrderID);
			m_nRet = buffer.ReadInt32();
			m_nMoneys = buffer.ReadInt32();
			m_nNum = buffer.ReadInt32();
			m_nProductID = buffer.ReadInt32();
			buffer.ReadBytes(m_szProductName);
		}
	}

	// S->C 竞技场玩家
	public class MsgData_sSceneObjectEnterVirtualPlayer : MsgData
	{
		public uint DataSize;	// 消息数据长度
		public long Guid;	// 唯一标识
		public byte ObjType;	// 对象类型
		public int PosX;	// X坐标
		public int PosY;	// Y坐标(unity为Z坐标)
		public int Dir;	// 朝向
		public long RoleID;	// 玩家id
		public byte[] Name = new byte[32];	// 角色名字
		public long Power;	// 战斗力
		public int Level;	// 玩家等级
		public int Job;	// 职业
		public int Dress;	// 衣服
		public int Weapon;	// 武器
		public int FashionHead;	// 时装头
		public int FashionWeapon;	// 时装武器
		public int FashionDress;	// 时装衣服
		public int WuHunID;	// 灵兽ID
		public int ShenBing;	// 神兵ID
		public int Gender;	// 性别
		public int Icon;	// 头像
		public int Wing;	// 翅膀
		public int SuitFlag;	// 套装标识
		public int MagicKey;	// 法宝
		public int ZhenfaId;	// 阵法
		public int MagicKeyStar;	// 魔宠星级
		public double Atk;	// atk
		public double HP;	// hp
		public double SubDef;	// subdef
		public double Def;	// def
		public double Cri;	// cri
		public double CriValue;	// crivalue
		public double AbsAtk;	// absatk
		public double DefCri;	// defcri
		public double SubCri;	// subcri
		public double DmgSub;	// dmgsub
		public double DmgAdd;	// dmgadd
		public int[] SkillIDs = new int[10];	// 技能

		public override void unpack(NetReadBuffer buffer)
		{
			DataSize = buffer.ReadUInt32();
			Guid = buffer.ReadInt64();
			ObjType = buffer.ReadUInt8();
			PosX = buffer.ReadInt32();
			PosY = buffer.ReadInt32();
			Dir = buffer.ReadInt32();
			RoleID = buffer.ReadInt64();
			buffer.ReadBytes(Name);
			Power = buffer.ReadInt64();
			Level = buffer.ReadInt32();
			Job = buffer.ReadInt32();
			Dress = buffer.ReadInt32();
			Weapon = buffer.ReadInt32();
			FashionHead = buffer.ReadInt32();
			FashionWeapon = buffer.ReadInt32();
			FashionDress = buffer.ReadInt32();
			WuHunID = buffer.ReadInt32();
			ShenBing = buffer.ReadInt32();
			Gender = buffer.ReadInt32();
			Icon = buffer.ReadInt32();
			Wing = buffer.ReadInt32();
			SuitFlag = buffer.ReadInt32();
			MagicKey = buffer.ReadInt32();
			ZhenfaId = buffer.ReadInt32();
			MagicKeyStar = buffer.ReadInt32();
			Atk = buffer.ReadDouble();
			HP = buffer.ReadDouble();
			SubDef = buffer.ReadDouble();
			Def = buffer.ReadDouble();
			Cri = buffer.ReadDouble();
			CriValue = buffer.ReadDouble();
			AbsAtk = buffer.ReadDouble();
			DefCri = buffer.ReadDouble();
			SubCri = buffer.ReadDouble();
			DmgSub = buffer.ReadDouble();
			DmgAdd = buffer.ReadDouble();
			for (int i = 0; i < 10; i++)
			{
				int __item = buffer.ReadInt32();
				SkillIDs[i]=__item;
			}
		}
	}
}

