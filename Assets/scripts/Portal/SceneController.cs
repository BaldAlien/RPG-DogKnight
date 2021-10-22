using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : Singleton<SceneController>
{
    public GameObject playerPrefab;
    private GameObject player;


    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    public void transToDestination(Transfer transfer)
    {
        switch (transfer.transitionType)
        {
            case Transfer.TransferType.SameScene:
                StartCoroutine(Transition(SceneManager.GetActiveScene().name, transfer.destinationTag));
                break;
            case Transfer.TransferType.DifferentScene:
                StartCoroutine(Transition(transfer.sceneName, transfer.destinationTag));
                break;
        }
    }
    IEnumerator Transition(string sceneName, TransferDestination.DestinationTag destinationTag)
    {
        if (sceneName != SceneManager.GetActiveScene().name)
        {
            yield return SceneManager.LoadSceneAsync(sceneName);
            if (sceneName != "SampleScene")
                yield return Instantiate(playerPrefab, GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            yield break;
        }
        else
        {
            player = GameManager.Instance.playerStats.gameObject;
            player.transform.SetPositionAndRotation(GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            yield return null;
        }
    }

    private TransferDestination GetDestination(TransferDestination.DestinationTag destinationTag)
    {
        var destinations = FindObjectsOfType<TransferDestination>();
        foreach (var destination in destinations)
        {
            if (destination.destinationTag == destinationTag)
                return destination;
        }
        return null;
    }
}