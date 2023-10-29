using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //public variables
    public float runSpeed = 100f;
    public float jumpForce = 400f;
    public float horizontalMove = 0f;
    public bool attacking = false;

    //private variables
    private PlayerController controller;
    private Animator animator;
    private bool jump = false;

    private void Start()
    {
        //connections
        controller = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!attacking)
        {
            //input move
            horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
            animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

            //input jump
            if (Input.GetButtonDown("Jump"))
            {
                jump = true;
                animator.SetBool("IsJumping", true);
            }
        }
    }

    private void FixedUpdate()
    {
        //apply move
        controller.Move(horizontalMove * Time.fixedDeltaTime);
        controller.Jump(jump, jumpForce);

        //stop jump
        jump = false;
    }

    public void OnLanding()
    {
        //apply land
        animator.SetBool("IsJumping", false);
    }
}