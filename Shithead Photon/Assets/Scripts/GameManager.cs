using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    private int playersInRoom = 0;
    private int playersReady = 0;

    private void LoadLobby()
    {
        if(!PhotonNetwork.IsMasterClient) 
            Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");

        Debug.LogFormat("PhotonNetwork : Loading Lobby");
        PhotonNetwork.LoadLevel("Lobby");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.LogFormat("OnPlayerEnteredRoom() {0}", newPlayer.NickName);

        playersInRoom++;
        //if (PhotonNetwork.IsMasterClient)
        //{
        //    Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient);

        //    LoadLobby();
        //}
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.LogFormat("OnPlayerLeftRoom() {0}", otherPlayer.NickName);
        playersInRoom--;
    }

    public override void OnJoinedRoom()
    {
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
}
