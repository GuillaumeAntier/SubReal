using UnityEngine;

public class MirrorRotation : MonoBehaviour
{
    [Header("Mouse Rotation")]
    public bool useMouseRotation = true;
    public KeyCode mouseRotationKey = KeyCode.Mouse0; // Bouton gauche de la souris
    public float mouseSensitivity = 2.0f;
    
    [Header("Camera Control")]
    public PlayerCam playerCamScript; // Référence au script qui contrôle la caméra
    
    [Header("References")]
    public ObjectGrab objectGrabSystem;
    
    private Transform lastRotatedMirror;
    private bool isRotatingWithMouse = false;
    private Vector3 originalCameraRotation;
    
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
    }
    
    void Update()
    {
        GameObject heldObject = objectGrabSystem.GetHeldObject();
        
        if (heldObject != null && heldObject.GetComponent<LaserMirror>() != null)
        {
            lastRotatedMirror = heldObject.transform;
            
            // Vérifier si le bouton de la souris est enfoncé
            if (Input.GetKeyDown(mouseRotationKey))
            {
                isRotatingWithMouse = true;
                
                // Désactiver le contrôle de la caméra
                if (playerCamScript != null)
                {
                    originalCameraRotation = playerCamScript.transform.eulerAngles;
                    playerCamScript.enabled = false;
                }
            }
            
            // Vérifier si le bouton de la souris est relâché
            if (Input.GetKeyUp(mouseRotationKey))
            {
                isRotatingWithMouse = false;
                
                // Réactiver le contrôle de la caméra
                if (playerCamScript != null)
                {
                    playerCamScript.enabled = true;
                }
            }
            
            // Rotation du miroir si le bouton de la souris est enfoncé
            if (isRotatingWithMouse)
            {
                RotateMirrorWithMouse(heldObject.transform);
            }
        }
        else
        {
            isRotatingWithMouse = false;
            
            // S'assurer que le contrôle de la caméra est activé
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
        
        if (mouseX != 0f)
        {
            mirrorTransform.Rotate(Vector3.up, -mouseX);
        }
        
        if (mouseY != 0f)
        {
            mirrorTransform.Rotate(Vector3.right, mouseY);
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