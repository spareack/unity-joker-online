using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Saver : MonoBehaviour
{
    public SuperSave save = null;

    private void Awake()
    {
        // PlayerPrefs.DeleteAll();

        if (PlayerPrefs.HasKey("SuperSave")) save = JsonUtility.FromJson<SuperSave>(PlayerPrefs.GetString("SuperSave"));
        else
        {
            string id = "" + UnityEngine.Random.Range(100000, 999999) + UnityEngine.Random.Range(100000, 999999);
            save = new SuperSave(id);
            saveChanges();
        }

        // for_test();
    }

    void for_test() // можешь затестить
    {
        // if (PlayerPrefs.HasKey("SuperSave")) save = JsonUtility.FromJson<SuperSave>(PlayerPrefs.GetString("SuperSave"));
        // save.myFriends.Add(new SuperSave.MyFriend("636289305481", "Niknext", 150));
        // save.myFriends.Add(new SuperSave.MyFriend("12345", "Vanya", 50));
        // PlayerPrefs.SetString("SuperSave", JsonUtility.ToJson(save));
        // PlayerPrefs.Save();

        // SuperSave instance = JsonUtility.FromJson<SuperSave>(PlayerPrefs.GetString("SuperSave"));
        // Debug.Log(instance);
        // Debug.Log(instance.ID);
        // Debug.Log(instance.Nickname);
        // Debug.Log(instance.myFriends);
        // Debug.Log(instance.myFriends.Count);
        // Debug.Log(instance.myFriends[0]);
        // Debug.Log(instance.myFriends[0].ID);
        // Debug.Log(instance.myFriends[0].Nickname);
        // Debug.Log(instance.myFriends[0].Rating);
    }

    public void saveChanges()
    {
        PlayerPrefs.SetString("SuperSave", JsonUtility.ToJson(save));
        PlayerPrefs.Save();
    }

    public void addToFriendsList(string ID, string Nickname, int Rating)
    {
        if (save.myFriends.Any(x => x.ID == ID)) return;
        
        save.myFriends.Add(new SuperSave.MyFriend(ID, Nickname, Rating));
        saveChanges();
    }
}

[Serializable]
public class SuperSave
{
    public string ID;
    public string Nickname;
    public int Rating;
    public string avatarPath;
    public List<MyFriend> myFriends;

    public SuperSave(string ID)
    {
        this.ID = ID;
        Nickname = "Unknown";
        Rating = 0;
        avatarPath = "";
        myFriends = new List<MyFriend>();
    }

    [Serializable]
    public class MyFriend
    {
        public string ID;
        public string Nickname;
        public int Rating;

        public MyFriend(string ID, string Nickname, int Rating)
        {
            this.ID = ID;
            this.Nickname = Nickname;
            this.Rating = Rating;
        }
    }
}
