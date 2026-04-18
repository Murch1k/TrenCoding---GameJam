using System;
using UnityEngine;

public class SlenderManager : MonoBehaviour
{
    [Header("Настройки")]
    public int appearOnDay = 3;
    public float lookAngleThreshold = 25f;
    public float disappearDelay = 0.5f;

    private Transform playerCamera;
    private bool isVisible = false;
    private bool isDisappearing = false;
    private float disappearTimer = 0f;

    void Start()
    {
        gameObject.SetActive(false);
        playerCamera = Camera.main.transform;
    }

    void Update()
    {
        if (GlobalCycleManager.Instance == null) return;
        if (GlobalCycleManager.Instance.currentDay < appearOnDay) return;

        if (!isVisible && !isDisappearing)
            ShowSlender();

        if (isVisible && !isDisappearing)
        {
            Vector3 dir = (transform.position - playerCamera.position).normalized;
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

    void ShowSlender()
    {
        isVisible = true;
        isDisappearing = false;
        disappearTimer = 0f;
        gameObject.SetActive(true);
    }

    void HideSlender()
    {
        isVisible = false;
        isDisappearing = false;
        gameObject.SetActive(false);
    }

    void StartDisappear()
    {
        isDisappearing = true;
        isVisible = false;
        gameObject.SetActive(false);
        Invoke(nameof(Respawn), UnityEngine.Random.Range(10f, 20f));
    }

    void Respawn()
    {
        if (GlobalCycleManager.Instance == null) return;
        if (GlobalCycleManager.Instance.currentDay < appearOnDay) return;
        isDisappearing = false;
        ShowSlender();
    }

    public void OnNewDay()
    {
        CancelInvoke(nameof(Respawn));
        HideSlender();
        if (GlobalCycleManager.Instance != null &&
            GlobalCycleManager.Instance.currentDay >= appearOnDay)
            Invoke(nameof(ShowSlender), 2f);
    }
}