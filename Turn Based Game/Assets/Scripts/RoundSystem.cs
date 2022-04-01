using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;

public class RoundSystem : NetworkBehaviour
{
    private TBGNetworkManager networkManager;
    private TBGNetworkManager NetworkManager
    {
        get
        {
            if (networkManager != null) return networkManager;
            return networkManager = TBGNetworkManager.singleton as TBGNetworkManager;
        }
    }

    //Server
    public override void OnStartServer()
    {
        GameEvents.Instance.OnServerStopped += CleanUpServer;
        GameEvents.Instance.OnPlayerReady += CheckStartRound;
    }

    [ServerCallback]
    private void OnDestroy() => CleanUpServer();

    [Server]
    private void CleanUpServer()
    {
        GameEvents.Instance.OnServerStopped -= CleanUpServer;
        GameEvents.Instance.OnPlayerReady -= CheckStartRound;
    }

    [ServerCallback]
    public void StartRound() => RpcStartRound();

    [Server]
    private void CheckStartRound()
    {
        if (NetworkManager.playerList.Count(x => x.GetComponent<Client>().IsReady) != NetworkManager.playerList.Count) return;

        RpcStartRound();
    }

    //Client
    [ClientRpc]
    private void RpcStartRound()
    {
        Debug.Log("Start Round");
    }
}
