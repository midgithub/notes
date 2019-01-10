using UnityEngine;
using System.Collections;
using System.IO;
using XLua;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Reflection;

namespace SG
{

    public enum LogLevel
    {
        INFO,
        WARNING,
        ERROR,
    }

    [LuaCallCSharp]
[Hotfix]
    public class LogMgr : MonoBehaviour
    {

        ////是否通关过后 还继续触发  false 为关闭
        //public static bool isTriggerMovie = false ;

        //用于打开   和关闭日志输出    false 为关闭
        //配置摞到ClientSetting里面，//日志输出开关 0 关闭 1打开  IS_DEBUG= 0
        public static bool m_isDebug = true;

        //使用
        public static readonly bool useAssetBoundle = false;


        public static bool isDebug
        {
            get { return m_isDebug; }
        }

        public static void SetLogSwitch(bool value)
        {
            m_isDebug = value;
            if (m_isDebug == false)
            {
                //Application.targetFrameRate = 60;//此处限定60帧 

                UnityLog = new MsgLog(LogNull);
                UnityError = new MsgLog(LogErrorNull);
                UnityWarning = new MsgLog(LogWarningNull);

            }
            else
            {
                Application.targetFrameRate = 0;//  调试状态下不锁

                UnityLog = new MsgLog(UnityEngine.Debug.Log);
                UnityError = new MsgLog(UnityEngine.Debug.LogError);
                UnityWarning = new MsgLog(UnityEngine.Debug.LogWarning);
            }
        }


        //add by lzp 12:19 2015/8/15 GM fps 开关配置 true为打开，false关闭
        public static bool mGMSwitch = true;
        public static bool GMSwitch
        {
            get { return mGMSwitch; }
            set { mGMSwitch = value; }
        }

        /// <summary>
        /// 是否在物品弹窗先物品ID。
        /// </summary>
        public static bool mDebugInfo = true;
        public static bool DebugInfo
        {
            get { return mDebugInfo; }
            set { mDebugInfo = value; }
        }


        private static StreamWriter writer = null;


        public delegate void MsgLog(string strMsg);

        public static MsgLog UnityLog = new MsgLog(UnityEngine.Debug.Log);
        public static MsgLog UnityError = new MsgLog(UnityEngine.Debug.LogError);
        public static MsgLog UnityWarning = new MsgLog(UnityEngine.Debug.LogWarning);

        public static System.Collections.Generic.Queue<string> sQueueMsg = new System.Collections.Generic.Queue<string>();

        public void ThreadLog(string msg)
        {
            LogMgr.sQueueMsg.Enqueue(msg);
        }

        void Start()
        {
            init();
            LoginInit();
        }

        void Update()
        {
            if (sQueueMsg.Count > 0)
            {
                string msg = sQueueMsg.Peek();
                sQueueMsg.Dequeue();
                LogMgr.Log(msg);
            }
        }

        public static void LoginInit()
        {
            LogMgr.GMSwitch = ClientSetting.Instance.GetBoolValue("GMSwitch");
            LogMgr.DebugInfo = ClientSetting.Instance.GetBoolValue("DebugInfo");
            m_isDebug = ClientSetting.Instance.GetBoolValue("IS_DEBUG");
            //isTriggerMovie = ClientSetting.Instance.GetBoolValue("TriggerMovie");
            bool bErrorpt = ClientSetting.Instance.GetBoolValue("IS_PT_ERROR"); ;
            if (m_isDebug == false)
            {
                UnityLog = new MsgLog(LogNull);
                UnityError = new MsgLog(LogErrorNull);
                UnityWarning = new MsgLog(LogWarningNull);
                if (bErrorpt)
                {
                    UnityError = new MsgLog(UnityEngine.Debug.LogError);
                }
            }
            else
            {

                //UnityLog = new MsgLog(unityLog);
                UnityLog = new MsgLog(UnityEngine.Debug.Log);
                UnityError = new MsgLog(UnityEngine.Debug.LogError);
                UnityWarning = new MsgLog(UnityEngine.Debug.LogWarning);
            }
        }

