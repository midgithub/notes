using System;
using UnityEngine;
using System.IO;
using System.Collections;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using XLua;
using System.Threading;
using SG;
using SenLib;

[Hotfix]
public class CLoadedPackageInfo
{
    public int m_nID = 0;
    public int m_nType = 0;
    public string m_nUrl = string.Empty;
}

[Hotfix]
public class CSubPackageUnit
{
    public int id { private set; get; }
    public int type { private set; get; }   //1：登入下载  0：后台下载
    public string desc1 { private set; get; }
    public string desc2 { private set; get; }
    public string desc3 { private set; get; }
    public string url { private set; get; }

    public void Init(XML node)
    {
        if (node == null)
        {
            return;
        }

        id = node.GetInt("id");
        type = node.GetInt("type");
        desc1 = node.GetValue("desc1");
        desc2 = node.GetValue("desc2");
        desc3 = node.GetValue("desc3");
        url = CurrentBundleVersion.ResUpdateCdnUrl + node.GetValue("url");
    }
}

[Hotfix]
public class CASyncSubPackageAction
{
    public bool m_bAction = false;
    public int m_nID = 0;
    public long m_nData1 = 0;
    public long m_nData2 = 0;
}

[Hotfix]
public class CASyncSubPackageThreadParam
{
    public int m_nID = 0;
    public string m_strURL = "";
    public string m_strSavePath = "";
    public string m_strSaveDir = "";
}

[Hotfix]
public class CVersionManager : MonoBehaviour
{
    public static CVersionManager Instance;

    private WaitForEndOfFrame m_wait;
    public event Action<VersionProgressEvent> OnVersionProgressEvent;
    private VersionProgressEvent m_eventVersion;
    private void TriggerVersionProgressEvent()
    {
        if (null != OnVersionProgressEvent)
        {
            OnVersionProgressEvent(m_eventVersion);
        }
    }

    // 特殊字符： 配置中{AppName}替换为CurrentBundleVersion.productName
    private const string PRE_STRING_APPNAME = "{AppName}";
    // 特殊字符： 配置中{AppOS}替换为 操作系统平台名（android\ios\windows）
    private const string PRE_STRING_APPOS = "{AppOS}";
    // 特殊字符： 配置中{AppVersion}替换为程序版本号
    private const string PRE_STRING_APPVERSION = "{AppVersion}";

    private const string SubpackageXmlPath = "package/{AppOS}/pack_download.xml";               //pack xml 路径
    private const string ResVersionFilePath = "update/res/{AppOS}/{AppVersion}/";               //素材扩展名
    private const string ApkUrlFile = "update/pak/{AppOS}/{AppName}/{AppVersion}/apkUrl.txt";             //记录apk包下载地址文件
    private const string BackstagePath = "backstage/{AppOS}/{AppName}/{AppVersion}/backstage.csv";               //后台标识

    private string ReplaceEscapeCharacter(string strValue)
    {
        string strValueNew = strValue;

        strValueNew = strValueNew.Replace(PRE_STRING_APPNAME, Application.bundleIdentifier);
        strValueNew = strValueNew.Replace(PRE_STRING_APPOS, Util.GetOS());
        strValueNew = strValueNew.Replace(PRE_STRING_APPVERSION, GetAppVersion());

        return strValueNew;
    }

    private const string STR_RES_MAX_RESVERSION_FILE = "MaxResVersion.txt";
    private const string STR_RES_RESVERSION_INFOS_FILE = "{0:d}/ResVersion{1:d}.xml";
    private const string STR_RES_DOWNINFO_FILE = "{0:d}/{1}";

    // 资源版本信息文件
    private const string STR_VERSION_FILE = "DirVersion/resversion.ver";
    public const string STR_VERSION_DIR = "DirVersion";  //解压目录
    private const string STR_UPDATE_DIR = "DirUpdate";    

    // 文件根目录
    private int m_localResVersion = 0;      //本地资源版本
    private int m_urlResVersion = 0;       //远程资源版本
    private string m_strResPackageURL = string.Empty; //资源网络路径 "http://xxx/normal/update/res/Android/3.3/1003/1000_1003.LTZip"
    private string m_strResPackageName = string.Empty;    //1000_1003.LTZip
    private string m_strLocalResPackageFullPath = string.Empty;   //本地路径下载存放 update/1000_1003.LTZip 

    private string m_publicIp = string.Empty;
    private const string STR_TEST_USER_FILE = "TestUser.xml";
    private bool m_checkTestUsers = false;

    public bool ContinueUpdateRes
    {
        private set;
        get;
    }

    private void Awake()
    {
        Instance = this;
        transform.name = "CVersionManager";
        DontDestroyOnLoad(gameObject);
#if UNITY_IOS
        AppConst.BundleFlag = ClientSetting.Instance.GetStringValue("BundleFlag");
        ClientSetting.Instance.ReLoadClientSettingData();
#endif

        WinUpdate.OnSplashOver += CheckExtractResource;
        WinUpdate.ShowUI();

        ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;

        m_eventVersion = new VersionProgressEvent();
        m_wait = new WaitForEndOfFrame();

        m_localResVersion = 0;
        m_urlResVersion = 0;
        m_strResPackageURL = string.Empty;
        m_strResPackageName = string.Empty;
        m_strLocalResPackageFullPath = string.Empty;

        //CheckExtractResource();
    }

    private void OnDestroy()
    {
        WinUpdate.OnSplashOver -= CheckExtractResource;
    }

    void Update()
    {
        CheckAsyncFPointDown();
    }

