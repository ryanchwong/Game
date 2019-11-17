using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] float speed = 1.0f;
    [SerializeField] float jumpForce = 2.0f;
    public int health;

    private Animator animator;
    private Rigidbody2D body2d;
    private BoxCollider2D box2d;
    private EdgeCollider2D edge2d;

    private bool grounded = false;
    private bool combatIdle = false;
    private bool isDead = false;

    //Attack
    private float timeBtwAttack;
    private float meleeTimeBtwAttack;
    private float rangedTimeBtwAttack;

    [SerializeField] float speedWhileAttacking;
    public LayerMask layerEnemies;
    private int attackChain;
    public float basicHit1Range;
    public float basicHit2Range;
    public Transform basicHit1Pos;
    public Transform basicHit2Pos;
    public int damage;
    public Animator camAnim;



    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        body2d = GetComponent<Rigidbody2D>();
        box2d = GetComponent<BoxCollider2D>();
        edge2d = GetComponent<EdgeCollider2D>();
        attackChain = 2;
        meleeTimeBtwAttack = 0.38f;
        rangedTimeBtwAttack = 0.7f;
    }

    // Update is called once per frame
    void Update()
    {
        //Check if character just landed on the ground
        if (!grounded)
        {
            grounded = true;
            animator.SetBool("Grounded", grounded);
        }

        //Check if character just started falling
        if (grounded)
        {
            grounded = false;
            animator.SetBool("Grounded", grounded);
        }

        // -- Handle input and movement --

        float inputX = Input.GetAxis("Horizontal");

        // Swap direction of sprite depending on walk direction

        if (inputX > 0)
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        else if (inputX < 0)
        {
            transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        }

        // Move
        if (timeBtwAttack <= 0)
        {
            body2d.velocity = new Vector2(inputX * speed, body2d.velocity.y);
        }
        else
        {
            body2d.velocity = new Vector2(inputX * 0.5f, body2d.velocity.y);
        }
            

        

        //Attack
        if (timeBtwAttack <= 0)
        {
            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.K))
            {
                animator.SetTrigger("Shoot");
                Launch();
                timeBtwAttack = rangedTimeBtwAttack;
            }
            else if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.J))
            {
                animator.SetTrigger("BasicHit" + attackChain.ToString());
                if (attackChain == 1)
                {
                    StartCoroutine(BasicHit1());
                    attackChain = 2;
                }
                else if (attackChain == 2)
                {
                    StartCoroutine(BasicHit2());
                    attackChain = 1;
                }
                timeBtwAttack = meleeTimeBtwAttack;
            }
        }
        else
        {
            timeBtwAttack -= Time.deltaTime;
        }


        //Jump
        if (Input.GetKeyDown("space") && grounded)
        {
            animator.SetTrigger("Jump");
            grounded = false;
            animator.SetBool("Grounded", grounded);
            body2d.velocity = new Vector2(body2d.velocity.x, jumpForce);
        }

        //Run
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
            animator.SetInteger("AnimState", 2);

        //Combat Idle

        //Idle
        else
            animator.SetInteger("AnimState", 0);
    }

    public IEnumerator BasicHit1()
    {
        //Time needed for the blade to hit in the animations. Calcuated by looking at the exact frame of the sword's swing divided by the sample rate(12)
        yield return new WaitForSeconds(0.21f);
        
        //tbc
        camAnim.SetTrigger("Shake");
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(basicHit1Pos.position, basicHit1Range, layerEnemies);
        AttackEnemy(enemiesToDamage);
        
        //movement disabled until attack animation is finished
    }

    public IEnumerator BasicHit2()
    {

        yield return new WaitForSeconds(0.12f);
        //tbc
        camAnim.SetTrigger("Shake");
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(basicHit2Pos.position, basicHit2Range, layerEnemies);
        AttackEnemy(enemiesToDamage);

    }



    //Easier testing
    public void AttackEnemy(Collider2D[] enemiesToDamage)
    {
        for (int i = 0; i < enemiesToDamage.Length; i++)
        {
            //GolemController should change to enemy                            not sure if needed tbc
            if ((enemiesToDamage[i].GetComponent<Enemy>() != null) && (enemiesToDamage[i].GetComponent<Enemy>().health>0))
            {
                enemiesToDamage[i].GetComponent<Enemy>().TakeDamage(damage);
                //enemiesToDamage[i].GetComponent<Rigidbody2D>().AddForce(-transform.right * 20000);
                Debug.Log("hit!");
            }
        }
    }

    public void Launch()
    {

    }

    //taking damage
    public void TakingDamage(int damage)
    {
        health -= damage;
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(basicHit1Pos.position,basicHit1Range);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(basicHit2Pos.position, basicHit2Range);

    }

}
