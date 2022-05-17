using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Mirror;
using UnityEditor;

public class TBGNetworkManager : NetworkManager
{
    public List<GameObject> playerList { get; private set; }
    public List<Client> ClientList { get; set; } = new List<Client>();
    public static TBGNetworkManager Instance { get; private set; }
    [SerializeField] private SceneScript sceneScript;
    [SerializeField] private GameObject roundSystem;

    public GameEvents events { get; private set; } = new GameEvents();

    public override void Awake()
    {
        if (Instance != null)
            Destroy(this.gameObject);
        else
            Instance = this;

        playerList = new List<GameObject>();
        base.Awake();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("Server has started");

        //GameObject roundSystemInstance = Instantiate(roundSystem);
        //NetworkServer.Spawn(roundSystemInstance);
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Debug.Log(conn + " Has connected");
        GameObject playerObj = Instantiate(playerPrefab);
        Client client = playerObj.GetComponent<Client>();
        if (client == null) Debug.LogError("Object doesn't have Client script attached");

        client.SetBaseClientInfo(conn, conn.connectionId, PlayerState.WaitWithoutCard, false);

        playerList.Add(playerObj);
        ClientList.Add(client);
        Debug.Log("ClientList count server: " + ClientList.Count);
        NetworkServer.AddPlayerForConnection(conn, playerObj);
    }

    public string PlayerToString(int pIndex = 0)
    {
        return playerList[0].GetComponent<Client>().ToString();
    }

    public override void OnStopServer()
    {
        events.ServerStopped();

        playerList.Clear();
    }
}
