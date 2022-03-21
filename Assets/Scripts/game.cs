using Photon.Pun;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine;
using System;
using UnityEditor;
using System.Linq;

using Random = UnityEngine.Random;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class game : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public bool singlePlayerGame = false;

    [SerializeField] public networkManager Network;
    [SerializeField] private DataCheck DC;

    [SerializeField] private string enemyID = "";
    private bool waitingOpponent = true;

    [SerializeField] private GameObject addFriendButton;
    [SerializeField] private CanvasScaler scaler;

    [SerializeField] SendOptions sendOptions = new SendOptions { Reliability = true };

    [SerializeField] private GameObject waitingMenu;
    [SerializeField] private GameObject gameMenu;
    [SerializeField] private GameObject loadingMenu;

    [SerializeField] private Text timesRemainText;
    [SerializeField] private Text whichTurnText;
    [SerializeField] private Text gamePlace;

    [SerializeField] private Image firstAvatar;
    [SerializeField] private Image firstAvatarOutline;

    [SerializeField] private Sprite whiteOutline;
    [SerializeField] private Sprite redOutline;

    [SerializeField] private Coroutine calcTimeForTurn = null;

    [SerializeField] private GameObject winEffect;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;

    [SerializeField] private GameObject trumpPlace;

    //[SerializeField] private IronMouse IM;
    [SerializeField] private Saver S;

    private float superTimer;

    //-------- Joker 3D

    private int waitCount = -1;

    private int whichTurn = -1;
    public bool myTurn = false;
    public int firstTurnSuit = -1;

    private int lastChooseCardsCount = -1;
    private int lastChooseCardIndex = -1;

    private Coroutine waitCoroutine = null;


    //----------------
    private int stage;
    private int round = 0;

    private Player[] allPlayers = new Player[4];

    private int[] playersScore = new int[] { 0, 0, 0 ,0 };
    private int[] orderCardCount = new int[] { 0, 0, 0, 0 };
    private int[] winCardCount = new int[] { 0, 0, 0, 0 };
    public int[][] cardsChosen = new int[][] { new int[]{0, 0}, new int[]{0, 0}, new int[]{0, 0}, new int[]{0, 0} };
    public Card[] throwedCards = new Card[4];

    public List<int[]>[] playerCards = null;

    private int[] currentTrump = new int[] {-1, -1};
    private GameObject trumpObject = null;
    private int trumpSuit = -1;

    private int myIndex = 0;

    void Init()
    {
        var Players = PhotonNetwork.PlayerList.OrderBy(x => x.ActorNumber).ToList();
        for(int i = 0; i < Players.Count; i += 1) allPlayers[i] = Players[i];
        myIndex = Array.IndexOf(allPlayers, PhotonNetwork.LocalPlayer);
    }

    [SerializeField] private GameObject orderMenu;
    [SerializeField] private Button[] howManyCardOrderButton;

    [SerializeField] private GameObject jokerChoseUpDown;
    [SerializeField] private GameObject jokerChoseSuit;
    [SerializeField] private int jokerCount;
    [SerializeField] private int jokerIndex;

    [SerializeField] public List<handCards> allHands = new List<handCards>();

    // [SerializeField] public List<List<int>> allCards = new List<List<int>>()
    // {
    //     new List<int>() { 6, 7, 8, 9, 10, 11, 12, 13, 14 },  // червы
    //     new List<int>() { 6, 7, 8, 9, 10, 11, 12, 13, 14 },  // бубиc
    //     new List<int>() { 6, 7, 8, 9, 10, 11, 12, 13, 14 },  // крести
    //     new List<int>() { 6, 7, 8, 9, 10, 11, 12, 13, 14 },  // пики
    //     new List<int>() { 1, 2 }  // Джокеры  красный / черный
    // };
    
    [SerializeField] public List<List<int>> allCardsIndexes = new List<List<int>>()
    {
        new List<int>() { 4, 5, 6, 7, 8, 9, 10, 11, 12 },  // червы
        new List<int>() { 4, 5, 6, 7, 8, 9, 10, 11, 12 },  // бубиc
        new List<int>() { 4, 5, 6, 7, 8, 9, 10, 11, 12 },  // крести
        new List<int>() { 4, 5, 6, 7, 8, 9, 10, 11, 12 },  // пики
        new List<int>() { 0, 1 }  // Джокеры  красный / черный
    };

    [SerializeField] public int[] allJokerCounts = new int[4];
    [SerializeField] public List<List<Texture2D>> allCardsPrefabs = new List<List<Texture2D>>();

    [SerializeField] private List<Texture2D> hearts = new List<Texture2D>();
    [SerializeField] private List<Texture2D> diamonds = new List<Texture2D>();
    [SerializeField] private List<Texture2D> clubs = new List<Texture2D>();
    [SerializeField] private List<Texture2D> spades = new List<Texture2D>();
    [SerializeField] private List<Texture2D> jokers = new List<Texture2D>();
    [SerializeField] public Texture2D shirt;

    [SerializeField] private List<int> cardCountForLevel = new List<int>() 
    {
        1, 2, 3, 4, 5
    };
    
    // [SerializeField] private List<int> cardCountForLevel = new List<int>() 
    // {
    //     1, 2, 3, 4, 5, 6, 7, 8,  // 1 круг
    //     9, 9, 9, 9,  // 2 круг
    //     8, 7, 6, 5, 4, 3, 2, 1,  // 3 круг
    //     9, 9, 9, 9  // 4 круг
    // };

    //private int whichTurn;

    void Awake()
    {
        allCardsPrefabs.Add(hearts);
        allCardsPrefabs.Add(diamonds);
        allCardsPrefabs.Add(clubs);
        allCardsPrefabs.Add(spades);
        allCardsPrefabs.Add(jokers);
    }

    void setPlayerChoose(int playerNum, int cardsCount)
    {
        int index = getRealHandIndex(playerNum);
        allHands[index].setCardsOrdered(cardsCount);
    }

    void setPlayerReceived(int playerNum, int cardsCount, int[] scores)
    {
        int index = getRealHandIndex(playerNum);
        allHands[index].setCardsReceived(cardsCount);
        for(int i = 0; i < 4; i += 1) allHands[getRealHandIndex(i)].setScore(scores[i]);
    }

    void setPlayerTurn(int playerNum, int[] cardValue, int cardIndex, bool hiden)
    {
        int index = getRealHandIndex(playerNum);
        throwedCards[index] = allHands[index].throwCard(cardValue, cardIndex, hiden);
    }

    int getRealHandIndex(int playerNum)
    {
        if (myIndex % 2 != 0) 
        {
            int count = (playerNum - myIndex) % 4;
            while(count < 0) count += 4;

            return count;
        }
        else return (myIndex + playerNum) % 4;
    }

    void enableOutlineGlow(int playerNum)
    {
        int index = getRealHandIndex(playerNum);

        for(int i = 0; i < 4; i += 1) allHands[i].outLine.SetActive(false);

        allHands[index].outLine.SetActive(true);
    }

    private void OpenOrderMenu(int roundNum, int lastOrderedSum)
    {
        for (int i = 0; i < howManyCardOrderButton.Length; i += 1) howManyCardOrderButton[i].interactable = false;
        for (int i = 0; i < cardCountForLevel[roundNum] + 1; i += 1) howManyCardOrderButton[i].interactable = true;

        if (lastOrderedSum != -1)
        {
            int index = cardCountForLevel[roundNum] - lastOrderedSum;
            if (index >= 0 && index < howManyCardOrderButton.Length)
            {
                howManyCardOrderButton[cardCountForLevel[roundNum] - lastOrderedSum].interactable = false;
            }
        }
        orderMenu.SetActive(true);
    }

    public void choseCardsCount(int count)
    {
        myTurn = false;
        allHands[0].outLine.SetActive(false);

        if (!singlePlayerGame) sendEvent20(PhotonNetwork.LocalPlayer, true, count, -1);
        else
        {
            lastChooseCardsCount = count;
            waitCount += 1;
        }
        CloseOrderMenu();
    }

    public void choseCardValue(int cardIndex, int[] cardValue, List<Card> cardList)
    {
        if (firstTurnSuit != -1 && cardValue[0] != 4)
        {
            if (cardValue[0] != firstTurnSuit)
            {
                if (cardList.Any(x => x.Suit == firstTurnSuit)) return;

                if (trumpSuit != 4 && cardValue[0] != trumpSuit)
                {
                    if (cardList.Any(x => x.Suit == trumpSuit)) return;
                }
            }
        }

        if (cardValue[0] == 4)
        {
            if (firstTurnSuit == -1) jokerChoseSuit.SetActive(true);
            else jokerChoseUpDown.SetActive(true);
            myTurn = false;
            jokerIndex = cardIndex;
            return;
        }

        myTurn = false;
        allHands[0].outLine.SetActive(false);

        if (!singlePlayerGame) sendEvent20(PhotonNetwork.LocalPlayer, false, cardIndex, -1);
        else
        {
            lastChooseCardIndex = cardIndex;
            jokerCount = -1;
            waitCount += 1;
        }
        return;
    }

    public int getCardIndexForBotTurn(int playerNum)
    {
        if (playerCards[playerNum].Any(x => x[0] == firstTurnSuit))
        {
            var item = playerCards[playerNum].Where(x => x[0] == firstTurnSuit).OrderBy(r => Random.Range(0, 1000)).First();
            return playerCards[playerNum].IndexOf(item);
        }
        else if (playerCards[playerNum].Any(x => x[0] == trumpSuit))
        {
            var item = playerCards[playerNum].Where(x => x[0] == trumpSuit).OrderBy(r => Random.Range(0, 1000)).First();
            return playerCards[playerNum].IndexOf(item);
        }
        else return Random.Range(0, playerCards[playerNum].Count);
    }


    public void setJokerChoseUpDown(int count)
    {
        jokerChoseUpDown.SetActive(false);
        allHands[0].outLine.SetActive(false);

        if (!singlePlayerGame) sendEvent20(PhotonNetwork.LocalPlayer, false, jokerIndex, count);
        else
        {
            lastChooseCardIndex = jokerIndex;
            jokerCount = count;
            waitCount += 1;
        }
    }

    public void setJokerChoseSuit(int count)
    {
        jokerChoseSuit.SetActive(false);
        allHands[0].outLine.SetActive(false);
        if (!singlePlayerGame) sendEvent20(PhotonNetwork.LocalPlayer, false, jokerIndex, 2 + count);
        else
        {
            lastChooseCardIndex = jokerIndex;
            jokerCount = 2 + count;
            waitCount += 1;
        }
    }

    private void CloseOrderMenu()
    {
        orderMenu.SetActive(false);
    }

    private int Scoring(int numPlayer)
    {
        if (orderCardCount[numPlayer] != winCardCount[numPlayer] && winCardCount[numPlayer] == 0) // Штанга
        {
            playersScore[numPlayer] -= 200;
            return -200;
        }
        else if (orderCardCount[numPlayer] != winCardCount[numPlayer]) // Перегрузка
        {
            playersScore[numPlayer] += 10;
            return 10;
        }
        else if (orderCardCount[numPlayer] == 0) // Пас
        {
            playersScore[numPlayer] += 50;
            return 50;
        }
        else if (orderCardCount[numPlayer] == winCardCount[numPlayer]) // Взятка
        {
            if (cardCountForLevel[round] == 1 && orderCardCount[numPlayer] == 1) playersScore[numPlayer] += 100;
            else if (cardCountForLevel[round] == 2 && orderCardCount[numPlayer] == 2) playersScore[numPlayer] += 200;
            else if (cardCountForLevel[round] == 2 && orderCardCount[numPlayer] < 2) playersScore[numPlayer] += 150;
            else if (cardCountForLevel[round] == 3 && orderCardCount[numPlayer] == 3) playersScore[numPlayer] += 300;
            else if (cardCountForLevel[round] == 3 && orderCardCount[numPlayer] < 3) playersScore[numPlayer] += 200;
            else if (cardCountForLevel[round] == 4 && orderCardCount[numPlayer] == 4) playersScore[numPlayer] += 400;
            else if (cardCountForLevel[round] == 4 && orderCardCount[numPlayer] < 4) playersScore[numPlayer] += 250;
            else if (cardCountForLevel[round] == 5 && orderCardCount[numPlayer] == 5) playersScore[numPlayer] += 500;
            else if (cardCountForLevel[round] == 5 && orderCardCount[numPlayer] < 5) playersScore[numPlayer] += 300;
            else if (cardCountForLevel[round] == 6 && orderCardCount[numPlayer] == 6) playersScore[numPlayer] += 600;
            else if (cardCountForLevel[round] == 6 && orderCardCount[numPlayer] < 6) playersScore[numPlayer] += 350;
            else if (cardCountForLevel[round] == 7 && orderCardCount[numPlayer] == 7) playersScore[numPlayer] += 700;
            else if (cardCountForLevel[round] == 7 && orderCardCount[numPlayer] < 7) playersScore[numPlayer] += 400;
            else if (cardCountForLevel[round] == 8 && orderCardCount[numPlayer] == 8) playersScore[numPlayer] += 800;
            else if (cardCountForLevel[round] == 8 && orderCardCount[numPlayer] < 8) playersScore[numPlayer] += 450;
            else if (cardCountForLevel[round] == 9 && orderCardCount[numPlayer] == 9) playersScore[numPlayer] += 900;
            else return -2;
        }
        return 0;
    }

    void Start()
    {
        if (!singlePlayerGame)
        {
            if (PhotonNetwork.PlayerList.Length > 3)
            {
                loadingMenu.SetActive(true);
                waitingMenu.SetActive(false);
                gameMenu.SetActive(false);
            }
            else
            {
                waitingMenu.SetActive(true);
                gameMenu.SetActive(false);
                loadingMenu.SetActive(false);
            }
            loadSavings();
        }
        else StartCoroutine(startMainSingleRoundCoroutine());
    }

    void Update()
    {
        if (!waitingOpponent) superTimer += Time.deltaTime;

        if (!waitingOpponent && PhotonNetwork.PlayerList.Length < 2) Network.leaveGame();
    }

    void loadSavings()
    {
        if (S.save.avatarPath != "")
        {
            string path = S.save.avatarPath;
            setAvatarImageTo(path, firstAvatar);
        }
        // firstNickname.text = S.save.Nickname;
    }

    int setAvatarImageTo(string path, Image avatarImage)
    {
        Texture2D texture = NativeGallery.LoadImageAtPath(path);
        if (texture == null) return 1;
        avatarImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        return 0;
    }

    public void startLoadGame()
    {
        if (PhotonNetwork.IsMasterClient) StartCoroutine(startLoadCoroutine());

        loadingMenu.SetActive(true);
        waitingMenu.SetActive(false);
        gameMenu.SetActive(false);
    }

    IEnumerator startLoadCoroutine()
    {
        waitCount = 0;
        sendEvent1();
    
        while(waitCount < 3) yield return null;

        sendEvent3();
    }

    public void StartGame()
    {
        waitingOpponent = false;
        gameMenu.SetActive(true);
        waitingMenu.SetActive(false);
        loadingMenu.SetActive(false);
        Init();

        if (PhotonNetwork.IsMasterClient)
        {
            round = 0;
            StartCoroutine(startPreparingRoundCoroutine());
        }
    }


    IEnumerator startMainSingleRoundCoroutine()
    {
        randomCards(cardCountForLevel[round]);

        int[] mas1 = playerCards[0].Select(x => x[0]).ToArray();
        int[] mas2 = playerCards[0].Select(x => x[1]).ToArray();
        StartCoroutine(Distribution(mas1, mas2, currentTrump));
        

        yield return new WaitForSeconds(cardCountForLevel[round] * 0.1f * 4);

        int playerNum = Random.Range(0, 4);

        for (int i = 0; i < 4; i++)
        {
            enableOutlineGlow(playerNum);

            if (playerNum == myIndex)
            {
                if (i == 3) OpenOrderMenu(round, orderCardCount.Sum());
                else OpenOrderMenu(round, -1);

                waitCount = 0;
                var t = 0f;
                while(waitCount < 1 && t < 20f) 
                {
                    t += Time.deltaTime;
                    yield return null;
                }

                if (waitCount < 1)
                {
                    orderCardCount[playerNum] = 0;
                    CloseOrderMenu();
                    setPlayerChoose(playerNum, orderCardCount[playerNum]);
                }
                else 
                {
                    orderCardCount[playerNum] = lastChooseCardsCount;
                    setPlayerChoose(playerNum, orderCardCount[playerNum]);
                }
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
                var trumpCardsCount = playerCards[playerNum].Where(x => x[0] == trumpSuit || x[0] == 4).ToList().Count;
                orderCardCount[playerNum] = trumpCardsCount;
                setPlayerChoose(playerNum, orderCardCount[playerNum]);
            }
            playerNum += 1;
            playerNum %= 4;
        }

// - - - - - - - - - 

        for (int j = 0; j < cardCountForLevel[round]; j += 1)
        {
            firstTurnSuit = -1;

            for (int i = 0; i < 4; i++)
            {
                enableOutlineGlow(playerNum);

                if (playerNum == myIndex)
                {
                    myTurn = true;

                    jokerCount = -1;
                    waitCount = 0;
                    var t = 0f;
                    while(waitCount < 1 && t < 40f)
                    {
                        t += Time.deltaTime;
                        yield return null;
                    }

                    if (waitCount < 1)
                    {
                        myTurn = false;
                        jokerChoseSuit.SetActive(false);
                        jokerChoseUpDown.SetActive(false);
                        allHands[0].outLine.SetActive(false);

                        int randomCardIndex = Random.Range(0, playerCards[playerNum].Count);
                        cardsChosen[playerNum] = playerCards[playerNum][randomCardIndex];

                        if (cardsChosen[playerNum][0] == 4)
                        {
                            if (i == 0) allJokerCounts[playerNum] = Random.Range(0, 2);
                            else allJokerCounts[playerNum] = Random.Range(2, 6);
                        }
                        setPlayerTurn(playerNum, cardsChosen[playerNum], randomCardIndex, allJokerCounts[playerNum] == 1);
                        playerCards[playerNum].RemoveAt(randomCardIndex);
                    }
                    else
                    {
                        cardsChosen[playerNum % 4] = playerCards[playerNum][lastChooseCardIndex];
                        allJokerCounts[playerNum] = jokerCount;
                        setPlayerTurn(playerNum, cardsChosen[playerNum], lastChooseCardIndex, allJokerCounts[playerNum] == 1);
                        playerCards[playerNum].RemoveAt(lastChooseCardIndex);
                    }
                }
                else
                {
                    yield return new WaitForSeconds(0.5f);

                    lastChooseCardIndex = getCardIndexForBotTurn(playerNum);
                    cardsChosen[playerNum] = playerCards[playerNum][lastChooseCardIndex];

                    if (playerCards[playerNum][lastChooseCardIndex][0] == 4)
                    {
                        if (i == 0) allJokerCounts[playerNum] = Random.Range(2, 6);
                        else allJokerCounts[playerNum] = (winCardCount[playerNum] < orderCardCount[playerNum] ? 0 : 1);
                    }
                    else allJokerCounts[playerNum] = -1;
                    setPlayerTurn(playerNum, cardsChosen[playerNum], lastChooseCardIndex, allJokerCounts[playerNum] == 1);
                    playerCards[playerNum].RemoveAt(lastChooseCardIndex);
                }

                if (i == 0)
                {
                    if (cardsChosen[playerNum][0] == 4) firstTurnSuit = allJokerCounts[playerNum] - 2;
                    else firstTurnSuit = cardsChosen[playerNum][0];
                }

                playerNum += 1;
                playerNum %= 4;
            }

            int winnerIndex = Array.IndexOf(cardsChosen, maxCardValue(playerNum));
            winCardCount[winnerIndex] += 1;
            setPlayerReceived(winnerIndex, winCardCount[winnerIndex], playersScore);
            playerNum = winnerIndex;

            // if (j == cardCountForLevel[round] - 1)
            // {
            //     for(int i = 1; i < 4; i += 1) Scoring(i);
            //     Debug.Log(Scoring(0));
            //     setPlayerReceived(0, winCardCount[0], playersScore);
            //     winCardCount = new int[] { 0, 0, 0, 0 };

            //     round += 1;
            //     StartCoroutine(startMainSingleRoundCoroutine());
            // }

            yield return StartCoroutine(receiveCardsAnimate(winnerIndex));

            // yield return new WaitForSeconds(2f);
            clearThrowedCards();
        }
        for(int i = 1; i < 4; i += 1) Scoring(i);
        Debug.Log(Scoring(0));
        setPlayerReceived(0, winCardCount[0], playersScore);
        winCardCount = new int[] { 0, 0, 0, 0 };

        round += 1;
        if (round >= cardCountForLevel.Count)
        {
            endGameFunction();
            yield break;
        }
        checkEndRoundAchievemnt();
        StartCoroutine(startMainSingleRoundCoroutine());
    }


