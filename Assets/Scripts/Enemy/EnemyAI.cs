using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyAI : MonoBehaviour
{
    //public variables
    public float speed = 200f;
    public float jumpForce = 15f;
    public float exclamationJumpForce = 5f;
    public float exclamationWaitTime = 0.5f;
    public float nextWaypointDistance = 3f;
    public float sideDetectionRange = 1f;
    public bool isChecked = false;
    public bool follow = false;
    public bool isSkill = false;
    public LayerMask blockLayers;
    public LayerMask playerLayers;
    public Transform target;
    public Animator animator;
    public GameObject exclamation;
    public EnemyController controller;

    //private variables
    private GameObject player;
    private Rigidbody2D rb;
    private Collider2D sideDetection;
    private Collider2D blocked;
    private Collider2D detectionArea, notDetectionArea;

    void Start()
    {
        //connections   
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        //connections
        detectionArea = Physics2D.OverlapArea(new Vector2(transform.position.x - 5, transform.position.y + 1), new Vector2(transform.position.x + 5, transform.position.y - 2), playerLayers);
        notDetectionArea = Physics2D.OverlapArea(new Vector2(transform.position.x - 15, transform.position.y + 3), new Vector2(transform.position.x + 15, transform.position.y - 4), playerLayers);

        if (controller.isGrounded)
        {
            if (detectionArea != null)
            {
                follow = true;
            }

            else if (notDetectionArea == null)
            {
                isChecked = false;
                follow = false;
                isSkill = false;
            }
        }

        Move();

        Jump();
    }



    public void Move()
    {
        //detect when in range
        if (isChecked == false && follow == true && isSkill == false)
        {
            controller.Jump(true, exclamationJumpForce);
            animator.SetBool("IsJumping", true);
            StartCoroutine(ExclamationCreatorNormal());
            isChecked = true;
        }

        //detect when hit with skill
        if (isChecked == false && follow == false && isSkill == true)
        {
            StartCoroutine(ExclamationCreatorSkill());
            isChecked = true;
        }

        //apply move
        if (controller.isGrounded)
        {
            if (isChecked == true)
            {
                float direction = Mathf.Sign(player.transform.position.x - transform.position.x);
                float force = direction * speed;
                animator.SetFloat("Speed", transform.localScale.x * rb.velocity.x);
                controller.Move(force * Time.deltaTime);
            }
            else
            {
                rb.velocity = new Vector2(0, 0);
                animator.SetFloat("Speed", 0);
            }
        }
    }

    private void Jump()
    {
        //apply jump
        sideDetection = Physics2D.OverlapArea(new Vector2(transform.position.x, transform.position.y - 0.5f), new Vector2(transform.position.x + transform.localScale.x * sideDetectionRange, transform.position.y - 0.5f), blockLayers);
        blocked = Physics2D.OverlapPoint(new Vector2(transform.position.x + transform.localScale.x * sideDetectionRange, transform.position.y + 0.5f), blockLayers);

        //jump
        if (sideDetection != null && notDetectionArea != null && blocked == null && controller.isGrounded)
        {
            controller.Jump(true, jumpForce);
            animator.SetBool("IsJumping", true);
        }

        //road blocked
        else if (sideDetection != null && blocked != null && controller.isGrounded)
        {
            rb.velocity = new Vector2(0, 0);
            animator.SetBool("IsJumping", false);
            animator.SetFloat("Speed", 0);
        }
    }

    IEnumerator ExclamationCreatorNormal()
    {
        //create exclamation
        GameObject exc = Instantiate(exclamation);
        exc.transform.position = new Vector2(transform.position.x, transform.position.y + 0.8f);
        yield return new WaitForSeconds(exclamationWaitTime);
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        Destroy(exc);
    }

    IEnumerator ExclamationCreatorSkill()
    {
        //create exclamation
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        GameObject exc = Instantiate(exclamation);
        exc.transform.position = new Vector2(transform.position.x, transform.position.y + 0.8f);
        yield return new WaitForSeconds(0.5f);
        Destroy(exc);
    }

    public void OnLanding()
    {
        //apply land
        animator.SetBool("IsJumping", false);
    }
}