using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour
{
    [Header("Настройки урона")]
    public float damageAmount = 20f;
    public float attackRange = 1.5f;
    public LayerMask playerLayer;

    [Header("Тайминги")]
    public float delayBeforeDamage = 0.5f;

    [Header("Точка удара")]
    public Transform attackPoint;

    public void StartAttackVisual()
    {
        StartCoroutine(HitRoutine());
    }

    private IEnumerator HitRoutine()
    {
        yield return new WaitForSeconds(delayBeforeDamage);

        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);

        foreach (Collider2D player in hitPlayers)
        {
            PlayerHealth health = player.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(damageAmount);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}