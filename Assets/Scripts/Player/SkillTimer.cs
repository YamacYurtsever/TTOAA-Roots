using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTimer : MonoBehaviour
{
    //public variables
    [Header("Skill1")]
    public SkillTimerBar skill1CooldownManager;

    [Header("Skill2")]
    public SkillTimerBar skill2CooldownManager;

    //private variables
    private PlayerSkill playerSkill;

    private float skill1Cooldown;
    private float skill2Cooldown;

    void Start()
    {
        //connections
        playerSkill = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSkill>();

        skill1Cooldown = playerSkill.skill1Cooldown;
        skill2Cooldown = playerSkill.skill2Cooldown;

        skill1CooldownManager.SetCooldown(skill1Cooldown);
        skill2CooldownManager.SetCooldown(skill2Cooldown);

        //close cursor
        Cursor.visible = false;
    }

    void Update()
    {
        //set visualize cooldowns
        skill1CooldownManager.SetTime(skill1Cooldown - playerSkill.GetSkill1Time());
        skill2CooldownManager.SetTime(skill2Cooldown - playerSkill.GetSkill2Time());
    }
}