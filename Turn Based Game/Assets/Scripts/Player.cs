using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player : MonoBehaviour
{
    public PlayerState currentState { get; private set; }

    public bool IsReady { get; set; }

    public int ID { get; private set; }
    public NetworkConnection connection { get; private set; }

    public void SetCurrentState(PlayerState pState) => currentState = pState;
    [Server]
    public void SetID(int pID) => ID = pID;
    [Server]
    public void SetConnection(NetworkConnection conn) => connection = conn;
}
