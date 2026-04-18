using UnityEngine;

// Вешается на объект компьютера с SphereCollider (isTrigger = true).
// Капсула игрока должна иметь тег "Player".
// Когда игрок в зоне и жмёт E — вызывает ComputerWorkstation.EnterComputer().
[RequireComponent(typeof(Collider))]
public class ComputerInteractZone : MonoBehaviour
{
    [SerializeField] private ComputerWorkstation workstation;
    [SerializeField] private GameObject interactPrompt; // UI "[E] Войти в систему" (World Space Canvas или обычный)
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private bool playerInRange = false;

    private void Start()
    {
        if (interactPrompt != null) interactPrompt.SetActive(false);
    }

    private void Update()
    {
        if (!playerInRange) return;
        if (workstation != null && workstation.IsActive) return; // уже сидим

        if (Input.GetKeyDown(interactKey))
        {
            workstation.EnterComputer();
            if (interactPrompt != null) interactPrompt.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactPrompt != null && !workstation.IsActive)
                interactPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactPrompt != null) interactPrompt.SetActive(false);
        }
    }

    // Вызывается из ComputerWorkstation при выходе — снова показать промпт если игрок рядом
    public void OnWorkstationExited()
    {
        if (playerInRange && interactPrompt != null)
            interactPrompt.SetActive(true);
    }
}