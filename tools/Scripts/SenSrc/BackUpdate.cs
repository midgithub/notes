using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using XLua;
using SG;
using SenLib;

[Hotfix]
public class BackUpdate : MonoBehaviour
{
    private Slider _proBar;
    private Text _updatePro;
    private Text _txtTip;

    public static bool ShowBackUpdate
    {
        private set;
        get;
    }

    private Transform _selectBtn;

    void Awake() {
        ShowBackUpdate = false;

        if (!CVersionManager.Instance.IsAllSubPackageDownloaded)
        {
            if (ClientSetting.Instance.GetBoolValue("IS_REVIEW"))
            {
                ShowBackUpdate = true;
            }
            else
            {
                bool showed = PlayerPrefs.GetInt("HightAccoutShowed", 0) > 0;
                bool cfgshow = ClientSetting.Instance.GetBoolValue("ShowBackDownload");
                int limitLv = ClientSetting.Instance.GetIntValue("SubpackageDownloadLevel");
                Util.LogWarning("SubpackageDownloadLevel: " + limitLv);

                int playerMaxLv = Account.Instance.RoleMaxLevel;
                if (cfgshow && playerMaxLv > limitLv && !showed)
                {
                    ShowBackUpdate = true;
                }
            }
        }

        if (!ShowBackUpdate)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (null != _selectBtn)
        {
            _selectBtn.gameObject.SetActive(true);
        }

        if (ShowBackUpdate)
        {
            if (CVersionManager.Instance.IsAllSubPackageDownloaded)
            {
                PlayerPrefs.SetInt("HightAccoutShowed", 1);
                PlayerPrefs.Save();
            }

            CVersionManager.Instance.OnVersionProgressEvent -= OnUpdateResEvent;
        }

        ShowBackUpdate = false;
    }

    private void Start()
    {
        Init();
        CVersionManager.Instance.OnVersionProgressEvent += OnUpdateResEvent;
    }

    void Init()
    {
        _proBar = transform.Find("Slider").GetComponent<Slider>();
        _proBar.value = 0;

         _updatePro = transform.Find("Slider/updatePro").GetComponent<Text>();
        _updatePro.text = string.Empty;

        _txtTip = transform.Find("Tip").GetComponent<Text>();
        _txtTip.text = string.Empty;

        _selectBtn = transform.parent.Find("PageRoleSelect/Start");
        if(null != _selectBtn)
        {
            _selectBtn.gameObject.SetActive(false);
        }
    }

    private void OnUpdateResEvent(VersionProgressEvent data)
    {
        switch (data.state)
        {
            case EVersionState.PackageUpdating:  //每个分包更新中
                _proBar.value = (float)data.curPro / data.dataLength;
                _updatePro.text = string.Format("{0} / {1}", Util.GetFileLengthStr(data.curPro), Util.GetFileLengthStr(data.dataLength));
                _txtTip.text = "资源更新中...";
                break;
            case EVersionState.AllPackageDownloaded:  //所有后台分包更新完
                _txtTip.text = "资源更新完毕，准备游戏";
                _updatePro.text = string.Empty;

                Destroy(gameObject);
                break;
            case EVersionState.PackageUpdateFail:  //分包更新失败
                _txtTip.text = "资源更新失败";
                _updatePro.text = string.Empty;
                break;
            case EVersionState.PackageExtracting:  //分包解压同步
                _txtTip.text = "资源解压中...";
                _updatePro.text = string.Empty;
                break;
            case EVersionState.PackageExtractSuccess:  //分包解压成功
                _txtTip.text = "资源解压成功";
                break;
            case EVersionState.PackageExtractFail:  //分包解压失败
                _txtTip.text = "BackUpdate 解压失败";
                break;
            default:
                break;
        };
    }
}
