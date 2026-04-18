using UnityEngine;
using UnityEngine.SceneManagement;

public class PCInteraction : MonoBehaviour
{
    // Указываем точный путь или имя из SampleScene
    [SerializeField] private string computerSceneName = "SampleScene";
    private bool playerIsNear = false;

    void Update()
    {
        // Если игрок рядом и нажал E
        if (playerIsNear && Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene(computerSceneName);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = true;
            // Можно вывести подсказку в консоль для проверки
            Debug.Log("Нажми E, чтобы войти в компьютер");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = false;
        }
    }
}