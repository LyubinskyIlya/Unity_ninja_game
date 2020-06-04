using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AI : MonoBehaviour
{

    protected bool isAlive = true;
    protected Animator anim;
    protected Rigidbody2D rb;
    protected bool underAttack = false;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }



    async protected Task ExecuteAfter(int miliseconds, Action function)
    {
        await Task.Delay(miliseconds);
        function();
    }


    virtual protected void enableMove()
    {
        underAttack = false;
    }

    virtual public void attackPush(Vector2 direction, float force, ForceMode2D mode = ForceMode2D.Force)
    {
        rb.AddForce(direction * force, mode);
        underAttack = true;
        // создал Task чтобы не было ворнинга о том, что мы не ждем выполнение делигируемой функции
        Task delayTask = ExecuteAfter(500, enableMove);
    }

    virtual public void Die()
    {
        isAlive = false;
        anim.SetBool("isAlive", false);
    }    
}
