using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableProjectile : MonoBehaviour
{
	public Vector2 direction;
	public Vector2 endPos;
	public bool enemyProjectile;
	public bool hasHit = false;
	public bool hasRotation = false;
	public float speed = 15f;
	public int rotationSpeed = 1080;
	public GameObject owner;
	private Rigidbody2D rb2d;

    private void Awake()
    {
		TryGetComponent<Rigidbody2D>(out rb2d);
    }

    private void OnEnable()
    {
		if(rb2d)
		rb2d.bodyType = RigidbodyType2D.Dynamic;
    }
    private void OnDisable()
    {
		
    }

	// Update is called once per frame
	void FixedUpdate()
	{
		if (!hasHit)
        {
			rb2d.velocity = direction * speed;
			//transform.position = Vector2.MoveTowards(transform.position, endPos, Time.deltaTime * speed);
        }
		/*if((Vector2)transform.position == endPos && !hasHit)
        {
			hasHit = true;
			rb2d.bodyType = RigidbodyType2D.Static;
			PlayerInputs.Instance.myAttack.KnifeStuck();
			Debug.Log("stuck at" + transform.position);
			FixLocation();
		}*/
		if (hasRotation)
		{
			transform.Rotate(Vector3.back, Time.deltaTime * rotationSpeed * direction.x);
		}
	}
    private void OnTriggerEnter2D(Collider2D collision)
    {
		if (enemyProjectile)
		{
			if (collision.gameObject.tag == "Player" && collision.gameObject != owner)
			{
				Debug.Log("hit player");
				collision.gameObject.GetComponent<CharacterController2D>().ApplyDamage(2f, transform.position);
				Destroy(this.gameObject);
			}
			else if (collision.gameObject.tag == "Ground")
			{
				Debug.Log("hit ground");
				Destroy(this.gameObject);
			}
		}
		else
		{
			if (owner != null && collision.gameObject != owner && collision.gameObject.tag == "Enemy")
			{
				var enemy = collision.gameObject.GetComponent<IDamageable>();
				enemy.ApplyDamage(Mathf.Sign(direction.x) * 2f, Vector2.zero);
			}
            else if (collision.gameObject.tag == "Ground")
            {
                hasHit = true;
                rb2d.bodyType = RigidbodyType2D.Static;
                PlayerInputs.Instance.myAttack.KnifeStuck();
                Debug.Log("did collide");
                FixLocation();
            }
            if (collision.gameObject.CompareTag("Rope"))
            {
				collision.GetComponentInParent<Girl>().FreeSelf();
            }
			if (collision.gameObject.TryGetComponent<AdHazardCore>(out AdHazardCore ad))
			{
				ad.ApplyDamage(100, Vector3.zero);
			}
		}
	}

	public void FixLocation()
    {
		if (Vector3.Distance(transform.position, endPos) < 1)
		{
			Debug.Log("adjusted");
			transform.position = endPos;
		}
		else
        {
			Debug.Log(endPos);
        }
    }
}
