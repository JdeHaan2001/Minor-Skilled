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
    [SerializeField] private Text turnIndicator;
    [SerializeField] private Text winIndication;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject winLosePanel;

    private List<PlayingCard> cardsToBePlayed = new List<PlayingCard>();
    private List<PlayingCard> playedCards = new List<PlayingCard>();
    
    private const string keyCardsToBePlayed = "cardsToBePlayed";
    private const string currentTurnText = "Your turn";
    private const string waitForTurnText = "NOT your turn";

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
            this.photonView.RPC("setPanelActive", RpcTarget.All, false, true);

            //Makes all the playingcards
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

        //Set Starting player
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                this.photonView.RPC("dealCards", PhotonNetwork.PlayerList[i]);

            this.photonView.RPC("setPlayerState", PhotonNetwork.PlayerList[0], (int)PlayerStates.CurrentTurn);
            this.photonView.RPC("setTurnIndicatorText", PhotonNetwork.PlayerList[0], currentTurnText);

            for (int i = 1; i < PhotonNetwork.PlayerList.Length; i++)
            {
                this.photonView.RPC("setPlayerState", PhotonNetwork.PlayerList[i], (int)PlayerStates.WaitingForTurn);
                this.photonView.RPC("setTurnIndicatorText", PhotonNetwork.PlayerList[i], waitForTurnText);
            }
        }
    }

    /// <summary>
    /// Gets called when a card is clicked
    /// </summary>
    private void playCard(GameObject cardObj, CardType pType, int pValue)
    {
        Debug.Log($"Player: {PhotonNetwork.NickName} is trying to play card {pType} {pValue}");

        if (playerObj.GetComponent<GamePlayer>().currentState == PlayerStates.CurrentTurn)
        {
            if (cardObj.tag == "InHand")
            {
                Debug.Log($"InHand: {pType} : {pValue}");
                if (checkCardValue(pType, pValue))
                    handleCardPlay(pType, pValue, cardObj);
            }
            else if (cardObj.tag == "FaceUp" && playerObj.GetComponent<GamePlayer>().CardsInHand.Count == 0)
            {
                Debug.Log($"FaceUp: {pType} : {pValue}");
                if (checkCardValue(pType, pValue))
                    handleCardPlay(pType, pValue, cardObj);
            }
            else if (cardObj.tag == "FaceDown" && playerObj.GetComponent<GamePlayer>().CardsInHand.Count == 0 &&
                     playerObj.GetComponent<GamePlayer>().CardsFaceUp.Count == 0)
            {
                Debug.Log($"FaceDown: {pType} : {pValue}");

                if (checkCardValue(pType, pValue))
                    handleCardPlay(pType, pValue, cardObj);

                if (checkIfPlayerWon())
                {
                    Debug.Log("Player won");

                    setWinText(true);
                    this.photonView.RPC("setWinText", RpcTarget.Others, false);

                    setPanelActive(true, false);
                    this.photonView.RPC("setPanelActive", RpcTarget.Others, true, false);
                }
            }
            else
                Debug.Log($"Can't play {cardObj.tag}");
        }
        else
        {
            Debug.Log("Can't play. Not your turn");
        }
    }

    /// <summary>
    /// Sets all the player states in order to give the next player their turn
    /// </summary>
    private void handleNextRound(bool forceNextRound = false)
    {
        if (!forceNextRound)
        {
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                Debug.Log($"Checking player {PhotonNetwork.NickName}'s state");

                if (PhotonNetwork.PlayerList[i] == PhotonNetwork.LocalPlayer &&
                    playerObj.GetComponent<GamePlayer>().currentState == PlayerStates.CurrentTurn)
                {
                    Debug.Log($"Previous state: {playerObj.GetComponent<GamePlayer>().currentState}. " +
                        $"New state: {PlayerStates.WaitingForTurn}");

                    var timeL = new System.DateTimeOffset(System.DateTime.Now).ToUnixTimeSeconds();
                    setPlayerState((int)PlayerStates.WaitingForTurn, new PhotonMessageInfo(PhotonNetwork.LocalPlayer, (int)timeL, this.photonView));
                    setTurnIndicatorText(waitForTurnText, new PhotonMessageInfo(PhotonNetwork.LocalPlayer, (int)timeL, this.photonView));

                    if (i < PhotonNetwork.PlayerList.Length - 1)
                    {
                        this.photonView.RPC("setPlayerState", PhotonNetwork.PlayerList[i + 1], (int)PlayerStates.CurrentTurn);
                        this.photonView.RPC("setTurnIndicatorText", PhotonNetwork.PlayerList[i + 1], currentTurnText);
                        break;
                    }
                    else if (i == PhotonNetwork.PlayerList.Length - 1)
                    {
                        this.photonView.RPC("setPlayerState", PhotonNetwork.PlayerList[0], (int)PlayerStates.CurrentTurn);
                        this.photonView.RPC("setTurnIndicatorText", PhotonNetwork.PlayerList[0], currentTurnText);
                        break;
                    }
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                Debug.Log("Checking state");
                if (PhotonNetwork.PlayerList[i] == PhotonNetwork.LocalPlayer)
                {
                    var timeL = new System.DateTimeOffset(System.DateTime.Now).ToUnixTimeSeconds();
                    setPlayerState((int)PlayerStates.WaitingForTurn, new PhotonMessageInfo(PhotonNetwork.LocalPlayer, (int)timeL , this.photonView));
                    setTurnIndicatorText(waitForTurnText, new PhotonMessageInfo(PhotonNetwork.LocalPlayer, (int)timeL, this.photonView));

                    if (i < PhotonNetwork.PlayerList.Length - 1)
                    {
                        this.photonView.RPC("setPlayerState", PhotonNetwork.PlayerList[i + 1], (int)PlayerStates.CurrentTurn);
                        this.photonView.RPC("setTurnIndicatorText", PhotonNetwork.PlayerList[i + 1], currentTurnText);
                        break;
                    }
                    else if (i == PhotonNetwork.PlayerList.Length - 1)
                    {
                        this.photonView.RPC("setPlayerState", PhotonNetwork.PlayerList[0], (int)PlayerStates.CurrentTurn);
                        this.photonView.RPC("setTurnIndicatorText", PhotonNetwork.PlayerList[0], currentTurnText);
                        break;
                    }
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Handles what happens when a card gets played
    /// </summary>
    private void handleCardPlay(CardType pType, int pValue, GameObject cardObj)
    {
        if (pValue != 2 && pValue != 3 && pValue != 7 && pValue != 10)
        {
            if (lastPlayedCardValue == 1 && pValue == 1)
            {
                lastPlayedCardValue = pValue;
                this.photonView.RPC("updateLastPlayedCardUI", RpcTarget.All, pType, pValue);
            }
            else if (pValue >= lastPlayedCardValue && lastPlayedCardValue != 1) //The ace is the highest card but has a value of 1 due to automisation issues
            {
                lastPlayedCardValue = pValue;
                this.photonView.RPC("updateLastPlayedCardUI", RpcTarget.All, pType, pValue);
            }
        }
        else if (pValue == 2 || pValue == 3 || pValue == 7 || pValue == 10)
        {
            switch (pValue)
            {
                case 2:
                    lastPlayedCardValue = 0;
                    this.photonView.RPC("updateLastPlayedCardUI", RpcTarget.All, pType, pValue);
                    break;
                case 3:
                    this.photonView.RPC("updateLastPlayedCardUI", RpcTarget.All, pType, pValue);
                    break;
                case 7:
                    if (7 >= lastPlayedCardValue)
                    {
                        lastPlayedCardValue = pValue;
                        this.photonView.RPC("updateLastPlayedCardUI", RpcTarget.All, pType, pValue);
                    }
                    break;
                case 10:
                    lastPlayedCardValue = 0;
                    playedCards.Clear();
                    playedCards.TrimExcess();
                    this.photonView.RPC("updatePlayedCardsList", RpcTarget.All, CardArrayToIntArray(playedCards.ToArray()));
                    this.photonView.RPC("destroyLastPlayedCardObj", RpcTarget.All);
                    break;
            }
        }
        else
        {
            Debug.Log("Can't play card, card has unknown value: " + pValue);
            return;
        }

        //Update last played card value
        this.photonView.RPC("setLastPlayedCardValue", RpcTarget.All, lastPlayedCardValue);

        //Update the list that tracks all the played cards that are still in the game
        if (pValue != 10)
        {
            playedCards.Add(cardObj.GetComponent<CardHolder>().card);
            this.photonView.RPC("updatePlayedCardsList", RpcTarget.All, CardArrayToIntArray(playedCards.ToArray()));
        }

        //Updates the player's cards in hand list
        GamePlayer player = playerObj.GetComponent<GamePlayer>();

        player.GetCardInHand().Remove(cardObj.GetComponent<CardHolder>().card);
        player.GetCardInHand().TrimExcess();
        Destroy(cardObj);

        if (cardsToBePlayed.Count != 0)
        {
            PlayingCard card = cardsToBePlayed[0];

            player.GetCardInHand().Add(card);
            InstantiatePlayingCardInHand(card);

            cardsToBePlayed.Remove(card);
            cardsToBePlayed.TrimExcess();
            this.photonView.RPC("updateCardsToBePlayedList", RpcTarget.All, CardArrayToIntArray(cardsToBePlayed.ToArray()));
        }

        //Sets playerstates to give an indication if it's their turn or not
        if (pValue != 10)
            handleNextRound();
    }

    /// <summary>
    /// Checks if the value of the card is higher or equal to the previous played card; or if the card is a special card
    /// </summary>
    private bool checkCardValue(CardType pType, int pValue)
    {
        if (pValue != 2 && pValue != 3 && pValue != 7 && pValue != 10)
        {
            if (lastPlayedCardValue != 7 && pValue == 1)
                return true;
            else if (pValue >= lastPlayedCardValue && lastPlayedCardValue != 1) //The ace is the highest card but has a value of 1 due to automisation issues
                return true;
            else
                return false;
        }
        else if (pValue == 2 || pValue == 3 || pValue == 7 || pValue == 10)
        {
            switch (pValue)
            {
                case 2:
                    return true;
                case 3:
                    return true;
                case 7:
                    if (lastPlayedCardValue <= 7)
                        return true;
                    else
                        return false;
                case 10:
                    return true;
            }
            return false;
        }
        else
            return false;
    }
    
    /// <summary>
    /// Updates the UI of the last played card
    /// </summary>
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
    /// Destroys the game object of the last played card
    /// </summary>
    [PunRPC]
    private void destroyLastPlayedCardObj()
    {
        Destroy(lastPlayedCardObj);
        lastPlayedCardObj = null;
    }

    /// <summary>
    /// Sets the value of the last played card
    /// </summary>
    /// <param name="pValue"></param>
    [PunRPC]
    private void setLastPlayedCardValue(int pValue)
    {
        Debug.Log($"Chaning last played card value from {lastPlayedCardValue} to {pValue}");
        lastPlayedCardValue = pValue;
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
    /// Converts an array of integers to an array playingcard objects
    /// </summary>
    /// <param name="pInts"></param>
    /// <returns></returns>
    private PlayingCard[] IntArrayToCardArray(int[] pInts)
    {
        PlayingCard[] cards = new PlayingCard[pInts.Length];

        for (int i = 0; i < cards.Length; i++)
        {
            cards[i] = PlayingCard.IntToCard(pInts[i]);
            cards[i].SetSprite(GetCardSprite(cards[i].cardType, cards[i].cardValue));
        }

        return cards;
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
            InstantiatePlayingCardInHand(card);

        foreach (PlayingCard card in gamePlayer.GetCardFaceUp())
        {
            GameObject obj = Instantiate(cardHolderObj, faceUpCardPos);

            obj.GetComponent<Image>().sprite = card.cardSprite;
            obj.transform.name = card.cardSprite.name;
            obj.tag = "FaceUp";

            CardHolder holder = obj.GetComponent<CardHolder>();
            holder.type = card.cardType;
            holder.value = card.cardValue;

            obj.GetComponent<CardButton>().CardClick += playCard;
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

            obj.GetComponent<CardButton>().CardClick += playCard;
        }
    }

    /// <summary>
    /// Instantiates a playing card game object for the player
    /// </summary>
    /// <param name="pCard"></param>
    private void InstantiatePlayingCardInHand(PlayingCard pCard)
    {
        GameObject obj = Instantiate(cardHolderObj, new Vector3(cardSpawnPos.position.x, cardSpawnPos.position.y, 0f),
                                        Quaternion.identity, cardSpawnPos);
        obj.GetComponent<Image>().sprite = pCard.cardSprite;
        obj.transform.name = pCard.cardSprite.name;
        obj.tag = "InHand";

        CardHolder holder = obj.GetComponent<CardHolder>();
        holder.type = pCard.cardType;
        holder.value = pCard.cardValue;
        holder.card = pCard;

        obj.GetComponent<CardButton>().CardClick += playCard;
    }

    /// <summary>
    /// Updates the list of the last played cards
    /// </summary>
    /// <param name="pList"></param>
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

        this.photonView.RPC("updateCardsToBePlayedList", RpcTarget.All, CardArrayToIntArray(cardsToBePlayed.ToArray()));
    }

    /// <summary>
    /// Deals the cards for the player
    /// </summary>
    [PunRPC]
    private void dealCards()
    {
        GamePlayer player = playerObj.GetComponent<GamePlayer>();

        if (playerObj == null)
        {
            Debug.LogError("PlayerObj is null", this);
            return;
        }

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
    private void updateCardsToBePlayedList(int[] pCardArray)
    {
        //cardsToBePlayed = pCardArray.ToList();
        cardsToBePlayed = IntArrayToCardArray(pCardArray).ToList();
    }

    /// <summary>
    /// Updates the played card list
    /// </summary>
    /// <param name="pCardArray"></param>
    [PunRPC]
    private void updatePlayedCardsList(int[] pCardArray)
    {
        playedCards = IntArrayToCardArray(pCardArray).ToList();
    }

    /// <summary>
    /// Sets the state for the player. If the given state is currentTurn there will also be a check if the player
    /// is able to play cards
    /// </summary>
    /// <param name="pState"></param>
    [PunRPC]
    private void setPlayerState(int pState, PhotonMessageInfo info)
    {
        Debug.Log($"Setting player state. Function called by {info.Sender}");
        Debug.Log($"Current player state: {(PlayerStates)pState}");
        if ((PlayerStates)pState == PlayerStates.CurrentTurn)
        {
            GamePlayer player = playerObj.GetComponent<GamePlayer>();
            if (player != null)
            {
                bool canPlay = false;

                if (player.CardsInHand.Count != 0)
                {
                    Debug.Log($"Player: {PhotonNetwork.NickName} has more then 0 cards in hand");
                    canPlay = checkIfAbleToPlayCard(player.CardsInHand);
                }
                else if (player.CardsInHand.Count == 0 && player.CardsFaceUp.Count != 0)
                {
                    Debug.Log($"Player: {PhotonNetwork.NickName} has less then 0 cards in hand");
                    canPlay = checkIfAbleToPlayCard(player.CardsFaceUp);
                }

                if (!canPlay)
                {
                    Debug.Log("No cards are playable. Grabbing all cards");

                    foreach (PlayingCard card in playedCards)
                    {
                        player.AddCardInHand(card);
                        InstantiatePlayingCardInHand(card);
                    }

                    playedCards.Clear();
                    playedCards.TrimExcess();
                    this.photonView.RPC("updatePlayedCardsList", RpcTarget.All, CardArrayToIntArray(playedCards.ToArray()));
                    this.photonView.RPC("destroyLastPlayedCardObj", RpcTarget.All);

                    this.photonView.RPC("setLastPlayedCardValue", RpcTarget.All, 0);

                    Debug.Log("Starting next round");
                    handleNextRound(true);
                }
                else
                {
                    Debug.Log($"Player {PhotonNetwork.NickName}: chaning state from {player.currentState} to {(PlayerStates)pState}");
                    player.currentState = (PlayerStates)pState;
                }
            }
        }
        else
            playerObj.GetComponent<GamePlayer>().currentState = (PlayerStates)pState;
    }

    /// <summary>
    /// Loops through the given list to check if a card, from that list, can be played
    /// </summary>
    private bool checkIfAbleToPlayCard(List<PlayingCard> pList)
    {
        foreach (PlayingCard card in pList)
        {
            if (checkCardValue(card.cardType, card.cardValue))
            {
                Debug.Log("Setting canPlay to TRUE");
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Checks if the local player has won or not
    /// </summary>
    private bool checkIfPlayerWon()
    {
        GamePlayer player = playerObj.GetComponent<GamePlayer>();

        if (player != null) {
            if (player.CardsInHand.Count == 0 && player.CardsFaceDown.Count == 0 && player.CardsFaceUp.Count == 0)
                return true;
            else
                return false;
        }
        return false;
    }

    /// <summary>
    /// sets text to indicate if it's the player's turn or not
    /// </summary>
    /// <param name="pText"></param>
    [PunRPC]
    private void setTurnIndicatorText(string pText, PhotonMessageInfo info)
    {
        Debug.Log($"Setting player turn indicatior text. Function called by {info.Sender}");
        Debug.Log($"Setting player {PhotonNetwork.NickName} turn text to {pText}, while current state is {playerObj.GetComponent<GamePlayer>().currentState}");
        //turnIndicator.text = pText;
        GamePlayer player = playerObj.GetComponent<GamePlayer>();

        if (player != null)
        {
            if (player.currentState == PlayerStates.CurrentTurn)
            {
                turnIndicator.text = "Your Turn";
            }
            else
                turnIndicator.text = "NOT your turn";
        }
    }

    /// <summary>
    /// Sets the game panel and win/lose panel active or inactive
    /// </summary>
    [PunRPC]
    private void setPanelActive(bool pWinLosePanel, bool pGamePanel)
    {
        gamePanel.SetActive(pGamePanel);
        winLosePanel.SetActive(pWinLosePanel);
    }

    /// <summary>
    /// Sets the text on the win/lose panel
    /// </summary>
    [PunRPC]
    private void setWinText(bool pHasWon)
    {
        if (pHasWon)
            winIndication.text = "You win!";
        else
            winIndication.text = "You lost!";
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