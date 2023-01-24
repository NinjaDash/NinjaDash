using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdLaser : MonoBehaviour
{
    public AdHazardCore myHazard;
    public void Retract()
    {
        GetComponent<Collider2D>().enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !myHazard.AdPlayed)
        {
            Debug.Log("Play Ad Here");
            var cc = collision.gameObject.GetComponent<CharacterController2D>();
            OnPlayerHit(cc);
        }
    }

    public void OnPlayerHit(CharacterController2D CC)
    {
        CC.ApplyDamage(2f, transform.position);
        myHazard.RetractAll();
    }
}