// ======================


    IEnumerator startPreparingRoundCoroutine()
    {
        randomCards(cardCountForLevel[round]);
        sendEvent31(playerCards, currentTrump);

        yield return new WaitForSeconds(cardCountForLevel[round] * 0.1f * 4);

        int playerNum = Random.Range(0, 4);
        float t = 0f;
        waitCount = 0;

        for (int i = 0; i < 4; i++)
        {
            if (i == 3) sendEvent10(playerNum, true, orderCardCount.Sum());
            else sendEvent10(playerNum, true, -1);

            t = 0f;
            while(waitCount < i+1 && t < 20f) 
            {
                t += Time.deltaTime;
                yield return null;
            }

            if (waitCount < i+1)
            {
                orderCardCount[playerNum] = cardCountForLevel[round];
                sendEvent11(allPlayers[playerNum], true);
                sendEvent32(playerNum, true, cardCountForLevel[round]);
                waitCount += 1;
            }
            else 
            {
                orderCardCount[playerNum] = lastChooseCardsCount;
                sendEvent32(playerNum, true, lastChooseCardsCount);
            }

            playerNum += 1;
            playerNum %= 4;
        }
        StartCoroutine(startMainRoundCoroutine(playerNum));
    }

    IEnumerator startMainRoundCoroutine(int playerNum)
    {
        for (int j = 0; j < cardCountForLevel[round]; j += 1)
        {
            waitCount = 0;
            int firstSuit = -1;

            for (int i = 0; i < 4; i++)
            {
                float t = 0f;
                sendEvent10(playerNum, false, firstSuit);

                while(waitCount < i+1 && t < 40f)
                {
                    t += Time.deltaTime;
                    yield return null;
                }

                if (waitCount < i+1)
                {
                    sendEvent11(allPlayers[playerNum], false);

                    int randomCardIndex = Random.Range(0, playerCards[playerNum].Count);
                    cardsChosen[playerNum] = playerCards[playerNum][randomCardIndex];

                    if (cardsChosen[playerNum][0] == 4)
                    {
                        if (i == 0) allJokerCounts[playerNum] = Random.Range(0, 2);
                        else allJokerCounts[playerNum] = Random.Range(2, 6);
                    }
                    sendEvent32(playerNum, false, cardsChosen[playerNum], randomCardIndex, allJokerCounts[playerNum] == 1);
                    playerCards[playerNum].RemoveAt(randomCardIndex);
                    waitCount += 1;
                }
                else
                {
                    allJokerCounts[playerNum] = jokerCount;
                    cardsChosen[playerNum] = playerCards[playerNum][lastChooseCardIndex];
                    sendEvent32(playerNum, false, cardsChosen[playerNum], lastChooseCardIndex, allJokerCounts[playerNum] == 1);
                    playerCards[playerNum].RemoveAt(lastChooseCardIndex);
                }

                if (i == 0)
                {
                    if (cardsChosen[playerNum][0] == 4) firstSuit = allJokerCounts[playerNum] - 2;
                    else firstSuit = cardsChosen[playerNum][0];
                }

                playerNum += 1;
                playerNum %= 4;
            }

            int winnerIndex = Array.IndexOf(cardsChosen, maxCardValue(playerNum));
            winCardCount[winnerIndex] += 1;

            // for(int i = 0; i < 4; i += 1) Scoring(i);
            sendEvent33(winnerIndex, winCardCount[winnerIndex], playersScore);
            
            playerNum = winnerIndex;
            yield return new WaitForSeconds(3.5f);
            clearThrowedCards();
        }
        for(int i = 0; i < 4; i += 1) Scoring(i);
        sendEvent33(0, winCardCount[0], playersScore);
        winCardCount = new int[] { 0, 0, 0, 0 };

        round += 1;
        if (round >= cardCountForLevel.Count)
        {
            endGameFunction();
            yield break;
        }
        checkEndRoundAchievemnt();
        StartCoroutine(startPreparingRoundCoroutine());
    }

    IEnumerator receiveCardsAnimate(int winnerIndex)
    {
        int index = getRealHandIndex(winnerIndex);
        yield return new WaitForSeconds(1f);

        float duration = 0.5f;
        Vector3[] startPosOffsets = new Vector3[] { Vector3.down, Vector3.left, Vector3.up, Vector3.right };
        Vector3[] startPoses = new Vector3[4];
        Quaternion[] startRotates = new Quaternion[4];

        for(int i = 0; i < 4; i += 1) startPoses[i] = throwedCards[i].transform.position;
        for(int i = 0; i < 4; i += 1) startRotates[i] = throwedCards[i].transform.rotation;

        for(float t = 0; t < duration; t += Time.deltaTime)
        {
            for(int i = 0; i < throwedCards.Length; i += 1)
            {
                Vector3 endPos = startPoses[index] + startPosOffsets[i] * 0.3f;
                endPos.z = throwedCards[i].transform.position.z;
                throwedCards[i].transform.position = Vector3.Lerp(throwedCards[i].transform.position, endPos, t / duration);
                throwedCards[i].transform.rotation = Quaternion.Slerp(throwedCards[i].transform.rotation, startRotates[index], t / duration);
            }
            yield return null;
        }
        for(int i = 0; i < throwedCards.Length; i += 1)
        {
            Vector3 endPos = startPoses[index] + startPosOffsets[i] * 0.3f;
            endPos.z = throwedCards[i].transform.position.z;
            throwedCards[i].transform.position = endPos;
            throwedCards[i].transform.rotation = startRotates[index];
        }


        duration = 1f;

        for(int i = 0; i < throwedCards.Length; i += 1) startPoses[i] = throwedCards[i].transform.position;

        for(float t = 0; t < duration; t += Time.deltaTime)
        {
            for(int i = 0; i < throwedCards.Length; i += 1)
            {
                Vector3 endPos = startPoses[i] + startPosOffsets[index] * 15f;
                endPos.z = throwedCards[i].transform.position.z;
                throwedCards[i].transform.position = Vector3.Lerp(throwedCards[i].transform.position, endPos, t / duration);
            }
            yield return null;
        }
        for(int i = 0; i < throwedCards.Length; i += 1)
        {
            Vector3 endPos = startPoses[i] + startPosOffsets[index] * 15f;
            endPos.z = throwedCards[i].transform.position.z;
            throwedCards[i].transform.position = endPos;
        }

        yield return new WaitForSeconds(0);
    }

    void clearOrderCounts()
    {
        foreach(var hand in allHands) hand.setCardsOrdered(0);
    }
    void clearReceivedCounts()
    {
        foreach(var hand in allHands) hand.setCardsReceived(0);
    }

    int[] maxCardValue(int playerNum)
    {
        int[] maxCard = null;

        if (cardsChosen.Any(x => x[0] == 4))
        {
            List<int[]> jokersTemp = new List<int[]>();

            for(int i = 0 ; i < cardsChosen.Length; i += 1)
            {
                if (cardsChosen[i][0] == 4 && allJokerCounts[i] != 1)
                {
                    Debug.Log("ohh joker with count = " + allJokerCounts[i]);
                    jokersTemp.Add(cardsChosen[i]);
                }
            }
            if (jokersTemp.Count > 0)
            {
                Debug.Log("joker win with - " + jokersTemp.Last()[0] + " " + jokersTemp.Last()[1]);
                return jokersTemp.Last();
            } 
        }

        if (cardsChosen.Any(x => x[0] == currentTrump[0]))
        {
            var trumpCards = cardsChosen.Where(x => x[0] == currentTrump[0]).ToList();
            maxCard = trumpCards.Where(x => x[1] == trumpCards.Max(y => y[1])).First();
            Debug.Log("trump win with " + maxCard[0] + " " + maxCard[1]);
        }
        else
        {
            var tempCards = cardsChosen.Where(x => x[0] == cardsChosen[playerNum % 4][0]).ToList();
            maxCard = tempCards.Where(x => x[1] == tempCards.Max(y => y[1])).First();
            Debug.Log("casual win with " + maxCard[0] + " " + maxCard[1]);
        }
        return maxCard;
    }

    List<List<int>> deepCopy(List<List<int>> lst)
    {
        List<List<int>> count = new List<List<int>>();

        for (int i = 0; i < lst.Count; i++) count.Add(new List<int>(lst[i]));

        return count;
    }

    void randomCards(int cardsCount)
    {
        List<List<int>> allCardsIndexesTemp = deepCopy(allCardsIndexes);
        playerCards = new List<int[]>[4] {new List<int[]>(), new List<int[]>(), new List<int[]>(), new List<int[]>()};

        for (int j = 0; j < playerCards.Length; j += 1)
        {
            for(int i = 0; i < cardsCount; i += 1)
            {
                var (x, y) = getRandomCardIndex(allCardsIndexesTemp);

                // int x = Random.Range(0, allCardsIndexesTemp.Count);
                // int y = Random.Range(0, allCardsIndexesTemp[x].Count);

                playerCards[j].Add(new int[] {x, allCardsIndexesTemp[x][y]} );
                allCardsIndexesTemp[x].RemoveAt(y);
                if (allCardsIndexesTemp[x].Count < 1) allCardsIndexesTemp.RemoveAt(x);
            }
        }
        TrumpDefinition(allCardsIndexesTemp);
    }

    (int, int) getRandomCardIndex(List<List<int>> allCardsIndexesTemp)
    {
        List<(int, int)> allIndexes = new List<(int, int)>();
        for(int i = 0; i < allCardsIndexesTemp.Count; i += 1)
        {
            for(int j = 0; j < allCardsIndexesTemp[i].Count; j += 1) allIndexes.Add((i, j));
        }
        return allIndexes.OrderBy(x => Random.Range(0, 1000)).First();
    }

    void clearThrowedCards()
    {
        for (int i = 0; i < throwedCards.Length; i++)
        {
            if (throwedCards[i] != null) Destroy(throwedCards[i].gameObject);
        }
    }

    private IEnumerator Distribution(int[] mas1, int[] mas2, int[] trump)
    {
        if (trumpObject != null) StartCoroutine(leaveTrumpAnimation());
        foreach(var hand in allHands) hand.clearCards();
        clearReceivedCounts();
        clearOrderCounts();
        clearThrowedCards();

        for (int i = 0; i < cardsChosen.Length; i++) cardsChosen[i] = new int[2];

        for (int j = 0; j < mas1.Length; j++)
        {
            for (int i = 0; i < allHands.Count; i++)
            {
                if (i == 0) allHands[0].addCard(mas1[j], mas2[j], false, true);
                else allHands[i].addCard(0, 0, true);

                yield return new WaitForSeconds(0.1f);
            }
        }
        checkStartRoundAchievemnt();
        StartCoroutine(AddTrump(trump[0], trump[1]));
    }

    IEnumerator leaveTrumpAnimation()
    {
        float duration = 1f;
        var endPos = trumpObject.transform.position + Vector3.up * 10f;
        for(float t = 0; t < duration; t += Time.deltaTime)
        {
            trumpObject.transform.position = Vector3.Lerp(trumpObject.transform.position, endPos, t / duration);
            yield return null;
        }
        trumpObject.transform.position = endPos;
    }

    private IEnumerator AddTrump(int suit, int Value)
    {
        if (trumpObject != null) Destroy(trumpObject);

        trumpObject = Instantiate(allHands[0].cardPrefab, transform);
        trumpObject.transform.position = trumpPlace.transform.position;
        trumpObject.transform.rotation = trumpPlace.transform.rotation;
        
        Card cardScr = trumpObject.GetComponent<Card>();
        cardScr.setCardValue(suit, Value, false, allHands[0]);
        trumpSuit = suit;

        float duration = 1f;

        var startPos = trumpObject.transform.position + Vector3.up * 10;
        var endPos = trumpObject.transform.position;

        trumpObject.transform.position = startPos;

        for(float t = 0; t < duration; t += Time.deltaTime)
        {
            trumpObject.transform.position = Vector3.Lerp(trumpObject.transform.position, endPos, t / duration);
            yield return null;
        }
        trumpObject.transform.position = endPos;
    }

    private void TrumpDefinition(List<List<int>> remainingCards)
    {
        int x = Random.Range(0, remainingCards.Count);
        int y = Random.Range(0, remainingCards[x].Count);
        currentTrump = new int[] {x, remainingCards[x][y]};
    }

    private void endGameFunction()
    {
        var myPlace = new HashSet<int>(playersScore).OrderBy(x => x).ToList().IndexOf(playersScore[myIndex]);

        StartCoroutine(YouWin(myPlace));

        // if (playersScore[myIndex] == playersScore.Max()) StartCoroutine(YouWin(myPlace));
        // else StartCoroutine(YouLose(myPlace));

        // Network.leaveGame();
        //else drawGame();
    }

    private IEnumerator YouWin(int myPlace)
    {
        S.save.Rating += 50;
        S.saveChanges();
        gamePlace.text = myPlace.ToString();
        checkEndGameAchievemnt(myPlace);

        winEffect.SetActive(true);
        winScreen.SetActive(true);
        yield return new WaitForSeconds(5f);

        if (superTimer >= 200)
        {
            //IM.ShowRewards();
            yield return new WaitForSeconds(1f);
        }

        Network.leaveGame();
    }
    void checkStartRoundAchievemnt()
    {
        if (DC == null) return;
        if (allHands[myIndex].cardList.Any(x => x.Suit == 4)) 
        {
            DC.achievementProgress[5] += 1;
            DC.achievementProgress[7] += allHands[myIndex].cardList.Where(x => x.Suit == 4).Count();
            if (allHands[myIndex].cardList.Where(x => x.Suit == 4).Count() > 1) DC.achievementProgress[6] += 1;
        }
        DC.checkForAchievement();
    }

    void checkEndRoundAchievemnt()
    {
        if (DC == null) return;
        if (orderCardCount[myIndex] == cardCountForLevel[round] && orderCardCount[myIndex] == winCardCount[myIndex])
        {
            DC.achievementProgress[11] += 1;
            DC.achievementProgress[12] += 1;
            DC.achievementProgress[13] += 1;
            DC.achievementProgress[14] += 1;
        }
        else DC.achievementProgress[14] = 0;

        if (orderCardCount[myIndex] == 9 && orderCardCount[myIndex] == winCardCount[myIndex])
        {
            DC.achievementProgress[15] += 1;
            DC.achievementProgress[16] += 1;
            DC.achievementProgress[17] += 1;
            DC.achievementProgress[18] += 1;
        }

        if (orderCardCount[myIndex] > winCardCount[myIndex])
        {
            if (winCardCount[myIndex] == 0) DC.achievementProgress[23] += 1;
            DC.achievementProgress[22] += 1;
        }
        DC.checkForAchievement();
    }

    void checkEndGameAchievemnt(int myPlace)
    {
        if (DC == null) return;
        if (playersScore[myIndex] > 7000)
        {
            DC.save.achievementLevel[20] += 1;
            DC.showAchieveWin(20);
        }
        if (myPlace == 0) 
        {
            DC.save.achievementProgress[0] += 1;

            DC.save.achievementProgress[8] += 1;
            DC.save.achievementProgress[9] += 1;
            DC.save.achievementProgress[10] += 1;

            if ( ( playersScore.Max() - playersScore.Min() ) * 2 > playersScore.Max() )
            {
                DC.save.achievementLevel[19] += 1;
                DC.showAchieveWin(19);
            }
        }
        else
        {
            DC.save.achievementProgress[8] = 0;
            DC.save.achievementProgress[9] = 0;
            DC.save.achievementProgress[10] = 0;
        }

        if (myPlace < 2)
        {
            DC.save.achievementProgress[1] += 1;
            DC.save.achievementProgress[2] += 1;
            DC.save.achievementProgress[3] += 1;
            DC.save.achievementProgress[4] += 1;

        }

        if (myPlace == 3)
        {
            DC.save.achievementProgress[24] += 1;
        }
        DC.checkForAchievement();
    }
    private IEnumerator YouLose(int myPlace)
    {
        loseScreen.SetActive(true);
        yield return new WaitForSeconds(5f);

        if (superTimer >= 200)
        {
            //IM.ShowRewards();
            yield return new WaitForSeconds(1f);
        }
        Network.leaveGame();
    }

