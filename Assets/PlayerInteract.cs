using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInteract : MonoBehaviour
{
    public float interactDistance = 5f;
    public MorningRoutine routineManager;
    public string computerSceneName = "SampleScene";

    private SleepSystem sleepSystem;

    void Start()
    {
        sleepSystem = FindFirstObjectByType<SleepSystem>();
    }

    void Update()
    {
        // Блокируем взаимодействие пока идёт анимация сна
        if (sleepSystem != null && sleepSystem.IsSleeping) return;

        Debug.DrawRay(transform.position, transform.forward * interactDistance, Color.red);

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactDistance))
            {
                string tag = GetTagInParents(hit.collider.transform);
                Debug.Log("Попал в: " + hit.collider.gameObject.name + " | Тег: " + tag);

                if (tag == "Drawer")
                    routineManager.TakeFoodFromDrawer();
                else if (tag == "Microwave")
                    routineManager.InteractWithMicrowave();
                else if (tag == "Desk")
                    routineManager.PutFoodOnDesk();
                else if (tag == "PC")
                    TryUsePC();
                else if (tag == "Bed")
                    TryGoToSleep();
            }
            else
            {
                Debug.Log("Ни во что не попал.");
            }
        }
    }

    private void TryUsePC()
    {
        if (routineManager.foodOnDesk.activeSelf)
        {
            SaveState();
            Debug.Log("Загружаем сцену с компьютером...");
            SceneManager.LoadScene(computerSceneName);
        }
        else
        {
            Debug.Log("Сначала поешь!");
        }
    }

    private void SaveState()
    {
        var g = GlobalCycleManager.Instance;
        if (g == null) return;

        g.savedPlayerPosition = transform.parent != null
            ? transform.parent.position
            : transform.position;
        g.savedPlayerRotationY = transform.parent != null
            ? transform.parent.eulerAngles.y
            : transform.eulerAngles.y;

        g.savedFoodOnDesk = routineManager.foodOnDesk.activeSelf;
        g.savedFoodInHand = routineManager.foodInHand.activeSelf;
        g.savedFoodInMicrowave = routineManager.foodInMicrowave.activeSelf;
        g.savedFoodInDrawer = routineManager.foodInDrawer != null && routineManager.foodInDrawer.activeSelf;
        g.savedHasFood = routineManager.hasFood;
        g.savedFoodIsCooked = routineManager.foodIsCooked;
    }

    private void TryGoToSleep()
    {
        if (GlobalCycleManager.Instance == null) return;

        if (GlobalCycleManager.Instance.isWorkDone)
        {
            if (sleepSystem != null)
            {
                sleepSystem.GoToSleep();
            }
            else
            {
                // Запасной вариант без анимации
                Debug.Log("Спокойной ночи...");
                GlobalCycleManager.Instance.AdvanceDay();
                routineManager.ResetForNewDay();
                FindFirstObjectByType<PosterChanger>()?.UpdatePoster();
                FindFirstObjectByType<SlenderManager>()?.OnNewDay();
                FindFirstObjectByType<AmbientManager>()?.PlayForCurrentDay();
            }
        }
        else
        {
            Debug.Log("Сначала нужно поработать за компьютером!");
        }
    }

    private string GetTagInParents(Transform t)
    {
        while (t != null)
        {
            if (t.tag != "Untagged")
                return t.tag;
            t = t.parent;
        }
        return "Untagged";
    }
}