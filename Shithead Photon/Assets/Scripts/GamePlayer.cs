using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Realtime;
//using Photon.Pun;

public class GamePlayer : MonoBehaviour
{
    public List<PlayingCard> CardsInHand { get; private set; } = new List<PlayingCard>();
    public List<PlayingCard> CardsFaceUp { get; private set; } = new List<PlayingCard>();
    public List<PlayingCard> CardsFaceDown { get; private set; } = new List<PlayingCard>();

    public void AddCardInHand(PlayingCard pCard) => CardsInHand.Add(pCard);
    public void AddCardFaceUp(PlayingCard pCard) => CardsFaceUp.Add(pCard);
    public void AddCardFaceDown(PlayingCard pCard) => CardsFaceDown.Add(pCard);

    public void PlayCard()
    {
        throw new NotImplementedException("Playcard() hasn't been implemented yet");
    }

    //[PunRPC]
    private void updateUI()
    {
        throw new NotImplementedException("updateUI() Hasn't been implemented yet");
    }
}
