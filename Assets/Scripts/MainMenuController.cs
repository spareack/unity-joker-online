using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Animator profileAnim;

    [SerializeField] private GameObject settingsMenu;

    private int state = 1;

    private void Awake()
    {
        StartCoroutine(FirstOpen());
    }

    private void Start()
    {
        
        //profileAnim.Play("right");
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
