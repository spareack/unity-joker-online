using Photon.Pun;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine;
using System;
using System.Linq;
using UnityEditor;

public class game : MonoBehaviourPunCallbacks, IOnEventCallback
{
    [SerializeField] public networkManager Network;

    [SerializeField] private bool whichTurn;
    [SerializeField] private bool readyForGameStart = false;

    [SerializeField] private GameObject BorderPrefab;
    [SerializeField] private border component;

    [SerializeField] public Border[,] bordersList;
    private int[] lastIndex = null;

    [SerializeField] private GameObject krestik;
    [SerializeField] private GameObject nolik;

    [SerializeField] private GameObject waitingMenu;
    [SerializeField] private GameObject gameMenu;
    [SerializeField] private GameObject loadingMenu;

    [SerializeField] private Text timesRemainText;
    [SerializeField] private Text whichTurnText;
    [SerializeField] private Text firstNickname;
    [SerializeField] private Text secondNickname;

    [SerializeField] private Image firstAvatar;
    [SerializeField] private Image secondAvatar;

    [SerializeField] private Coroutine calcTimeForTurn = null;

    [SerializeField] private int pointEarned = 0;

    private bool myTurn = false;

    private int height = 5;
    private int width = 7;

    public class Border
    {
        public GameObject obj;

        public Image imageComponent;
        public Button buttonComponent;

        public int[] borderIndex;

        public bool pushed;
        public int whoPushed;
        public bool edgeBorder;

        public game instance;

        public Border(GameObject obj, int[] index, game instance)
        {
            this.obj = obj;
            this.instance = instance;

            imageComponent = obj.GetComponent<Image>();
            buttonComponent = obj.GetComponent<Button>();

            buttonComponent.onClick.AddListener(() => instance.pushBorder(index));

            borderIndex = index;
            pushed = false;
            whoPushed = -1;

            // Func<int, int, bool> check = (y, x) => (y == 1 || y == instance.height * 2 - 1 || x == 0 || x == instance.width-2);
            edgeBorder = checkBorderEdge(index[0], index[1]);
            if (edgeBorder) imageComponent.color = Color.black;
        }

        private bool checkBorderEdge(int y, int x)
        {
            if (y % 2 == 0) return (x == 0 || x == instance.width-1);
            else return (y == 1 || y == instance.height * 2 - 1 );
        }
    }

