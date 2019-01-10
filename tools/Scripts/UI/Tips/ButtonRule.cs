using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using XLua;



[CSharpCallLua]
public delegate void ShowInfo(int id);
[LuaCallCSharp]
[Hotfix]
public class ButtonRule : MonoBehaviour
{
    public int m_RuleId;
    void Start()
    {
        Button button = this.GetComponent<Button>();
        if(button != null)
        {
            button.onClick.AddListener(OnRuleClick);
        }
    }

    void OnRuleClick()
    {
        PanelBase panel = MainPanelMgr.Instance.ShowDialog("UIRule");
        if (null != panel)
        {
            XLua.LuaTable tbl = panel.GetEnvTable();
            ShowInfo setFunc = null;
            if (null != tbl)
            {
                tbl.Get("ShowInfo", out setFunc);
            }

            if (null != setFunc)
            {
                setFunc(m_RuleId);
            }
        }

    }
}

