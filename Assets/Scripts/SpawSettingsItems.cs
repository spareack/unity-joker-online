using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawSettingsItems : MonoBehaviour
{
    [Range(1, 50)]
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
    [SerializeField] private Sprite[]tablesVariant;

    private int whichScroll = 2;

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
    }
    private void SpawnScroll1()
    {
        contentRect1 = content1.GetComponent<RectTransform>();
        instPans1 = new GameObject[panCount1];
        pansPos1 = new Vector2[panCount1];
        for (int i = 0; i < panCount1; i++)
        {
            instPans1[i] = Instantiate(panPrefab1, content1.transform, false);

            Image[] img = instPans1[i].GetComponentsInChildren<Image>();
            img[1].sprite = tablesVariant[i];

            Text[] txt = instPans1[i].GetComponentsInChildren<Text>();
            txt[0].text = LanguageSystem.lng.tablesVariantName[i];
            txt[1].text = LanguageSystem.lng.tablesVariantText[i];
            txt[2].text = LanguageSystem.lng.tablesVariantPrice[i];


            if (i == 0) continue;
            instPans1[i].transform.localPosition = new Vector2(instPans1[i].transform.localPosition.x,
                instPans1[i - 1].transform.localPosition.y - panPrefab1.GetComponent<RectTransform>().sizeDelta.y - panOffset);
            pansPos1[i] = -instPans1[i].transform.localPosition;
        }

        settingsMenues[1].SetActive(false);
    }

    private void SpawnScroll2()
    {
        contentRect2 = content2.GetComponent<RectTransform>();
        instPans2 = new GameObject[panCount2];
        pansPos2 = new Vector2[panCount2];
        for (int i = 0; i < panCount2; i++)
        {
            instPans2[i] = Instantiate(panPrefab2, content2.transform, false);
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
        }
        settingsMenues[num].SetActive(true);
        whichScroll = num;
    }
}
