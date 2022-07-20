using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Photon.Realtime;
//using Photon.Pun;

public enum PlayerStates {  WaitingForTurn = 0, CurrentTurn, PlayFaceUp, PlayFaceDown, Win, Lose};

public class GamePlayer : MonoBehaviour
{
    public List<PlayingCard> CardsInHand { get; private set; } = new List<PlayingCard>();
    public List<PlayingCard> CardsFaceUp { get; private set; } = new List<PlayingCard>();
    public List<PlayingCard> CardsFaceDown { get; private set; } = new List<PlayingCard>();

    public PlayerStates currentState = PlayerStates.WaitingForTurn;

    /// <summary>
    /// Sets the initial cards. All 3 arrays need to have a length of 3, otherwise an error will be thrown
    /// </summary>
    public void InitCards(PlayingCard[] pCardsInHand, PlayingCard[] pCardsFaceUp, PlayingCard[] pCardsFaceDown)
    {
        #region Error Handling
        if (pCardsInHand.Length != 3)
            Debug.LogError("pCardsInHand needs to be 3. Current length " + pCardsInHand.Length);
        else if(pCardsFaceUp.Length != 3)
            Debug.LogError("pCardsFaceUp needs to be 3. Current length " + pCardsFaceUp.Length);
        else if(pCardsFaceDown.Length != 3)
            Debug.LogError("pCardsFaceDown needs to be 3. Current length " + pCardsFaceDown.Length);
        #endregion
        CardsInHand = pCardsInHand.ToList();
        CardsFaceUp = pCardsFaceUp.ToList();
        CardsFaceDown = pCardsFaceDown.ToList();
    }

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
