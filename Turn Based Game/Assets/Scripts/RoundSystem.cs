using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class RoundSystem : NetworkBehaviour
{
    #region server
    [ServerCallback]
    public void StartRound()
    {
        RpcStartRound();
    }

    [Server]
    private void CheckToStartRound(NetworkConnection conn)
    {
        int count = 0;

        foreach (KeyValuePair<NetworkConnection, GameObject> connection in NetworkGameManager.Instance.playerDict)
        {
            if (connection.Value.GetComponent<Player>().IsReady)
                count++;
        }

        if (count != NetworkGameManager.Instance.playerDict.Count) return;
    }
    #endregion

    #region Client
    [ClientRpc]
    private void RpcStartRound()
    {
        Debug.Log("Start Round");
        //Check Highest Card
    }
    #endregion
}
