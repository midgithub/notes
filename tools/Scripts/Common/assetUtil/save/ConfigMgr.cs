 
using XLua;
using System.Text;
using UnityEngine;

[Hotfix]
public class ConfigMgr : ConfigBase
{
    static ConfigMgr mInstance;
    public static ConfigMgr Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = new ConfigMgr(@"/config.json");
            }
            return mInstance;
        }
    } 


    public ConfigMgr(string _fileName)
        : base(_fileName)
    {

    }

    public override void init()
    {
        base.init();
     }




     
}

