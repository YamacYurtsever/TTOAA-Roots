using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : MonoBehaviour
{
    //public variables
    public int healingAmount = 50;

    //private variables
    private GameObject player;
    private PlayerHealthSystem healthSystem;
    private Collider2D col;

    void Start()
    {
        //connections
        player = GameObject.FindGameObjectWithTag("Player");
        healthSystem = player.GetComponent<PlayerHealthSystem>();
        col = GetComponent<Collider2D>();
    }

    private void Update()
    {
        //increase health
        if (col.IsTouching(player.GetComponent<Collider2D>()))
        {
            healthSystem.IncreaseHealth(healingAmount);
            if (healthSystem.currentHealth > healthSystem.maxHealth)
            {
                healthSystem.currentHealth = healthSystem.maxHealth;
                healthSystem.healthText.text = healthSystem.currentHealth.ToString();
                healthSystem.healthBar.SetHealth(healthSystem.currentHealth);
            }
            Destroy(gameObject);
        }
    }
}
