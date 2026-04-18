using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SleepSystem : MonoBehaviour
{
    [Header("UI")]
    public Image fadeImage;
    public TMP_Text dayText;
    public float fadeDuration = 1.5f;
    public float textShowDuration = 2f;

    [Header("Анимация игрока")]
    public Animator playerAnimator;
    public string sleepAnimTrigger = "Sleep";
    public string wakeAnimTrigger = "Wake";
    public float sleepAnimDuration = 2f; // секунд до затемнения

    [Header("Ссылки")]
    public MorningRoutine morningRoutine;
    public GameObject playerBody;        // тело игрока (не камера)
    public Transform sleepPosition;      // куда переместить игрока (у кровати)
    public Transform wakePosition;       // откуда встать

    private bool isSleeping = false;

    void Start()
    {
        // Прячем UI в начале
        SetFadeAlpha(0f);
        if (dayText != null) dayText.gameObject.SetActive(false);
    }

    public bool IsSleeping => isSleeping;

    public void GoToSleep()
    {
        if (isSleeping) return;
        StartCoroutine(SleepRoutine());
    }

    private IEnumerator SleepRoutine()
    {
        isSleeping = true;

        // Блокируем управление
        SetPlayerControl(false);

        // Двигаем игрока к кровати
        if (sleepPosition != null && playerBody != null)
            playerBody.transform.position = sleepPosition.position;

        // Играем анимацию ложиться
        if (playerAnimator != null)
            playerAnimator.SetTrigger(sleepAnimTrigger);

        // Ждём пока ляжет
        yield return new WaitForSeconds(sleepAnimDuration);

        // Затемняем экран
        yield return StartCoroutine(Fade(0f, 1f));

        // Применяем смену дня
        GlobalCycleManager.Instance.AdvanceDay();
        morningRoutine.ResetForNewDay();
        FindFirstObjectByType<PosterChanger>()?.UpdatePoster();
        FindFirstObjectByType<SlenderManager>()?.OnNewDay();
        FindFirstObjectByType<AmbientManager>()?.PlayForCurrentDay();

        // Показываем текст дня
        if (dayText != null)
        {
            dayText.gameObject.SetActive(true);
            dayText.text = "День " + GlobalCycleManager.Instance.currentDay;
        }

        yield return new WaitForSeconds(textShowDuration);

        // Прячем текст
        if (dayText != null) dayText.gameObject.SetActive(false);

        // Двигаем игрока к месту пробуждения
        if (wakePosition != null && playerBody != null)
            playerBody.transform.position = wakePosition.position;

        // Играем анимацию вставать
        if (playerAnimator != null)
            playerAnimator.SetTrigger(wakeAnimTrigger);

        // Осветляем экран
        yield return StartCoroutine(Fade(1f, 0f));

        // Возвращаем управление
        SetPlayerControl(true);
        isSleeping = false;

        Debug.Log("Доброе утро! День " + GlobalCycleManager.Instance.currentDay);
    }

    private IEnumerator Fade(float from, float to)
    {
        if (fadeImage == null) yield break;
        fadeImage.gameObject.SetActive(true);
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            SetFadeAlpha(Mathf.Lerp(from, to, t / fadeDuration));
            yield return null;
        }
        SetFadeAlpha(to);
        if (to == 0f) fadeImage.gameObject.SetActive(false);
    }

    private void SetFadeAlpha(float alpha)
    {
        if (fadeImage == null) return;
        var c = fadeImage.color;
        c.a = alpha;
        fadeImage.color = c;
    }

    private void SetPlayerControl(bool enabled)
    {
        // Отключаем скрипты управления игроком
        var playerInteract = FindFirstObjectByType<PlayerInteract>();
        if (playerInteract != null) playerInteract.enabled = enabled;

        var mouseLook = FindFirstObjectByType<MouseLook>();
        if (mouseLook != null) mouseLook.enabled = enabled;

        // Прячем/показываем курсор
        Cursor.lockState = enabled ? CursorLockMode.Locked : CursorLockMode.None;
    }
}