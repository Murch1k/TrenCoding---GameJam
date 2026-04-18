using UnityEngine;
using System.Collections;

public class MorningRoutine : MonoBehaviour
{
    [Header("Объекты еды")]
    public GameObject foodInHand;
    public GameObject foodInMicrowave;
    public GameObject foodOnDesk;
    public GameObject foodInDrawer; // каша лежащая в ящике

    [Header("Мебель")]
    public GameObject drawerObject;

    [Header("Микроволновка")]
    public Light microwaveLight;       // источник света внутри
    public Color cookingColor = Color.yellow; // цвет лампочки

    [Header("Состояния")]
    public bool hasFood = false;
    public bool foodIsCooked = false;

    private bool isDrawerOpen = false;
    private bool isAnimating = false;

    void Start()
    {
        // В начале каша лежит в ящике
        if (foodInDrawer != null) foodInDrawer.SetActive(true);
        if (microwaveLight != null) microwaveLight.enabled = false;
    }

    // ─── Ящик ───────────────────────────────────────────────
    public void TakeFoodFromDrawer()
    {
        if (isAnimating) return;

        if (!isDrawerOpen)
        {
            StartCoroutine(MoveDrawer(1.02f));
            isDrawerOpen = true;
            Debug.Log("Ящик открыт! Нажми E ещё раз чтобы взять еду.");
        }
        else if (isDrawerOpen && !hasFood && !foodIsCooked)
        {
            // Берём еду из ящика
            if (foodInDrawer != null) foodInDrawer.SetActive(false); // каша исчезает из ящика
            hasFood = true;
            foodInHand.SetActive(true);

            StartCoroutine(MoveDrawer(0.467f)); // закрываем
            isDrawerOpen = false;

            Debug.Log("Еда в руках! Иди к микроволновке.");
        }
    }

    private IEnumerator MoveDrawer(float targetX)
    {
        isAnimating = true;
        float speed = 1.5f;

        Vector3 startPos = drawerObject.transform.localPosition;
        Vector3 endPos = startPos;
        endPos.x = targetX;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * speed;
            drawerObject.transform.localPosition = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        drawerObject.transform.localPosition = endPos;
        isAnimating = false;
    }

    // ─── Микроволновка ──────────────────────────────────────
    public void InteractWithMicrowave()
    {
        if (hasFood && !foodIsCooked)
        {
            hasFood = false;
            foodInHand.SetActive(false);
            foodInMicrowave.SetActive(true);
            StartCoroutine(CookFood());
            Debug.Log("Еда в микроволновке. Ждём 10 секунд...");
        }
        else if (!hasFood && foodIsCooked && foodInMicrowave.activeSelf)
        {
            foodInMicrowave.SetActive(false);
            foodInHand.SetActive(true);
            hasFood = true;

            // Выключаем лампочку когда забираем
            if (microwaveLight != null) microwaveLight.enabled = false;

            Debug.Log("Еда готова! Неси к столу.");
        }
        else if (!hasFood && !foodIsCooked && foodInMicrowave.activeSelf)
        {
            Debug.Log("Ещё готовится, подожди!");
        }
    }

    private IEnumerator CookFood()
    {
        // Включаем лампочку
        if (microwaveLight != null)
        {
            microwaveLight.enabled = true;
            microwaveLight.color = cookingColor;
        }

        Debug.Log("Готовится...");
        yield return new WaitForSeconds(10f);
        foodIsCooked = true;

        // Мигаем когда готово
        StartCoroutine(BlinkLight());
        Debug.Log("Готово! Подойди и забери.");
    }

    private IEnumerator BlinkLight()
    {
        for (int i = 0; i < 6; i++)
        {
            if (microwaveLight != null)
                microwaveLight.enabled = !microwaveLight.enabled;
            yield return new WaitForSeconds(0.2f);
        }
        if (microwaveLight != null) microwaveLight.enabled = true;
    }

    // ─── Стол ───────────────────────────────────────────────
    public void PutFoodOnDesk()
    {
        if (hasFood && foodIsCooked)
        {
            hasFood = false;
            foodInHand.SetActive(false);
            foodOnDesk.SetActive(true);
            Debug.Log("Приятного аппетита!");
        }
        else if (!hasFood)
            Debug.Log("Сначала возьми еду!");
        else if (!foodIsCooked)
            Debug.Log("Еда ещё не готова!");
    }
}