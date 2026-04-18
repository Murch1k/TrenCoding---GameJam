using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInteract : MonoBehaviour
{
    public float interactDistance = 5f;
    public MorningRoutine routineManager;
    public string computerSceneName = "SampleScene";

    void Update()
    {
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
            Debug.Log("Загружаем сцену с компьютером...");
            SceneManager.LoadScene(computerSceneName);
        }
        else
        {
            Debug.Log("Сначала поешь!");
        }
    }

    private void TryGoToSleep()
    {
        // Спать можно только если поработал за компом
        if (GlobalCycleManager.Instance != null && GlobalCycleManager.Instance.isWorkDone)
        {
            Debug.Log("Спокойной ночи...");
            GlobalCycleManager.Instance.AdvanceDay();
            routineManager.ResetForNewDay();
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