using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;
    public Image fadePanel; // Чёрная панель для фейда
    public float fadeDuration = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Фейд ин при загрузке любой сцены
        StartCoroutine(FadeIn());
    }

    public IEnumerator FadeOut()
    {
        fadePanel.gameObject.SetActive(true);
        float timer = 0f;
        Color color = fadePanel.color;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            fadePanel.color = color;
            yield return null;
        }
        color.a = 1f;
        fadePanel.color = color;
    }

    public IEnumerator FadeIn()
    {
        float timer = 0f;
        Color color = fadePanel.color;
        color.a = 1f;
        fadePanel.color = color;
        fadePanel.gameObject.SetActive(true);

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            fadePanel.color = color;
            yield return null;
        }
        color.a = 0f;
        fadePanel.color = color;
        fadePanel.gameObject.SetActive(false);
    }

    // УНИВЕРСАЛЬНЫЙ МЕТОД — работает и с string, и с int!
    public void LoadSceneWithFade(object sceneIdentifier)
    {
        if (sceneIdentifier is string sceneName)
        {
            StartCoroutine(LoadSceneRoutine(sceneName));
        }
        else if (sceneIdentifier is int sceneIndex)
        {
            StartCoroutine(LoadSceneRoutine(sceneIndex));
        }
        else
        {
            Debug.LogError("Бро, передай нормальный sceneIdentifier — string или int!");
        }
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        yield return StartCoroutine(FadeOut());
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator LoadSceneRoutine(int sceneIndex)
    {
        yield return StartCoroutine(FadeOut());
        SceneManager.LoadScene(sceneIndex);
    }
}