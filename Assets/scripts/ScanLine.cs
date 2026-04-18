using UnityEngine;
using UnityEngine.UI;

public class ScanLines : MonoBehaviour
{
    public float speed = 80f;
    private RawImage movingLine;

    void Start()
    {
        // Статичные редкие полосы
        int h = 60;
        Texture2D tex = new Texture2D(1, h);
        tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Repeat;

        for (int i = 0; i < h; i++)
        {
            if (i < 1)
                tex.SetPixel(0, i, new Color(0.5f, 0f, 0f, 0.15f));
            else
                tex.SetPixel(0, i, Color.clear);
        }
        tex.Apply();

        var staticImg = GetComponent<RawImage>();
        staticImg.texture = tex;
        staticImg.color = Color.white;
        staticImg.uvRect = new Rect(0, 0, 1, Screen.height / h);

        // Создаём отдельную движущуюся полосу
        GameObject line = new GameObject("MovingLine");
        line.transform.SetParent(transform.parent);
        line.transform.SetSiblingIndex(transform.GetSiblingIndex() + 1);

        movingLine = line.AddComponent<RawImage>();
        movingLine.color = new Color(0.8f, 0f, 0f, 0.4f);

        RectTransform rt = line.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(1, 0);
        rt.sizeDelta = new Vector2(0, 2);
        rt.anchoredPosition = new Vector2(0, Screen.height);
    }

    void Update()
    {
        RectTransform rt = movingLine.GetComponent<RectTransform>();
        rt.anchoredPosition -= new Vector2(0, speed * Time.deltaTime);
        if (rt.anchoredPosition.y < -10)
            rt.anchoredPosition = new Vector2(0, Screen.height);
    }
}