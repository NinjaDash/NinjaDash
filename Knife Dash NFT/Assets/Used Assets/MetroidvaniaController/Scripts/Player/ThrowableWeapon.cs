using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableWeapon : MonoBehaviour
{
	public Vector2 direction;
	public bool hasHit = false;
	public float speed = 10f;
	private Rigidbody2D rb2d;

    // Start is called before the first frame update
    void Awake()
    {
		rb2d = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
		hasHit = false;
		rb2d.bodyType = RigidbodyType2D.Dynamic;
    }

    private void OnDisable()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
		if (!hasHit)
		rb2d.velocity = direction * speed;
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		Debug.Log("dadsadasd");
		if (collision.gameObject.tag == "Enemy")
		{
			//collision.gameObject.SendMessage("ApplyDamage", Mathf.Sign(direction.x) * 2f);
			//Destroy(gameObject);
		}
		else if (collision.gameObject.tag != "Player")
		{
			Debug.Log("dadsadasd");
			//Destroy(gameObject);
			hasHit = true;
			rb2d.bodyType = RigidbodyType2D.Static;
			//rb2d.velocity = Vector2.zero;
		}
	}
}
