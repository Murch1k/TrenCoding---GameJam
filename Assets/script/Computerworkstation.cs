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
    [SerializeField] private GameObject feedPanel;        // Панель с постом
    [SerializeField] private GameObject emailPanel;       // Панель почты
    [SerializeField] private Button emailTabButton;       // Кнопка ПОЧТА
    [SerializeField] private Button feedTabButton;        // Кнопка ЛЕНТА
    [SerializeField] private GameObject dayCompletePanel;
    [SerializeField] private TMP_Text progressText;

    [Header("UI — переход")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;

    [Header("Игрок")]
    [SerializeField] private MonoBehaviour playerController;
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private Camera workstationCamera;

    [Header("Вход/выход")]
    [SerializeField] private KeyCode exitKey = KeyCode.Escape;
    [SerializeField] private ComputerInteractZone interactZone;

    public bool IsActive { get; private set; }

    private Queue<PostData> todayQueue = new Queue<PostData>();
    private int processedCount = 0;
    private PostData currentPost;

    private void Start()
    {
        if (workstationCanvas != null) workstationCanvas.gameObject.SetActive(false);
        if (dayCompletePanel != null) dayCompletePanel.SetActive(false);
        if (emailPanel != null) emailPanel.SetActive(false);
        if (fadeImage != null)
        {
            var c = fadeImage.color; c.a = 0f; fadeImage.color = c;
            fadeImage.gameObject.SetActive(false);
        }
        if (postUI != null)
        {
            postUI.OnDeleteClicked += HandleDelete;
            postUI.OnApproveClicked += HandleApprove;
        }
        if (emailTabButton != null) emailTabButton.onClick.AddListener(ShowEmailPanel);
        if (feedTabButton != null) feedTabButton.onClick.AddListener(ShowFeedPanel);
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

    public void EnterComputer()
    {
        if (IsActive) return;
        StartCoroutine(EnterRoutine());
    }

    private IEnumerator EnterRoutine()
    {
        IsActive = true;
        yield return StartCoroutine(Fade(0f, 1f));

        if (playerController != null) playerController.enabled = false;
        if (playerCamera != null) playerCamera.SetActive(false);
        if (workstationCamera != null) workstationCamera.gameObject.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

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

        if (workstationCanvas != null) workstationCanvas.gameObject.SetActive(false);
        if (workstationCamera != null) workstationCamera.gameObject.SetActive(false);
        if (playerCamera != null) playerCamera.SetActive(true);
        if (playerController != null) playerController.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        IsActive = false;
        GameEvents.RaiseExitComputer();
        if (interactZone != null) interactZone.OnWorkstationExited();

        yield return StartCoroutine(Fade(1f, 0f));
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
        c.a = to; fadeImage.color = c;
        if (to == 0f) fadeImage.gameObject.SetActive(false);
    }
}