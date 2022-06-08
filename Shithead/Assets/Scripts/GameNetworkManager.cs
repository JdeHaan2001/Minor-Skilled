using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameNetworkManager : NetworkManager
{
    public ServerMessages ServerMsg;

    private Dictionary<int, Player> connections = new Dictionary<int, Player>();

    

    public override void OnStartServer()
    {
        connections.Clear();

        Debug.Log($"Server has started at {DateTime.Now}");
    }

    public override void OnStopServer()
    {
        Debug.Log($"Stopping Server at {DateTime.Now}");
        connections.Clear();
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        Debug.Log(conn + " Has Connected");

        GameObject playerObj = Instantiate(playerPrefab);
        Player player = playerObj.GetComponent<Player>();
        player.SetClientID(conn.connectionId);

        connections.Add(conn.connectionId, player);
        NetworkServer.AddPlayerForConnection(conn, playerObj);

    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        ServerMsg.statusText = $"{conn.connectionId} has left";
        NetworkServer.Destroy(connections[conn.connectionId].gameObject);
        connections.Remove(conn.connectionId);
        Debug.Log($"{conn} has left the server");
    }
}
