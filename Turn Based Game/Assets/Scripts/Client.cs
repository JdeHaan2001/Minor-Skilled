using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Client : NetworkBehaviour
{
    public PlayerState CurrentState;
    public NetworkConnection Connection;
    [SyncVar(hook = nameof(HandleReadyStatusChange))]
    public bool IsReady;
    public int ID;
    public int gameNumber = -1;

    public SceneScript sceneScript { get; private set; }
    public Text ReadyText;
    public Button GetNumberButton;

    private void Awake()
    {
        sceneScript = FindObjectOfType<SceneScript>();
        ReadyText = GameObject.Find("Ready Text").GetComponent<Text>();
        GetNumberButton = GameObject.Find("Get Number").GetComponent<Button>();
    }

    public override void OnStartLocalPlayer()
    {
        Debug.Log("Works");
        GetNumberButton.onClick.AddListener(delegate { CmdGetNumber(connectionToClient); });

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
            }
        }
    }

    public override void OnStopLocalPlayer()
    {
        sceneScript.statusText = $"Player{ID} has left";
        GetNumberButton.onClick.RemoveListener(delegate { CmdGetNumber(connectionToClient); });
        Debug.Log($"Player{ID} has left");
    }

    [TargetRpc]
    public void CmdGetNumber(NetworkConnection conn)
    {
        if (isLocalPlayer)
        {
            Debug.Log("Getting number");
            if (!IsReady)
            {
                gameNumber = Random.Range(0, 10);
                IsReady = true;
                CurrentState = PlayerState.waitWithCard;
                GameEvents.Instance.PlayerReady();
            }
            Debug.Log("Game number = " + gameNumber);
        }
    }

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
}
