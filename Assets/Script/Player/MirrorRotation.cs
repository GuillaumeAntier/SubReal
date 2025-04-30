using UnityEngine;

public class MirrorRotation : MonoBehaviour
{
    [Header("Rotation Controls")]
    public KeyCode rotateLeftKey = KeyCode.G;
    public KeyCode rotateRightKey = KeyCode.J;
    public KeyCode rotateUpKey = KeyCode.Y;
    public KeyCode rotateDownKey = KeyCode.H;
    public float rotationSpeed = 45f; 
    
    [Header("References")]
    public ObjectGrab objectGrabSystem;
    
    void Start()
    {
        if (objectGrabSystem == null)
        {
            objectGrabSystem = GetComponent<ObjectGrab>();
            
            if (objectGrabSystem == null)
            {
                objectGrabSystem = FindObjectOfType<ObjectGrab>();
                
                if (objectGrabSystem == null)
                {
                    Debug.LogError("Impossible de trouver le composant ObjectGrab. Veuillez l'assigner manuellement.");
                }
            }
        }
    }
    
    void Update()
    {
        GameObject heldObject = objectGrabSystem.GetHeldObject();
        
        if (heldObject != null && heldObject.GetComponent<LaserMirror>() != null)
        {
            if (Input.GetKey(rotateLeftKey))
            {
                heldObject.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            }
            else if (Input.GetKey(rotateRightKey))
            {
                heldObject.transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
            }
            
            if (Input.GetKey(rotateUpKey))
            {
                heldObject.transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);
            }
            else if (Input.GetKey(rotateDownKey))
            {
                heldObject.transform.Rotate(Vector3.right, -rotationSpeed * Time.deltaTime);
            }
        }
    }
}