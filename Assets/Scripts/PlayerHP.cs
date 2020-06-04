using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

public class PlayerHP : MonoBehaviour
{
    public int maxHealth = 100;

    private int Health;
    private float scale;
    private GameObject parent;
    private bool invulnerable = false;

    private void Start()
    {
        Health = maxHealth;
        scale = transform.localScale.x;
        parent = transform.parent.gameObject;
    }


    async Task ExecuteAfter(int miliseconds, Action function)
    {
        await Task.Delay(miliseconds);
        function();
    }

    void vulnarableAgain()
    {
        invulnerable = false;
    }

    public void takeDamage(int damage)
    {
        if (!invulnerable)
        {
            Health = Mathf.Max(0, Health - damage);
            transform.localScale = new Vector3(scale * Health / maxHealth, transform.localScale.y);
            if (Health == 0)
            {
                parent.GetComponent<PlayerControllerScript>().Die();
            }
            else
            {
                invulnerable = true;
                // do not wait, do not warn
                Task tmp = ExecuteAfter(1000, vulnarableAgain);
            }
        }
    }
}