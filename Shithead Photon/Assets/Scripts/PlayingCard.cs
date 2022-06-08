using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType { CLUBS, SPADES, DIAMONDS, HEARTS, JOKER };

public class PlayingCard : ScriptableObject
{
    public CardType cardType { get; private set; }
    public int cardValue { get; private set; }
    public Sprite cardSprite { get; private set; }

    public void InitCard(CardType pCardType, int pValue, Sprite pSprite)
    {
        cardType = pCardType;
        cardValue = pValue;
        cardSprite = pSprite;
    }

    public Sprite GetSprite() => cardSprite;
    public CardType GetCardType() => cardType;
    public int GetCardValue() => cardValue;

    public override string ToString()
    {
        return $"Card Type: {cardValue}\n Card Value: {cardValue}";
    }
}