using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    //public variables
    public Slider slider;

    public void SetMaxHealth(int maxHealth)
    {
        //set max health
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
    }

    public void SetHealth(int health)
    {
        //set health
        slider.value = health;
        transform.gameObject.SetActive(true);
    }
}