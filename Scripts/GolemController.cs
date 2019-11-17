using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemController : Enemy
{

    public int GolemHealth;
    public float GolemSpeed;
    public float GolemRange;
    public float GolemAttackRange;
    public int GolemDamage;
    public Transform attackPos;
    public LayerMask layerPlayer;

    //attack
    private float timeBtwAttack;
    private float golemTimeBtwAttack;



    // Start is called before the first frame update
    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        health = GolemHealth;
        speed = GolemSpeed;
        timeBtwAttack = 0;
        golemTimeBtwAttack = 1.3f;
        damage = GolemDamage;
    }
    
    // Update is called once per frame
    void Update()
    {
        //Debug.Log(staggered);
        //health > 0 so it will not trace when it is dead
        if (timeBtwAttack <= 0) {
            if ((playerLocation != null) && (health > 0))
            {

                LookAtPlayer();
                //TracePlayer(GolemRange, speed, playerLocation.position, transform.position);

                if (Vector2.Distance(transform.position, playerLocation.position) > GolemRange)
                {
                    animator.SetBool("Moving", true);
                    transform.position = Vector2.MoveTowards(transform.position, playerLocation.position, GolemSpeed * Time.deltaTime);
                }
                else if (Vector2.Distance(transform.position, playerLocation.position) <= GolemRange)
                {
                    animator.SetBool("Moving", false);

                    //attack
                    animator.SetTrigger("Attack");

                    StartCoroutine(GolemHit());

                    Debug.Log("Not moving");
                    timeBtwAttack = golemTimeBtwAttack;
                }
            }
            }
            else
            {
                animator.SetBool("Moving", false);
                timeBtwAttack -= Time.deltaTime;
                //Debug.Log("time");
            }
        

        //Death tbc to enemy instead maybe?
        if (health == 0)
        {
            OnDeath();
        }
    }
    IEnumerator GolemHit()
    {
        animator.SetTrigger("Attack");       

        //~10/12 second calculated from the frame of the golem swinging his fists
        yield return new WaitForSeconds(0.85f);

        Collider2D playerToDamage = Physics2D.OverlapCircle(attackPos.position, GolemAttackRange, layerPlayer);
        if (playerToDamage != null && staggered != true)
        {
            DamagePlayer(playerToDamage);

        }

    }
    void OnDrawGizmosSelected()
    {

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, GolemAttackRange);
    }

}
