using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    private List<GameObject> cardList = new List<GameObject>();

    [Range(0, 4)]
    public int JokerAmount = 2;

    public override void OnStartServer()
    {
        for (int i = 1; i <= 52; i++)
        {
            Debug.Log(i);
            if (i <= 13) 
            {
                cardList.Add(MakeCard(CardType.CLUBS, i));
                Debug.Log(CardType.CLUBS + " " + i);
            }
            else if (i > 13 && i <= 26) 
            {
                cardList.Add(MakeCard(CardType.DIAMONDS, i % 13));
                Debug.Log(CardType.DIAMONDS + " " + i % 13);
            }
            else if (i > 26 && i <= 39) 
            {
                cardList.Add(MakeCard(CardType.SPADES, i % 13));
                Debug.Log(CardType.SPADES + " " + i % 13);
            }
            else if (i > 39 && i <= 52) 
            {
                cardList.Add(MakeCard(CardType.HEARTS, i % 13));
                Debug.Log(CardType.HEARTS + " " + i % 13);
            }
        }

        for (int j = 0; j < JokerAmount; j++)
        {
            cardList.Add(MakeCard(CardType.JOKER, 0));
        }

        
    }

    private GameObject MakeCard(CardType pCardType, int pValue)
    {
        GameObject card = new GameObject($"{pCardType}_{pValue}", typeof(NetworkIdentity), typeof(PlayingCard));
        card.GetComponent<PlayingCard>().InitCard(pCardType, pValue);
        //Debug.Log(card.GetComponent<PlayingCard>());
        return card;
    }
}
