//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using XLua;
using UnityEngine;

/// <summary>
/// Event Hook class lets you easily add remote event listener functions to an object.
/// Example usage: UIEventListener.Get(gameObject).onClick += MyClickFunction;
/// </summary>

[AddComponentMenu("NGUI/Internal/Event Listener")]

public class UIEventListener : MonoBehaviour
{
	public delegate void VoidDelegate (GameObject go);
	public delegate void BoolDelegate (GameObject go, bool state);
	public delegate void FloatDelegate (GameObject go, float delta);
	public delegate void VectorDelegate (GameObject go, Vector2 delta);
	public delegate void ObjectDelegate (GameObject go, GameObject draggedObject);
	public delegate void KeyCodeDelegate (GameObject go, KeyCode key);

	public object parameter;

	public VoidDelegate onSubmit;
	public VoidDelegate onClick;
	public VoidDelegate onDoubleClick;
	public BoolDelegate onHover;
	public BoolDelegate onPress;
	public BoolDelegate onSelect;
	public FloatDelegate onScroll;
	public VectorDelegate onDrag;
	public ObjectDelegate onDrop;
	public KeyCodeDelegate onKey;

    public VoidDelegate onLongPress;


	void OnSubmit ()				{ if (onSubmit != null) onSubmit(gameObject); }
	void OnClick ()					{ if (onClick != null) onClick(gameObject); }
	void OnDoubleClick ()			{ if (onDoubleClick != null) onDoubleClick(gameObject); }
	void OnHover (bool isOver)		{ if (onHover != null) onHover(gameObject, isOver); }
    



	void OnSelect (bool selected)	{ if (onSelect != null) onSelect(gameObject, selected); }
	void OnScroll (float delta)		{ if (onScroll != null) onScroll(gameObject, delta); }
	void OnDrag (Vector2 delta)		{ if (onDrag != null) onDrag(gameObject, delta); }
	void OnDrop (GameObject go)		{ if (onDrop != null) onDrop(gameObject, go); }
	void OnKey (KeyCode key)		{ if (onKey != null) onKey(gameObject, key); }

	/// <summary>
	/// Get or add an event listener to the specified game object.
	/// </summary>

	static public UIEventListener Get (GameObject go)
	{
		UIEventListener listener = go.GetComponent<UIEventListener>();
		if (listener == null) listener = go.AddComponent<UIEventListener>();
		return listener;
	}












    //lmjedit

    //判断长按
    float _longClickDuration = 0.5f;
    //float _lastPress = 0;
    Vector2 position;
    bool isLastPressed;

    void OnPress(bool isPressed)
    {

        if (onPress != null) onPress(gameObject, isPressed);

        if (isPressed)
        {
            //LogMgr.UnityLog("isPressed _lastPress " + _lastPress.ToString());
            //_lastPress = Time.realtimeSinceStartup;
            Invoke("CheckLongPress", _longClickDuration + 0.01f);

            position = UICamera.lastTouchPosition;
        }
        //else if (isLastPressed == true && Time.realtimeSinceStartup - _lastPress > _longClickDuration)
        //{
        //    //放开长按，隐藏按钮
        //    Invoke("hideButton", _longClickDuration + 0.1f);
        //}

        isLastPressed = isPressed;

        //Debug.Log("OnPress " + isPressed); 

    }

    void CheckLongPress()
    {

        //Debug.Log("CheckLongPress " + isLastPressed + (Time.realtimeSinceStartup - _lastPress) ); 

        if (isLastPressed == true)// && Time.realtimeSinceStartup - _lastPress > _longClickDuration)
        {
            //Debug.Log("CheckLongPress 1" + isLastPressed + (Time.realtimeSinceStartup - _lastPress)); 

            //SG.LogMgr.UnityLog("Time.realtimeSinceStartup :" + Time.realtimeSinceStartup.ToString());
            float distance = (position - UICamera.lastTouchPosition).magnitude;
            if (distance < 2)
            {
                //Debug.Log("CheckLongPress 2"); 
                if (onLongPress != null) onLongPress(gameObject);
            }

        }

    }



}

