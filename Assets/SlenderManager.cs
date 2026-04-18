using UnityEngine;

[System.Serializable]
public class DayAppear
{
    public int day;
    public bool shouldAppear;
}

public class SlenderManager : MonoBehaviour
{
    [Header("Настройки")]
    public float lookAngleThreshold = 25f;
    public float disappearDelay = 0.5f;
    public GameObject slenderObject;

    [Header("Позиции по дням (пустые объекты в сцене)")]
    public Transform[] dayPositionTransforms;

    [Header("Появление по дням")]
    public DayAppear[] dayAppearSettings;

    [Header("Звуки")]
    public AudioClip appearSound;
    public AudioClip disappearSound;

    private Transform playerCamera;
    private bool isVisible = false;
    private bool isDisappearing = false;
    private bool hasAppearedToday = false;
    private float disappearTimer = 0f;
    private Animator animator;
    private AudioSource audioSource;

    void Start()
    {
        playerCamera = Camera.main.transform;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f;

        if (slenderObject != null)
        {
            slenderObject.SetActive(false);
            animator = slenderObject.GetComponent<Animator>();
        }

        // Запускаем появление через 3 сек если нужен в этот день
        DayAppear current = GetCurrentDayAppear();
        if (current != null && current.shouldAppear)
            Invoke(nameof(ShowSlender), 3f);
    }

    void Update()
    {
        if (GlobalCycleManager.Instance == null) return;

        DayAppear current = GetCurrentDayAppear();

        // Если в этот день не появляется — прячем
        if (current == null || !current.shouldAppear)
        {
            if (isVisible) HideSlender();
            return;
        }

        // Проверяем взгляд только если видим слендера
        if (isVisible && !isDisappearing)
        {
            Vector3 dir = (slenderObject.transform.position - playerCamera.position).normalized;
            float angle = Vector3.Angle(playerCamera.forward, dir);

            if (angle < lookAngleThreshold)
            {
                disappearTimer += Time.deltaTime;
                if (disappearTimer >= disappearDelay)
                    StartDisappear();
            }
            else
            {
                disappearTimer = 0f;
            }
        }
    }

    DayAppear GetCurrentDayAppear()
    {
        if (dayAppearSettings == null) return null;
        int day = GlobalCycleManager.Instance != null
            ? GlobalCycleManager.Instance.currentDay
            : 1;
        foreach (var d in dayAppearSettings)
            if (d.day == day) return d;
        return null;
    }

    void ShowSlender()
    {
        if (hasAppearedToday) return; // уже появлялся сегодня
        if (GlobalCycleManager.Instance == null) return;

        DayAppear current = GetCurrentDayAppear();
        if (current == null || !current.shouldAppear) return;

        hasAppearedToday = true;
        isVisible = true;
        isDisappearing = false;
        disappearTimer = 0f;

        // Ставим на позицию маркера текущего дня
        int index = GlobalCycleManager.Instance.currentDay - 1;
        if (dayPositionTransforms != null
            && index < dayPositionTransforms.Length
            && dayPositionTransforms[index] != null)
        {
            slenderObject.transform.position = dayPositionTransforms[index].position;
            slenderObject.transform.rotation = dayPositionTransforms[index].rotation;
        }

        if (slenderObject != null) slenderObject.SetActive(true);
        if (animator != null) animator.SetTrigger("Idle");
        if (appearSound != null) audioSource.PlayOneShot(appearSound);

        Debug.Log("Слендер появился на день " + GlobalCycleManager.Instance.currentDay);
    }

    void HideSlender()
    {
        isVisible = false;
        isDisappearing = false;
        if (slenderObject != null) slenderObject.SetActive(false);
    }

    void StartDisappear()
    {
        isDisappearing = true;
        isVisible = false;

        if (animator != null) animator.SetTrigger("Scream");
        if (disappearSound != null) audioSource.PlayOneShot(disappearSound);

        Invoke(nameof(HideAfterScream), 1.5f);
        Debug.Log("Слендер исчез!");
    }

    void HideAfterScream()
    {
        if (slenderObject != null) slenderObject.SetActive(false);
        // Больше не появляется — только один раз за день
    }

    public void OnNewDay()
    {
        CancelInvoke();
        HideSlender();

        isDisappearing = false;
        isVisible = false;
        hasAppearedToday = false; // сброс для нового дня

        DayAppear current = GetCurrentDayAppear();
        if (current != null && current.shouldAppear)
            Invoke(nameof(ShowSlender), 3f); // появится через 3 сек
    }
}