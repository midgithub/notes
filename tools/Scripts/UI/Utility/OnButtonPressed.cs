using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using XLua;
using SG;
/// <summary>
/// 脚本位置：UGUI按钮组件身上
/// 脚本功能：实现按钮长按状态的判断
/// 创建时间：2015年11月17日
/// </summary>

// 继承：按下，抬起和离开的三个接口
[LuaCallCSharp]

public class OnButtonPressed : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    // 延迟时间
    private float delay = 0.2f;

    // 按钮是否是按下状态
    private bool isDown = false;

    // 按钮最后一次是被按住状态时候的时间
    private float lastIsDownTime;
     [CSharpCallLua]
    public delegate void UnityAction();
    public UnityAction action = null;
	public GuideWidget mgw = null;
    void Update()
    {
        // 如果按钮是被按下状态
        if (isDown)
        {
            // 当前时间 -  按钮最后一次被按下的时间 > 延迟时间0.2秒
            if (Time.time - lastIsDownTime > delay)
            {
                // 触发长按方法
                //Debug.Log("长按");
                // 记录按钮最后一次被按下的时间
                lastIsDownTime = Time.time;
                if (action != null)action();
            }
        }

    }

    // 当按钮被按下后系统自动调用此方法
    public void OnPointerDown(PointerEventData eventData)
    {
        isDown = true;
        lastIsDownTime = Time.time;
        if (action != null) action();
		if (mgw != null) {
			mgw.OnButtonClick ();
			Debug.Log ("---------00----------");
		}
    }

    // 当按钮抬起的时候自动调用此方法
    public void OnPointerUp(PointerEventData eventData)
    {
        isDown = false;
    }

    // 当鼠标从按钮上离开的时候自动调用此方法
    public void OnPointerExit(PointerEventData eventData)
    {
        isDown = false;
    }
}

