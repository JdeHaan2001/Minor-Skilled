using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class SceneScript : NetworkBehaviour
{
    public Text ServerText;
    public Client client;

    [SyncVar(hook = nameof(OnStatusTextChanged))]
    public string statusText;

    private void OnStatusTextChanged(string pOld, string pNew)
    {
        ServerText.text = statusText;
    }
}
