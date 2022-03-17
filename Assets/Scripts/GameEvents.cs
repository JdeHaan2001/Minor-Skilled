using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class GameEvents : NetworkBehaviour
{
    public enum PlayerPlace
    {
        Left = 0, Right
    };

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

    public event Action<PlayerPlace> OnScore;
    public event Action<string> OnUIUpdate;

    [ServerCallback]
    public void Score(PlayerPlace pPlayerPlace)
    {
        if (OnScore != null)
            OnScore(pPlayerPlace);
    }
    [ServerCallback]
    public void UIUpdate(string pText)
    {
        if (OnUIUpdate != null)
            OnUIUpdate(pText);
    }
}
