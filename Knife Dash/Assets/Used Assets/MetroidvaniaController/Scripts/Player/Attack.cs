using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[DefaultExecutionOrder(1)]
public class Attack : MonoBehaviour
{
	public float dmgValue = 4;
	public GameObject throwableObject;
	public Collider2D groundSafety;
	public ParticleSystem Trail;
	public Transform attackCheck;
	private Rigidbody2D m_Rigidbody2D;
	private Collider2D m_Collider2D;
	public Animator animator;
	public bool canAttack = true;

	public bool canThrowKnife = true;
	public bool isTimeToCheck = false;
	public bool PerformingTeleport = false;
	[SerializeField] float teleportspeed;
	[SerializeField] float knifeCooldDown;
	[SerializeField] LayerMask groundLayer;
	[SerializeField] LayerMask groundobjects;

	private CamShake shakeCam;

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
		m_Collider2D = GetComponent<Collider2D>();
		shakeCam = FindObjectOfType<CamShake>();
		if(PlayerInputs.Instance)
        {
			PlayerInputs.Instance.myAttack = this;
        }
	}
    void Update()
    {
		
	}

	IEnumerator AttackCooldown()
	{
		yield return new WaitForSeconds(0.25f);
		canAttack = true;
	}

	public void DoDashDamage()
	{
		dmgValue = Mathf.Abs(dmgValue);
		Collider2D[] collidersEnemies = Physics2D.OverlapCircleAll(attackCheck.position, 0.9f);
		for (int i = 0; i < collidersEnemies.Length; i++)
		{
			if (collidersEnemies[i].gameObject.tag == "Enemy")
			{
				if (collidersEnemies[i].transform.position.x - transform.position.x < 0)
				{
					dmgValue = -dmgValue;
				}
				collidersEnemies[i].gameObject.SendMessage("ApplyDamage", dmgValue);
				shakeCam.GetComponent<CamShake>().ShakeCamera();
			}
		}
	}



    #region Changes

	public void OnAttack()
    {
		if (canAttack)
		{
			canAttack = false;
			animator.SetBool("IsAttacking", true);
			StartCoroutine(AttackCooldown());
		}
	}
	bool canTeleport = false;
	Coroutine knifeCD;
	Coroutine textTimer;
	public void OnFireUI()
	{
		//if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;

		if (canThrowKnife)
		{
			ThrowKnife();
			UIManager.Instance.knifeButtonToggle();
			return;
		}

		if (canTeleport)
		{
			if (knifeCD != null)
			{
				StopCoroutine(knifeCD);
			}
			StartCoroutine(TeleportPlayer());
		}
	}

	RaycastHit2D ObjectsRaycast;
	Vector3 ObjectSafetyChecker = new Vector3();
	public void ThrowKnife()
    {
		
		canThrowKnife = false;
		canTeleport = true;
		animator.SetBool("IsAttacking", true);
		
        Vector2 direction = new Vector2(transform.localScale.x, 0);
		RaycastHit2D GroundRaycast = Physics2D.Raycast(this.transform.position, direction ,50, groundLayer);
		ObjectsRaycast = Physics2D.Raycast(this.transform.position, direction ,50, groundobjects);
		ObjectSafetyChecker = ObjectsRaycast ? ObjectsRaycast.point - direction * 0.4f : (Vector2)transform.position + direction * 30;


        #region setting up knife
		throwableObject.SetActive(true);
		throwableObject.transform.position = transform.position + new Vector3(0f, -0.2f);
		throwableObject.transform.localScale = Vector2.up - direction;
		throwableObject.GetComponent<ThrowableProjectile>().hasHit = false;
		throwableObject.GetComponent<ThrowableProjectile>().direction = direction;

		throwableObject.GetComponent<ThrowableProjectile>().endPos = GroundRaycast? GroundRaycast.point - direction * 0.4f : (Vector2)transform.position + direction * 30;
		Debug.Log(GroundRaycast.point + " , " + direction);
		
        #endregion
        if (knifeCD != null)
		{
			StopCoroutine(knifeCD);
		}
		knifeCD = StartCoroutine(KnifeCooldown(1.5f, 0.5f)); // when knife never got stuck
	}



	IEnumerator TeleportPlayer()
    {
		Trail.Play();
		PlayerInputs.canControl = false;
		PlayerInputs.Instance.myCC.invincible = true;
		PlayerInputs.Instance.myCC.canMove = false;
		m_Collider2D.enabled = false;
		groundSafety.enabled = false;
		m_Rigidbody2D.gravityScale = 0;
		PerformingTeleport = true;

        #region flip
        Vector3 targetPos = throwableObject.transform.position;
		if (((targetPos.x - transform.position.x) > 0 && !PlayerInputs.Instance.myCC.m_FacingRight) ||
			((targetPos.x - transform.position.x) < 0 && PlayerInputs.Instance.myCC.m_FacingRight))
        {
			PlayerInputs.Instance.myCC.Flip();

		}
		#endregion


		TeleportRaycast(transform.position, targetPos); // damage targets

        #region Teleporting player
        float timer = 0;
        while (Vector2.Distance(targetPos, this.transform.position) > 3f && timer < 1)
        {
            timer += Time.fixedDeltaTime;
            Vector3 startPos = m_Rigidbody2D.position;

            m_Rigidbody2D.MovePosition(m_Rigidbody2D.position + (teleportspeed * Time.fixedDeltaTime * ((Vector2)throwableObject.transform.position - m_Rigidbody2D.position)));
            //transform.Translate((targetPos - startPos) * Time.deltaTime, Space.Self);
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }
		groundSafety.enabled = true;
		if (throwableObject.GetComponent<Rigidbody2D>().IsTouchingLayers(groundobjects))
        {
			Debug.Log("Touching stone");
			transform.position = ObjectSafetyChecker;
        }
		else
        {
			Debug.Log("Touching wall");
			transform.position = targetPos;
        }
		#endregion

		PlayerInputs.Instance.myCC.invincible = false;
		PerformingTeleport = false;

		if(knifeCD != null)
        {
			StopCoroutine(knifeCD);
        }
		StartCoroutine(KnifeCooldown(0, knifeCooldDown)); // after using knife to teleport

		float drag = m_Rigidbody2D.drag;
		m_Rigidbody2D.drag = 1000;
		m_Collider2D.enabled = true;
		m_Rigidbody2D.gravityScale = 5;


		yield return new WaitForSeconds(0.5f);
		PlayerInputs.Instance.myCC.canMove = true;
		m_Rigidbody2D.drag = drag;
		PlayerInputs.canControl = true;
		Trail.Stop();
	}
	public void KnifeStuck()
    {
		if (knifeCD! != null)
		{
			StopCoroutine(knifeCD);
		}
		knifeCD = StartCoroutine(KnifeCooldown(10, 5)); //when knife is stuck to wall
    }

	private void TeleportRaycast(Vector3 startPos , Vector3 EndPos)
    {
		RaycastHit2D[] hit = Physics2D.RaycastAll(startPos, EndPos - startPos,Vector2.Distance(EndPos ,startPos));
		Debug.DrawLine(startPos, EndPos, Color.red, 5);
        for (int i = 0; i < hit.Length; i++)
        {
			int temp = i;
			if (hit[temp].collider != null)
			{
				hit[temp].collider.TryGetComponent<IDamageable>(out IDamageable dm);
				if (dm != null)
				{
					if(hit[temp].collider.CompareTag("Enemy"))
                    {
						hit[temp].collider.gameObject.layer = LayerMask.NameToLayer("IgnorePlayer");
						hit[temp].collider.gameObject.SendMessage("PlaySlashEffect", 100f);
						shakeCam.GetComponent<CamShake>().ShakeCamera();
					}
					Debug.Log("killing enemy" + hit[temp].collider.gameObject.name);
					dm.ApplyDamage(100, startPos);
				}
				if(hit[temp].collider.CompareTag("Rope"))
                {
					hit[temp].collider.GetComponentInParent<Girl>().FreeSelf();
                }
				else if(hit[temp].collider.CompareTag("Girl"))
                {
					hit[temp].collider.GetComponent<Girl>().FreeSelf();
				}

				/*hit[temp].collider.TryGetComponent<AdLaser>(out AdLaser laser);
				if (laser != null)
				{
					laser.OnPlayerHit(GetComponent<CharacterController2D>());
				}*/
			}
		}
		
	}

	public void RetrieveKnife()
    {
		if (knifeCD! != null)
		{
			StopCoroutine(knifeCD);
		}
		knifeCD = StartCoroutine(KnifeCooldown(0, 5)); // when knife Retrieved manually
	}

	IEnumerator KnifeCooldown(float DisableDelay , float enableDelay)
	{
        yield return new WaitForSeconds(DisableDelay);

		Debug.Log("knife disabled");
		throwableObject.SetActive(false);
		UIManager.Instance.knifeButtonToggle();
		canTeleport = false;

        if (textTimer != null)
        {
            StopCoroutine(textTimer);
        }
        textTimer = StartCoroutine(UIManager.Instance.KnifeCDTimer(enableDelay));

		yield return new WaitForSeconds(enableDelay);
        Debug.Log("can throw now");

		canThrowKnife = true;
	}
    #endregion


	public void Temp()
    {
		
    }
}