//===================== Events ===============


    public void sendEvent1() // начинайте загрузку пацаны
    {
        RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.Others };

        // Texture2D texture = null;
        // object[] mas = null;

        // if (S.save.avatarPath != "")
        // {
        //     string path = S.save.avatarPath;
        //     texture = NativeGallery.LoadImageAtPath(path, 512, false);

        //     mas = new object[] {texture.EncodeToJPG(), texture.width, texture.height, texture.format, 
        //                                                 fromMainServer, "Jopa", S.save.Rating, S.save.ID};
        // }
        // else mas = new object[] {null, null, null, null, fromMainServer, "Jopa", S.save.Rating, S.save.ID};

        PhotonNetwork.RaiseEvent(1, null, options, sendOptions);
    }

    public void sendEvent2() // я загрузился, мастер
    {
        RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
        PhotonNetwork.RaiseEvent(2, null, options, sendOptions);
    }

    public void sendEvent3() // заебись можем начинать катку
    {
        RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(3, null, options, sendOptions);
    }
    public void sendEvent10(int playerNum, bool chosing, int firstSuit) // скажем челу что он ходит
    {
        RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(10, new object[] {chosing, playerNum, round, firstSuit}, options, sendOptions);
    }

    public void sendEvent11(Player player, bool chosing) // скажем челу что он не успел
    {
        RaiseEventOptions options = new RaiseEventOptions { TargetActors = new int[] {player.ActorNumber} };
        PhotonNetwork.RaiseEvent(11, chosing, options, sendOptions);
    }

    public void sendEvent20(Player player, bool chosing, int cardsCount, int jokerCount) // я выбрал число
    {
        RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
        PhotonNetwork.RaiseEvent(20, new object[] {myIndex, chosing, cardsCount, jokerCount}, options, sendOptions);
    }

    public void sendEvent31(List<int[]>[] cardsTable, int[] trump) // скажем всем о начале раунда
    {
        for (int i = 0; i < cardsTable.Length; i += 1)
        {
            int[] mas1 = cardsTable[i].Select(x => x[0]).ToArray();
            int[] mas2 = cardsTable[i].Select(x => x[1]).ToArray();

            RaiseEventOptions options = new RaiseEventOptions { TargetActors = new int[] {allPlayers[i].ActorNumber} };
            PhotonNetwork.RaiseEvent(31, new object[] {mas1, mas2, trump}, options, sendOptions);
        }
    }

    public void sendEvent32(int playerNum, bool chosing, int cardsCount) // скажем всем о выборе количества карт
    {
        RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(32, new object[] {playerNum, chosing, cardsCount}, options, sendOptions);
    }
    public void sendEvent32(int playerNum, bool chosing, int[] cardValue, int cardIndex, bool jokerCount) // скажем всем о выборе карты
    {
        // var sendList = allPlayers.Select(x => x.ActorNumber).ToList();
        // sendList.Remove(allPlayers[playerNum].ActorNumber);
        // RaiseEventOptions options = new RaiseEventOptions { TargetActors = sendList.ToArray() };
        RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(32, new object[] {playerNum, chosing, cardValue, cardIndex, jokerCount}, options, sendOptions);
    }
    public void sendEvent33(int playerNum, int count, int[] scores) // скажем всем кто победил
    {
        RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(33, new object[] {playerNum, count, scores}, options, sendOptions);
    }

    IEnumerator getWinnerIndexCoroutine(int playerNum, int count, int[] scores)
    {
        setPlayerReceived(playerNum, count, scores);
        yield return StartCoroutine(receiveCardsAnimate(playerNum));
        // clearThrowedCards();
        // clearOrderCounts();
    }


