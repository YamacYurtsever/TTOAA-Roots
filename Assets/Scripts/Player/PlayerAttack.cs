using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    //public variables
    public int attackDamage = 25;
    public float attackRange = 0.5f;
    public float attackRate = 2f;
    public float sideKnockbackStrength = 1f;
    public float upKnockbackStrength = 1f;
    public float attackAnimationLength = 1f;
    public LayerMask enemyLayers;
    public Transform attackPoint;

    //private variables
    private Animator animator;
    private PlayerMovement movement;
    private float nextAttackTime = 0f;

    private void Start()
    {
        //connections
        animator = GetComponent<Animator>();
        movement = GetComponent<PlayerMovement>(); 
    }

    private void Update()
    {
        //input attack
        if (Time.time >= nextAttackTime && Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(Attack());
            nextAttackTime = Time.time + 1f / attackRate;
        }
    }

    IEnumerator Attack()
    {
        //apply attack
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(attackAnimationLength);
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyHealthSystem>().TakeDamage(attackDamage);
            float direction = Mathf.Sign(enemy.transform.position.x - transform.position.x);
            Rigidbody2D enemyRigidbody = enemy.GetComponent<Rigidbody2D>();
            enemyRigidbody.velocity = new Vector2(0, 0);
            enemyRigidbody.AddForce(new Vector2(direction * sideKnockbackStrength, upKnockbackStrength), ForceMode2D.Impulse);
            StartCoroutine(Disabler(enemy));
        }
    }

    IEnumerator Disabler(Collider2D enemy)
    {
        //disable enemy when hit
        enemy.GetComponent<EnemyAttack>().enabled = false;
        movement.attacking = true;
        yield return new WaitForSeconds(0.25f);
        enemy.GetComponent<EnemyAttack>().enabled = true;
        movement.attacking = false;
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