using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public int maxHealth = 100;

    private int Health;
    private float scale;
    private GameObject parent;

    private void Start()
    {
        Health = maxHealth;
        scale = transform.localScale.x;
        parent = transform.parent.gameObject;
    }

    public void takeDamage(int damage)
    {
        Health = Mathf.Max(0, Health - damage);
        transform.localScale = new Vector3(scale * Health / maxHealth, transform.localScale.y);
        if (Health == 0)
        {
            parent.GetComponent<AI>().Die();
        }
    }
}
