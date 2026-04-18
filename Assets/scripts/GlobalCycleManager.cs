using UnityEngine;

public class GlobalCycleManager : MonoBehaviour
{
    // Статическая ссылка, чтобы любой скрипт мог написать GlobalCycleManager.Instance
    public static GlobalCycleManager Instance { get; private set; }

    [Header("Настройки времени")]
    public int currentDay = 1;
    public bool isWorkDone = false; // Пример: выполнил ли игрок работу за компом сегодня

    private void Awake()
    {
        // Реализация Singleton (Одиночки)
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Объект не удалится при смене сцены
        }
        else
        {
            Destroy(gameObject); // Удаляем дубликат, если мы вернулись в первую сцену
        }
    }

    public void AdvanceDay()
    {
        currentDay++;
        isWorkDone = false; // Сбрасываем флаги для нового дня
        Debug.Log("Наступил день №" + currentDay);
    }
}