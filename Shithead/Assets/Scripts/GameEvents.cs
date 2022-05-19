using System;
using Mirror;

public class GameEvents
{
    public event Action<NetworkConnectionToClient> OnClientConnect;
    public event Action<NetworkConnectionToClient> OnClientDisconnect;
    public event Action OnGameStart;
    public event Action OnGameEnd;

    public void ClientConnect(NetworkConnectionToClient conn)
    {
        if (OnClientConnect != null)
            OnClientConnect(conn);
    }
    public void ClientDisconnect(NetworkConnectionToClient conn)
    {
        if (OnClientDisconnect != null)
            OnClientDisconnect(conn);
    }
    public void GameStart()
    {
        if (OnGameStart != null)
            OnGameStart();
    }
    public void GameEnd()
    {
        if (OnGameEnd != null)
            OnGameEnd();
    }
}
