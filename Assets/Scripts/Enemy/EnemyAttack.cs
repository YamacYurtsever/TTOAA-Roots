using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    //public variables
    public int attackDamage = 25;
    public float attackRange = 0.5f;
    public float attackRate = 2f;
    public float attackAnimationLength = 1f;
    public float sideKnockbackStrength = 1f;
    public float upKnockbackStrength = 1f;
    public int touchAttackDamage = 10;
    public float touchSideKnockbackStrength = 1f;
    public float touchUpKnockbackStrength = 1f;
    public LayerMask playerLayers;
    public Animator animator;
    public Transform attackPoint;

    //private variables
    private float nextAttackTime = 0f;
    private Collider2D detection;
    private Collider2D col, playerCol;
    private Rigidbody2D rb;
    private GameObject player;

    private void Start()
    {
        //connections
        col = gameObject.GetComponent<Collider2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerCol = player.GetComponent<Collider2D>();
        rb = gameObject.GetComponent<Rigidbody2D>();

        InvokeRepeating("Touch", 0f, 0.5f);
    }

    private void Update()
    {
        //connections
        detection = Physics2D.OverlapPoint(new Vector2(transform.position.x + transform.localScale.x * 0.5f, transform.position.y - 0.5f), playerLayers);

        //attack input
        if (Time.time >= nextAttackTime)
        {
            if (detection!= null)
            {
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    private void Touch()
    {
        //apply touch damage
        if (col.IsTouching(playerCol))
        {
            player.GetComponent<PlayerHealthSystem>().TakeDamage(touchAttackDamage);
            float direction = player.transform.position.x - transform.position.x;
            Rigidbody2D playerRigidbody = player.GetComponent<Rigidbody2D>();
            playerRigidbody.AddForce(new Vector2(direction * touchSideKnockbackStrength, touchUpKnockbackStrength), ForceMode2D.Impulse);
            StartCoroutine(Disabler(playerCol));
        }
    }

    private void Attack()
    {
        //apply attack
        StartCoroutine(AttackAnimation());
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayers);
        foreach (Collider2D player in hitPlayers)
        {
            player.GetComponent<PlayerHealthSystem>().TakeDamage(attackDamage);
            float direction = player.transform.position.x - transform.position.x;
            Rigidbody2D playerRigidbody = player.GetComponent<Rigidbody2D>();
            playerRigidbody.velocity = new Vector2(0, 0);
            playerRigidbody.AddForce(new Vector2(direction * sideKnockbackStrength, upKnockbackStrength), ForceMode2D.Impulse);
            StartCoroutine(Disabler(player));
        }
    }

    IEnumerator AttackAnimation()
    {
        //wait for attack animation
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(attackAnimationLength);
        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    IEnumerator Disabler(Collider2D player)
    {
        //stop moving when hit
        player.GetComponent<PlayerAttack>().enabled = false;
        transform.gameObject.GetComponent<EnemyAI>().enabled = false;
        yield return new WaitForSeconds(0.25f);
        player.GetComponent<PlayerAttack>().enabled = true;
        transform.gameObject.GetComponent<EnemyAI>().enabled = true;
    }

    private void OnDrawGizmosSelected()
    {
        //draw attack sphere
        if (attackPoint == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}