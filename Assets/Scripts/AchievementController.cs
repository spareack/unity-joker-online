using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementController : MonoBehaviour
{
    [SerializeField] private GameObject achivementPrefeb;
    [SerializeField] private GameObject achievInfoCard;

    [SerializeField] private Text[] achievInfoCardText;

    [SerializeField] private DataCheck DC;
    [SerializeField] private MainMenuController MainMenuControllerScr;



    private void Start()
    {
        SpawnAchiev();
    }

    private void SpawnAchiev()
    {
        float y = 0.6f;
        for (int i = 0; i < 4; i++)
        {
            float x = -8f;
            for (int j = 0; j < 6; j++)
            {
                int num = i * 6 + j;
                Vector3 pos = new Vector3(x, y, -9f);
                GameObject achiev = Instantiate(achivementPrefeb, pos, Quaternion.identity, transform);
                achiev.GetComponent<Button>().onClick.AddListener(() => ButtonController(num));
                x += 2.25f;
            }
            y -= 1.2f;
        }
        MainMenuControllerScr.OpenOrColeProfile();
    }

    public void ButtonController(int num)
    {
        achievInfoCardText[0].text = LanguageSystem.lng.AchievementName[num];
        achievInfoCardText[1].text = LanguageSystem.lng.AchievementText[num];
        achievInfoCardText[2].text = DC.DS.achievementLevel[num] + " раз";

        achievInfoCard.SetActive(true);
    }

    public void CloseInfoPanel()
    {
        achievInfoCard.SetActive(false);
    }
}
