using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Animator profileAnim;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private ServerSync serverSync;
    
    private int state = 1;

    private void Awake()
    {
        StartCoroutine(FirstOpen());
    }

    private void Start()
    {
        //profileAnim.Play("right");

        //StartCoroutine(getClan(1)); 
    }

    public IEnumerator joinClan(string actor_num, int clan_id)
    {
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
            //request.response.clan_id 
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


            //request.response.clan_name
            //request.response.description
            //request.response.clan_score
            //request.response.messages_data

            foreach (var player in request.response.members)
            {
                //player.name
                //player.actor_num
                //player.score
            }
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

            foreach (var player in request.response.players)
            {
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
    
    public void OpenOrCloseSettings(int num)
    {
        if (num == 1) settingsMenu.SetActive(true);
        else if (num == 0) settingsMenu.SetActive(false);
    }

    private IEnumerator FirstOpen()
    {
        settingsMenu.SetActive(true);
        yield return null;
        settingsMenu.SetActive(false);
    }
}
