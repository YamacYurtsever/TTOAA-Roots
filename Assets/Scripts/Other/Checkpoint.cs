using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    //public variables
    public int lives = 3;
    public Animator checkpointAnimator;
    public bool hasCheckpoint = false;

    //private variables
    private GameObject player;
    private PlayerHealthSystem healthSystem;
    private Collider2D playerCol, col;

    void Start()
    {
        //connections
        player = GameObject.FindGameObjectWithTag("Player");
        healthSystem = player.GetComponent<PlayerHealthSystem>();
        playerCol = player.GetComponent<Collider2D>();
        col = GetComponent<Collider2D>();
    }

    private void Update()
    {
        //open checkpoint and fill health
        if (col.IsTouching(playerCol) && hasCheckpoint == false)
        {
            healthSystem.currentHealth = healthSystem.maxHealth;
            healthSystem.healthText.text = healthSystem.currentHealth.ToString();
            healthSystem.healthBar.SetHealth(healthSystem.currentHealth);
            hasCheckpoint = true;
            checkpointAnimator.SetFloat("lives", lives);
        }
    }
}