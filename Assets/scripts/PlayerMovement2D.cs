using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    [Header("Движение")]
    public float moveSpeed = 8f; // Скорость ходьбы
    
    [Header("Прыжок")]
    public float jumpForce = 12f; // Сила прыжка
    public Transform groundCheck; // проверка земля (центр квадрата под ногами)
    public LayerMask groundLayer; // земля
    public Vector2 boxSize = new Vector2(0.9f, 0.15f); // Размер квадрата: ширина почти как перс, высота мелкая для точности
    
    [Header("Дэш")]
    public float dashForce = 20f; // Сила рывка, чем больше тем дальше
    public float dashDuration = 0.15f; // Сколько длится дэш, короткий для баланса вселенной
    public float dashCooldown = 1f; // антиспам
    
    private Rigidbody2D rb;
    private float horizontalInput; // Ввод по горизонтали A/D или стрелки
    private bool isFacingRight = true; // фейсинг сторона
    private bool isDashing = false; // Флаг дэша
    private float dashTimer = 0f; // длительность деша
    private float cooldownTimer = 0f; // антиспам контроль
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Берём ригидбади без него будет капут пезда и бро ты умрешь и.т.д
    }
    
    void Update()
    {
        // Эта хуйня отключает клаву при шифте
        if (isDashing) return; 
        
        horizontalInput = Input.GetAxisRaw("Horizontal"); // Берём ввод, Raw так в ютубе сказали+так более резко будет по идее
        
        // Прыжок на пробел, но только если на земле
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
        
        // Дэш на Left Shift — рывок в сторону мыши только по X
        if (Input.GetKeyDown(KeyCode.LeftShift) && cooldownTimer <= 0f)
        {
            StartDash();
        }
        
        // Поворот по клавишам, пока не в дэше
        if (horizontalInput > 0) isFacingRight = true;
        else if (horizontalInput < 0) isFacingRight = false;
        
        // Флип
        transform.localScale = new Vector3(isFacingRight ? 1 : -1, 1, 1);
        
        // антиспам
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;
    }
    
    void FixedUpdate()
    {
        // Во время дэша вообще ничего не трогаем а то капут физика пезда бро ты умрешь и.т.д
        if (isDashing)
        {
            dashTimer -= Time.fixedDeltaTime;
            if (dashTimer <= 0f)
            {
                isDashing = false;
                cooldownTimer = dashCooldown; // антиспам
            }
            return;
        }
        
        // Обычная ходьба
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }
    
    private void StartDash()
    {
        // Берём позицию мыши в мире
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f; // зет нахуй не нужен
        
        // Считаем направление только по X
        float dashDirection = Mathf.Sign(mouseWorldPos.x - transform.position.x);
        
        // Если мышь ровно над головой — дэшим по текущему направлению
        if (dashDirection == 0f) dashDirection = isFacingRight ? 1f : -1f;
        
        // Поворачиваем перса в сторону дэша
        isFacingRight = dashDirection > 0f;
        transform.localScale = new Vector3(isFacingRight ? 1 : -1, 1, 1);
        
        // Запускаем дэш
        isDashing = true;
        dashTimer = dashDuration;
        rb.linearVelocity = new Vector2(dashDirection * dashForce, 0f); // Y сбрасываем
    }
    
    // НОВАЯ ПРОВЕРКА ЗЕМЛИ — КВАДРАТОМ, БРО! Надёжнее круга в 100 раз
    private bool IsGrounded()
    {
        // OverlapBox: позиция центра, размер бокса, угол 0 (не крутим), слой земли
        return Physics2D.OverlapBox(groundCheck.position, boxSize, 0f, groundLayer);
    }
    
    // чтоб видеть где проверка земли — теперь квадрат, полезно когда все по пизде
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(groundCheck.position, boxSize);
        }
    }
}