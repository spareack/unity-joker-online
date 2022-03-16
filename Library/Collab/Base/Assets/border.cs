using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class border : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField] public game gameScr;
    [SerializeField] private int status = 0; // 0 - не нажата, 1 - нажата первым, 2 - вторым, 3 - граничная

    [SerializeField] public int[] borderIndex;

    public void OnPointerDown(PointerEventData eventData)
    {
        gameScr.pushBorder(borderIndex);
        Debug.Log("DOWN");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("UP");
    }
    public void OnPointerExit(PointerEventData eventData)
    {
    }
}
