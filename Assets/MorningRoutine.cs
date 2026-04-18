using UnityEngine;
using System.Collections;

public class MorningRoutine : MonoBehaviour
{
    [Header("Объекты еды")]
    public GameObject foodInHand;
    public GameObject foodInMicrowave;
    public GameObject foodOnDesk;
    public GameObject foodInDrawer;

    [Header("Мебель")]
    public GameObject drawerObject;

    [Header("Микроволновка")]
    public Light microwaveLight;
    public Color cookingColor = Color.yellow;

    [Header("Состояния")]
    public bool hasFood = false;
    public bool foodIsCooked = false;

    private bool isDrawerOpen = false;
    private bool isAnimating = false;

    void Start()
    {
        if (GlobalCycleManager.Instance != null && GlobalCycleManager.Instance.justReturnedFromPC)
        {
            RestoreState();
            Debug.Log("Работа сделана! Иди спать.");
            return;
        }

        ResetForNewDay();
    }

    private void RestoreState()
    {
        var g = GlobalCycleManager.Instance;

        // Восстанавливаем еду
        hasFood = g.savedHasFood;
        foodIsCooked = g.savedFoodIsCooked;

        if (foodInHand != null) foodInHand.SetActive(g.savedFoodInHand);
        if (foodInMicrowave != null) foodInMicrowave.SetActive(g.savedFoodInMicrowave);
        if (foodOnDesk != null) foodOnDesk.SetActive(g.savedFoodOnDesk);
        if (foodInDrawer != null) foodInDrawer.SetActive(g.savedFoodInDrawer);

        if (drawerObject != null)
        {
            Vector3 pos = drawerObject.transform.localPosition;
            pos.x = 0.467f;
            drawerObject.transform.localPosition = pos;
        }

        if (microwaveLight != null) microwaveLight.enabled = false;

        // Восстанавливаем позицию игрока
        // Ищем тело игрока (объект с тегом Player)
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.transform.position = g.savedPlayerPosition;
            player.transform.eulerAngles = new Vector3(0, g.savedPlayerRotationY, 0);
        }
    }



    // ─── Сброс для нового дня ────────────────────────────────
    public void ResetForNewDay()
    {
        hasFood = false;
        foodIsCooked = false;
        isDrawerOpen = false;
        isAnimating = false;

        if (foodInHand != null) foodInHand.SetActive(false);
        if (foodInMicrowave != null) foodInMicrowave.SetActive(false);
        if (foodOnDesk != null) foodOnDesk.SetActive(false);
        if (foodInDrawer != null) foodInDrawer.SetActive(true); // каша снова в ящике

        // Ящик закрыт
        if (drawerObject != null)
        {
            Vector3 pos = drawerObject.transform.localPosition;
            pos.x = 0.467f;
            drawerObject.transform.localPosition = pos;
        }

        if (microwaveLight != null) microwaveLight.enabled = false;

        Debug.Log("День " + (GlobalCycleManager.Instance != null ? GlobalCycleManager.Instance.currentDay : 1) + ". Доброе утро!");
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
            if (foodInDrawer != null) foodInDrawer.SetActive(false);
            hasFood = true;
            foodInHand.SetActive(true);
            StartCoroutine(MoveDrawer(0.467f));
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
        if (microwaveLight != null)
        {
            microwaveLight.enabled = true;
            microwaveLight.color = cookingColor;
        }
        yield return new WaitForSeconds(10f);
        foodIsCooked = true;
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
            Debug.Log("Приятного аппетита! Теперь садись за комп.");
        }
        else if (!hasFood)
            Debug.Log("Сначала возьми еду!");
        else if (!foodIsCooked)
            Debug.Log("Еда ещё не готова!");
    }
}