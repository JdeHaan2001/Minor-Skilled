using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.U2D;
using UnityEngine.UI;
using Hastable = ExitGames.Client.Photon.Hashtable;

public class CardManager : GameManager
{
    //TODO: Make override function to update the playingcardlist

    [SerializeField] private List<PlayingCard> playingCardList = new List<PlayingCard>();
    [SerializeField] private SpriteAtlas spriteAtlas;

    private List<PlayingCard> cardsToBePlayed = new List<PlayingCard>();
    private List<PlayingCard> playedCards = new List<PlayingCard>();
    
    private const string keyCardsToBePlayed = "cardsToBePlayed";
    private PlayingCard _card = null;
    private GameObject playerObj = null;

    //[Range(0, 4)]
    private int JokerAmount = 0;
    private int lastPlayedCardValue = -1;

    public static CardManager Instance { get; private set; }

    public GameObject PlayerPrefab;    

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 1; i <= 52; i++)
            {
                if (i <= 13)
                    playingCardList.Add(MakePlayingCard(CardType.Clubs, i));
                else if (i > 13 && i <= 26)
                    playingCardList.Add(MakePlayingCard(CardType.Diamonds, i % 13 == 0 ? 13 : i % 13));
                else if (i > 26 && i <= 39)
                    playingCardList.Add(MakePlayingCard(CardType.Spades, i % 13 == 0 ? 13 : i % 13));
                else if (i > 39 && i <= 52)
                    playingCardList.Add(MakePlayingCard(CardType.Hearts, i % 13 == 0 ? 13 : i % 13));
            }

            #region make jokers
            ///Loads joker cards, TODO: Find art for joker cards
            //for (int j = 0; j < JokerAmount; j++)
            //{
            //    playingCardList.Add(MakePlayingCard(CardType.JOKER, 0));
            //}
            #endregion

            //int[] cardInts = new int[playingCardList.Count];

            List<PlayingCard> shuffledList = new List<PlayingCard>(shuffleCards(playingCardList));
            playingCardList = shuffledList;

            //for (int i = 0; i < playingCardList.Count; i++)
            //    cardInts[i] = PlayingCard.CardToInt(playingCardList[i]);

            this.photonView.RPC("SetCardsToBePlayed", RpcTarget.All, CardArrayToIntArray(playingCardList.ToArray()));
        }

        if (PlayerPrefab == null)
            Debug.Log("PlayerPrefab is null", this);
        else
        {
            Debug.Log("Instantiating local player object");
            PhotonNetwork.Instantiate(this.PlayerPrefab.name, new Vector3(0,0,0), Quaternion.identity, 0);
            playerObj = GameObject.FindGameObjectWithTag("Player");//Would like to find a better way to get this
        }

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            this.photonView.RPC("dealCards", PhotonNetwork.PlayerList[i]);

        //TODO: Set Starting player
    }

    /// <summary>
    /// Gets sprite from sprite atlas
    /// </summary>
    /// <returns></returns>
    public Sprite GetCardSprite(CardType pCardType, int pValue)
    {
        return spriteAtlas.GetSprite($"{pCardType}_{pValue}_white");
    }

    /// <summary>
    /// Returns a new playing card class with the given type and value
    /// </summary>
    private PlayingCard MakePlayingCard(CardType pCardType, int pValue)
    {      
        return new PlayingCard(pCardType, pValue, GetCardSprite(pCardType, pValue));
    }

    /// <summary>
    /// Shuffles a given playing card List
    /// </summary>
    /// <returns>Returns a new shuffled version of the given list</returns>
    private List<PlayingCard> shuffleCards(List<PlayingCard> pList)
    {
        int listIndex = pList.Count;
        List<PlayingCard> shuffledList = new List<PlayingCard>(pList);
        while (listIndex > 1)
        {
            listIndex--;
            int randNumber = Random.Range(0, listIndex + 1);
            PlayingCard card = shuffledList[randNumber];
            shuffledList[randNumber] = shuffledList[listIndex];
            shuffledList[listIndex] = card;
        }
        foreach (PlayingCard card in shuffledList)
            Debug.Log(card.ToString());
        return shuffledList;
    }

    [PunRPC]
    private void SetCardsToBePlayed(int[] pList)
    {
        cardsToBePlayed.Clear();
        cardsToBePlayed.TrimExcess();
        Debug.Log("pList length: " + pList.Length);
        for (int i = 0; i < pList.Length; i++)
        {
            cardsToBePlayed.Add(PlayingCard.IntToCard(pList[i]));
            cardsToBePlayed[i].SetSprite(GetCardSprite(cardsToBePlayed[i].cardType, cardsToBePlayed[i].cardValue));
        }
        Debug.Log("cardsToBePlayed length: " + cardsToBePlayed.Count);
    }

    private int[] CardArrayToIntArray(PlayingCard[] pCards)
    {
        int[] cardInts = new int[pCards.Length];

        for (int i = 0; i < cardInts.Length; i++)
            cardInts[i] = PlayingCard.CardToInt(pCards[i]);

        return cardInts;
    }

    [PunRPC]
    private void dealCards()
    {
        if (playerObj == null)
        {
            Debug.LogError("PlayerObj is null", this);
            return;
        }

        GamePlayer player = playerObj.GetComponent<GamePlayer>();

        if (player == null)
        {
            Debug.LogError("PlayerObj doesn't contain GamePlayer component", this);
            return;
        }

            PlayingCard[] cardsInHand = new PlayingCard[3];
            PlayingCard[] cardsFaceUp = new PlayingCard[3];
            PlayingCard[] cardsFaceDown = new PlayingCard[3];

            for (int i = 0; i < 9; i++)
            {
                int index = i % 3;
                if (i < 3)
                    cardsFaceDown[index] = cardsToBePlayed[i];
                else if (i >= 3 && i < 6)
                    cardsFaceUp[index] = cardsToBePlayed[i];
                else if (i >= 6 && i < 9)
                    cardsInHand[index] = cardsToBePlayed[i];
            }

            player.InitCards(cardsInHand, cardsFaceUp, cardsFaceDown);

            //Need to remove the used cards from the cards to be played list
            for (int j = 0; j < 9; j++)
            {
                cardsToBePlayed.Remove(cardsToBePlayed[j]);
            }
            Debug.Log("cardsToBePLayed length after player init: " + cardsToBePlayed.ToArray().Length);
            this.photonView.RPC("SetCardsToBePlayed", RpcTarget.All, CardArrayToIntArray(cardsToBePlayed.ToArray()));
    }
}