    public bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }

    #region 解压
    private void CheckExtractResource()
    {
        string strVersionFilePath = Util.DataPath + STR_VERSION_FILE;
        bool extracted = File.Exists(strVersionFilePath);
        if (extracted || AppConst.DebugMode)
        {
            CheckResVersion();
            return;   //文件已经解压过了
        }
        StartCoroutine(OnExtractResource());    //启动释放协成 
    }

    IEnumerator OnExtractResource()
    {
        string dataPath = Util.DataPath;  //解压到数据目录
        string streamPath = Util.AppContentPath(); //游戏包资源目录

        if (Directory.Exists(dataPath)) Directory.Delete(dataPath, true);
        Directory.CreateDirectory(dataPath);

        string infile = streamPath + "files.txt";
        string outfile = dataPath + "files.txt";
        if (File.Exists(outfile)) File.Delete(outfile);
        yield return ExtractFile(infile, outfile);
        yield return m_wait;

        //释放所有文件到数据目录
        string[] files = File.ReadAllLines(outfile);
        m_eventVersion.state = EVersionState.Extracting;
        m_eventVersion.curPro = 0;
        m_eventVersion.dataLength = files.Length;
        TriggerVersionProgressEvent();

        foreach (var file in files)
        {
            string[] fs = file.Split('|');
            infile = streamPath + fs[0];  //
            outfile = dataPath + fs[0];

            Util.Log("正在解包文件:>" + infile);

            string dir = Path.GetDirectoryName(outfile);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            yield return ExtractFile(infile, outfile);

            m_eventVersion.curPro += 1;
            TriggerVersionProgressEvent();
            yield return m_wait;
        }

        yield return new WaitForSeconds(0.1f);

        m_eventVersion.state = EVersionState.ExtracSuccess;
        TriggerVersionProgressEvent();

        // 写资源版本号
        UpdateLocalResVersionFile(CurrentBundleVersion.ResVersion);
        //释放完成，开始启动更新资源
        CheckResVersion();
    }

    private void UpdateLocalResVersionFile(int newVersion)
    {
        string versionDir = Util.DataPath + STR_VERSION_DIR;
        DirectoryInfo dirOutDir = new DirectoryInfo(versionDir);
        if (!dirOutDir.Exists)
        {
            Directory.CreateDirectory(versionDir);
        }

        string strVersionPath = Util.DataPath + STR_VERSION_FILE;
        if (File.Exists(strVersionPath))
        {
            File.Delete(strVersionPath);
        }

        string strContent = string.Format("{0},{1}", GetAppVersion(), newVersion);
        File.WriteAllText(strVersionPath, strContent);
    }

    IEnumerator ExtractFile(string infile, string outfile)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            WWW www = new WWW(infile);
            yield return www;

            if (www.isDone)
            {
                File.WriteAllBytes(outfile, www.bytes);
            }
            yield return 0;
        }
        else
        {
            if (File.Exists(outfile))
            {
                File.Delete(outfile);
            }
            File.Copy(infile, outfile, true);
        }
    }
    #endregion

    #region 资源更新

    // 获取程序版本
    public string GetAppVersionClient()
    {
#if UNITY_EDITOR||UNITY_ANDROID
        Debug.Log("AppVersionClient: " + Application.version);
        return Application.version;
#elif UNITY_IOS
        Debug.Log("AppVersionClient: " + SDKMgr.Instance.appversion());
        return SDKMgr.Instance.appversion();
#else
         return "";
#endif

    }

    // 获取程序版本
    public string GetAppVersion()
    {
        Util.LogWarning("AppVersion: " + CurrentBundleVersion.AppVersion);
        return CurrentBundleVersion.AppVersion;
    }

    //
    public int GetLocalResVersion()
    {
        if(m_localResVersion == 0)
        {
            InitLocalResVersion();
        }

        return m_localResVersion;
    }

    // 获取本地资源版本
    public void InitLocalResVersion()
    {
        if (m_localResVersion != 0)
        {
            return;
        }

        // 尝试读取外部资源版本号
        string strVersionPath = Util.DataPath + STR_VERSION_FILE;
        if (!File.Exists(strVersionPath))
        {
            m_localResVersion = CurrentBundleVersion.ResVersion;
            return;
        }

        string strContent = File.ReadAllText(strVersionPath);
        strContent = strContent.Replace("\r", "");
        strContent = strContent.Replace("\n", "");

        string[] strLines = strContent.Split(',');

        if (strLines.Length < 2)
        {
            m_localResVersion = CurrentBundleVersion.ResVersion;
            return;
        }

        if (!strLines[0].Equals(GetAppVersion()))
        {
            m_localResVersion = CurrentBundleVersion.ResVersion;
            return;
        }

        try
        {
            m_localResVersion = Convert.ToInt32(strLines[1]);
        }
        catch (System.Exception ex)
        {
            Util.LogError(ex.Message);

            m_localResVersion = CurrentBundleVersion.ResVersion;
        }

        if(m_localResVersion < 1000)
        {
            m_localResVersion = 1000;
        }
        Util.LogWarning("初始化本地资源版本: " + m_localResVersion);
    }

    /// <summary>
    /// 获取apk版本资源文件夹
    /// </summary>
    /// <returns></returns>
    private string GetAppVerionURL()
    {
        string strPath = CurrentBundleVersion.ResUpdateCdnUrl + ReplaceEscapeCharacter(ResVersionFilePath);

        return strPath;
    }

    /// <summary>
    /// 获取apk版本资源文件夹
    /// </summary>
    /// <returns></returns>
    private string GetAppVerionHostURL()
    {
        string strPath = CurrentBundleVersion.ResUpdateHostUrl + ReplaceEscapeCharacter(ResVersionFilePath);

        return strPath;
    }

    /// <summary>
    /// 获取apk下载地址文件
    /// </summary>
    /// <returns></returns>
    private string GetApkUrlFile()
    {
        string strPath = CurrentBundleVersion.ResUpdateCdnUrl + ReplaceEscapeCharacter(ApkUrlFile);
        strPath += GetRandomParam();

        return strPath;
    }

    /// <summary>
    /// 获取后台标识
    /// </summary>
    /// <returns></returns>
    public string GetBackstageClientSet()
    {
        string strPath = CurrentBundleVersion.ResUpdateCdnUrl + ReplaceEscapeCharacter(BackstagePath);
        strPath += GetRandomParam();

        return strPath;
    }

    private string GetRandomParam()
    {
        return string.Format("?t={0}", Util.GetTimeStamp(DateTime.Now).ToString());
    }

    // 资源版本号文件
    private string GetResMaxVersionURL()
    {
        string strPath = GetAppVerionURL() + STR_RES_MAX_RESVERSION_FILE;
        strPath += GetRandomParam();

        Util.LogWarning("GetResMaxVersionURL " + strPath);

        return strPath;
    }

    //
    private string GetTestUserConfigURL()
    {
        string strPath = GetAppVerionURL() + STR_TEST_USER_FILE;
        strPath += GetRandomParam();

        Util.LogWarning("GetTestUserConfigURL " + strPath);

        return strPath;
    }

    // 检测版本更新入口
    public void CheckResVersion()
    {
        InitLocalResVersion();

        if (!AppConst.UpdateMode)
        {
            IsAllSubPackageDownloaded = true;
            OnResourceInited();
            return;
        }

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            m_eventVersion.state = EVersionState.NoInternet;
            TriggerVersionProgressEvent();

            return;
        }

        string content = ClientSetting.Instance.GetStringValue("PublicIp");
        if (!string.IsNullOrEmpty(content))
        {
            m_publicIp = SenLib.Helper.GetIp(content);
            Debug.Log("ip 地址：" + m_publicIp);
        }

        StartCoroutine(LoadBackStageFile());
    }

    private IEnumerator LoadBackStageFile()
    {
        string url = GetBackstageClientSet();
        Util.LogWarning("GetTextFromBackstagePath=  " + url);
        WWW www = new WWW(url);
        yield return www;

        if (www.error != null)
        {
            Util.LogError("GetTextFromBackstagePath 文件失败---------" + url);
        }
        else
        {
            ClientSetting.Instance.ReloadClientBackstage(www.bytes);
        }
        www.Dispose();

        //提审状态下的整包不更新
        if (!ClientSetting.Instance.GetBoolValue("SubPackageType") && ClientSetting.Instance.GetBoolValue("IS_REVIEW"))
        {
            IsAllSubPackageDownloaded = true;
            OnResourceInited();
            yield break;
        }
        else
        {
            StartCoroutine(LoadResVersionFile());
        }
    }

    //下载对应app版本当前资源版本信息
    private IEnumerator LoadResVersionFile()
    {
        string strVersinURL = GetResMaxVersionURL();
        Util.Log("LoadResNewVersion " + " " + strVersinURL);

        WWW www = new WWW(strVersinURL);
        yield return www;

        if (www.error != null)
        {
            Util.LogError("获取apk版本对应远程资源版本文件失败---------" + strVersinURL);
            CheckResVersionNone();   //CheckResVersionFail();
            yield break;
        }
        else
        {
            string strResVersionServer = Encoding.UTF8.GetString(www.bytes);
            StartCoroutine(CompareResVersion(strResVersionServer, true));
        }
        www.Dispose();
    }

    private IEnumerator CompareResVersion(string urlVersion, bool checkTestUsers)
    {
        m_checkTestUsers = checkTestUsers;

        Debug.Log("比对远程资源版本：" + urlVersion);
        string urlAppVersion = "1.0.0";
        m_urlResVersion = 1000; // 最高版本

        string strResVersionServer = urlVersion;

        strResVersionServer = strResVersionServer.TrimEnd();
        strResVersionServer = strResVersionServer.Replace("\r", "");
        strResVersionServer = strResVersionServer.Replace("\n", "");

        string[] strLines = strResVersionServer.Split(',');

        if (strLines.Length < 2)
        {
            Util.LogError("LoadResNewVersion Error By Length " + strResVersionServer);
            CheckResVersionFail();
            yield break;
        }

        if (string.IsNullOrEmpty(strLines[1]))
        {
            Util.LogError("LoadResNewVersion Error By Empty " + strResVersionServer);
            CheckResVersionFail();
            yield break;
        }

        try
        {
            urlAppVersion = strLines[0];
            m_urlResVersion = Convert.ToInt32(strLines[1]);

            if(strLines.Length > 2)
            {
                ContinueUpdateRes = Convert.ToInt32(strLines[2]) > 0;
            }
        }
        catch (System.Exception ex)
        {
            Util.LogError(ex.Message);
            CheckResVersionFail();
            yield break;
        }

        string nCurAppVersion = GetAppVersion();     //检测apk大版本更新
        {
            string[] urlAppInfo = urlAppVersion.Split('.');
            string[] localAppInfo = nCurAppVersion.Split('.');
            if (urlAppInfo.Length < 2 || localAppInfo.Length < 2)
            {
                Util.LogError("app 版本格式错误---");
                CheckResVersionFail();
                yield break;
            }

            if (Convert.ToInt32(urlAppInfo[0]) > Convert.ToInt32(localAppInfo[0]) || Convert.ToInt32(urlAppInfo[1]) > Convert.ToInt32(localAppInfo[1]))
            {
                Util.Log("当前版本：" + localAppInfo);
                Util.Log("发现大版本：" + urlAppVersion);

                string apkUpdateUrl = GetApkUrlFile();
                Util.LogWarning("apkUpdateUrl " + " " + apkUpdateUrl);

                WWW www_apk = new WWW(apkUpdateUrl);
                yield return www_apk;

                if (www_apk.error != null)
                {
                    Util.LogError("获取apk大版本更新地址文件失败---------" + apkUpdateUrl);
                    CheckResVersionFail();
                }
                else
                {
                    string apkDownUrl = Encoding.UTF8.GetString(www_apk.bytes);

                    apkDownUrl = apkDownUrl.TrimEnd();
                    apkDownUrl = apkDownUrl.Replace("\r", "");
                    apkDownUrl = apkDownUrl.Replace("\n", "");

                    m_eventVersion.state = EVersionState.ApkUpdate;
                    m_eventVersion.info = apkDownUrl;
                    TriggerVersionProgressEvent();
                }

                www_apk.Dispose();
                yield break;
            }
        }

        ContinueCheckResUpdate();
    }

    public void ContinueCheckResUpdate()
    {
        if (m_urlResVersion < 1000)
        {
            m_urlResVersion = 1000;
        }

        Util.LogWarning(string.Format("apk{0} 最新资源版本是: {1}", GetAppVersion(), m_urlResVersion));

        if (m_urlResVersion > m_localResVersion)
        {
            Util.LogWarning(string.Format("apk{0} 旧资源版本: {1}", GetAppVersion(), m_localResVersion));
            StartCoroutine(LoadResDownInfoFile(m_localResVersion, m_urlResVersion, false));
        }
        else
        {
            Util.LogWarning(string.Format("apk{0} 已是最新资源版本: {1}", GetAppVersion(), m_localResVersion));
            if (m_checkTestUsers)
            {
                StartCoroutine(DownloadTestUserConfig());
            }
            else
            {
                CheckResVersionNone();
            }
        }
    }

    private IEnumerator DownloadTestUserConfig()
    {
        string strTestUserURL = GetTestUserConfigURL();
        Util.Log("下载文件： " + strTestUserURL);
        WWW wwwTestUser = new WWW(strTestUserURL);  

        yield return wwwTestUser;

        if (wwwTestUser.error != null)
        {
            Util.LogError(wwwTestUser + "下载文件 失败---" + wwwTestUser.error);
            CheckResVersionNone();
            yield break;
        }

        byte[] bytes = wwwTestUser.bytes;

        string strTestVersion = string.Empty;
        List<string> testUsers = new List<string>();

        try
        {
            XML root;
            if (!bytes.TryParse(out root))
            {
                Util.LogError(strTestUserURL + "解析测试配置文件失败---");
                CheckResVersionNone();
                yield break;
            }

            XML testVerion = root.Element("TestVersion");
            if (testVerion == null)
            {
                Util.LogError(strTestUserURL + "解析 TestVersion 失败---");
                CheckResVersionNone();
                yield break;
            }

            strTestVersion = testVerion.GetValue("version");

            foreach (XML childElement in testVerion.Elements())
            {
                string ip = childElement.GetValue("ip");
                if (!testUsers.Contains(ip))
                {
                    testUsers.Add(ip);
                }
            }
        }
        catch (System.Exception ex)
        {
            Util.LogError(ex.Message);
            CheckResVersionNone();
            yield break;
        }

        if (testUsers.Count > 0 && !string.IsNullOrEmpty(strTestVersion))
        {
            //yield return StartCoroutine(GetPublicIpAddress());
            if (!string.IsNullOrEmpty(m_publicIp) && testUsers.Contains(m_publicIp))
            {
                Debug.Log("user test start -----");
                StartCoroutine(CompareResVersion(strTestVersion, false));
                yield break;
            }
        }

        CheckResVersionNone();
    }

    private IEnumerator GetPublicIpAddress()
    {
        string url = ClientSetting.Instance.GetStringValue("CheckIpUrl");
        if (string.IsNullOrEmpty(url))
        {
            yield break;
        }
        Debug.Log("CheckIpUrl: " + url);

        WWW www = new WWW(url);
        yield return www;

        if (www.error != null)
        {
            Util.LogError(string.Format("无法访问 {0}---------", url));
        }
        else
        {
            string content = www.text;
            m_publicIp = SenLib.Helper.GetIp(content);
            Debug.Log("IP 地址：" + m_publicIp);
        }
        www.Dispose();
    }

    //下载具体要下载文件的配置文件
    private IEnumerator LoadResDownInfoFile(int nCurResVersion, int nServerResVersion, bool bTryPermanentHost)
    {
        // 先使用CDN网络读取资源，失败后用永久域名读取资源，再失败提示玩家稍后重试
        string strAppVerionURL = "";
        if (bTryPermanentHost)
        {
            strAppVerionURL = GetAppVerionHostURL();
        }
        else
        {
            strAppVerionURL = GetAppVerionURL();
        }

        string strMaxResURL = strAppVerionURL + string.Format(STR_RES_RESVERSION_INFOS_FILE, nServerResVersion, nServerResVersion);
        Util.Log("下载文件： " + strMaxResURL);
        WWW wwwMaxRes = new WWW(strMaxResURL);  //ResVersion1003.xml

        yield return wwwMaxRes;

        if (wwwMaxRes.error != null)
        {
            Util.LogError(strMaxResURL + "下载文件 失败---" + wwwMaxRes.error);
            OnLoadResDownInfoFileFail(nCurResVersion, nServerResVersion, bTryPermanentHost);
            yield break;
        }

        byte[] bytes = wwwMaxRes.bytes;
        string strDownInfoURL = string.Empty;

        try
        {
            XML root;
            if (!bytes.TryParse(out root))
            {
                Util.LogError(strMaxResURL + "解析 SecurityElementRoot 失败---");
                OnLoadResDownInfoFileFail(nCurResVersion, nServerResVersion, bTryPermanentHost);
                yield break;
            }

            XML appName = root.Element("AppName");
            if (appName == null)
            {
                Util.LogError(strMaxResURL + "解析 AppName 失败---");
                OnLoadResDownInfoFileFail(nCurResVersion, nServerResVersion, bTryPermanentHost);
                yield break;
            }

            foreach (XML childElement in appName.Elements())
            {
                string id = childElement.GetValue("id");
                if (id == null || !id.Equals(nCurResVersion.ToString()))
                {
                    continue;
                }

                strDownInfoURL = childElement.GetValue("downinfo");  //downinfo1000_1003.xml
                break;
            }
        }
        catch (System.Exception ex)
        {
            Util.LogError(ex.Message);
            OnLoadResDownInfoFileFail(nCurResVersion, nServerResVersion, bTryPermanentHost);
            yield break;
        }

        if (string.IsNullOrEmpty(strDownInfoURL))
        {
            Util.LogError(strMaxResURL + "未找到 downinfo.xml 配置信息");
            OnLoadResDownInfoFileFail(nCurResVersion, nServerResVersion, bTryPermanentHost);
            yield break;
        }
        wwwMaxRes.Dispose();

        string strRealPackageURL = strAppVerionURL + string.Format(STR_RES_DOWNINFO_FILE, nServerResVersion, strDownInfoURL);
        Util.Log("下载文件： " + strRealPackageURL);

        WWW wwwPackage = new WWW(strRealPackageURL);
        yield return wwwPackage;

        if (wwwPackage.error != null)
        {
            Util.LogError(strRealPackageURL + "下载文件 失败--- " + wwwPackage.error);
            OnLoadResDownInfoFileFail(nCurResVersion, nServerResVersion, bTryPermanentHost);
            yield break;
        }


        byte[] packageBytes = wwwPackage.bytes;
        bool bExist = false;
        try
        {
            XML root;
            if (!packageBytes.TryParse(out root))
            {
                Util.LogError(strRealPackageURL + "解析 SecurityElementRoot 失败---");
                OnLoadResDownInfoFileFail(nCurResVersion, nServerResVersion, bTryPermanentHost);
                yield break;
            }

            XML node = root.Element("mode");
            if (node != null)
            {
                bExist = true;
                m_strResPackageName = node.GetValue("packetname");
                m_strResPackageURL = node.GetValue("packet");  //完整路径 包的地址   strAppVerionURL + string.Format(STR_RES_DOWNINFO_FILE, nServerResVersion, m_strResPackageName);

                DownloadResNewVersionPackage();
            }
        }
        catch (System.Exception ex)
        {
            Util.LogError(ex.Message);
            OnLoadResDownInfoFileFail(nCurResVersion, nServerResVersion, bTryPermanentHost);
            yield break;
        }

        wwwPackage.Dispose();

        if (!bExist)
        {
            OnLoadResDownInfoFileFail(nCurResVersion, nServerResVersion, bTryPermanentHost);
            yield break;
        }
    }

    //下载具体要下载文件的配置文件 失败 重试一次
    private void OnLoadResDownInfoFileFail(int nCurResVersion, int nServerResVersion, bool bTryPermanentHost)
    {
        if (bTryPermanentHost)
        {
            CheckResVersionFail();
        }
        else
        {
            StartCoroutine(LoadResDownInfoFile(nCurResVersion, nServerResVersion, true));
        }
    }

    // 确定后 下载资源更新包
    private void DownloadResNewVersionPackage()
    {
        string strPackageDir = CreateResUpdateDir();
        m_strLocalResPackageFullPath = strPackageDir + "/" + m_strResPackageName;

        StartCoroutine(ResFPointDown(m_strResPackageURL, m_strLocalResPackageFullPath, false));
    }

    //本地update路径
    private string CreateResUpdateDir()
    {
        string strPackageDir = Util.DataPath + STR_UPDATE_DIR;
        DirectoryInfo dirOutDir = new DirectoryInfo(strPackageDir);
        if (!dirOutDir.Exists)
        {
            Directory.CreateDirectory(strPackageDir);
        }

        return strPackageDir;
    }

    // 资源下载
    private IEnumerator ResFPointDown(string strURL, string strSaveFile, bool bTryPermanentHost)
    {
        System.Net.HttpWebRequest request = null;
        System.IO.FileStream fs = null;
        long countLength = 0;
        Util.LogWarning("FPointDown url:" + strURL);

        //打开上次下载的文件或新建文件 
        long lStartPos = 0;
        yield return m_wait;

        bool timeOut = false;
        try
        {
            request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(strURL);
            request.Timeout = 5000;
            countLength = request.GetResponse().ContentLength;

            if (System.IO.File.Exists(strSaveFile))
            {
                File.Delete(strSaveFile);
                //fs = System.IO.File.OpenWrite(strSaveFile);
                //lStartPos = fs.Length;
                //if (countLength - lStartPos <= 0)
                //{
                //    fs.Close();
                //    ResFPointDownSuccess();
                //    yield break;
                //}
                //fs.Seek(lStartPos, System.IO.SeekOrigin.Current); //移动文件流中的当前指针 
            }
            //else
            {
                fs = new System.IO.FileStream(strSaveFile, System.IO.FileMode.Create);
            }

            if (lStartPos > 0)
            {
                request.AddRange((int)lStartPos); //设置Range值
            }
        }
        catch (System.Exception ex)
        {
            if (request != null)
            {
                request.Abort();
            }

            Util.LogError(ex.Message);
            timeOut = true;

            //ResFPointDownFail(bTryPermanentHost);
            //yield break;
        }

        if (timeOut)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                m_eventVersion.state = EVersionState.NoInternet;
                TriggerVersionProgressEvent();
            }
            else
            {
                StartCoroutine(ResFPointDown(strURL, strSaveFile, bTryPermanentHost));
            }

            yield break;
        }

        if (null == request || null == fs)
        {
            Util.LogError("ResFPointDown null error");
            ResFPointDownFail(bTryPermanentHost);
            yield break;
        }

        System.IO.Stream ns = null;

        try
        {
            ns = request.GetResponse().GetResponseStream();

            if (ns == null)
            {
                Util.LogError("ResFPointDown Error By ns is null");
                ResFPointDownFail(bTryPermanentHost);
                yield break;
            }

            ns.ReadTimeout = 10000;
        }
        catch (System.Exception ex)
        {
            Util.LogError(ex.Message);
            ResFPointDownFail(bTryPermanentHost);
            yield break;
        }

        int len = 128 * 1024;  // 128KB Buff

        byte[] nbytes = new byte[len];
        int nReadSize = 0;
        nReadSize = ns.Read(nbytes, 0, len);
        while (nReadSize > 0)
        {
            try
            {
                fs.Write(nbytes, 0, nReadSize);
                nReadSize = 0;
                nbytes = new byte[len];
            }
            catch (System.Exception ex)
            {
                Util.LogError(ex.Message);
                File.Delete(strSaveFile);
                ns.Close();
                fs.Close();
                if (request != null)
                {
                    request.Abort();
                }

                ResFPointDownFail(bTryPermanentHost);
                yield break;
            }

            try
            {
                nReadSize = ns.Read(nbytes, 0, len);
            }
            catch (System.Exception ex)
            {
                Util.LogError(ex.Message);
                ns.Close();
                fs.Close();
                if (request != null)
                {
                    request.Abort();
                }

                StartCoroutine(ResFPointDown(strURL, strSaveFile, bTryPermanentHost));
                yield break;
            }

            m_eventVersion.state = EVersionState.ResUpdating;
            m_eventVersion.curPro = fs.Length;
            m_eventVersion.dataLength = countLength;
            TriggerVersionProgressEvent();

            yield return m_wait;
        }

        if (fs.Length < countLength)   //网络断开，数据读取
        {
            Util.LogError("资源更新读取流数据出错-----");
            ns.Close();
            fs.Close();
            if (request != null)
            {
                request.Abort();
            }

            //ResFPointDownFail(bTryPermanentHost);
            StartCoroutine(ResFPointDown(strURL, strSaveFile, bTryPermanentHost));
            yield break;
        }

        ns.Close();
        fs.Close();
        if (request != null)
        {
            request.Abort();
        }

        ResFPointDownSuccess();
    }

    private void ResFPointDownSuccess()
    {
        m_eventVersion.state = EVersionState.ResUpdateSuccess;
        TriggerVersionProgressEvent();

        StartCoroutine(OnUnCompressResPackage());
    }

    private void ResFPointDownFail(bool bTryPermanentHost)
    {
        // 先使用CDN网络读取资源，失败后用永久域名读取资源，再失败提示玩家稍后重试
        if (bTryPermanentHost)
        {
            CheckResVersionFail();
        }
        else
        {
            // 删除已下载的更新包文件（文件可能是脏数据）
            RemoveResUpdateDir();
            CreateResUpdateDir();

            StartCoroutine(ResFPointDown(m_strResPackageURL, m_strLocalResPackageFullPath, true));
        }
    }

    private IEnumerator OnUnCompressResPackage()
    {
        yield return m_wait;

        string filePath = m_strLocalResPackageFullPath;
        string destPath = Util.DataPath + STR_VERSION_DIR;

        DirectoryInfo dirOutDir = new DirectoryInfo(destPath);
        if (!dirOutDir.Exists)
        {
            Directory.CreateDirectory(destPath);
        }

        bool success = false;
        try
        {
            Util.Log("call os uncompress---");
            success = ZipHelper.UnCompress(filePath, destPath);
        }
        catch (Exception ex)
        {
            Util.LogError("OnUnCompressPackage :" + ex.Message);
        }

        if (!success)
        {
            m_eventVersion.state = EVersionState.ResExtractFail;
            TriggerVersionProgressEvent();
        }

        RemoveResUpdateDir(); //解压完，删除update的文件

        if (success)
        {
            // 重新读取外部资源版本号
            m_localResVersion = m_urlResVersion;
            UpdateLocalResVersionFile(m_localResVersion);

            string curAppVersion = GetAppVersion();
            ClientSetting.Instance.ReLoadClientSettingData();

            string newAppVersion = GetAppVersion();
            if (!curAppVersion.Equals(newAppVersion))
            {
                CheckResVersion();
            }
            else
            {
                UnCompressResSuccess();
            }
        }
    }

    private void RemoveResUpdateDir()
    {
        string strPackageDir = Util.DataPath + STR_UPDATE_DIR;
        DirectoryInfo dirOutDir = new DirectoryInfo(strPackageDir);

        if (dirOutDir.Exists)
        {
            Directory.Delete(strPackageDir, true);
        }
    }

    //资源已是最新版本
    private void CheckResVersionNone()
    {
        m_eventVersion.state = EVersionState.ResUpdateSuccess;
        TriggerVersionProgressEvent();

        InitSubPackageVersion();
        StartCoroutine(DownloadSubPackagesCfg());
    }

    //资源下载解压成功
    private void UnCompressResSuccess()
    {
        Util.LogWarning("资源解压成功");
        m_eventVersion.state = EVersionState.ResExtractSuccess;
        TriggerVersionProgressEvent();

        InitSubPackageVersion();
        StartCoroutine(DownloadSubPackagesCfg());
    }

    //检测资源版本失败
    private void CheckResVersionFail()
    {
        m_eventVersion.state = EVersionState.ResUpdateFail;
        TriggerVersionProgressEvent();
    }
