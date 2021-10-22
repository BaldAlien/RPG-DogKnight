using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
// using UnityEngine.Events;

// [System.Serializable]
// public class EventVector3 : UnityEvent<Vector3> { }
public class MouseManager : Singleton<MouseManager>
{
    public Texture2D point, doorway, attack, target, arrow;
    //public static MouseManager Instance;
    RaycastHit hitInfo;
    //public event Action<Vector3> OnMouseClicked;
    //public event Action<GameObject> OnEnemyClicked;


    // public EventVector3 OnMouseClicked;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    void Update()
    {
        SetCursorTexture();
        //MouseControl();
    }

    void SetCursorTexture()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hitInfo))
        {
            //换光标
            switch (hitInfo.collider.gameObject.tag)
            {
                case "Ground": Cursor.SetCursor(target, new Vector2(16, 16), CursorMode.Auto); break;
                case "Enemy": Cursor.SetCursor(attack, new Vector2(16, 16), CursorMode.Auto); break;
            }
        }
    }

    // void MouseControl()
    // {
    //     if (Input.GetMouseButtonDown(1) && hitInfo.collider != null)
    //     {
    //         if (hitInfo.collider.gameObject.CompareTag("Ground"))
    //         {
    //             OnMouseClicked?.Invoke(hitInfo.point);
    //         }
    //         if (hitInfo.collider.gameObject.CompareTag("Enemy"))
    //         {
    //             OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);
    //         }
    //     }
    // }


}


