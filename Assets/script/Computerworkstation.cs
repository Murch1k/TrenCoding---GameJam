using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ComputerWorkstation : MonoBehaviour
{
    [Header("Данные")]
    [SerializeField] private PostDatabase postDatabase;
    [SerializeField] private List<EmailData> allEmails = new List<EmailData>();
    [SerializeField] private int postsPerDay = 8;
    [SerializeField] private int currentDay = 1;

    [Header("UI — рабочий стол")]
    [SerializeField] private Canvas workstationCanvas;
    [SerializeField] private PostUI postUI;
    [SerializeField] private EmailUI emailUI;
    [SerializeField] private GameObject feedPanel;
    [SerializeField] private GameObject emailPanel;
    [SerializeField] private Button emailTabButton;
    [SerializeField] private Button feedTabButton;
    [SerializeField] private GameObject dayCompletePanel;
    [SerializeField] private TMP_Text progressText;

    [Header("UI — переход")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;

    [Header("Вход/выход")]
    [SerializeField] private KeyCode exitKey = KeyCode.Escape;

    public bool IsActive { get; private set; }

    private Queue<PostData> todayQueue = new Queue<PostData>();
    private int processedCount = 0;
    private PostData currentPost;

    private void Start()
    {
        // Прячем всё
        if (workstationCanvas != null) workstationCanvas.gameObject.SetActive(false);
        if (dayCompletePanel != null) dayCompletePanel.SetActive(false);
        if (emailPanel != null) emailPanel.SetActive(false);

        // Fade сразу чёрный
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            var c = fadeImage.color;
            c.a = 1f;
            fadeImage.color = c;
        }

        if (postUI != null)
        {
            postUI.OnDeleteClicked += HandleDelete;
            postUI.OnApproveClicked += HandleApprove;
        }

        if (emailTabButton != null) emailTabButton.onClick.AddListener(ShowEmailPanel);
        if (feedTabButton != null) feedTabButton.onClick.AddListener(ShowFeedPanel);

        // Сразу входим
        StartCoroutine(EnterRoutine());
    }

    private void OnDestroy()
    {
        if (postUI != null)
        {
            postUI.OnDeleteClicked -= HandleDelete;
            postUI.OnApproveClicked -= HandleApprove;
        }
    }

    private void Update()
    {
        if (IsActive && Input.GetKeyDown(exitKey))
            ExitComputer();
    }

    public void ShowEmailPanel()
    {
        if (feedPanel != null) feedPanel.SetActive(false);
        if (emailPanel != null) emailPanel.SetActive(true);
    }

    public void ShowFeedPanel()
    {
        if (emailPanel != null) emailPanel.SetActive(false);
        if (feedPanel != null) feedPanel.SetActive(true);
    }

    // Вход — вызывается автоматически при старте
    private IEnumerator EnterRoutine()
    {
        IsActive = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Показываем UI пока ещё чёрный экран
        if (workstationCanvas != null) workstationCanvas.gameObject.SetActive(true);
        if (dayCompletePanel != null) dayCompletePanel.SetActive(false);
        if (emailPanel != null) emailPanel.SetActive(false);
        if (feedPanel != null) feedPanel.SetActive(true);

        if (emailUI != null)
        {
            var todayEmails = allEmails
                .Where(e => e != null && e.showOnDay == currentDay)
                .ToList();
            emailUI.SetEmails(todayEmails);
        }

        BuildTodayQueue();
        processedCount = 0;
        ShowNextPost();
        GameEvents.RaiseEnterComputer();

        // Проявляемся из чёрного
        yield return StartCoroutine(Fade(1f, 0f));
    }

    public void ExitComputer()
    {
        if (!IsActive) return;
        StartCoroutine(ExitRoutine());
    }

    private IEnumerator ExitRoutine()
    {
        yield return StartCoroutine(Fade(0f, 1f));
        IsActive = false;
        GameEvents.RaiseExitComputer();
        // Здесь можно загрузить другую сцену:
        // UnityEngine.SceneManagement.SceneManager.LoadScene("HomeScene");
    }

    private void BuildTodayQueue()
    {
        todayQueue.Clear();
        var posts = postDatabase.GetRandomPosts(postsPerDay, currentDay);
        foreach (var p in posts) todayQueue.Enqueue(p);
    }

    private void ShowNextPost()
    {
        if (todayQueue.Count == 0) { FinishWorkDay(); return; }
        currentPost = todayQueue.Dequeue();
        postUI.Display(currentPost);
        UpdateProgress();
    }

    private void UpdateProgress()
    {
        if (progressText != null)
            progressText.text = $"{processedCount} / {postsPerDay}";
    }

    private void HandleDelete() => Judge(true);
    private void HandleApprove() => Judge(false);

    private void Judge(bool shouldDelete)
    {
        if (currentPost == null) return;
        bool correct = (shouldDelete == currentPost.isHarmful);
        if (correct) GameEvents.RaiseCorrect();
        else GameEvents.RaiseWrong();
        processedCount++;
        ShowNextPost();
    }

    private void FinishWorkDay()
    {
        if (dayCompletePanel != null) dayCompletePanel.SetActive(true);
        postUI.Clear();
        UpdateProgress();
        GameEvents.RaiseWorkDayComplete();
    }

    private IEnumerator Fade(float from, float to)
    {
        if (fadeImage == null) yield break;
        fadeImage.gameObject.SetActive(true);
        float t = 0f;
        var c = fadeImage.color;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(from, to, t / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }
        c.a = to;
        fadeImage.color = c;
        if (to == 0f) fadeImage.gameObject.SetActive(false);
    }
}