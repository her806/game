using UnityEngine;

public class Hazard : MonoBehaviour
{
    public float damage = 100f;
    public bool killInstantly = true;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HandleDamage(collision.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HandleDamage(collision.gameObject);
        }
    }

    void HandleDamage(GameObject player)
    {
        PlayerHealth health = player.GetComponent<PlayerHealth>();

        if (health != null)
        {
            if (killInstantly)
            {
                health.TakeDamage(health.maxHealth);
            }
            else
            {
                health.TakeDamage(damage);
            }
        }
    }
}