#endregion

    #region 分包下载
    private enum PackageDownloadType
    {
        Login = 1,
        Back,  
    }
    private const string STR_LOADEDPACKAGE_FILE = "SubPackage/LoadedPackages.ver";
    public const string STR_SUBPACKAGE_DIR = "SubPackage";

    private int m_nextDownloadPackageId = -1;   //
    private bool m_loginLoadOver = false;
    private Dictionary<int, CSubPackageUnit> m_packageCfgs = new Dictionary<int, CSubPackageUnit>();  // 全部分包信息
    private Dictionary<int, CLoadedPackageInfo> m_loadedPackages = new Dictionary<int, CLoadedPackageInfo>();  // 已下载分包信息 
    private int m_limitdownloadSpeed = 1;

    public bool IsAllSubPackageDownloaded
    {
        private set;
        get;
    }


    // 线程下载
    private bool m_bAsyncFPointDownAction = false;
    // 以下变量需要线程锁
    private HashSet<int> m_setDowning = new HashSet<int>();
    private CASyncSubPackageAction m_asyncFPointSyncAction = new CASyncSubPackageAction();
    private CASyncSubPackageAction m_asyncPackageExtractAction = new CASyncSubPackageAction();
    private CASyncSubPackageAction m_asyncFPointSyncSuccess = new CASyncSubPackageAction();
    private CASyncSubPackageAction m_asyncFPointSyncFail = new CASyncSubPackageAction();

    // 初始化历史下载包
    private void InitSubPackageVersion()
    {
        m_nextDownloadPackageId = -1;
        m_loginLoadOver = false;
        m_loadedPackages.Clear();
        
        int sp = ClientSetting.Instance.GetIntValue("SubpackageDownloadDelay");
        Util.LogWarning("SubpackageDownloadDelay:" + sp);
        if (sp > 0)
        {
            m_limitdownloadSpeed = sp;
        }

        string strVersionPath = Util.DataPath + STR_LOADEDPACKAGE_FILE;
        if (!File.Exists(strVersionPath))
        {
            return;
        }

        string strContent = File.ReadAllText(strVersionPath);
        strContent = strContent.Replace("\r", "");
        string[] strLines = strContent.Split('\n');

        foreach (var strLine in strLines)
        {
            string[] strLineArr = strLine.Split(',');

            if (strLineArr.Length >= 3)
            {
                int nID = 0;
                int type = 0;
                if (int.TryParse(strLineArr[0], out nID)
                    && int.TryParse(strLineArr[1], out type))
                {
                    CLoadedPackageInfo pack = new CLoadedPackageInfo();

                    pack.m_nID = nID;
                    pack.m_nType = type;
                    pack.m_nUrl = strLineArr[2];

                    m_loadedPackages.Add(nID, pack);
                }
            }
        }
    }

    /// <summary>
    /// SubPackageDownloadXmlURL
    /// </summary>
    /// <returns></returns>
    private string GetSubPackageDownloadXmlURL()
    {
        string strPath = CurrentBundleVersion.ResUpdateCdnUrl + ReplaceEscapeCharacter(SubpackageXmlPath);
        strPath += GetRandomParam();

        return strPath;
    }

    private IEnumerator DownloadSubPackagesCfg()
    {
        m_packageCfgs.Clear();

        //整包跳过分包下载
        if (!ClientSetting.Instance.GetBoolValue("SubPackageType"))
        {
            LoadPackageCfgSuccess();
            yield break;
        }

        string strPath = GetSubPackageDownloadXmlURL();
        Util.Log("TSS version download URL: >>>>>>>>>>> " + strPath);

        WWW www = new WWW(strPath);
        yield return www;

        if (www.error != null)
        {
            Util.LogError(strPath + " LoadPackageInfo Load Fail. " + www.error);
            LoadPackageCfgSuccess();  //LoadPackageCfgFail();
            yield break;
        }

        XML root;
        if (!www.bytes.TryParse(out root))
        {
            Util.LogError(string.Format("Parse file {0} error !", strPath));
            LoadPackageCfgFail();
            yield break;
        }

        foreach (XML node in root.Elements("pack"))
        {
            CSubPackageUnit cfg = new CSubPackageUnit();
            cfg.Init(node);
            m_packageCfgs.Add(cfg.id,cfg);
        }
        www.Dispose();

        LoadPackageCfgSuccess();
    }

    private CSubPackageUnit GetPackageCfgByID(int nID)
    {
        foreach (var item in m_packageCfgs)
        {
            if (item.Key == nID)
            {
                return item.Value;
            }
        }

        return null;
    }

    private CLoadedPackageInfo GetLoaedPackageByID(int nID)
    {
        foreach (var item in m_loadedPackages)
        {
            if (item.Key == nID)
            {
                return item.Value;
            }
        }

        return null;
    }

    private int GetNextDownLoadPackageId()
    {
        foreach (var cfg in m_packageCfgs.Values)
        {
            if (cfg.type == (int)PackageDownloadType.Login && (null == GetLoaedPackageByID(cfg.id)) )
            {
                return cfg.id;
            }
        }

        if (!m_loginLoadOver)
        {
            m_loginLoadOver = true;  //
            OnResourceInited();
        }

        foreach (var cfg in m_packageCfgs.Values)
        {
            if (cfg.type == (int)PackageDownloadType.Back && (null == GetLoaedPackageByID(cfg.id)))
            {
                return cfg.id;
            }
        }

       return -1;
    }

    private void LoadPackageCfgFail()
    {
        m_eventVersion.state = EVersionState.PackageCfgFail;
        TriggerVersionProgressEvent();
    }

    private void LoadPackageCfgSuccess()
    {
        DownloadNextPackage();
    }

    private void DownloadNextPackage()
    {
        m_nextDownloadPackageId = GetNextDownLoadPackageId();
        if (m_nextDownloadPackageId > 0)
        {
            DownSubPackage(m_nextDownloadPackageId);
        }
        else  //全部下载结束
        {
            Debug.LogWarning("IsAllSubPackageDownloaded");
            IsAllSubPackageDownloaded = true;

            m_eventVersion.state = EVersionState.AllPackageDownloaded;
            TriggerVersionProgressEvent();
        }
    }

    private string GetSaveSubPackagePath(int nID)
    {
        string strPackageDir = Util.DataPath + STR_SUBPACKAGE_DIR;
        DirectoryInfo dirOutDir = new DirectoryInfo(strPackageDir);

        if (!dirOutDir.Exists)
        {
            Directory.CreateDirectory(strPackageDir);
        }

        return string.Format("{0}/package_{1:d}.pkg", strPackageDir, nID);
    }

    private void DownSubPackage(int nID)
    {
        if (IsInDownSubPackage(nID) || IsHaveDowning())
        {
            return;
        }

        var cfg = GetPackageCfgByID(nID);
        if(null != cfg)
        {
            CASyncSubPackageThreadParam param = new CASyncSubPackageThreadParam();
            param.m_nID = nID;
            param.m_strURL = cfg.url;
            param.m_strSavePath = GetSaveSubPackagePath(nID);
            param.m_strSaveDir = Util.DataPath + STR_SUBPACKAGE_DIR;

            bool ret = ThreadPool.QueueUserWorkItem(DoAsyncFPointDownWithSubPackage, param);
            if (!ret)
            {
                Util.LogError("线程排队失败---- " + nID);
            }
        }
        else
        {
            Util.LogError("down package cfg is null, id: " + nID);
        }
    }

    private void DoAsyncFPointDownWithSubPackage(object act)
    {
        CASyncSubPackageThreadParam action = act as CASyncSubPackageThreadParam;

        if (action == null)
        {
            return;
        }

        AddDowning(action.m_nID);

        System.Net.HttpWebRequest request = null;
        System.IO.FileStream fs = null;
        long countLength = 0;

        //打开上次下载的文件或新建文件 
        long lStartPos = 0;

        try
        {
            Util.Log("TSS spit package URL: >>>>>>>>>>>> " + action.m_strURL);

            request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(action.m_strURL);
            request.Timeout = 5000;
            countLength = request.GetResponse().ContentLength;

            if (System.IO.File.Exists(action.m_strSavePath))  
            {
                File.Delete(action.m_strSavePath);
                //fs = System.IO.File.OpenWrite(action.m_strSavePath);
                //lStartPos = fs.Length;
                //if (countLength - lStartPos <= 0)
                //{
                //    fs.Close();
                //    OnFPointDownProgressWithSubPackage(action.m_nID, countLength, countLength);
                //    DoFPointDownSuccessWithSubPackage(action.m_nID, action.m_strSavePath, action.m_strSaveDir);
                //    return;
                //}
                //fs.Seek(lStartPos, System.IO.SeekOrigin.Current); //移动文件流中的当前指针 
            }

            //else
            {
                fs = new System.IO.FileStream(action.m_strSavePath, System.IO.FileMode.Create);
            }

            if (lStartPos > 0)   //断点下载
            {
                request.AddRange((int)lStartPos);  //设置Range值
            }
        }
        catch (System.Exception ex)
        {
            if (request != null)
            {
                request.Abort();
            }

            Util.LogError(ex.Message);
            DoFPointDownFailWithSubPackage(action.m_nID, action.m_strSavePath);
            return;
        }

        if (null == request || null == fs)
        {
            Util.LogError("FPointDown null error");
            DoFPointDownFailWithSubPackage(action.m_nID, action.m_strSavePath);
            return;
        }

        System.IO.Stream ns = null;
        try
        {
            ns = request.GetResponse().GetResponseStream();

            if (ns == null)
            {
                Util.LogError("ResFPointDown Error By ns is null");
                DoFPointDownFailWithSubPackage(action.m_nID, action.m_strSavePath);
                return;
            }

            ns.ReadTimeout = 10000;
        }
        catch (System.Exception ex)
        {
            Util.LogError(ex.Message);
            DoFPointDownFailWithSubPackage(action.m_nID, action.m_strSavePath);
            return;
        }

        int len = 128 * 1024;  // 128KB Buff

        byte[] nbytes = new byte[len];
        int nReadSize = 0;
        nReadSize = ns.Read(nbytes, 0, len);
        while (nReadSize > 0)
        {
            try
            {
                fs.Write(nbytes, 0, nReadSize);
                nReadSize = 0;
                nbytes = new byte[len];
            }
            catch (System.Exception ex)
            {
                Util.LogError(ex.Message);
                ns.Close();
                fs.Close();
                if (request != null)
                {
                    request.Abort();
                }

                DoFPointDownFailWithSubPackage(action.m_nID, action.m_strSavePath, true);
                return;
            }

            try
            {
                nReadSize = ns.Read(nbytes, 0, len);
            }
            catch (System.Exception ex)
            {
                Util.LogError(ex.Message);
               
                ns.Close();
                fs.Close();
                if (request != null)
                {
                    request.Abort();
                }

                DoFPointDownFailWithSubPackage(action.m_nID, action.m_strSavePath, true);
                return;
            }

            OnFPointDownProgressWithSubPackage(action.m_nID, fs.Length, countLength);

            if (m_loginLoadOver && !BackUpdate.ShowBackUpdate)  
            {
                Thread.Sleep(m_limitdownloadSpeed);
            }
        }

        if (fs.Length < countLength)
        {
            Util.LogError("读取流数据出错-----");
            ns.Close();
            fs.Close();
            if (request != null)
            {
                request.Abort();
            }

            DoFPointDownFailWithSubPackage(action.m_nID, action.m_strSavePath, true);
            return;
        }

        ns.Close();
        fs.Close();
        if (request != null)
        {
            request.Abort();
        }

        DoFPointDownSuccessWithSubPackage(action.m_nID, action.m_strSavePath, action.m_strSaveDir);
    }

    private bool IsInDownSubPackage(int nID)
    {
        lock (m_setDowning)
        {
            return m_setDowning.Contains(nID);
        }
    }

    private bool IsHaveDowning()
    {
        lock (m_setDowning)
        {
            return m_setDowning.Count > 0;
        }
    }

    private void AddDowning(int nID)
    {
        lock (m_setDowning)
        {
            if (!m_setDowning.Contains(nID))
            {
                m_setDowning.Add(nID);
            }
        }
    }

    private void DelDowning(int nID)
    {
        lock (m_setDowning)
        {
            m_setDowning.Remove(nID);
        }
    }

    private void OnFPointDownProgressWithSubPackage(int nID, long nDownSize, long nDownTotalSize)
    {
        lock (m_asyncFPointSyncAction)
        {
            m_asyncFPointSyncAction.m_bAction = true;
            m_asyncFPointSyncAction.m_nID = nID;
            m_asyncFPointSyncAction.m_nData1 = nDownSize;
            m_asyncFPointSyncAction.m_nData2 = nDownTotalSize;
        }

        m_bAsyncFPointDownAction = true;
    }

    //下载Package 成功完成，解压
    private void DoFPointDownSuccessWithSubPackage(int nID, string strSaveFile, string strSaveDir)
    {
        DelDowning(nID);

        OnPackageExtractWithSubPackage(nID);

        bool success = ZipHelper.UncompressZip(strSaveFile, strSaveDir);
        if (!success)
        {
            DoFPointDownFailWithSubPackage(nID, strSaveFile, true);
            return;
        }

        lock (m_asyncFPointSyncSuccess)
        {
            m_asyncFPointSyncSuccess.m_bAction = true;
            m_asyncFPointSyncSuccess.m_nID = nID;
        }

        m_bAsyncFPointDownAction = true;
    }

    private void OnPackageExtractWithSubPackage(int nID)
    {
        lock (m_asyncPackageExtractAction)
        {
            m_asyncPackageExtractAction.m_bAction = true;
            m_asyncPackageExtractAction.m_nID = nID;
        }

        m_bAsyncFPointDownAction = true;
    }

    //分包下载失败
    private void DoFPointDownFailWithSubPackage(int nID, string strSaveFile, bool bDeleteFile = false)
    {
        DelDowning(nID);

        if (bDeleteFile)
        {
            if (File.Exists(strSaveFile))
            {
                File.Delete(strSaveFile);
            }
        }

        lock (m_asyncFPointSyncFail)
        {
            m_asyncFPointSyncFail.m_bAction = true;
            m_asyncFPointSyncFail.m_nID = nID;
        }

        m_bAsyncFPointDownAction = true;
    }

    //update
    private void CheckAsyncFPointDown()
    {
        if (!m_bAsyncFPointDownAction)
        {
            return;
        }

        m_bAsyncFPointDownAction = false;

        lock (m_asyncFPointSyncAction)
        {
            if (m_asyncFPointSyncAction.m_bAction)
            {
                m_asyncFPointSyncAction.m_bAction = false;

                m_eventVersion.state = EVersionState.PackageUpdating;
                m_eventVersion.curPro = m_asyncFPointSyncAction.m_nData1;
                m_eventVersion.dataLength = m_asyncFPointSyncAction.m_nData2;
                TriggerVersionProgressEvent();
            }
        }

        lock (m_asyncPackageExtractAction)
        {
            if (m_asyncPackageExtractAction.m_bAction)
            {
                m_asyncPackageExtractAction.m_bAction = false;

                m_eventVersion.state = EVersionState.PackageExtracting;
                TriggerVersionProgressEvent();
            }
        }

        lock (m_asyncFPointSyncSuccess)
        {
            if (m_asyncFPointSyncSuccess.m_bAction)
            {
                m_asyncFPointSyncSuccess.m_bAction = false;

                UpdateSubPackageVersion(m_asyncFPointSyncSuccess.m_nID);

                m_eventVersion.state = EVersionState.PackageExtractSuccess;
                TriggerVersionProgressEvent();

                DownloadNextPackage();
            }
        }

        lock (m_asyncFPointSyncFail)
        {
            if (m_asyncFPointSyncFail.m_bAction)
            {
                m_asyncFPointSyncFail.m_bAction = false;

                m_eventVersion.state = EVersionState.PackageUpdateFail;
                TriggerVersionProgressEvent();

                DownloadNextPackage();
            }
        }
    }

    //下载解压一个分包成功
    private void UpdateSubPackageVersion(int nID)
    {
        Util.LogWarning("------------------分包下载解压成功： " + nID);
        CSubPackageUnit cfg = GetPackageCfgByID(nID);
        if (null != cfg)
        {
            CLoadedPackageInfo pack = new CLoadedPackageInfo();
            pack.m_nID = nID;
            pack.m_nType = cfg.type;
            pack.m_nUrl = cfg.url;

            m_loadedPackages.Add(pack.m_nID, pack);

            // 重置资源号
            string strVersionPath = Util.DataPath + STR_LOADEDPACKAGE_FILE;
            if (!File.Exists(strVersionPath))
            {
                File.Delete(strVersionPath);
            }

            StringBuilder strContent = new StringBuilder();
            foreach (var item in m_loadedPackages.Values)
            {
                strContent.AppendFormat("{0:d},{1:d},{2:d}\n", item.m_nID, item.m_nType, item.m_nUrl);
            }

            File.WriteAllText(strVersionPath, strContent.ToString());
        }
        else
        {
            Util.LogError("package cfg is null, id； " + nID);
        }
    }

    #endregion

    /// <summary>
    /// 资源初始化结束
    /// </summary>
    private void OnResourceInited()
    {
        CoreEntry.CoreRootObj.AddComponent<LoadModule>();
        LoadModule.Instance.Init(delegate (object data)
        {
            OnInitialize();
        });
    }

    private void OnInitialize()
    {
        Util.LogWarning("资源初始化结束---------");
        m_eventVersion.state = EVersionState.PackageUpdateSuccess;
        TriggerVersionProgressEvent();

        ClientSetting.Instance.ReLoadClientSettingData();
        MapMgr.Instance.EnterInitScene();
    }
}
