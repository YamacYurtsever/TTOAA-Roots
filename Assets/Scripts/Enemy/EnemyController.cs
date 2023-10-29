using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyController : MonoBehaviour
{
	//public variables
	[Range(0, .3f)] public float movementSoothing = 0.05f;
	public bool isGrounded;
	public LayerMask groundLayers;
	public Transform groundCheck;
	public Animator animator;
	public GameObject healthBar;

	//private variables
	private const float groundedRadius = 0.2f;
	private Rigidbody2D rb;
	private bool isFacingRight = true;
	private Vector3 velocity = Vector3.zero;

	//events
	[Header("Events")]
	[Space]
	public UnityEvent OnLandEvent;
	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	private void Awake()
	{
		//connections
		rb = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
		{
			OnLandEvent = new UnityEvent();
		}
	}

	private void Update()
	{
		//connections
		bool wasGrounded = isGrounded;
		isGrounded = false;

		//apply land
		bool touchingGround = Physics2D.OverlapCircle(groundCheck.position, groundedRadius, groundLayers);

		if (touchingGround)
		{
			isGrounded = true;
			if (!wasGrounded)
			{
				OnLandEvent.Invoke();
			}
		}
	}

	public void Move(float move)
	{
		Vector3 targetVelocity = new Vector2(move, rb.velocity.y);
		rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSoothing);

		//apply move
		if (move > 0 && !isFacingRight)
		{
			Flip();
		}
		else if (move < 0 && isFacingRight)
		{
			Flip();
		}
	}

	public void Jump(bool jump, float jumpForce)
	{
		//apply jump
		if (isGrounded && jump)
		{
			rb.AddForce(new Vector2(0f, jumpForce));
		}
	}

	private void Flip()
	{
		//apply flip
		isFacingRight = !isFacingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
		healthBar.transform.localScale = new Vector2(healthBar.transform.localScale.x * -1, healthBar.transform.localScale.y);
	}
}