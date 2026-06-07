using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    [Header("Бинды клавиш")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode dashKey = KeyCode.LeftShift;
    public KeyCode attackKey = KeyCode.Mouse0;
    public KeyCode dodgeKey = KeyCode.LeftAlt;

    [Header("Настройки движения")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;

    [Header("Дэш")]
    public float dashForce = 20f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 1f;

    [Header("Додж (Уворот)")]
    public float dodgeDuration = 0.5f; 
    public float dodgeCooldown = 1.5f;
    public int playerLayer = 3;  
    public int enemyLayer = 6;  

    [Header("Проверка земли")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public Vector2 boxSize = new Vector2(0.9f, 0.15f);

    [Header("Имена параметров Animator")]
    public string pIsRunning = "isRunning";
    public string pIsGrounded = "isGrounded";
    public string pYVelocity = "yVelocity";
    public string pAttack = "attack";
    public string pDash = "dash";
    public string pDodge = "dodge"; 
    public string pDie = "die";

    private Rigidbody2D rb;
    private Animator anim;
    private float horizontalInput;
    private bool isFacingRight = true;
    private bool isDashing = false;
    private bool isDodging = false;
    private float dashTimer = 0f;
    private float dashCooldownTimer = 0f;
    private float dodgeCooldownTimer = 0f;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDead || isDashing) return;

        horizontalInput = Input.GetAxisRaw("Horizontal");

        // делай джамп
        if (Input.GetKeyDown(jumpKey) && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // бум
        if (Input.GetKeyDown(attackKey))
        {
            anim.SetTrigger(pAttack);
        }

        // не бум
        if (Input.GetKeyDown(dashKey) && dashCooldownTimer <= 0f)
        {
            StartDash();
        }

        // нит
        if (Input.GetKeyDown(dodgeKey) && dodgeCooldownTimer <= 0f && !isDodging)
        {
            StartDodge();
        }

        FlipLogic();
        UpdateAnimatorParams();

        if (dashCooldownTimer > 0f) dashCooldownTimer -= Time.deltaTime;
        if (dodgeCooldownTimer > 0f) dodgeCooldownTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (isDead) return;

        if (isDashing)
        {
            dashTimer -= Time.fixedDeltaTime;
            if (dashTimer <= 0f)
            {
                isDashing = false;
                dashCooldownTimer = dashCooldown;
            }
            return;
        }

        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    void UpdateAnimatorParams()
    {
        anim.SetBool(pIsRunning, Mathf.Abs(horizontalInput) > 0.1f);
        anim.SetBool(pIsGrounded, IsGrounded());
        anim.SetFloat(pYVelocity, rb.linearVelocity.y);
    }

    void FlipLogic()
    {
        if (horizontalInput > 0) isFacingRight = true;
        else if (horizontalInput < 0) isFacingRight = false;
        transform.localScale = new Vector3(isFacingRight ? 1 : -1, 1, 1);
    }

    private void StartDash()
    {
        isDashing = true;
        dashTimer = dashDuration;
        anim.SetTrigger(pDash);
        float dashDir = isFacingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(dashDir * dashForce, 0f);
    }

    private void StartDodge()
    {
        isDodging = true;
        dodgeCooldownTimer = dodgeCooldown;
        anim.SetTrigger(pDodge);
        
        
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);
        
        
        Invoke("EndDodge", dodgeDuration);
    }

    private void EndDodge()
    {
        isDodging = false;
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        anim.SetTrigger(pDie);
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
    }

    private bool IsGrounded() => Physics2D.OverlapBox(groundCheck.position, boxSize, 0f, groundLayer);

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(groundCheck.position, boxSize);
        }
    }
}