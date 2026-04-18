using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public float interactDistance = 3f;
    public MorningRoutine routineManager; // Перетащи сюда скрипт рутины

    void Update()
    {
        // Если нажали ЛКМ (или "E")
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactDistance))
            {
                // Проверяем, во что попали по тегу
                if (hit.collider.CompareTag("Drawer"))
                {
                    routineManager.TakeFoodFromDrawer();
                    // Чтобы ящик выезжал, можно просто сдвинуть его тут:
                    // hit.transform.localPosition = new Vector3(...);
                }
                else if (hit.collider.CompareTag("Microwave"))
                {
                    routineManager.InteractWithMicrowave();
                }
                else if (hit.collider.CompareTag("Desk"))
                {
                    routineManager.PutFoodOnDesk();
                }
            }
        }
    }
}