using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Video;

public class Shuriken : MonoBehaviour
{
    //public const int inverseRotateLimit = 100;
    //public int speed = 100;
    //public Vector3 degree = new Vector3(0, 0, 10);
    public const int rotateSpeedPerFrame = 10;
    public const int damage = 20;
    public float timeAlive = 5;

    private Rigidbody2D rb;
    private float bornTime;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            var bar = collision.transform.Find("HealthBar");
            if (bar)
            {
                var bar_sctipt = bar.GetComponent<HealthBar>();
                if (bar_sctipt)
                {
                    bar_sctipt.takeDamage(damage);
                }
            }
            Destroy(gameObject);
        }
        else
        {
            if (rb)
            {
                var cc = GetComponent<CircleCollider2D>();
                cc.enabled = false;
                rb.constraints = RigidbodyConstraints2D.FreezeAll;
            }
            bornTime = Time.time - timeAlive + 1f; // delete after x seconds
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            var bar = collision.transform.Find("HealthBar");
            if (bar)
            {
                var bar_sctipt = bar.GetComponent<HealthBar>();
                if (bar_sctipt)
                {
                    bar_sctipt.takeDamage(damage);
                }
            }
            Destroy(gameObject);
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bornTime = Time.time;
    }

    void FixedUpdate()
    {
        if (Time.time - bornTime > timeAlive)
        {
            Destroy(gameObject);
        }
        var speed = rb.velocity.magnitude;
        var degree = Mathf.Min(speed, rotateSpeedPerFrame);
        transform.Rotate(new Vector3(0, 0, degree));
    }
}
