using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

using Random = UnityEngine.Random;

public class handCards : MonoBehaviour
{
    [SerializeField] public List<Card> cardList = new List<Card>();
    [SerializeField] public GameObject cardPrefab;
    [SerializeField] public game GameScr;

    [SerializeField] public Text cardsOrdered;
    [SerializeField] public Text cardsReceived;
    [SerializeField] public Text scoreText;

    [SerializeField] private float maxWidth = 10f;

    [SerializeField] private GameObject cardPlace;
    [SerializeField] public GameObject outLine;


    public Card throwCard(int[] cardValue, int cardIndex, bool hiden)
    {
        var card = cardList[cardIndex];
        var comp = card.GetComponent<Card>();

        bool flip = GameScr.allHands.IndexOf(this) != 0;

        if (!flip) comp.setCardValue(cardValue[0], cardValue[1], hiden, this);
        else comp.setCardValueBack(cardValue[0], cardValue[1], hiden, this);

        StartCoroutine(throwCardCoroutine(card, flip));
        cardList.RemoveAt(cardIndex);
        
        return comp;
    }


    IEnumerator throwCardCoroutine(Card card, bool flip)
    {
        float duration = 1f;

        int val = 4 - GameScr.throwedCards.Where(x => x == null).ToList().Count;
        var endPos = cardPlace.transform.position + Vector3.back * 0.0001f * val;

        Quaternion endRot = Quaternion.identity;
        if (flip) endRot = Quaternion.Euler(cardPlace.transform.localRotation.eulerAngles + new Vector3(0, 180, 0));
        else endRot = cardPlace.transform.localRotation;

        for(float t = 0; t < duration; t += Time.deltaTime)
        {
            card.transform.position = Vector3.Lerp(card.transform.position, endPos, t / duration);
            card.transform.localRotation = Quaternion.Lerp(card.transform.localRotation, endRot, t / duration);
            card.transform.localScale = Vector3.Lerp(card.transform.localScale, cardPrefab.transform.localScale, t / duration);
            yield return null;
        }
        card.transform.position = endPos;
        card.transform.localRotation = endRot;
        card.transform.localScale = cardPrefab.transform.localScale;

        normalizeCardsPos();
    }

    public void setCardsOrdered(int count)
    {
        cardsOrdered.text = "Заказано: " + count;
    }
    public void setCardsReceived(int count)
    {
        cardsReceived.text = "Взято: " + count;
    }
    public void setScore(int count)
    {
        scoreText.text = "Счет: " + count;
    }

    public void addCard(int suit, int Value, bool hiden, bool myHand = false)
    {
        var card = Instantiate(cardPrefab, transform);
        if (myHand) card.transform.localScale *= 1.25f;

        var comp = card.GetComponent<Card>();
        comp.handCardsScr = this;
        comp.setCardValue(suit, Value, hiden, this);

        cardList.Add(comp);
        normalizeCardsPos();
    }

    public void removeCard(int index)
    {
        Destroy(cardList[index].gameObject);
        cardList.RemoveAt(index);

        normalizeCardsPos();
    }

    public void clearCards()
    {
        while(cardList.Count > 0)
        {
            Destroy(cardList[0].gameObject);
            cardList.RemoveAt(0);
        }
        normalizeCardsPos();
    }

    void normalizeCardsPos()
    {
        var startPos = transform.position;
        var endPos = startPos + new Vector3(maxWidth, 0, 0);

        for(int i = 0; i < cardList.Count; i += 1)
        {
            StartCoroutine(setPosTo(cardList[i].gameObject, Vector3.Lerp(startPos, endPos, (float)i / cardList.Count)));
            // cardList[i].transform.localPosition = Vector3.Lerp(startPos, endPos, (float)i / cardList.Count);
        }
    }

    IEnumerator setPosTo(GameObject obj, Vector3 endPos)
    {
        float duration = 0.3f;
        var startPos = obj.transform.localPosition;
        for(float t = 0; t < duration; t += Time.deltaTime)
        {
            obj.transform.localPosition = Vector3.Lerp(startPos, endPos, t / duration);
            yield return null;
        }
        obj.transform.localPosition = endPos;
    }

    void hideCardsValue()
    {
        foreach(var card in cardList) card.valueText.text = "X";
    }
    void showCardsValue()
    {
        foreach(var card in cardList) card.valueText.text = "" + card.Value;
    }
}
