using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Движение")]
    public float moveSpeed = 4f;

    [Header("Мышь")]
    public float mouseSensitivity = 2f;
    public Transform cameraHolder; // перетащи сюда объект камеры

    private CharacterController cc;
    private float verticalRotation = 0f;

    private void Start()
    {
        cc = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // Движение
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 move = transform.right * h + transform.forward * v;
        cc.SimpleMove(move * moveSpeed);

        // Мышь — поворот тела по горизонтали
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(0f, mouseX, 0f);

        // Мышь — наклон камеры по вертикали
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -80f, 80f);
        if (cameraHolder != null)
            cameraHolder.localEulerAngles = new Vector3(verticalRotation, 0f, 0f);
    }
}