using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using UnityEngine.Rendering;

public class PlayerHP : MonoBehaviour
{
    public int maxHealth = 100;

    private int Health;
    private float scale;
    private GameObject parent;
    private bool invulnerable = false;
    private PlayerControllerScript playerController;

    private void Start()
    {
        Health = maxHealth;
        scale = transform.localScale.x;
        parent = transform.parent.gameObject;
        playerController = parent.GetComponent<PlayerControllerScript>();
    }


    async Task ExecuteAfter(int miliseconds, Action function)
    {
        await Task.Delay(miliseconds);
        function();
    }

    async Task ExecuteOnTimer(int miliseconds, int times, Action function)
    {
        for (int i = 0; i < times; i++)
        {
            await Task.Delay(miliseconds);
            function();
        }
    }

    void ChangeInvisableStatus()
    {
        var s = playerController.GetComponent<SpriteRenderer>();
        s.enabled = !s.enabled;
    }    

    void vulnarableAgain()
    {
        invulnerable = false;
    }

    public void takeDamage(int damage)
    {
        Debug.Log("player damaged");
        if (!invulnerable)
        {
            Health = Mathf.Max(0, Health - damage);
            transform.localScale = new Vector3(scale * Health / maxHealth, transform.localScale.y);
            //var scr = parent.GetComponent<PlayerControllerScript>();
            if (Health == 0)
            {
                playerController.Die();
            }
            else
            {
                invulnerable = true;
                // do not wait, do not warn
                Task tmp = ExecuteAfter(1000, vulnarableAgain);
                tmp = ExecuteOnTimer(100, 10, ChangeInvisableStatus);
            }
        }
    }
}