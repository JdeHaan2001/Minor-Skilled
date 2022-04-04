using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class RoundSystem : NetworkBehaviour
{
    public Text ServerText;
    private SceneScript sceneScript;
    private TBGNetworkManager networkManager;
    //private TBGNetworkManager NetworkManager
    //{
    //    get
    //    {
    //        if (networkManager != null) return networkManager;
    //        return networkManager = TBGNetworkManager.singleton as TBGNetworkManager;
    //    }
    //}

    //Server
    public override void OnStartServer()
    {
        networkManager = FindObjectOfType<TBGNetworkManager>();
        sceneScript = FindObjectOfType<SceneScript>();

        networkManager.events.OnServerStopped += CleanUpServer;
        networkManager.events.OnPlayerReady += CheckStartRound;
    }

    [ServerCallback]
    private void OnDestroy() => CleanUpServer();

    [Server]
    private void CleanUpServer()
    {
        networkManager.events.OnServerStopped -= CleanUpServer;
        networkManager.events.OnPlayerReady -= CheckStartRound;
    }

    [ServerCallback]
    public void StartRound() => RpcStartRound();

    
    private void CheckStartRound()
    {
        ServerText = GameObject.Find("Scene Text").GetComponent<Text>();

        Debug.Log("Total Clients: " + networkManager.ClientList.Count);
        foreach (Client client in networkManager.ClientList)
            Debug.Log(client.ToString());

        if (networkManager.ClientList.Count(x => x.IsReady) != networkManager.ClientList.Count)
        {
            Debug.Log("Not everyone is ready");
            return;
        }

        RpcStartRound();
    }

    //Client
    private void RpcStartRound()
    {
        Debug.Log("Start Round");
        Client winningClient = null;
        foreach (Client client in networkManager.ClientList)
        {
            Debug.Log($"Player{client.ID} Game number: {client.gameNumber}");
            if (winningClient == null)
                winningClient = client;
            else if (client.gameNumber > winningClient.gameNumber)
                    winningClient = client;
        }

        //foreach (Client client in networkManager.ClientList)
        //{
        //    Debug.Log("Checking if client has won or not");
        //    Debug.Log($"Current client{client.ID}   Winning client{winningClient.ID}");
        //    if (client.ID != winningClient.ID)
        //        TrpcSendGameInfo(client.connectionToClient, client, false);
        //    else
        //        TrpcSendGameInfo(client.connectionToClient, client, true);
        //}

        Debug.Log(networkManager.ClientList.Count);
        for (int i = 0; i < networkManager.ClientList.Count; i++)
        {
            if (networkManager.ClientList[i].ID != winningClient.ID)
                TrpcSendGameInfo(networkManager.ClientList[i].connectionToClient, networkManager.ClientList[i], false);
            else
                TrpcSendGameInfo(networkManager.ClientList[i].connectionToClient, networkManager.ClientList[i], true);
        }
    }
    
    [TargetRpc]
    private void TrpcSendGameInfo(NetworkConnection conn, Client client, bool hasWon)
    {
        ServerText.text = hasWon == true ? "You Won!!!" : "You Lost!!!";
        Debug.Log("Player won? " + hasWon);
        //Debug.Log("Sending info to player" + client.Connection.connectionId);
        //sceneScript.statusText = hasWon == true ? "You Won!!!" : "You Lost!!!";
        //client.ChangeStatusText(hasWon == true ? "You Won!!!" : "You Lost!!!");
    }

    [Command]
    private void Test(Client client, bool hasWon)
    {
        sceneScript.statusText = hasWon == true ? "You Won!!!" : "You Lost!!!";
    }
}
