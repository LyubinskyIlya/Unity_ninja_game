using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{

    public int damage = 20;

     private void OnTriggerEnter2D(Collider2D collision)
     {
         if (collision.tag == "Enemy")
         {
            // damage
             var bar = collision.transform.Find("HealthBar");
             if (bar)
             {
                 var barSctipt = bar.GetComponent<HealthBar>();
                 if (barSctipt)
                 {
                     barSctipt.takeDamage(damage);
                 }            
             }

            //push
            var enemyScript = collision.gameObject.GetComponent<AI>();
            var direction = collision.ClosestPoint((Vector2)transform.position);
            var force = 50;
            enemyScript.attackPush(direction, force, ForceMode2D.Impulse);
         }
     }
}
