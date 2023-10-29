using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthSystem : MonoBehaviour
{
    //public variables
    public int maxHealth = 100;
    public float deathAnimationTime = 0.5f;
    public bool hitBySkill2 = false;
    public EnemyHealthBar healthBar;

    //private variables
    private int currentHealth;
    private Animator animator;
    private Rigidbody2D rb;

    private void Start()
    {
        //connections
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int damage)
    {
        //take damage
        currentHealth -= damage;
        animator.SetTrigger("Hurt");
        if (currentHealth <= 0)
        {
            Die();
        }
        healthBar.SetHealth(currentHealth);
    }

    private void Die()
    {
        //start destroying game object
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        rb.gravityScale = 0f;
        transform.gameObject.GetComponent<Collider2D>().enabled = false;
        transform.gameObject.GetComponent<EnemyController>().enabled = false;
        transform.gameObject.GetComponent<EnemyAI>().enabled = false;
        transform.gameObject.GetComponent<EnemyAttack>().enabled = false;
        animator.SetBool("IsDead", true);
        StartCoroutine(Destroyer());
    }

    IEnumerator Destroyer()
    {
        //destroy game object
        yield return new WaitForSeconds(deathAnimationTime);
        Destroy(gameObject);
    }
}