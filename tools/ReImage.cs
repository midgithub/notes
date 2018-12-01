using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class ReImage : EditorWindow
{
    [MenuItem("GameObject/UI/Image")]
    static void CreatImage()
    {
        if (Selection.activeTransform)
        {
            if (Selection.activeTransform.GetComponentInParent<Canvas>())
            {
                GameObject go = new GameObject("image", typeof(Image));
                go.GetComponent<Image>().raycastTarget = false;
                go.transform.SetParent(Selection.activeTransform);
                go.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
                go.GetComponent<RectTransform>().localScale = Vector3.one;
            }
        }
    }

    [MenuItem("GameObject/UI/Text")]
    static void CreatText()
    {
        if (Selection.activeTransform)
        {
            if (Selection.activeTransform.GetComponentInParent<Canvas>())
            {
                GameObject go = new GameObject("text", typeof(Text));
                go.GetComponent<Text>().raycastTarget = false;
                go.transform.SetParent(Selection.activeTransform);
                go.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
                go.GetComponent<RectTransform>().localScale = Vector3.one;
            }
        }
    }
}
