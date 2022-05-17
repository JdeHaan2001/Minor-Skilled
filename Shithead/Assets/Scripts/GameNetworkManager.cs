using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameNetworkManager : NetworkManager
{
    private List<Player> connections = new List<Player>();

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        

    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        connections.Clear();
        Debug.Log($"Server has started at {DateTime.Now}");
    }

    public override void OnStopServer()
    {
        Debug.Log($"Stopping Server at {DateTime.Now}");
        connections.Clear();
        base.OnStopServer();
    }
}
