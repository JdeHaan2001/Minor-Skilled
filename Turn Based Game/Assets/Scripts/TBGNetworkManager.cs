using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Mirror;
using UnityEditor;

public class TBGNetworkManager : NetworkManager
{
    public List<GameObject> playerList { get; private set; }
    public TBGNetworkManager Instance { get; private set; }
    [SerializeField] private SceneScript sceneScript;
    [SerializeField] private GameObject roundSystem;

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

        GameObject roundSystemInstance = Instantiate(roundSystem);
        NetworkServer.Spawn(roundSystemInstance);
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Debug.Log(conn + " Has connected");
        GameObject playerObj = playerPrefab.gameObject;
        Client client = playerObj.GetComponent<Client>();
        if (client == null) Debug.LogError("Object doesn't have Client script attached");

        client.SetBaseClientInfo(conn, conn.connectionId, PlayerState.WaitWithoutCard, false);

        playerList.Add(playerObj);

        NetworkServer.AddConnection(conn);

        base.OnServerAddPlayer(conn);
    }

    public override void OnStopServer()
    {
        GameEvents.Instance.ServerStopped();

        playerList.Clear();
    }
}
