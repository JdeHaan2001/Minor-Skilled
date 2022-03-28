using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkGameManager : NetworkManager
{
    Dictionary<NetworkConnection, GameObject> playerDict = new Dictionary<NetworkConnection, GameObject>(5);
    public static NetworkGameManager Instance { get; private set; }

    public override void Awake()
    {
        if (Instance != null)
            Destroy(this.gameObject);
        else
            Instance = this;

        base.Awake();
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        //Add player to playerDict
        GameObject playerObj = Instantiate(playerPrefab);
        playerDict.Add(conn, playerObj);

        NetworkServer.AddConnection(conn);
        GameManager.Instance.SendMessageToAllClients($"{conn} has connected");
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        //Send message to all clients that a client disconnected
        playerDict.Remove(conn);
        base.OnServerDisconnect(conn);
        GameManager.Instance.SendMessageToAllClients($"{conn} has Disconnected");
    }
}
