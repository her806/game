using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    public string deathTrigger = "die";
    public float restartDelay = 2.5f;

    private Animator anim;
    private Rigidbody2D rb;
    private PlayerMovement2D movement;
    private bool isDead = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<PlayerMovement2D>();
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        if (anim != null) anim.SetTrigger(deathTrigger);
        if (movement != null) movement.enabled = false;
        
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        Invoke("RestartLevel", restartDelay);
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}