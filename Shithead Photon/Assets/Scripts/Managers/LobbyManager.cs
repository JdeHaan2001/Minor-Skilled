using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class LobbyManager : GameManager, IPunObservable
{
    [SerializeField] private Button readyBtn;
    [SerializeField] private Text readyCountTxt;
    private List<Player> playersReadyList = new List<Player>();

    public byte playersInRoom { get; private set; } = 0;
    private int playersReady;

    private void Start()
    {
        playersInRoom = PhotonNetwork.CurrentRoom.PlayerCount;
        Debug.Log("Players in Room: " + playersInRoom);
        
        this.photonView.RPC("UpdateUI", RpcTarget.All);
    }

    [PunRPC]
    private void UpdateUI(bool add)
    {
        //readyCountTxt.text = playersReadyList.Count.ToString() + "/" + PhotonNetwork.CurrentRoom.PlayerCount;
        playersReady += add == true ? 1 : -1;
        readyCountTxt.text = playersReady.ToString() + "/" + PhotonNetwork.CurrentRoom.PlayerCount;
    }

    [PunRPC]
    private void UpdateUI() => readyCountTxt.text = playersReady.ToString() + "/" + PhotonNetwork.CurrentRoom.PlayerCount;

    public void ReadyButton()
    {
        if (PhotonNetwork.LocalPlayer.IsReady)
            UnReady();
        else
            Ready();
        
        //this.photonView.RPC("UpdateUI", RpcTarget.All);
    }

    

    private void Ready()
    {
        Debug.Log("Player Ready: " + PhotonNetwork.LocalPlayer.NickName);
        playersReadyList.Add(PhotonNetwork.LocalPlayer);
        //playersReady++;
        PhotonNetwork.LocalPlayer.IsReady = true;
        Debug.Log("Total players ready = " + playersReadyList.Count);

        readyBtn.GetComponentInChildren<Text>().text = "Un-Ready";
        this.photonView.RPC("UpdateUI", RpcTarget.All, true);

        Debug.Log("Players in Room: " + PhotonNetwork.CurrentRoom.PlayerCount);

        if (playersReady == PhotonNetwork.CurrentRoom.PlayerCount)
            this.photonView.RPC("loadGameLevel", RpcTarget.All, "GameScene");

    }

    private void UnReady()
    {
        Debug.Log("Player un-ready: " + PhotonNetwork.LocalPlayer.NickName);
        playersReadyList.Remove(PhotonNetwork.LocalPlayer);
        //playersReady--;
        PhotonNetwork.LocalPlayer.IsReady = false;
        Debug.Log("Total players ready = " + playersReadyList.Count);

        readyBtn.GetComponentInChildren<Text>().text = "Ready";
        this.photonView.RPC("UpdateUI", RpcTarget.All, false);

    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        //photonView.RPC("UpdateUI", RpcTarget.All);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        //playersReady--;
        //photonView.RPC("UpdateUI", RpcTarget.All);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(PhotonNetwork.CurrentRoom.PlayerCount);

            stream.SendNext(playersReadyList.ToArray());
            stream.SendNext(playersReady);
        }
        else if (stream.IsReading)
        {
            this.playersInRoom = (byte)stream.ReceiveNext();

            var playersReadyArray = (Player[])stream.ReceiveNext();
            this.playersReadyList = playersReadyArray.ToList();
            //photonView.RPC("UpdateUI", RpcTarget.All);
            playersReady = (int)stream.ReceiveNext();
        }

    }
}
