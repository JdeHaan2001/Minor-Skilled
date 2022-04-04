using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Client : NetworkBehaviour
{
    public PlayerState CurrentState;
    public NetworkConnection Connection;
    //(hook =nameof(HandleReadyStatusChange))
    [SyncVar] public bool IsReady;
    public int ID;
    public int gameNumber = -1;

    public SceneScript sceneScript { get; private set; }
    public Text ReadyText;
    public Button GetNumberButton;

    private TBGNetworkManager networkManager;
    private GameEvents events;

    private void Awake()
    {
        networkManager = FindObjectOfType<TBGNetworkManager>();
        sceneScript = FindObjectOfType<SceneScript>();
        ReadyText = GameObject.Find("Ready Text").GetComponent<Text>();
        GetNumberButton = GameObject.Find("Get Number").GetComponent<Button>();
    }

    public override void OnStartLocalPlayer()
    {
        GetNumberButton.onClick.AddListener(delegate { CmdGetNumber(); });
        //TBGNetwork.ClientList.Add(this);
        Debug.Log("Client List count client: " + networkManager.ClientList.Count);
        Debug.Log($"Added to client{ID} list on server");

        sceneScript.client = this;
        sceneScript.statusText = $"Player{ID} has joined";
        Debug.Log($"Player{ID} has joined");

        ReadyText.text = "Not Ready";
    }
    
    private void HandleReadyStatusChange(bool oldValue, bool newValue)
    {
        if (newValue)
        {
            sceneScript.statusText = $"Player{ID} Ready";
            ReadyText.text = "Ready";
            GetNumberButton.gameObject.SetActive(false);
        }
        else
        {
            sceneScript.statusText = $"Player{ID} Not Ready";
            ReadyText.text = "Not Ready";
            GetNumberButton.gameObject.SetActive(true);
        }
    }

    [Command]
    public void CmdSendPlayerMessage()
    {
        if (sceneScript)
        {
            sceneScript.statusText = $"Player{ID} says hello";
            Debug.Log($"Player{ID} says hello");
        }
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                Debug.Log(ID);
                CmdSendPlayerMessage();
                sceneScript.statusText = $"Player{ID} {GetNumberButton}";
                Debug.Log($"Player{ID} {GetNumberButton}");
                Debug.Log(TBGNetworkManager.Instance.PlayerToString(0));
            }
        }
    }

    public override void OnStopLocalPlayer()
    {
        sceneScript.statusText = $"Player{ID} has left";
        GetNumberButton.onClick.RemoveListener(delegate { CmdGetNumber(); });
        Debug.Log($"Player{ID} has left");
    }

    [Command]
    public void CmdGetNumber()
    {
        if (!IsReady)
        {
            Debug.Log("Getting number");
            IsReady = true;
            gameNumber = Random.Range(0, 10);
            CurrentState = PlayerState.waitWithCard;
            sceneScript.statusText = $"Player{ID} Ready";
            //callPlayerReadyEvent();
            TrpcGetNumber(Connection);
            Debug.Log("Done Getting Number");
        }
    }

    [TargetRpc]
    private void TrpcGetNumber(NetworkConnection conn)
    {
        CmdSetStatusText($"Player{ID} Ready");
        ReadyText.text = "Ready";
        GetNumberButton.gameObject.SetActive(false);

        callPlayerReadyEvent();
        //networkManager.events.PlayerReady();
        Debug.Log("Called Player ready event");
    }

    [Command]
    public void CmdSetStatusText(string text)
    {
        sceneScript.statusText = text;
    }

    public void ChangeStatusText(string text) => CmdSetStatusText(text);

    [Command]
    private void callPlayerReadyEvent() => networkManager.events.PlayerReady();

    [Server]
    public void SetCurrentState(PlayerState pState) => CurrentState = pState;
    [Server]
    public void SetID(int pID) => ID = pID;
    [Server]
    public void SetConnection(NetworkConnection pConn) => Connection = pConn;
    [Server]
    public void SetIsReady(bool pReady) => IsReady = pReady;
    [Server]
    public void SetBaseClientInfo(NetworkConnection pConn, int pID, PlayerState pState, bool pReady)
    {
        Connection = pConn;
        ID = pID;
        CurrentState = pState;
        IsReady = pReady;
        Debug.Log($"Connection = {Connection}\n ID = {ID}\n playerState = {CurrentState}\n isReady = {IsReady}");
    }

    public new string ToString()
    {
        return $"Connection: {Connection}, ID: {ID}, Current state: {CurrentState}, Is ready: {IsReady}";
    }
}
