using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Transform playerTransform;

    void Update()
    {
        // CameraFollowPlayer();
        ScrollView();
        // RotateView();
        CameraFollowMouse();
    }

    bool isRotate = false;//是否旋转相机
    Vector3 offsetPosition;//补偿向量
    float distance;//相机到玩家距离
    public float scrollSpeed;//缩放速度
    public float rotateSpeed;//旋转速度
    void Start()
    {
        //playerTransform = GameObject.Find("Player").GetComponent<Transform>();
        offsetPosition = transform.position - playerTransform.position;
        transform.position = offsetPosition + playerTransform.position;
        //vcam1 = GameObject.Find("MainCamera").GetComponent<Camera>();
    }

    void ScrollView()//缩放相机
    {
        // distance = offsetPosition.magnitude;
        // distance -= Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
        // offsetPosition = offsetPosition.normalized * distance;

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, 40, 120);
            Camera.main.fieldOfView = Camera.main.fieldOfView - Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
        }

    }

    void RotateView()//旋转相机
    {
        if (Input.GetMouseButtonDown(0))
        {
            isRotate = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            isRotate = false;
        }
        if (isRotate)
        {
            transform.RotateAround(playerTransform.position, Vector3.up, rotateSpeed * Input.GetAxis("Mouse X"));
            transform.RotateAround(playerTransform.position, transform.right, rotateSpeed * Input.GetAxis("Mouse Y"));
            offsetPosition = transform.position - playerTransform.position;
        }
    }

    void CameraFollowPlayer()
    {
        Vector3 targetPosition = playerTransform.position + offsetPosition;
        // transform.position=Vector3.Lerp(transform.position,targetPosition,Time.deltaTime*smoothing);
        transform.position = targetPosition;
        transform.LookAt(playerTransform.position);
    }

    private float Mouse_X = 0.0f, Mouse_Y = 0.0f;
    private float sensitivityMouse = 100f;
    private float MinY = 5, MaxY = 180, Distance = 5f, Damping = 2.5f;
    public bool isNeedDamping = true;

    void CameraFollowMouse()
    {
        Mouse_X += Input.GetAxis("Mouse X") * sensitivityMouse * 0.02f;
        Mouse_Y -= Input.GetAxis("Mouse Y") * sensitivityMouse * 0.01f;
        Mouse_Y = Y_Limit(Mouse_Y);

        Quaternion mRatation = Quaternion.Euler(Mouse_Y, Mouse_X, 0);
        Vector3 mPosition = mRatation * new Vector3(0.0f, 0.3f, -Distance) + playerTransform.position;

        if (isNeedDamping)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, mRatation, Time.deltaTime * Damping);
            transform.position = Vector3.Lerp(transform.position, mPosition, Time.deltaTime * Damping);
        }
        else
        {
            transform.rotation = mRatation;
            transform.position = mPosition;
        }
    }

    float Y_Limit(float value)
    {
        if (value < -360)
            value += 360;
        else if (value > 360)
            value -= 360;

        return Mathf.Clamp(value, MinY, MaxY);
    }
}
