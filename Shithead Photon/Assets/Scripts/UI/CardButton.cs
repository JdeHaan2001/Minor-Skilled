using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardButton : MonoBehaviour
{
    public delegate void OnCardClick(GameObject cardObj, CardType pType, int pValue);

    public event OnCardClick CardClick;

    private CardHolder cardHolder;

    private void Start()
    {
        cardHolder = GetComponent<CardHolder>();
        this.GetComponent<Button>().onClick.AddListener(raiseCardClick);
    }

    private void raiseCardClick()
    {
        LogSystem.Log($"Clicked on Card {cardHolder.type} {cardHolder.value}");
        CardClick?.Invoke(this.gameObject, cardHolder.type, cardHolder.value);
    }
}
