using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;

[LuaCallCSharp]
public enum ThirdParty_ExtNameType
{
     ACTION_ENTER_SERVER,
     ACTION_LEVEL_UP,
     ACTION_CREATE_ROLE,

     ACTION,

     ROLE_ID,
     ROLE_NAME,
     ROLE_LEVEL,
     ZONE_ID,
     ZONE_NAME ,
     BALANCE,
     VIP,
     PARTYNAME,
     
     APP_VERSION,
     APP_RES_VERSION,
     APP_SERVERlIST,
     APP_DEFAULTLIST,
     CHANNELID,
};

[LuaCallCSharp]
public interface IThirdPartySDK  {

    void Init(string gameObject);

    void Login();

    void Logout();

    void Pay(int configID);

    void Exit();

    void SetExtData(Dictionary<string, string> data);

    void ReleaseResource();

    string GetExtName(ThirdParty_ExtNameType type);

    void EnterUserCenter();

    bool IsLogin();

    string appversion();
}



