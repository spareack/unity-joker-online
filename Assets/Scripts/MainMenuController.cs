using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Animator profileAnim;

    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject topListMenu;
    [SerializeField] private GameObject createClanMenu;
    [SerializeField] private GameObject claneMenu;
    [SerializeField] private GameObject rigistrMenu;

    [SerializeField] private InputField clanNameInput;
    [SerializeField] private InputField clanDiscriptionInput;

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
        if (DC.save.youSaveNameCheck == 1)
        {
            rigistrMenu.SetActive(true);
        }
        StartCoroutine(getRating()); 
    }

    public IEnumerator joinClan(string actor_num, int clan_id)
    {
        // actor_num = айди пользователя
        var request = new ServerSync.JoinClan();
        yield return StartCoroutine(request.JoinClanCoroutine(actor_num, clan_id));
        if (request.statusCheck != null && request.statusCheck.status == 0) Debug.Log("good");
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
        else Debug.Log("error");
    }
    public IEnumerator getClan(int clan_id)
    {
        var request = new ServerSync.GetClan();
        yield return StartCoroutine(request.GetClanCoroutine(clan_id));
        if (request.response != null && request.response.status == 0)
        {
            Debug.Log("good " + request.response.clan_name); // request.response - данные клана

            ClanMenuScr.clanNameText.text = request.response.clan_name;
            ClanMenuScr.clanDiscriptionText.text = request.response.description;
            //request.response.clan_name
            //request.response.description
            //request.response.clan_score
            //request.response.messages_data - не работает

            ClanMenuScr.panCount1 = request.response.members.Count;

            foreach (var player in request.response.members)
            {
                ClanMenuScr.allNames.Add(player.name);
                ClanMenuScr.allNames.Add("" + player.score);
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
            Debug.Log("good"); // request.response - топ челов
            listTopPlayersScr.panCount1 = request.response.players.Count;
            foreach (var player in request.response.players)
            {
                listTopPlayersScr.allNames.Add(player.name);
                listTopPlayersScr.allScore.Add("" + player.score);
                // Переменные для игрокорвов
                //player.actor_num
                //player.name
                //player.clan_id
                //player.score
            }
        }
        else Debug.Log("error");
    }
    public IEnumerator registerNewPlayer(string name, string actor_num)
    {
        var request = new ServerSync.RegisterNewPlayer();
        yield return StartCoroutine(request.RegisterNewPlayerCoroutine(name, actor_num));
        if (request.response.status == 0) Debug.Log("good"); // good
        else Debug.Log("error");
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

    public void CreateClaneButton()
    {
        if (clanNameInput.text != "" && clanDiscriptionInput.text != "")
        {
            StartCoroutine(createClan(S.save.Nickname, clanNameInput.text, clanDiscriptionInput.text));
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
            settingsMenu.SetActive(false);
        }
    }

    private IEnumerator FirstOpen()
    {
        settingsMenu.SetActive(true);
        yield return null;
        settingsMenu.SetActive(false);
    }
}
