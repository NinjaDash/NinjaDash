using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBounds : MonoBehaviour
{
    float timer = 0;
    bool hasLeftArea = false;
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") )
        {
            timer = 0;
            hasLeftArea = true;
            Debug.Log("Player left area");
        }
        else if(collision.CompareTag("Enemy"))
        {
            collision.GetComponent<IDamageable>().ApplyDamage(100, Vector3.zero);
        }
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player entered area");
            hasLeftArea = false;
        }
    }

    public void Update()
    {
        if (hasLeftArea)
        {
            timer += Time.deltaTime;
        }
        if(timer >= 3 && hasLeftArea)
        {
            hasLeftArea = false;
            Debug.Log("Game Over" + PlayerInputs.Instance.myCC.transform.position);
            PlayerInputs.Instance.myCC.ApplyDamage(100 , Vector2.zero);
        }
    }
}
