using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
[DefaultExecutionOrder(2)]
public class PlayerMovement : MonoBehaviour {

	public CharacterController2D controller;
	public Animator animator;
	private PlayerInputs _input;
	private Rigidbody2D _rb2d;
	public float runSpeed = 40f;

	float horizontalMove = 0f;
	bool jump = false;
	bool dash = false;
	public bool useKeyboard;

	//bool dashAxis = false;


	private void Awake()
	{
		if (PlayerInputs.Instance)
		{
			_input = PlayerInputs.Instance;
			_input.myPlayer = this;
		}
		_rb2d = GetComponent<Rigidbody2D>();
	}
	// Update is called once per frame
	float speed = 0;
	void Update() 
	{


		if (!useKeyboard)
		{
			if (_input.LeftPressed())
			{
				horizontalMove = -1 * runSpeed;
			}
			else if (_input.RightPressed())
			{
				horizontalMove = 1 * runSpeed;
			}
			else
			{
				horizontalMove = 0;
			}
		}
		//horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
		if (Mathf.Abs(_rb2d.velocity.x) > 0.1f)
		{
			 speed = Mathf.Sign(_rb2d.velocity.x) * runSpeed;
		}
		else
        {
			speed = 0;
        }
		animator.SetFloat("Speed", Mathf.Abs(speed));




		/*if (Input.GetKeyDown(KeyCode.Z))
		{
			jump = true;
		}*/

		/*if (Input.GetKeyDown(KeyCode.C))
		{
			dash = true;
		}*/

		/*if (Input.GetAxisRaw("Dash") == 1 || Input.GetAxisRaw("Dash") == -1) //RT in Unity 2017 = -1, RT in Unity 2019 = 1
		{
			if (dashAxis == false)
			{
				dashAxis = true;
				dash = true;
			}
		}
		else
		{
			dashAxis = false;
		}
		*/

	}

	public void OnFall()
	{
		animator.SetBool("IsJumping", true);
	}

	public void OnLanding()
	{
		animator.SetBool("IsJumping", false);
		animator.SetBool("IsDoubleJumping", false);
	}

	void FixedUpdate()
	{
		// Move our character
		if (PlayerInputs.canControl)
		{
			controller.Move(horizontalMove * Time.fixedDeltaTime, jump, dash);
		}
		else
		{
			controller.Move(0, false, false);
		}
		jump = false;
		dash = false;
	}

	#region mychanges
	public void OnJump()
	{
		//if (!PlayerInputs.canControl) return;
		jump = true;
	}

	public void OnMove(InputValue value)
	{
		
		
		if (value.Get<Vector2>().magnitude > 0) { useKeyboard = true; }
		horizontalMove = (int)value.Get<Vector2>().x * runSpeed;
	}

	public void OnDash()
    {
		//if (!PlayerInputs.canControl) return;
		dash = true;
    }

    
    #endregion
}
