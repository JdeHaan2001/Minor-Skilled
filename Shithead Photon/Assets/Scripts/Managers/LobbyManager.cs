using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : GameManager
{
    [SerializeField] private Button readyBtn;
    [SerializeField] private Text readyCountTxt;
    private List<Player> playersReadyList = new List<Player>();

    private void Awake()
    {
        readyCountTxt.text = "0/" + PhotonNetwork.CurrentRoom.PlayerCount;
    }

    public void ReadyButton()
    {
        if (playersReadyList.Contains(PhotonNetwork.LocalPlayer))
            UnReady();
        else
            Ready();
    }

    private void Ready()
    {
        Debug.Log("Player Ready: " + PhotonNetwork.LocalPlayer.NickName);
        playersReadyList.Add(PhotonNetwork.LocalPlayer);
        Debug.Log("Total players ready = " + playersReadyList.Count);

        readyBtn.GetComponentInChildren<Text>().text = "Un-Ready";
        readyCountTxt.text = playersReadyList.Count.ToString() + "/" + PhotonNetwork.CurrentRoom.PlayerCount;

        if (playersReadyList.Count == PhotonNetwork.CurrentRoom.PlayerCount)
            PhotonNetwork.LoadLevel("GameScene");

    }

    private void UnReady()
    {
        Debug.Log("Player un-ready: " + PhotonNetwork.LocalPlayer.NickName);
        playersReadyList.Remove(PhotonNetwork.LocalPlayer);
        Debug.Log("Total players ready = " + playersReadyList.Count);

        readyBtn.GetComponentInChildren<Text>().text = "Ready";
        readyCountTxt.text = playersReadyList.Count.ToString() + "/" + PhotonNetwork.CurrentRoom.PlayerCount;

    }
}
