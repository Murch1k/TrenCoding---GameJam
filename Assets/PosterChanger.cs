using UnityEngine;

public class PosterChanger : MonoBehaviour
{
    [Header("Постеры для каждого дня (5 штук)")]
    public Material[] dayPosters = new Material[5];

    private Renderer posterRenderer;

    void Start()
    {
        posterRenderer = GetComponent<Renderer>();
        UpdatePoster();
    }

    public void UpdatePoster()
    {
        if (GlobalCycleManager.Instance == null) return;

        int day = GlobalCycleManager.Instance.currentDay;

        // day идёт от 1 до 5, массив от 0 до 4
        int index = Mathf.Clamp(day - 1, 0, dayPosters.Length - 1);

        if (dayPosters[index] != null)
        {
            posterRenderer.material = dayPosters[index];
            Debug.Log("Постер для дня " + day + " установлен.");
        }
        else
        {
            Debug.LogWarning("Постер для дня " + day + " не назначен!");
        }
    }
}