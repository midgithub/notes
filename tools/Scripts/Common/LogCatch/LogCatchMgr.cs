
using XLua;
using UnityEngine;
using System.Collections;

[Hotfix]
public class LogCatchMgr : MonoBehaviour
{

	void Awake ()
	{
        gameObject.AddComponent<BuglyInit>();
        BuglyAgent.RegisterLogCallback(LogCallback);
	}

    public void SetUserId(string id)
    {
        BuglyAgent.SetUserId(id);
    }

    public void LogCallback(string condition, string stackTrace, LogType type)
    {

    }
}


