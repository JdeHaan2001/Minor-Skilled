using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType { Clubs, Spades, Diamonds, Hearts, Joker };

public class PlayingCard
{
    public CardType cardType { get; private set; }
    public int cardValue { get; private set; }
    public Sprite cardSprite { get; private set; }

    public PlayingCard (CardType pCardType, int pValue, Sprite pSprite)
    {
        cardType = pCardType;
        cardValue = pValue;
        cardSprite = pSprite;

        Debug.Log("Comes here");
    }

    public void InitCard(CardType pCardType, int pValue, Sprite pSprite)
    {
        cardType = pCardType;
        cardValue = pValue;
        cardSprite = pSprite;

        //Debug.Log("Comes here");
    }

    public Sprite GetSprite() => cardSprite;
    public CardType GetCardType() => cardType;
    public int GetCardValue() => cardValue;

    public override string ToString()
    {
        return $"Card Type: {cardType}\n Card Value: {cardValue}";
    }
}