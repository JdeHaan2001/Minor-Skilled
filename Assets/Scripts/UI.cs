using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
public class UI : NetworkBehaviour
{
    public TextMeshProUGUI UIText = null;
    private void Start()
    {
        GameEvents.Instance.OnUIUpdate += updateUI;
    }

    [ClientRpc]
    private void updateUI(string pText) => UIText.text = pText;
}
