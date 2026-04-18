using UnityEngine;

public class RescaleToTexture : MonoBehaviour
{
    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null && renderer.material.mainTexture != null)
        {
            float width = renderer.material.mainTexture.width;
            float height = renderer.material.mainTexture.height;

            // Устанавливаем масштаб объекта пропорционально текстуре
            transform.localScale = new Vector3(width / height, 1, 1);
            // Если используешь Plane, возможно нужно (width/height, 1, 1) или наоборот
        }
    }
}