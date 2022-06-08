using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class ServerMessages : NetworkBehaviour
{
    public Text ServerText;

    [SyncVar(hook = nameof(OnStatusTextChanged))]
    public string statusText;

    private void OnStatusTextChanged(string Old, string New)
    {
        ServerText.text = statusText;
    }
}
