using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using ExitGames.Client.Photon;
using UnityEngine.UI;
using Photon.Pun;

public class chatScr : MonoBehaviour, IChatClientListener
{
    [SerializeField] private Saver SV;
    [SerializeField] private RectTransform contentRect;
    [SerializeField] private GameObject textPrefabInactive;
    [SerializeField] private GameObject textPrefab;

    [SerializeField] private Button sendButton;
    [SerializeField] private InputField inputField;

    List<RectTransform> messagesList = new List<RectTransform>();
    ChatClient chatClient = null;

    public void sendPublicMessageButton()
    {
        var messageText = inputField.text;
        chatClient.PublishMessage("def", messageText);
        //addMessageToList(messageText);
    }

    public void addMessageToList(string text)
    {
        var messageInterval = 30f;

        var message = Instantiate(textPrefab, contentRect.transform);
        message.GetComponent<Text>().text = text;
        var rectComp = message.GetComponent<RectTransform>();

        if (messagesList.Count > 0)
        {
            var endPos = messagesList[messagesList.Count - 1].anchoredPosition + new Vector2(0, -messageInterval);
            rectComp.anchoredPosition = endPos;
        
            contentRect.sizeDelta += new Vector2(0, messageInterval * 2);
            foreach (var mess in messagesList) mess.anchoredPosition += new Vector2(0, messageInterval);
        }
        else message.transform.position = textPrefabInactive.transform.position;

        messagesList.Add(rectComp);

        contentRect.anchoredPosition += new Vector2(0, messageInterval * 2f);

    }

    void Start()
    {
        sendButton.interactable = false;

        chatClient = new ChatClient( this );
        chatClient.ChatRegion = "EU";
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, 
                            PhotonNetwork.AppVersion, 
                            new AuthenticationValues(SV.save.Nickname));

    }

    void Update()
    {
        chatClient.Service();
        //chatClient.TryGetChannel("def", out ChatChannel channel);
        //Debug.Log(channel);

    }

    public void DebugReturn(DebugLevel level, string message) { }
    public void OnChatStateChange(ChatState state) { Debug.Log(state); }
    public void OnConnected() { 
        chatClient.Subscribe(new string[] { "def" });
    }
    public void OnDisconnected() { Debug.Log("im Disconntected"); }
    public void OnGetMessages(string channelName, string[] senders, object[] messages) 
    {
        for (int i = 0; i < senders.Length; i++)
        {
            var messageText = string.Format("{0}: {1}", senders[i], messages[i]);
            addMessageToList(messageText);
        }
    }
    public void OnPrivateMessage(string sender, object message, string channelName) { }
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message) { }
    public void OnSubscribed(string[] channels, bool[] results) {
        sendButton.interactable = true;
    }

    public void OnUnsubscribed(string[] channels) { Debug.Log("im Unsubcribed"); }
    public void OnUserSubscribed(string channel, string user) { Debug.Log(user + " ubcribed"); }
    public void OnUserUnsubscribed(string channel, string user) { Debug.Log(user + " unsubcribed"); }
}
