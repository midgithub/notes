using XLua;
using SG;
using UnityEngine;

[Hotfix]
public class xgPush  {

	// Use this for initialization
   static public void PushDeviceTokenInfo() {
#if UNITY_ANDROID && !UNITY_EDITOR
        PushManager.Instance.EnableDebug(true);
		PushManager.Instance.SetAccessId(2100306779);
		PushManager.Instance.SetAccessKey("A8896KFNPP1B");
		PushManager.Instance.StartPushService(delegate(bool success, string msg)
		{
           // Debug.Log("========="+ success);

            if (success)
		    {
                string str = PushManager.Instance.GetToken();
                Debug.Log(str);
                byte[] byteArray = System.Text.Encoding.Default.GetBytes(str);
                NetLogicGame.Instance.SendDeviceTokenInfo(byteArray);
            }
		});
#endif

    }
}

