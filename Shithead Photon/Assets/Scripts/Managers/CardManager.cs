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

    private List<PlayingCard> cardsToBePlayed;
    private List<PlayingCard> playedCards = new List<PlayingCard>();
    
    private const string keyCardsToBePlayed = "cardsToBePlayed";
    private PlayingCard _card = null;

    //[Range(0, 4)]
    private int JokerAmount = 0;

    private void Start()
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

        //cardsToBePlayed = new List<PlayingCard>(playingCardList);

        //customProperties[keyCardsToBePlayed] = playingCardList;
        //PhotonNetwork.SetPlayerCustomProperties(customProperties);

        //this.photonView.RPC("SetCardsToBePlayed", RpcTarget.All, playingCardList);
        //this.photonView.RPC("SetCard", RpcTarget.AllViaServer, playingCardList[0]);
    }

    private PlayingCard MakePlayingCard(CardType pCardType, int pValue)
    {      
        return new PlayingCard(pCardType, pValue, spriteAtlas.GetSprite($"{pCardType}_{pValue}_white"));
    }

    //[PunRPC]
    //private void SetCardsToBePlayed(PlayingCard[] pList) => cardsToBePlayed = pList.ToList();

    //[PunRPC]
    //private void SetCard(PlayingCard card) => _card = card;

    //private void SendCards(PlayingCard data)
    //{
    //    //this.photonView.RPC("RPCReceiveCards", RpcTarget.All, PlayingCard.Serialize(data));
    //}

    //[PunRPC]
    //private void RPCReceiveCards(byte[] datas)
    //{
    //    //PlayingCard card = (PlayingCard)PlayingCard.Deserialize(datas);
    //    Debug.Log("Received byte array");
    //}
}
