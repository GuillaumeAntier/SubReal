using UnityEngine;

public class ObjectGrab : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;  
    public Transform holdPosition;

    [Header("Grab Settings")]
    public float grabDistance = 3f;
    public float throwForce = 10f;
    public LayerMask grabbableLayer;
    public KeyCode grabKey = KeyCode.E;
    public KeyCode throwKey = KeyCode.F;

    private GameObject heldObject;
    private Rigidbody heldRigidbody;
    private bool isHolding = false;

    void Start()
    {
        if (playerCamera == null)
        {
            Debug.LogError("Player Camera not assigned on ObjectGrab script!");
        }
        
        if (holdPosition == null)
        {
            Debug.LogError("Hold Position not assigned on ObjectGrab script!");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(grabKey))
        {
            if (!isHolding)
                GrabObject();
            else
                DropObject();
        }

        if (Input.GetKeyDown(throwKey) && isHolding)
        {
            ThrowObject();
        }

        if (isHolding && heldObject != null)
        {
            heldRigidbody.linearVelocity = (holdPosition.position - heldObject.transform.position) * 10f;
            
            heldRigidbody.angularVelocity = Vector3.Lerp(heldRigidbody.angularVelocity, Vector3.zero, Time.deltaTime * 5f);
        }
    }

    void GrabObject()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, grabDistance, grabbableLayer))
        {   
            Debug.Log("Hit object: " + hit.collider.gameObject.name);
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                heldObject = hit.collider.gameObject;
                heldRigidbody = rb;
                
                heldRigidbody.useGravity = false;
                heldRigidbody.linearDamping = 10;
                heldRigidbody.angularDamping = 10;
                heldRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
                
                isHolding = true;
                Debug.Log("Grabbed object: " + heldObject.name);
            }
        }
        else
        {
            Debug.Log("No grabbable object in range.");
        }
    }

    void DropObject()
    {
        if (heldObject != null)
        {
            heldRigidbody.useGravity = true;
            heldRigidbody.linearDamping = 1;
            heldRigidbody.angularDamping = 0.05f;
            
            Debug.Log("Dropped object: " + heldObject.name);
            
            heldObject = null;
            heldRigidbody = null;
            isHolding = false;
        }
    }

    void ThrowObject()
    {
        if (heldObject != null)
        {
            heldRigidbody.useGravity = true;
            heldRigidbody.linearDamping = 1;
            heldRigidbody.angularDamping = 0.05f;
            
            heldRigidbody.AddForce(playerCamera.transform.forward * throwForce, ForceMode.Impulse);
            
            heldObject = null;
            heldRigidbody = null;
            isHolding = false;
        }
    }
}