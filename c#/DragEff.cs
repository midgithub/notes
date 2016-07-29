using UnityEngine;
using System.Collections;

public class DragEff : MonoBehaviour {

    private ARPlayer _playerCtrl;
	void Start () {
        _playerCtrl = transform.parent.GetComponent<ARPlayer>();
    }
	
	void Update () {
#if UNITY_IPHONE
        if (Input.touchCount <= 0)
         {
             return;
         }

         Touch touch = Input.GetTouch(0);
         ClickOn(touch.position);
#else
        if (Input.GetMouseButton(0))
        {
            ClickOn(Input.mousePosition);
        }
#endif
    }

    void ClickOn(Vector2 pos)
    {
        Ray ray = Camera.main.ScreenPointToRay(pos);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        foreach (RaycastHit hit in hits)
        {
            BoxCollider bc = hit.collider as BoxCollider;
            if (bc != null && bc.name.Contains("Plane"))
            {
                Vector3 p = hit.point;
                p.y = 0;
                //_playerCtrl.AtkPos = p;
            }
        }
    }
}
