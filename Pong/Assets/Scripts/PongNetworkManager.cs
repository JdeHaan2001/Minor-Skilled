using Mirror;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
public class PongNetworkManager : NetworkManager
{
    [HideInInspector] public static PongNetworkManager Instance { get; private set; }

    public Transform LeftPlayerSpawnPos = null;
    public Transform RightPlayerSpawnPos = null;

    public TextMeshProUGUI ScoreTxt = null;

    public int maxPoints = 5;

    public Dictionary<GameEvents.PlayerPlace, NetworkConnection> players = new Dictionary<GameEvents.PlayerPlace, NetworkConnection>(2);


    private int leftPlayerPoints = 0;
    private int rightPlayerPoints = 0; 

    GameObject ball;

    public override void Awake()
    {
        Instance = this;

        base.Awake();
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        Transform spawnPos = numPlayers == 0 ? LeftPlayerSpawnPos : RightPlayerSpawnPos;
        GameObject player = Instantiate(playerPrefab, spawnPos.position, spawnPos.rotation);

        players.Add((numPlayers == 0 ? GameEvents.PlayerPlace.Left : GameEvents.PlayerPlace.Right), conn);
        NetworkServer.AddPlayerForConnection(conn, player);

        if (numPlayers == 2)
        {
            ResetPoints();
            GameEvents.Instance.UIUpdate($"{leftPlayerPoints} : {rightPlayerPoints}");
            GameEvents.Instance.OnScore += AddPoints;
            ball = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "Ball"));
            NetworkServer.Spawn(ball);
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if (ball != null)
            NetworkServer.Destroy(ball);
        ResetPoints();
        GameEvents.Instance.OnScore -= AddPoints;
        players.Clear();
        players = new Dictionary<GameEvents.PlayerPlace, NetworkConnection>(2);
        base.OnServerDisconnect(conn);
    }

    [ServerCallback]
    public void ResetPoints()
    {
        leftPlayerPoints = 0;
        rightPlayerPoints = 0;
    }

    public void AddPoints(GameEvents.PlayerPlace pPlace)
    {
        NetworkServer.Destroy(ball);
        Debug.Log("Gets here");
        GameManager.instance.AddScore(pPlace);

        if (pPlace == GameEvents.PlayerPlace.Left)
            leftPlayerPoints++;
        else
            rightPlayerPoints++;

        GameEvents.Instance.UIUpdate($"{leftPlayerPoints} : {rightPlayerPoints}");

        ball = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "Ball"));
        ball.GetComponent<BallMovement>().SetDirection(pPlace == GameEvents.PlayerPlace.Left ?
                                                        GameEvents.PlayerPlace.Right : GameEvents.PlayerPlace.Left);
        NetworkServer.Spawn(ball);
    }

}
