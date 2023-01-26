 using UnityEngine;
using System.Collections;

[DefaultExecutionOrder(3)]
public class Enemy : MonoBehaviour, IDamageable
{

	public eState enemyState;
	public GameObject projectile;
	public float life = 10;
	private bool isDead;
	private bool isPlat;
	private bool isObstacle;
	private Transform fallCheck;
	private Transform wallCheck;
	private Transform attackCheck;
	public LayerMask turnLayerMask;
	private Rigidbody2D rb;
	private Animator _animator;

	private bool facingRight = true;

	public float speed = 5f;

	public ParticleSystem PS;
	public ParticleSystem PS2;

	public bool isInvincible = false;
	private bool isHitted = false;

	public eState initState { get; private set; }

	void Awake() {
		LevelDataHolder.Instance.AmountOfEnemies++;
		fallCheck = transform.Find("FallCheck");
		wallCheck = transform.Find("WallCheck");
		attackCheck = transform.Find("attackCheck");
		rb = GetComponent<Rigidbody2D>();
		_animator = GetComponent<Animator>();		
	}

    private void Start()
    {
		initState = enemyState;
        if (initState == eState.STATIC)
        {
			_animator.SetBool("isIdle", true);
		}

		float x = transform.localScale.x;
		facingRight = x == 1;
    }

    // Update is called once per frame
    void FixedUpdate() {

		if (life <= 0 && !isDead) {
			isDead = true;
			_animator.SetBool("IsDead", true);
			StartCoroutine(DestroyEnemy());
			return;
		}

        switch (enemyState)
        {
            case eState.idle:
				{
					break;
				}
            case eState.walking:
				{
					if (!_animator.GetBool("isRunning"))
					{
						_animator.SetBool("isRunning", true);
					}
					Walking();
					break;
				}
            case eState.meele:
				{
					PreAttackMelee();
					break;
				}
            case eState.range:
				{
					PreAttackRange();
					break;
				}
        }
       
    }

	private void Walking()
    {
		if (enemyState == eState.STATIC) return;
		isPlat = Physics2D.OverlapCircle(fallCheck.position, .2f, 1 << LayerMask.NameToLayer("Ground"));
		isObstacle = Physics2D.OverlapCircle(wallCheck.position, .2f, turnLayerMask);
		
		if (!isHitted && life > 0 && Mathf.Abs(rb.velocity.y) < 0.5f)
		{
			if (isPlat && !isObstacle && !isHitted)
			{
				if (facingRight)
				{
					rb.velocity = new Vector2(speed, rb.velocity.y);
				}
				else
				{
					rb.velocity = new Vector2(-speed, rb.velocity.y);
				}
			}
			else
			{
				Flip();
			}
		}
	}
	private void PreAttackMelee()
    {
		enemyState = eState.standBy;
		_animator.SetTrigger("Attacking");
		rb.velocity = Vector2.zero;
		
    }
	public void AttackMelee()
    {
		Collider2D[] colliders = Physics2D.OverlapCircleAll(attackCheck.position, 1);
        for (int i = 0; i < colliders.Length; i++)
        {
			colliders[i].TryGetComponent<IDamageable>(out IDamageable dmg);
			if(dmg != null && colliders[i].CompareTag("Player"))
            {
				Debug.Log("enemy hitting me");
				dmg.ApplyDamage(2, this.transform.position);
            }
        }
	}

	private void PreAttackRange()
    {
		enemyState = eState.standBy;
		_animator.SetTrigger("Throwing");
		rb.velocity = Vector2.zero;
	}

	private void AttackRange()
	{
		float direction = facingRight ? 1 : -1;
		GameObject p = Instantiate(projectile, this.transform.position,Quaternion.Euler(0,0, -90 * direction));
		ThrowableProjectile pScript = p.GetComponent<ThrowableProjectile>();
	
		Debug.Log(facingRight);
		pScript.enemyProjectile = true;
		pScript.direction = Vector2.right * direction;
		pScript.owner = this.gameObject;

		if (initState != eState.STATIC)
		{
			enemyState = eState.walking;
		}
		Destroy(p, 5);
	}

	void Flip() {
		// Switch the way the player is labelled as facing.
		facingRight = !facingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	public void ApplyDamage(float damage, Vector3 P_direction)
	{
		if (!isInvincible)
		{
			float direction = damage / Mathf.Abs(damage);
			if (direction < 0 && !facingRight)
			{
				Flip();
			}
			else if (direction > 0 && facingRight)
			{
				Flip();
			}
			if(enemyState == eState.idle && initState!=eState.STATIC)
            {
				enemyState = eState.walking;
            }
			damage = Mathf.Abs(damage);
			_animator.SetBool("Hit", true);
			Debug.Log("TESTING TEST");
			life -= damage;
			rb.velocity = Vector2.zero;
			rb.AddForce(new Vector2(direction * 500f, 100f));
			StartCoroutine(HitTime());
		}
	}
	public void PlaySlashEffect()
	{
		PS.Play();
		PS2.Play();
	}

    IEnumerator HitTime()
	{
		isHitted = true;
		//isInvincible = true;
		yield return new WaitForSeconds(0.05f);
		isHitted = false;
		//isInvincible = false;
	}

	IEnumerator DestroyEnemy()
	{
		LevelDataHolder.Instance.EnemiesKilled++;
		CapsuleCollider2D capsule = GetComponent<CapsuleCollider2D>();
		capsule.size = new Vector2(1f, 0.25f);
		capsule.offset = new Vector2(0f, -0.8f);
		capsule.direction = CapsuleDirection2D.Horizontal;
		yield return new WaitForSeconds(0.25f);
		rb.velocity = new Vector2(0, rb.velocity.y);
		yield return new WaitForSeconds(3f);
		Destroy(gameObject);
	}
}
public enum eState
{
	idle,
	walking,
	standBy,
	meele,
	range,
	STATIC
}

