using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class LanguageSystem : MonoBehaviour
{
    private string json;
    public static languages lng = new languages();
    public TextAsset test1;

    private string currentLanguage;
    private Dictionary<string, string> localizedText;
    private static bool isReady = false;

    public delegate void ChangeLangText();
    public event ChangeLangText OnLanguageChanged;

    private void Awake()
    {
        PlayerPrefs.SetString("Language", "ru_RU");

        LanguageLoad();
    }

    public void LanguageLoad()
    {
#if UNITY_ANDROID
        string path = Path.Combine(Application.streamingAssetsPath + "/Language/" + PlayerPrefs.GetString("Language") + ".json");
        WWW reader = new WWW(path);
        //UnityWebRequest unityWebRequest = new UnityWebRequest(path);
        while (!reader.isDone) { }
        json = reader.text;
        lng = JsonUtility.FromJson<languages>(json);
#endif
#if UNITY_EDITOR
        json = File.ReadAllText(Application.streamingAssetsPath + "/Language/" + PlayerPrefs.GetString("Language") + ".json");
        lng = JsonUtility.FromJson<languages>(json);
#endif
    }


}


public class languages
{
    public string[] AchievementName = new string[25];
    public string[] AchievementText = new string[25];
    public string[] tablesVariantName = new string[15];
    public string[] tablesVariantText = new string[15];
    public string[] tablesVariantPrice = new string[15];
}

