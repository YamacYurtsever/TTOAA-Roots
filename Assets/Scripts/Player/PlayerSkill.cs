using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    //general variables

    private GameObject player;
    private Animator animator;
    private PlayerController controller;
    private LayerMask blockLayer;
    private LayerMask enemyLayer;
    private bool firstW = true, firstE = true;

    //skill 1 variables
    [Header("Skill 1")]

    public GameObject skill1Sprite;
    public float skill1Cooldown = 5f;
    public float skill1Speed = 5f;
    public int skill1Damage = 30;
    public float skill1SideKnockbackStrength = 5f;
    public float skill1UpKnockbackStrength = 5f;
    public bool openW = false;

    private GameObject skill1;
    private float skill1Timer = 0f;
    private bool skill1Destroyed;
    private bool canPressW = true;
    private PlayerMovement movement;

    //skill 3 variables
    [Header("Skill 2")]

    public GameObject skill2Sprite;
    public float skill2Cooldown = 15f;
    public float skill2Speed = 10f;
    public int skill2Damage = 80;
    public float skill2SideKnockbackStrength = 5f;
    public float skill2UpKnockbackStrength = 5f;
    public GameObject enemyStorage;
    public bool openE = false;

    private GameObject skill2;
    private float skill2Timer = 0f;
    private Collider2D bottomChecker, sideChecker;
    private bool skill2Destroyed;
    private bool canPressE = true;


    private void Start()
    {
        //connections
        player = gameObject;
        animator = player.GetComponent<Animator>();
        controller = player.GetComponent<PlayerController>();
        movement = player.GetComponent<PlayerMovement>();
        blockLayer = LayerMask.GetMask("Blocks");
        enemyLayer = LayerMask.GetMask("Enemy");
    }

    private void Update()
    {
        //connections
        bottomChecker = Physics2D.OverlapPoint(new Vector2(player.transform.position.x + player.transform.localScale.x / 2, player.transform.position.y - 1f), blockLayer);
        sideChecker = Physics2D.OverlapPoint(new Vector2(player.transform.position.x + player.transform.localScale.x / 2, player.transform.position.y), blockLayer);

        //input skill 1
        if (openW && Input.GetKeyDown(KeyCode.W) && canPressW)
        {
            if (Time.time > skill1Timer + skill1Cooldown || firstW)
            {
                animator.SetTrigger("Skill 1");
                ThrowSkill1();
                skill1Timer = Time.time;
                firstW = false;
            }
        }

        //input skill 2
        else if (openE && Input.GetKeyDown(KeyCode.E) && canPressE && controller.isGrounded && !sideChecker && bottomChecker)
        {
            if (Time.time > skill2Timer + skill2Cooldown || firstE)
            {
                for (int i = 0; i < enemyStorage.transform.childCount; i++)
                {
                    enemyStorage.transform.GetChild(i).GetComponent<EnemyHealthSystem>().hitBySkill2 = false;
                }
                animator.SetTrigger("Skill 2");
                ThrowSkill2();
                skill2Timer = Time.time;
                firstE = false;
            }
        }
    }

    //Skill 1 
    private void ThrowSkill1()
    {
        skill1 = Instantiate(skill1Sprite);
        skill1.transform.position = new Vector2(player.transform.position.x + player.transform.localScale.x / 2, player.transform.position.y - 0.5f);

        skill1.GetComponent<Rigidbody2D>().velocity = new Vector2(skill1Speed * player.transform.localScale.x, 0);
        skill1.transform.localScale = new Vector2(player.transform.localScale.x, 1);

        Collider2D waterCannonCollider = skill1.GetComponent<Collider2D>();
        StartCoroutine(CheckSkill1Hit(waterCannonCollider));
    }

    private IEnumerator CheckSkill1Hit(Collider2D waterCannonCollider)
    {

        if (waterCannonCollider.IsTouchingLayers(enemyLayer) || waterCannonCollider.IsTouchingLayers(blockLayer))
        {
            Collider2D enemy = Physics2D.OverlapCircle(waterCannonCollider.bounds.center, 0.5f, enemyLayer);

            if (enemy != null)
            {
                enemy.GetComponent<EnemyHealthSystem>().TakeDamage(skill1Damage);
                enemy.GetComponent<Animator>().SetTrigger("Hurt");
                float direction = enemy.transform.position.x - transform.position.x;
                Rigidbody2D enemyRigidbody = enemy.GetComponent<Rigidbody2D>();
                EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
                if (enemyAI.isChecked == true)
                {
                    enemyRigidbody.velocity = new Vector2(0, 0);
                    enemyRigidbody.AddForce(new Vector2(direction * skill1SideKnockbackStrength, skill1UpKnockbackStrength), ForceMode2D.Impulse);
                }
                else
                {
                    enemyRigidbody.velocity = new Vector2(0, 0);
                    enemyRigidbody.AddForce(new Vector2(direction * skill1SideKnockbackStrength / 7f, skill1UpKnockbackStrength), ForceMode2D.Impulse);
                    enemyAI.isSkill = true;
                }
                StartCoroutine(Disabler(enemy));
            }

            Destroy(skill1);
            skill1Destroyed = true;
        }

        yield return new WaitForSeconds(0.02f);

        if (skill1Destroyed)
        {
            skill1Destroyed = false;
        }

        else
        {
            yield return StartCoroutine(CheckSkill1Hit(waterCannonCollider));
        }
    }

    //Skill 2
    private void ThrowSkill2()
    {
        skill2 = Instantiate(skill2Sprite);
        skill2.transform.position = new Vector2(player.transform.position.x + player.transform.localScale.x / 2, player.transform.position.y - 0.5f);

        skill2.GetComponent<Rigidbody2D>().velocity = new Vector2(skill2Speed * player.transform.localScale.x, 0);
        skill2.transform.localScale = new Vector2(player.transform.localScale.x, 1);

        Collider2D skill2Collider = skill2.GetComponent<Collider2D>();
        StartCoroutine(CheckSkill2Hit(skill2Collider));
    }

    private IEnumerator CheckSkill2Hit(Collider2D skill2Collider)
    {
        bottomChecker = Physics2D.OverlapPoint(new Vector2(skill2.transform.position.x + skill2.transform.localScale.x / 2, skill2.transform.position.y - 1f), blockLayer);
        sideChecker = Physics2D.OverlapPoint(new Vector2(skill2.transform.position.x + skill2.transform.localScale.x / 2, skill2.transform.position.y), blockLayer);

        if (skill2Collider.IsTouchingLayers(enemyLayer))
        {
            Collider2D enemy = Physics2D.OverlapCircle(skill2Collider.bounds.center, 0.5f, enemyLayer);

            if (enemy != null && enemy.GetComponent<EnemyHealthSystem>().hitBySkill2 == false)
            {
                enemy.GetComponent<EnemyHealthSystem>().hitBySkill2 = true;
                enemy.GetComponent<EnemyHealthSystem>().TakeDamage(skill2Damage);
                enemy.GetComponent<Animator>().SetTrigger("Hurt");
                float direction = enemy.transform.position.x - transform.position.x;
                Rigidbody2D enemyRigidbody = enemy.GetComponent<Rigidbody2D>();
                EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
                if (enemyAI.isChecked == true)
                {
                    enemyRigidbody.AddForce(new Vector2(direction * skill2SideKnockbackStrength, skill2UpKnockbackStrength), ForceMode2D.Impulse);
                }
                else
                {
                    enemyRigidbody.AddForce(new Vector2(direction * skill2SideKnockbackStrength / 7f, skill2UpKnockbackStrength), ForceMode2D.Impulse);
                    enemyAI.isSkill = true;
                }
                StartCoroutine(Disabler(enemy));
            }
        }

        if (sideChecker != null || bottomChecker == null)
        {
            Destroy(skill2);
            skill2Destroyed = true;
        }

        yield return new WaitForSeconds(0.02f);

        if (skill2Destroyed)
        {
            skill2Destroyed = false;
        }

        else
        {
            yield return StartCoroutine(CheckSkill2Hit(skill2Collider));
        }
    }

    //Disabler
    private IEnumerator Disabler(Collider2D enemy)
    {
        enemy.GetComponent<EnemyAttack>().enabled = false;
        movement.attacking = true;
        yield return new WaitForSeconds(0.25f);
        enemy.GetComponent<EnemyAttack>().enabled = true;
        movement.attacking = false;
    }

    //Get Cooldowns
    public float GetSkill1Time()
    {
        if (firstW)
        {
            return skill1Cooldown;
        }
        return Time.time - skill1Timer;
    }

    public float GetSkill2Time()
    {
        if (firstE)
        {
            return skill2Cooldown;
        }
        return Time.time - skill2Timer;
    }
}