using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;

public class CardManager : GameManager
{
    private List<GameObject> cardList = new List<GameObject>();
    private List<PlayingCard> playingCardList = new List<PlayingCard>();
    private List<PlayingCard> cardsToBePlayed;
    private List<PlayingCard> playedCards = new List<PlayingCard>();
    private List<Sprite> cardSprites = new List<Sprite>();
    private List<Sprite> cardSpritesSpades = new List<Sprite>();
    private List<Sprite> cardSpritesClubs = new List<Sprite>();
    private List<Sprite> cardSpritesHearts = new List<Sprite>();
    private List<Sprite> cardSpritesDiamonds = new List<Sprite>();

    //[Range(0, 4)]
    private int JokerAmount = 0;

    private void Start()
    {
        //TODO: Load art for cards into a list
        Debug.Log("Loading sprites from Resources Folder");
        Object[] loadedSprites = Resources.LoadAll("Cards", typeof(Sprite));
        Debug.Log("Loaded sprites count: " + loadedSprites.Length);

        for (int s = 0; s < loadedSprites.Length; s++)
        {
            cardSprites.Add(loadedSprites[s] as Sprite);
        }

        for (int i = 1; i <= 52; i++)
        {
            if (i <= 13)
            {
                playingCardList.Add(MakePlayingCard(CardType.Clubs, i));
                //Debug.Log(CardType.CLUBS + " " + i);
                //Debug.Log(playingCardList[i - 1]);
            }
            else if (i > 13 && i <= 26)
            {
                playingCardList.Add(MakePlayingCard(CardType.Diamonds, i % 13 == 0 ? 13 : i % 13));
                //Debug.Log(CardType.DIAMONDS + " " + (i % 13 == 0 ? 13 : i % 13));
            }
            else if (i > 26 && i <= 39)
            {
                playingCardList.Add(MakePlayingCard(CardType.Spades, i % 13 == 0 ? 13 : i % 13));
                //Debug.Log(CardType.SPADES + " " + (i % 13 == 0 ? 13 : i % 13));
            }
            else if (i > 39 && i <= 52)
            {
                playingCardList.Add(MakePlayingCard(CardType.Hearts, i % 13 == 0 ? 13 : i % 13));
                //Debug.Log(CardType.HEARTS + " " + (i % 13 == 0 ? 13 : i % 13));
            }
        }

        ///Loads joker cards, TODO: Find art for joker cards
        //for (int j = 0; j < JokerAmount; j++)
        //{
        //    playingCardList.Add(MakePlayingCard(CardType.JOKER, 0));
        //}

        cardsToBePlayed = new List<PlayingCard>(playingCardList);
    }

    private PlayingCard MakePlayingCard(CardType pCardType, int pValue)
    {
        PlayingCard card = null;

        foreach (Sprite sprite in cardSprites)
        {
            if (sprite.name.Contains(pCardType.ToString()) && sprite.name.Contains(pValue.ToString()))
            {
                card = new PlayingCard(pCardType, pValue, sprite);
                break;
            }
        }
        Debug.Log(card.ToString());
        //TODO: Get correct sprite in InitCard  
        //card.InitCard(pCardType, pValue);
        return card;
    }
}
