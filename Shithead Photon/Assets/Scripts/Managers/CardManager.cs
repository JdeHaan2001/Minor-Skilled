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
    [SerializeField] private Transform cardSpawnPos;
    [SerializeField] private GameObject cardHolderObj;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Transform faceUpCardPos;
    [SerializeField] private Transform faceDownCardPos;

    private List<PlayingCard> cardsToBePlayed = new List<PlayingCard>();
    private List<PlayingCard> playedCards = new List<PlayingCard>();
    
    private const string keyCardsToBePlayed = "cardsToBePlayed";
    private PlayingCard _card = null;
    private GameObject playerObj = null;
    private GameObject lastPlayedCardObj = null;

    //[Range(0, 4)]
    private int JokerAmount = 0;
    private int lastPlayedCardValue = -1;
    [Tooltip("Scale to adjust size of the card sprite.\nThe higher the number the smaller the card")]
    [SerializeField] [Range(1f, 10f)] private float cardScale = 3f;

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

            List<PlayingCard> shuffledList = new List<PlayingCard>(shuffleCards(playingCardList));
            playingCardList = shuffledList;

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

        //Set Starting player
        if (PhotonNetwork.IsMasterClient)
        {
            this.photonView.RPC("setPlayerState", PhotonNetwork.PlayerList[0], (int)PlayerStates.CurrentTurn);

            for (int i = 1; i < PhotonNetwork.PlayerList.Length; i++)
                this.photonView.RPC("setPlayerState", PhotonNetwork.PlayerList[i], (int)PlayerStates.WaitingForTurn);
        }
    }

    private void getCardValue(GameObject cardObj, CardType pType, int pValue) 
    {
        if (cardObj.tag == "InHand")
        {
            Debug.Log($"InHand: {pType} : {pValue}");
            checkCardValue(pType, pValue);
        }
        else if (cardObj.tag == "FaceUp" && playerObj.GetComponent<GamePlayer>().CardsInHand.Count == 0)
        {
            Debug.Log($"FaceUp: {pType} : {pValue}");
            checkCardValue(pType, pValue);
        }
        else if (cardObj.tag == "FaceDown" && playerObj.GetComponent<GamePlayer>().CardsInHand.Count == 0 &&
                 playerObj.GetComponent<GamePlayer>().CardsFaceUp.Count == 0)
        {
            Debug.Log($"FaceDown: {pType} : {pValue}");
            checkCardValue(pType, pValue);
        }
        else
            Debug.Log($"Can't play {cardObj.tag}");
    }

    private bool checkCardValue(CardType pType, int pValue)
    {
        if (pValue != 2 && pValue != 3 && pValue != 7 && pValue != 10)
        {
            if (pValue >= lastPlayedCardValue)
            {
                //TODO: play card
                lastPlayedCardValue = pValue;
                Debug.Log("Spawning Card");
                this.photonView.RPC("updateLastPlayedCardUI", RpcTarget.All, pType, pValue);
                return true;
            }
            else
                return false;
        }
        else if (pValue == 2 || pValue == 3 || pValue == 7 || pValue == 10)
        {
            switch (pValue)
            {
                case 2:
                    lastPlayedCardValue = pValue;
                    this.photonView.RPC("updateLastPlayedCardUI", RpcTarget.All, pType, pValue);
                    return true;
                case 3:
                    lastPlayedCardValue = pValue;
                    this.photonView.RPC("updateLastPlayedCardUI", RpcTarget.All, pType, pValue);
                    return true;
                case 7:
                    lastPlayedCardValue = pValue;
                    this.photonView.RPC("updateLastPlayedCardUI", RpcTarget.All, pType, pValue);
                    return true;
                case 10:
                    lastPlayedCardValue = pValue;
                    this.photonView.RPC("updateLastPlayedCardUI", RpcTarget.All, pType, pValue);
                    return true;
            }

            return false;
        }
        else
            return false;
    }

    [PunRPC]
    private void updateLastPlayedCardUI(CardType pType, int pValue)
    {
        if (lastPlayedCardObj == null)
        {
            lastPlayedCardObj = Instantiate(cardHolderObj, canvas.transform);
            lastPlayedCardObj.GetComponent<Image>().sprite = GetCardSprite(pType, pValue);
            lastPlayedCardObj.transform.name = "LastPlayedCard";
            lastPlayedCardObj.GetComponent<RectTransform>().sizeDelta /= 4.5f;
        }
        else
            lastPlayedCardObj.GetComponent<Image>().sprite = GetCardSprite(pType, pValue);
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

    /// <summary>
    /// Converts an array of PlayingCard objects to an array of integers
    /// </summary>
    /// <param name="pCards"></param>
    /// <returns></returns>
    private int[] CardArrayToIntArray(PlayingCard[] pCards)
    {
        int[] cardInts = new int[pCards.Length];

        for (int i = 0; i < cardInts.Length; i++)
            cardInts[i] = PlayingCard.CardToInt(pCards[i]);

        return cardInts;
    }

    /// <summary>
    /// Initializes the UI for the cards
    /// </summary>
    //TODO: Make functions for the foreach loops
    [PunRPC]
    private void initCardUI()
    {
        GamePlayer gamePlayer = playerObj.GetComponent<GamePlayer>();

        Debug.Log("CardsInHand" + gamePlayer.GetCardInHand());

        foreach (PlayingCard card in gamePlayer.GetCardInHand())
        {
            GameObject obj = Instantiate(cardHolderObj, new Vector3(cardSpawnPos.position.x, cardSpawnPos.position.y, 0f),
                                        Quaternion.identity, cardSpawnPos);
            obj.GetComponent<Image>().sprite = card.cardSprite;
            obj.transform.name = card.cardSprite.name;
            obj.tag = "InHand";

            CardHolder holder = obj.GetComponent<CardHolder>();
            holder.type = card.cardType;
            holder.value = card.cardValue;

            obj.GetComponent<CardButton>().CardClick += getCardValue;
        }

        foreach (PlayingCard card in gamePlayer.GetCardFaceUp())
        {
            GameObject obj = Instantiate(cardHolderObj, faceUpCardPos);

            obj.GetComponent<Image>().sprite = card.cardSprite;
            obj.transform.name = card.cardSprite.name;
            obj.tag = "FaceUp";

            CardHolder holder = obj.GetComponent<CardHolder>();
            holder.type = card.cardType;
            holder.value = card.cardValue;

            obj.GetComponent<CardButton>().CardClick += getCardValue;
        }

        foreach (PlayingCard card in gamePlayer.GetCardFaceDown())
        {
            GameObject obj = Instantiate(cardHolderObj, faceDownCardPos);
            obj.transform.eulerAngles += new Vector3(0, 180, 0); //Rotate card so that back of the card is showing
            obj.GetComponent<Image>().color = Color.black;
            obj.transform.name = card.cardSprite.name;
            obj.tag = "FaceDown";

            CardHolder holder = obj.GetComponent<CardHolder>();
            holder.type = card.cardType;
            holder.value = card.cardValue;

            obj.GetComponent<CardButton>().CardClick += getCardValue;
        }
    }

    [PunRPC]
    private void SetCardsToBePlayed(int[] pList)
    {
        cardsToBePlayed.Clear();
        cardsToBePlayed.TrimExcess();

        for (int i = 0; i < pList.Length; i++)
        {
            cardsToBePlayed.Add(PlayingCard.IntToCard(pList[i]));
            cardsToBePlayed[i].SetSprite(GetCardSprite(cardsToBePlayed[i].cardType, cardsToBePlayed[i].cardValue));
        }

        //updateCardsToBePlayedList(cardsToBePlayed.ToArray());
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

        //this.photonView.RPC("SetCardsToBePlayed", RpcTarget.All, CardArrayToIntArray(cardsToBePlayed.ToArray()));
        SetCardsToBePlayed(CardArrayToIntArray(cardsToBePlayed.ToArray()));
        //this.photonView.RPC("initCardUI", RpcTarget.All);
        initCardUI();
    }

    /// <summary>
    /// Sync a card array for all clients. Need to convert list to an array because Lists 
    /// aren't serializable by PUN
    /// </summary>
    [PunRPC]
    private void updateCardsToBePlayedList(PlayingCard[] pCardArray)
    {
        cardsToBePlayed = pCardArray.ToList();
    }

    [PunRPC]
    private void setPlayerState(int pState)
    {
        playerObj.GetComponent<GamePlayer>().currentState = (PlayerStates)pState;
    }

    public override void OnLeftRoom()
    {
        playedCards.Clear();
        playedCards.TrimExcess();

        cardsToBePlayed.Clear();
        cardsToBePlayed.TrimExcess();
        base.OnLeftRoom();
    }
}
