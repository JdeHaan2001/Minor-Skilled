using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

[RequireComponent(typeof(InputField))]
public class PlayerNameInputField : MonoBehaviour
{
    private const string playerNamePrefKey = "PlayerName";

    private void Start()
    {
        string defaultName = string.Empty;
        InputField inputField = this.GetComponent<InputField>();

        if (inputField != null)
        {
            if (PlayerPrefs.HasKey(playerNamePrefKey))
            {
                defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                inputField.text = defaultName;
            }
        }

        PhotonNetwork.NickName = defaultName;
    }

    /// <summary>
    /// Sets the name of the player, and save it in the PlayerPrefs for future sessions.
    /// </summary>
    /// <param name="value">The name of the Player</param>
    public void SetPlayerName(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            Debug.LogError("Player name is null or empty", this);
            return;
        }

        PhotonNetwork.NickName = value;

        PlayerPrefs.SetString(playerNamePrefKey, value);
    }
}
