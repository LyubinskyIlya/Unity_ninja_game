using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class Scorpion_AI : AI
{
    public Vector2 speed = new Vector2(10, 0);
    public int touchDamage = 10;

    private bool isFacingRight = false;
    private BoxCollider2D bc;
    // Start is called before the first frame update


    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
    }


    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 Scale = transform.localScale;
        Scale.x *= -1;
        transform.localScale = Scale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            Flip();
        }
        else if (collision.tag == "Player")
        {
            onTouchPlayer(collision, touchDamage);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            Flip();
        }
        else if (collision.gameObject.tag == "Player")
        {
            onTouchPlayer(collision.collider, touchDamage);
        }
    }

    private void FixedUpdate()
    {
        if (isAlive && !underAttack)
        {
            rb.velocity = isFacingRight ? speed : -1 * speed;
        }
        if ((rb.velocity.x > 0 && !isFacingRight) || (rb.velocity.x < 0 && isFacingRight))
        {
            Flip();
        }
        bc.isTrigger = underAttack ? false : true;
    }


    override public void attackPush(Vector2 direction, float force, ForceMode2D mode = ForceMode2D.Force)
    {
        Debug.Log("scorp under attack");
        rb.AddForce((Vector2)Vector3.Project(direction, Vector3.right).normalized * force, mode);
        underAttack = true;
        // создал Task чтобы не было ворнинга о том, что мы не ждем выполнение делигируемой функции
        Task delayTask = ExecuteAfter(500, enableMove);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
