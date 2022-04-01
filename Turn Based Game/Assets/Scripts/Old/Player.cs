using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
    public PlayerState CurrentState { get; private set; }

    public bool IsReady { get; set; }

    public int ID { get; private set; }
    public NetworkConnection Connection { get; private set; }
    public SceneScript SceneScript;

    public Canvas playerCanvas = null;
    private int gameNumber = -1;

    private void Awake()
    {
        SceneScript = GameObject.FindObjectOfType<SceneScript>();
    }

    private void Start()
    {
        Debug.Log("Comes here");
        base.OnStartClient();
        CmdSetupPlayer($"{Connection} Joined the server");
    }

    public override void OnStartClient()
    {
        Debug.Log("Comes here");
        base.OnStartClient();
        CmdSetupPlayer($"{Connection} Joined the server");
    }
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        CmdSetupPlayer($"{Connection} Left the server");
    }

    [Command]
    public void CmdSetupPlayer(string pText)
    {
        if (SceneScript)
            SceneScript.statusText = pText;
        else
            Debug.Log("SceneScript is NULL");
    }

    public void SetCurrentState(PlayerState pState) => CurrentState = pState;
    [Server]
    public void SetID(int pID) => ID = pID;
    [Server]
    public void SetConnection(NetworkConnection conn) => Connection = conn;

    public void GrabNumber()
    {
        CmdGetNumber();
    }

    [Command]
    private void CmdGetNumber()
    {
        TgpGetNumber(Connection);
    }

    [TargetRpc]
    private void TgpGetNumber(NetworkConnection conn)
    {
        if (CurrentState == PlayerState.WaitWithoutCard)
        {
            if (gameNumber == -1)
            {
                gameNumber = Random.Range(0, 10);
                CurrentState = PlayerState.waitWithCard;
                IsReady = true;
                Debug.Log($"Card number: {gameNumber}");
                Debug.Log($"Player state: {CurrentState}");
            }
        }
    }
}
