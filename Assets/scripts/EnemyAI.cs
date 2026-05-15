using UnityEngine;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public List<Transform> patrolPoints;
    public float moveSpeed = 2f;
    public float chaseRange = 5f;
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;
    public float deathDestroyDelay = 2f;

    private int currentPointIndex = 0;
    private float nextAttackTime;
    private Animator anim;
    private SpriteRenderer sprite;
    private Rigidbody2D rb;
    private bool isDead = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            StopMoving();
            if (Time.time >= nextAttackTime)
            {
                Attack();
            }
        }
        else if (distanceToPlayer <= chaseRange)
        {
            MoveTowards(player.position);
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Count == 0) return;

        Transform target = patrolPoints[currentPointIndex];
        MoveTowards(target.position);

        if (Vector2.Distance(transform.position, target.position) < 0.2f)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Count;
        }
    }

    void MoveTowards(Vector2 target)
    {
        anim.SetBool("isRunning", true);
        transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
        sprite.flipX = target.x > transform.position.x;
    }

    void StopMoving()
    {
        anim.SetBool("isRunning", false);
    }

    void Attack()
    {
        anim.SetTrigger("attack");
        nextAttackTime = Time.time + attackCooldown;

        EnemyAttack attackScript = GetComponent<EnemyAttack>();
        if (attackScript != null)
        {
            attackScript.StartAttackVisual();
        }
    }

    public void TakeHit()
    {
        anim.SetTrigger("hit");
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        anim.SetTrigger("die");

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Destroy(gameObject, deathDestroyDelay);
        this.enabled = false;
    }
}