using XLua;
﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SG;

/// <summary>
/// 初始化场景的UI
/// </summary>
[Hotfix]
public class UIInitGame : MonoBehaviour
{
    /// <summary>
    /// 进度调其实宽度。
    /// </summary>
    public float ProgresXStart = 23;

    /// <summary>
    /// 进度调结束宽度。
    /// </summary>
    public float ProgresXEnd = 843;

    public RectTransform Progress;
    public Text _tip;
    //过审开关 0/关，1/开
    int IS_REVIEW = 0;
    string[] _strtip = new string[4] { "组队秘境可以获取大量的资源！",
        "建议优先完成主线任务！","任务都做完了可以打boss获取大量装备","跨服战场可以随意PK的"};
    private void Awake()
    {
        IS_REVIEW = ClientSetting.Instance.GetIntValue("IS_REVIEW");
        SetHideProgress();
        SetProgress(0);
        StartCoroutine(SetTip());
    }
    private void Start()
    {
      
    }

    private void OnDestroy()
    {
        RawImage imageBg = transform.FindChild("Back").GetComponent<RawImage>();
        if (imageBg)
        {
            imageBg.texture = null;
        }
        ThirdPartyEntry._textureBg = null;
    }

    private void SetHideProgress()
    {
        if (IS_REVIEW == 0)
        {
            Progress.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            Progress.transform.parent.gameObject.SetActive(false);
        }
    }
    IEnumerator SetTip()
    {
        if (IS_REVIEW == 0) yield break;
        yield return new WaitForSeconds(0.5f);
        _tip.text = _strtip[Random.Range(0, _strtip.Length)];
        StartCoroutine(SetTip());
        yield break;
    }
    /// <summary>
    /// 设置进度。
    /// </summary>
    /// <param name="v">进度值（0-1）。</param>
    public void SetProgress(float v)
    {
        if (IS_REVIEW == 1)
        {
            return;
        }
        float w = Mathf.Lerp(ProgresXStart, ProgresXEnd, v);
        Progress.sizeDelta = new Vector2(w, 13);
    }

    public void SetLoadTip(string tip)
    {
        _tip.text = tip;
    }
}

