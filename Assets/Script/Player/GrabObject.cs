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
    public LayerMask playerLayer;
    
    [Header("Hold Distance Settings")]
    public float minHoldDistance = 1f;
    public float maxHoldDistance = 5f;
    public float holdDistanceScrollSpeed = 0.5f;
    private float currentHoldDistance = 2f;

    [Header("Audio")]
    [SerializeField] private AudioSource grabAudioSource;
    [SerializeField] private AudioClip grabSound;
    [SerializeField] private AudioClip dropSound;
    [SerializeField] private AudioClip throwSound;
    
    private GameObject heldObject;
    private Rigidbody heldRigidbody;
    private bool isHolding = false;
    private int originalLayer;

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
        
        currentHoldDistance = Mathf.Clamp((minHoldDistance + maxHoldDistance) / 2f, minHoldDistance, maxHoldDistance);
        
        if (grabAudioSource == null)
        {
            grabAudioSource = GetComponent<AudioSource>();
            if (grabAudioSource == null)
            {
                grabAudioSource = gameObject.AddComponent<AudioSource>();
                grabAudioSource.playOnAwake = false;
                grabAudioSource.spatialBlend = 0f; 
            }
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
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            if (scrollInput != 0)
            {
                currentHoldDistance += scrollInput * holdDistanceScrollSpeed;
                currentHoldDistance = Mathf.Clamp(currentHoldDistance, minHoldDistance, maxHoldDistance);
                Debug.Log("Distance de maintien ajustée à: " + currentHoldDistance);
            }
            
            Vector3 targetPosition = playerCamera.transform.position + playerCamera.transform.forward * currentHoldDistance;
            
            heldRigidbody.linearVelocity = (targetPosition - heldObject.transform.position) * 10f;
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

                originalLayer = heldObject.layer;

                heldObject.layer = LayerMask.NameToLayer("Carried"); 
                
                heldRigidbody.useGravity = false;
                heldRigidbody.linearDamping = 10;
                heldRigidbody.angularDamping = 10;
                heldRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
                
                PlaySound(grabSound);
                
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
            heldObject.layer = originalLayer;
            heldRigidbody.useGravity = true;
            heldRigidbody.linearDamping = 1;
            heldRigidbody.angularDamping = 0.05f;
            
            PlaySound(dropSound);
            
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
            heldObject.layer = originalLayer;
            heldRigidbody.useGravity = true;
            heldRigidbody.linearDamping = 1;
            heldRigidbody.angularDamping = 0.05f;
            
            heldRigidbody.AddForce(playerCamera.transform.forward * throwForce, ForceMode.Impulse);
            
            PlaySound(throwSound);
            
            heldObject = null;
            heldRigidbody = null;
            isHolding = false;
        }
    }
    
    private void PlaySound(AudioClip sound)
    {
        if (grabAudioSource != null && sound != null)
        {
            grabAudioSource.clip = sound;
            grabAudioSource.Play();
        }
    }

    public GameObject GetHeldObject()
    {
        return heldObject;
    }
}