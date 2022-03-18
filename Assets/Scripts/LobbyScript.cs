using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System.Linq;


public class LobbyScript : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button createRoomButton;
    [SerializeField] private Button joinRoomButton;

    [SerializeField] private InputField createInput;
    [SerializeField] private InputField joinInput;
    [SerializeField] private InputField NicknameInput;

    [SerializeField] public Image LocalProfileImage;
    [SerializeField] public Sprite defaultTexture;

    [SerializeField] public Text personalScore;
    [SerializeField] private Saver S;

    [SerializeField] private GameObject ScrollContent;
    [SerializeField] private GameObject friendPrefab;

    [SerializeField] private List<GameObject> friendsStrings = new List<GameObject>();

    void OnApplicationPause(bool isPaused)
    {
        //IronSource.Agent.onApplicationPause(isPaused);
    }

    void Start()
    {
        if (PhotonNetwork.NetworkClientState == ClientState.PeerCreated)
        {
            // S.save.avatarPath = "E:/FrendsPhoto/����/EE1.jpg";
            // S.saveChanges();

            PhotonNetwork.NickName = S.save.Nickname;
            PhotonNetwork.AuthValues = new AuthenticationValues(S.save.ID);
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.GameVersion = "1";
            PhotonNetwork.ConnectUsingSettings();
            loadSavings();

            StartCoroutine(refreshFriendsListCoroutine());
            StartCoroutine(reconnectingCoroutine());
        }
    }

    IEnumerator refreshFriendsListCoroutine()
    {
        while(!PhotonNetwork.IsConnectedAndReady) yield return null;
        refreshFriendsList();

        yield return new WaitForSeconds(5f);
        StartCoroutine(refreshFriendsListCoroutine());
    }

    public void refreshFriendsList()
    {
        string[] friendsIDs = S.save.myFriends.Select(x => x.ID).ToArray();

        if (friendsIDs.Length > 0) PhotonNetwork.FindFriends(friendsIDs);
    }

    public void StartSinglePlayer()
    {
        SceneManager.LoadScene(2);
    }

    public override void OnFriendListUpdate(List<FriendInfo> friendsInfo)
    {
        refreshScrollPanel(friendsInfo);
    }

    private void refreshScrollPanel(List<FriendInfo> friendsInfo)
    {
        var content = ScrollContent.GetComponent<RectTransform>();
        content.sizeDelta = new Vector2(content.sizeDelta.x, friendsInfo.Count * 120);

        foreach(var x in friendsStrings) Destroy(x);
        friendsStrings.Clear();

        for(int i = 0; i < friendsInfo.Count; i += 1)
        {
            GameObject friend = Instantiate(friendPrefab, ScrollContent.transform);

            string name = S.save.myFriends.Where(x => x.ID == friendsInfo[i].UserId).Select(x => x.Nickname).First();

            string text = "";
            // text = friendsInfo[i].IsOnline ? (friendsInfo[i].IsInRoom ? "Online in room " + friend.Room : "Online") : "Offline";

            if (friendsInfo[i].IsOnline)
            {
                if (friendsInfo[i].IsInRoom) text = name + " - " + "Online in Room " + friendsInfo[i].Room;
                else text = name + " - " + "Online in Menu";
            }
            else text = name + " - " +  "Offline";

            friend.GetComponent<Text>().text = text;
            friend.transform.localPosition = new Vector3(500, -100 - i*100, 0);
            friendsStrings.Add(friend);

            // friend.Name 
            // friend.UserId
            // friend.Room
            // friend.IsInRoom
        }
    }


    void Update()
    {
        // Debug.Log(PhotonNetwork.NetworkClientState);
        // Debug.Log(PhotonNetwork.IsConnectedAndReady);

        if (PhotonNetwork.NetworkClientState == ClientState.Disconnected) StartCoroutine(reconnectingCoroutine());
        // Debug.Log(PhotonNetwork.NetworkClientState);

    }

    IEnumerator reconnectingCoroutine()
    {
        PhotonNetwork.Reconnect();

        createRoomButton.interactable = false;
        joinRoomButton.interactable = false;

        while(!PhotonNetwork.IsConnectedAndReady) yield return null;

        createRoomButton.interactable = true;
        joinRoomButton.interactable = true;
    }

    IEnumerator waitJoinOrCreateButtons()
    {
        createRoomButton.interactable = false;
        joinRoomButton.interactable = false;

        while(!PhotonNetwork.IsConnectedAndReady) yield return null;

        createRoomButton.interactable = true;
        joinRoomButton.interactable = true;
    }

    void loadSavings()
    {
        if (S.save.avatarPath != "") setAvatarImage(S.save.avatarPath);
        if (S.save.Nickname != "Nickname") NicknameInput.text = S.save.Nickname;
        personalScore.text = "" + S.save.Rating;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.NickName + " connected to Master");
        createRoomButton.interactable = true;
        joinRoomButton.interactable = true;
    }

    public void createRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        PhotonNetwork.CreateRoom(createInput.text, roomOptions);
        StartCoroutine(waitJoinOrCreateButtons());
    }

    public void joinRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.JoinRoom(joinInput.text);
        StartCoroutine(waitJoinOrCreateButtons());
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.NickName + " joined to Room with " + joinInput + " pass");
        SceneManager.LoadScene(1);
    }

    public void ShowMediaPicker()
    {
        if (Application.isEditor)
        {
            // Do something else, since the plugin does not work inside the editor
        }
        else
        {
            NativeGallery.GetImageFromGallery((path) =>
            {
                if (setAvatarImage(path) == 0) saveNewAvatarPath(path);
            });
        }
    }

    int setAvatarImage(string path)
    {
        Texture2D texture = NativeGallery.LoadImageAtPath(path);
        if (texture == null) return 1;
        LocalProfileImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        return 0;
    }

    void saveNewAvatarPath(string path)
    {
        S.save.avatarPath = path;
        S.saveChanges();
    }
    public void saveNewNickname()
    {
        S.save.Nickname = NicknameInput.text;
        S.saveChanges();
        PhotonNetwork.NickName = S.save.Nickname;
    }
    public void removeAvatar()
    {
        S.save.avatarPath = "";
        S.saveChanges();
        LocalProfileImage.sprite = defaultTexture;
    }
}
