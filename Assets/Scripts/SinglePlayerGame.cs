using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlayerGame : MonoBehaviour
{
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject card;


    private List<List<int>> allCards = new List<List<int>>()
    {
        new List<int>() { 6, 7, 8, 9, 10, 11, 12, 13, 14 },  // червы
        new List<int>() { 6, 7, 8, 9, 10, 11, 12, 13, 14 },  // бубиc
        new List<int>() { 6, 7, 8, 9, 10, 11, 12, 13, 14 },  // крести
        new List<int>() { 6, 7, 8, 9, 10, 11, 12, 13, 14 },  // пики
        new List<int>() { 1, 2 }  // Джокеры  красный / черный
    };

    private List<int> cardCountForLevel = new List<int>()
    {
        1, 2, 3, 4, 5, 6, 7, 8,  // 1 круг
        9, 9, 9, 9,  // 2 круг
        8, 7, 6, 5, 4, 3, 2, 1,  // 3 круг
        9, 9, 9, 9  // 4 круг
    };

    private List<int[]>[] playerCards = new List<int[]>[4] { new List<int[]>(), new List<int[]>(), new List<int[]>(), new List<int[]>() };

    private float round = 1;
    private float halfRound;

    private int whoseMove;

    private void Start()
    {
        whoseMove = Random.Range(0, 4);
        if (whoseMove == 0)
        {
        }
        else if (whoseMove == 1) StartCoroutine(Bot1());
        else if (whoseMove == 2) StartCoroutine(Bot2());
        else if (whoseMove == 3) StartCoroutine(Bot3());

        randomCards(1);
    }

    private void Update()
    {
    }

    private void Distribution()
    {
       for (int i = 0; i < playerCards[0].Count; i++)
        {
            GameObject currentCard = Instantiate(card,transform);
            currentCard.transform.position = canvas.transform.position + new Vector3(i, -0.4f, -0.1f);
        }
    }
    void randomCards(int cardsCount)
    {
        List<List<int>> allCardsTemp = DeepCopy(allCards);

        for (int j = 0; j < 4; j += 1)
        {
            for (int i = 0; i < cardsCount; i += 1)
            {
                // int x = Random.Range(0, 4);
                int x = Random.Range(0, allCardsTemp.Count);
                Debug.Log(x);
                int y = Random.Range(0, allCardsTemp[x].Count);
                Debug.Log(y);

                playerCards[j].Add(new int[] { x, allCardsTemp[x][y] });

                allCardsTemp[x].RemoveAt(y);
                if (allCardsTemp[x].Count < 1) allCardsTemp.RemoveAt(x);
            }
        }
        Distribution();
    }
    List<List<int>> DeepCopy(List<List<int>> lst)
    {
        List<List<int>> count = new List<List<int>>();

        for (int i = 0; i < lst.Count; i++) count.Add(new List<int>(lst[i]));

        return count;
    }

    private IEnumerator Bot1()
    {
        yield return new WaitForSeconds(1f);
        Debug.Log(1);
    }
    private IEnumerator Bot2()
    {
        yield return new WaitForSeconds(1f);
        Debug.Log(2);
    }
    private IEnumerator Bot3()
    {
        yield return new WaitForSeconds(1f);
        Debug.Log(3);
    }
}
