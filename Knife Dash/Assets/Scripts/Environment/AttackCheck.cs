using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCheck : MonoBehaviour
{
    private Enemy parent;
    bool canAttack = true;
    bool canThrow = true;
    public LayerMask blockProjectile;
    public float timer = 0;
    private void Awake()
    {
        parent = GetComponentInParent<Enemy>();
    }

    private void Update()
    {
        if (parent.life <= 0) return;
        if (canThrow)
        {
            RaycastHit2D hit = Physics2D.Raycast(this.transform.position, transform.localToWorldMatrix.MultiplyVector(transform.right), 10, blockProjectile);
            if (hit)
            {
                if (hit.collider.CompareTag("Player"))
                {
                    canThrow = false;
                    parent.enemyState = eState.range;
                }
                Debug.DrawLine(transform.position, hit.point, Color.red);
            }
        }
        else
        {
            timer += Time.deltaTime;
            if(timer > 4)
            {
                timer = 0;
                canThrow = true;
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (canAttack)
        {
            if (collision.gameObject.CompareTag("Player") && parent.life > 0)
            {
                parent.enemyState = eState.meele;
                Invoke(nameof(EnableSelf), 1);
                canAttack = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && parent.life > 0 && parent.initState != eState.STATIC)
        {
            parent.enemyState = eState.walking;
        }
    }

    private void EnableSelf()
    {
        canAttack  =true;
    }
}
