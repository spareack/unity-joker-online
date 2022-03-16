using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class waitingScreen : MonoBehaviour
{
    [SerializeField] private Text waitingText;

    void Start()
    {
        StartCoroutine(waitingCoroutine());
    }

    IEnumerator waitingCoroutine()
    {
        waitingText.text = "Waiting Opponent";
        yield return new WaitForSeconds(1f);

        waitingText.text = "Waiting Opponent.";
        yield return new WaitForSeconds(1f);

        waitingText.text = "Waiting Opponent..";
        yield return new WaitForSeconds(1f);
        
        waitingText.text = "Waiting Opponent...";
        yield return new WaitForSeconds(1f);

        StartCoroutine(waitingCoroutine());
    }
}
