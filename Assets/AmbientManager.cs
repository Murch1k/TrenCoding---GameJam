using UnityEngine;

public class AmbientManager : MonoBehaviour
{
    [Header("Фоновые звуки по дням")]
    public AudioClip[] dailySounds; // 5 слотов, по одному на день

    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.spatialBlend = 0f; // 2D
        audioSource.volume = 0.4f;

        PlayForCurrentDay();
    }

    public void PlayForCurrentDay()
    {
        if (GlobalCycleManager.Instance == null) return;
        if (dailySounds == null || dailySounds.Length == 0) return;

        int day = GlobalCycleManager.Instance.currentDay;
        int index = Mathf.Clamp(day - 1, 0, dailySounds.Length - 1);

        // Не переключаем если тот же звук уже играет
        if (audioSource.clip == dailySounds[index] && audioSource.isPlaying) return;

        audioSource.clip = dailySounds[index];
        audioSource.Play();
        Debug.Log("Фоновый звук для дня " + day);
    }
}