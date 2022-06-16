using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using Hastable = ExitGames.Client.Photon.Hashtable;

public class CardManager : GameManager
{
    //TODO: Make override function to update the playingcardlist

    [SerializeField] private List<PlayingCard> playingCardList = new List<PlayingCard>();
    private List<PlayingCard> cardsToBePlayed;
    private List<PlayingCard> playedCards = new List<PlayingCard>();
    private List<Sprite> cardSprites = new List<Sprite>();

    private ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
    private const string keyCardsToBePlayed = "cardsToBePlayed";
    private PlayingCard _card = null;

    //[Range(0, 4)]
    private int JokerAmount = 0;

    private void Start()
    {
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
                playingCardList.Add(MakePlayingCard(CardType.Clubs, i));
            else if (i > 13 && i <= 26)
                playingCardList.Add(MakePlayingCard(CardType.Diamonds, i % 13 == 0 ? 13 : i % 13));
            else if (i > 26 && i <= 39)
                playingCardList.Add(MakePlayingCard(CardType.Spades, i % 13 == 0 ? 13 : i % 13));
            else if (i > 39 && i <= 52)
                playingCardList.Add(MakePlayingCard(CardType.Hearts, i % 13 == 0 ? 13 : i % 13));
        }

        ///Loads joker cards, TODO: Find art for joker cards
        //for (int j = 0; j < JokerAmount; j++)
        //{
        //    playingCardList.Add(MakePlayingCard(CardType.JOKER, 0));
        //}

        //cardsToBePlayed = new List<PlayingCard>(playingCardList);

        //customProperties[keyCardsToBePlayed] = playingCardList;
        //PhotonNetwork.SetPlayerCustomProperties(customProperties);

        //this.photonView.RPC("SetCardsToBePlayed", RpcTarget.All, playingCardList);
        //this.photonView.RPC("SetCard", RpcTarget.AllViaServer, playingCardList[0]);

        if (photonView.IsMine)
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hastable() { { "Cards", playingCardList.ToArray() } });
    }

    private PlayingCard MakePlayingCard(CardType pCardType, int pValue)
    {
        PlayingCard card = null;
        string SpriteName = string.Empty;
        foreach (Sprite sprite in cardSprites)
        {
            if (sprite.name == $"{pCardType}_{pValue}_white" && pValue <= 10)
            {
                card = new PlayingCard(pCardType, pValue, sprite);
                SpriteName = sprite.name;
                Debug.Log($"Created card: {pCardType}_{pValue}_white with sprite: {sprite.name}");
                break;
            }
            else if (sprite.name == $"{pCardType}_Jack_white" && pValue == 11)
            {
                card = new PlayingCard(pCardType, pValue, sprite);
                SpriteName = sprite.name;
                Debug.Log($"Created card: {pCardType}_{pValue}_white with sprite: {sprite.name}");
                break;
            }
            else if (sprite.name == $"{pCardType}_Queen_white" && pValue == 12)
            {
                card = new PlayingCard(pCardType, pValue, sprite);
                SpriteName = sprite.name;
                Debug.Log($"Created card: {pCardType}_{pValue}_white with sprite: {sprite.name}");
                break;
            }
            else if (sprite.name == $"{pCardType}_King_white" && pValue == 13)
            {
                card = new PlayingCard(pCardType, pValue, sprite);
                SpriteName = sprite.name;
                Debug.Log($"Created card: {pCardType}_{pValue}_white with sprite: {sprite.name}");
                break;
            }
            else if (sprite.name == $"{pCardType}_A_white" && pValue == 1)
            {
                card = new PlayingCard(pCardType, pValue, sprite);
                SpriteName = sprite.name;
                Debug.Log($"Created card: {pCardType}_{pValue}_white with sprite: {sprite.name}");
                break;
            }

            SpriteName = sprite.name;
        }

        if (card == null)
            Debug.Log($"Card {pCardType}_{pValue}_white could not be made\nSprite name: {SpriteName}");

        return card;
    }

    [PunRPC]
    private void SetCardsToBePlayed(PlayingCard[] pList) => cardsToBePlayed = pList.ToList();

    [PunRPC]
    private void SetCard(PlayingCard card) => _card = card;

    private void SendCards(PlayingCard data)
    {
        this.photonView.RPC("RPCReceiveCards", RpcTarget.All, PlayingCard.Serialize(data));
    }

    [PunRPC]
    private void RPCReceiveCards(byte[] datas)
    {
        PlayingCard card = (PlayingCard)PlayingCard.Deserialize(datas);
        Debug.Log("Received byte array");
    }
}
