using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 60f;

    public Camera playerCamera;
    private Rigidbody rb;
    private float cameraPitch = 0f;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>();
            
            if (playerCamera == null)
            {
                playerCamera = Camera.main;
                Debug.LogWarning("Aucune caméra enfant trouvée. Utilisation de la caméra principale.");
            }
        }

        if (playerCamera == null)
        {
            Debug.LogError("Aucune caméra trouvée. Veuillez assigner une caméra au script PlayerController.");
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (playerCamera == null) return;
        
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(Vector3.up, mouseX);

        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -maxLookAngle, maxLookAngle);
        playerCamera.transform.localRotation = Quaternion.Euler(cameraPitch, 0, 0);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void FixedUpdate()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = transform.forward * verticalInput + transform.right * horizontalInput;

        if (movement.magnitude > 1)
        {
            movement.Normalize();
        }

        Vector3 velocity = new Vector3(movement.x * moveSpeed, rb.linearVelocity.y, movement.z * moveSpeed);
        rb.linearVelocity = velocity;

        CheckGrounded();
    }

    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
    }

    void CheckGrounded()
    {
        float rayLength = 1.1f;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, rayLength);
    }
}