using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public float interactDistance = 5f;
    public MorningRoutine routineManager;

    void Update()
    {
        Debug.DrawRay(transform.position, transform.forward * interactDistance, Color.red);

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactDistance))
            {
                // Ищем тег на объекте, его родителе и деде
                string tag = GetTagInParents(hit.collider.transform);

                Debug.Log("Попал в: " + hit.collider.gameObject.name + " | Тег: " + tag);

                if (tag == "Drawer")
                    routineManager.TakeFoodFromDrawer();
                else if (tag == "Microwave")
                    routineManager.InteractWithMicrowave();
                else if (tag == "Desk")
                    routineManager.PutFoodOnDesk();
            }
            else
            {
                Debug.Log("Ни во что не попал.");
            }
        }
    }

    // Ищет тег вверх по иерархии (на случай вложенных объектов)
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