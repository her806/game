using UnityEngine;

public class perexod : MonoBehaviour
{
    [Header("Настройки перехода")]
    public string sceneToLoad = "NextLevel"; // Имя сцены
    public int sceneIndex = -1; // Если >= 0 — приоритет у индекса

    [Header("Коллайдер триггера (для удобства)")]
    [Tooltip("Любой коллайдер на этом объекте — для валидации в редакторе")]
    public Collider2D triggerCollider;

    private void Reset()
    {
        triggerCollider = GetComponent<Collider2D>();
    }

    private void OnValidate()
    {
        if (triggerCollider != null && !triggerCollider.isTrigger)
        {
            Debug.LogWarning($"Коллайдер на {gameObject.name} НЕ триггер! Поставь Is Trigger = true", this);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (sceneIndex >= 0)
            {
                FadeManager.Instance.LoadSceneWithFade(sceneIndex); // int — работает
            }
            else
            {
                FadeManager.Instance.LoadSceneWithFade(sceneToLoad); // string — работает
            }
        }
    }
}