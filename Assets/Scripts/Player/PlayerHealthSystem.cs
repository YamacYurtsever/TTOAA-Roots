using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHealthSystem : MonoBehaviour
{
    //public variables
    public int currentHealth;
    public int maxHealth = 500;
    public float deathAnimationTime = 0.5f;
    public PlayerHealthBar healthBar;
    public TextMeshProUGUI healthText;
    public Checkpoint checkpoint;

    //private variables
    private Animator animator;
    private GameObject healthTextObject;
    private Rigidbody2D rb;

    private void Start()
    {
        //connections
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        healthTextObject = GameObject.FindGameObjectWithTag("PlayerHealthText");
        healthText = healthTextObject.GetComponent<TextMeshProUGUI>();
        healthText.text = currentHealth.ToString();
        rb = gameObject.GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        //take damage
        currentHealth -= damage;
        animator.SetTrigger("Hurt");
        if (currentHealth <= 0)
        {
            healthText.text = "0";
            healthBar.SetHealth(0);
            StartCoroutine(Die());
        }
        else
        {
            healthText.text = currentHealth.ToString();
            healthBar.SetHealth(currentHealth);
        }
    }

    public void IncreaseHealth(int healthIncrease)
    {
        //increase health
        currentHealth += healthIncrease;
        healthText.text = currentHealth.ToString();
        healthBar.SetHealth(currentHealth);
    }

    IEnumerator Die()
    {
        //start destroying game object
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        transform.gameObject.GetComponent<Collider2D>().enabled = false;
        transform.gameObject.GetComponent<PlayerController>().enabled = false;
        transform.gameObject.GetComponent<PlayerMovement>().enabled = false;
        transform.gameObject.GetComponent<PlayerAttack>().enabled = false;
        animator.SetBool("IsDead", true);
        yield return new WaitForSeconds(deathAnimationTime);

        if (checkpoint.hasCheckpoint == true && checkpoint.lives > 1)
        {
            //revive
            transform.gameObject.GetComponent<Collider2D>().enabled = true;
            transform.gameObject.GetComponent<PlayerController>().enabled = true;
            transform.gameObject.GetComponent<PlayerMovement>().enabled = true;
            transform.gameObject.GetComponent<PlayerAttack>().enabled = true;
            gameObject.transform.position = checkpoint.gameObject.transform.position;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            checkpoint.lives--;
            checkpoint.checkpointAnimator.SetFloat("lives", checkpoint.lives);
            currentHealth = maxHealth;
            healthBar.SetHealth(maxHealth);
            healthText.text = maxHealth.ToString();
            animator.SetBool("IsDead", false);
        }
        else
        {
            //destroy
            Destroy(gameObject);
            SceneLoader.RestartScene();
        }
    }
}