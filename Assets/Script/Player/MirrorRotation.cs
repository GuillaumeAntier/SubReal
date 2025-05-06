using UnityEngine;

public class MirrorRotation : MonoBehaviour
{
    [Header("Mouse Rotation")]
    public bool useMouseRotation = true;
    public KeyCode mouseRotationKey = KeyCode.Mouse0; 
    public float mouseSensitivity = 2.0f;
    
    [Header("Camera Control")]
    public PlayerCam playerCamScript; 
    
    [Header("References")]
    public ObjectGrab objectGrabSystem;
    
    private Transform lastRotatedMirror;
    private bool isRotatingWithMouse = false;
    private Vector3 originalCameraRotation;
    private Camera mainCamera; 
    
    void Start()
    {
        if (objectGrabSystem == null)
        {
            objectGrabSystem = GetComponent<ObjectGrab>();
            
            if (objectGrabSystem == null)
            {
                objectGrabSystem = FindFirstObjectByType<ObjectGrab>();
                
                if (objectGrabSystem == null)
                {
                    Debug.LogError("Impossible de trouver le composant ObjectGrab. Veuillez l'assigner manuellement.");
                }
            }
        }
        
        if (playerCamScript == null)
        {
            playerCamScript = GetComponent<PlayerCam>();
            
            if (playerCamScript == null)
            {
                playerCamScript = FindFirstObjectByType<PlayerCam>();
            }
        }
        
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogWarning("Caméra principale non trouvée. Certaines fonctionnalités pourraient ne pas fonctionner correctement.");
        }
    }
    
    void Update()
    {
        GameObject heldObject = objectGrabSystem.GetHeldObject();
        
        if (heldObject != null && heldObject.GetComponent<LaserMirror>() != null)
        {
            lastRotatedMirror = heldObject.transform;
            
            if (Input.GetKeyDown(mouseRotationKey))
            {
                isRotatingWithMouse = true;
                
                if (playerCamScript != null)
                {
                    originalCameraRotation = playerCamScript.transform.eulerAngles;
                    playerCamScript.enabled = false;
                }
            }
            
            if (Input.GetKeyUp(mouseRotationKey))
            {
                isRotatingWithMouse = false;
                
                if (playerCamScript != null)
                {
                    playerCamScript.enabled = true;
                }
            }
            
            if (isRotatingWithMouse)
            {
                RotateMirrorWithMouse(heldObject.transform);
            }
        }
        else
        {
            isRotatingWithMouse = false;
            
            if (playerCamScript != null && !playerCamScript.enabled)
            {
                playerCamScript.enabled = true;
            }
        }
    }
    
    private void RotateMirrorWithMouse(Transform mirrorTransform)
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        if (mainCamera != null)
        {
            if (mouseX != 0f)
            {
                mirrorTransform.Rotate(Vector3.up, -mouseX, Space.World);
            }
            
            if (mouseY != 0f)
            {
                Vector3 cameraRight = mainCamera.transform.right;
                mirrorTransform.Rotate(cameraRight, mouseY, Space.World);
            }
        }
        else
        {
            if (mouseX != 0f)
            {
                mirrorTransform.Rotate(Vector3.up, -mouseX);
            }
            
            if (mouseY != 0f)
            {
                mirrorTransform.Rotate(Vector3.right, mouseY);
            }
        }
    }
    
    public Transform GetLastRotatedMirror()
    {
        return lastRotatedMirror;
    }
    
    public bool IsRotatingWithMouse()
    {
        return isRotatingWithMouse;
    }
}