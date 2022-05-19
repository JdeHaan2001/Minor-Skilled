using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    private List<GameObject> cardList = new List<GameObject>();
    private List<PlayingCard> playingCardList = new List<PlayingCard>();
    private List<PlayingCard> cardsToBePlayed;
    private List<PlayingCard> playedCards = new List<PlayingCard>();

    [Range(0, 4)]
    public int JokerAmount = 2;

    public override void OnStartServer()
    {
        for (int i = 1; i <= 52; i++)
        {
            if (i <= 13) 
            {
                playingCardList.Add(MakePlayingCard(CardType.CLUBS, i));
                Debug.Log(CardType.CLUBS + " " + i);
            }
            else if (i > 13 && i <= 26) 
            {
                playingCardList.Add(MakePlayingCard(CardType.DIAMONDS, i % 13 == 0 ? 13 : i % 13));
                Debug.Log(CardType.DIAMONDS + " " + (i % 13 == 0 ? 13 : i % 13));
            }
            else if (i > 26 && i <= 39) 
            {
                playingCardList.Add(MakePlayingCard(CardType.SPADES, i % 13 == 0 ? 13 : i % 13));
                Debug.Log(CardType.SPADES + " " + (i % 13 == 0 ? 13 : i % 13));
            }
            else if (i > 39 && i <= 52) 
            {
                playingCardList.Add(MakePlayingCard(CardType.HEARTS, i % 13 == 0 ? 13 : i % 13));
                Debug.Log(CardType.HEARTS + " " + (i % 13 == 0 ? 13 : i % 13));
            }
        }

        for (int j = 0; j < JokerAmount; j++)
        {
            playingCardList.Add(MakePlayingCard(CardType.JOKER, 0));
        }

        cardsToBePlayed = new List<PlayingCard>(playingCardList);
    }

    private PlayingCard MakePlayingCard(CardType pCardType, int pValue)
    {
        PlayingCard card = new PlayingCard();
        card.InitCard(pCardType, pValue);
        return card;
    }
}