        public static void init()
        {
            if (m_isDebug == false)
            {
                UnityLog = new MsgLog(LogNull);
                UnityError = new MsgLog(LogErrorNull);
                UnityWarning = new MsgLog(LogWarningNull);

            }
            else
            {

                //UnityLog = new MsgLog(unityLog);
                UnityLog = new MsgLog(UnityEngine.Debug.Log);
                UnityError = new MsgLog(UnityEngine.Debug.LogError);
                UnityWarning = new MsgLog(UnityEngine.Debug.LogWarning);
            }
        }

        // logLevel: log级别（info， warning， error）
        // strChannel: 频道
        // strMessage: log具体信息
        public void Log(LogLevel logLevel, string strChannel, string strMessage)
        {
            // 只在windows开发环境下运行才输出日志
            if (Application.platform != RuntimePlatform.WindowsEditor)
                return;

            if (writer == null)
                InitWriter();

            string strLog = " <" + (int)(Time.time * 1000) + ", " + logLevel.ToString() + ", " + strChannel + ">: " + strMessage;

            writer.WriteLine(strLog);
            writer.Flush();
        }


        private static void WriteLog(LogLevel logLevel, string strChannel, string strMessage)
        {
            // 只在windows开发环境下运行才输出日志
            if (Application.platform != RuntimePlatform.WindowsEditor)
                return;

            if (writer == null)
                InitWriter();

            string strLog = " <" + (int)(Time.time * 1000) + ", " + logLevel.ToString() + ", " + strChannel + ">: " + strMessage;

            writer.WriteLine(strLog);
            writer.Flush();
        }

        //空接口  用于关闭日志
        // strMessage: log具体信息
        public static void LogNull(string strMessage) { }        // 
        public static void LogErrorNull(string strMessage) { }
        public static void LogWarningNull(string strMessage) { }

        private static void InitWriter()
        {
            // 只在windows开发环境下运行才输出日志
            if (Application.platform != RuntimePlatform.WindowsEditor)
                return;

            string projectPath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/Assets"));
            string targetPath = projectPath + "/Log";
            Directory.CreateDirectory(targetPath);
            targetPath += "/log.log";

            writer = new StreamWriter(targetPath, false);
        }







        enum LOG_TYPE
        {
            DEGUG_LOG = 0,
            WARNING_LOG,
            ERROR_LOG
        }

        public delegate void OnOutputLog(string _msg);
        static public OnOutputLog onOutputLog = null;
        private static void WriteLog(string msg, LOG_TYPE type, bool _showInConsole = false)
        {
            switch (type)
            {
                case LOG_TYPE.DEGUG_LOG:
                    Debug.Log(msg);
                    break;
                case LOG_TYPE.ERROR_LOG:
                    Debug.LogError(msg);
                    break;
                case LOG_TYPE.WARNING_LOG:
                    Debug.LogWarning(msg);
                    break;
            }
        }

        public static void ErrorLog(string fort, params object[] areges)
        {
            if (!m_isDebug) return;
            if (areges.Length > 0)
            {
                string msg = string.Format(fort, areges);
                WriteLog(msg, LOG_TYPE.ERROR_LOG, true);
            }
            else
            {
                WriteLog(fort, LOG_TYPE.ERROR_LOG, true);
            }

        }
        public static void WarningLog(string fort, params object[] areges)
        {
            if (!m_isDebug) return;
            if (areges.Length > 0)
            {
                string msg = string.Format(fort, areges);
                WriteLog(msg, LOG_TYPE.WARNING_LOG, true);
            }
            else
            {
                WriteLog(fort, LOG_TYPE.WARNING_LOG, true);
            }
        }
        public static void DebugLog(string fort, params object[] areges)
        {
            if (!m_isDebug) return;
            if (areges.Length > 0)
            {
                string msg = string.Format(fort, areges);
                WriteLog(msg, LOG_TYPE.DEGUG_LOG, true);
            }
            else
            {
                WriteLog(fort, LOG_TYPE.DEGUG_LOG, true);
            }
        }

