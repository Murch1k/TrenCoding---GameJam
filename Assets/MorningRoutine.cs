using UnityEngine;
// Убираем лишние using, чтобы не было ошибки "ambiguous reference"

public class MorningRoutine : MonoBehaviour
{
    [Header("Объекты еды")]
    public GameObject foodInHand;
    public GameObject foodInMicrowave;
    public GameObject foodOnDesk;

    [Header("Мебель")]
    public GameObject drawerObject; // Тот самый ящик Drawer.004

    [Header("Состояния")]
    public bool hasFood = false;
    public bool foodIsCooked = false;
    private bool isDrawerOpen = false;

    public void TakeFoodFromDrawer()
    {
        // Если еды нет и ящик закрыт — открываем и даем еду
        if (!hasFood && !foodIsCooked && !isDrawerOpen)
        {
            isDrawerOpen = true;

            // Двигаем ящик по твоим координатам X
            Vector3 pos = drawerObject.transform.localPosition;
            pos.x = 1.02f;
            drawerObject.transform.localPosition = pos;

            hasFood = true;
            foodInHand.SetActive(true);

            UnityEngine.Debug.Log("Ящик открыт, еда в руках");
        }
    }

    public void InteractWithMicrowave()
    {
        if (hasFood && !foodIsCooked)
        {
            hasFood = false;
            foodInHand.SetActive(false);
            foodInMicrowave.SetActive(true);
            StartCoroutine(CookFood());
        }
        else if (!hasFood && foodIsCooked && foodInMicrowave.activeSelf)
        {
            foodInMicrowave.SetActive(false);
            foodInHand.SetActive(true);
            hasFood = true;
        }
    }

    private System.Collections.IEnumerator CookFood()
    {
        UnityEngine.Debug.Log("Готовится...");
        yield return new WaitForSeconds(3f);
        foodIsCooked = true;
        UnityEngine.Debug.Log("Готово! Можно забирать.");
    }

    public void PutFoodOnDesk()
    {
        if (hasFood && foodIsCooked)
        {
            hasFood = false;
            foodInHand.SetActive(false);
            foodOnDesk.SetActive(true);
            UnityEngine.Debug.Log("Приятного аппетита. Пора за работу.");
        }
    }
}