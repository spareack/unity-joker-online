using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClanMenu : MonoBehaviour
{
    public Text clanNameText;
    public Text clanDiscriptionText;

    [Header("Controllers")]
    public int panCount1;
    [Range(0, 1000)]
    public int panOffset;
    [Range(0f, 20f)]
    public float snapSpeed;
    [Header("Other Objects")]

    public GameObject panPrefab1;

    private GameObject[] instPans1;
    private Vector2[] pansPos1;

    private RectTransform contentRect1;
    private Vector2 contentVector1;

    private int selectedPanID1;
    private bool isScrolling1;
    public ScrollRect scrollRectType1;

    [SerializeField] private GameObject content1;



    public List<string> allNames = new List<string>();
    public List<string> allScore = new List<string>();

    private int whichScroll = 2;


    [SerializeField] private DataCheck DC;

    private void Awake()
    {
    }

    private void Start()
    {
    }
    public void SpawnScroll1()
    {
        contentRect1 = content1.GetComponent<RectTransform>();
        instPans1 = new GameObject[panCount1];
        pansPos1 = new Vector2[panCount1];
        for (int i = 0; i < panCount1; i++)
        {
            instPans1[i] = Instantiate(panPrefab1, content1.transform, false);

            Text[] txt = instPans1[i].GetComponentsInChildren<Text>();
            int num = i;

            txt[0].text = allNames[i];
            //txt[1].text = LanguageSystem.lng.tablesVariantText[i];
            txt[1].text = allScore[i];

            if (i == 0) continue;
            instPans1[i].transform.localPosition = new Vector2(instPans1[i].transform.localPosition.x,
                instPans1[i - 1].transform.localPosition.y - panPrefab1.GetComponent<RectTransform>().sizeDelta.y - panOffset);
            pansPos1[i] = -instPans1[i].transform.localPosition;
        }
    }

    private void FixedUpdate()
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
    public void Scrolling(bool scroll)
    {
        isScrolling1 = scroll;
        if (scroll) scrollRectType1.inertia = true;
    }
}
