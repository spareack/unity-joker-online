using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using ExitGames.Client.Photon;
using UnityEngine.UI;

public class chatScr : MonoBehaviour, IChatClientListener
{
    [SerializeField] private Saver SV;
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject textPrefabInactive;
    [SerializeField] private GameObject textPrefab;

    List<GameObject> messagesList = new List<GameObject>();

    public void addMessageToList(string text)
    {
        var message = Instantiate(textPrefab, content.transform);
        message.GetComponent<Text>().text = text;
        if (messagesList.Count > 0) message.transform.position = messagesList[messagesList.Count - 1].transform.position;
        else message.transform.position = textPrefabInactive.transform.position;
    }

    void Start()
    {
         var chatClient = new ChatClient( this );
         chatClient.ChatRegion = "EU";
         chatClient.Connect("def", "1", new AuthenticationValues(SV.save.ID));
         chatClient.Service();

         chatClient.Subscribe(new string[] { "def" });
         chatClient.PublishMessage("def", "im connected, My ID is " + SV.save.ID + " !");

    }

    public void DebugReturn(DebugLevel level, string message) { }
    public void OnChatStateChange(ChatState state) { }
    public void OnConnected() { }
    public void OnDisconnected() { }
    public void OnGetMessages(string channelName, string[] senders, object[] messages) 
    {
        string msgs = "";
        for (int i = 0; i < senders.Length; i++)
        {
            addMessageToList((string)messages[i]);
            //msgs = string.Format("{0}{1}={2}, ", msgs, senders[i], messages[i]);
        }
    }
    public void OnPrivateMessage(string sender, object message, string channelName) { }
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message) { }
    public void OnSubscribed(string[] channels, bool[] results) { }
    public void OnUnsubscribed(string[] channels) { }
    public void OnUserSubscribed(string channel, string user) { }
    public void OnUserUnsubscribed(string channel, string user) { }
}
