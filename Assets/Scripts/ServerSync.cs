using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ServerSync : MonoBehaviour
{
    private void Start()
    {
        //var players = new List<ChangeScore.Players> {
        //        new ChangeScore.Players { actor_num="123", score_change=25 },
        //        new ChangeScore.Players { actor_num="456", score_change=25 }
        //    };

        //new ChangeScore(this, players);

        //new GetClan(this, 2);

        //new CreateClan(this, "789", "Mediki", "Lechim ludei");
    }

    public class JoinClan
    {
        public Status_check response = null;
        public IEnumerator JoinClanCoroutine(string actor_num, int clan_id)
        {
            var uwr = new UnityWebRequest("https://dumka.pythonanywhere.com/join_clan", "POST");

            var data = new SendClass { actor_num = actor_num, clan_id = clan_id };
            var json_data = JsonUtility.ToJson(data);
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json_data);

            uwr.uploadHandler = new UploadHandlerRaw(jsonToSend);
            uwr.downloadHandler = new DownloadHandlerBuffer();
            uwr.SetRequestHeader("Content-Type", "application/json");

            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.ConnectionError) Debug.Log("Error While Sending: " + uwr.error);
            else response = JsonUtility.FromJson<Status_check>(uwr.downloadHandler.text);
        }

        [System.Serializable]
        public class SendClass
        {
            public string actor_num = string.Empty;
            public int clan_id = 0;
        }
    }


    public class CreateClan
    {
        public ResponseClass response = null;
        public IEnumerator CreateClanCoroutine(string actor_num, string name, string description)
        {
            Debug.Log(actor_num + " !!!!!1");
            var uwr = new UnityWebRequest("https://dumka.pythonanywhere.com/create_clan", "POST");

            var data = new SendClass { actor_num = actor_num, clan_name = name, description = description };
            var json_data = JsonUtility.ToJson(data);
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json_data);

            uwr.uploadHandler = new UploadHandlerRaw(jsonToSend);
            uwr.downloadHandler = new DownloadHandlerBuffer();
            uwr.SetRequestHeader("Content-Type", "application/json");

            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.ConnectionError) Debug.Log("Error While Sending: " + uwr.error);
            else response = JsonUtility.FromJson<ResponseClass>(uwr.downloadHandler.text);
        }

        [System.Serializable]
        public class SendClass
        {
            public string actor_num = string.Empty;
            public string clan_name = string.Empty;
            public string description = string.Empty;
        }

        [System.Serializable]
        public class ResponseClass
        {
            public int status = 0;
            public string info = string.Empty;
            public int clan_id = 0;
        }
    }


    public class GetClan
    {
        public ResponseClass response = null;
        public IEnumerator GetClanCoroutine(int clan_id)
        {
            var uwr = new UnityWebRequest("https://dumka.pythonanywhere.com/get_clan", "POST");

            var data = new Clan_info_send { clan_id = clan_id };
            var json_data = JsonUtility.ToJson(data);
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json_data);

            uwr.uploadHandler = new UploadHandlerRaw(jsonToSend);
            uwr.downloadHandler = new DownloadHandlerBuffer();
            uwr.SetRequestHeader("Content-Type", "application/json");

            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.ConnectionError) Debug.Log("Error While Sending: " + uwr.error);
            else response = JsonUtility.FromJson<ResponseClass>(uwr.downloadHandler.text);
        }

        [System.Serializable]
        public class Clan_info_send
        {
            public int clan_id = -1;
        }

        [System.Serializable]
        public class ResponseClass
        {
            public string clan_name = string.Empty;
            public string description = string.Empty;
            public int clan_score = 0;
            public List<Member> members = null;
            public List<MessagesData> messages_data = null;

            public int status = -1;
            public string info = string.Empty;
        }

        [System.Serializable]
        public class Member
        {
            public string actor_num = string.Empty;
            public string name = string.Empty;
            public int score = 0;
        }

        [System.Serializable]
        public class MessagesData
        {
            public string sender = string.Empty;
            public string text = string.Empty;
        }
    }


    public class GetRating
    {
        public ResponseClass response = null;
        public IEnumerator GetRatingCoroutine()
        {
            UnityWebRequest uwr = UnityWebRequest.Get("https://dumka.pythonanywhere.com/get_rating");
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.ConnectionError) Debug.Log("Error While Sending: " + uwr.error);
            else response = JsonUtility.FromJson<ResponseClass>(uwr.downloadHandler.text);
        }

        [System.Serializable]
        public class ResponseClass
        {
            public List<PlayerInfo> players = new List<PlayerInfo>();
            public int status = 0;
            public string info = string.Empty;

            [System.Serializable]
            public class PlayerInfo
            {
                public string name = string.Empty;
                public string actor_num = string.Empty;
                public int clan_id = -1;
                public int score = 0;
            }
        }
    }


    public class RegisterNewPlayer
    {
        public Status_check response = null;
        public IEnumerator RegisterNewPlayerCoroutine(string name, string actor_num)
        {
            var uwr = new UnityWebRequest("https://dumka.pythonanywhere.com/register_new_user", "POST");

            var data = new SendClass { name = name, actor_num = actor_num };
            var json_data = JsonUtility.ToJson(data);
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json_data);

            uwr.uploadHandler = new UploadHandlerRaw(jsonToSend);
            uwr.downloadHandler = new DownloadHandlerBuffer();
            uwr.SetRequestHeader("Content-Type", "application/json");

            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.ConnectionError) Debug.Log("Error While Sending: " + uwr.error);
            else response = JsonUtility.FromJson<Status_check>(uwr.downloadHandler.text);
        }
        public class SendClass
        {
            public string name = string.Empty;
            public string actor_num = string.Empty;
        }
    }


    public class ChangeScore
    {
        public Status_check response = null;
        public IEnumerator changeScoreCoroutine(List<Players> players)
        {
            var uwr = new UnityWebRequest("https://dumka.pythonanywhere.com/change_rating", "POST");

            var data = new SendClass { players = players };
            var json_data = JsonUtility.ToJson(data);
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json_data);

            uwr.uploadHandler = new UploadHandlerRaw(jsonToSend);
            uwr.downloadHandler = new DownloadHandlerBuffer();
            uwr.SetRequestHeader("Content-Type", "application/json");

            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.ConnectionError) Debug.Log("Error While Sending: " + uwr.error);
            else response = JsonUtility.FromJson<Status_check>(uwr.downloadHandler.text);
        }

        [System.Serializable]
        public class SendClass
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

    public class Status_check
    {
        public int status = 0;
        public string info = string.Empty;
    }
}
