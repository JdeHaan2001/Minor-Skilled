using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
    private ServerMessages serverMsg;

    private int ID;

    private void Awake()
    {
        serverMsg = FindObjectOfType<ServerMessages>();
    }

    public override void OnStartClient()
    {
        transform.name = "Player" + ID;
        serverMsg.statusText = $"{ID} has joined";
        Debug.Log("Sent message" + $" {ID} has joined");
    }

    public void SetClientID(int pID) => ID = pID;
}
