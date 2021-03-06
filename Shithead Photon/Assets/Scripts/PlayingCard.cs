using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public enum CardType { Clubs, Spades, Diamonds, Hearts, Joker };

[Serializable]
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
    }

    public PlayingCard (CardType pCardType, int pValue)
    {
        cardType = pCardType;
        cardValue = pValue;
    }

    #region serialization
    //public static byte[] Serialize(object obj)
    //{
    //    PlayingCard card = (PlayingCard)obj;
    //    //cardType
    //    byte[] cardTypeBytes = BitConverter.GetBytes(((int)card.cardType));
    //    if (BitConverter.IsLittleEndian)
    //        Array.Reverse(cardTypeBytes);

    //    //cardValue
    //    byte[] cardValueBytes = BitConverter.GetBytes((card.cardValue));
    //    if (BitConverter.IsLittleEndian)
    //        Array.Reverse(cardTypeBytes);

    //    //cardSprite
    //    byte[] textureBytes = card.cardSprite.texture.GetRawTextureData();
    //    if (BitConverter.IsLittleEndian)
    //        Array.Reverse(textureBytes);

    //    return joinBytes(cardTypeBytes, cardValueBytes, textureBytes);
    //}

    //public static object Deserialize(byte[] bytes)
    //{
    //    //cardType
    //    byte[] type = new byte[4];
    //    Array.Copy(bytes, 0, type, 0, type.Length);
    //    if (BitConverter.IsLittleEndian)
    //        Array.Reverse(type);
    //    CardType ct = (CardType)BitConverter.ToInt32(type, 0); //ct = card type

    //    //cardValue
    //    byte[] value = new byte[4];
    //    Array.Copy(bytes, 4, value, 0, value.Length);
    //   // if (BitConverter.IsLittleEndian)
    //        Array.Reverse(value);
    //    int cv = BitConverter.ToInt32(value, 0); //cv = card value

    //    //texture
    //    byte[] texture = new byte[bytes.Length - (type.Length + value.Length)]; //Gets the remaining bytes out of the given byte array
    //    Array.Copy(bytes, 8, texture, 0, texture.Length);
    //    if (BitConverter.IsLittleEndian)
    //        Array.Reverse(texture);
    //    Texture2D text = new Texture2D(2, 2, TextureFormat.RGBA32, false);
    //    text.LoadRawTextureData(texture);
    //    text.Apply();

    //    Debug.Log($"Deserialized stuff {ct} {cv} {text.width}");

    //    Sprite sprite = Sprite.Create(text, Rect.zero, Vector2.zero);

    //    return new PlayingCard(ct, cv, sprite);
    //}

    //private static byte[] joinBytes(params byte[][] arrays)
    //{
    //    byte[] rv = new byte[arrays.Sum(a => a.Length)];
    //    int offset = 0;
    //    foreach (byte[] array in arrays) 
    //    {
    //        Buffer.BlockCopy(array, 0, rv, offset, array.Length);
    //        offset += array.Length;
    //    }

    //    return rv;
    //}
    #endregion

    public Sprite GetSprite() => cardSprite;
    public CardType GetCardType() => cardType;
    public int GetCardValue() => cardValue;

    public static byte[] Serialize(PlayingCard pCard)
    {
        int cardInt = CardToInt(pCard);

        byte[] bytes = BitConverter.GetBytes(cardInt);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        return bytes;
    }

    public static PlayingCard Deserialize(byte[] pBytes)
    {
        byte[] bytes = pBytes;
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        int cardInt = BitConverter.ToInt32(bytes, 0);

        return IntToCard(cardInt);
    }

    /// <summary>
    /// Convert a playing card to an INT to be able to send it over the Photon network.
    /// </summary>
    /// <param name="pCard"></param>
    /// <returns>Returns an INT based on the card type and value. Hearts = 100, Clubs = 200, Diamonds = 300, Spades = 400</returns>
    public static int CardToInt(PlayingCard pCard)
    {
        if (pCard.cardType == CardType.Hearts)
            return 100 + pCard.cardValue;
        else if (pCard.cardType == CardType.Clubs)
            return 200 + pCard.cardValue;
        else if (pCard.cardType == CardType.Diamonds)
            return 300 + pCard.cardValue;
        else // Spades
            return 400 + pCard.cardValue;
    }

    /// <summary>
    /// Converts an INT to a playing card. This is used to convert an INT that has been sent over the Photon Network back to a playing card
    /// </summary>
    public static PlayingCard IntToCard(int pValue)
    {
        if (pValue >= 100 && pValue <= 113)
            return new PlayingCard(CardType.Hearts, pValue - 100);
        else if (pValue >= 200 && pValue <= 213)
            return new PlayingCard(CardType.Clubs, pValue - 200);
        else if (pValue >= 300 && pValue <= 313)
            return new PlayingCard(CardType.Diamonds, pValue - 300);
        else if (pValue >= 400 && pValue <= 413)
            return new PlayingCard(CardType.Spades, pValue - 400);
        else
            return null;
    }

    public override string ToString()
    {
        return $"Card Type: {cardType}\n Card Value: {cardValue}";
    }
}