    void Start()
    {
        if (PhotonNetwork.PlayerList.Length > 1) 
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

    void loadSavings()
    {
        if (PlayerPrefs.HasKey("avatarPath")) 
        {
            string path = PlayerPrefs.GetString("avatarPath");
            setAvatarImageTo(path, firstAvatar);
        }
        if (PlayerPrefs.HasKey("Nickname"))
        {
            string name = PlayerPrefs.GetString("Nickname");
            firstNickname.text = name;
        }
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
        sendEvent34(true);
        loadingMenu.SetActive(true);
        waitingMenu.SetActive(false);
        gameMenu.SetActive(false);
    }

    public void StartGame(bool myTurnValue)
    {
        gameMenu.SetActive(true);
        waitingMenu.SetActive(false);
        loadingMenu.SetActive(false);

        spawnLevel();
        myTurn = myTurnValue;
        calcTimeForTurn = StartCoroutine(calcTimesForTurn());
    }

    IEnumerator calcTimesForTurn()
    {
        if (myTurn) whichTurnText.text = "Твой ход";
        else whichTurnText.text = "Ход соперника";

        float duration = 10;
        for(float t = 0; t < duration; t += Time.deltaTime)
        {
            var s = ((int)(duration - t) >= 10 ? "0:" : "0:0");
            timesRemainText.text = s + (int)(duration - t);

            // if (PhotonNetwork.PlayerList.Length < 2) yield break;
            yield return null;
        }
        if (myTurn)
        {
            myTurn = false;
            if (lastIndex != null) bordersList[lastIndex[0], lastIndex[1]].imageComponent.color = Color.black;
            lastIndex = null;
            sendEvent15();
            calcTimeForTurn = StartCoroutine(calcTimesForTurn());
        }
    }

    void spawnLevel()
    {
        bordersList = new Border[height*2, width];
        
        for(int i = 0; i < height * 2; i += 1)
        {
            for(int j = 0; j < width; j += 1)
            {
                if (i % 2 == 0)
                {
                    if (i > 0)
                    {
                        var verticalBorder = Instantiate(BorderPrefab, transform);
                        verticalBorder.transform.position = new Vector3( j + 0.5f, i / 2, 0);
                        bordersList[i, j] = new Border(verticalBorder, new int[] {i, j}, this);
                    }
                }
                else
                {
                    if (j < width-1)
                    {
                        var horizontalBorder = Instantiate(BorderPrefab, transform);
                        horizontalBorder.transform.Rotate(0, 0, 90);
                        horizontalBorder.transform.position = new Vector3( j + 1f, (i + 1) / 2 - 0.5f, 0);
                        bordersList[i, j] = new Border(horizontalBorder, new int[] {i, j}, this);
                    }
                }
            }
        }
        transform.position += new Vector3(3f, 0, 0);
        transform.localScale *= 2f;
    }

    public void pushBorder(int[] indexList)
    {
        if (bordersList[indexList[0], indexList[1]].pushed || bordersList[indexList[0], indexList[1]].edgeBorder || !myTurn) return;
        if (lastIndex != null) bordersList[lastIndex[0], lastIndex[1]].imageComponent.color = Color.black;

        lastIndex = indexList;
        bordersList[indexList[0], indexList[1]].imageComponent.color = Color.red;
    }


    bool checkBorders(List<int[]> lst)
    {
        foreach(var z in lst)
        {
            var y = z[0];
            var x = z[1];

            if (0 <= y && y < height * 2 && 0 <= x && x < width && bordersList[y, x] != null)
            {
                if (!bordersList[y, x].pushed && !bordersList[y, x].edgeBorder)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void confirmBorder()
    {
        if (lastIndex == null || !myTurn) return;

        int getTurn = 0;

        var y = lastIndex[0];
        var x = lastIndex[1];
        var playerId = PhotonNetwork.LocalPlayer.ActorNumber;
    
        bordersList[y, x].pushed = true;
        bordersList[y, x].whoPushed = playerId;

        bordersList[y, x].imageComponent.color = Color.blue;

        // Func<int, bool> check = (y, x) => (0 <= y && y < height * 2 && 0 <= x && x < width && bordersList[y, x] != null && bordersList[y, x].pushed);
        List<int[]> posList = null;
        bool checkPushed = false;

        if (y % 2 != 0)
        {
            // int[,] nearBorderPos = new int[3,2] {{y-2, x}, {y-1, x}, {y-1, x+1}};
            // List<int[]> nearBorderPos = new List<int[]>() { new int[]{y-2, x}, new int[]{y-1, x}, new int[]{y-1, x+1} };
            // int[,] nearBorderPos = new int[3,2] {{y-2, x}, {y-1, x}, {y-1, x+1}};

            posList = new List<int[]>() { new int[]{y-2, x}, new int[]{y-1, x}, new int[]{y-1, x+1}};

            if (checkBorders(posList))
            {
                pointEarned += 1;
                getTurn += 1;
                GameObject obj = Instantiate(nolik, transform);
                var posX = (bordersList[y-1, x].obj.transform.localPosition.x + bordersList[y-1, x+1].obj.transform.localPosition.x) / 2f;
                var posY = (bordersList[y-2, x].obj.transform.localPosition.y + bordersList[y, x].obj.transform.localPosition.y) / 2f;
                obj.transform.localPosition = new Vector3(posX, posY, 0);
                obj.transform.localScale *= 2f;
                foreach(var pos in posList) bordersList[pos[0], pos[1]].imageComponent.color = Color.blue;
            }

// =======================

            posList = new List<int[]>() { new int[]{y+1, x}, new int[]{y+1, x+1}, new int[]{y+2, x}};

            if (checkBorders(posList))
            {
                pointEarned += 1;
                getTurn += 1;
                GameObject obj = Instantiate(nolik, transform);
                var posX = (bordersList[y+1, x].obj.transform.localPosition.x + bordersList[y+1, x+1].obj.transform.localPosition.x) / 2f;
                var posY = (bordersList[y+2, x].obj.transform.localPosition.y + bordersList[y, x].obj.transform.localPosition.y) / 2f;
                obj.transform.localPosition = new Vector3(posX, posY, 0);
                obj.transform.localScale *= 2f;
                foreach(var pos in posList) bordersList[pos[0], pos[1]].imageComponent.color = Color.blue;
            }
        }
        else
        {
            posList = new List<int[]>() { new int[]{y, x+1}, new int[]{y+1, x}, new int[]{y-1, x}};

            if (checkBorders(posList))
            {
                pointEarned += 1;
                getTurn += 1;
                GameObject obj = Instantiate(nolik, transform);
                var posX = (bordersList[y, x+1].obj.transform.localPosition.x + bordersList[y, x].obj.transform.localPosition.x) / 2f;
                var posY = (bordersList[y+1, x].obj.transform.localPosition.y + bordersList[y-1, x].obj.transform.localPosition.y) / 2f;
                obj.transform.localPosition = new Vector3(posX, posY, 0);
                obj.transform.localScale *= 2f;
                foreach(var pos in posList) bordersList[pos[0], pos[1]].imageComponent.color = Color.blue;
            }

// =======================

            posList = new List<int[]>() { new int[]{y, x-1}, new int[]{y-1, x-1}, new int[]{y+1, x-1}};

            if (checkBorders(posList))
            {
                pointEarned += 1;
                getTurn += 1;
                GameObject obj = Instantiate(nolik, transform);
                var posX = (bordersList[y, x-1].obj.transform.localPosition.x + bordersList[y, x].obj.transform.localPosition.x) / 2f;
                var posY = (bordersList[y-1, x-1].obj.transform.localPosition.y + bordersList[y+1, x-1].obj.transform.localPosition.y) / 2f;
                obj.transform.localPosition = new Vector3(posX, posY, 0);
                obj.transform.localScale *= 2f;
                foreach(var pos in posList) bordersList[pos[0], pos[1]].imageComponent.color = Color.blue;
            }
        }

        myTurn = (getTurn > 0 ? true : false);
        sendEvent42(lastIndex, getTurn);
        lastIndex = null;

        if (calcTimeForTurn != null) StopCoroutine(calcTimeForTurn);

        if (gameIsEnded()) endGameFunction();

        else calcTimeForTurn = StartCoroutine(calcTimesForTurn());
    }


    public void confirmBorderWithPlayerNumber(int[] playerData)
    {
        if (calcTimeForTurn != null) StopCoroutine(calcTimeForTurn);
        calcTimeForTurn = StartCoroutine(calcTimesForTurn());

        var y = playerData[0];
        var x = playerData[1];
        var playerId = playerData[2];

        if (bordersList[y, x].pushed || bordersList[y, x].edgeBorder) return;
    
        bordersList[y, x].pushed = true;
        bordersList[y, x].whoPushed = playerId;

        bordersList[y, x].imageComponent.color = Color.green;

        // Func<int, bool> check = (y, x) => (0 <= y && y < height * 2 && 0 <= x && x < width && bordersList[y, x] != null && bordersList[y, x].pushed);
        List<int[]> posList = null;
        bool checkPushed = false;

        if (y % 2 != 0)
        {
            // int[,] nearBorderPos = new int[3,2] {{y-2, x}, {y-1, x}, {y-1, x+1}};
            // List<int[]> nearBorderPos = new List<int[]>() { new int[]{y-2, x}, new int[]{y-1, x}, new int[]{y-1, x+1} };
            // int[,] nearBorderPos = new int[3,2] {{y-2, x}, {y-1, x}, {y-1, x+1}};

            posList = new List<int[]>() { new int[]{y-2, x}, new int[]{y-1, x}, new int[]{y-1, x+1}};

            if (checkBorders(posList))
            {
                pointEarned -= 1;
                GameObject obj = Instantiate(krestik, transform);
                var posX = (bordersList[y-1, x].obj.transform.localPosition.x + bordersList[y-1, x+1].obj.transform.localPosition.x) / 2f;
                var posY = (bordersList[y-2, x].obj.transform.localPosition.y + bordersList[y, x].obj.transform.localPosition.y) / 2f;
                obj.transform.localPosition = new Vector3(posX, posY, 0);
                obj.transform.localScale *= 2f;
                foreach(var pos in posList) bordersList[pos[0], pos[1]].imageComponent.color = Color.green;
            }

// =======================

            posList = new List<int[]>() { new int[]{y+1, x}, new int[]{y+1, x+1}, new int[]{y+2, x}};

            if (checkBorders(posList))
            {
                pointEarned -= 1;
                GameObject obj = Instantiate(krestik, transform);
                var posX = (bordersList[y+1, x].obj.transform.localPosition.x + bordersList[y+1, x+1].obj.transform.localPosition.x) / 2f;
                var posY = (bordersList[y+2, x].obj.transform.localPosition.y + bordersList[y, x].obj.transform.localPosition.y) / 2f;
                obj.transform.localPosition = new Vector3(posX, posY, 0);
                obj.transform.localScale *= 2f;
                foreach(var pos in posList) bordersList[pos[0], pos[1]].imageComponent.color = Color.green;
            }
        }
        else
        {
            posList = new List<int[]>() { new int[]{y, x+1}, new int[]{y+1, x}, new int[]{y-1, x}};

            if (checkBorders(posList))
            {
                pointEarned -= 1;
                GameObject obj = Instantiate(krestik, transform);
                var posX = (bordersList[y, x+1].obj.transform.localPosition.x + bordersList[y, x].obj.transform.localPosition.x) / 2f;
                var posY = (bordersList[y+1, x].obj.transform.localPosition.y + bordersList[y-1, x].obj.transform.localPosition.y) / 2f;
                obj.transform.localPosition = new Vector3(posX, posY, 0);
                obj.transform.localScale *= 2f;
                foreach(var pos in posList) bordersList[pos[0], pos[1]].imageComponent.color = Color.green;
            }

// =======================

            posList = new List<int[]>() { new int[]{y, x-1}, new int[]{y-1, x-1}, new int[]{y+1, x-1}};

            if (checkBorders(posList))
            {
                pointEarned -= 1;
                GameObject obj = Instantiate(krestik, transform);
                var posX = (bordersList[y, x-1].obj.transform.localPosition.x + bordersList[y, x].obj.transform.localPosition.x) / 2f;
                var posY = (bordersList[y-1, x-1].obj.transform.localPosition.y + bordersList[y+1, x-1].obj.transform.localPosition.y) / 2f;
                obj.transform.localPosition = new Vector3(posX, posY, 0);
                obj.transform.localScale *= 2f;
                foreach(var pos in posList) bordersList[pos[0], pos[1]].imageComponent.color = Color.green;
            }
        }
        if (gameIsEnded()) endGameFunction();
    }

    private bool gameIsEnded()
    {
        for(int y = 0; y < height * 2; y += 1)
        {
            for(int x = 0; x < width; x += 1)
            {
                if (bordersList[y, x] != null)
                {
                    if (!bordersList[y, x].pushed && !bordersList[y, x].edgeBorder)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    private void endGameFunction()
    {
        // Network.leaveGame();
        
        // if (pointEarned > 0) winGame();
        // else if (pointEarned < 0) loseGame();
        // else drawGame();
    }

    public void sendEvent42(int[] mas, int getTurn)
    {
        RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        var message = new int[] { mas[0], mas[1], PhotonNetwork.LocalPlayer.ActorNumber, getTurn};
        PhotonNetwork.RaiseEvent(42, message, options, sendOptions);
    }
    public void sendEvent15()
    {
        RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(15, null, options, sendOptions);
    }
    public void sendEvent34(bool fromMainServer)
    {
        Debug.Log("sendEvent34Response");
        RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        Texture2D texture = null;
        object[] mas = null;

        if (PlayerPrefs.HasKey("avatarPath")) 
        {
            string path = PlayerPrefs.GetString("avatarPath");
            texture = NativeGallery.LoadImageAtPath(path, 512, false);
            // SetTextureImporterFormat(texture, true);

            mas = new object[] {texture.EncodeToJPG(), texture.width, texture.height, texture.format, fromMainServer, firstNickname.text};
            // Debug.Log(texture.GetRawTextureData().Length);
            Debug.Log(texture.EncodeToPNG().Length);
            Debug.Log(texture.EncodeToJPG().Length);
        }
        PhotonNetwork.RaiseEvent(34, mas, options, sendOptions);
        Debug.Log("event end");
    }
    public void sendEvent100()
    {
        RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(100, null, options, sendOptions);
    }



    // public static void SetTextureImporterFormat( Texture2D texture, bool isReadable)
    // {
    //     if ( null == texture ) return;

    //     string assetPath = AssetDatabase.GetAssetPath( texture );
    //     var tImporter = AssetImporter.GetAtPath( assetPath ) as TextureImporter;
    //     if ( tImporter != null )
    //     {
    //         tImporter.textureType = TextureImporterType.Default;

    //         tImporter.isReadable = isReadable;

    //         AssetDatabase.ImportAsset( assetPath );
    //         AssetDatabase.Refresh();
    //     }
    // }

    public void OnEvent(EventData photonEvent)
    {
        // Debug.Log(photonEvent.ToStringFull());
        switch (photonEvent.Code)
        {
            case 42:
                int[] mas = (int[]) photonEvent.CustomData;
                myTurn = (mas[3] > 0 ? false : true);
                confirmBorderWithPlayerNumber(mas);
                break;

            case 15:
                myTurn = true;
                if (calcTimeForTurn != null) StopCoroutine(calcTimeForTurn);
                calcTimeForTurn = StartCoroutine(calcTimesForTurn());
                break;

            case 34:
                Debug.Log("34");
                object[] CustomData = (object[])photonEvent.CustomData;

                byte[] byteData = (byte[])CustomData[0]; // вот тут ошибка!
                Debug.Log(byteData.Length);
                int textureWidth = (int)CustomData[1];
                int textureHeight = (int)CustomData[2];
                TextureFormat textureFormat = (TextureFormat)CustomData[3];
                bool fromMainServer = (bool)CustomData[4];
                string Nickname = (string)CustomData[5];

                secondNickname.text = Nickname;

                if (byteData.Length != 0) 
                {
                    Texture2D texCopy = new Texture2D(textureWidth, textureHeight, textureFormat, false);
                    // texCopy.LoadRawTextureData(byteData);
                    texCopy.LoadImage(byteData);
                    // texCopy.Apply();
                    secondAvatar.sprite = Sprite.Create(texCopy, new Rect(0, 0, texCopy.width, texCopy.height), Vector2.zero);
                }
                
                if (fromMainServer) sendEvent34(false);
                else 
                {
                    StartGame(true);
                    sendEvent100();
                }
                break;

            case 100:
                StartGame(false);
                break;


            // case 35:
            //     object[] CustomData = (object[])photonEvent.CustomData;

            //     byte[] byteData = (byte[])CustomData[0];
            //     Debug.Log(byteData.Length);
            //     int textureWidth = (int)CustomData[1];
            //     int textureHeight = (int)CustomData[2];
            //     TextureFormat textureFormat = (TextureFormat)CustomData[3];
            //     bool fromMainServer = (bool)CustomData[4];

            //     if (byteData.Length != 0) 
            //     {
            //         Texture2D texCopy = new Texture2D(textureWidth, textureHeight, textureFormat, false);
            //         // texCopy.LoadRawTextureData(byteData);
            //         texCopy.LoadImage(byteData);
            //         // texCopy.Apply();
            //         secondAvatar.sprite = Sprite.Create(texCopy, new Rect(0, 0, texCopy.width, texCopy.height), Vector2.zero);
            //     }
            //     if (fromMainServer) sendEvent34(false);
            //     break;
            // case 37:
            //     object[] CustomData = (object[])photonEvent.CustomData;

            //     byte[] byteData = (byte[])CustomData[0];
            //     Debug.Log(byteData.Length);
            //     int textureWidth = (int)CustomData[1];
            //     int textureHeight = (int)CustomData[2];
            //     TextureFormat textureFormat = (TextureFormat)CustomData[3];
            //     bool fromMainServer = (bool)CustomData[4];

            //     if (byteData.Length != 0) 
            //     {
            //         Texture2D texCopy = new Texture2D(textureWidth, textureHeight, textureFormat, false);
            //         // texCopy.LoadRawTextureData(byteData);
            //         texCopy.LoadImage(byteData);
            //         // texCopy.Apply();
            //         secondAvatar.sprite = Sprite.Create(texCopy, new Rect(0, 0, texCopy.width, texCopy.height), Vector2.zero);
            //     }
            //     if (fromMainServer) sendEvent34(false);
            //     break;
        }
    }
}