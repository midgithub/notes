using UnityEngine;
using System.Collections;

public class RotateTest : MonoBehaviour {

    //坐标轴方向和旋转 左手定则
    //1.欧拉旋转的旋转顺序是Z、X、Y  如Transform.Rotate
    //2.
    //Transform.Rotate 每次调用时都是使用调用当前的坐标系
    void Start () {
        //Euler to Quaternion
        Quaternion q1 = Quaternion.Euler(Vector3.one); //= Quaternion.EulerAngles(1, 1, 1);

        //Quaternion to Euler
        Vector3 a = q1.eulerAngles; //q1.ToEuler() 和 Quaternion.ToEulerAngles(q1); 返回（0,0,0） 错误不能用

        //AngleAxis to Quaternion
        //用Vector3. 与*transform.rotation 得到的效果是当前状态 绕世界某个轴转的效果
        Quaternion q2 = Quaternion.AngleAxis(90, Vector3.up); //与 Quaternion.AxisAngle(Vector3.up,90); 不用
        //transform.rotation = q2 * transform.rotation;
        //q2 = Quaternion.AngleAxis(-90, Vector3.right);
        //transform.rotation = q2 * transform.rotation;

        // Quaternion to AngleAxis
        float ag = 0;
        Vector3 ax = Vector3.zero;
        q2.ToAngleAxis(out ag, out ax); //ToAxisAngle() 返回的 ag 是用弧度表示的

        Quaternion q3 = Quaternion.AngleAxis(90, transform.up); // transform. 第一次调用是用在某节点下 最初状态时(旋转都为0)那个轴 的坐标系，绕最初该轴转过一次后才是当前的坐标系
        //transform.localRotation = q3 * transform.localRotation;
        //q3 = Quaternion.AngleAxis(90, transform.up);
        //transform.localRotation = q3 * transform.localRotation;
        //q3 = Quaternion.AngleAxis(90, transform.up);
        //transform.localRotation = q3 * transform.localRotation;

        //transform.Rotate(new Vector3(0, 90, 45)); //效果同下，原因：进行一次旋转时不一起旋转当前坐标系(Z、X、Y顺序)和 旋转时，把坐标系一起转动(Y,X,Z顺序)效果一样；
        //transform.Rotate(new Vector3(0, 90, 0));
        //transform.Rotate(new Vector3(0, 0, 45));
        Debug.Log("debug");
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
