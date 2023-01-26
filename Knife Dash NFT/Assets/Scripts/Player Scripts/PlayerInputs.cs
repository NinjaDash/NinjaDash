using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(0)]
public class PlayerInputs : MonoBehaviour
{
    public static PlayerInputs Instance;
    public PlayerMovement myPlayer;
    public CharacterController2D myCC;
    public Attack myAttack;

    public static bool canControl = true;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public bool leftPressed { get; set; }
    public bool rightPressed { get; set; }

    public bool LeftPressed()
    {
        return leftPressed && canControl;
    }
    public bool RightPressed()
    {
        return rightPressed && canControl;
    }

    public void OnDash()
    {
        if (!canControl) return;
        if (myPlayer)
        {
            myPlayer.OnDash();
        }
    }

    public void OnJump()
    {
        if (!canControl) return;
        if (myPlayer)
        {
            myPlayer.OnJump();
        }
    }

    public void OnFire()
    {
        if (!canControl) return;
        if (myAttack)
        {
            myAttack.OnFireUI();
        }
    }

    public void RetrieveKnife()
    {
        if (!canControl) return;
        if (myAttack)
        {
            myAttack.RetrieveKnife();
        }
    }

    public void useUI()
    {
        if(myPlayer)
            myPlayer.useKeyboard = false;
    }

}
