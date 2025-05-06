using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Teleporter : MonoBehaviour
{
    public Transform destinationPoint; 
    public Transform playerTransform;  

    public float teleportDelay = 10f; 
    public bool useEffect = true; 
    public GameObject teleportEffect; 
    
    public bool teleportPlayer = true;
    public LayerMask teleportableLayers; 

    [Header("Audio")]
    [SerializeField] private AudioSource teleportAudioSource;
    [SerializeField] private AudioClip teleportStartSound;
    
    private bool canTeleport = true; 
    private HashSet<GameObject> objectsOnPlate = new HashSet<GameObject>(); 

    private void Start()
    {
        if (teleportAudioSource == null)
        {
            teleportAudioSource = GetComponent<AudioSource>();
            if (teleportAudioSource == null)
            {
                teleportAudioSource = gameObject.AddComponent<AudioSource>();
                teleportAudioSource.playOnAwake = false;
                teleportAudioSource.spatialBlend = 1.0f; 
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canTeleport || destinationPoint == null || objectsOnPlate.Contains(other.gameObject))
        {
            return;
        }

        bool isTeleportableObject = ((1 << other.gameObject.layer) & teleportableLayers) != 0;
        bool isPlayer = other.CompareTag("Player");
        if ((isPlayer && teleportPlayer) || isTeleportableObject)
        {   
            objectsOnPlate.Add(other.gameObject);
            canTeleport = false;
            StartCoroutine(TeleportObject(other.gameObject, isPlayer));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (objectsOnPlate.Contains(other.gameObject))
        {
            objectsOnPlate.Remove(other.gameObject);
        }
    }

    private IEnumerator TeleportObject(GameObject obj, bool isPlayer)
    {
        PlayTeleportSound();
        
        if (teleportDelay > 0)
            yield return new WaitForSeconds(teleportDelay);

        if (useEffect && teleportEffect != null)
        {
            Instantiate(teleportEffect, obj.transform.position, Quaternion.identity);
        }

        Vector3 targetPosition = destinationPoint.position;
        Quaternion targetRotation = destinationPoint.rotation;

        if (isPlayer)
        {
            CharacterController controller = obj.GetComponent<CharacterController>();
            if (controller != null)
            {
                controller.enabled = false;  
                playerTransform.position = targetPosition;
                playerTransform.rotation = targetRotation;

                RaycastHit hit;
                if (Physics.Raycast(obj.transform.position + Vector3.up * 10, Vector3.down, out hit))
                {
                    obj.transform.position = hit.point;  
                }

                controller.enabled = true;   
            }
            else
            {
                playerTransform.position = destinationPoint.position;
                playerTransform.rotation = destinationPoint.rotation;
            }

            GameObject[] carriedObjects = GameObject.FindGameObjectsWithTag("Block");
            foreach (GameObject carriedObject in carriedObjects)
            {
                if (carriedObject.layer == LayerMask.NameToLayer("Carried"))
                {
                    carriedObject.transform.position = targetPosition + new Vector3(0, 1f, 0); 
                }
            }   
        }
        else
        {
            if (obj.layer != LayerMask.NameToLayer("Carried"))
            {
                Rigidbody rb = obj.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    obj.transform.position = targetPosition;
                }
                else
                {
                    obj.transform.position = targetPosition;
                }
            }
        }

        if (useEffect && teleportEffect != null)
        {
            Instantiate(teleportEffect, obj.transform.position, Quaternion.identity);
        }
    

        yield return new WaitForSeconds(0.5f);
        canTeleport = true;
    }
    
    private void PlayTeleportSound()
    {
        if (teleportAudioSource != null && teleportStartSound != null)
        {
            teleportAudioSource.clip = teleportStartSound;
            teleportAudioSource.Play();
        }
    }
}