//===================== Event Callbacks =============


    public void OnEvent(EventData photonEvent)
    {
        switch (photonEvent.Code)
        {
            case 33: // о конец круга, кто то выиграл
            {
                object[] CustomData = (object[])photonEvent.CustomData;
                int playerNum = (int)CustomData[0];
                int count = (int)CustomData[1];
                int[] scores = (int[])CustomData[2];
                StartCoroutine(getWinnerIndexCoroutine(playerNum, count, scores));
                break;
            }

            case 32: // о кто то походил
            {
                object[] CustomData = (object[])photonEvent.CustomData;
                int playerNum = (int)CustomData[0];
                bool chosing = (bool)CustomData[1];

                if (chosing) setPlayerChoose(playerNum, (int)CustomData[2]);
                else setPlayerTurn(playerNum, (int[])CustomData[2], (int)CustomData[3], (bool)CustomData[4]);

                break;
            }

            case 31: // о раунд начался
            {
                object[] CustomData = (object[])photonEvent.CustomData;
                int[] mas1 = (int[])CustomData[0];
                int[] mas2 = (int[])CustomData[1];
                int[] trump = (int[])CustomData[2];
                StartCoroutine(Distribution(mas1, mas2, trump));
                break;
            }
            case 20: // епта челик походил, а я мастер
            {
                object[] CustomData = (object[])photonEvent.CustomData;
                int playerNum = (int)CustomData[0];
                bool chosing = (bool)CustomData[1];
                int count = (int)CustomData[2];
                int jokerCountTemp = (int)CustomData[3];


                Debug.Log("get count !!" + playerNum + " " + chosing + " " + count + " " + jokerCountTemp);
                if (chosing) lastChooseCardsCount = count;
                else
                {
                    lastChooseCardIndex = count;
                    jokerCount = jokerCountTemp;
                }
                waitCount += 1;
                break;
            }

            case 11: // сука мышка не зажимается
            {
                bool chosing = (bool)photonEvent.CustomData;
                if (chosing) CloseOrderMenu();
                else 
                {
                    jokerChoseSuit.SetActive(false);
                    jokerChoseUpDown.SetActive(false);
                    myTurn = false;
                    allHands[0].outLine.SetActive(false);
                }
                break;
            }

            case 10: // чей то ход, ебать
            {
                object[] CustomData = (object[])photonEvent.CustomData;
                bool chosing = (bool)CustomData[0];
                int playerNum = (int)CustomData[1];
                int roundNum = (int)CustomData[2];
                int lastOrderedSum = (int)CustomData[3];
                
                enableOutlineGlow(playerNum);

                if (playerNum == myIndex)
                {
                    if (chosing) OpenOrderMenu(roundNum, lastOrderedSum);
                    else
                    {
                        firstTurnSuit = (int)CustomData[3];
                        myTurn = true;
                    }
                }
                break;
            }

            case 1: // челик пиздит мои данные чтобы начать игру
            {
                // object[] CustomData = (object[])photonEvent.CustomData;

                // if (CustomData.Length != 0 && CustomData[0] != null)
                // {
                //     byte[] byteData = (byte[])CustomData[0];
                //     Debug.Log(byteData.Length);
                //     int textureWidth = (int)CustomData[1];
                //     int textureHeight = (int)CustomData[2];
                //     TextureFormat textureFormat = (TextureFormat)CustomData[3];

                //     Texture2D texCopy = new Texture2D(textureWidth, textureHeight, textureFormat, false);
                //     texCopy.LoadImage(byteData);
                // }

                // bool fromMainServer = (bool)CustomData[4];
                // string Nickname = (string)CustomData[5];
                // int score = (int)CustomData[6];
                // enemyID = (string)CustomData[7];

                // if (false && S.save.myFriends.Any(x => x.ID == enemyID)) addFriendButton.SetActive(false);

                // if (fromMainServer) sendEvent34(false);
                // else 
                // {
                //     StartGame();
                //     sendEvent101();
                // }

                sendEvent2();
                break;
            }
            case 2:
                waitCount += 1;
                break;

            case 3:
                StartGame();
                break;
        }
    }

    void OnApplicationPause(bool isPaused)
    {
        //IronSource.Agent.onApplicationPause(isPaused);
    }

    public void AddFriends()
    {
        // S.addToFriendsList(enemyID, secondNickname.text, Int32.Parse(secondScore.text));
        // addFriendButton.SetActive(false);
    }

}