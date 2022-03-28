using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;

public class GameEvents : NetworkBehaviour
{
    public static GameEvents Instance = null;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Multiple instances of this object were found", this);
            Destroy(this.gameObject);
        }
        else
            Instance = this;
    }

    public event Action OnPlayerConnect;
    public event Action OnPlayerDisconnect;

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
}
