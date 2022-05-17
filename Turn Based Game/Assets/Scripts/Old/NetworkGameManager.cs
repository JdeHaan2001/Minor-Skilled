using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkGameManager : NetworkManager
{
    public Dictionary<NetworkConnection, GameObject> playerDict { get; private set; }
    public static NetworkGameManager Instance { get; private set; }

    public override void Awake()
    {
        if (Instance != null)
            Destroy(this.gameObject);
        else
            Instance = this;

        playerDict = new Dictionary<NetworkConnection, GameObject>(5);

        base.Awake();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("Server has started");
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Debug.Log(conn + " Has connected");
        GameObject playerObj = Instantiate(playerPrefab);
        Player player = playerObj.GetComponent<Player>();
        if (player == null) Debug.LogError("Object doesn't have Player script attached");

        player.SetConnection(conn);
        player.SetID(conn.connectionId);
        GameManager.Instance.SetPlayerState(playerObj.GetComponent<Player>(), PlayerState.Waiting);

        playerDict.Add(conn, playerObj);

        NetworkServer.AddConnection(conn);
        GameEvents.Instance.PlayerConnect();
        //NetworkServer.Spawn(playerObj, conn);
        //GameManager.Instance.CmdSendPlayerMessage($"{conn} has connected");
        //GameManager.Instance.SendMessageToAllClients($"{conn} has connected");
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        Debug.Log(conn + " Has disconnected");
        //GameManager.Instance.SetPlayerState(playerDict[conn].GetComponent<Player>(), PlayerState.Disconnected);

        //playerDict.Remove(conn);
        base.OnServerDisconnect(conn);
        //GameManager.Instance.CmdSendPlayerMessage($"{conn} has Disconnected");
        //GameManager.Instance.SendMessageToAllClients($"{conn} has Disconnected");
    }
}
