using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transfer : MonoBehaviour
{
    public enum TransferType
    {
        SameScene, DifferentScene
    }
    [Header("Tramsfer Information")]
    public string sceneName;
    public TransferType transitionType;
    public TransferDestination.DestinationTag destinationTag;
    private bool canTrans;

    void OnTriggerStay(Collider other)
    {
        canTrans = true;
    }

    void OnTriggerExit(Collider other)
    {
        canTrans = false;
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.E) && canTrans)
        {
            SceneController.Instance.transToDestination(this);
        }
    }

}