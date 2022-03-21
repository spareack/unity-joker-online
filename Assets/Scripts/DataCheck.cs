using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DataCheck : MonoBehaviour
{
    public DataSave save = null;
    public GameObject winAchieveBoard = null;
    public Text winAchieveBoardText = null;
    public int[] achievementProgress = new int[25];

    private void Awake()
    {
        // PlayerPrefs.DeleteAll();

        if (PlayerPrefs.HasKey("DataSave")) save = JsonUtility.FromJson<DataSave>(PlayerPrefs.GetString("DataSave"));
        else
        {
            save = new DataSave();
            saveChanges();
        }
    }
    public void saveChanges()
    {
        PlayerPrefs.SetString("DataSave", JsonUtility.ToJson(save));
        PlayerPrefs.Save();
    }

    public void checkForAchievement()
    {
        for(int i = 0; i < achievementProgress.Length; i += 1)
        {
            if (achievementProgress[i] >= save.achievementGoal[i])
            {
                achievementProgress[i] -= save.achievementGoal[i];
                save.achievementLevel[i] += 1;
                showAchieveWin(i);
            }
        }
        for(int i = 0; i < save.achievementProgress.Length; i += 1)
        {
            if (save.achievementProgress[i] >= save.achievementGoal[i])
            {
                save.achievementProgress[i] -= save.achievementGoal[i];
                save.achievementLevel[i] += 1;
                showAchieveWin(i);
            }
        }
        saveChanges();
    }

    public void showAchieveWin(int index)
    {
        StartCoroutine(showAchieveWinCoroutine(index));
    }
    IEnumerator showAchieveWinCoroutine(int index)
    {
        winAchieveBoardText.text = "Получено достижение: №" + index;
        float duration = 0.5f;
        var startPos = new Vector3(-72, 628, 0);;
        var endPos = new Vector3(-72, 452, 0);
        for(float t = 0; t < duration; t += Time.deltaTime)
        {
            winAchieveBoard.transform.localPosition = Vector3.Lerp(startPos, endPos, t / duration);
            yield return null;
        }

        yield return new WaitForSeconds(2f);

        for(float t = 0; t < duration; t += Time.deltaTime)
        {
            winAchieveBoard.transform.localPosition = Vector3.Lerp(endPos, startPos, t / duration);
            yield return null;
        }
    }
}

[Serializable]
public class DataSave
{
    public int[] achievementGoal = new int[25]
    {
        100, // 0 --- | Выиграть 100 рейтинговых матчей
        10, // 1 --- | Игрок 10 раз подряд занимает не ниже второго места
        20, // 2 --- | Игрок 20 раз подряд занимает не ниже второго места
        30, // 3 --- | Игрок 30 раз подряд занимает не ниже второго места
        50, // 4 --- | Игрок 50 раз подряд занимает не ниже второго места
        5, // 5 --- | 5 раздач подряд игроку приходит Джокер
        3, // 6 --- | 3 раздачи подряд игроку приходят 2 Джокера
        20, // 7 --- | Более 20 джокеров за одну партию
        3, // 8 --- | Дается игроку, занявшему 3 раза подряд первое место
        4, // 9 --- | Дается игроку, занявшему 4 раза подряд первое место
        5, // 10 --- | Дается игроку, занявшему 5 раза подряд первое место
        2, // 11 --- | 2 премии за игру
        3, // 12 --- | 3 премии за игру
        4, // 13 --- | 4 премии за игру
        4, // 14 --- | Игрок 4 раза вышел на премию, подряд
        1, // 15 --- | Игрок взял 9 из 9
        2, // 16 --- | Игрок взял 9 из 9, дважды за оду партию
        3, // 17 --- | Игрок взял 9 из 9, трижды за оду партию
        4, // 18 --- | Игрок взял 9 из 9, четырежды за оду партию
        1, // 19 --- | Одержана победа с двукратным перевосходством
        7000, // 20 --- | Более 7000 очков за партию
        10, // 21 --- | Игрок занял 4 место 10 раз
        15, // 22 --- Недобор 15 раз в пределах одной партии
        6, // 23 --- | Дается игроку, который умудрился получить 6 ''штанг'' в одной пульке
        1 // 24 --- | Даётся, при покупке любого предмета в магазине
    };
    public int[] achievementLevel = new int[25];
    public int[] achievementProgress = new int[25];
}
