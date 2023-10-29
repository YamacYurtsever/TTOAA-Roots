using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
	//public variables
	[Range(0, .3f)] public float movementSoothing = 0.05f;
	public float wallSlideSpeed = 0f;
	public float wallJumpTime = 0f;
	public float wallDistance = 0.5f;
	public float wallJumpForceY = 500f;
	public float wallJumpForceX = 400f;
	public float sideWallJumpForceX = 750f;
	public float jumpWaiter = 0.25f;
	public bool isGrounded;
	public LayerMask groundLayers;
	public GameObject dustParticle;
	public Transform groundCheck;

	//private variables
	private const float groundedRadius = 0.3f;
	private Rigidbody2D rb;
	private bool isFacingRight = true;
	private Vector3 velocity = Vector3.zero;
	private PlayerMovement playerMovement;
	private bool isWallSliding;
	private float jumpTime;
	private Collider2D wallCheckHit;
	private float jumpCounter = 0f;
	private Animator animator;
	private bool canJump = true;


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
		animator = GetComponent<Animator>();
	    playerMovement = GetComponent<PlayerMovement>();

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
			animator.SetBool("IsSliding", false);
			jumpTime = Time.time + wallJumpTime;
			if (!wasGrounded)
			{
				OnLandEvent.Invoke();
			}
		}

		//apply wall slide and jump
		wallCheckHit = Physics2D.OverlapPoint(new Vector2(transform.position.x + (wallDistance * transform.localScale.x), transform.position.y - 0.5f), groundLayers);

		if (wallCheckHit != null && !isGrounded && playerMovement.horizontalMove != 0f)
		{
			isWallSliding = true;
			Flip();
			animator.SetBool("IsSliding", true);
			jumpTime = Time.time + wallJumpTime;
		}

		else if (jumpTime < Time.time)
		{
			animator.SetBool("IsSliding", false);
			if (isWallSliding == true)
			{
				Flip();
				isWallSliding = false;
			}
		}

		if (isWallSliding)
		{
			rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlideSpeed, float.MaxValue));

			if (Input.GetButtonDown("Jump") && jumpCounter < 3f && canJump)
			{
				if (playerMovement.horizontalMove > 0)
				{
					if (isFacingRight)
					{
						rb.velocity = new Vector2(0, 0);
						rb.AddForce(new Vector2(sideWallJumpForceX, wallJumpForceY));
					}
					else
					{
						rb.velocity = new Vector2(0, 0);
						rb.AddForce(new Vector2(-wallJumpForceX, wallJumpForceY));
					}
				}
				else if (playerMovement.horizontalMove < 0)
				{
					if (isFacingRight)
					{
						rb.velocity = new Vector2(0, 0);
						rb.AddForce(new Vector2(wallJumpForceX, wallJumpForceY));
					}
					else
					{
						rb.velocity = new Vector2(0, 0);
						rb.AddForce(new Vector2(-sideWallJumpForceX, wallJumpForceY));
					}
				}

				Instantiate(dustParticle, new Vector2(transform.position.x, transform.position.y - 0.9f), transform.rotation);
				jumpCounter++;
				animator.SetBool("IsJumping", true);
			}
		}

		if (isGrounded)
		{
			jumpCounter = 0f;
		}
	}

	public void Move(float move)
	{
		//apply move
		Vector3 targetVelocity = new Vector2(move, rb.velocity.y);
		rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSoothing);

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
		if (isGrounded && jump && !isWallSliding && canJump)
		{
			rb.velocity = new Vector2(rb.velocity.x, 0f);
			rb.AddForce(new Vector2(0f, jumpForce));
			Instantiate(dustParticle, new Vector2(transform.position.x, transform.position.y - 0.9f), transform.rotation);
		}
	}

	private void Flip()
	{
		//apply flip
		isFacingRight = !isFacingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}