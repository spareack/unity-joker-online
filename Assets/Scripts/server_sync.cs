using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class server_sync : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(changeScoreCoroutine());
    }

    public void registerNewPlayerOnServer(string name, string actor_num)
    {
        StartCoroutine(registerNewPlayerCoroutine("https://dumka.pythonanywhere.com/register_new_player", name, actor_num));
    }

    IEnumerator get_rating()
    {
        UnityWebRequest uwr = UnityWebRequest.Get("https://dumka.pythonanywhere.com/get_rating");
        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError) Debug.Log("Error While Sending: " + uwr.error);
        else
        {
            var info_list = JsonUtility.FromJson<InfoList>(uwr.downloadHandler.text);

            if (info_list != null && info_list.status == 0)
            {
                Debug.Log("current status: " + info_list.status);
                Debug.Log("1 player: " + info_list.players[0].name + " " + info_list.players[0].score);
                Debug.Log("2 player: " + info_list.players[1].name + " " + info_list.players[1].score);
            }
            else Debug.Log("error post registerNewPlayer");
        }
    }

    IEnumerator registerNewPlayerCoroutine(string url, string name, string actor_num)
    {
        var uwr = new UnityWebRequest(url, "POST");

        var data = new New_player { name = name, actor_num = actor_num };
        var json_data = JsonUtility.ToJson(data);
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json_data);

        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");

        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError) Debug.Log("Error While Sending: " + uwr.error);
        else
        {
            var status_check = JsonUtility.FromJson<Status_check>(uwr.downloadHandler.text);
            if (status_check != null && status_check.status == 0) Debug.Log("succesfull post registerNewPlayer");
            else Debug.Log("error post registerNewPlayer");
        }
    }

    IEnumerator changeScoreCoroutine()
    {
        var uwr = new UnityWebRequest("https://dumka.pythonanywhere.com/change_rating", "POST");

        var players = new List<Players> {
            new Players { actor_num="123", score_change=25},
            new Players { actor_num="456", score_change=25}
        };

        var data = new RatingChangeList { players=players };
        var json_data = JsonUtility.ToJson(data);
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json_data);

        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");

        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError) Debug.Log("Error While Sending: " + uwr.error);
        else
        {
            var status_check = JsonUtility.FromJson<Status_check>(uwr.downloadHandler.text);
            if (status_check != null && status_check.status == 0)
            {
                Debug.Log("succesfull post registerNewPlayer");
                StartCoroutine(get_rating());
            }
            else Debug.Log("error post registerNewPlayer - " + status_check.info);
        }
    }


    private class New_player
    {
        public string name = string.Empty;
        public string actor_num = string.Empty;
    }

    private class Status_check
    {
        public int status = 0;
        public string info = string.Empty;
    }

    [System.Serializable]
    private class InfoList
    {
        public List<PlayerInfo> players = new List<PlayerInfo>();
        public int status = 0;

        [System.Serializable]
        public class PlayerInfo
        {
            public string name = string.Empty;
            public int score = 0;
        }

    }

    [System.Serializable]
    private class RatingChangeList
    {
        public List<Players> players = new List<Players>();
    }
    [System.Serializable]
    public class Players
    {
        public string actor_num = string.Empty;
        public int score_change = 0;
    }

}
