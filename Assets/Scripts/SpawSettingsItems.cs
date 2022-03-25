using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawSettingsItems : MonoBehaviour
{
    [Header("Controllers")]
    public int panCount1;
    public int panCount2;
    [Range(0, 1000)]
    public int panOffset;
    [Range(0f, 20f)]
    public float snapSpeed;
    [Header("Other Objects")]

    public GameObject panPrefab1;
    public GameObject panPrefab2;


    private GameObject[] instPans1;
    private GameObject[] instPans2;
    private Vector2[] pansPos1;
    private Vector2[] pansPos2;

    private RectTransform contentRect1;
    private RectTransform contentRect2;
    private Vector2 contentVector1;
    private Vector2 contentVector2;

    private int selectedPanID1;
    private int selectedPanID2;
    private bool isScrolling1;
    private bool isScrolling2;
    public ScrollRect scrollRect2;
    public ScrollRect scrollRectType1;
    public ScrollRect scrollRectType2;

    [SerializeField] private GameObject content1;
    [SerializeField] private GameObject content2;

    [SerializeField] private GameObject[] settingsMenues;

    [SerializeField] private Button[] settingsMenuesButton;

    [SerializeField] private Sprite[] tablesVariant;
    [SerializeField] private Sprite[] decksVariant;

    private Color buyedColor = new Color(1, 0.7795244f, 0);

    private Color pressedColor = new Color(0.5411765f, 0.5411765f, 0.5411765f);
    private Color unPressedColor = new Color(0.42f, 0.42f, 0.42f);



    private List<Button> buttons1 = new List<Button>();
    private List<Button> buttons2 = new List<Button>();
    private List<Image> images1 = new List<Image>();
    private List<Image> images2 = new List<Image>();

    [SerializeField] private List<Button> settingButton = new List<Button>();

    private int whichScroll = 2;


    [SerializeField] private DataCheck DC;

    private void Awake()
    {
        for (int i = 0; i < settingsMenues.Length; i++)
        {
            settingsMenues[i].SetActive(true);
        }
    }

    private void Start()
    {
        SpawnScroll1();
        SpawnScroll2();
        SettingsGiveColor();
    }
    private void SpawnScroll1()
    {
        DC.save.tables[0] = 1;
        contentRect1 = content1.GetComponent<RectTransform>();
        instPans1 = new GameObject[panCount1];
        pansPos1 = new Vector2[panCount1];
        for (int i = 0; i < panCount1; i++)
        {
            instPans1[i] = Instantiate(panPrefab1, content1.transform, false);

            Image[] img = instPans1[i].GetComponentsInChildren<Image>();
            Text[] txt = instPans1[i].GetComponentsInChildren<Text>();
            Button[] but = instPans1[i].GetComponentsInChildren<Button>();
            int num = i;

            img[1].sprite = tablesVariant[i];
            
            txt[0].text = LanguageSystem.lng.tablesVariantName[i];
            txt[1].text = LanguageSystem.lng.tablesVariantText[i];
            txt[2].text = LanguageSystem.lng.tablesVariantPrice[i];

            but[0].onClick.AddListener(() => MenuBuyButton1(num));

            buttons1.Add(but[0]);
            images1.Add(img[2]);

            if (DC.save.tables[i] != 0)
            {
                but[0].GetComponent<Image>().color = buyedColor;
                img[2].gameObject.SetActive(false);
            }
            if(DC.save.choosenTable == i)
            {
                but[0].gameObject.SetActive(false);
            }


            if (i == 0) continue;
            instPans1[i].transform.localPosition = new Vector2(instPans1[i].transform.localPosition.x,
                instPans1[i - 1].transform.localPosition.y - panPrefab1.GetComponent<RectTransform>().sizeDelta.y - panOffset);
            pansPos1[i] = -instPans1[i].transform.localPosition;
        }

        settingsMenues[1].SetActive(false);
    }

    private void SpawnScroll2()
    {
        DC.save.decks[0] = 1;
        contentRect2 = content2.GetComponent<RectTransform>();
        instPans2 = new GameObject[panCount2];
        pansPos2 = new Vector2[panCount2];
        for (int i = 0; i < panCount2; i++)
        {
            instPans2[i] = Instantiate(panPrefab2, content2.transform, false);

            Image[] img = instPans2[i].GetComponentsInChildren<Image>();
            Text[] txt = instPans2[i].GetComponentsInChildren<Text>();
            Button[] but = instPans2[i].GetComponentsInChildren<Button>();
            int num = i;

            img[1].sprite = decksVariant[i];

            txt[0].text = LanguageSystem.lng.decksVariantName[i];
            txt[1].text = LanguageSystem.lng.decksVariantText[i];
            txt[2].text = LanguageSystem.lng.decksVariantPrice[i];

            but[0].onClick.AddListener(() => MenuBuyButton2(num));

            buttons2.Add(but[0]);
            images2.Add(img[2]);

            if (DC.save.decks[i] != 0)
            {
                but[0].GetComponent<Image>().color = buyedColor;
                img[2].gameObject.SetActive(false);
            }
            if (DC.save.choosenDeck == i)
            {
                but[0].gameObject.SetActive(false);
            }

            if (i == 0) continue;
            instPans2[i].transform.localPosition = new Vector2(instPans2[i].transform.localPosition.x,
                instPans2[i - 1].transform.localPosition.y - panPrefab2.GetComponent<RectTransform>().sizeDelta.y - panOffset);
            pansPos2[i] = -instPans2[i].transform.localPosition;
        }

        settingsMenues[2].SetActive(false);
    }

    private void ChangeScrollRect(int num)
    {
        if (num == 1)
        {
            // scrollRectType1;
            whichScroll = 1;
        }
        else if (num == 2)
        {
            //scrollRectType2;
            whichScroll = 2;
        }
    }

    private void FixedUpdate()
    {
        if (whichScroll == 1)
        {
            if (contentRect1.anchoredPosition.y <= pansPos1[0].y && !isScrolling1 || contentRect1.anchoredPosition.y >= pansPos1[pansPos1.Length - 1].y && !isScrolling1)
            {
                scrollRectType1.inertia = false;
            }
            float nearestPos = float.MaxValue;
            for (int i = 1; i < panCount1 - 2; i++)
            {
                float distance = Mathf.Abs(contentRect1.anchoredPosition.y - pansPos1[i].y);
                if (distance < nearestPos)
                {
                    nearestPos = distance;
                    selectedPanID1 = i;
                }
            }
            float scrollVelocity = Mathf.Abs(scrollRectType1.velocity.y);
            if (scrollVelocity < 700 && !isScrolling1) scrollRectType1.inertia = false;
            if (isScrolling1 || scrollVelocity > 700) return;
            contentVector1.y = Mathf.SmoothStep(contentRect1.anchoredPosition.y, pansPos1[selectedPanID1].y, snapSpeed * Time.fixedDeltaTime);
            contentRect1.anchoredPosition = contentVector1;
        }
        else if (whichScroll == 2)
        {
            if (contentRect2.anchoredPosition.y <= pansPos2[0].y && !isScrolling2 || contentRect2.anchoredPosition.y >= pansPos2[pansPos2.Length - 1].y && !isScrolling2)
            {
                scrollRectType2.inertia = false;
            }
            float nearestPos = float.MaxValue;
            for (int i = 1; i < panCount2 - 2; i++)
            {
                float distance = Mathf.Abs(contentRect2.anchoredPosition.y - pansPos2[i].y);
                if (distance < nearestPos)
                {
                    nearestPos = distance;
                    selectedPanID2 = i;
                }
            }
            float scrollVelocity = Mathf.Abs(scrollRectType2.velocity.y);
            if (scrollVelocity < 700 && !isScrolling2) scrollRectType2.inertia = false;
            if (isScrolling2 || scrollVelocity > 700) return;
            contentVector2.y = Mathf.SmoothStep(contentRect2.anchoredPosition.y, pansPos2[selectedPanID2].y, snapSpeed * Time.fixedDeltaTime);
            contentRect2.anchoredPosition = contentVector2;
        }

    }
    public void Scrolling(bool scroll)
    {
        if (whichScroll == 1)
        {
            isScrolling1 = scroll;
            if (scroll) scrollRectType1.inertia = true;
        }
        else if (whichScroll == 2)
        {
            isScrolling2 = scroll;
            if (scroll) scrollRectType2.inertia = true;
        }
    }

    public void ButtonSettingsController(int num)
    {
        for(int i = 0; i < settingsMenues.Length; i++)
        {
            settingsMenues[i].SetActive(false);
            settingsMenuesButton[i].GetComponent<Image>().color = unPressedColor;
        }
        settingsMenues[num].SetActive(true);
        settingsMenuesButton[num].GetComponent<Image>().color = pressedColor;

        whichScroll = num;
    }


    public void MenuBuyButton1(int num)
    {
        // Добавить проверку на монеты
        if (DC.save.tables[num] == 0)
        {
            DC.save.tables[num] = 1;
            DC.save.choosenTable = num;
            buttons1[num].GetComponent<Image>().color = buyedColor;
            images1[num].gameObject.SetActive(false);
        }
        else
        {
            DC.save.choosenTable = num;
        }

        for (int i = 0; i < buttons1.Count; i++)
        {
            buttons1[i].gameObject.SetActive(true);
        }
        buttons1[DC.save.choosenTable].gameObject.SetActive(false);
        DC.saveChanges();
    }

    public void MenuBuyButton2(int num)
    {
        // Добавить проверку на монеты
        if (DC.save.decks[num] == 0)
        {
            DC.save.decks[num] = 1;
            DC.save.choosenDeck = num;
            buttons2[num].GetComponent<Image>().color = buyedColor;
            images2[num].gameObject.SetActive(false);
        }
        else
        {
            DC.save.choosenDeck = num;
        }

        for (int i = 0; i < buttons2.Count; i++)
        {
            buttons2[i].gameObject.SetActive(true);
        }
        buttons2[DC.save.choosenDeck].gameObject.SetActive(false);
        DC.saveChanges();
    }

    public void SettingsController(int num)
    {
        if (DC.save.settings[num] == 0)
        {
            DC.save.settings[num] = 1;
            settingButton[num].GetComponent<Image>().color = Color.green;
        }
        else if (DC.save.settings[num] == 1)
        {
            DC.save.settings[num] = 0;
            settingButton[num].GetComponent<Image>().color = Color.white;
        }
        DC.saveChanges();
    }

    private void SettingsGiveColor()
    {
        for (int i = 0; i < settingButton.Count; i++)
        {
            if (DC.save.settings[i] == 1) settingButton[i].GetComponent<Image>().color = Color.green;
        }
        settingsMenues[3].SetActive(false);
    }
}
