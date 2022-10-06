using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ChangeScenes : MonoBehaviour
{
    public void ChangeLevel(string pSceneName)
    {
        PhotonNetwork.LoadLevel(pSceneName);
    }
}
