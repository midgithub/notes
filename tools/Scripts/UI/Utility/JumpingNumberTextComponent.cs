/* ==============================================================================
 * 功能描述：数字动态变化Text
 * 创 建 者：shuchangliu
 * ==============================================================================*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using XLua;

//来源:https://www.cnblogs.com/leoin2012/p/7231228.html

[LuaCallCSharp]
[Hotfix]
public class JumpingNumberTextComponent : MonoBehaviour
{
    [SerializeField]
    [Tooltip("按最高位起始顺序设置每位数字Text（显示组）")]
    private List<Text> _numbers;
    [SerializeField]
    [Tooltip("按最高位起始顺序设置每位数字Text（替换组）")]
    private List<Text> _unactiveNumbers;
    /// <summary>
    /// 动画时长
    /// </summary>
    [SerializeField]
    private float _duration = 1.5f;
    /// <summary>
    /// 数字每次滚动时长
    /// </summary>
    [SerializeField]
    private float _rollingDuration = 0.05f;
    /// <summary>
    /// 数字每次变动数值
    /// </summary>
    private int _speed;
    /// <summary>
    /// 滚动延迟（每进一位增加一倍延迟，让滚动看起来更随机自然）
    /// </summary>
    [SerializeField]
    private float _delay = 0.008f;
    /// <summary>
    /// Text文字宽高
    /// </summary>
    private Vector2 _numberSize;
    /// <summary>
    /// 当前数字
    /// </summary>
    private int _curNumber;
    /// <summary>
    /// 起始数字
    /// </summary>
    private int _fromNumber;
    /// <summary>
    /// 最终数字
    /// </summary>
    private int _toNumber;
    /// <summary>
    /// 各位数字的缓动实例
    /// </summary>
    private List<Tweener> _tweener = new List<Tweener>();
    /// <summary>
    /// 是否处于数字滚动中
    /// </summary>
    private bool _isJumping;
    /// <summary>
    /// 滚动完毕回调
    /// </summary>
    public Action OnComplete;

    private void Awake()
    {
        if (_numbers.Count == 0 || _unactiveNumbers.Count == 0)
        {
            Debug.LogError("[JumpingNumberTextComponent] 还未设置Text组件!");
            return;
        }
        _numberSize = _numbers[0].rectTransform.sizeDelta;
    }

    public float duration
    {
        get { return _duration; }
        set
        {
            _duration = value;
        }
    }

    private float _different;
    public float different
    {
        get { return _different; }
    }

    public void Change(int from, int to)
    {
        bool isRepeatCall = _isJumping && _fromNumber == from && _toNumber == to;
        if (isRepeatCall) return;

        bool isContinuousChange = (_toNumber == from) && ((to - from > 0 && _different > 0) || (to - from < 0 && _different < 0));
        if (_isJumping && isContinuousChange)
        {
        }
        else
        {
            _fromNumber = from;
            _curNumber = _fromNumber;
        }
        _toNumber = to;

        _different = _toNumber - _fromNumber;
        _speed = (int)Math.Ceiling(_different / (_duration * (1 / _rollingDuration)));
        _speed = _speed == 0 ? (_different > 0 ? 1 : -1) : _speed;

        SetNumber(_curNumber, false);
        _isJumping = true;
        StopCoroutine("DoJumpNumber");
        StartCoroutine("DoJumpNumber");
    }

    public int number
    {
        get { return _toNumber; }
        set
        {
            if (_toNumber == value) return;
            Change(_curNumber, _toNumber);
        }
    }

    IEnumerator DoJumpNumber()
    {
        while (true)
        {
            if (_speed > 0)//增加
            {
                _curNumber = Math.Min(_curNumber + _speed, _toNumber);
            }
            else if (_speed < 0) //减少
            {
                _curNumber = Math.Max(_curNumber + _speed, _toNumber);
            }
            SetNumber(_curNumber, true);

            if (_curNumber == _toNumber)
            {
                StopCoroutine("DoJumpNumber");
                _isJumping = false;
                if (OnComplete != null) OnComplete();
                yield return null;
            }
            yield return new WaitForSeconds(_rollingDuration);
        }
    }

    /// <summary>
    /// 设置战力数字
    /// </summary>
    /// <param name="v"></param>
    /// <param name="isTween"></param>
    public void SetNumber(int v, bool isTween)
    {
        char[] c = v.ToString().ToCharArray();
        Array.Reverse(c);
        string s = new string(c);

        if (!isTween)
        {
            for (int i = 0; i < _numbers.Count; i++)
            {
                if (i < s.Count())
                    _numbers[i].text = s[i] + "";
                else
                    _numbers[i].text = "0";
            }
        }
        else
        {
            while (_tweener.Count > 0)
            {
                _tweener[0].Complete();
                _tweener.RemoveAt(0);
            }

            for (int i = 0; i < _numbers.Count; i++)
            {
                if (i < s.Count())
                {
                    _unactiveNumbers[i].text = s[i] + "";
                }
                else
                {
                    _unactiveNumbers[i].text = "0";
                }

                _unactiveNumbers[i].rectTransform.anchoredPosition = new Vector2(_unactiveNumbers[i].rectTransform.anchoredPosition.x, (_speed > 0 ? -1 : 1) * _numberSize.y);
                _numbers[i].rectTransform.anchoredPosition = new Vector2(_unactiveNumbers[i].rectTransform.anchoredPosition.x, 0);

                if (_unactiveNumbers[i].text != _numbers[i].text)
                {
                    DoTween(_numbers[i], (_speed > 0 ? 1 : -1) * _numberSize.y, _delay * i);
                    DoTween(_unactiveNumbers[i], 0, _delay * i);

                    Text tmp = _numbers[i];
                    _numbers[i] = _unactiveNumbers[i];
                    _unactiveNumbers[i] = tmp;
                }
            }
        }
    }

    public void DoTween(Text text, float endValue, float delay)
    {
        Tweener t = DOTween.To(() => text.rectTransform.anchoredPosition, (x) =>
        {
            text.rectTransform.anchoredPosition = x;
        }, new Vector2(text.rectTransform.anchoredPosition.x, endValue), _rollingDuration - delay).SetDelay(delay);
        _tweener.Add(t);
    }


    [ContextMenu("测试数字变化")]
    public void TestChange()
    {
        Change(UnityEngine.Random.Range(1, 1), UnityEngine.Random.Range(1, 100000));
    }
    [ContextMenu("测试数字变化1")]
    public void TestChange1()
    {  
        Change(12, 98);
    }
  

}

