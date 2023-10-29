using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTimerBar : MonoBehaviour
{
    //public variables
    public Slider slider;
    public GameObject shade;

    public void SetCooldown(float time)
    {
        //set cooldown
        slider.maxValue = time;
        slider.value = time;
    }

    public void SetTime(float time)
    {
        //set current time
        slider.value = time;
        transform.gameObject.SetActive(true);
    }

    public void CheckIfCooldownFinished()
    {
        //check if cooldown finished
        if (slider.value == slider.maxValue)
        {
            shade.SetActive(false);
        }
        else
        {
            shade.SetActive(true);
        }
    }
}