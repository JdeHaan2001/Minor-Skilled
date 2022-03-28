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

    [ClientRpc][ServerCallback]
    public void SendMessageToAllClients(string pText)
    {
        serverMsgTxt.text = "All: " + pText;
        Debug.Log("All: " + pText);
    }

    [TargetRpc][ServerCallback]
    public void SendMessageToTarget(NetworkConnection conn, string pText)
    {
        serverMsgTxt.text = "You: " + pText;
        Debug.Log("You" + pText);
    }
}
