using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public handCards handCardsScr;

    public int Value { get; set; }
    public int Suit { get; set; }
    public bool hiden { get; set; }
    public Texture2D Texture;
    public Text valueText;
    public Text suitText;

    private Vector3 mOffset;
    private Vector3 previousPosition;
    private float mZCoord;
    private bool onPlase;
    public string destinationTag = "DropArea";

    public SpriteRenderer renderer;
    public SpriteRenderer backRenderer;

    public Image rendererImage;
    public Image backRendererImage;

    public void setCardValue(int suit, int Value, bool hiden, handCards handCardsScr)
    {
        this.Suit = suit;
        this.Value = Value;
        this.hiden = hiden;

        if (hiden)
        {
            Texture2D texture = handCardsScr.GameScr.shirt;
            rendererImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }
        else
        {
            Texture2D texture = handCardsScr.GameScr.allCardsPrefabs[suit][Value];
            rendererImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }
    }
    public void setCardValueBack(int suit, int Value, bool hiden, handCards handCardsScr)
    {
        this.Suit = suit;
        this.Value = Value;
        this.hiden = hiden;

        if (hiden)
        {
            Texture2D texture = handCardsScr.GameScr.shirt;
            backRendererImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }
        else
        {
            Texture2D texture = handCardsScr.GameScr.allCardsPrefabs[suit][Value];
            backRendererImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }
    }


    private void OnMouseDown()
    {
        if (handCardsScr.GameScr.myTurn)
        {
            int index = handCardsScr.cardList.IndexOf(this);
            handCardsScr.GameScr.choseCardValue(index, new int[] {Suit, Value}, handCardsScr.cardList);

            // gameObject.SetActive(false);
        }

        // if (!onPlase)
        // {
        //     previousPosition = gameObject.transform.position;
        //     mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        //     mOffset = gameObject.transform.position - GetMouseWorldPos();
        // }
    }

    private void OnMouseUp()
    {
        // if (!onPlase)
        // {
        //     var rayOrigin = Camera.main.transform.position;
        //     var rayDirection = GetMouseWorldPos() - Camera.main.transform.position;
        //     RaycastHit hitInfo;
        //     if (Physics.Raycast(rayOrigin, rayDirection, out hitInfo) && handCardsScr.GameScr.myTurn)
        //     {
        //         if (hitInfo.transform.tag == destinationTag)
        //         {
        //             onPlase = true;
        //             handCardsScr.throwCard(new int[] {Value, Suit}, this);
        //             gameObject.SetActive(false);
        //             // transform.position = hitInfo.transform.position - new Vector3(0f, 3f, 0.1f);
        //         }
        //         else
        //         {
        //             transform.position = previousPosition;
        //         }
        //     }
        // }
    }

    private void OnMouseDrag()
    {
        // if (!onPlase)
        // {
        //     transform.position = GetMouseWorldPos() + mOffset;
        // }
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mZCoord;

        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

}
