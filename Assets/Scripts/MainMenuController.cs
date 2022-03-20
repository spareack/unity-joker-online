using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Animator profileAnim;

    private int state = 1;

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

}
