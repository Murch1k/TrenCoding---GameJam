using UnityEngine;

public class GlobalCycleManager : MonoBehaviour
{
    public static GlobalCycleManager Instance { get; private set; }

    public int currentDay = 1;
    public bool isWorkDone = false;
    public bool justReturnedFromPC = false;

    // Сохранённое состояние сцены
    [HideInInspector] public Vector3 savedPlayerPosition;
    [HideInInspector] public float savedPlayerRotationY;
    [HideInInspector] public bool savedFoodOnDesk;
    [HideInInspector] public bool savedFoodInHand;
    [HideInInspector] public bool savedFoodInMicrowave;
    [HideInInspector] public bool savedFoodInDrawer;
    [HideInInspector] public bool savedHasFood;
    [HideInInspector] public bool savedFoodIsCooked;

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

    public void AdvanceDay()
    {
        currentDay++;
        isWorkDone = false;
        justReturnedFromPC = false;
        Debug.Log("Наступил день №" + currentDay);
    }
}