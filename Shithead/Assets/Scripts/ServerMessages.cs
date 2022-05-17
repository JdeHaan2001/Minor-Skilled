using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ServerMessages : NetworkBehaviour
{
    public static ServerMessages Instance { get; set; }

    private void Awake()
    {
        if (Instance != null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }
}
