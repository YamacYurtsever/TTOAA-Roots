using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    //public Variables
    public GameObject enemies;
    public Collider2D playerCol;

    //private Variables
    private bool doorIsOpen = false;
    private float allEnemies;
    private float currentEnemies;
    private Collider2D col;
    private Animator animator;

    void Start()
    {
        //connections
        allEnemies = enemies.transform.childCount;
        col = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        //open the door
        currentEnemies = enemies.transform.childCount;

        if (currentEnemies <= allEnemies / 5)
        {
            animator.SetBool("isOpen", true);
            doorIsOpen = true;
            if (col.IsTouching(playerCol) && doorIsOpen)
            {
                SceneLoader.LoadNextScene();
            }
        }
    }
}