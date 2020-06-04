using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat_AI : AI
{
    public float speed = 10;
    //public int attackImpulse = 50;
    public int touchDamage = 10;

    private GameObject player;
   // private Rigidbody2D rb;
    private bool isFacingRight = true;
    private float last_hit_time = -5;
    Vector3 last_hit_direction;
    Vector3 after_hit_direction;

    void Start()
    {
        isAlive = true;
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        anim.SetBool("isAlive", isAlive);
       // Debug.Log(rb);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // тут надо допилить чтоб не наносился урон если underAttack==true
        if (other.tag == "Player")
        {
            last_hit_time = Time.time;
            last_hit_direction = (player.transform.position - transform.position).normalized;
            after_hit_direction = new Vector3(last_hit_direction.y, -last_hit_direction.x, last_hit_direction.z);
            if (Vector3.Dot(after_hit_direction, Vector3.up) < 0)
            {
                after_hit_direction *= -1;
            }
            var bar = other.transform.Find("PlayerHP"); 
            var scr = bar.GetComponent<PlayerHP>();
            if (!underAttack)
            {
                scr.takeDamage(touchDamage);
            }
        }
    }

    /*private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Attack")
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, collision.transform.position);

            if (hit.collider != null)
            {
                // Draws a line from the normal of the object that you clicked
                Debug.DrawLine((Vector2)transform.position, hit.normal, Color.yellow, 10.0f);
                //var direction = 
            }
        }
    }*/

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 Scale = transform.localScale;
        Scale.x *= -1;
        transform.localScale = Scale;
    }


    private void FixedUpdate()
    {
        if (isAlive && !underAttack)
        {
            Vector3 direction;
            if (Time.time - last_hit_time < 0.5)
            {
                direction = after_hit_direction;
                Debug.Log("Escaping");
            }
            else
            {
                direction = (player.transform.position - transform.position).normalized;
                Debug.Log("Attacking");
            }
            rb.velocity = (Vector2)direction * speed;
        }
        if ((rb.velocity.x > 0 && !isFacingRight) || (rb.velocity.x < 0 && isFacingRight))
        {
            Flip();
        }
    }

    void Disappear()
    {
        Destroy(gameObject);
    }    

    public override void Die()
    {
        isAlive = false;
        anim.SetBool("isAlive", false);
        var bc = GetComponent<BoxCollider2D>();
        bc.isTrigger = false;
        rb.gravityScale = 1;
        var tmp = ExecuteAfter(5000, Disappear);
    }
}
