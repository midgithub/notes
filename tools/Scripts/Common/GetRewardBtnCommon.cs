
using XLua;
using UnityEngine;
using System.Collections;


public class GetRewardBtnCommon : MonoBehaviour {

    BoxCollider mCollider;

    void Awake()
    {
        mCollider = GetComponent<BoxCollider>();
        SetColliderActive(true);
    }

    void OnClick()
    {
        SetColliderActive(false);
        StartCoroutine(SetColliderActiveForTime());
    }

    IEnumerator SetColliderActiveForTime()
    {
        yield return new WaitForSeconds(1);
        SetColliderActive(true);
    }

    void SetColliderActive(bool flag)
    {
        if (mCollider != null)
            mCollider.enabled = flag;
    }
}

