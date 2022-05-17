using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;

public class GameEvents
{
    public static GameEvents Instance = null;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Multiple instances of this object were found");
        }
        else
            Instance = this;
    }

    public event Action OnPlayerConnect;
    public event Action OnPlayerDisconnect;
    public event Action OnPlayerReady;
    public event Action OnServerStopped;

    public void PlayerConnect()
    {
        if (OnPlayerConnect != null)
            OnPlayerConnect();
    }
    public void PlayerDisconnect()
    {
        if (OnPlayerDisconnect != null)
            OnPlayerDisconnect();
    }
    public void PlayerReady()
    {
        if (OnPlayerReady != null)
            OnPlayerReady();
    }
    public void ServerStopped()
    {
        if (OnServerStopped != null)
            OnServerStopped();
    }
}
