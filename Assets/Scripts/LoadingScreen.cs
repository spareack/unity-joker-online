using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private Text waitingText;

    void Start()
    {
        StartCoroutine(waitingCoroutine());
    }

    IEnumerator waitingCoroutine()
    {
        waitingText.text = "Preparing game";
        yield return new WaitForSeconds(1f);

        waitingText.text = "Preparing game.";
        yield return new WaitForSeconds(1f);

        waitingText.text = "Preparing game..";
        yield return new WaitForSeconds(1f);
        
        waitingText.text = "Preparing game...";
        yield return new WaitForSeconds(1f);

        StartCoroutine(waitingCoroutine());
    }
}
