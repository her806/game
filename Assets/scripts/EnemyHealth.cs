using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 50f;
    private float currentHealth;
    private Animator anim;
    private EnemyAI ai;

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        ai = GetComponent<EnemyAI>();
    }

    public void TakeDamage(float damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        
        if (anim != null) anim.SetTrigger("hit");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (ai != null) ai.Die();
    }
}