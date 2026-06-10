using UnityEngine;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

public class SatyrUltimateTool : MonoBehaviour
{
    public bool isMenuVisible = false;
    public KeyCode toggleKey = KeyCode.Insert;
    public float speedScale = 1.0f;
    public bool isNoclip = false;
    public bool isRecording = false;
    public bool isPlaying = false;

    private GameObject player;
    private Rigidbody2D rb;
    private Collider2D col;
    private Animator anim;
    private string speedInput = "1.0";
    
    private List<Vector3> recordedPos = new List<Vector3>();
    private float playbackTime = 0f;
    private float recordTimer = 0f;
    private const float RECORD_INTERVAL = 0.02f; // Запись каждые 20мс игрового времени
    private string filePath;

    [System.Serializable]
    public class MacroWrapper { public List<Vector3> p; }

    void Awake()
    {
        if (FindObjectsOfType<SatyrUltimateTool>().Length > 1) { Destroy(gameObject); return; }
        DontDestroyOnLoad(gameObject);
        filePath = Path.Combine(Application.persistentDataPath, "macro_data.json");
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey)) isMenuVisible = !isMenuVisible;

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                rb = player.GetComponent<Rigidbody2D>();
                col = player.GetComponent<Collider2D>();
                anim = player.GetComponent<Animator>();
            }
        }

        // Изменение скорости игры
        if (Mathf.Abs(Time.timeScale - speedScale) > 0.001f)
        {
            Time.timeScale = speedScale;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }

        // Синхронизация анимации: при спидхаке анимация должна ускоряться вместе с миром
        if (anim != null) 
        {
            anim.speed = speedScale > 0 ? speedScale : 1f; 
        }

        if (player != null)
        {
            if (isNoclip) RunNoclip();
            else ResetPhysics();

            // Логика записи и воспроизведения, завязанная на deltaTime
            HandleMacroSystem();
        }
    }

    void HandleMacroSystem()
    {
        if (isRecording)
        {
            recordTimer += Time.deltaTime;
            if (recordTimer >= RECORD_INTERVAL)
            {
                recordedPos.Add(player.transform.position);
                recordTimer = 0f;
            }
        }
        else if (isPlaying)
        {
            if (recordedPos.Count < 2)
            {
                isPlaying = false;
                return;
            }

            playbackTime += Time.deltaTime;
            
            // Вычисляем текущий индекс в зависимости от прошедшего игрового времени
            float exactFrame = playbackTime / RECORD_INTERVAL;
            int indexA = Mathf.FloorToInt(exactFrame);
            int indexB = indexA + 1;

            if (indexB < recordedPos.Count)
            {
                // Плавная интерполяция между точками для защиты от подергиваний
                float t = exactFrame - indexA;
                player.transform.position = Vector3.Lerp(recordedPos[indexA], recordedPos[indexB], t);
            }
            else
            {
                // Конец макроса
                isPlaying = false;
                playbackTime = 0f;
            }
        }
    }

    void SaveMacro()
    {
        MacroWrapper wrapper = new MacroWrapper { p = recordedPos };
        File.WriteAllText(filePath, JsonUtility.ToJson(wrapper));
    }

    void LoadMacro()
    {
        if (File.Exists(filePath))
        {
            MacroWrapper wrapper = JsonUtility.FromJson<MacroWrapper>(File.ReadAllText(filePath));
            recordedPos = wrapper.p;
            playbackTime = 0f;
        }
    }

    void RunNoclip()
    {
        if (rb == null) return;
        rb.gravityScale = 0; 
        rb.linearVelocity = Vector2.zero; // В Unity 2025/2026 вместо velocity используется linearVelocity
        
        if (col != null) col.enabled = false;
        float h = Input.GetAxisRaw("Horizontal"), v = Input.GetAxisRaw("Vertical");
        player.transform.position += new Vector3(h, v, 0) * 15f * Time.unscaledDeltaTime;
    }

    void ResetPhysics()
    {
        if (rb != null && rb.gravityScale == 0) 
        { 
            rb.gravityScale = 1; 
            if (col != null) col.enabled = true; 
        }
    }

    void OnGUI()
    {
        if (!isMenuVisible) return;
        GUI.Box(new Rect(10, 10, 260, 420), "SATYR TIME-INDEPENDENT TOOL");

        if (GUI.Button(new Rect(25, 40, 100, 30), isRecording ? "STOP REC" : "RECORD"))
        {
            if (!isRecording) { recordedPos.Clear(); recordTimer = 0f; }
            isRecording = !isRecording; isPlaying = false;
        }
        if (GUI.Button(new Rect(135, 40, 100, 30), isPlaying ? "STOP PLAY" : "PLAY"))
        {
            isPlaying = !isPlaying; isRecording = false; playbackTime = 0f;
        }

        if (GUI.Button(new Rect(25, 80, 100, 30), "SAVE JSON")) SaveMacro();
        if (GUI.Button(new Rect(135, 80, 100, 30), "LOAD JSON")) LoadMacro();

        GUI.Label(new Rect(25, 120, 210, 20), $"Current Speed: {Time.timeScale:F2}x");
        speedInput = GUI.TextField(new Rect(25, 140, 80, 20), speedInput);
        if (GUI.Button(new Rect(115, 140, 120, 20), "APPLY SPEED"))
        {
            if (float.TryParse(speedInput.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out float res))
                speedScale = res;
        }

        isNoclip = GUI.Toggle(new Rect(25, 175, 210, 20), isNoclip, " Enable NOCLIP");
        if (GUI.Button(new Rect(25, 210, 210, 30), "CLEAR DATA")) { recordedPos.Clear(); playbackTime = 0f; }
        
        int currentFrameUI = isPlaying ? Mathf.FloorToInt(playbackTime / RECORD_INTERVAL) : 0;
        GUI.Label(new Rect(25, 250, 210, 50), $"Status: {(isRecording ? "REC" : isPlaying ? "PLAY" : "IDLE")}\nProgress: {currentFrameUI} / {recordedPos.Count}");
    }
}