        private static void ErrorLog(string msg)
        {
            WriteLog(msg, LOG_TYPE.ERROR_LOG);
        }

        private static void WarningLog(string msg)
        {
            WriteLog(msg, LOG_TYPE.WARNING_LOG);
        }

        public static void DebugLog(string msg)
        {
            if (!m_isDebug) return;
            WriteLog(msg, LOG_TYPE.DEGUG_LOG);
        }

        public static void Log(string logString, string stackTrace, LogType type)
        {
            if (!m_isDebug) return;
            switch (type)
            {
                case LogType.Log:
                    LogMgr.DebugLog(logString);
                    break;
                case LogType.Warning:
                    LogMgr.WarningLog(logString);
                    break;
                case LogType.Error:
                    LogMgr.ErrorLog(logString);
                    break;
            }
        }









        #region--------------------Add By XuXiang--------------------

        /// <summary>
        /// 日志记录。
        /// </summary>
[Hotfix]
        public class LogRecord
        {
            public LogType Type;
            public string Condition;
            public string StackTrace;
        }

        public static void StartSaveLog()
        {
            Application.logMessageReceived += LogCallback;
        }

        public static void StopSaveLog()
        {
            Application.logMessageReceived -= LogCallback;
        }

        private static void LogCallback(string condition, string stackTrace, LogType type)
        {
            //日志和警告过滤
            if (type == LogType.Log || type == LogType.Warning)
            {
                return;
            }

            LogRecord save = Records.Count >= MaxLogRecord ? Records.Dequeue() : new LogRecord();
            save.Type = type;
            save.Condition = condition;
            save.StackTrace = stackTrace;
            Records.Enqueue(save);

            EventParameter ep = EventParameter.Get();
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_RECORD_LOG, ep);
        }

        public static string GetLosShow()
        {
            StringBuilder sb = new StringBuilder();
            var e = Records.GetEnumerator();
            while (e.MoveNext())
            {
                LogRecord record = e.Current;
                sb.AppendLine(string.Format("{0} - {1}", record.Type, record.Condition));
                if (record.Type == LogType.Exception)
                {
                    sb.AppendLine(record.StackTrace);
                }
            }
            e.Dispose();

            return sb.ToString();
        }

        /// <summary>
        /// 输出日志。
        /// </summary>
        /// <param name="format">日志格式。</param>
        /// <param name="args">日志参数。</param>
        public static void Log(string format, params object[] args)
        {
            if (m_isDebug)
            {
                Debug.LogFormat(format, args);
            }
        }

        /// <summary>
        /// 输出警告日志。
        /// </summary>
        /// <param name="format">日志格式。</param>
        /// <param name="args">日志参数。</param>
        public static void LogWarning(string format, params object[] args)
        {
            if (m_isDebug)
            {
                Debug.LogWarningFormat(format, args);
            }
        }

        /// <summary>
        /// 输出错误日志。
        /// </summary>
        /// <param name="format">日志格式。</param>
        /// <param name="args">日志参数。</param>
        public static void LogError(string format, params object[] args)
        {
            if (m_isDebug)
            {
                Debug.LogErrorFormat(format, args);
            }
        }

        /// <summary>
        /// 是否启用自动AI日志。
        /// </summary>
        public static bool ShowAILog = false;

        /// <summary>
        /// 输出自动AI日志。
        /// </summary>
        public static void LogAI(string format, params object[] args)
        {
            if (ShowAILog)
            {
                LogWarning(format, args);
            }
        }

