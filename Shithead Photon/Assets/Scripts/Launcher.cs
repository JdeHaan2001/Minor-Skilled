using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Hastable = ExitGames.Client.Photon.Hashtable;

public class Launcher : MonoBehaviourPunCallbacks
{

    [SerializeField] private GameObject controlPanel;
    [SerializeField] private GameObject progressLabel;
    [SerializeField] private byte maxPlayersPerRoom = 4;

    /// <summary>
    /// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon,
    /// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
    /// Typically this is used for the OnConnectedToMaster() callback.
    /// </summary>
    bool isConnecting;

    string gameVersion = "1";

    private void Awake()
    {
        // #Critical
        // this makes sure we can use PhotonNetwork.LoadLevel()
        // on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        //Making sure that we can send PlayingCard classes over the photon network
        //PhotonPeer.RegisterType(typeof(PlayingCard), (byte)'M', PlayingCard.Serialize, PlayingCard.Deserialize);
        //Debug.Log("Registered custom PlayingCard Type");
        
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
    }

    /// <summary>
    /// Start the connection process.
    /// - If already connected, we attempt joining a random room
    /// - if not yet connected, Connect this application instance to Photon Cloud Network
    /// </summary>
    public void Connect()
    {
        progressLabel.SetActive(true);
        controlPanel.SetActive(false);

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            isConnecting = PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }

    public override void OnConnectedToMaster()
    {
        if (isConnecting)
        {
            //Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");
            LogSystem.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");
            PhotonNetwork.JoinRandomRoom();
        }
    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        isConnecting = false;
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
        Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
    }

    public override void OnJoinedRoom()
    {
        LogSystem.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");

        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            LogSystem.Log("Loading the Room for 1");

            PhotonNetwork.LoadLevel("Lobby");
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //Create a new room when the client can't join a room
        //Could be because no room exists or all rooms are full
        CreateRoom();
        LogSystem.Log("Created a room");
    }

    private void CreateRoom()
    {
        int randomRoomName = Random.Range(0, 5000);
        //RoomOptions roomOptions =
        //    new RoomOptions()
        //    {
        //        IsVisible = true,
        //        IsOpen = true,
        //        MaxPlayers = maxPlayersPerRoom,
        //    };

        //Hastable RoomCustomProps = new Hastable();
        //RoomCustomProps.Add("Cards", new PlayingCard[] { });
        //roomOptions.CustomRoomProperties = RoomCustomProps;

        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
    }
    
}
