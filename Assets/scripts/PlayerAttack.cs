using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    [Header("Настройки урона")]
    public float damageAmount = 25f;
    public float attackRange = 1.2f;
    public LayerMask enemyLayer;
    public KeyCode attackKey = KeyCode.Mouse0;

    [Header("Тайминги")]
    public float damageDelay = 0.2f;

    [Header("Точка удара")]
    public Transform attackPoint;

    private Animator anim;
    private bool isAttacking = false;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(attackKey) && !isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        
        if (anim != null) anim.SetTrigger("attack");

        yield return new WaitForSeconds(damageDelay);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyHealth health = enemy.GetComponent<EnemyHealth>();
            if (health != null)
            {
                health.TakeDamage(damageAmount);
            }
        }

        yield return new WaitForSeconds(0.1f); 
        isAttacking = false;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}