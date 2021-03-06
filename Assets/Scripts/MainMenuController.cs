using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Animator profileAnim;

    [SerializeField] private Text yourNameText;
    [SerializeField] private Text yourRankText;
    [SerializeField] private Text[] yourMMRText;
    [SerializeField] private Text[] allGamesPlayedText;
    [SerializeField] private Text wonGamesText;
    [SerializeField] private Text gamesWithFirstPlaceText;
    [SerializeField] private Text placeInWorldRaitingText;

    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject topListMenu;
    [SerializeField] private GameObject joinClanMenu;
    [SerializeField] private GameObject createClanMenu;
    [SerializeField] private GameObject claneMenu;
    [SerializeField] public GameObject rigistrMenu;

    [SerializeField] private InputField clanNameInput;
    [SerializeField] private InputField clanDiscriptionInput;

    [SerializeField] private InputField clanIdInput;

    [SerializeField] private ServerSync serverSync;

    [SerializeField] private ListTopPlayers listTopPlayersScr;
    [SerializeField] private ClanMenu ClanMenuScr;

    [SerializeField] private DataCheck DC;
    [SerializeField] private Saver S;
    
    private int state = 1;

    private void Awake()
    {
        StartCoroutine(FirstOpen());
        
    }

    private void Start()
    {
        //profileAnim.Play("right");
        if (DC.save.youSaveNameCheck == 0)
        {
            rigistrMenu.SetActive(true);
        }
        StartCoroutine(getRating());
        Debug.Log(S.save.Nickname);
    }

    public IEnumerator joinClan(string actor_num, int clan_id)
    {
        // actor_num = ???? ????????????
        var request = new ServerSync.JoinClan();
        yield return StartCoroutine(request.JoinClanCoroutine(actor_num, clan_id));
        if (request.response != null && request.response.status == 0) Debug.Log("good");
        else Debug.Log("error");
    }
    public IEnumerator createClan(string actor_num, string name, string description)
    {
        var request = new ServerSync.CreateClan();
        yield return StartCoroutine(request.CreateClanCoroutine(actor_num, name, description));
        if (request.response != null && request.response.status == 0)
        {
            Debug.Log("good");
            DC.save.yourClaneID = request.response.clan_id;
            DC.saveChanges();
        }
        else Debug.Log("error " + request.response.info);
    }
    public IEnumerator getClan(int clan_id)
    {
        var request = new ServerSync.GetClan();
        yield return StartCoroutine(request.GetClanCoroutine(clan_id));
        if (request.response != null && request.response.status == 0)
        {
            Debug.Log("good " + request.response.clan_name); // request.response - ?????? ?????

            ClanMenuScr.clanNameText.text = request.response.clan_name + " | " + clan_id;
            ClanMenuScr.clanDiscriptionText.text = request.response.description;
            //request.response.clan_name
            //request.response.description
            //request.response.clan_score
            //request.response.messages_data - ?? ????????

            ClanMenuScr.panCount1 = request.response.members.Count;

            foreach (var player in request.response.members)
            {
                ClanMenuScr.allNames.Add(player.name);
                ClanMenuScr.allScore.Add("" + player.score);
                //player.name
                //player.actor_num
                //player.score
            }
            ClanMenuScr.SpawnScroll1();
        }
        else Debug.Log("error");
    }

    public IEnumerator getRating()
    {
        var request = new ServerSync.GetRating();
        yield return StartCoroutine(request.GetRatingCoroutine());
        if (request.response != null && request.response.status == 0)
        {
            Debug.Log("good"); // request.response - ??? ?????
            listTopPlayersScr.panCount1 = request.response.players.Count;

            int count = 0;

            foreach (var player in request.response.players)
            {
                listTopPlayersScr.allNames.Add(player.name);
                listTopPlayersScr.allScore.Add("" + player.score);
                count++;
                Debug.Log(player.name);
                if (S.save.Nickname == player.name)
                {
                    DataRaitingReWrite(player.score, count);
                }
                // ?????????? ??? ??????????
                //player.actor_num
                //player.name
                //player.clan_id
                //player.score
            }
        }
        else Debug.Log("error " + request.response.info);
    }
    public IEnumerator registerNewPlayer(string name, string actor_num)
    {
        var request = new ServerSync.RegisterNewPlayer();
        yield return StartCoroutine(request.RegisterNewPlayerCoroutine(name, actor_num));
        if (request.response.status == 0) Debug.Log("good reg"); // good
        else Debug.Log("error reg" + request.response.info);
    }
    public IEnumerator changeScore(List<ServerSync.ChangeScore.Players> players)
    {
        var request = new ServerSync.ChangeScore();
        yield return StartCoroutine(request.changeScoreCoroutine(players));
        if (request.response.status == 0) Debug.Log("good"); // good
        else Debug.Log("error");
    }


    public void OpenOrColeProfile()
    {
        if (state == 0) 
        {
            state = 1;
            profileAnim.Play("left");
        }
        else if (state == 1)
        {
            state = 0;
            profileAnim.Play("right");
        }
    }
    public void OpenOrCloseJoinClan(int num)
    {
        if (num == 1) joinClanMenu.SetActive(true);
        else if (num == 0) joinClanMenu.SetActive(false);
    }

    public void OpenOrCloseTopList(int num)
    {
        if (num == 1) topListMenu.SetActive(true);
        else if (num == 0) topListMenu.SetActive(false);
    }
    
    public void OpenOrCloseSettings(int num)
    {
        if (num == 1) settingsMenu.SetActive(true);
        else if (num == 0) settingsMenu.SetActive(false);
    }

    public void OpenOrCloseCreateClan(int num)
    {
        if (num == 1) createClanMenu.SetActive(true);
        else if (num == 0) createClanMenu.SetActive(false);
    }
    public void JoinClanButton()
    {
        if (clanIdInput.text != "")
        {
            StartCoroutine(joinClan(S.save.ID, Int32.Parse(clanIdInput.text)));
            joinClanMenu.SetActive(false);
        }
        DC.save.yourClaneID = Int32.Parse(clanIdInput.text);
        DC.saveChanges();
    }

    public void CreateClaneButton()
    {
        if (clanNameInput.text != "" && clanDiscriptionInput.text != "")
        {
            StartCoroutine(createClan(S.save.ID, clanNameInput.text, clanDiscriptionInput.text));
        }
    }

    public void OpenOrCloseClaneMenu(int num)
    {
        if (num == 1)
        {
            Debug.Log(1);
            claneMenu.SetActive(true);
            StartCoroutine(getClan(DC.save.yourClaneID));
        }
        else if (num == 0) 
        {
            claneMenu.SetActive(false);
            ClanMenuScr.ClearMembersPanel();
        }
    }

    private IEnumerator FirstOpen()
    {
        settingsMenu.SetActive(true);
        yield return null;
        settingsMenu.SetActive(false);
    }

    private void DataRaitingReWrite(int yourMMR, int placeInWorldRaiting)
    {
        DC.save.yourMMR = yourMMR;
        if (DC.save.yourMMR >= Int32.Parse(LanguageSystem.lng.ranksHowManyPoints[DC.save.yourRank]))
        {
            DC.save.yourRank++;
        }
        //DC.save.allGamesPlayed;
        //DC.save.wonGames;
        //DC.save.gamesWithFirstPlace;
        DC.save.placeInWorldRaiting = placeInWorldRaiting;

        DataProfileReWrite();
    }

    private void DataProfileReWrite()
    {
        yourNameText.text = S.save.Nickname;
        yourRankText.text = LanguageSystem.lng.ranks[DC.save.yourRank];
        for (int i = 0; i < yourMMRText.Length; i++) yourMMRText[i].text = "" + DC.save.yourMMR;
        for (int i = 0; i < allGamesPlayedText.Length; i++) allGamesPlayedText[i].text = "" + DC.save.allGamesPlayed;
        wonGamesText.text = "" + DC.save.wonGames;
        gamesWithFirstPlaceText.text = "" + DC.save.gamesWithFirstPlace;
        placeInWorldRaitingText.text = "" + DC.save.placeInWorldRaiting;
    }
}
