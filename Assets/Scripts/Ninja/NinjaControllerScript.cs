using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using System.Diagnostics;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

public class NinjaControllerScript : PlayerControllerScript
{
    public float maxSpeed = 10f;
    //public float jumpForce = 600;
    public float jumpSpeed = 10;
    public float maxJumpingTime = 1;
    public GameObject attack_1_1;
    public GameObject attack_1_2;
    public GameObject attack_3_1;
    public GameObject attack_3_2;
    public GameObject attack_jump;
    public Transform shuri_point;
    public GameObject shuriken;
    public int shuriForce = 100;
    //public List<AudioClip> clips = new List<AudioClip>();

    private bool isFacingRight = true;
    private Animator anim;
    private Rigidbody2D rb;
    private AudioSource[] Audios;
    private Dictionary<string, AudioSource> AudioByName;
    private Vector2 ccCenterOffset;
    private float ccRadius;
    private bool isGrounded = true;
    private bool freezeMovement = false;
    private bool groundAttacking = false;
    private bool lastFrameisGrounded = true;
    private float lastJumpTime = -1;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        Audios = GetComponents<AudioSource>();
        AudioByName = Audios.ToDictionary(k => k.clip.name);

        var cc = GetComponent<CircleCollider2D>();
        var scale = transform.localScale.y;
        ccCenterOffset = cc.offset * scale;
        ccRadius = cc.radius * scale;
        attack_1_1.SetActive(false);
        attack_1_2.SetActive(false);
        attack_3_1.SetActive(false);
        attack_3_2.SetActive(false);
        attack_jump.SetActive(false);
    }


    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 Scale = transform.localScale;
        Scale.x *= -1;
        transform.localScale = Scale;
    }

    bool CheckGround()
    {
        float scale = 1.1f;
        Vector2 origin = (Vector2)transform.position + ccCenterOffset;
        Vector2 direction = Vector2.down;
        float distance = ccRadius * scale;
        LayerMask mask = LayerMask.GetMask("Ground");
        Vector2 half_width = new Vector2(distance, 0);

        var hit = Physics2D.Raycast(origin, direction, distance, mask);
        if (hit)
        {
            return true;
        }
        else if (hit = Physics2D.Raycast(origin + half_width, direction, distance, mask))
        {
            return true;
        }
        else if (hit = Physics2D.Raycast(origin - half_width, direction, distance, mask))
        {
            return true;
        }
        return false;
    }

    void StartAttack_1_1()
    {
        attack_1_1.SetActive(true);
        AudioByName["Strike"].Play();
    }

    void StopAttack_1_1()
    {
        anim.SetBool("attack-1-1", false);
        attack_1_1.SetActive(false);

        if (Input.GetMouseButton(0))
        {
            anim.SetBool("attack-1-2", true);
            StartAttack_1_2();
        }
        else
        {
            groundAttacking = false;
            freezeMovement = false;
        }
    }

    void StartAttack_1_2()
    {
        attack_1_2.SetActive(true);
        AudioByName["Strike"].Play();
    }

    void StopAttack_1_2()
    {
        freezeMovement = false;
        groundAttacking = false;
        anim.SetBool("attack-1-2", false);
        attack_1_2.SetActive(false);
    }

    void StartJumpAttack()
    {
        //anim.SetBool("attack_jump", true);
        attack_jump.SetActive(true);
        AudioByName["Strike"].Play();
    }

    void StopJumpAttack()
    {
        anim.SetBool("attack_jump", false);
        attack_jump.SetActive(false);
        AudioByName["Strike"].Stop();
    }

    void StartAttack_3_1()
    {
        attack_3_1.SetActive(true);
        AudioByName["Strike"].Play();
    }

    void StopAttack_3_1()
    {
        attack_3_1.SetActive(false);
    }
    
    void StartAttack_3_2()
    {
        attack_3_2.SetActive(true);
        AudioByName["Strike"].Play();
    }

    void StopAttack_3_2()
    {
        freezeMovement = false;
        groundAttacking = false;
        anim.SetBool("attack-3", false);
        attack_3_2.SetActive(false);
    }

    void castShuriken()
    {
        // var origin = transform.position + transform.forward * 3;// + shuri_point.transform.position * shuriken.transform.localScale.x;
        // origin.x *= transform.localScale.x; // rigth direction
        var origin = shuri_point.transform.position;
        GameObject shuri = Instantiate(shuriken, origin, transform.rotation);
        var rb = shuri.GetComponent<Rigidbody2D>();
        var direction = transform.right;
        if (!isFacingRight)
        {
            direction *= -1;
        }
        rb.AddForce((Vector2)direction * shuriForce, ForceMode2D.Impulse);
        // rb.AddForce((Vector2)(origin-transform.position).normalized * shuriForce, ForceMode2D.Impulse);
        AudioByName["CastShuri"].Play();
    }

    void StopShurikenAttack()
    {
        freezeMovement = false;
        groundAttacking = false;
        anim.SetBool("attack_shuriken", false);
    }

    private void FixedUpdate()
    {
        float move = Input.GetAxisRaw("Horizontal");
        //float move = Input.GetAxis("Horizontal");
        anim.SetFloat("Speed", Mathf.Abs(move));
        var runSound = AudioByName["Run"];
        if (!freezeMovement)
        {
            rb.velocity = new Vector2(move * maxSpeed, rb.velocity.y);

            if ((move > 0 && !isFacingRight) || (move < 0 && isFacingRight))
            {
                Flip();
            }
            if (Mathf.Abs(move) > 0.01 && !runSound.isPlaying && isGrounded)
            {
                runSound.Play();
            }
            else if (runSound.isPlaying && (Mathf.Abs(move) < 0.01 || !isGrounded))
            {
                runSound.Stop();
            }
        }
        else if (runSound.isPlaying)
        {
            runSound.Stop();
        }
        lastFrameisGrounded = isGrounded;
        isGrounded = CheckGround();
        anim.SetBool("isGrounded", isGrounded);
        if (lastFrameisGrounded && ! isGrounded)
        {
            AudioByName["PreJump"].Play();
        }
        else if (! lastFrameisGrounded && isGrounded)
        {
            AudioByName["PostJump"].Play();
        }
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space) || rb.velocity.y == 0) // stop jumping anyway
        {
            lastJumpTime = -5;
        }
        if (isGrounded && Input.GetKeyDown(KeyCode.Space) && ! freezeMovement)
        {
            lastJumpTime = Time.time;
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
            //rb.AddForce(new Vector2(0, jumpForce));
        }
        else if ( ((Time.time - lastJumpTime) < maxJumpingTime) && Input.GetKey(KeyCode.Space) && !freezeMovement)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }
        else if (isGrounded && Input.GetMouseButtonDown(0) && !groundAttacking)
        {
            groundAttacking = true; //they are the same now
            freezeMovement = true; // but freeze mb enable in other situation
            rb.velocity = Vector2.zero;
            anim.SetBool("attack-1-1", true);
        }
        else if (isGrounded && Input.GetMouseButtonDown(1) && !groundAttacking)
        {
            groundAttacking = true;
            freezeMovement = true;
            rb.velocity = Vector2.zero;
            anim.SetBool("attack-3", true);
        }
        else if (! isGrounded && Input.GetMouseButton(0))
        {
            anim.SetBool("attack_jump", true);
        }
        else if (isGrounded && Input.GetKeyDown(KeyCode.E) && !groundAttacking)
        {
            groundAttacking = true;
            freezeMovement = true;
            rb.velocity = Vector2.zero;
            anim.SetBool("attack_shuriken", true);
        }
    }
}
