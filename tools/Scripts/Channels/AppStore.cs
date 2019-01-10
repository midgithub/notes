using XLua;
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Net;
using System.IO;
using System.Text;
[Hotfix]
public class AppStore  {
#if UNITY_IOS
    //[DllImport ("__Internal")]
    //public static extern bool CanPurchase();//CanBuy?
    //[DllImport ("__Internal")]
    //public static extern void InitAndLoadStore(string pids);
    //[DllImport ("__Internal")]
    //public static extern void BuySomething(string PID, string ObjName, string FunName, string ValName, string CallbackObject, string SuccessFunction, string FailureFunction, string RestoreFunction, string OnFunction,string CallbackInfo);
    //[DllImport ("__Internal")]
    //public static extern void RestoreBuy();
#endif

                           public GameObject buyfail;
	public GameObject buysuccess;
    public GameObject NetFail;
	public GameObject ReOver;

	void GoOn(){
		GameObject tmp = null;
		tmp = GameObject.Find("paymoneywaittingmask(Clone)");
		if( tmp != null )
            GameObject.Destroy(tmp);
	}
	//BuySuccess
	void GoOnSuccess(){
		GameObject tmp = null;
		tmp = GameObject.Find("paymoneywaittingmask(Clone)");
		if( tmp != null )
            GameObject.Destroy(tmp);
        GameObject.Instantiate(buysuccess, buysuccess.transform.position, buysuccess.transform.rotation);
	}
	//BuyFail
	void GoOnFail(){
		GameObject tmp = null;
		tmp = GameObject.Find("paymoneywaittingmask(Clone)");
		if( tmp != null )
            GameObject.Destroy(tmp);
        GameObject.Instantiate(buyfail, buyfail.transform.position, buyfail.transform.rotation);
	}
	void GoOnRestore(){
		GameObject tmp = null;
		tmp = GameObject.Find("paymoneywaittingmask(Clone)");
		if( tmp != null )
            GameObject.Destroy(tmp);
        GameObject.Instantiate(ReOver, ReOver.transform.position, ReOver.transform.rotation);
	}
    void CheckNet()
    {
		GameObject tmp = null;
		tmp = GameObject.Find("paymoneywaittingmask(Clone)");
		if( tmp != null )
            GameObject.Destroy(tmp);
        GameObject.Instantiate(NetFail, NetFail.transform.position, NetFail.transform.rotation);
    }

    static public string HttpGet(string Url, string postDataStr)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
        request.Method = "GET";
        request.ContentType = "text/html;charset=UTF-8";
        request.Timeout = 1000;
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        Stream myResponseStream = response.GetResponseStream();
        StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
        string retString = myStreamReader.ReadToEnd();
        myStreamReader.Close();
        myResponseStream.Close();

        return retString;
    }
}

