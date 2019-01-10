using XLua;


namespace SG
{
[Hotfix]
	public class NetMsgLoginDef
	{

		public const byte LoginSystem    		 = 0;		//默认子系统//.

        public enum enLoginSystem	{
	        cLogin                        = 1,		//login to the game (byte accountid,byte: actorid)
	        cSelectServer				  = 2,		//选择服务器.
            cCreateAccount                = 3,      //创建账号

	        cCreateActor				  = 1,		//创建角色.
	        cDeleteActor				  = 3,		//删除角色.
	        cActorList					  = 5,		//角色列表.
	        cEnterGame					  = 7,		//进入游戏.
	        cRandomName					  = 9,		//随机名字.

            sUserLogin                    = 1,      //登陆结果.
	        sSelectServerAck			  = 2,		//选择服务器.
	        sCreateActor				  = 2,		//创建角色.
            sCreateAccount                = 3,      //创建账号
	        sDeleteActor				  = 4,		//删除角色.
	        sActorList					  = 6,		//角色列表.
	        sEnterGame					  = 8,		//进入游戏.
	        sRandomName					  = 10,		//随机名字.
        }
		public enum logintype
		{
			dev = 1,    // 开发.
			sdk = 4,    // 渠道.
		}
    }
};


