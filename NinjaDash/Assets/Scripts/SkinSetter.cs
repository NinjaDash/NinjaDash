using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinSetter : MonoBehaviour
{
    [SerializeField] Animator myAnimator;

    private void Awake()
    {
        GetComponentInParent<CharacterController2D>().animator = myAnimator;
        GetComponentInParent<PlayerMovement>().animator = myAnimator;
        GetComponentInParent<Attack>().animator = myAnimator;
    }
}