        /// <summary>
        /// 保存堆栈。
        /// </summary>
        /// <param name="text">堆栈内容。</param>
        public static void SaveStackTrace(string text)
        {
#if UNITY_EDITOR
            //追加到文件
            string file = Application.dataPath + "/../StackTrace.log";
            FileStream stream = new FileStream(file, FileMode.OpenOrCreate);
            stream.Position = stream.Length;            //跳到末尾
            StreamWriter writer = new StreamWriter(stream);
            string t = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            writer.WriteLine(t);
            writer.WriteLine();
            writer.Write(text);
            writer.WriteLine();
            writer.WriteLine();
            writer.WriteLine();
            writer.Flush();
            writer.Close();
            writer.Dispose();
            writer = null;
            stream = null;
#endif
        }

#if UNITY_EDITOR

        [UnityEditor.Callbacks.OnOpenAssetAttribute(0)]
        static bool OnOpenAsset(int instanceID, int line)
        {
            //判断选中的文本是否还有类名
            string stackTrace = GetSourceText();
            if (!string.IsNullOrEmpty(stackTrace) && stackTrace.Contains(ClassName))
            {
                //匹配[文件路径:行号]部分 过滤出调用堆栈
                Match matches = Regex.Match(stackTrace, @"\(at (.+)\)", RegexOptions.IgnoreCase);
                bool xlua = false;
                while (matches.Success)
                {
                    //找到不是自身文件输出的那行即为日志输出调用行
                    string pathline = matches.Groups[1].Value;
                    if (!pathline.Contains(FileName))
                    {
                        //分析出文件路径与行号并用编辑器打开并指定对应行
                        int splitIndex = pathline.LastIndexOf(":");
                        string path = pathline.Substring(0, splitIndex);        //文件相对路径
                        line = System.Convert.ToInt32(pathline.Substring(splitIndex + 1));      //行号
                        string root = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("Assets"));    //根路径
                        string fullPath = (root + path).Replace('/', '\\');         //完整路径
                        if (xlua)
                        {
                            if (fullPath.EndsWith("LuaBehaviour.cs"))
                            {
                                UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(fullPath, line);   //打开并标记
                                break;
                            }
                        }
                        else
                        {
                            if (fullPath.EndsWith("XLua\\Src\\MethodWarpsCache.cs"))
                            {
                                xlua = true;
                            }
                            else
                            {
                                UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(fullPath, line);   //打开并标记
                                break;
                            }
                        }
                    }
                    matches = matches.NextMatch();
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取控制台选中的文本。
        /// </summary>
        /// <returns>制台选中的文本。</returns>
        public static string GetSourceText()
        {
            //获取控制台窗口
            Type ConsoleWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow");
            FieldInfo info = ConsoleWindowType.GetField("ms_ConsoleWindow", BindingFlags.Static | BindingFlags.NonPublic);
            object wnd = info.GetValue(null);
            if (wnd != null && (object)EditorWindow.focusedWindow == wnd)
            {
                //获取当前选中的文本
                info = ConsoleWindowType.GetField("m_ActiveText", BindingFlags.Instance | BindingFlags.NonPublic);
                string activeText = info.GetValue(wnd).ToString();

                return activeText;
            }

            return null;
        }
#endif

#if UNITY_EDITOR
        /// <summary>
        /// 获取或设置是否启用日志。
        /// </summary>
        public static bool EnableLog = true;
#else
        /// <summary>
        /// 获取或设置是否启用日志。
        /// </summary>
        public static bool EnableLog = false;
#endif

        /// <summary>
        /// 类名。
        /// </summary>
        public static string ClassName = "SG.LogMgr";

        /// <summary>
        /// 文件名称。
        /// </summary>
        public static string FileName = "LogMgr.cs";

        /// <summary>
        /// 最大记录的日志数量。
        /// </summary>
        public static int MaxLogRecord = 20;

        /// <summary>
        /// 日志列表。
        /// </summary>
        public static Queue<LogRecord> Records = new Queue<LogRecord>();

        #endregion
    }
}

