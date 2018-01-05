using UnityEngine;
using System.Collections;

public class ErrorDisplay : MonoBehaviour {

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }
    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    private string m_logs;
    void HandleLog(string logString, string stackTrace, LogType type)
    {
        m_logs += logString + "\n";
    }

    public bool Log;
    private Vector2 m_scroll;
    GUIStyle st;

    void OnGUI()
    {
        if (!Log)
            return;

        if(null == st)
        {
            st = new GUIStyle();
            st.normal.background = null;
            st.normal.textColor = new Color(1, 0, 0);
            st.fontSize = 30;
        }
            
        m_scroll = GUILayout.BeginScrollView(m_scroll);
        GUILayout.Label(m_logs,st);
        GUILayout.EndScrollView();
    }
}
