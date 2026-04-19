using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button playButton;
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1.5f;

    [Header("Сцена")]
    [SerializeField] private string targetScene = "scene52";

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Начинаем с чёрного, проявляемся
        if (fadeImage != null)
        {
            var c = fadeImage.color;
            c.a = 1f;
            fadeImage.color = c;
            fadeImage.gameObject.SetActive(true);
        }

        if (playButton != null)
            playButton.onClick.AddListener(OnPlay);

        StartCoroutine(FadeIn());
    }

    private void Update()
    {
        // Любая клавиша тоже запускает игру
        if (Input.anyKeyDown)
            OnPlay();
    }

    private void OnPlay()
    {
        // Отписываемся чтобы не вызвать дважды
        if (playButton != null) playButton.onClick.RemoveAllListeners();
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        yield return StartCoroutine(Fade(0f, 1f));
        SceneManager.LoadScene(targetScene);
    }

    private IEnumerator FadeIn()
    {
        yield return StartCoroutine(Fade(1f, 0f));
    }

    private IEnumerator Fade(float from, float to)
    {
        if (fadeImage == null) yield break;
        fadeImage.gameObject.SetActive(true);
        float t = 0f;
        var c = fadeImage.color;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, t / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }
        c.a = to;
        fadeImage.color = c;
        if (to == 0f) fadeImage.gameObject.SetActive(false);
    }
}