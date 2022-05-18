using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public enum CardType { CLUBS, SPADES, DIAMONDS, HEARTS, JOKER };

public class PlayingCard : NetworkBehaviour
{
    public CardType cardType { get; private set; }
    public int CardValue { get; private set; }

    public void InitCard(CardType pCardType, int pValue)
    {
        cardType = pCardType;
        CardValue = pValue;
    }

    public override string ToString()
    {
        return $"Card Type: {CardValue}\n Card Value: {CardValue}";
    }
}
