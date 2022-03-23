using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class GameManager : NetworkBehaviour
{
    public TextMeshProUGUI winText = null;

    [HideInInspector] public static GameManager instance { get; private set; }

    private int leftPlayerScore = 0;
    private int rightPlayerScore = 0;

    private void Awake()
    {
        if (instance != null)
            Destroy(this.gameObject);
        else
            instance = this;
        Debug.Log(instance);
    }

    private void Start()
    {
        winText.text = "";
    }

    private void checkScore()
    {
        if (leftPlayerScore == PongNetworkManager.Instance.maxPoints)
        {
            Debug.Log("Left won");
            cmdHandleWin((int)GameEvents.PlayerPlace.Left);
        }
        else if (rightPlayerScore == PongNetworkManager.Instance.maxPoints)
        {
            Debug.Log("Right won");
            cmdHandleWin((int)GameEvents.PlayerPlace.Right);
        }
        Debug.Log("No one won");
    }

    
    private void cmdHandleWin(int pPlace)
    {
        SetScreen(PongNetworkManager.Instance.players[(GameEvents.PlayerPlace)pPlace], pPlace, true);
        Debug.Log("Handeling Win");
        if (pPlace == (int)GameEvents.PlayerPlace.Left)
        {
            SetScreen(PongNetworkManager.Instance.players[(GameEvents.PlayerPlace)pPlace], pPlace, true);
            SetScreen(PongNetworkManager.Instance.players[GameEvents.PlayerPlace.Right], (int)GameEvents.PlayerPlace.Right, false);
        }
        else
        {
            SetScreen(PongNetworkManager.Instance.players[(GameEvents.PlayerPlace)pPlace], pPlace, true);
            SetScreen(PongNetworkManager.Instance.players[GameEvents.PlayerPlace.Left], (int)GameEvents.PlayerPlace.Left, false);
        }
    }

    [TargetRpc]
    private void SetScreen(NetworkConnection target, int pPlace, bool pHasWon)
    {
        if (pHasWon)
            winText.text = "You Won!";
        else
            winText.text = "You Lost!";
    }

    
    public void AddScore(GameEvents.PlayerPlace pPlace)
    {
        Debug.Log(pPlace);
        if (pPlace == GameEvents.PlayerPlace.Left)
            leftPlayerScore++;
        else
            rightPlayerScore++;

        checkScore();
    }
}
