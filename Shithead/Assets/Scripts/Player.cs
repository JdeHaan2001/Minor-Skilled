using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
    private List<PlayingCard> cardsInHand = new List<PlayingCard>();
    private List<PlayingCard> cardsFaceDown = new List<PlayingCard>(3);
    private List<PlayingCard> cardsFaceUp = new List<PlayingCard>(3);

    private ServerMessages serverMsg;

    private int ID;

    private void Awake()
    {
        serverMsg = FindObjectOfType<ServerMessages>();
    }

    public override void OnStartClient()
    {
        transform.name = "Player" + ID;
        serverMsg.statusText = $"{ID} has joined";
        Debug.Log("Sent message" + $" {ID} has joined");
    }

    public void SetClientID(int pID) => ID = pID;
}
