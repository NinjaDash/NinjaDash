using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentInteraction : MonoBehaviour
{
    public LayerMask Ground;
    public LayerMask Wall;
    public LayerMask Damage;
    public CamShake camShake;
    Rigidbody2D rb2d;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Hazard"))
        { 
            if (!collision.gameObject.TryGetComponent<AdHazardCore>(out AdHazardCore ad))
            {
                GetComponent<IDamageable>().ApplyDamage(2, collision.ClosestPoint(this.transform.position));
                StartCoroutine(enableDamageDealer(collision));
            }
        }
        if(collision.CompareTag("Rope"))
        {
            collision.GetComponentInParent<Girl>().FreeSelf();
        }
        if(collision.CompareTag("Girl"))
        {
            collision.GetComponent<Girl>().FreeSelf();
        }

    }

    IEnumerator enableDamageDealer(Collider2D CD)
    {
        CD.enabled = false;
        yield return new WaitForSeconds(1.5f);
        CD.enabled = true;
    }
   /* public void OnCollisionEnter2D(Collision2D collision)
    {
        //if((Ground & (1 << collision.gameObject.layer)) != 0)
        if (collision.collider.CompareTag("Ground"))
        {
            Debug.Log("Touching Ground");
        }
        
    }*/
    public void OnCollisionExit2D(Collision2D collision)
    {
        
    }
}
