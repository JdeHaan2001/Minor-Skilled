using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class GameManager : NetworkBehaviour
{
    public TextMeshProUGUI serverMsgTxt = null;
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
            Destroy(this.gameObject);
        else
            Instance = this;
    }

    private void Start()
    {
        GameEvents.Instance.OnPlayerConnect += SendMessageToAllClients;
    }


    public void SendMessageToAllClients()
    {
        sendMessageToClients("Client has connected");
    }

    [ClientRpc]
    private void sendMessageToClients(string pText)
    {
        serverMsgTxt.text = "All: " + pText;
        Debug.Log("All: " + pText);
    }

    [TargetRpc]
    public void SendMessageToTarget(NetworkConnection conn, string pText)
    {
        serverMsgTxt.text = "You: " + pText;
        Debug.Log("You" + pText);
    }

    [ServerCallback]
    public void SetPlayerState(Player pPlayer, PlayerState pState)
    {
        if (pPlayer != null)
            pPlayer.SetCurrentState(pState);
        else
            Debug.LogWarning("Player is NULL", this);
    }
}
