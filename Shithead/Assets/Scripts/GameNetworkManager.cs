using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameNetworkManager : NetworkManager
{
    public ServerMessages ServerMsg;

    private List<Player> connections = new List<Player>();

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

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        Debug.Log(conn + " Has Connected");

        GameObject playerObj = Instantiate(playerPrefab);
        Player player = playerObj.GetComponent<Player>();
        player.SetClientID(conn.connectionId);

        connections.Add(player);
        NetworkServer.AddPlayerForConnection(conn, playerObj);

    }
}
